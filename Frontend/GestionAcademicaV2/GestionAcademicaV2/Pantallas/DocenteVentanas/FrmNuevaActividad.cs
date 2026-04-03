using GestionAcademicaV2.Modelos;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmNuevaActividad : Form
    {
        private readonly int _cargaId;
        private readonly int _parcial;
        private readonly DataTable _dtActividadesActuales;
        private readonly Conexion conexion = new Conexion();
        private string _prefijoActual = "";
        public bool SeCreoActividad { get; private set; } = false;

        public FrmNuevaActividad(int cargaId, int parcial, DataTable dtActividadesActuales)
        {
            InitializeComponent();
            _cargaId = cargaId;
            _parcial = parcial;
            _dtActividadesActuales = dtActividadesActuales?.Copy() ?? new DataTable();
        }
        private void FrmNuevaActividad_Load(object sender, EventArgs e)
        {
            txtDescripcion.Focus();
            CargarTiposActividad();
        }


        private void cbActividad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbActividad.SelectedItem == null)
                return;

            string tipo = cbActividad.SelectedItem.ToString()!;
            _prefijoActual = GenerarPrefijoActividad(tipo);

            txtDescripcion.Text = _prefijoActual;
            txtDescripcion.SelectionStart = txtDescripcion.Text.Length;
            txtDescripcion.Focus();
        }

        private string GenerarPrefijoActividad(string tipo)
        {
            int siguienteNumero = ObtenerSiguienteCorrelativo(tipo);

            return $"{tipo} {siguienteNumero} - ";
        }

        private int ObtenerSiguienteCorrelativo(string tipo)
        {
            if (_dtActividadesActuales == null || _dtActividadesActuales.Rows.Count == 0)
                return 1;

            int maximo = 0;

            foreach (DataRow row in _dtActividadesActuales.Rows)
            {
                string descripcion = row["Descripcion"]?.ToString()?.Trim().ToUpper() ?? "";

                if (!descripcion.StartsWith(tipo.ToUpper()))
                    continue;

                int numero = ExtraerNumeroCorrelativo(descripcion, tipo);

                if (numero > maximo)
                    maximo = numero;
            }

            return maximo + 1;
        }

        private int ExtraerNumeroCorrelativo(string descripcion, string tipo)
        {
            string patron = $@"^{Regex.Escape(tipo.ToUpper())}\s+(\d+)\b";
            Match match = Regex.Match(descripcion.ToUpper(), patron);

            if (match.Success && int.TryParse(match.Groups[1].Value, out int numero))
                return numero;

            return 0;
        }
        private void CargarTiposActividad()
        {
            cbActividad.Items.Clear();
            cbActividad.Items.Add("TAREA");
            cbActividad.Items.Add("PRUEBA");
            cbActividad.Items.Add("EXAMEN");
            cbActividad.SelectedIndex = -1;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
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

            CrearActividad(descripcionFinal, valor);
        }
        private string ConstruirDescripcionFinal()
        {
            if (cbActividad.SelectedItem == null)
                return string.Empty;

            string tipo = cbActividad.SelectedItem.ToString()!.Trim().ToUpper();
            string detalle = txtDescripcion.Text.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(detalle))
                return string.Empty;

            int siguiente = ObtenerSiguienteCorrelativo(tipo);

            return $"{tipo} {siguiente} - {detalle}";
        }
        private void CrearActividad(string descripcionFinal, decimal valor)
        {
            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_CrearActividad", cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CargaID", _cargaId);
                cmd.Parameters.AddWithValue("@Parcial", _parcial);
                cmd.Parameters.AddWithValue("@Descripcion", descripcionFinal);
                cmd.Parameters.AddWithValue("@Valor", valor);

                cn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Actividad creada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear actividad: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}