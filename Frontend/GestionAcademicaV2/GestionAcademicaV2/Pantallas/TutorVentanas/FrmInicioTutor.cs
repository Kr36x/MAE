using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.TutorVentanas
{
    public partial class FrmInicioTutor : Form
    {
        private readonly int _idUsuario;
        private int _idTutor;

        private readonly Conexion _conexion = new Conexion();

        private enum VistaTutor
        {
            Ninguna,
            Asistencia,
            Calificaciones,
            Reuniones
        }

        private VistaTutor _vistaActual = VistaTutor.Ninguna;

        private readonly Color _colorOlive = Color.FromArgb(145, 148, 0);
        private readonly Color _colorOliveDark = Color.FromArgb(120, 122, 0);
        private readonly Color _colorBg = Color.FromArgb(245, 246, 248);
        private readonly Color _colorCard = Color.White;
        private readonly Color _colorBorder = Color.FromArgb(228, 231, 235);
        private readonly Color _colorText = Color.FromArgb(45, 45, 45);
        private readonly Color _colorSubtext = Color.FromArgb(110, 110, 110);

        private readonly Color _colorAccentGray = Color.FromArgb(120, 120, 120);
        private readonly Color _colorAccentGrayDark = Color.FromArgb(95, 95, 95);
        private readonly Color _colorGridHeader = Color.FromArgb(110, 110, 110);
        private readonly Color _colorGridSelection = Color.FromArgb(232, 234, 238);

        public FrmInicioTutor(int idUsuario)
        {
            InitializeComponent();

            _idUsuario = idUsuario;
            _idTutor = 0;

            AplicarEstiloVisual();
            ConfigurarFormulario();
            ConfigurarEventos();
            CargarDatosIniciales();
        }

        private string ObtenerNombreTutor()
        {
            using SqlConnection cn = _conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 1 Nombre
        FROM Tutor
        WHERE TutorID = @TutorID;", cn);

            cmd.Parameters.AddWithValue("@TutorID", _idTutor);

            cn.Open();
            object? result = cmd.ExecuteScalar();

            return result == null || result == DBNull.Value
                ? "tutor"
                : result.ToString() ?? "tutor";
        }

        private string FormatearNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return string.Empty;

            string[] partes = nombre.Replace(".", " ")
                                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < partes.Length; i++)
            {
                if (partes[i].Length > 0)
                    partes[i] = char.ToUpper(partes[i][0]) + partes[i][1..].ToLower();
            }

            return string.Join(" ", partes);
        }
        private void ConfigurarFormulario()
        {
            lblTituloDashboard.Text = "INICIO TUTOR";
            lblSaludoDocente.Text = "Bienvenido, tutor.";
            lblPestaña.Text = "PESTAÑA";
            lblFechaConsulta.Text = string.Empty;

            guna2HtmlLabel3.Text = "FILTROS DE CONSULTA";

            LimpiarTarjetaAsistencia();
            LimpiarTarjetaCalificaciones();
            LimpiarTarjetaReuniones();

            dgvInfo.DataSource = null;
            dgvInfo.AutoGenerateColumns = true;
            dgvInfo.AllowUserToAddRows = false;
            dgvInfo.AllowUserToDeleteRows = false;
            dgvInfo.ReadOnly = true;
            dgvInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInfo.MultiSelect = false;
            dgvInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInfo.RowHeadersVisible = false;

            btnVerAsistencia.Enabled = false;
            btnVerCalificaciones.Enabled = false;
            btnVerReuniones.Enabled = false;

            dtpFecha.Value = DateTime.Today;
        }

        private void ConfigurarEventos()
        {
            Load += FrmInicioTutor_Load;

            btnConsultar.Click += btnConsultar_Click;
            btnLimpiar.Click += btnLimpiar_Click;

            btnVerAsistencia.Click += btnVerAsistencia_Click;
            btnVerCalificaciones.Click += btnVerCalificaciones_Click;
            btnVerReuniones.Click += btnVerReuniones_Click;
        }

        private void FrmInicioTutor_Load(object? sender, EventArgs e)
        {
            ActualizarFechaHora();
        }

        private void CargarDatosIniciales()
        {
            try
            {
                _idTutor = ObtenerTutorIdPorUsuario(_idUsuario);

                if (_idTutor <= 0)
                {
                    MessageBox.Show(
                        "No se encontró un tutor asociado al usuario actual.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                string nombreTutor = FormatearNombre(ObtenerNombreTutor());
                lblSaludoDocente.Text = $"Bienvenido, {nombreTutor}.";

                CargarAnios();
                CargarEstudiantes();
                ActualizarFechaHora();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar la pantalla del tutor.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ActualizarFechaHora()
        {
            lblFechaActual.Text = $"Fecha: {DateTime.Now:dd/MM/yyyy}";
            lblHoraActual.Text = $"Hora: {DateTime.Now:hh:mm:ss tt}";
        }

        private void AplicarEstiloVisual()
        {
            BackColor = _colorBg;
            pnlMainDashboard.FillColor = _colorBg;

            EstilizarContenedorBlanco(guna2Panel1);
            EstilizarContenedorBlanco(guna2Panel3);
            EstilizarContenedorBlanco(guna2Panel4);
            EstilizarContenedorBlanco(guna2Panel6);
            EstilizarContenedorBlanco(guna2Panel23);
            EstilizarContenedorBlanco(guna2Panel11);

            EstilizarHeaderOlive(guna2Panel8, guna2HtmlLabel1);
            EstilizarHeaderOlive(guna2Panel9, guna2HtmlLabel2);
            EstilizarHeaderOlive(guna2Panel24, guna2HtmlLabel20);
            EstilizarHeaderOlive(guna2Panel13, guna2HtmlLabel3);
            EstilizarHeaderOlive(guna2Panel14, lblPestaña);

            EstilizarBotonPrimario(btnConsultar);
            EstilizarBotonSecundario(btnLimpiar);

            EstilizarBotonResumen(btnVerAsistencia);
            EstilizarBotonResumen(btnVerCalificaciones);
            EstilizarBotonResumen(btnVerReuniones);

            EstilizarCombobox(cbEstudiante);
            EstilizarCombobox(cbAnio);
            EstilizarDatePicker(dtpFecha);

            EstilizarLabelsTexto();
            EstilizarGrid();

            dgvInfo.Location = new Point(10, 38);
            dgvInfo.Size = new Size(910, 130);
            lblFechaConsulta.Location = new Point(12, 10);
        }

        private void EstilizarContenedorBlanco(Guna.UI2.WinForms.Guna2Panel panel)
        {
            panel.FillColor = _colorCard;
            panel.BorderColor = _colorBorder;
            panel.BorderThickness = 1;
            panel.BorderRadius = 10;
            panel.BackColor = Color.Transparent;
        }

        private void EstilizarHeaderOlive(Guna.UI2.WinForms.Guna2Panel panel, Guna.UI2.WinForms.Guna2HtmlLabel label)
        {
            panel.FillColor = _colorOlive;
            panel.BorderRadius = 8;

            label.ForeColor = Color.White;
            label.BackColor = _colorOlive;
            label.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        }

        private void EstilizarBotonPrimario(Guna.UI2.WinForms.Guna2Button btn)
        {
            btn.FillColor = _colorOlive;
            btn.HoverState.FillColor = _colorOliveDark;
            btn.PressedColor = _colorOliveDark;
            btn.ForeColor = Color.White;
            btn.BorderRadius = 8;
            btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private void EstilizarBotonSecundario(Guna.UI2.WinForms.Guna2Button btn)
        {
            btn.FillColor = Color.FromArgb(125, 125, 125);
            btn.HoverState.FillColor = Color.FromArgb(105, 105, 105);
            btn.ForeColor = Color.White;
            btn.BorderRadius = 8;
            btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        private void EstilizarBotonResumen(Guna.UI2.WinForms.Guna2Button btn)
        {
            btn.FillColor = _colorOlive;
            btn.HoverState.FillColor = _colorOliveDark;
            btn.ForeColor = Color.White;
            btn.BorderRadius = 8;
            btn.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.BorderThickness = 0;
        }

        private void EstilizarCombobox(Guna.UI2.WinForms.Guna2ComboBox cb)
        {
            cb.BorderRadius = 8;
            cb.BorderColor = _colorBorder;
            cb.FillColor = Color.White;
            cb.ForeColor = _colorText;
            cb.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            cb.FocusedState.BorderColor = _colorGridHeader;
            cb.HoverState.BorderColor = _colorGridHeader;
        }

        private void EstilizarDatePicker(Guna.UI2.WinForms.Guna2DateTimePicker dtp)
        {
            dtp.BorderRadius = 8;
            dtp.FillColor = Color.White;
            dtp.ForeColor = _colorText;
            dtp.Font = new Font("Segoe UI", 9.5f);
            dtp.BorderColor = _colorBorder;
        }

        private void EstilizarLabelsTexto()
        {
            lblTitulo.ForeColor = _colorText;
            lblTitulo.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);

            guna2HtmlLabel12.ForeColor = _colorText;
            guna2HtmlLabel12.Font = new Font("Segoe UI", 20F, FontStyle.Regular);

            guna2HtmlLabel19.ForeColor = _colorText;
            guna2HtmlLabel19.Font = new Font("Segoe UI", 16F, FontStyle.Regular);

            guna2HtmlLabel10.ForeColor = _colorSubtext;
            guna2HtmlLabel10.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            lblTituloDashboard.ForeColor = _colorText;
            lblTituloDashboard.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            lblSaludoDocente.ForeColor = _colorSubtext;
            lblSaludoDocente.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            lblFechaActual.ForeColor = _colorText;
            lblHoraActual.ForeColor = _colorText;
            guna2HtmlLabel8.ForeColor = _colorText;

            lbAreaAsignatura.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            guna2HtmlLabel5.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            lblasda.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);

            lblFechaConsulta.ForeColor = Color.White;
            lblFechaConsulta.BackColor = _colorOlive;
            lblFechaConsulta.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            ConfigurarLabelResumen(lblA1);
            ConfigurarLabelResumen(lblA2);
            ConfigurarLabelResumen(lblA3);
            ConfigurarLabelResumen(lblA4);

            ConfigurarLabelResumen(lblB1);
            ConfigurarLabelResumen(lblB2);
            ConfigurarLabelResumen(lblB3);
            ConfigurarLabelResumen(lblB4);

            ConfigurarLabelResumen(lblC1);
            ConfigurarLabelResumen(lblC2);
            ConfigurarLabelResumen(lblC3);
            ConfigurarLabelResumen(lblC4);
        }

        private void ConfigurarLabelResumen(Guna.UI2.WinForms.Guna2HtmlLabel lbl)
        {
            lbl.ForeColor = _colorText;
            lbl.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lbl.BackColor = Color.Transparent;
        }

        private void EstilizarGrid()
        {
            dgvInfo.BackgroundColor = Color.White;
            dgvInfo.BorderStyle = BorderStyle.None;
            dgvInfo.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvInfo.GridColor = Color.FromArgb(238, 240, 244);

            dgvInfo.EnableHeadersVisualStyles = false;
            dgvInfo.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvInfo.ColumnHeadersHeight = 36;
            dgvInfo.ColumnHeadersDefaultCellStyle.BackColor = _colorGridHeader;
            dgvInfo.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvInfo.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
            dgvInfo.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvInfo.DefaultCellStyle.BackColor = Color.White;
            dgvInfo.DefaultCellStyle.ForeColor = _colorText;
            dgvInfo.DefaultCellStyle.SelectionBackColor = _colorGridSelection;
            dgvInfo.DefaultCellStyle.SelectionForeColor = _colorText;
            dgvInfo.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            dgvInfo.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 251);
            dgvInfo.AlternatingRowsDefaultCellStyle.SelectionBackColor = _colorGridSelection;
            dgvInfo.AlternatingRowsDefaultCellStyle.SelectionForeColor = _colorText;

            dgvInfo.RowTemplate.Height = 32;
            dgvInfo.RowHeadersVisible = false;
            dgvInfo.DefaultCellStyle.Padding = new Padding(2, 0, 2, 0);
        }

        private void MarcarBotonActivo(Guna.UI2.WinForms.Guna2Button botonActivo)
        {
            Guna.UI2.WinForms.Guna2Button[] botones =
            {
        btnVerAsistencia,
        btnVerCalificaciones,
        btnVerReuniones
    };

            foreach (var btn in botones)
            {
                btn.FillColor = _colorOlive;
                btn.HoverState.FillColor = _colorOliveDark;
                btn.ForeColor = Color.White;
                btn.BorderThickness = 0;
            }

            botonActivo.FillColor = _colorAccentGray;
            botonActivo.HoverState.FillColor = _colorAccentGrayDark;
            botonActivo.ForeColor = Color.White;
        }

        private int ObtenerTutorIdPorUsuario(int idUsuario)
        {
            using SqlConnection cn = _conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 TutorID
                FROM Tutor
                WHERE UsuarioID = @UsuarioID;", cn);

            cmd.Parameters.AddWithValue("@UsuarioID", idUsuario);

            cn.Open();
            object? result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                return 0;

            return Convert.ToInt32(result);
        }

        private void CargarEstudiantes()
        {
            using SqlConnection cn = _conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    E.EstudianteID,
                    E.Nombre
                FROM TutorEstudiante TE
                INNER JOIN Estudiante E ON E.EstudianteID = TE.EstudianteID
                WHERE TE.TutorID = @TutorID
                ORDER BY E.Nombre;", cn);

            cmd.Parameters.AddWithValue("@TutorID", _idTutor);

            DataTable dt = new DataTable();
            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            cbEstudiante.DataSource = dt;
            cbEstudiante.DisplayMember = "Nombre";
            cbEstudiante.ValueMember = "EstudianteID";
            cbEstudiante.SelectedIndex = -1;
        }

        private void CargarAnios()
        {
            cbAnio.Items.Clear();

            int anioActual = DateTime.Now.Year;

            for (int i = anioActual - 2; i <= anioActual + 1; i++)
                cbAnio.Items.Add(i);

            cbAnio.SelectedItem = anioActual;
        }

        private bool ValidarFiltros()
        {
            if (cbEstudiante.SelectedIndex < 0 || cbEstudiante.SelectedValue == null)
            {
                MessageBox.Show(
                    "Seleccione un estudiante.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                cbEstudiante.Focus();
                return false;
            }

            if (cbAnio.SelectedItem == null)
            {
                MessageBox.Show(
                    "Seleccione un año.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                cbAnio.Focus();
                return false;
            }

            return true;
        }

        private void btnConsultar_Click(object? sender, EventArgs e)
        {
            if (!ValidarFiltros())
                return;

            try
            {
                CargarResumenAsistencia();
                CargarResumenCalificaciones();
                CargarResumenReuniones();

                btnVerAsistencia.Enabled = true;
                btnVerCalificaciones.Enabled = true;
                btnVerReuniones.Enabled = true;

                MostrarAsistencia();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al consultar la información.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object? sender, EventArgs e)
        {
            cbEstudiante.SelectedIndex = -1;

            if (cbAnio.Items.Count > 0)
                cbAnio.SelectedItem = DateTime.Now.Year;
            else
                cbAnio.SelectedIndex = -1;

            dtpFecha.Value = DateTime.Today;

            LimpiarTarjetaAsistencia();
            LimpiarTarjetaCalificaciones();
            LimpiarTarjetaReuniones();

            lblPestaña.Text = "PESTAÑA";
            lblFechaConsulta.Text = string.Empty;

            dgvInfo.DataSource = null;
            dgvInfo.Columns.Clear();

            btnVerAsistencia.Enabled = false;
            btnVerCalificaciones.Enabled = false;
            btnVerReuniones.Enabled = false;

            btnVerAsistencia.FillColor = _colorOlive;
btnVerCalificaciones.FillColor = _colorOlive;
btnVerReuniones.FillColor = _colorOlive;

btnVerAsistencia.HoverState.FillColor = _colorOliveDark;
btnVerCalificaciones.HoverState.FillColor = _colorOliveDark;
btnVerReuniones.HoverState.FillColor = _colorOliveDark;

            _vistaActual = VistaTutor.Ninguna;
        }

        private void btnVerAsistencia_Click(object? sender, EventArgs e)
        {
            if (!ValidarFiltros())
                return;

            MostrarAsistencia();
        }

        private void btnVerCalificaciones_Click(object? sender, EventArgs e)
        {
            if (!ValidarFiltros())
                return;

            MostrarCalificaciones();
        }

        private void btnVerReuniones_Click(object? sender, EventArgs e)
        {
            if (!ValidarFiltros())
                return;

            MostrarReuniones();
        }

        private void MostrarAsistencia()
        {
            DataTable dt = ObtenerAsistencia(
                _idTutor,
                Convert.ToInt32(cbEstudiante.SelectedValue),
                dtpFecha.Value.Date);

            _vistaActual = VistaTutor.Asistencia;

            dgvInfo.DataSource = dt;
            lblPestaña.Text = "ASISTENCIA";
            lblFechaConsulta.Text = $"Fecha consultada: {dtpFecha.Value:dd/MM/yyyy}";

            FormatearGridAsistencia();
            MarcarBotonActivo(btnVerAsistencia);
        }

        private void MostrarCalificaciones()
        {
            DataTable dt = ObtenerCalificaciones(
                _idTutor,
                Convert.ToInt32(cbEstudiante.SelectedValue),
                Convert.ToInt32(cbAnio.SelectedItem));

            _vistaActual = VistaTutor.Calificaciones;

            dgvInfo.DataSource = dt;
            lblPestaña.Text = "CALIFICACIONES";
            lblFechaConsulta.Text = $"Año consultado: {cbAnio.SelectedItem}";

            FormatearGridCalificaciones();
            MarcarBotonActivo(btnVerCalificaciones);
        }

        private void MostrarReuniones()
        {
            DataTable dt = ObtenerReuniones(
                _idTutor,
                Convert.ToInt32(cbEstudiante.SelectedValue),
                Convert.ToInt32(cbAnio.SelectedItem));

            _vistaActual = VistaTutor.Reuniones;

            DataTable dtProgramadas = FiltrarReunionesProgramadas(dt);

            dgvInfo.DataSource = dtProgramadas;
            lblPestaña.Text = "REUNIONES PROGRAMADAS";
            lblFechaConsulta.Text = $"Año consultado: {cbAnio.SelectedItem}";

            FormatearGridReuniones();
            MarcarBotonActivo(btnVerReuniones);

            if (dtProgramadas.Rows.Count == 0)
            {
                lblFechaConsulta.Text = $"Año consultado: {cbAnio.SelectedItem} | No hay reuniones programadas";
            }
        }
        private DataTable FiltrarReunionesProgramadas(DataTable dtOriginal)
        {
            if (dtOriginal == null || dtOriginal.Rows.Count == 0)
                return dtOriginal?.Clone() ?? new DataTable();

            var filas = dtOriginal.AsEnumerable()
                .Where(x =>
                {
                    string estado = Convert.ToString(x["Estado"])?.Trim() ?? string.Empty;

                    return estado.Equals("PROGRAMADA", StringComparison.OrdinalIgnoreCase)
                        || estado.Equals("PENDIENTE", StringComparison.OrdinalIgnoreCase);
                });

            if (!filas.Any())
                return dtOriginal.Clone();

            return filas.CopyToDataTable();
        }
        private void CargarResumenAsistencia()
        {
            int estudianteId = Convert.ToInt32(cbEstudiante.SelectedValue);
            string estudiante = cbEstudiante.Text;
            string gradoSeccion = ObtenerGradoSeccionDelEstudiante(estudianteId);

            DataTable dt = ObtenerAsistencia(_idTutor, estudianteId, dtpFecha.Value.Date);

            int totalClases = dt.Rows.Count;

            int presentes = dt.AsEnumerable()
                .Count(x => string.Equals(
                    Convert.ToString(x["Estado"])?.Trim(),
                    "PRESENTE",
                    StringComparison.OrdinalIgnoreCase));

            int ausentes = dt.AsEnumerable()
                .Count(x => string.Equals(
                    Convert.ToString(x["Estado"])?.Trim(),
                    "AUSENTE",
                    StringComparison.OrdinalIgnoreCase));

            int conObservacion = dt.AsEnumerable()
                .Count(x => !string.IsNullOrWhiteSpace(Convert.ToString(x["Observacion"])));

            lblA1.Text = $"Estudiante: {estudiante}";
            lblA2.Text = $"Grado/Sección: {gradoSeccion}";
            lblA3.Text = $"Clases del día: {totalClases}";
            lblA4.Text = $"P: {presentes} | A: {ausentes} | Obs: {conObservacion}";
        }

        private void CargarResumenCalificaciones()
        {
            int estudianteId = Convert.ToInt32(cbEstudiante.SelectedValue);
            string estudiante = cbEstudiante.Text;
            string gradoSeccion = ObtenerGradoSeccionDelEstudiante(estudianteId);

            DataTable dt = ObtenerCalificaciones(
                _idTutor,
                estudianteId,
                Convert.ToInt32(cbAnio.SelectedItem));

            int excelente = dt.AsEnumerable()
                .Count(x => string.Equals(
                    Convert.ToString(x["Estado"])?.Trim(),
                    "EXCELENTE",
                    StringComparison.OrdinalIgnoreCase));

            int medio = dt.AsEnumerable()
                .Count(x => string.Equals(
                    Convert.ToString(x["Estado"])?.Trim(),
                    "MEDIO",
                    StringComparison.OrdinalIgnoreCase));

            int critico = dt.AsEnumerable()
                .Count(x => string.Equals(
                    Convert.ToString(x["Estado"])?.Trim(),
                    "CRITICO",
                    StringComparison.OrdinalIgnoreCase));

            decimal promedioGeneral = 0m;

            if (dt.Rows.Count > 0)
            {
                promedioGeneral = dt.AsEnumerable()
                    .Where(x => x["PromedioClase"] != DBNull.Value)
                    .Select(x => Convert.ToDecimal(x["PromedioClase"]))
                    .DefaultIfEmpty(0m)
                    .Average();
            }

            lblB1.Text = $"Estudiante: {estudiante}";
            lblB2.Text = $"Grado/Sección: {gradoSeccion}";
            lblB3.Text = $"Exc: {excelente} | Med: {medio} | Cri: {critico}";
            lblB4.Text = $"Promedio general: {promedioGeneral:N2}";
        }

        private void CargarResumenReuniones()
        {
            int estudianteId = Convert.ToInt32(cbEstudiante.SelectedValue);
            string estudiante = cbEstudiante.Text;
            string gradoSeccion = ObtenerGradoSeccionDelEstudiante(estudianteId);

            DataTable dt = ObtenerReuniones(
                _idTutor,
                estudianteId,
                Convert.ToInt32(cbAnio.SelectedItem));

            int pendientes = dt.AsEnumerable()
                .Count(x => string.Equals(
                    Convert.ToString(x["Estado"])?.Trim(),
                    "PENDIENTE",
                    StringComparison.OrdinalIgnoreCase));

            int realizadas = dt.AsEnumerable()
                .Count(x => string.Equals(
                    Convert.ToString(x["Estado"])?.Trim(),
                    "REALIZADA",
                    StringComparison.OrdinalIgnoreCase));

            int canceladas = dt.AsEnumerable()
                .Count(x => string.Equals(
                    Convert.ToString(x["Estado"])?.Trim(),
                    "CANCELADA",
                    StringComparison.OrdinalIgnoreCase));

            lblC1.Text = $"Estudiante: {estudiante}";
            lblC2.Text = $"Grado/Sección: {gradoSeccion}";
            lblC3.Text = $"Total reuniones: {dt.Rows.Count}";
            lblC4.Text = $"Pend: {pendientes} | Real: {realizadas} | Canc: {canceladas}";
        }

        private DataTable ObtenerAsistencia(int tutorId, int estudianteId, DateTime fecha)
        {
            using SqlConnection cn = _conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_AsistenciaTutorEstudiante", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@tutorID", tutorId);
            cmd.Parameters.AddWithValue("@estudianteID", estudianteId);
            cmd.Parameters.AddWithValue("@fecha", fecha);

            DataTable dt = new DataTable();
            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        private DataTable ObtenerCalificaciones(int tutorId, int estudianteId, int anio)
        {
            using SqlConnection cn = _conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_CalificacionesTutorEstudiante", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@tutorID", tutorId);
            cmd.Parameters.AddWithValue("@estudianteID", estudianteId);
            cmd.Parameters.AddWithValue("@anio", anio);

            DataTable dt = new DataTable();
            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        private DataTable ObtenerReuniones(int tutorId, int estudianteId, int anio)
        {
            using SqlConnection cn = _conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_ReunionesTutorEstudiante", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@tutorID", tutorId);
            cmd.Parameters.AddWithValue("@estudianteID", estudianteId);
            cmd.Parameters.AddWithValue("@anio", anio);

            DataTable dt = new DataTable();
            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        private string ObtenerGradoSeccionDelEstudiante(int estudianteId)
        {
            try
            {
                using SqlConnection cn = _conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 1
                        CONCAT(G.NombreGrado, ' ', S.Letra) AS GradoSeccion
                    FROM Matricula M
                    INNER JOIN Seccion S ON S.SeccionID = M.SeccionID
                    INNER JOIN Grado G ON G.GradoID = S.GradoID
                    WHERE M.EstudianteID = @EstudianteID
                      AND M.Anio = @Anio;", cn);

                cmd.Parameters.AddWithValue("@EstudianteID", estudianteId);
                cmd.Parameters.AddWithValue("@Anio", Convert.ToInt32(cbAnio.SelectedItem));

                cn.Open();
                object? result = cmd.ExecuteScalar();

                return result == null || result == DBNull.Value
                    ? "No disponible"
                    : result.ToString() ?? "No disponible";
            }
            catch
            {
                return "No disponible";
            }
        }

        private void LimpiarTarjetaAsistencia()
        {
            lblA1.Text = "Estudiante:";
            lblA2.Text = "Grado/Sección:";
            lblA3.Text = "Tema:";
            lblA4.Text = "Medio:";
        }

        private void LimpiarTarjetaCalificaciones()
        {
            lblB1.Text = "Estudiante:";
            lblB2.Text = "Grado/Sección:";
            lblB3.Text = "Tema:";
            lblB4.Text = "Medio:";
        }

        private void LimpiarTarjetaReuniones()
        {
            lblC1.Text = "Estudiante:";
            lblC2.Text = "Grado/Sección:";
            lblC3.Text = "Tema:";
            lblC4.Text = "Medio:";
        }

        private void FormatearGridAsistencia()
        {
            if (dgvInfo.Columns.Count == 0)
                return;

            foreach (DataGridViewColumn col in dgvInfo.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            if (dgvInfo.Columns.Contains("AsistenciaID"))
                dgvInfo.Columns["AsistenciaID"].Visible = false;

            if (dgvInfo.Columns.Contains("EstudianteID"))
                dgvInfo.Columns["EstudianteID"].Visible = false;

            if (dgvInfo.Columns.Contains("Nombre"))
                dgvInfo.Columns["Nombre"].HeaderText = "Estudiante";

            if (dgvInfo.Columns.Contains("Asignatura"))
                dgvInfo.Columns["Asignatura"].HeaderText = "Asignatura";

            if (dgvInfo.Columns.Contains("Fecha"))
                dgvInfo.Columns["Fecha"].HeaderText = "Fecha";

            if (dgvInfo.Columns.Contains("Estado"))
                dgvInfo.Columns["Estado"].HeaderText = "Estado";

            if (dgvInfo.Columns.Contains("Observacion"))
                dgvInfo.Columns["Observacion"].HeaderText = "Observación";
        }

        private void FormatearGridCalificaciones()
        {
            if (dgvInfo.Columns.Count == 0)
                return;

            foreach (DataGridViewColumn col in dgvInfo.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            if (dgvInfo.Columns.Contains("NombreGrado"))
                dgvInfo.Columns["NombreGrado"].HeaderText = "Grado";

            if (dgvInfo.Columns.Contains("Asignatura"))
                dgvInfo.Columns["Asignatura"].HeaderText = "Asignatura";

            if (dgvInfo.Columns.Contains("Parcial"))
                dgvInfo.Columns["Parcial"].HeaderText = "Parcial";

            if (dgvInfo.Columns.Contains("PromedioClase"))
            {
                dgvInfo.Columns["PromedioClase"].HeaderText = "Promedio";
                dgvInfo.Columns["PromedioClase"].DefaultCellStyle.Format = "N2";
            }

            if (dgvInfo.Columns.Contains("Estado"))
                dgvInfo.Columns["Estado"].HeaderText = "Estado";
        }

        private void FormatearGridReuniones()
        {
            if (dgvInfo.Columns.Count == 0)
                return;

            foreach (DataGridViewColumn col in dgvInfo.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            if (dgvInfo.Columns.Contains("FechaHora"))
            {
                dgvInfo.Columns["FechaHora"].HeaderText = "Fecha y hora";
                dgvInfo.Columns["FechaHora"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            }

            if (dgvInfo.Columns.Contains("Nombre"))
                dgvInfo.Columns["Nombre"].HeaderText = "Estudiante";

            if (dgvInfo.Columns.Contains("GradoSeccion"))
                dgvInfo.Columns["GradoSeccion"].HeaderText = "Grado/Sección";

            if (dgvInfo.Columns.Contains("Docente"))
                dgvInfo.Columns["Docente"].HeaderText = "Docente";

            if (dgvInfo.Columns.Contains("Asignatura"))
                dgvInfo.Columns["Asignatura"].HeaderText = "Asignatura";

            if (dgvInfo.Columns.Contains("Tema"))
                dgvInfo.Columns["Tema"].HeaderText = "Tema";

            if (dgvInfo.Columns.Contains("MedioDifusion"))
                dgvInfo.Columns["MedioDifusion"].HeaderText = "Medio";

            if (dgvInfo.Columns.Contains("Estado"))
                dgvInfo.Columns["Estado"].HeaderText = "Estado";
        }

        private void guna2HtmlLabel20_Click(object sender, EventArgs e)
        {
        }
    }
}