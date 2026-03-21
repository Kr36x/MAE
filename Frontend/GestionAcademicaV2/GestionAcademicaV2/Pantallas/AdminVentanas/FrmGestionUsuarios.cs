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
        private PantallaAdmin pantallaPrincipal;
        public FrmGestionUsuarios(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (!txtCorreo.Text.Contains("@"))
            {
                MessageBox.Show("El correo debe contener el símbolo @");
                return;
            }
        }

        private void cbbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbRol.SelectedIndex == 0)
            {
                lbTituloUsuario.Text = "ADMINISTRADOR";
                lbEspecialidad.Visible = false;
                txtEspecialidad.Visible = false;
                lbPosicion.Visible = true;
                txtPosicion.Visible = true;
                lbUsuario.Visible = true;
                txtUsuario.Visible = true;
                lbContrasena.Visible = true;
                txtContrasena.Visible = true;
                lbCorreo.Visible = true;
                txtCorreo.Visible = true;
                lbSexo.Visible = true;
                cbbSexo.Visible = true;
                lbDireccion.Visible = true;
                txtDireccion.Visible = true;
                lbTelefono.Visible = true;
                txtTelefono.Visible = true;
                lbEstado.Visible = true;
                cbbEstado.Visible = true;
                lbFechaNacimiento.Visible = true;
                dtpFechaNacimiento.Visible = true;
                lbParentesco.Visible = false;
                txtParentesco.Visible = false;
                lbLugarTrabajo.Visible = false;
                txtLugarTrabajo.Visible = false;
            }
            else if (cbbRol.SelectedIndex == 1)
            {
                lbTituloUsuario.Text = "DOCENTE";
                lbEspecialidad.Visible = true;
                txtEspecialidad.Visible = true;
                lbPosicion.Visible = false;
                txtPosicion.Visible = false;
                lbUsuario.Visible = true;
                txtUsuario.Visible = true;
                lbContrasena.Visible = true;
                txtContrasena.Visible = true;
                lbCorreo.Visible = true;
                txtCorreo.Visible = true;
                lbSexo.Visible = true;
                cbbSexo.Visible = true;
                lbDireccion.Visible = true;
                txtDireccion.Visible = true;
                lbTelefono.Visible = true;
                txtTelefono.Visible = true;
                lbEstado.Visible = true;
                cbbEstado.Visible = true;
                lbFechaNacimiento.Visible = true;
                dtpFechaNacimiento.Visible = true;
                lbParentesco.Visible = false;
                txtParentesco.Visible = false;
                lbLugarTrabajo.Visible = false;
                txtLugarTrabajo.Visible = false;
            }
            else if (cbbRol.SelectedIndex == 2)
            {
                lbTituloUsuario.Text = "TUTOR";
                lbUsuario.Visible = true;
                txtUsuario.Visible = true;
                lbContrasena.Visible = true;
                txtContrasena.Visible = true;
                lbCorreo.Visible = true;
                txtCorreo.Visible = true;
                lbParentesco.Visible = true;
                txtParentesco.Visible = true;
                lbLugarTrabajo.Visible = true;
                txtLugarTrabajo.Visible = true;
                lbTelefono.Visible = true;
                txtTelefono.Visible = true;
                lbEstado.Visible = true;
                cbbEstado.Visible = true;
                lbEspecialidad.Visible = false;
                txtEspecialidad.Visible = false;
                lbPosicion.Visible = false;
                txtPosicion.Visible = false;
                lbSexo.Visible = false;
                cbbSexo.Visible = false;
                lbDireccion.Visible = false;
                txtDireccion.Visible = false;
                lbFechaNacimiento.Visible = false;
                dtpFechaNacimiento.Visible = false;
            }
        }

        private void lbParentesco_Click(object sender, EventArgs e)
        {

        }

        private void txtIdentidad_TextChanged(object sender, EventArgs e)
        {

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
            {
                return;
            }

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                return;
            }

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                return;
            }

            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        private void btActualizar_Click(object sender, EventArgs e)
        {
            if (!txtCorreo.Text.Contains("@"))
            {
                MessageBox.Show("El correo debe contener el símbolo @");
                return;
            }
        }

        private void btLimpiar_Click(object sender, EventArgs e)
        {
            txtNombre.Clear();
            txtIdentidad.Clear();
            txtUsuario.Clear();
            cbbRol.Text = "";
            txtContrasena.Clear();
            txtCorreo.Clear();
            txtParentesco.Clear();
            txtLugarTrabajo.Clear();
            txtTelefono.Clear();
            cbbEstado.Text = "";
            txtEspecialidad.Clear();
            txtPosicion.Clear();
            cbbSexo.Text = "";
            txtDireccion.Clear();
            dtpFechaNacimiento.Value = DateTime.Now;
        }

        private void lbTituloUsuario_Click(object sender, EventArgs e)
        {

        }
    }
}

