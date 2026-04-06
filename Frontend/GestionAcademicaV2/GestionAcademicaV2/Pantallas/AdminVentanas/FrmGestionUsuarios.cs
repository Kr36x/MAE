using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmGestionUsuarios : Form
    {
        private int usuarioID;

        public FrmGestionUsuarios()
        {
            InitializeComponent();
            usuarioID = 0;
        }
        public FrmGestionUsuarios(int id)
        {
            InitializeComponent();
            usuarioID = id;
            CargarDatosUsuario();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (!txtCorreo.Text.Contains("@"))
            {
                MessageBox.Show("El correo debe contener el símbolo @");
                return;
            }
            else
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                SqlParameter[] p =
                {
                    new SqlParameter("@usuario", txtUsuario.Text),
                    new SqlParameter("@correo", txtCorreo.Text),
                    new SqlParameter("@password", txtContrasena.Text),
                    new SqlParameter("@rol", cbbRol.Text),
                    new SqlParameter("@nombre", txtNombre.Text),
                    new SqlParameter("@identidad", txtIdentidad.Text),
                    new SqlParameter("@telefono", txtTelefono.Text),

                    // Admin / Docente
                    new SqlParameter("@sexoAD", cbbSexo.Visible ? (object)cbbSexo.Text.Substring(0,1) : DBNull.Value),
                    new SqlParameter("@direccionAD", txtDireccion.Visible ? (object)txtDireccion.Text : DBNull.Value),

                    // Admin
                    new SqlParameter("@posicionA", txtPosicion.Visible ? (object)txtPosicion.Text : DBNull.Value),

                    // Docente
                    new SqlParameter("@fechaNacimientoD", dtpFechaNacimiento.Visible ? (object)dtpFechaNacimiento.Value : DBNull.Value),
                    new SqlParameter("@especialidadD", txtEspecialidad.Visible ? (object)txtEspecialidad.Text : DBNull.Value),

                    // Tutor
                    new SqlParameter("@parentescoT", cbbParentesco.Visible ? (object)cbbParentesco.Text : DBNull.Value),
                    new SqlParameter("@lugartrabajoT", txtLugarTrabajo.Visible ? (object)txtLugarTrabajo.Text : DBNull.Value),

                    // CREAR o EDITAR según el constructor
                    new SqlParameter("@usuarioID", usuarioID == 0 ? (object)DBNull.Value : usuarioID)
                };

                DataTable dt = util.EjecutarSPParametros("spMAE_Crear_EditarUsuario", p);

                MessageBox.Show("Datos guardados correctamente.");
            }
        }

        private void CargarDatosUsuario()
        {
            EjecutarUtilidades util = new EjecutarUtilidades();

            SqlParameter[] p =
            {
                new SqlParameter("@usuarioID", usuarioID)
            };

            DataTable dt = util.EjecutarSPParametros("spMAE_TraerUsuarios", p);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron datos del usuario.");
                return;
            }

            DataRow row = dt.Rows[0];

            txtUsuario.Text = row["Usuario"].ToString();
            txtCorreo.Text = row["Correo"].ToString();
            cbbRol.Text = row["Rol"].ToString();
            txtContrasena.Text = row["Password"].ToString();

            string rol = cbbRol.Text.ToString();

            if (rol == "ADMINISTRADOR")
            {
                ctnDatosUsuario.Visible = true;
                lbUsuario.Visible = true;
                lbAvisoUsuario.Visible = true;
                txtUsuario.Visible = true;
                lbContrasena.Visible = true;
                lbAvisoContrasena.Visible = true;
                txtContrasena.Visible = true;
                lbCorreo.Visible = true;
                lbAvisoCorreo.Visible = true;
                txtCorreo.Visible = true;
                lbParentesco.Visible = false;
                lbAvisoParentesco.Visible = false;
                cbbParentesco.Visible = false;
                lbLugarTrabajo.Visible = false;
                lbAvisoTrabajo.Visible = false;
                txtLugarTrabajo.Visible = false;
                lbTelefono.Visible = true;
                lbAvisoTelefono.Visible = true;
                txtTelefono.Visible = true;
                lbEspecialidad.Visible = false;
                lbAvisoEspecialidad.Visible = false;
                txtEspecialidad.Visible = false;
                lbPosicion.Visible = true;
                lbAvisoPosicion.Visible = true;
                txtPosicion.Visible = true;
                lbSexo.Visible = true;
                lbAvisoGenero.Visible = true;
                cbbSexo.Visible = true;
                lbDireccion.Visible = true;
                lbAvisoDireccion.Visible = true;
                txtDireccion.Visible = true;
                lbFechaNacimiento.Visible = false;
                lbAvisoFecha.Visible = false;
                dtpFechaNacimiento.Visible = false;
                // Datos
                txtNombre.Text = row["Nombre"].ToString();
                txtIdentidad.Text = row["Identidad"].ToString();
                txtTelefono.Text = row["Telefono"].ToString();
                txtDireccion.Text = row["Direccion"].ToString();
                txtPosicion.Text = row["Posicion"].ToString();
                string sexo = row["Sexo"].ToString();
                if (sexo == "M")
                {
                    cbbSexo.Text = "MASCULINO";
                }
                else if (sexo == "F")
                {
                    cbbSexo.Text = "FEMENINO";
                }
                else
                {
                    cbbSexo.Text = "";
                }
                txtUsuario.Enabled = false;
                cbbRol.Enabled = false;
                txtNombre.Enabled = false;
                txtIdentidad.Enabled = false;
                cbbSexo.Enabled = false;
            }
            else if (rol == "DOCENTE")
            {
                ctnDatosUsuario.Visible = true;
                lbUsuario.Visible = true;
                lbAvisoUsuario.Visible = true;
                txtUsuario.Visible = true;
                lbContrasena.Visible = true;
                lbAvisoContrasena.Visible = true;
                txtContrasena.Visible = true;
                lbCorreo.Visible = true;
                lbAvisoCorreo.Visible = true;
                txtCorreo.Visible = true;
                lbParentesco.Visible = false;
                lbAvisoParentesco.Visible = false;
                cbbParentesco.Visible = false;
                lbLugarTrabajo.Visible = false;
                lbAvisoTrabajo.Visible = false;
                txtLugarTrabajo.Visible = false;
                lbTelefono.Visible = true;
                lbAvisoTelefono.Visible = true;
                txtTelefono.Visible = true;
                lbEspecialidad.Visible = true;
                lbAvisoEspecialidad.Visible = true;
                txtEspecialidad.Visible = true;
                lbPosicion.Visible = false;
                lbAvisoPosicion.Visible = false;
                txtPosicion.Visible = false;
                lbSexo.Visible = true;
                lbAvisoGenero.Visible = true;
                cbbSexo.Visible = true;
                lbDireccion.Visible = true;
                lbAvisoDireccion.Visible = true;
                txtDireccion.Visible = true;
                lbFechaNacimiento.Visible = true;
                lbAvisoFecha.Visible = true;
                dtpFechaNacimiento.Visible = true;
                // Datos
                txtNombre.Text = row["Nombre"].ToString();
                txtIdentidad.Text = row["Identidad"].ToString();
                txtTelefono.Text = row["Telefono"].ToString();
                txtDireccion.Text = row["Direccion"].ToString();
                dtpFechaNacimiento.Value = Convert.ToDateTime(row["FechaNacimiento"]);
                txtEspecialidad.Text = row["Especialidad"].ToString();
                string sexo = row["Sexo"].ToString();
                if (sexo == "M")
                {
                    cbbSexo.Text = "MASCULINO";
                }
                else if (sexo == "F")
                {
                    cbbSexo.Text = "FEMENINO";
                }
                else
                {
                    cbbSexo.Text = "";
                }
                txtUsuario.Enabled = false;
                cbbRol.Enabled = false;
                txtNombre.Enabled = false;
                txtIdentidad.Enabled = false;
                cbbSexo.Enabled = false;
                dtpFechaNacimiento.Enabled = false;
            }
            else if (rol == "TUTOR")
            {
                lbUsuario.Visible = true;
                lbAvisoUsuario.Visible = true;
                txtUsuario.Visible = true;
                lbContrasena.Visible = true;
                lbAvisoContrasena.Visible = true;
                txtContrasena.Visible = true;
                lbCorreo.Visible = true;
                lbAvisoCorreo.Visible = true;
                txtCorreo.Visible = true;
                lbParentesco.Visible = true;
                lbAvisoParentesco.Visible = true;
                cbbParentesco.Visible = true;
                lbLugarTrabajo.Visible = true;
                lbAvisoTrabajo.Visible = true;
                txtLugarTrabajo.Visible = true;
                lbTelefono.Visible = true;
                lbAvisoTelefono.Visible = true;
                txtTelefono.Visible = true;
                lbEspecialidad.Visible = false;
                lbAvisoEspecialidad.Visible = false;
                txtEspecialidad.Visible = false;
                lbPosicion.Visible = false;
                lbAvisoPosicion.Visible = false;
                txtPosicion.Visible = false;
                lbSexo.Visible = false;
                lbAvisoGenero.Visible = false;
                cbbSexo.Visible = false;
                lbDireccion.Visible = false;
                lbAvisoDireccion.Visible = false;
                txtDireccion.Visible = false;
                lbFechaNacimiento.Visible = false;
                lbAvisoFecha.Visible = false;
                dtpFechaNacimiento.Visible = false;
                // Datos
                txtNombre.Text = row["Nombre"].ToString();
                txtIdentidad.Text = row["Identidad"].ToString();
                txtTelefono.Text = row["Telefono"].ToString();
                cbbParentesco.Text = row["Parentesco"].ToString();
                txtLugarTrabajo.Text = row["LugarTrabajo"].ToString();

                txtUsuario.Enabled = false;
                cbbRol.Enabled = false;
                txtNombre.Enabled = false;
                txtIdentidad.Enabled = false;
                cbbParentesco.Enabled = false;
            }
        }

        private void cbbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            ctnDatosUsuario.Visible = true;
            if (cbbRol.SelectedIndex == 0)
            {
                lbUsuario.Visible = true;
                lbAvisoUsuario.Visible = true;
                txtUsuario.Visible = true;
                lbContrasena.Visible = true;
                lbAvisoContrasena.Visible = true;
                txtContrasena.Visible = true;
                lbCorreo.Visible = true;
                lbAvisoCorreo.Visible = true;
                txtCorreo.Visible = true;
                lbParentesco.Visible = false;
                lbAvisoParentesco.Visible = false;
                cbbParentesco.Visible = false;
                lbLugarTrabajo.Visible = false;
                lbAvisoTrabajo.Visible = false;
                txtLugarTrabajo.Visible = false;
                lbTelefono.Visible = true;
                lbAvisoTelefono.Visible = true;
                txtTelefono.Visible = true;
                lbEspecialidad.Visible = false;
                lbAvisoEspecialidad.Visible = false;
                txtEspecialidad.Visible = false;
                lbPosicion.Visible = true;
                lbAvisoPosicion.Visible = true;
                txtPosicion.Visible = true;
                lbSexo.Visible = true;
                lbAvisoGenero.Visible = true;
                cbbSexo.Visible = true;
                lbDireccion.Visible = true;
                lbAvisoDireccion.Visible = true;
                txtDireccion.Visible = true;
                lbFechaNacimiento.Visible = false;
                lbAvisoFecha.Visible = false;
                dtpFechaNacimiento.Visible = false;
            }
            else if (cbbRol.SelectedIndex == 1)
            {
                lbUsuario.Visible = true;
                lbAvisoUsuario.Visible = true;
                txtUsuario.Visible = true;
                lbContrasena.Visible = true;
                lbAvisoContrasena.Visible = true;
                txtContrasena.Visible = true;
                lbCorreo.Visible = true;
                lbAvisoCorreo.Visible = true;
                txtCorreo.Visible = true;
                lbParentesco.Visible = false;
                lbAvisoParentesco.Visible = false;
                cbbParentesco.Visible = false;
                lbLugarTrabajo.Visible = false;
                lbAvisoTrabajo.Visible = false;
                txtLugarTrabajo.Visible = false;
                lbTelefono.Visible = true;
                lbAvisoTelefono.Visible = true;
                txtTelefono.Visible = true;
                lbEspecialidad.Visible = true;
                lbAvisoEspecialidad.Visible = true;
                txtEspecialidad.Visible = true;
                lbPosicion.Visible = false;
                lbAvisoPosicion.Visible = false;
                txtPosicion.Visible = false;
                lbSexo.Visible = true;
                lbAvisoGenero.Visible = true;
                cbbSexo.Visible = true;
                lbDireccion.Visible = true;
                lbAvisoDireccion.Visible = true;
                txtDireccion.Visible = true;
                lbFechaNacimiento.Visible = true;
                lbAvisoFecha.Visible = true;
                dtpFechaNacimiento.Visible = true;
            }
            else if (cbbRol.SelectedIndex == 2)
            {
                lbUsuario.Visible = true;
                lbAvisoUsuario.Visible = true;
                txtUsuario.Visible = true;
                lbContrasena.Visible = true;
                lbAvisoContrasena.Visible = true;
                txtContrasena.Visible = true;
                lbCorreo.Visible = true;
                lbAvisoCorreo.Visible = true;
                txtCorreo.Visible = true;
                lbParentesco.Visible = true;
                lbAvisoParentesco.Visible = true;
                cbbParentesco.Visible = true;
                lbLugarTrabajo.Visible = true;
                lbAvisoTrabajo.Visible = true;
                txtLugarTrabajo.Visible = true;
                lbTelefono.Visible = true;
                lbAvisoTelefono.Visible = true;
                txtTelefono.Visible = true;
                lbEspecialidad.Visible = false;
                lbAvisoEspecialidad.Visible = false;
                txtEspecialidad.Visible = false;
                lbPosicion.Visible = false;
                lbAvisoPosicion.Visible = false;
                txtPosicion.Visible = false;
                lbSexo.Visible = false;
                lbAvisoGenero.Visible = false;
                cbbSexo.Visible = false;
                lbDireccion.Visible = false;
                lbAvisoDireccion.Visible = false;
                txtDireccion.Visible = false;
                lbFechaNacimiento.Visible = false;
                lbAvisoFecha.Visible = false;
                dtpFechaNacimiento.Visible = false;
            }
        }

        private void lbParentesco_Click(object sender, EventArgs e)
        {

        }

        private void txtIdentidad_TextChanged(object sender, EventArgs e)
        {
            int cursor = txtIdentidad.SelectionStart;

            string limpio = new string(txtIdentidad.Text.Where(char.IsDigit).ToArray());

            if (limpio.Length > 13)
                limpio = limpio.Substring(0, 13);

            string formateado = limpio;

            if (limpio.Length > 4)
                formateado = limpio.Insert(4, "-");


            if (limpio.Length > 8)
                formateado = formateado.Insert(9, "-");

            int diff = formateado.Length - txtIdentidad.Text.Length;

            txtIdentidad.TextChanged -= txtIdentidad_TextChanged;
            txtIdentidad.Text = formateado;
            txtIdentidad.TextChanged += txtIdentidad_TextChanged;

            txtIdentidad.SelectionStart = Math.Max(0, cursor + diff);
        }

        private void txtTelefono_TextChanged(object sender, EventArgs e)
        {
            if (txtTelefono.Text.Length == 4 && !txtTelefono.Text.Contains("-"))
            {
                txtTelefono.Text += "-";
                txtTelefono.SelectionStart = txtTelefono.Text.Length;
            }
        }

        private void txtIdentidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Solo se permiten números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Solo se permiten números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;

                MessageBox.Show("Solo se aceptan letras.",
                                "Validación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void btActualizar_Click(object sender, EventArgs e)
        {

        }

        private void btLimpiar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lbTituloUsuario_Click(object sender, EventArgs e)
        {

        }

        private void FrmGestionUsuarios_Load(object sender, EventArgs e)
        {
            if (usuarioID == 0)
            {
                ctnDatosUsuario.Visible = false;
            }
            else
            {
                ctnDatosUsuario.Visible = true;
            }


        }

        private void lbAvisoTrabajo_Click(object sender, EventArgs e)
        {

        }

        private void btVer_Click(object sender, EventArgs e)
        {
            if (txtContrasena.PasswordChar == '\0')
            {
                txtContrasena.PasswordChar = '*';
            }
            else
            {
                txtContrasena.PasswordChar = '\0';
            }

        }

        private void txtEspecialidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;

                MessageBox.Show("Solo se aceptan letras.",
                                "Validación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void btInformacion_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Este formulario sirve para poder crear, actualizar y gestionar los usuarios del sistema.",
                                "Información",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
        }

        private void dtpFechaNacimiento_ValueChanged(object sender, EventArgs e)
        {
            DateTime fecha = dtpFechaNacimiento.Value;
            DateTime hoy = DateTime.Now;

            int edad = hoy.Year - fecha.Year;

            if (fecha.Date > hoy.AddYears(-edad))
                edad--;

            if (edad < 18)
            {
                MessageBox.Show("Debe ser mayor de 18 años.",
                                "Validación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                dtpFechaNacimiento.Value = hoy.AddYears(-18);
            }

        }
    }
}

