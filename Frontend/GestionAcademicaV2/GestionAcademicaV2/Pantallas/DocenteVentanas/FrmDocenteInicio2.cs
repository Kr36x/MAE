using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmDocenteInicio2 : Form
    {
        private readonly int docenteId;
        private readonly Conexion conexion = new Conexion();

        private int _docenteIdReal = 0;
        private int _anioActivo = 0;
        private string _cicloTexto = "";
        private DataTable _dtCargas = new DataTable();

        private readonly Color colorFondo = Color.FromArgb(242, 244, 247);
        private readonly Color colorTarjeta = Color.White;
        private readonly Color colorBorde = Color.FromArgb(224, 224, 224);
        private readonly Color colorVerde = Color.FromArgb(0, 128, 0);
        private readonly Color colorVerdeOscuro = Color.FromArgb(0, 102, 0);
        private readonly Color colorAzul = Color.FromArgb(24, 105, 255);
        private readonly Color colorTexto = Color.FromArgb(35, 35, 35);
        private readonly Color colorTextoSuave = Color.FromArgb(110, 110, 110);
        private readonly Color colorDorado = Color.FromArgb(212, 163, 52);

        private readonly int usuarioId;
        private int docenteIdReal = 0;
        public FrmDocenteInicio2(int docenteId)
        {
            InitializeComponent();
            this.docenteId = docenteId;
            Load += FrmDocenteInicio2_Load;
        }

        private void FrmDocenteInicio2_Load(object sender, EventArgs e)
        {
            try
            {
                docenteIdReal = ObtenerDocenteIdPorUsuarioId(docenteId);
                ConfigurarVista();
                InicializarContextoDocente();
                CargarDashboardDesdeSP();
                ConectarBotones();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar el inicio docente: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            BackColor = colorFondo;
            pnlMainDashboard.BackColor = colorFondo;
            ConfigurarVistaBonita();
            //MessageBox.Show($"FrmDocenteInicio2 recibió docenteId = {docenteId}");
        }

        #region CONFIGURACION UI
        private int ObtenerDocenteIdPorUsuarioId(int usuarioId)
        {
            int docenteId = 0;

            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
            SELECT DocenteID
            FROM Docente
            WHERE UsuarioID = @UsuarioID;", cn);

                cmd.Parameters.AddWithValue("@UsuarioID", usuarioId);

                cn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    docenteId = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo resolver el DocenteID desde el UsuarioID.\n\n" + ex.Message,
                    "Sistema MAE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }

            return docenteId;
        }
        private void ConfigurarVista()
        {
            BackColor = colorFondo;
            pnlMainDashboard.BackColor = colorFondo;

            ConfigurarCards();
            ConfigurarGridAsignaturas();
            ConfigurarProgresos();
            ConfigurarEtiquetasBase();
        }

        private void ConfigurarVistaBonita()
        {
            AplicarEstiloCard(guna2Panel3);
            AplicarEstiloCard(cardAsignaturas);
            AplicarEstiloCard(cardSecciones);
            AplicarEstiloCard(cardReuniones);
            AplicarEstiloCard(cardAvanceParcial);
            AplicarEstiloCard(guna2Panel4);
            AplicarEstiloCard(guna2Panel6);
            AplicarEstiloCard(guna2Panel15);
            AplicarEstiloCard(guna2Panel14);

            EstilizarHeaderPrincipal();
            EstilizarKpis();
            EstilizarSeccionReunion();
            EstilizarGridAsignaturas();
            EstilizarBloquesResumen();
        }

        private void AplicarEstiloCard(Guna.UI2.WinForms.Guna2Panel panel)
        {
            panel.FillColor = colorTarjeta;
            panel.BorderColor = colorBorde;
            panel.BorderThickness = 1;
            panel.BorderRadius = 7;
            panel.ShadowDecoration.Enabled = true;
            panel.ShadowDecoration.BorderRadius = 7;
            panel.ShadowDecoration.Depth = 12;
            panel.ShadowDecoration.Color = Color.FromArgb(40, 0, 0, 0);
        }

        private void ConfigurarCards()
        {
            var cards = new[]
            {
                cardAsignaturas, cardSecciones, cardReuniones, cardAvanceParcial,
                guna2Panel4, guna2Panel6, guna2Panel15, guna2Panel14
            };

            foreach (var card in cards)
            {
                card.FillColor = Color.White;
                card.BorderColor = Color.FromArgb(220, 220, 220);
                card.BorderThickness = 1;
                card.BorderRadius = 10;
            }

            guna2Panel3.FillColor = Color.White;
            guna2Panel3.BorderColor = Color.FromArgb(220, 220, 220);
            guna2Panel3.BorderThickness = 1;
            guna2Panel3.BorderRadius = 12;
        }

        private void EstilizarHeaderPrincipal()
        {
            guna2Panel3.FillColor = Color.White;
            guna2Panel3.BorderColor = colorBorde;
            guna2Panel3.BorderThickness = 1;
            guna2Panel3.BorderRadius = 8;

            lblTituloDashboard.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTituloDashboard.ForeColor = colorTexto;

            lblSaludoDocente.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            lblSaludoDocente.ForeColor = colorTexto;

            lblFechaActual.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblFechaActual.ForeColor = colorTexto;

            lblHoraActual.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblHoraActual.ForeColor = colorTextoSuave;

            guna2HtmlLabel8.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            guna2HtmlLabel8.ForeColor = colorTextoSuave;
        }

        private void EstilizarKpis()
        {
            EstilizarKpi(lblTituloAsignaturas, lblValorAsignaturas, lblSubAsignaturas, colorVerde);
            EstilizarKpi(lblTituloSecciones, lblValorSecciones, lblSubSecciones, colorAzul);
            EstilizarKpi(lblTituloReuniones, lblValorReuniones, lblSubReuniones, colorDorado);
            EstilizarKpi(lblTituloAvance, lblValorAvance, lblSubAvance, colorVerdeOscuro);
        }

        private void EstilizarKpi(
            Guna.UI2.WinForms.Guna2HtmlLabel lblTitulo,
            Guna.UI2.WinForms.Guna2HtmlLabel lblValor,
            Guna.UI2.WinForms.Guna2HtmlLabel lblSub,
            Color colorValor)
        {
            lblTitulo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTitulo.ForeColor = colorTextoSuave;

            lblValor.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblValor.ForeColor = colorValor;

            lblSub.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblSub.ForeColor = colorTextoSuave;
        }

        private void EstilizarSeccionReunion()
        {
            var labels = new[]
            {
                lblReunionFechaValor,
                lblReunionHoraValor,
                lblReunionEstudianteValor,
                lblReunionGradoValor,
                lblReunionTemaValor,
                lblReunionMedioValor
            };

            foreach (var lbl in labels)
            {
                lbl.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
                lbl.ForeColor = colorTexto;
                lbl.BackColor = Color.Transparent;
            }

            btnVerReuniones.BorderRadius = 8;
            btnVerReuniones.FillColor = colorVerde;
            btnVerReuniones.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnVerReuniones.ForeColor = Color.White;
        }

        private void EstilizarGridAsignaturas()
        {
            dgvAsignaturas.ThemeStyle.BackColor = Color.White;
            dgvAsignaturas.BackgroundColor = Color.White;
            dgvAsignaturas.BorderStyle = BorderStyle.None;
            dgvAsignaturas.GridColor = Color.FromArgb(230, 230, 230);

            dgvAsignaturas.ColumnHeadersHeight = 34;
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.BackColor = colorVerdeOscuro;
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.SelectionBackColor = colorVerdeOscuro;
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvAsignaturas.DefaultCellStyle.BackColor = Color.White;
            dgvAsignaturas.DefaultCellStyle.ForeColor = colorTexto;
            dgvAsignaturas.DefaultCellStyle.Font = new Font("Segoe UI", 9.2F);
            dgvAsignaturas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 248, 255);
            dgvAsignaturas.DefaultCellStyle.SelectionForeColor = colorTexto;
            dgvAsignaturas.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
            dgvAsignaturas.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgvAsignaturas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvAsignaturas.RowTemplate.Height = 28;
            dgvAsignaturas.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvAsignaturas.EnableHeadersVisualStyles = false;

            if (dgvAsignaturas.Columns.Contains("Asignatura"))
                dgvAsignaturas.Columns["Asignatura"].FillWeight = 38;

            if (dgvAsignaturas.Columns.Contains("Secciones"))
                dgvAsignaturas.Columns["Secciones"].FillWeight = 38;

            if (dgvAsignaturas.Columns.Contains("TotalSecciones"))
                dgvAsignaturas.Columns["TotalSecciones"].FillWeight = 25;
        }

        private void EstilizarBloquesResumen()
        {
            lblParcialActivo.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblActividadesCreadas.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblTotalAcumulado.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblRestante.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            lblActividadesPlan.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblActividadesCalificadas.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblEstudiantesEvaluados.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            lblPorcentajeActividades.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPorcentajeActividades.ForeColor = colorVerdeOscuro;

            lblPorcentajeEvaluacion.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPorcentajeEvaluacion.ForeColor = colorDorado;

            prgActividades.Height = 18;
            prgActividades.BorderRadius = 9;

            prgEvaluacion.Height = 18;
            prgEvaluacion.BorderRadius = 9;

            guna2Button1.BorderRadius = 8;
            guna2Button1.FillColor = colorVerde;

            btnIrCalificaciones.BorderRadius = 8;
            btnIrCalificaciones.FillColor = colorVerde;
        }

        private void ConfigurarGridAsignaturas()
        {
            dgvAsignaturas.Columns.Clear();
            dgvAsignaturas.Rows.Clear();

            dgvAsignaturas.AllowUserToAddRows = false;
            dgvAsignaturas.AllowUserToDeleteRows = false;
            dgvAsignaturas.AllowUserToResizeRows = false;
            dgvAsignaturas.AllowUserToResizeColumns = false;
            dgvAsignaturas.ReadOnly = true;
            dgvAsignaturas.MultiSelect = false;
            dgvAsignaturas.RowHeadersVisible = false;
            dgvAsignaturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAsignaturas.BackgroundColor = Color.White;
            dgvAsignaturas.BorderStyle = BorderStyle.None;
            dgvAsignaturas.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvAsignaturas.EnableHeadersVisualStyles = false;
            dgvAsignaturas.GridColor = Color.FromArgb(230, 230, 230);
            dgvAsignaturas.ColumnHeadersHeight = 34;
            dgvAsignaturas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvAsignaturas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAsignaturas.ScrollBars = ScrollBars.Vertical;
            dgvAsignaturas.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dgvAsignaturas.DefaultCellStyle.BackColor = Color.White;
            dgvAsignaturas.DefaultCellStyle.ForeColor = colorTexto;
            dgvAsignaturas.DefaultCellStyle.Font = new Font("Segoe UI", 9.2F, FontStyle.Regular);
            dgvAsignaturas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 248, 255);
            dgvAsignaturas.DefaultCellStyle.SelectionForeColor = colorTexto;
            dgvAsignaturas.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
            dgvAsignaturas.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgvAsignaturas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 252);
            dgvAsignaturas.RowTemplate.Height = 34;

            var colAsignatura = new DataGridViewTextBoxColumn
            {
                Name = "Asignatura",
                HeaderText = "ASIGNATURA",
                ReadOnly = true,
                FillWeight = 38
            };

            var colSecciones = new DataGridViewTextBoxColumn
            {
                Name = "Secciones",
                HeaderText = "SECCIONES",
                ReadOnly = true,
                FillWeight = 38,
                DefaultCellStyle = { WrapMode = DataGridViewTriState.True }
            };

            var colTotal = new DataGridViewTextBoxColumn
            {
                Name = "TotalSecciones",
                HeaderText = "TOTAL",
                ReadOnly = true,
                FillWeight = 25
            };

            colTotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvAsignaturas.Columns.Add(colAsignatura);
            dgvAsignaturas.Columns.Add(colSecciones);
            dgvAsignaturas.Columns.Add(colTotal);

            foreach (DataGridViewColumn col in dgvAsignaturas.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void ConfigurarProgresos()
        {
            prgActividades.Minimum = 0;
            prgActividades.Maximum = 100;
            prgActividades.Value = 0;
            prgActividades.FillColor = Color.FromArgb(225, 225, 225);
            prgActividades.ProgressColor = Color.FromArgb(42, 168, 74);
            prgActividades.ProgressColor2 = Color.FromArgb(92, 184, 92);

            prgEvaluacion.Minimum = 0;
            prgEvaluacion.Maximum = 100;
            prgEvaluacion.Value = 0;
            prgEvaluacion.FillColor = Color.FromArgb(225, 225, 225);
            prgEvaluacion.ProgressColor = Color.Goldenrod;
            prgEvaluacion.ProgressColor2 = Color.Khaki;
        }

        private void ConfigurarEtiquetasBase()
        {
            lblTituloDashboard.Text = "INICIO DOCENTE";
            lblSaludoDocente.Text = "Bienvenido, docente.";

            lblTituloAsignaturas.Text = "ASIGNATURAS";
            lblTituloSecciones.Text = "SECCIONES";
            lblTituloReuniones.Text = "REUNIONES";
            lblTituloAvance.Text = "AVANCE PARCIAL";

            lblSubAsignaturas.Text = "asignadas";
            lblSubSecciones.Text = "activas";
            lblSubReuniones.Text = "pendientes";
            lblSubAvance.Text = "planificado";

            lblReunionFechaValor.Text = "<b>Fecha:</b> Sin datos";
            lblReunionHoraValor.Text = "<b>Hora:</b> Sin datos";
            lblReunionEstudianteValor.Text = "<b>Estudiante:</b> Sin datos";
            lblReunionGradoValor.Text = "<b>Grado/Sección:</b> Sin datos";
            lblReunionTemaValor.Text = "<b>Tema:</b> Sin datos";
            lblReunionMedioValor.Text = "<b>Medio:</b> Sin datos";

            lblParcialActivo.Text = "<b>Parcial activo:</b> -";
            lblActividadesCreadas.Text = "<b>Actividades creadas:</b> 0";
            lblTotalAcumulado.Text = "<b>Total acumulado:</b> 0.00 / 100.00";
            lblRestante.Text = "<b>Restante:</b> 100.00";
            lblPorcentajeActividades.Text = "0%";

            lblActividadesPlan.Text = "<b>Actividades planificadas:</b> 0";
            lblActividadesCalificadas.Text = "<b>Actividades calificadas:</b> 0";
            lblEstudiantesEvaluados.Text = "<b>Estudiantes evaluados:</b> 0 / 0";
            lblPorcentajeEvaluacion.Text = "0%";
        }

        #endregion

        #region DASHBOARD SP

        private void InicializarContextoDocente()
        {
            _docenteIdReal = docenteId;
            _cicloTexto = "";
            _anioActivo = 0;
        }
        private bool TieneColumna(IDataRecord reader, string nombreColumna)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), nombreColumna, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        private void CargarDashboardDesdeSP()
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_DashboardDocente", cn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DocenteID", docenteId);
            cmd.Parameters.AddWithValue("@Anio", 0);

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            cn.Open();
            da.Fill(ds);

            if (ds.Tables.Count < 6)
                throw new Exception("El procedimiento no devolvió todos los bloques esperados del dashboard.");

            DataTable dtHeader = ds.Tables[0];
            DataTable dtKpi = ds.Tables[1];
            DataTable dtReunion = ds.Tables[2];
            DataTable dtAsignaturasRaw = ds.Tables[3];
            DataTable dtResumen = ds.Tables[4];
            DataTable dtEvaluacion = ds.Tables[5];

            // HEADER
            if (dtHeader.Rows.Count > 0)
            {
                DataRow row = dtHeader.Rows[0];

                _docenteIdReal = row["DocenteID"] == DBNull.Value ? docenteId : Convert.ToInt32(row["DocenteID"]);
                _cicloTexto = row["CicloEscolar"]?.ToString() ?? "";
                _anioActivo = row["AnioActivo"] == DBNull.Value ? DateTime.Today.Year : Convert.ToInt32(row["AnioActivo"]);

                string nombre = row["NombreDocente"]?.ToString() ?? "Docente";

                lblSaludoDocente.Text = $"Bienvenido, {nombre}";
                //lblSaludoDocente.Text = $"Bienvenido, {nombre} (Usr:{docenteId} / Doc:{_docenteIdReal})";
                lblFechaActual.Text = $"Fecha: {DateTime.Now:dd/MM/yyyy}";
                lblHoraActual.Text = $"Hora: {DateTime.Now:hh:mm tt}".ToLower();
                guna2HtmlLabel8.Text = $"Ciclo: {_cicloTexto}";
            }

            // KPI
            if (dtKpi.Rows.Count > 0)
            {
                DataRow row = dtKpi.Rows[0];

                int totalAsignaturas = row["TotalAsignaturas"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalAsignaturas"]);
                int totalSecciones = row["TotalSecciones"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalSecciones"]);
                int totalReunionesPendientes = row["TotalReunionesPendientes"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalReunionesPendientes"]);
                decimal avanceParcial = row["AvanceParcial"] == DBNull.Value ? 0 : Convert.ToDecimal(row["AvanceParcial"]);

                lblValorAsignaturas.Text = totalAsignaturas.ToString();
                lblValorSecciones.Text = totalSecciones.ToString();
                lblValorReuniones.Text = totalReunionesPendientes.ToString();
                lblValorAvance.Text = $"{avanceParcial:0.#}%";

                lblSubAsignaturas.Text = totalAsignaturas == 1 ? "asignada" : "asignadas";
                lblSubSecciones.Text = totalSecciones == 1 ? "activa" : "activas";
                lblSubReuniones.Text = totalReunionesPendientes == 1 ? "pendiente" : "pendientes";
                lblSubAvance.Text = avanceParcial >= 100 ? "completado" : "planificado";
            }

            // PRÓXIMA REUNIÓN
            if (dtReunion.Rows.Count > 0)
            {
                DataRow row = dtReunion.Rows[0];
                DateTime fechaHora = Convert.ToDateTime(row["FechaHora"]);

                lblReunionFechaValor.Text = $"<b>Fecha:</b> {fechaHora:dd/MM/yyyy}";
                lblReunionHoraValor.Text = $"<b>Hora:</b> {fechaHora:hh:mm tt}".ToLower();
                lblReunionEstudianteValor.Text = $"<b>Estudiante:</b> {FormatoTituloNormal(row["Estudiante"]?.ToString())}";
                lblReunionGradoValor.Text = $"<b>Grado/Sección:</b> {FormatearGradoSeccionCompleto(row["NombreGrado"]?.ToString() ?? "", row["Seccion"]?.ToString() ?? "")}";
                lblReunionTemaValor.Text = $"<b>Tema:</b> {FormatoTituloNormal(row["Tema"]?.ToString())}";
                lblReunionMedioValor.Text = $"<b>Medio:</b> {FormatoTituloNormal(row["MedioDifusion"]?.ToString())}";
            }
            else
            {
                lblReunionFechaValor.Text = "<b>Fecha:</b> Sin reunión programada";
                lblReunionHoraValor.Text = "<b>Hora:</b> -";
                lblReunionEstudianteValor.Text = "<b>Estudiante:</b> -";
                lblReunionGradoValor.Text = "<b>Grado/Sección:</b> -";
                lblReunionTemaValor.Text = "<b>Tema:</b> -";
                lblReunionMedioValor.Text = "<b>Medio:</b> -";
            }

            // ASIGNATURAS
            _dtCargas = dtAsignaturasRaw.Copy();
            dgvAsignaturas.Rows.Clear();

            if (dtAsignaturasRaw.Rows.Count > 0)
            {
                var agrupado = dtAsignaturasRaw.AsEnumerable()
                    .GroupBy(r => new
                    {
                        AsignaturaID = Convert.ToInt32(r["AsignaturaID"]),
                        Asignatura = r["Asignatura"]?.ToString() ?? ""
                    })
                    .Select(g =>
                    {
                        var seccionesCortas = g.Select(x => FormatearGradoSeccionCorto(
                                x["Grado"]?.ToString() ?? "",
                                x["Seccion"]?.ToString() ?? ""))
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();

                        return new
                        {
                            Asignatura = FormatoTituloNormal(g.Key.Asignatura),
                            Secciones = ConstruirTextoSeccionesResumen(seccionesCortas),
                            Total = seccionesCortas.Count
                        };
                    })
                    .OrderBy(x => x.Asignatura)
                    .ToList();

                foreach (var item in agrupado)
                {
                    int rowIndex = dgvAsignaturas.Rows.Add(item.Asignatura, item.Secciones, item.Total);
                    dgvAsignaturas.Rows[rowIndex].Tag = item.Asignatura;
                }
            }

            dgvAsignaturas.ClearSelection();

            // RESUMEN ACTIVIDADES
            if (dtResumen.Rows.Count > 0)
            {
                DataRow row = dtResumen.Rows[0];

                int parcialActual = row["ParcialActual"] == DBNull.Value ? 0 : Convert.ToInt32(row["ParcialActual"]);
                int actividadesCreadas = row["ActividadesCreadas"] == DBNull.Value ? 0 : Convert.ToInt32(row["ActividadesCreadas"]);
                decimal totalAcumulado = row["TotalAcumulado"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalAcumulado"]);
                decimal restante = row["Restante"] == DBNull.Value ? 100 : Convert.ToDecimal(row["Restante"]);

                lblParcialActivo.Text = $"<b>Parcial activo:</b> {ConvertirParcialARomano(parcialActual)}";
                lblActividadesCreadas.Text = $"<b>Actividades creadas:</b> {actividadesCreadas}";
                lblTotalAcumulado.Text = $"<b>Total acumulado:</b> {totalAcumulado:N2} / 100.00";
                lblRestante.Text = $"<b>Restante:</b> {restante:N2}";
                lblPorcentajeActividades.Text = $"{totalAcumulado:0.#}%";

                int valorBarra = Convert.ToInt32(Math.Round(totalAcumulado, 0));
                valorBarra = Math.Max(0, Math.Min(100, valorBarra));
                prgActividades.Value = valorBarra;

                lblValorAvance.Text = $"{totalAcumulado:0.#}%";
            }

            // AVANCE EVALUACIÓN
            if (dtEvaluacion.Rows.Count > 0)
            {
                DataRow row = dtEvaluacion.Rows[0];

                int actividadesPlanificadas = row["ActividadesPlanificadas"] == DBNull.Value ? 0 : Convert.ToInt32(row["ActividadesPlanificadas"]);
                int actividadesCalificadas = row["ActividadesCalificadas"] == DBNull.Value ? 0 : Convert.ToInt32(row["ActividadesCalificadas"]);
                int estudiantesEvaluados = row["EstudiantesEvaluados"] == DBNull.Value ? 0 : Convert.ToInt32(row["EstudiantesEvaluados"]);
                int estudiantesTotales = row["EstudiantesTotales"] == DBNull.Value ? 0 : Convert.ToInt32(row["EstudiantesTotales"]);
                decimal porcentajeEvaluacion = row["PorcentajeEvaluacion"] == DBNull.Value ? 0 : Convert.ToDecimal(row["PorcentajeEvaluacion"]);

                lblActividadesPlan.Text = $"<b>Actividades planificadas:</b> {actividadesPlanificadas}";
                lblActividadesCalificadas.Text = $"<b>Actividades calificadas:</b> {actividadesCalificadas}";
                lblEstudiantesEvaluados.Text = $"<b>Estudiantes evaluados:</b> {estudiantesEvaluados} / {estudiantesTotales}";
                lblPorcentajeEvaluacion.Text = $"{porcentajeEvaluacion:0.#}%";

                int valorBarra = Convert.ToInt32(Math.Round(porcentajeEvaluacion, 0));
                valorBarra = Math.Max(0, Math.Min(100, valorBarra));
                prgEvaluacion.Value = valorBarra;
            }
        }

        #endregion

        #region HELPERS

        private string FormatearGradoSeccionCorto(string grado, string seccion)
        {
            string gradoLimpio = (grado ?? "").Trim().ToUpper();
            string seccionLimpia = (seccion ?? "").Trim().ToUpper();

            string gradoCorto = gradoLimpio switch
            {
                "KINDER" => "Kinder",
                "PREBASICA" => "Prebásica",
                "PRIMERO" => "1ero",
                "SEGUNDO" => "2do",
                "TERCERO" => "3ero",
                "CUARTO" => "4to",
                "QUINTO" => "5to",
                "SEXTO" => "6to",
                "SEPTIMO" => "7mo",
                "OCTAVO" => "8vo",
                "NOVENO" => "9no",
                "DECIMO" => "10mo",
                "UNDECIMO" => "11vo",
                _ => FormatoTituloNormal(grado)
            };

            if (string.IsNullOrWhiteSpace(seccionLimpia))
                return gradoCorto;

            return $"{gradoCorto} - {seccionLimpia}";
        }

        private string FormatearGradoSeccionCompleto(string grado, string seccion)
        {
            string gradoTexto = FormatoTituloNormal(grado);
            string seccionTexto = (seccion ?? "").Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(seccionTexto))
                return gradoTexto;

            return $"{gradoTexto} - {seccionTexto}";
        }

        private string FormatoTituloNormal(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "-";

            texto = texto.Trim().ToLower();
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto);
        }

        private string ConstruirTextoSeccionesResumen(List<string> secciones)
        {
            if (secciones == null || secciones.Count == 0)
                return "-";

            if (secciones.Count <= 3)
                return string.Join(Environment.NewLine, secciones);

            var primeras = secciones.Take(3).ToList();
            int restantes = secciones.Count - 3;

            return string.Join(Environment.NewLine, primeras) + Environment.NewLine + $"+ {restantes} más";
        }

        private string ConvertirParcialARomano(int parcial)
        {
            return parcial switch
            {
                1 => "I",
                2 => "II",
                3 => "III",
                4 => "IV",
                _ => "-"
            };
        }

        #endregion

        #region BOTONES Y DETALLE

        private void ConectarBotones()
        {
            btnVerReuniones.Click += btnVerReuniones_Click;
            guna2Button1.Click += guna2Button1_Click;
            btnIrCalificaciones.Click += btnIrCalificaciones_Click;

            dgvAsignaturas.CellDoubleClick += dgvAsignaturas_CellDoubleClick;
            dgvAsignaturas.KeyDown += dgvAsignaturas_KeyDown;
        }

        private void btnVerReuniones_Click(object sender, EventArgs e)
        {

            AbrirFormularioEnContenedor(new FrmDocenteConsultaReuniones(docenteIdReal));
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnContenedor(new FrmPlanifacacionActividades(docenteIdReal));
        }

        private void btnIrCalificaciones_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnContenedor(new FrmDocenteRegistroCalificaciones(docenteIdReal));
        }

        private void dgvAsignaturas_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            AbrirDetalleAsignaturaDesdeFila(e.RowIndex);
        }

        private void dgvAsignaturas_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && dgvAsignaturas.CurrentRow != null)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                AbrirDetalleAsignaturaDesdeFila(dgvAsignaturas.CurrentRow.Index);
            }
        }

        private void AbrirDetalleAsignaturaDesdeFila(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvAsignaturas.Rows.Count)
                return;

            string asignatura = dgvAsignaturas.Rows[rowIndex].Cells["Asignatura"].Value?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(asignatura))
                return;

            List<string> secciones = ObtenerSeccionesDeAsignatura(asignatura);
            MostrarDetalleAsignatura(asignatura, secciones);
        }

        private List<string> ObtenerSeccionesDeAsignatura(string asignaturaMostrada)
        {
            if (_dtCargas == null || _dtCargas.Rows.Count == 0)
                return new List<string>();

            var resultado = _dtCargas.AsEnumerable()
                .Where(r => FormatoTituloNormal(r["Asignatura"]?.ToString() ?? "") == asignaturaMostrada)
                .Select(r => FormatearGradoSeccionCompleto(
                    r["Grado"]?.ToString() ?? "",
                    r["Seccion"]?.ToString() ?? ""))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return resultado;
        }

        private void MostrarDetalleAsignatura(string asignatura, List<string> secciones)
        {
            using Form frm = new Form();

            frm.Text = "Detalle de asignatura";
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.Size = new Size(540, 450);
            frm.FormBorderStyle = FormBorderStyle.FixedDialog;
            frm.MaximizeBox = false;
            frm.MinimizeBox = false;
            frm.BackColor = Color.White;

            Panel pnlHeader = new Panel
            {
                BackColor = colorVerdeOscuro,
                Dock = DockStyle.Top,
                Height = 52
            };

            Label lblHeader = new Label
            {
                Text = "DETALLE DE ASIGNATURA",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0)
            };

            pnlHeader.Controls.Add(lblHeader);

            Label lblTitulo = new Label
            {
                Text = asignatura,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = colorTexto,
                AutoSize = false,
                Location = new Point(20, 68),
                Size = new Size(480, 30)
            };

            Label lblSub = new Label
            {
                Text = $"Secciones asignadas: {secciones.Count}",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = colorTextoSuave,
                AutoSize = false,
                Location = new Point(20, 100),
                Size = new Size(480, 24)
            };

            ListBox lst = new ListBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 135),
                Size = new Size(480, 220),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = colorTexto
            };

            foreach (string item in secciones)
                lst.Items.Add(item);

            Button btnCerrar = new Button
            {
                Text = "Cerrar",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Size = new Size(100, 34),
                Location = new Point(400, 368),
                BackColor = colorVerde,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => frm.Close();

            frm.Controls.Add(pnlHeader);
            frm.Controls.Add(lblTitulo);
            frm.Controls.Add(lblSub);
            frm.Controls.Add(lst);
            frm.Controls.Add(btnCerrar);

            frm.ShowDialog(this);
        }

        private void AbrirFormularioEnContenedor(Form frm)
        {
            if (Parent == null)
            {
                frm.Show();
                return;
            }

            Control contenedor = Parent;

            contenedor.Controls.Clear();

            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;

            contenedor.Controls.Add(frm);
            contenedor.Tag = frm;
            frm.Show();
        }

        #endregion

        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {
        }

        private void pnlMainDashboard_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}