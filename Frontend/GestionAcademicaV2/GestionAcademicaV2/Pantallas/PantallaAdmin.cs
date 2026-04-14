using GestionAcademicaV2.Modelos;
using GestionAcademicaV2.Pantallas.AdminVentanas;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Drawing;
using System.Windows.Forms;
using GestionAcademicaV2.Pantallas.DocenteVentanas;
namespace GestionAcademicaV2.Pantallas
{
    public partial class PantallaAdmin : Form
    {
        private Form formularioActivo = null;
        private bool menuExpandido = true;
        private readonly SesionUsuario usuarioActual;

        private readonly Color colorMenuPrincipal = Color.FromArgb(14, 102, 248);   // azul principal
        private readonly Color colorSubmenu = Color.FromArgb(30, 120, 255);         // azul más claro
        private readonly Color colorHoverPrincipal = Color.FromArgb(10, 85, 210);   // hover oscuro
        private readonly Color colorHoverSubmenu = Color.FromArgb(50, 140, 255);    // hover submenu
        private readonly Color colorActivo = Color.FromArgb(0, 70, 180);            // botón activo
        private readonly Color colorTexto = Color.White;
        private readonly Color colorTextoSub = Color.FromArgb(230, 240, 255);       // blanco azulado

        public PantallaAdmin(SesionUsuario usuario)
        {
            InitializeComponent();
            usuarioActual = usuario;

            ConfigurarPantalla();
            ConfigurarMenu();
            ConfigurarPanelUsuario();

        }

        public PantallaAdmin()
        {
            InitializeComponent();

            ConfigurarPantalla();
            ConfigurarMenu();
            ConfigurarPanelUsuario();

        }


        private void PantallaAdmin_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = FormatearNombre(usuarioActual?.Usuario ?? "Usuario prueba");
            lblRol.Text = usuarioActual?.Rol ?? "Administrador";
            lblId.Text = "ID: " + (usuarioActual?.UsuarioID.ToString() ?? "N/A");

            OcultarTodosLosSubmenus();
            MarcarActivo(btnInicio);
            AbrirFormularioEnPanel(new FrmDashboard(this));
        }

        #region Configuración inicial

        private void ConfigurarPantalla()
        {
            DoubleBuffered = true;

            if (PnlContenedorAdmin != null)
                PnlContenedorAdmin.BackColor = Color.Gainsboro;
        }

        private void ConfigurarMenu()
        {
            pnlMenu.BackColor = colorMenuPrincipal;

            // Principales
            EstiloBotonPrincipal(btnInicio);
            EstiloBotonPrincipal(btnGestionAcademica);
            EstiloBotonPrincipal(btnGestionInstitucional);
            EstiloBotonPrincipal(btnReportes);

            // Submenú Gestión Académica
            EstiloBotonSubmenu(btnEstudiantes);
            EstiloBotonSubmenu(btnMatricula);
            EstiloBotonSubmenu(btnDocentes);
            EstiloBotonSubmenu(btnGestionAsignaturas);
            EstiloBotonSubmenu(btnGestionGrado);

            // Submenú Gestión Institucional
            EstiloBotonSubmenu(btnGestionUsuarios);

            EstiloBotonSubmenu(btnVinculacionTutores);
            EstiloBotonSubmenu(btnGestionReuniones);
            EstiloBotonSubmenu(btnConfigCicloEscolar);


            // Submenú Reportes
            EstiloBotonSubmenu(btnReporteDocentes);
            EstiloBotonSubmenu(btnReporteDesercion);
            EstiloBotonSubmenu(btnConsolidadoAsignatura);

            pnlSubGestionAcademica.BackColor = colorSubmenu;
            pnlSubGestionInstitucional.BackColor = colorSubmenu;
            pnlSubReportes.BackColor = colorSubmenu;

            pnlSubGestionAcademica.Margin = new Padding(0);
            pnlSubGestionInstitucional.Margin = new Padding(0);
            pnlSubReportes.Margin = new Padding(0);

            // EVENTOS PRINCIPALES
            btnInicio.Click += btnInicio_Click;
            btnGestionAcademica.Click += btnGestionAcademica_Click;
            btnGestionInstitucional.Click += btnGestionInstitucional_Click;
            btnReportes.Click += btnReportes_Click;

            // EVENTOS SUBMENÚ GESTIÓN ACADÉMICA

            btnEstudiantes.Click += btnEstudiantes_Click;
            btnMatricula.Click += btnMatricula_Click;
            btnDocentes.Click += btnDocentes_Click;
            btnGestionAsignaturas.Click += btnGestionAsignaturas_Click;
            btnGestionGrado.Click += btnGestionGrado_Click;

            // EVENTOS SUBMENÚ GESTIÓN INSTITUCIONAL
            btnGestionUsuarios.Click += btnGestionUsuarios_Click;
            btnVinculacionTutores.Click += btnVinculacionTutores_Click;

            btnGestionReuniones.Click += btnGestionReuniones_Click;


            // EVENTOS SUBMENÚ REPORTES
            btnReporteDocentes.Click += btnReporteDocentes_Click;
            btnReporteDesercion.Click += btnReporteDesercion_Click;
            btnConsolidadoAsignatura.Click += btnConsolidadoAsignatura_Click;

            // OPCIONALES
            btnSalir.Click += btnSalir_Click;
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "¿Deseas cerrar sesión?",
                "Cerrar sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            Login loginAbierto = null;

            foreach (Form form in Application.OpenForms)
            {
                if (form is Login login)
                {
                    loginAbierto = login;
                    break;
                }
            }

            if (loginAbierto != null)
            {
                loginAbierto.LimpiarCampos();
                loginAbierto.Show();
                loginAbierto.BringToFront();
            }

            this.Close();
        }
        private void ConfigurarPanelUsuario()
        {
            lblUsuario.ForeColor = colorTexto;
            lblRol.ForeColor = colorTextoSub;
            lblId.ForeColor = colorTextoSub;

            lblUsuario.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRol.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            lblId.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
        }

        #endregion

        #region Estilos

        private void EstiloBotonPrincipal(Guna.UI2.WinForms.Guna2Button btn)
        {
            btn.FillColor = Color.Transparent;
            btn.ForeColor = colorTexto;
            btn.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btn.TextAlign = HorizontalAlignment.Left;
            btn.ImageAlign = HorizontalAlignment.Left;
            btn.Padding = new Padding(13, 0, 0, 0);
            btn.Margin = new Padding(0);
            btn.BorderRadius = 0;
            btn.HoverState.FillColor = colorHoverPrincipal;
            btn.PressedColor = colorActivo;
        }

        private void EstiloBotonSubmenu(Guna.UI2.WinForms.Guna2Button btn)
        {
            btn.FillColor = colorSubmenu;
            btn.ForeColor = colorTextoSub;
            btn.Font = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            btn.TextAlign = HorizontalAlignment.Left;
            btn.ImageAlign = HorizontalAlignment.Left;
            btn.Padding = new Padding(20, 0, 0, 0);
            btn.Margin = new Padding(0);
            btn.BorderRadius = 0;
            btn.HoverState.FillColor = colorHoverSubmenu;
            btn.PressedColor = colorActivo;
            btn.ImageSize = new Size(22, 22);
        }

        private void MarcarActivo(Control controlActivo)
        {
            LimpiarEstadoActivoMenu();

            if (controlActivo is Guna.UI2.WinForms.Guna2Button btn)
                btn.FillColor = colorActivo;
        }

        private void LimpiarEstadoActivoMenu()
        {
            foreach (Control control in pnlMenu.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2Button boton)
                    boton.FillColor = Color.Transparent;

                if (control is Panel panel)
                {
                    foreach (Control sub in panel.Controls)
                    {
                        if (sub is Guna.UI2.WinForms.Guna2Button botonSub)
                            botonSub.FillColor = colorSubmenu;
                    }
                }
            }
        }

        #endregion

        #region Acordeón

        private void OcultarTodosLosSubmenus()
        {
            pnlSubGestionAcademica.Visible = false;
            pnlSubGestionInstitucional.Visible = false;
            pnlSubReportes.Visible = false;
        }

        private void MostrarSubMenu(Panel subMenu)
        {
            if (!subMenu.Visible)
            {
                OcultarTodosLosSubmenus();
                subMenu.Visible = true;
            }
            else
            {
                subMenu.Visible = false;
            }
        }

        private void btnGestionAcademica_Click(object sender, EventArgs e)
        {
            MostrarSubMenu(pnlSubGestionAcademica);
            MarcarActivo(btnGestionAcademica);
        }

        private void btnGestionInstitucional_Click(object sender, EventArgs e)
        {
            MostrarSubMenu(pnlSubGestionInstitucional);
            MarcarActivo(btnGestionInstitucional);
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            MostrarSubMenu(pnlSubReportes);
            MarcarActivo(btnReportes);
        }

        #endregion

        #region Navegación

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

                PnlContenedorAdmin.Controls.Clear();
                PnlContenedorAdmin.Controls.Add(formularioHijo);
                PnlContenedorAdmin.Tag = formularioHijo;

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

        #endregion

        #region Eventos principales

        private void btnInicio_Click(object sender, EventArgs e)
        {
            OcultarTodosLosSubmenus();
            MarcarActivo(btnInicio);
            AbrirFormularioEnPanel(new FrmDashboard(this));

        }

        #endregion

        #region Gestión académica

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            //MarcarActivo(btnDashboard);
        }
        private void btnEstudiantes_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnEstudiantes);
            AbrirFormularioEnPanel(new FrmEstudiantes(this));
        }

        private void btnMatricula_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnMatricula);
            AbrirFormularioEnPanel(new FrmMatricula(this));
        }



        private void btnDocentes_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnDocentes);
            AbrirFormularioEnPanel(new CargaDocente(this));
        }

        private void btnGestionAsignaturas_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnGestionAsignaturas);
            AbrirFormularioEnPanel(new FrmGestionAsignaturas());
        }

        private void btnGestionGrado_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnGestionGrado);
            AbrirFormularioEnPanel(new FrmGestionGrado());
        }

        #endregion

        #region Gestión institucional

        private void btnGestionUsuarios_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnGestionUsuarios);
            AbrirFormularioEnPanel(new FrmUsuariosPersonal(this));
        }


        private void btnVinculacionTutores_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnVinculacionTutores);
            AbrirFormularioEnPanel(new FrmGestionVinculacionTutores());
        }



        private void btnGestionReuniones_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnGestionReuniones);
            AbrirFormularioEnPanel(new FrmGestionReuniones());
        }


        private void btnConfigCicloEscolar_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnConfigCicloEscolar);
            AbrirFormularioEnPanel(new FrmConfigCicloEscolar());
        }


        #endregion

        #region Reportes

        private void btnReporteDocentes_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnReporteDocentes);
            AbrirFormularioEnPanel(new ReporteDocentes());
        }

        private void btnReporteDesercion_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnReporteDesercion);
            AbrirFormularioEnPanel(new FrmReporteDesercionRetencion(this));
        }

        private void btnConsolidadoAsignatura_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnConsolidadoAsignatura);
            AbrirFormularioEnPanel(new FrmAdminConsolidadoAsignaturas());
        }

        #endregion

        #region Utilidades

        private string FormatearNombre(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return "";

            string[] partes = usuario.Replace(".", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < partes.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(partes[i]))
                    partes[i] = char.ToUpper(partes[i][0]) + partes[i][1..].ToLower();
            }

            return string.Join(" ", partes);
        }



        #endregion

    }
}