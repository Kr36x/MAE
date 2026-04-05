using GestionAcademicaV2.Modelos;
using GestionAcademicaV2.Pantallas.AdminVentanas;
using GestionAcademicaV2.Pantallas.DocenteVentanas;
using Guna.UI2.WinForms;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas
{
    public partial class PantallaAdmin : Form
    {
        private Form formularioActivo = null;
        private bool menuExpandido = true;
        private SesionUsuario usuarioActual;

        public PantallaAdmin(SesionUsuario usuario)
        {
            InitializeComponent();
            usuarioActual = usuario;
        }

        public PantallaAdmin()
        {
            InitializeComponent();
        }

        private void AbrirFormularioEnPanel(Form formularioHijo)
        {
            if (formularioActivo != null)
            {
                formularioActivo.Close();
            }

            formularioActivo = formularioHijo;
            formularioHijo.TopLevel = false;
            formularioHijo.FormBorderStyle = FormBorderStyle.None;
            formularioHijo.Dock = DockStyle.Fill;

            PnlContenedorAdmin.Controls.Clear();
            PnlContenedorAdmin.Controls.Add(formularioHijo);
            PnlContenedorAdmin.Tag = formularioHijo;

            formularioHijo.Show();
            formularioHijo.BringToFront();
        }

        private string FormatearNombre(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return "";

            string[] partes = usuario.Replace(".", " ").Split(' ');

            for (int i = 0; i < partes.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(partes[i]))
                {
                    partes[i] = char.ToUpper(partes[i][0]) + partes[i].Substring(1).ToLower();
                }
            }

            return string.Join(" ", partes);
        }

        private void PantallaAdmin_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = FormatearNombre(usuarioActual?.Usuario ?? "Usuario prueba");
            lblRol.Text = usuarioActual?.Rol ?? "Sin rol";
            lblID.Text = "ID: " + (usuarioActual?.UsuarioID.ToString() ?? "N/A");
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FrmDashboard(this));
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FrmGestionUsuarios(this));
        }

        private void btnDocentes_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new CargaDocente(this));
        }

        private void btnEstudiantes_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FrmEstudiantes(this));
        }

        private void btnMatricula_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FrmMatricula(this));
        }

        private void btnAsistencia_Click(object sender, EventArgs e)
        {

        }

        private void btnCalificaciones_Click(object sender, EventArgs e)
        {

        }

        private void btnReportes_Click(object sender, EventArgs e)
        {

        }

        private void pnlMenu_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnMenu_Click(object sender, EventArgs e)
        {

        }

        private void btnMenu_Click_1(object sender, EventArgs e)
        {
            if (menuExpandido)
            {
                pnlMenu.Width = 60;
                menuExpandido = false;
            }
            else
            {
                pnlMenu.Width = 220;
                menuExpandido = true;
            }
        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FrmGestionAsignaturas());
        }

        private void btnAsistencia_Click_1(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FrmReporteDesercionRetencion(this));
        }

        private void btnReunion_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FrmGestionReuniones());
        }
    }
}