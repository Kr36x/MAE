using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmConfigCicloEscolar : Form
    {
        private int paginaActual = 1;
        private int registrosPorPagina = 5;
        private int totalRegistrosGlobal = 0;

        public FrmConfigCicloEscolar()
        {
            InitializeComponent();

            Load += FrmConfigCicloEscolar_Load;

            dgvConfiguracion.CellFormatting += dgvConfiguracion_CellFormatting;
            dgvConfiguracion.CellClick += dgvConfiguracion_CellClick;
            dgvConfiguracion.CellPainting += dgvConfiguracion_CellPainting;
            dgvConfiguracion.CellMouseEnter += dgvConfiguracion_CellMouseEnter;
            dgvConfiguracion.CellMouseLeave += dgvConfiguracion_CellMouseLeave;

            cbbCicloEscolar.SelectedIndexChanged += cbbCicloEscolar_SelectedIndexChanged;
            btnNuevoCicloEscolar.Click += btnNuevoCicloEscolar_Click;
            btnSiguiente.Click += btnSiguiente_Click;
            btnAnterior.Click += btnAnterior_Click;
        }

        private void FrmConfigCicloEscolar_Load(object sender, EventArgs e)
        {
            ConfigurarDGV();
            CargarCiclos();
            CargarConfiguracion();
        }

        private void CargarCiclos()
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                DataTable dt = util.EjecutarSP("spMAE_ListarCiclosEscolares");

                if (dt == null)
                    return;

                DataRow fila = dt.NewRow();
                fila["CicloEscolar"] = "Todos";
                dt.Rows.InsertAt(fila, 0);

                cbbCicloEscolar.SelectedIndexChanged -= cbbCicloEscolar_SelectedIndexChanged;

                cbbCicloEscolar.DataSource = dt;
                cbbCicloEscolar.DisplayMember = "CicloEscolar";
                cbbCicloEscolar.ValueMember = "CicloEscolar";
                cbbCicloEscolar.SelectedIndex = 0;

                cbbCicloEscolar.SelectedIndexChanged += cbbCicloEscolar_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ciclos escolares: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarConfiguracion(string cicloFiltro = "Todos")
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                SqlParameter pCiclo = new SqlParameter("@CicloEscolar", cicloFiltro);
                SqlParameter pPagina = new SqlParameter("@NumeroPagina", paginaActual);
                SqlParameter pRegistros = new SqlParameter("@RegistrosPorPagina", registrosPorPagina);

                SqlParameter pTotal = new SqlParameter("@TotalRegistros", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                SqlParameter[] parametros = { pCiclo, pPagina, pRegistros, pTotal };

                DataTable dt = util.EjecutarSPParametros("spMAE_ListarConfiguracionPaginada", parametros);

                totalRegistrosGlobal = pTotal.Value != DBNull.Value
                    ? Convert.ToInt32(pTotal.Value)
                    : 0;

                dgvConfiguracion.AutoGenerateColumns = false;
                dgvConfiguracion.DataSource = null;

                if (dt == null || dt.Rows.Count == 0)
                {
                    ActualizarEstadoPaginacion();
                    return;
                }

                dgvConfiguracion.DataSource = dt;

                if (dgvConfiguracion.Columns.Contains("ConfigID"))
                    dgvConfiguracion.Columns["ConfigID"].Visible = false;

                if (dgvConfiguracion.Columns.Contains("CicloEscolar"))
                    dgvConfiguracion.Columns["CicloEscolar"].HeaderText = "CICLO ESCOLAR";

                if (dgvConfiguracion.Columns.Contains("Periodo"))
                    dgvConfiguracion.Columns["Periodo"].HeaderText = "PERIODO";

                if (dgvConfiguracion.Columns.Contains("FechaInicio"))
                    dgvConfiguracion.Columns["FechaInicio"].HeaderText = "INICIO DE PERIODO";

                if (dgvConfiguracion.Columns.Contains("FechaFin"))
                    dgvConfiguracion.Columns["FechaFin"].HeaderText = "FIN DE PERIODO";

                if (dgvConfiguracion.Columns.Contains("Activa"))
                    dgvConfiguracion.Columns["Activa"].HeaderText = "ESTADO";

                if (!dgvConfiguracion.Columns.Contains("Acciones"))
                {
                    DataGridViewTextBoxColumn colAcciones = new DataGridViewTextBoxColumn
                    {
                        Name = "Acciones",
                        HeaderText = "",
                        Width = 80
                    };

                    dgvConfiguracion.Columns.Add(colAcciones);
                }

                foreach (DataGridViewRow row in dgvConfiguracion.Rows)
                {
                    if (row.Cells["Acciones"] != null)
                        row.Cells["Acciones"].Value = "EDITAR";
                }

                foreach (DataGridViewColumn col in dgvConfiguracion.Columns)
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                if (dgvConfiguracion.Columns.Contains("CicloEscolar"))
                    dgvConfiguracion.Columns["CicloEscolar"].Width = 170;

                if (dgvConfiguracion.Columns.Contains("Periodo"))
                    dgvConfiguracion.Columns["Periodo"].Width = 110;

                if (dgvConfiguracion.Columns.Contains("FechaInicio"))
                    dgvConfiguracion.Columns["FechaInicio"].Width = 220;

                if (dgvConfiguracion.Columns.Contains("FechaFin"))
                    dgvConfiguracion.Columns["FechaFin"].Width = 220;

                if (dgvConfiguracion.Columns.Contains("Activa"))
                    dgvConfiguracion.Columns["Activa"].Width = 150;

                if (dgvConfiguracion.Columns.Contains("Acciones"))
                    dgvConfiguracion.Columns["Acciones"].Width = 80;

                ActualizarEstadoPaginacion();
                dgvConfiguracion.ClearSelection();
                dgvConfiguracion.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar configuración: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarDGV()
        {
            dgvConfiguracion.AutoGenerateColumns = false;
            dgvConfiguracion.AllowUserToAddRows = false;
            dgvConfiguracion.AllowUserToDeleteRows = false;
            dgvConfiguracion.AllowUserToResizeRows = false;
            dgvConfiguracion.AllowUserToResizeColumns = false;
            dgvConfiguracion.MultiSelect = false;
            dgvConfiguracion.ReadOnly = true;
            dgvConfiguracion.RowHeadersVisible = false;
            dgvConfiguracion.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvConfiguracion.EnableHeadersVisualStyles = false;
            dgvConfiguracion.BorderStyle = BorderStyle.None;
            dgvConfiguracion.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvConfiguracion.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvConfiguracion.BackgroundColor = Color.White;
            dgvConfiguracion.GridColor = Color.FromArgb(220, 220, 220);

            dgvConfiguracion.ColumnHeadersHeight = 42;
            dgvConfiguracion.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvConfiguracion.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(14, 102, 248);
            dgvConfiguracion.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvConfiguracion.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvConfiguracion.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvConfiguracion.DefaultCellStyle.BackColor = Color.White;
            dgvConfiguracion.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgvConfiguracion.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            dgvConfiguracion.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 242, 255);
            dgvConfiguracion.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);
            dgvConfiguracion.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvConfiguracion.RowsDefaultCellStyle.BackColor = Color.White;
            dgvConfiguracion.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 252);
            dgvConfiguracion.RowTemplate.Height = 50;
        }

        private string ObtenerEstado(bool activa, DateTime fechaFin)
        {
            DateTime hoy = DateTime.Today;

            if (activa && fechaFin >= hoy)
                return "ACTIVO";

            if (!activa && fechaFin < hoy)
                return "CERRADO";

            return "PRÓXIMO";
        }
        private string ObtenerEstadoFila(DataGridViewRow row)
        {
            object valorActiva = row.Cells["Activa"].Value;
            object valorFechaFin = row.Cells["FechaFin"].Value;

            if (valorActiva == null || valorActiva == DBNull.Value ||
                valorFechaFin == null || valorFechaFin == DBNull.Value)
                return "";

            if (valorActiva is bool activa)
            {
                DateTime fechaFin = Convert.ToDateTime(valorFechaFin);
                return ObtenerEstado(activa, fechaFin);
            }

            return valorActiva.ToString()?.Trim().ToUpper() ?? "";
        }
        private void ActualizarEstadoPaginacion()
        {
            int desde = totalRegistrosGlobal == 0 ? 0 : ((paginaActual - 1) * registrosPorPagina) + 1;
            int hasta = Math.Min(paginaActual * registrosPorPagina, totalRegistrosGlobal);

            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosGlobal / registrosPorPagina);
            if (totalPaginas == 0)
                totalPaginas = 1;

            Color grisNormal = Color.FromArgb(64, 64, 64);
            Color grisDeshabilitado = Color.FromArgb(220, 220, 220);

            btnAnterior.Enabled = paginaActual > 1;
            btnAnterior.ForeColor = btnAnterior.Enabled ? grisNormal : grisDeshabilitado;
            btnAnterior.FillColor = Color.White;
            btnAnterior.BackColor = Color.White;
            btnAnterior.DisabledState.FillColor = Color.White;
            btnAnterior.DisabledState.ForeColor = grisDeshabilitado;
            btnAnterior.DisabledState.BorderColor = Color.White;

            btnSiguiente.Enabled = paginaActual < totalPaginas;
            btnSiguiente.ForeColor = btnSiguiente.Enabled ? grisNormal : grisDeshabilitado;
            btnSiguiente.FillColor = Color.White;
            btnSiguiente.BackColor = Color.White;
            btnSiguiente.DisabledState.FillColor = Color.White;
            btnSiguiente.DisabledState.ForeColor = grisDeshabilitado;
            btnSiguiente.DisabledState.BorderColor = Color.White;

            lblInfoRegistro.Text = totalRegistrosGlobal == 0
                ? "Sin registros"
                : $"Registros del {desde} al {hasta}. Total de registros: {totalRegistrosGlobal}";

            lblNumeroPagina.Text = paginaActual.ToString();
        }

        private GraphicsPath RedondearRectangulo(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void btnNuevoCicloEscolar_Click(object sender, EventArgs e)
        {
            using FrmCreaEditaConfig frm = new FrmCreaEditaConfig(ModoOperacion.Crear);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                paginaActual = 1;
                CargarCiclos();
                CargarConfiguracion(cbbCicloEscolar.SelectedValue?.ToString() ?? "Todos");
            }
        }

        private void cbbCicloEscolar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbCicloEscolar.SelectedValue == null)
                return;

            paginaActual = 1;
            string cicloSeleccionado = cbbCicloEscolar.SelectedValue.ToString();
            CargarConfiguracion(cicloSeleccionado);
        }

        private void dgvConfiguracion_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvConfiguracion.Columns[e.ColumnIndex].Name == "Activa")
            {
                object valorActiva = dgvConfiguracion.Rows[e.RowIndex].Cells["Activa"].Value;
                object valorFechaFin = dgvConfiguracion.Rows[e.RowIndex].Cells["FechaFin"].Value;

                if (valorActiva == null || valorActiva == DBNull.Value ||
                    valorFechaFin == null || valorFechaFin == DBNull.Value)
                    return;

                if (valorActiva is bool activa)
                {
                    DateTime fechaFin = Convert.ToDateTime(valorFechaFin);
                    e.Value = ObtenerEstado(activa, fechaFin);
                    e.FormattingApplied = true;
                }
            }
        }

        private void dgvConfiguracion_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string colName = dgvConfiguracion.Columns[e.ColumnIndex].Name;

            if (colName == "Acciones")
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                DataGridViewRow row = dgvConfiguracion.Rows[e.RowIndex];
                string estado = ObtenerEstadoFila(row);

                bool estaCerrado = estado == "CERRADO";

                Color btnBack = estaCerrado
                    ? Color.FromArgb(239, 68, 68)
                    : Color.FromArgb(14, 102, 248);

                Color btnBorder = estaCerrado
                    ? Color.FromArgb(239, 68, 68)
                    : Color.FromArgb(14, 102, 248);

                Rectangle btnRect = new Rectangle(
                    e.CellBounds.X + 16,
                    e.CellBounds.Y + 8,
                    e.CellBounds.Width - 32,
                    e.CellBounds.Height - 16
                );

                using (GraphicsPath path = RedondearRectangulo(btnRect, 6))
                using (SolidBrush brush = new SolidBrush(btnBack))
                using (Pen pen = new Pen(btnBorder))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }

                Image icono = estaCerrado
                    ? GestionAcademicaV2.Properties.Resources.lock_white
                    : GestionAcademicaV2.Properties.Resources.edit_white;

                int iconSize = 16;
                Rectangle iconRect = new Rectangle(
                    btnRect.X + (btnRect.Width - iconSize) / 2,
                    btnRect.Y + (btnRect.Height - iconSize) / 2,
                    iconSize,
                    iconSize
                );

                e.Graphics.DrawImage(icono, iconRect);

                using Pen linePen = new Pen(dgvConfiguracion.GridColor);
                e.Graphics.DrawLine(
                    linePen,
                    e.CellBounds.Left,
                    e.CellBounds.Bottom - 1,
                    e.CellBounds.Right,
                    e.CellBounds.Bottom - 1
                );
            }
        }

        private void dgvConfiguracion_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvConfiguracion.Columns[e.ColumnIndex].Name == "Acciones")
            {
                DataGridViewRow row = dgvConfiguracion.Rows[e.RowIndex];
                string estado = ObtenerEstadoFila(row);

                if (estado == "CERRADO")
                {
                    MessageBox.Show("No se puede editar un período cerrado.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idConfig = Convert.ToInt32(row.Cells["ConfigID"].Value);

                using FrmCreaEditaConfig frm = new FrmCreaEditaConfig(ModoOperacion.Editar, idConfig);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CargarCiclos();
                    CargarConfiguracion(cbbCicloEscolar.SelectedValue?.ToString() ?? "Todos");
                }
            }
        }

        private void dgvConfiguracion_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                dgvConfiguracion.Cursor = Cursors.Default;
                return;
            }

            if (dgvConfiguracion.Columns[e.ColumnIndex].Name == "Acciones")
            {
                string estado = ObtenerEstadoFila(dgvConfiguracion.Rows[e.RowIndex]);
                dgvConfiguracion.Cursor = estado == "CERRADO" ? Cursors.Default : Cursors.Hand;
                return;
            }

            dgvConfiguracion.Cursor = Cursors.Default;
        }

        private void dgvConfiguracion_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvConfiguracion.Cursor = Cursors.Default;
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosGlobal / registrosPorPagina);
            if (totalPaginas == 0)
                totalPaginas = 1;

            if (paginaActual < totalPaginas)
            {
                paginaActual++;
                CargarConfiguracion(cbbCicloEscolar.SelectedValue?.ToString() ?? "Todos");
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                CargarConfiguracion(cbbCicloEscolar.SelectedValue?.ToString() ?? "Todos");
            }
        }

        private void lblInfoRegistro_Click(object sender, EventArgs e)
        {
        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }
    }
}