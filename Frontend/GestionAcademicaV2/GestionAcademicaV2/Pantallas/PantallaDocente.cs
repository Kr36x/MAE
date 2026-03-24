using GestionAcademicaV2.Modelos;
using GestionAcademicaV2.Pantallas.AdminVentanas;
using GestionAcademicaV2.Pantallas.DocenteVentanas;
using System;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas
{
    public partial class PantallaDocente : Form
    {
        private Form formularioActivo = null;
        private bool menuExpandido = true;
        private SesionUsuario usuarioActual;
        int idDocente;

        public PantallaDocente(SesionUsuario usuario)
        {
            InitializeComponent();
            usuarioActual = usuario;
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

            pnlContenedor.Controls.Clear();
            pnlContenedor.Controls.Add(formularioHijo);
            pnlContenedor.Tag = formularioHijo;

            formularioHijo.Show();
            formularioHijo.BringToFront();
        }

        public void MostrarBoletaPorNivel(string nivel, int estudianteID)
        {
            if (nivel == "PREBASICA")
                AbrirFormularioEnPanel(new FrmBoletaPrebasica(estudianteID));
        }

        private void PantallaDocente_Load(object sender, EventArgs e)
        {

        }

        private void btnMenu_Click(object sender, EventArgs e)
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

        private void btnCalificaciones_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FrmSeleccionBoleta(this));
        }
        private string FormatearNombre(string usuario)
        {
            if (string.IsNullOrEmpty(usuario))
                return "";

            string[] partes = usuario.Replace(".", " ").Split(' ');

            for (int i = 0; i < partes.Length; i++)
            {
                if (partes[i].Length > 0)
                {
                    partes[i] = char.ToUpper(partes[i][0]) + partes[i].Substring(1).ToLower();
                }
            }

            return string.Join(" ", partes);
        }
        private void PantallaDocente_Load_1(object sender, EventArgs e)
        {
            lblUsuario.Text = FormatearNombre(usuarioActual?.Usuario ?? "Usuario prueba");
            lblRol.Text = usuarioActual?.Rol ?? "Sin rol";
            lblId.Text = "ID: " + (usuarioActual?.UsuarioID.ToString() ?? "N/A");
        }
        

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            //FrmReporteAsistencia frm = new FrmReporteAsistencia(usuarioActual.UsuarioID);
            //frm.Show();
            AbrirFormularioEnPanel(new FrmSeleccionReportes(this, usuarioActual.UsuarioID));
        }

        public void MostrarReporteAsistencia(int docenteId)
        {
            AbrirFormularioEnPanel(new FrmReporteAsistencia(docenteId));
        }
            public void MostrarReporteCalificacion(int docenteId)
        {
            AbrirFormularioEnPanel(new FrmReporteSemanal(docenteId));
        }

        public void MostrarConsolidadoAsignaturas(int docenteId)
        {
            AbrirFormularioEnPanel(new FrmConsolidadoAsignaturas());
        }

        public void MostrarControlReuniones(int docenteId)
        {
            AbrirFormularioEnPanel(new FrmControlReuniones());
        }

        public void MoverPantallaAdmin(int docenteid)
        {
            AbrirFormularioEnPanel(new PantallaAdmin(usuarioActual));
        }
    }
}