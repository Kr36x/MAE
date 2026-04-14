using GestionAcademicaV2.Modelos;
using GestionAcademicaV2.Pantallas.DocenteVentanas;
using GestionAcademicaV2.Pantallas.TutorVentanas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas
{
    public partial class PantallaTutor : Form
    {
        private Form formularioActivo = null;
        private readonly SesionUsuario usuarioActual;
        private readonly Conexion conexion = new Conexion();
        public PantallaTutor(SesionUsuario usuario)
        {
            InitializeComponent();
            usuarioActual = usuario;

        }
        private void AbrirFormularioEnPanel(Form formularioHijo)
        {
            try
            {
                if (formularioActivo != null)
                {
                    formularioActivo.Close();
                    formularioActivo.Dispose();
                }

                formularioActivo = formularioHijo;
                formularioHijo.TopLevel = false;
                formularioHijo.FormBorderStyle = FormBorderStyle.None;
                formularioHijo.Dock = DockStyle.Fill;

                pnlInfo2.Controls.Clear();
                pnlInfo2.Controls.Add(formularioHijo);
                pnlInfo2.Tag = formularioHijo;

                formularioHijo.Show();
                formularioHijo.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al abrir la ventana.\n\n" + ex.Message,
                    "Sistema MAE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void btnInicio_Click(object sender, EventArgs e)
        {

        }

        private void PantallaTutor_Load(object sender, EventArgs e)
        {
            CargarDatosUsuario();
            AbrirFormularioEnPanel(new FrmInicioTutor(usuarioActual.UsuarioID));
        }
        private void CargarDatosUsuario()
        {
            lblUsuario.Text = FormatearNombre(usuarioActual?.Usuario ?? "Usuario");
            lblRol.Text = usuarioActual?.Rol ?? "Tutor";
            lblId.Text = "ID: " + (usuarioActual?.UsuarioID.ToString() ?? "N/A");
        }

        private string FormatearNombre(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return string.Empty;

            string[] partes = usuario.Replace(".", " ")
                                     .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < partes.Length; i++)
            {
                if (partes[i].Length > 0)
                    partes[i] = char.ToUpper(partes[i][0]) + partes[i][1..].ToLower();
            }

            return string.Join(" ", partes);
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "¿Deseas cerrar sesión?",
                "Cerrar sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                // Buscar el login abierto
                foreach (Form form in Application.OpenForms)
                {
                    if (form is Login login)
                    {
                        login.LimpiarCampos();
                        login.Show(); // volver a mostrar login
                        break;
                    }
                }

                this.Close(); // cerrar pantalla docente
            }
        }
    }
}
