using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using Guna.UI2.WinForms;


namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{

    public partial class FrmConsolidadoAsignaturas : Form
    {
        private List<ItemGraficoRendimiento> datosGrafico = new List<ItemGraficoRendimiento>();
        private readonly Conexion conexion = new Conexion();
        private DataTable dtGraficoCompleto = new DataTable();
        private int paginaGrafico = 0;
        private const int itemsPorPaginaGrafico = 5;
        private readonly Color azulPrincipal = Color.FromArgb(24, 105, 255);
        private void AplicarColoresUI()
        {
            panelBarraSuperior.BorderColor = azulPrincipal;
            panelBarraGrafico.BorderColor = azulPrincipal;
            panelBarraGrid.BorderColor = azulPrincipal;
            panelBarraSuperior.FillColor = azulPrincipal;
            panelBarraGrafico.FillColor = azulPrincipal;
            panelBarraGrid.FillColor = azulPrincipal;
        }
        public FrmConsolidadoAsignaturas()
        {
            InitializeComponent();
            AplicarColoresUI();
            panelGrafico.Paint += panelGrafico_Paint;
            dgvBoleta.CellPainting += dgvBoleta_CellPainting;
            dgvBoleta.CellClick += dgvBoleta_CellClick;
            dgvBoleta.ScrollBars = ScrollBars.Vertical;
            dgvBoleta.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvBoleta.DefaultCellStyle.SelectionForeColor = Color.FromArgb(45, 45, 45);
            dgvBoleta.RowsDefaultCellStyle.SelectionBackColor = Color.White;
            dgvBoleta.RowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(45, 45, 45);
            dgvBoleta.ColumnHeadersHeight = 50;
        }

        private List<DataRow> ObtenerFilasPaginaGrafico()
        {
            List<DataRow> filas = new List<DataRow>();

            if (dtGraficoCompleto == null || dtGraficoCompleto.Rows.Count == 0)
                return filas;

            int inicio = paginaGrafico * itemsPorPaginaGrafico;
            int fin = Math.Min(inicio + itemsPorPaginaGrafico, dtGraficoCompleto.Rows.Count);

            for (int i = inicio; i < fin; i++)
            {
                filas.Add(dtGraficoCompleto.Rows[i]);
            }

            return filas;
        }

        private void panelGrafico_Paint(object sender, PaintEventArgs e)
        {
            DibujarGraficoManual(e.Graphics, panelGrafico.ClientRectangle);
        }

        public class ItemGraficoRendimiento
        {
            public string Asignatura { get; set; } = "";
            public decimal Promedio { get; set; }
            public string Estado { get; set; } = "";
        }

        private void DibujarGraficoManual(Graphics g, Rectangle areaTotal)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            var filas = ObtenerFilasPaginaGrafico();

            if (filas.Count == 0)
            {
                using Font fontMsg = new Font("Segoe UI", 11, FontStyle.Regular);
                TextRenderer.DrawText(
                    g,
                    "No hay datos para mostrar",
                    fontMsg,
                    areaTotal,
                    Color.Gray,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
                return;
            }

            int margenIzq = 60;
            int margenDer = 25;
            int margenSup = 20;
            int margenInf = 55;

            Rectangle areaGrafico = new Rectangle(
                areaTotal.X + margenIzq,
                areaTotal.Y + margenSup,
                areaTotal.Width - margenIzq - margenDer,
                areaTotal.Height - margenSup - margenInf
            );

            // Grid y eje Y
            using Pen penGrid = new Pen(Color.FromArgb(220, 220, 220), 1);
            using Font fontEje = new Font("Segoe UI", 8);
            using SolidBrush brushTexto = new SolidBrush(Color.Black);

            for (int v = 0; v <= 100; v += 10)
            {
                int y = areaGrafico.Bottom - (int)(areaGrafico.Height * (v / 100f));
                g.DrawLine(penGrid, areaGrafico.Left, y, areaGrafico.Right, y);

                Rectangle rectLabel = new Rectangle(5, y - 8, margenIzq - 10, 16);
                TextRenderer.DrawText(
                    g,
                    v.ToString(),
                    fontEje,
                    rectLabel,
                    Color.Black,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter
                );
            }

            // símbolo %
            using Font fontPercent = new Font("Segoe UI", 10, FontStyle.Bold);
            TextRenderer.DrawText(
                g,
                "%",
                fontPercent,
                new Rectangle(10, areaGrafico.Top + areaGrafico.Height / 2 - 10, 20, 20),
                Color.Black,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );

            int cantidad = filas.Count;
            int espacio = 18;
            int anchoDisponible = areaGrafico.Width - ((cantidad - 1) * espacio);
            int anchoBarra = anchoDisponible / cantidad;

            for (int i = 0; i < cantidad; i++)
            {
                DataRow row = filas[i];

                string asignatura = row["Asignatura"]?.ToString() ?? "";
                decimal promedio = row["PromedioClase"] == DBNull.Value ? 0 : Convert.ToDecimal(row["PromedioClase"]);
                string estado = row["Estado"]?.ToString()?.ToUpper() ?? "";

                Color colorBarra = Color.FromArgb(255, 77, 79);
                if (estado == "EXCELENTE")
                    colorBarra = Color.FromArgb(92, 184, 92);
                else if (estado == "MEDIO")
                    colorBarra = Color.Goldenrod;

                int altoBarra = (int)(areaGrafico.Height * ((float)promedio / 100f));
                int x = areaGrafico.Left + i * (anchoBarra + espacio);
                int y = areaGrafico.Bottom - altoBarra;

                Rectangle rectBarra = new Rectangle(x, y, anchoBarra, altoBarra);

                DibujarBarraRedondeada(g, rectBarra, colorBarra, 22);

                // valor centrado
                using Font fontValor = new Font("Segoe UI", 10, FontStyle.Bold);
                TextRenderer.DrawText(
                    g,
                    promedio.ToString("0"),
                    fontValor,
                    rectBarra,
                    Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                // texto asignatura abajo
                string nombreCorto = asignatura.Length > 18
                    ? asignatura.Substring(0, 15) + "..."
                    : asignatura;

                Rectangle rectTexto = new Rectangle(x, areaGrafico.Bottom + 6, anchoBarra, 34);
                TextRenderer.DrawText(
                    g,
                    nombreCorto,
                    new Font("Segoe UI", 8.5f),
                    rectTexto,
                    Color.Black,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak
                );
            }

            // etiqueta inferior
            Rectangle rectEstudiantes = new Rectangle(areaGrafico.Left, areaTotal.Bottom - 28, areaGrafico.Width, 20);
            TextRenderer.DrawText(
                g,
                "ESTUDIANTES",
                new Font("Segoe UI", 8.5f),
                rectEstudiantes,
                Color.Black,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );
        }
        private void DibujarBarraRedondeada(Graphics g, Rectangle rect, Color color, int radio)
        {
            using GraphicsPath path = new GraphicsPath();

            int diametro = radio * 2;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diametro, diametro, 180, 90);
            path.AddArc(rect.Right - diametro, rect.Y, diametro, diametro, 270, 90);
            path.AddLine(rect.Right, rect.Y + radio, rect.Right, rect.Bottom);
            path.AddLine(rect.Right, rect.Bottom, rect.X, rect.Bottom);
            path.AddLine(rect.X, rect.Bottom, rect.X, rect.Y + radio);
            path.CloseFigure();

            using SolidBrush brush = new SolidBrush(color);
            g.FillPath(brush, path);
        }
        private void SetBotonEstado(Guna2Button btn, bool activo)
        {
            btn.Enabled = true; // 👈 SIEMPRE true

            if (activo)
            {
                btn.ForeColor = Color.White;
                btn.Cursor = Cursors.Hand;
                btn.Tag = "activo";
            }
            else
            {
                btn.ForeColor = Color.FromArgb(255, 255, 255, 120); // semi transparente
                btn.Cursor = Cursors.Default;
                btn.Tag = "inactivo";
            }
        }
        private void btnSiguienteGrafico_Click(object sender, EventArgs e)
        {
            if ((string)btnSiguienteGrafico.Tag == "inactivo")
                return;

            int totalPaginas = (int)Math.Ceiling(dtGraficoCompleto.Rows.Count / (double)itemsPorPaginaGrafico);

            if (paginaGrafico < totalPaginas - 1)
            {
                paginaGrafico++;
                ActualizarBotonesGrafico();
                panelGrafico.Invalidate();
            }
        }

        private void btnAnteriorGrafico_Click(object sender, EventArgs e)
        {
            if ((string)btnAnteriorGrafico.Tag == "inactivo")
                return;

            if (paginaGrafico > 0)
            {
                paginaGrafico--;
                ActualizarBotonesGrafico();
                panelGrafico.Invalidate();
            }
        }
        private void LimpiarVistaReporte()
        {
            dgvBoleta.Rows.Clear();

            dtGraficoCompleto = new DataTable();
            paginaGrafico = 0;

            lblRegistros.Text = "Registros del 0 al 0 total de 0 registros";
            lblRegistros.BackColor = this.BackColor;
            lblPaginaGrafico.Text = "Página 0 de 0";

            //btnAnteriorGrafico.Enabled = false;
            //btnSiguienteGrafico.Enabled = false;

            panelGrafico.Invalidate();
        }
        private void ActualizarBotonesGrafico()
        {
            if (dtGraficoCompleto == null || dtGraficoCompleto.Rows.Count == 0)
            {
                SetBotonEstado(btnAnteriorGrafico, false);
                SetBotonEstado(btnSiguienteGrafico, false);
                lblPaginaGrafico.Text = "Página 0 de 0";
                return;
            }

            int totalPaginas = (int)Math.Ceiling(dtGraficoCompleto.Rows.Count / (double)itemsPorPaginaGrafico);

            SetBotonEstado(btnAnteriorGrafico, paginaGrafico > 0);
            SetBotonEstado(btnSiguienteGrafico, paginaGrafico < totalPaginas - 1);

            lblPaginaGrafico.Text = $"Página {paginaGrafico + 1} de {totalPaginas}";
        }
        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }


        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarReporte();
        }
        private void FiltrarGrid()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            foreach (DataGridViewRow row in dgvBoleta.Rows)
            {
                if (row.IsNewRow) continue;

                string asignatura = row.Cells["Asignatura"].Value?.ToString()?.ToLower() ?? "";
                string docente = row.Cells["Docente"].Value?.ToString()?.ToLower() ?? "";
                string estado = row.Cells["Estado"].Value?.ToString()?.ToLower() ?? "";

                bool visible =
                    asignatura.Contains(filtro) ||
                    docente.Contains(filtro) ||
                    estado.Contains(filtro);

                row.Visible = visible;
            }
        }
        private void FrmConsolidadoAsignaturas_Load(object sender, EventArgs e)
        {
            CargarParciales();
            CargarAnios();
            CargarGrados();

            // opcional
            dgvBoleta.AllowUserToAddRows = false;
            dgvBoleta.AllowUserToDeleteRows = false;
            dgvBoleta.ReadOnly = true;
            dgvBoleta.RowHeadersVisible = false;
            dgvBoleta.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBoleta.AutoGenerateColumns = false;
        }


        private void cbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarSecciones();
        }


        private DataTable ObtenerBoletaParcial(int periodo, int gradoId, string seccion, int anio)
        {
            DataTable dt = new DataTable();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("spMAE_BoletaParcial", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@periodo", periodo);
                cmd.Parameters.AddWithValue("@gradoID", gradoId);
                cmd.Parameters.AddWithValue("@letraSeccion", seccion);
                cmd.Parameters.AddWithValue("@anio", anio);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        private void CargarReporte()
        {
            if (cbParcial.SelectedItem == null ||
                cbGrado.SelectedItem == null ||
                cbSeccion.SelectedItem == null ||
                cbAnio.SelectedItem == null)
            {
                LimpiarVistaReporte();
                return;
            }


            int parcial = Convert.ToInt32(cbParcial.SelectedItem);
            string nombreGrado = cbGrado.SelectedItem.ToString();
            string seccion = cbSeccion.SelectedItem.ToString();
            int anio = Convert.ToInt32(cbAnio.SelectedItem);

            int gradoId = ObtenerGradoId(nombreGrado);
            if (gradoId == 0)
            {
                MessageBox.Show("No se encontró el grado seleccionado.");
                return;
            }

            DataTable dt = ObtenerBoletaParcial(parcial, gradoId, seccion, anio);

            ConfigurarGridBoleta();

            if (dt == null || dt.Rows.Count == 0)
            {
                dgvBoleta.Rows.Clear();
                lblRegistros.Text = "Registros del 0 al 0 total de 0 registros";
                LimpiarVistaReporte();
                return;
            }

            LlenarGridBoleta(dt);
            dtGraficoCompleto = dt.Copy();
            paginaGrafico = 0;
            ActualizarBotonesGrafico();
            panelGrafico.Invalidate();
        }

        private int ObtenerGradoId(string nombreGrado)
        {
            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 1 GradoID
        FROM Grado
        WHERE NombreGrado = @NombreGrado;", cn))
            {
                cmd.Parameters.AddWithValue("@NombreGrado", nombreGrado);
                cn.Open();

                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    return 0;

                return Convert.ToInt32(result);
            }
        }

        public class ComboItem
        {
            public string Text { get; set; }
            public int Value { get; set; }

            public override string ToString() => Text;
        }


        private void ConfigurarGridBoleta()
        {
            dgvBoleta.Columns.Clear();
            dgvBoleta.Rows.Clear();

            dgvBoleta.AllowUserToAddRows = false;
            dgvBoleta.AllowUserToDeleteRows = false;
            dgvBoleta.AllowUserToResizeRows = false;
            dgvBoleta.AllowUserToResizeColumns = false;
            dgvBoleta.MultiSelect = false;
            dgvBoleta.ReadOnly = true;
            dgvBoleta.RowHeadersVisible = false;
            dgvBoleta.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBoleta.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvBoleta.BackgroundColor = Color.White;
            dgvBoleta.BorderStyle = BorderStyle.None;
            dgvBoleta.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvBoleta.GridColor = Color.FromArgb(230, 230, 230);
            dgvBoleta.EnableHeadersVisualStyles = false;

            dgvBoleta.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvBoleta.ColumnHeadersHeight = 35;
            dgvBoleta.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 105, 255);
            dgvBoleta.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBoleta.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvBoleta.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvBoleta.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(24, 105, 255);
            dgvBoleta.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgvBoleta.DefaultCellStyle.BackColor = Color.White;
            dgvBoleta.DefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 45);
            dgvBoleta.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            dgvBoleta.DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 249, 255);
            dgvBoleta.DefaultCellStyle.SelectionForeColor = Color.FromArgb(45, 45, 45);
            dgvBoleta.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvBoleta.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 252, 252);
            dgvBoleta.RowTemplate.Height = 48;

            dgvBoleta.Columns.Add("No", "N°");
            dgvBoleta.Columns.Add("Asignatura", "ASIGNATURA");
            dgvBoleta.Columns.Add("Docente", "DOCENTE");
            dgvBoleta.Columns.Add("Promedio", "PROMEDIO");
            dgvBoleta.Columns.Add("Estado", "ESTADO");
            dgvBoleta.Columns.Add("Acciones", "ACCIONES");

            dgvBoleta.Columns["No"].Width = 50;
            dgvBoleta.Columns["Asignatura"].Width = 240;
            dgvBoleta.Columns["Docente"].Width = 210;
            dgvBoleta.Columns["Promedio"].Width = 110;
            dgvBoleta.Columns["Estado"].Width = 140;
            dgvBoleta.Columns["Acciones"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvBoleta.Columns["Promedio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBoleta.Columns["No"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvBoleta.Columns["Estado"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvBoleta.Columns["Acciones"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void LlenarGridBoleta(DataTable dt)
        {
            dgvBoleta.Rows.Clear();

            int i = 1;
            foreach (DataRow row in dt.Rows)
            {
                decimal promedio = row["PromedioClase"] == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(row["PromedioClase"]);

                string estado = row["Estado"]?.ToString()?.ToUpper() ?? "";

                string accion = estado == "EXCELENTE"
                    ? "FELICITAR DOCENTE"
                    : estado == "CRITICO"
                        ? "VER DETALLE"
                        : "VER DETALLE";

                dgvBoleta.Rows.Add(
                    i,
                    row["Asignatura"]?.ToString() ?? "",
                    row["Docente"]?.ToString() ?? "",
                    promedio.ToString("0.00"),
                    estado,
                    accion
                );

                i++;
            }

            lblRegistros.Text = $"Registros del 1 al {dt.Rows.Count} total de {dt.Rows.Count} registros";
        }
        private void dgvBoleta_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string colName = dgvBoleta.Columns[e.ColumnIndex].Name;

            if (colName == "Estado")
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                string estado = e.FormattedValue?.ToString()?.ToUpper() ?? "";

                Color backColor = Color.FromArgb(245, 245, 245);
                Color foreColor = Color.FromArgb(90, 90, 90);

                if (estado == "EXCELENTE")
                {
                    backColor = Color.FromArgb(220, 248, 228);
                    foreColor = Color.FromArgb(22, 163, 74);
                }
                else if (estado == "MEDIO")
                {
                    backColor = Color.FromArgb(255, 243, 205);
                    foreColor = Color.FromArgb(180, 125, 0);
                }
                else if (estado == "CRITICO")
                {
                    backColor = Color.FromArgb(255, 230, 230);
                    foreColor = Color.FromArgb(239, 68, 68);
                }

                Rectangle pillRect = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + 11,
                    e.CellBounds.Width - 20,
                    e.CellBounds.Height - 22
                );

                using (SolidBrush brush = new SolidBrush(backColor))
                    e.Graphics.FillEllipse(brush, pillRect.X, pillRect.Y, 18, pillRect.Height);

                using (SolidBrush brush = new SolidBrush(backColor))
                    e.Graphics.FillEllipse(brush, pillRect.Right - 18, pillRect.Y, 18, pillRect.Height);

                using (SolidBrush brush = new SolidBrush(backColor))
                    e.Graphics.FillRectangle(brush, pillRect.X + 9, pillRect.Y, pillRect.Width - 18, pillRect.Height);

                TextRenderer.DrawText(
                    e.Graphics,
                    estado,
                    new Font("Segoe UI", 9F, FontStyle.Regular),
                    pillRect,
                    foreColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                using (Pen pen = new Pen(dgvBoleta.GridColor))
                {
                    e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
                }
            }
            else if (colName == "Acciones")
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                string texto = e.FormattedValue?.ToString() ?? "";

                Rectangle btnRect = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + 6,
                    e.CellBounds.Width - 20,
                    e.CellBounds.Height - 12
                );

                Color btnBack = Color.FromArgb(245, 245, 245);
                Color btnBorder = Color.FromArgb(210, 210, 210);
                Color btnText = Color.FromArgb(90, 90, 90);

                using (SolidBrush brush = new SolidBrush(btnBack))
                    e.Graphics.FillRectangle(brush, btnRect);

                using (Pen pen = new Pen(btnBorder))
                    e.Graphics.DrawRectangle(pen, btnRect);

                TextRenderer.DrawText(
                    e.Graphics,
                    texto,
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    btnRect,
                    btnText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                using (Pen pen = new Pen(dgvBoleta.GridColor))
                {
                    e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
                }
            }
        }


        private void dgvBoleta_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvBoleta.Columns[e.ColumnIndex].Name == "Acciones")
            {
                string accion = dgvBoleta.Rows[e.RowIndex].Cells["Acciones"].Value?.ToString() ?? "";
                string asignatura = dgvBoleta.Rows[e.RowIndex].Cells["Asignatura"].Value?.ToString() ?? "";
                string docente = dgvBoleta.Rows[e.RowIndex].Cells["Docente"].Value?.ToString() ?? "";
                string estado = dgvBoleta.Rows[e.RowIndex].Cells["Estado"].Value?.ToString() ?? "";

                if (accion == "FELICITAR DOCENTE")
                {
                    MessageBox.Show($"Felicitar a {docente} por {asignatura}");
                }
                else
                {
                    MessageBox.Show($"Ver detalle de {asignatura}");
                }
            }
        }
        private void FrmBoletaParcial_Load(object sender, EventArgs e)
        {
            CargarParciales();
            CargarAnios();
            CargarGrados();
            ActualizarTitulo();
        }


        private void cbSeccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void cbParcial_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarTitulo();
            CargarReporte();
        }

        private void cbAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarTitulo();
            CargarReporte();

        }



        // =========================
        // CARGA DE COMBOS
        // =========================
        private void CargarParciales()
        {
            cbParcial.Items.Clear();
            cbParcial.Items.Add("1");
            cbParcial.Items.Add("2");
            cbParcial.Items.Add("3");
            cbParcial.Items.Add("4");

            if (cbParcial.Items.Count > 0)
                cbParcial.SelectedIndex = 0;
        }

        private void CargarAnios()
        {
            cbAnio.Items.Clear();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT DISTINCT Anio
                FROM CargaAcademica
                ORDER BY Anio DESC;", cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cbAnio.Items.Add(dr["Anio"].ToString());
                    }
                }
            }

            if (cbAnio.Items.Count > 0)
                cbAnio.SelectedIndex = 0;
        }

        private void CargarGrados()
        {
            cbGrado.Items.Clear();
            cbSeccion.Items.Clear();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT NombreGrado
                FROM
                (
                    SELECT DISTINCT
                        G.NombreGrado,
                        CASE
                            WHEN G.NombreGrado = 'KINDER' THEN 1
                            WHEN G.NombreGrado = 'PREBASICA' THEN 2
                            WHEN G.NombreGrado = 'PRIMERO' THEN 3
                            WHEN G.NombreGrado = 'SEGUNDO' THEN 4
                            WHEN G.NombreGrado = 'TERCERO' THEN 5
                            WHEN G.NombreGrado = 'CUARTO' THEN 6
                            WHEN G.NombreGrado = 'QUINTO' THEN 7
                            WHEN G.NombreGrado = 'SEXTO' THEN 8
                            WHEN G.NombreGrado = 'SEPTIMO' THEN 9
                            WHEN G.NombreGrado = 'OCTAVO' THEN 10
                            WHEN G.NombreGrado = 'NOVENO' THEN 11
                            WHEN G.NombreGrado = 'DECIMO' THEN 12
                            WHEN G.NombreGrado = 'UNDECIMO' THEN 13
                            ELSE 99
                        END AS OrdenGrado
                    FROM Seccion S
                    INNER JOIN Grado G ON G.GradoID = S.GradoID
                ) X
                ORDER BY OrdenGrado;", cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cbGrado.Items.Add(dr["NombreGrado"].ToString());
                    }
                }
            }

            if (cbGrado.Items.Count > 0)
                cbGrado.SelectedIndex = 0;
        }
        private void CargarSecciones()
        {
            cbSeccion.Items.Clear();

            if (cbGrado.SelectedItem == null)
                return;

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT DISTINCT S.Letra
                FROM Seccion S
                INNER JOIN Grado G ON G.GradoID = S.GradoID
                WHERE G.NombreGrado = @Grado
                ORDER BY S.Letra;", cn))
            {
                cmd.Parameters.AddWithValue("@Grado", cbGrado.SelectedItem.ToString());

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cbSeccion.Items.Add(dr["Letra"].ToString());
                    }
                }
            }

            if (cbSeccion.Items.Count > 0)
                cbSeccion.SelectedIndex = 0;
        }

        private void ActualizarTitulo()
        {
            if (cbParcial.SelectedItem == null || cbAnio.SelectedItem == null)
                return;

            int parcial = Convert.ToInt32(cbParcial.SelectedItem);
            string anio = cbAnio.SelectedItem.ToString();

            string textoParcial = parcial switch
            {
                1 => "I PARCIAL",
                2 => "II PARCIAL",
                3 => "III PARCIAL",
                4 => "IV PARCIAL",
                _ => $"{parcial} PARCIAL"
            };

            lblTitulo.Text = $"CONSOLIDADO DE BOLETA DE CALIFICACIONES: {textoParcial} {anio}";
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            FiltrarGrid();
        }
    }
}
