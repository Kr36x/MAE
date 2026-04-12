using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmGestionVinculacionTutoresEditarTutor : Form
    {
        private readonly Conexion conexion = new Conexion();

        private readonly int _usuarioId;
        private readonly int _tutorId;

        private bool _formateandoTelefono = false;
        private string _correoActual = "";
        private string _passwordActual = "";
        public FrmGestionVinculacionTutoresEditarTutor(int usuarioId, int tutorId)
        {
            InitializeComponent();

            _usuarioId = usuarioId;
            _tutorId = tutorId;

            Load += FrmEditarTutor_Load;
            btnCancelar.Click += btnCancelar_Click;
            btnEditar.Click += btnEditar_Click;

            txtTelefono.KeyPress += txtTelefono_KeyPress;
            txtTelefono.TextChanged += txtTelefono_TextChanged;


        }

        private void FrmEditarTutor_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarFormulario();
                ConfigurarControlesEdicion();
                CargarParentescos();
                CargarCredencialesUsuario();
                CargarDatosTutor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
        private void CargarCredencialesUsuario()
        {
            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
            SELECT Correo, Password
            FROM Usuario
            WHERE UsuarioID = @usuarioID", cn);

                cmd.Parameters.AddWithValue("@usuarioID", _usuarioId);

                cn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    _correoActual = dr["Correo"]?.ToString() ?? "";
                    _passwordActual = dr["Password"]?.ToString() ?? "";
                }
                else
                {
                    throw new Exception("No se encontró el usuario asociado al tutor.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudieron cargar las credenciales actuales del usuario. " + ex.Message);
            }
        }
        #region CONFIGURACION

        private void ConfigurarFormulario()
        {
            cbParentesco.DropDownStyle = ComboBoxStyle.DropDownList;

            txtNombre.MaxLength = 100;
            txtDNI.MaxLength = 20;
            txtLugar.MaxLength = 150;

            // 8 dígitos + 1 guion visual => 9 chars máximos visibles
            txtTelefono.MaxLength = 9;

            txtNombre.PlaceholderText = "";
            txtLugar.PlaceholderText = "";
        }

        private void ConfigurarControlesEdicion()
        {
            // BLOQUEADOS porque el SP no los edita para Tutor
            txtNombre.ReadOnly = true;
            txtDNI.ReadOnly = true;
            cbParentesco.Enabled = false;

            txtNombre.TabStop = false;
            txtDNI.TabStop = false;

            txtNombre.FillColor = Color.FromArgb(245, 245, 245);
            txtDNI.FillColor = Color.FromArgb(245, 245, 245);
            cbParentesco.FillColor = Color.FromArgb(245, 245, 245);

            // EDITABLES
            txtLugar.ReadOnly = false;
            txtTelefono.ReadOnly = false;
        }

        private void CargarParentescos()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Valor", typeof(string));
            dt.Columns.Add("Texto", typeof(string));

            dt.Rows.Add("", "--SELECCIONE--");
            dt.Rows.Add("PADRE", "PADRE");
            dt.Rows.Add("MADRE", "MADRE");
            dt.Rows.Add("ABUELO(A)", "ABUELO(A)");
            dt.Rows.Add("TÍA", "TÍA");
            dt.Rows.Add("TÍO", "TÍO");
            dt.Rows.Add("TUTOR LEGAL", "TUTOR LEGAL");
            dt.Rows.Add("ENCARGADO LEGAL", "ENCARGADO LEGAL");
            dt.Rows.Add("OTRO", "OTRO");

            cbParentesco.DataSource = dt;
            cbParentesco.ValueMember = "Valor";
            cbParentesco.DisplayMember = "Texto";
            cbParentesco.SelectedIndex = 0;
        }

        #endregion

        #region CARGA DE DATOS

        private void CargarDatosTutor()
        {
            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        TutorID,
                        UsuarioID,
                        Nombre,
                        Identidad,
                        Telefono,
                        Parentesco,
                        LugarTrabajo
                    FROM Tutor
                    WHERE UsuarioID = @usuarioID", cn);

                cmd.Parameters.AddWithValue("@usuarioID", _usuarioId);

                cn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                {
                    MessageBox.Show("No se encontró la información del tutor.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.Cancel;
                    Close();
                    return;
                }

                txtNombre.Text = dr["Nombre"]?.ToString() ?? "";
                txtDNI.Text = dr["Identidad"]?.ToString() ?? "";
                txtLugar.Text = dr["LugarTrabajo"]?.ToString() ?? "";

                string parentesco = dr["Parentesco"]?.ToString() ?? "";
                cbParentesco.SelectedValue = parentesco;

                string telefonoBD = dr["Telefono"]?.ToString() ?? "";
                txtTelefono.Text = FormatearTelefono(ObtenerSoloDigitos(telefonoBD));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos del tutor: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        #endregion

        #region TELEFONO

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            string soloDigitos = ObtenerSoloDigitos(txtTelefono.Text);

            if (soloDigitos.Length >= 8)
                e.Handled = true;
        }

        private void txtTelefono_TextChanged(object sender, EventArgs e)
        {
            if (_formateandoTelefono)
                return;

            _formateandoTelefono = true;

            string digitos = ObtenerSoloDigitos(txtTelefono.Text);

            if (digitos.Length > 8)
                digitos = digitos.Substring(0, 8);

            txtTelefono.Text = FormatearTelefono(digitos);
            txtTelefono.SelectionStart = txtTelefono.Text.Length;

            _formateandoTelefono = false;
        }

        private string ObtenerSoloDigitos(string texto)
        {
            return new string((texto ?? "").Where(char.IsDigit).ToArray());
        }

        private string FormatearTelefono(string digitos)
        {
            if (string.IsNullOrWhiteSpace(digitos))
                return "";

            if (digitos.Length <= 4)
                return digitos;

            return digitos.Substring(0, 4) + "-" + digitos.Substring(4);
        }

        #endregion

        #region VALIDACIONES

        private bool ValidarFormulario()
        {
            string telefonoDigitos = ObtenerSoloDigitos(txtTelefono.Text);

            if (string.IsNullOrWhiteSpace(txtLugar.Text))
            {
                MessageBox.Show("Debe ingresar el lugar de trabajo.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLugar.Focus();
                return false;
            }

            if (telefonoDigitos.Length != 8)
            {
                MessageBox.Show("El teléfono debe contener exactamente 8 dígitos.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                return false;
            }

            return true;
        }

        #endregion

        #region GUARDAR
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario())
                return;

            DialogResult r = MessageBox.Show(
                "¿Desea guardar los cambios del tutor?",
                "Confirmación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_Crear_EditarUsuario", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Mantener valores actuales del usuario
                cmd.Parameters.AddWithValue("@usuario", "TEMP_TUTOR"); // el SP lo exige aunque en edición no lo use
                cmd.Parameters.AddWithValue("@correo", _correoActual);
                cmd.Parameters.AddWithValue("@password", _passwordActual);
                cmd.Parameters.AddWithValue("@rol", "Tutor");

                // Se envían aunque el SP actual de Tutor sólo use telefono y lugartrabajo
                cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@identidad", txtDNI.Text.Trim());

                string telefonoFormateado = FormatearTelefono(ObtenerSoloDigitos(txtTelefono.Text));
                cmd.Parameters.AddWithValue("@telefono", telefonoFormateado);

                cmd.Parameters.AddWithValue("@sexoAD", DBNull.Value);
                cmd.Parameters.AddWithValue("@direccionAD", DBNull.Value);
                cmd.Parameters.AddWithValue("@posicionA", DBNull.Value);
                cmd.Parameters.AddWithValue("@fechaNacimientoD", DBNull.Value);
                cmd.Parameters.AddWithValue("@especialidadD", DBNull.Value);

                cmd.Parameters.AddWithValue("@parentescoT", cbParentesco.SelectedValue?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@lugartrabajoT", txtLugar.Text.Trim());

                cmd.Parameters.AddWithValue("@usuarioID", _usuarioId);

                cn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Tutor editado correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL al editar tutor: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar tutor: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}