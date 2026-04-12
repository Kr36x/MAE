using GestionAcademicaV2.Modelos;
using GestionAcademicaV2.Pantallas.AdminVentanas;
using GestionAcademicaV2.Pantallas.DocenteVentanas;
using Microsoft.Data.SqlClient;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas
{
    public partial class PantallaDocente : Form
    {
        private Form formularioActivo = null;
        private bool menuExpandido = true;
        private readonly SesionUsuario usuarioActual;
        private readonly Conexion conexion = new Conexion();
        private int idDocente;

        private readonly Color colorMenuPrincipal = Color.DarkGreen;
        private readonly Color colorSubmenu = Color.FromArgb(18, 128, 18);
        private readonly Color colorHoverPrincipal = Color.FromArgb(0, 115, 0);
        private readonly Color colorHoverSubmenu = Color.FromArgb(28, 145, 28);
        private readonly Color colorActivo = Color.FromArgb(0, 120, 0);
        private readonly Color colorTexto = Color.White;
        private readonly Color colorTextoSub = Color.WhiteSmoke;

        public PantallaDocente(SesionUsuario usuario)
        {
            InitializeComponent();

            usuarioActual = usuario;
            idDocente = ObtenerDocenteIdPorUsuarioId(usuarioActual.UsuarioID);

            ConfigurarPantalla();
            ConfigurarMenu();
            ConfigurarPanelUsuario();
        }

        private void PantallaDocente_Load_1(object sender, EventArgs e)
        {
            CargarDatosUsuario();
            OcultarTodosLosSubmenus();
            MarcarActivo(btnInicio);
            AbrirFormularioEnPanel(new FrmDocenteInicio2(usuarioActual.UsuarioID));
            // Si luego haces un dashboard docente, aquí lo abres.
            // AbrirFormularioEnPanel(new FrmDashboardDocente(idDocente));
        }

        #region Configuración inicial

        private void ConfigurarPantalla()
        {
            DoubleBuffered = true;

            if (pnlContenedor != null)
            {
                pnlContenedor.BackColor = Color.Gainsboro;
            }
        }

        private void ConfigurarMenu()
        {
            pnlMenuContenido.BackColor = colorMenuPrincipal;
            pnlMenuContenido.AutoScroll = true;
            pnlMenuContenido.WrapContents = false;
            pnlMenuContenido.FlowDirection = FlowDirection.TopDown;

            // Si quieres dejar el panel usuario fuera del flujo en el futuro,
            // ahí sí habría que rehacer un poco el diseñador.
            // Por ahora te lo dejo funcional con tu estructura actual.

            EstiloBotonPrincipal(btnInicio);
            EstiloBotonPrincipal(btnGestionAcademica);
            EstiloBotonPrincipal(btnReportes2);
            EstiloBotonPrincipal(btnReuniones);

            EstiloBotonSubmenu(btnAsistencia2);
            EstiloBotonSubmenu(btnCalificaciones2);
            EstiloBotonSubmenu(btnPlanificacion);

            EstiloBotonSubmenu(btnReporteAsistencia);
            EstiloBotonSubmenu(btnReporteSemanal);
            EstiloBotonSubmenu(btnBoletas);

            pnlSubGestionAcademica.BackColor = colorSubmenu;
            pnlSubReportes.BackColor = colorSubmenu;

            pnlSubGestionAcademica.Margin = new Padding(0);
            pnlSubReportes.Margin = new Padding(0);

            btnInicio.Click += btnInicio_Click;
            btnAsistencia2.Click += btnAsistencia2_Click;
            btnCalificaciones2.Click += btnCalificaciones2_Click;
            btnPlanificacion.Click += btnPlanificacion_Click;
            btnReporteAsistencia.Click += btnReporteAsistencia_Click;
            btnReporteSemanal.Click += btnReporteSemanal_Click;
            btnReuniones.Click += btnReuniones_Click;

            // Si luego agregas un botón hamburguesa:
            // btnMenu.Click += btnMenu_Click;
        }

        private void ConfigurarPanelUsuario()
        {
            //pnlUsuario.FillColor = Color.FromArgb(0, 110, 0);
            pnlUsuario.Margin = new Padding(0, 12, 0, 0);

            lblUsuario.ForeColor = colorTexto;
            lblRol.ForeColor = colorTextoSub;
            lblId.ForeColor = colorTextoSub;

            lblUsuario.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRol.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            lblId.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);

            // Reacomodo fino
            guna2CirclePictureBox1.Location = new Point(15, 26);
            lblUsuario.Location = new Point(65, 22);
            lblRol.Location = new Point(65, 42);
            lblId.Location = new Point(65, 60);

            AgregarSeparadorSuperiorUsuario();
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
        private void AgregarSeparadorSuperiorUsuario()
        {
            Panel separador = new Panel
            {
                Name = "pnlSeparadorUsuario",
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(70, 170, 70)
            };

            pnlUsuario.Controls.Add(separador);
            separador.BringToFront();
        }

        private void CargarDatosUsuario()
        {
            lblUsuario.Text = FormatearNombre(usuarioActual?.Usuario ?? "Usuario prueba");
            lblRol.Text = usuarioActual?.Rol ?? "Docente";
            lblId.Text = "ID: " + (usuarioActual?.UsuarioID.ToString() ?? "N/A");
        }

        #endregion

        #region Estilos

        private void EstiloBotonPrincipal(Guna.UI2.WinForms.Guna2Button btn)
        {
            btn.FillColor = Color.Transparent;
            btn.ForeColor = colorTexto;
            btn.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btn.TextAlign = HorizontalAlignment.Left;
            btn.ImageAlign = HorizontalAlignment.Left;
            btn.Padding = new Padding(18, 0, 0, 0);
            btn.Margin = new Padding(0);
            btn.Size = new Size(220, 48);
            btn.BorderRadius = 0;
            btn.HoverState.FillColor = colorHoverPrincipal;
            btn.PressedColor = colorActivo;
        }

        private void EstiloBotonSubmenu(Guna.UI2.WinForms.Guna2Button btn)
        {
            btn.FillColor = colorSubmenu;
            btn.ForeColor = colorTextoSub;
            btn.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            btn.TextAlign = HorizontalAlignment.Left;
            btn.ImageAlign = HorizontalAlignment.Left;
            btn.Padding = new Padding(38, 0, 0, 0);
            btn.Margin = new Padding(0);
            btn.Size = new Size(220, 42);
            btn.BorderRadius = 0;
            btn.HoverState.FillColor = colorHoverSubmenu;
            btn.PressedColor = colorActivo;

            // Para que no se vea tan cargado
            btn.ImageSize = new Size(22, 22);
        }

        private void MarcarActivo(Control controlActivo)
        {
            LimpiarEstadoActivoMenu();

            if (controlActivo is Guna.UI2.WinForms.Guna2Button btn)
            {
                btn.FillColor = colorActivo;
            }
        }

        private void LimpiarEstadoActivoMenu()
        {
            foreach (Control control in pnlMenuContenido.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2Button boton)
                {
                    boton.FillColor = Color.Transparent;
                }

                if (control is Panel panel)
                {
                    foreach (Control sub in panel.Controls)
                    {
                        if (sub is Guna.UI2.WinForms.Guna2Button botonSub)
                        {
                            botonSub.FillColor = colorSubmenu;
                        }
                    }
                }
            }
        }

        #endregion

        #region Menú acordeón

        private void OcultarTodosLosSubmenus()
        {
            pnlSubGestionAcademica.Visible = false;
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

        private void btnReportes_Click(object sender, EventArgs e)
        {
            MostrarSubMenu(pnlSubReportes);
            MarcarActivo(btnReportes2);
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


        public void MostrarReporteAsistencia(int docenteId)
        {
            AbrirFormularioEnPanel(new FrmDocenteReporteAsistenciaMensual(docenteId));
        }

        public void MostrarReporteCalificacion(int docenteId)
        {
            AbrirFormularioEnPanel(new FrmDocenteReporteSemanalCalificaciones(docenteId));
        }

        public void MostrarConsolidadoAsignaturas(int docenteId)
        {
            AbrirFormularioEnPanel(new FrmAdminConsolidadoAsignaturas());
        }

        public void MostrarControlReuniones(int docenteId)
        {
            AbrirFormularioEnPanel(new FrmControlReuniones_Obsoleto());
        }

        public void MoverPantallaAdmin(int docenteid)
        {
            AbrirFormularioEnPanel(new PantallaAdmin(usuarioActual));
        }

        #endregion

        #region Eventos botones

        private void btnInicio_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnInicio);
            AbrirFormularioEnPanel(new FrmDocenteInicio2(usuarioActual.UsuarioID));
        }

        private void btnAsistencia2_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnAsistencia2);
            AbrirFormularioEnPanel(new FrmDocenteRegistroAsistencia(idDocente));
        }

        private void btnCalificaciones2_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnCalificaciones2);
            AbrirFormularioEnPanel(new FrmDocenteRegistroCalificaciones(idDocente));
        }

        private void btnPlanificacion_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnPlanificacion);
            AbrirFormularioEnPanel(new FrmPlanifacacionActividades(idDocente));
        }

        private void btnReporteAsistencia_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnReporteAsistencia);
            AbrirFormularioEnPanel(new FrmDocenteReporteAsistenciaMensual(usuarioActual.UsuarioID));
        }

        private void btnReporteSemanal_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnReporteSemanal);
            AbrirFormularioEnPanel(new FrmDocenteReporteSemanalCalificaciones(usuarioActual.UsuarioID));
        }

        private void btnReportesSeleccion_Click(object sender, EventArgs e)
        {
            MarcarActivo(btnReporteSemanal);
            AbrirFormularioEnPanel(new SeleccionReportes_Defensa(this,idDocente));
        }

        private void btnReuniones_Click(object sender, EventArgs e)
        {
            OcultarTodosLosSubmenus();
            MarcarActivo(btnReuniones);
            AbrirFormularioEnPanel(new FrmDocenteConsultaReuniones(idDocente));
        }

        #endregion

        #region Utilidades

        private string FormatearNombre(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return string.Empty;

            string[] partes = usuario.Replace(".", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < partes.Length; i++)
            {
                if (partes[i].Length > 0)
                {
                    partes[i] = char.ToUpper(partes[i][0]) + partes[i][1..].ToLower();
                }
            }

            return string.Join(" ", partes);
        }

        public int ObtenerDocenteIdPorUsuarioId(int usuarioId)
        {
            int docenteId = 0;

            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
                    SELECT DocenteID
                    FROM Docente
                    WHERE UsuarioID = @UsuarioID", cn);

                cmd.Parameters.AddWithValue("@UsuarioID", usuarioId);

                cn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    docenteId = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo obtener el DocenteID.\n\n" + ex.Message,
                    "Sistema MAE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return docenteId;
        }

        #endregion

        #region Extras opcionales

        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (menuExpandido)
            {
                pnlMenuContenido.Width = 72;
                menuExpandido = false;
                ContraerMenuVisual();
            }
            else
            {
                pnlMenuContenido.Width = 220;
                menuExpandido = true;
                ExpandirMenuVisual();
            }
        }

        private void ContraerMenuVisual()
        {
            OcultarTodosLosSubmenus();

            OcultarTextoBoton(btnInicio);
            OcultarTextoBoton(btnGestionAcademica);
            OcultarTextoBoton(btnReportes2);
            OcultarTextoBoton(btnReuniones);

            OcultarTextoBoton(btnAsistencia2);
            OcultarTextoBoton(btnCalificaciones2);
            OcultarTextoBoton(btnPlanificacion);
            OcultarTextoBoton(btnReporteAsistencia);
            OcultarTextoBoton(btnReporteSemanal);
            OcultarTextoBoton(btnBoletas);

            lblUsuario.Visible = false;
            lblRol.Visible = false;
            lblId.Visible = false;
        }

        private void ExpandirMenuVisual()
        {
            btnInicio.Text = "INICIO";
            btnGestionAcademica.Text = "GESTIÓN ACADÉMICA";
            btnReportes2.Text = "REPORTES";
            btnReuniones.Text = "REUNIONES";

            btnAsistencia2.Text = "ASISTENCIA";
            btnCalificaciones2.Text = "CALIFICACIONES";
            btnPlanificacion.Text = "PLANIFICACIÓN";
            btnReporteAsistencia.Text = "REPORTE ASISTENCIA";
            btnReporteSemanal.Text = "REPORTE SEMANAL";
            btnBoletas.Text = "BOLETAS";

            lblUsuario.Visible = true;
            lblRol.Visible = true;
            lblId.Visible = true;
        }

        private void OcultarTextoBoton(Guna.UI2.WinForms.Guna2Button btn)
        {
            btn.Text = string.Empty;
        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {
            // Por si luego quieres abrir perfil docente.
        }

        #endregion
    }
}