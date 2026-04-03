using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmEditarActividad : Form
    {
        private readonly int _actividadId;
        private readonly int _docenteId;
        private readonly string _descripcionOriginal;
        private readonly decimal _valorOriginal;
        private readonly DataTable _dtActividadesActuales;
        private readonly Conexion conexion = new Conexion();

        private string _tipoOriginal = "";
        private int _numeroOriginal = 0;

        public FrmEditarActividad(
            int actividadId,
            string descripcionActual,
            decimal valorActual,
            int docenteId,
            DataTable dtActividadesActuales)
        {
            InitializeComponent();

            _actividadId = actividadId;
            _descripcionOriginal = descripcionActual ?? "";
            _valorOriginal = valorActual;
            _docenteId = docenteId;
            _dtActividadesActuales = dtActividadesActuales?.Copy() ?? new DataTable();

            Load += FrmEditarActividad_Load;
            btnCancelar.Click += btnCancelar_Click;
            btnCrear.Click += btnCrear_Click;
        }

        private void FrmEditarActividad_Load(object sender, EventArgs e)
        {
            CargarTiposActividad();
            CargarDatosIniciales();

            btnCrear.Text = "GUARDAR";
        }

        private void CargarTiposActividad()
        {
            cbActividad.Items.Clear();
            cbActividad.Items.Add("TAREA");
            cbActividad.Items.Add("PRUEBA");
            cbActividad.Items.Add("EXAMEN");
        }

        private void CargarDatosIniciales()
        {
            _tipoOriginal = DetectarTipoActividad(_descripcionOriginal);
            _numeroOriginal = ExtraerNumero(_descripcionOriginal, _tipoOriginal);

            string detalle = ExtraerDetalleDescripcion(_descripcionOriginal, _tipoOriginal);

            if (!string.IsNullOrWhiteSpace(_tipoOriginal))
                cbActividad.SelectedItem = _tipoOriginal;
            else
                cbActividad.SelectedIndex = -1;

            txtDescripcion.Text = detalle;
            txtValor.Text = _valorOriginal.ToString("N2");
        }

        private string DetectarTipoActividad(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return "";

            string texto = descripcion.Trim().ToUpper();

            if (texto.StartsWith("TAREA")) return "TAREA";
            if (texto.StartsWith("PRUEBA")) return "PRUEBA";
            if (texto.StartsWith("EXAMEN")) return "EXAMEN";

            return "";
        }

        private int ExtraerNumero(string descripcion, string tipo)
        {
            if (string.IsNullOrWhiteSpace(descripcion) || string.IsNullOrWhiteSpace(tipo))
                return 0;

            string patron = $@"^{Regex.Escape(tipo.ToUpper())}\s+(\d+)\b";
            Match match = Regex.Match(descripcion.Trim().ToUpper(), patron);

            if (match.Success && int.TryParse(match.Groups[1].Value, out int numero))
                return numero;

            return 0;
        }

        private string ExtraerDetalleDescripcion(string descripcion, string tipo)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return "";

            string texto = descripcion.Trim();

            if (string.IsNullOrWhiteSpace(tipo))
                return texto;

            // Caso: TAREA 3 - ALGO
            string patronConNumero = $@"^{Regex.Escape(tipo)}\s+\d+\s*-\s*";
            string resultado = Regex.Replace(texto, patronConNumero, "", RegexOptions.IgnoreCase);

            // Caso: EXAMEN - ALGO o PRUEBA - ALGO
            if (resultado == texto)
            {
                string patronSinNumero = $@"^{Regex.Escape(tipo)}\s*-\s*";
                resultado = Regex.Replace(texto, patronSinNumero, "", RegexOptions.IgnoreCase);
            }

            return resultado.Trim();
        }

        private int ObtenerSiguienteCorrelativo(string tipo)
        {
            if (_dtActividadesActuales == null || _dtActividadesActuales.Rows.Count == 0)
                return 1;

            int maximo = 0;

            foreach (DataRow row in _dtActividadesActuales.Rows)
            {
                int actividadIdFila = row["ActividadID"] != DBNull.Value
                    ? Convert.ToInt32(row["ActividadID"])
                    : 0;

                // excluir la actividad actual para no competir consigo misma
                if (actividadIdFila == _actividadId)
                    continue;

                string descripcion = row["Descripcion"]?.ToString()?.Trim().ToUpper() ?? "";

                if (!descripcion.StartsWith(tipo.ToUpper()))
                    continue;

                int numero = ExtraerNumero(descripcion, tipo);

                if (numero > maximo)
                    maximo = numero;
            }

            return maximo + 1;
        }

        private string ConstruirDescripcionFinal()
        {
            if (cbActividad.SelectedItem == null)
                return string.Empty;

            string tipoSeleccionado = cbActividad.SelectedItem.ToString()!.Trim().ToUpper();
            string detalle = txtDescripcion.Text.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(detalle))
                return string.Empty;

            int numeroFinal;

            // Si el tipo no cambió, conservar el número original
            if (tipoSeleccionado == _tipoOriginal && _numeroOriginal > 0)
            {
                numeroFinal = _numeroOriginal;
            }
            else
            {
                numeroFinal = ObtenerSiguienteCorrelativo(tipoSeleccionado);
            }

            return $"{tipoSeleccionado} {numeroFinal} - {detalle}";
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (cbActividad.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione el tipo de actividad.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbActividad.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Ingrese la descripción de la actividad.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcion.Focus();
                return;
            }

            if (!decimal.TryParse(txtValor.Text.Trim(), out decimal valor))
            {
                MessageBox.Show("Ingrese un valor válido.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValor.Focus();
                return;
            }

            string descripcionFinal = ConstruirDescripcionFinal();

            if (string.IsNullOrWhiteSpace(descripcionFinal))
            {
                MessageBox.Show("No se pudo construir la descripción final.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GuardarEdicion(descripcionFinal, valor);
        }

        private void GuardarEdicion(string descripcionFinal, decimal valor)
        {
            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_EditarActividad", cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActividadID", _actividadId);
                cmd.Parameters.AddWithValue("@Descripcion", descripcionFinal);
                cmd.Parameters.AddWithValue("@Valor", valor);
                cmd.Parameters.AddWithValue("@DocenteID", _docenteId);

                cn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Actividad actualizada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar actividad: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}