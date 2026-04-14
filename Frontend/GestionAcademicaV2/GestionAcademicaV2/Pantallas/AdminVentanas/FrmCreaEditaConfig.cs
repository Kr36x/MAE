using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public enum ModoOperacion
    {
        Crear,
        Editar
    }

    public partial class FrmCreaEditaConfig : Form
    {
        private ModoOperacion modo;
        private int idConfigSeleccionado;

        public FrmCreaEditaConfig(ModoOperacion modoOperacion, int idConfig = 0)
        {
            InitializeComponent();
            CargarParciales();

            modo = modoOperacion;
            idConfigSeleccionado = idConfig;

            if (modo == ModoOperacion.Editar)
            {
                CargarDatosParaEditar(idConfigSeleccionado);
            }

            txtCicloEscolar.KeyPress += txtCicloEscolar_KeyPress;
            txtCicloEscolar.TextChanged += txtCicloEscolar_TextChanged;
        }

        private void CargarDatosParaEditar(int idConfig)
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                string consulta = $"SELECT CicloEscolar, Periodo, FechaInicio, FechaFin FROM Configuracion WHERE ConfigID = {idConfig}";
                DataTable dt = util.EjecutarConsulta(consulta);

                if (dt.Rows.Count > 0)
                {
                    txtCicloEscolar.Text = dt.Rows[0]["CicloEscolar"].ToString();
                    cbbPeriodo.SelectedItem = dt.Rows[0]["Periodo"].ToString();
                    dtpFechaInicio.Value = Convert.ToDateTime(dt.Rows[0]["FechaInicio"]);
                    dtpFechaFin.Value = Convert.ToDateTime(dt.Rows[0]["FechaFin"]);

                    // Si en editar no quieres que cambien estos campos:
                    // txtCicloEscolar.Enabled = false;
                    // cbbPeriodo.Enabled = false;
                }
                else
                {
                    MessageBox.Show("No se encontraron datos para editar.");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void CargarParciales()
        {
            cbbPeriodo.Items.Clear();
            cbbPeriodo.Items.Add("1");
            cbbPeriodo.Items.Add("2");
            cbbPeriodo.Items.Add("3");
            cbbPeriodo.Items.Add("4");
            cbbPeriodo.SelectedIndex = 0;
        }

        private bool ValidarCicloEscolar()
        {
            string ciclo = txtCicloEscolar.Text.Trim();

            if (!Regex.IsMatch(ciclo, @"^\d{4}-\d{4}$"))
            {
                MessageBox.Show("El ciclo escolar debe tener formato AAAA-AAAA. Ejemplo: 2027-2028",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCicloEscolar.Focus();
                return false;
            }

            string[] partes = ciclo.Split('-');
            int anio1 = int.Parse(partes[0]);
            int anio2 = int.Parse(partes[1]);

            if (anio2 != anio1 + 1)
            {
                MessageBox.Show("El segundo año debe ser consecutivo al primero.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCicloEscolar.Focus();
                return false;
            }

            return true;
        }

        private bool ValidarFechas()
        {
            string ciclo = txtCicloEscolar.Text.Trim();

            if (!Regex.IsMatch(ciclo, @"^\d{4}-\d{4}$"))
                return false;

            string[] partes = ciclo.Split('-');
            int anioInicioCiclo = int.Parse(partes[0]);
            int anioFinCiclo = int.Parse(partes[1]);

            DateTime inicio = dtpFechaInicio.Value.Date;
            DateTime fin = dtpFechaFin.Value.Date;

            DateTime fechaMinima = new DateTime(anioInicioCiclo, 1, 1);
            DateTime fechaMaxima = new DateTime(anioFinCiclo, 12, 31);

            if (inicio < fechaMinima)
            {
                MessageBox.Show(
                    $"La fecha de inicio no puede ser menor al 01/01/{anioInicioCiclo} para el ciclo {ciclo}.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaInicio.Focus();
                return false;
            }

            if (inicio > fechaMaxima)
            {
                MessageBox.Show(
                    $"La fecha de inicio no puede ser mayor al 31/12/{anioFinCiclo} para el ciclo {ciclo}.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaInicio.Focus();
                return false;
            }

            if (fin < fechaMinima)
            {
                MessageBox.Show(
                    $"La fecha fin no puede ser menor al 01/01/{anioInicioCiclo} para el ciclo {ciclo}.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaFin.Focus();
                return false;
            }

            if (fin > fechaMaxima)
            {
                MessageBox.Show(
                    $"La fecha fin no puede ser mayor al 31/12/{anioFinCiclo} para el ciclo {ciclo}.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaFin.Focus();
                return false;
            }

            if (fin <= inicio)
            {
                MessageBox.Show("La fecha fin debe ser mayor que la fecha inicio.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaFin.Focus();
                return false;
            }

            int diasDiferencia = (fin - inicio).Days;

            if (diasDiferencia < 75)
            {
                MessageBox.Show("El período no puede ser tan corto. Debe tener al menos 75 días.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaFin.Focus();
                return false;
            }

            if (diasDiferencia > 140)
            {
                MessageBox.Show("El período es demasiado largo para un parcial. Debe ser de 140 días o menos.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaFin.Focus();
                return false;
            }

            return true;
        }

        private int ObtenerUltimoParcial(string ciclo)
        {
            EjecutarUtilidades util = new EjecutarUtilidades();

            try
            {
                string consulta = $"SELECT ISNULL(MAX(Periodo),0) FROM Configuracion WHERE CicloEscolar = '{ciclo}'";
                DataTable dt = util.EjecutarConsulta(consulta);
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al consultar los periodos: " + ex.Message);
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
                return 0;
            }
        }

        private void LimpiarCampos()
        {
            txtCicloEscolar.Clear();
            cbbPeriodo.SelectedIndex = 0;
            dtpFechaInicio.Value = DateTime.Now;
            dtpFechaFin.Value = DateTime.Now;
        }

        private void btnAperturarCicloEscolar_Click(object sender, EventArgs e)
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                string ciclo = txtCicloEscolar.Text.Trim();
                int periodo = int.Parse(cbbPeriodo.SelectedItem.ToString());

                if (!ValidarCicloEscolar() || !ValidarFechas())
                    return;

                SqlParameter[] parametros = new SqlParameter[]
                {
                    new SqlParameter("@CicloEscolar", ciclo),
                    new SqlParameter("@Periodo", periodo),
                    new SqlParameter("@FechaInicio", dtpFechaInicio.Value.Date),
                    new SqlParameter("@FechaFin", dtpFechaFin.Value.Date)
                };

                if (modo == ModoOperacion.Crear)
                {
                    util.EjecutarSPParametros("spMAE_CrearConfiguracion", parametros);
                    MessageBox.Show($"Periodo {periodo} del ciclo {ciclo} creado correctamente!",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DialogResult = DialogResult.OK;
                    Close();
                }
                else if (modo == ModoOperacion.Editar)
                {
                    SqlParameter[] parametrosEditar = new SqlParameter[]
                    {
                        new SqlParameter("@ConfigID", idConfigSeleccionado),
                        new SqlParameter("@FechaInicio", dtpFechaInicio.Value.Date),
                        new SqlParameter("@FechaFin", dtpFechaFin.Value.Date)
                    };

                    util.EjecutarSPParametros("spMAE_EditarConfiguracion", parametrosEditar);
                    MessageBox.Show($"Periodo {periodo} del ciclo {ciclo} actualizado correctamente!",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los datos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtCicloEscolar_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo permitir números y teclas de control como Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // Máximo 9 caracteres: AAAA-AAAA
            if (!char.IsControl(e.KeyChar) && txtCicloEscolar.Text.Length >= 9)
            {
                e.Handled = true;
            }
        }

        private void txtCicloEscolar_TextChanged(object sender, EventArgs e)
        {
            int cursor = txtCicloEscolar.SelectionStart;
            string texto = txtCicloEscolar.Text.Replace("-", "");

            // Solo dejar números
            texto = new string(texto.Where(char.IsDigit).ToArray());

            // Limitar a 8 dígitos
            if (texto.Length > 8)
                texto = texto.Substring(0, 8);

            // Insertar guion automáticamente después de 4 dígitos
            if (texto.Length > 4)
                texto = texto.Insert(4, "-");

            if (txtCicloEscolar.Text != texto)
            {
                txtCicloEscolar.Text = texto;

                // Ajustar posición del cursor
                if (cursor == 5 && !txtCicloEscolar.Text.EndsWith("-"))
                    cursor++;

                txtCicloEscolar.SelectionStart = Math.Min(cursor, txtCicloEscolar.Text.Length);
            }
        }
        private void btnCancelarApertura_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
                "¿Está seguro que desea cancelar y cerrar el formulario?",
                "Confirmar Cancelación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                LimpiarCampos();
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
    }
}