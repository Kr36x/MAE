using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GestionAcademicaV2.Pantallas.DocenteVentanas.FrmConsolidadoAsignaturas;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Layout.Borders;

using DrawingColor = System.Drawing.Color;
using PdfColor = iText.Kernel.Colors.Color;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmControlReuniones : Form
    {
        private readonly Conexion conexion = new Conexion();
        private DataTable dtOriginal = new DataTable();
        public FrmControlReuniones()
        {
            InitializeComponent();
            dgvReuniones.CellPainting += dgvReuniones_CellPainting;
            dgvReuniones.CellClick += dgvReuniones_CellClick;
        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void FrmControlReuniones_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarDocentes();
            CargarMeses();
            CargarAnios();
            
            CargarReporte();
        }

        private void cbDocente_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void cbMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void cbAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            FiltrarGrid();
        }

        // =========================
        // CARGA DE COMBOS
        // =========================
        private void CargarDocentes()
        {
            cbDocente.Items.Clear();

            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand(@"
                SELECT DocenteID, Nombre
                FROM Docente
                ORDER BY Nombre;", cn);

            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                cbDocente.Items.Add(new ComboItem
                {
                    Value = Convert.ToInt32(dr["DocenteID"]),
                    Text = dr["Nombre"].ToString()
                });
            }

            if (cbDocente.Items.Count > 0)
                cbDocente.SelectedIndex = 0;
        }

        private void CargarMeses()
        {
            cbMes.Items.Clear();

            cbMes.Items.Add(new ComboItem { Value = 1, Text = "ENERO" });
            cbMes.Items.Add(new ComboItem { Value = 2, Text = "FEBRERO" });
            cbMes.Items.Add(new ComboItem { Value = 3, Text = "MARZO" });
            cbMes.Items.Add(new ComboItem { Value = 4, Text = "ABRIL" });
            cbMes.Items.Add(new ComboItem { Value = 5, Text = "MAYO" });
            cbMes.Items.Add(new ComboItem { Value = 6, Text = "JUNIO" });
            cbMes.Items.Add(new ComboItem { Value = 7, Text = "JULIO" });
            cbMes.Items.Add(new ComboItem { Value = 8, Text = "AGOSTO" });
            cbMes.Items.Add(new ComboItem { Value = 9, Text = "SEPTIEMBRE" });
            cbMes.Items.Add(new ComboItem { Value = 10, Text = "OCTUBRE" });
            cbMes.Items.Add(new ComboItem { Value = 11, Text = "NOVIEMBRE" });
            cbMes.Items.Add(new ComboItem { Value = 12, Text = "DICIEMBRE" });

            cbMes.SelectedValue = DateTime.Now.Month;
            if (cbMes.Items.Count > 0)
                cbMes.SelectedIndex = Math.Max(0, DateTime.Now.Month - 1);
        }

        private void CargarAnios()
        {
            cbAnio.Items.Clear();

            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand(@"
                SELECT DISTINCT YEAR(FechaHora) AS Anio
                FROM Reunion
                ORDER BY Anio DESC;", cn);

            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                cbAnio.Items.Add(dr["Anio"].ToString());
            }

            if (cbAnio.Items.Count > 0)
                cbAnio.SelectedIndex = 0;
        }

        // =========================
        // CONSULTA
        // =========================
        private DataTable ObtenerReunionesMensuales(int docenteId, int mes, int anio)
        {
            DataTable dt = new DataTable();

            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_RepReunionesMensuales", cn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@docenteID", docenteId);
            cmd.Parameters.AddWithValue("@mes", mes);
            cmd.Parameters.AddWithValue("@anio", anio);

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        private void CargarReporte()
        {
            if (cbDocente.SelectedItem == null || cbMes.SelectedItem == null || cbAnio.SelectedItem == null)
            {
                LimpiarVista();
                return;
            }

            int docenteId = ((ComboItem)cbDocente.SelectedItem).Value;
            int mes = ((ComboItem)cbMes.SelectedItem).Value;
            int anio = Convert.ToInt32(cbAnio.SelectedItem);

            DataTable dt = ObtenerReunionesMensuales(docenteId, mes, anio);
            dtOriginal = dt.Copy();

            LlenarGrid(dt);
        }

        private void LimpiarVista()
        {
            dgvReuniones.Rows.Clear();
            lblRegistros.Text = "Registros del 0 al 0 total de 0 registros";
            dtOriginal = new DataTable();
        }

        // =========================
        // GRID
        // =========================
        private void ConfigurarGrid()
        {
            dgvReuniones.Columns.Clear();
            dgvReuniones.Rows.Clear();

            dgvReuniones.AllowUserToAddRows = false;
            dgvReuniones.AllowUserToDeleteRows = false;
            dgvReuniones.AllowUserToResizeRows = false;
            dgvReuniones.AllowUserToResizeColumns = false;
            dgvReuniones.MultiSelect = false;
            dgvReuniones.ReadOnly = true;
            dgvReuniones.RowHeadersVisible = false;
            dgvReuniones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReuniones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dgvReuniones.BackgroundColor = DrawingColor.White;
            dgvReuniones.BorderStyle = BorderStyle.None;

            // AQUÍ ESTÁ LA CLAVE
            dgvReuniones.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvReuniones.GridColor = DrawingColor.FromArgb(220, 220, 220);

            dgvReuniones.EnableHeadersVisualStyles = false;
            dgvReuniones.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvReuniones.ColumnHeadersHeight = 56;

            dgvReuniones.ColumnHeadersDefaultCellStyle.BackColor = DrawingColor.FromArgb(24, 105, 255);
            dgvReuniones.ColumnHeadersDefaultCellStyle.ForeColor = DrawingColor.White;
            dgvReuniones.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvReuniones.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvReuniones.ColumnHeadersDefaultCellStyle.SelectionBackColor = DrawingColor.FromArgb(24, 105, 255);
            dgvReuniones.ColumnHeadersDefaultCellStyle.SelectionForeColor = DrawingColor.White;
            dgvReuniones.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);

            dgvReuniones.DefaultCellStyle.BackColor = DrawingColor.White;
            dgvReuniones.DefaultCellStyle.ForeColor = DrawingColor.FromArgb(35, 35, 35);
            dgvReuniones.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            dgvReuniones.DefaultCellStyle.SelectionBackColor = DrawingColor.FromArgb(245, 249, 255);
            dgvReuniones.DefaultCellStyle.SelectionForeColor = DrawingColor.FromArgb(35, 35, 35);
            dgvReuniones.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvReuniones.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            // Alternancia de filas
            dgvReuniones.RowsDefaultCellStyle.BackColor = DrawingColor.White;
            dgvReuniones.AlternatingRowsDefaultCellStyle.BackColor = DrawingColor.FromArgb(248, 248, 248);

            dgvReuniones.RowTemplate.Height = 58;

            dgvReuniones.Columns.Add("No", "N°");
            dgvReuniones.Columns.Add("FechaHora", "FECHA Y\nHORA");
            dgvReuniones.Columns.Add("Estudiante", "ESTUDIANTES");
            dgvReuniones.Columns.Add("GradoSeccion", "GRADO Y\nSECCION");
            dgvReuniones.Columns.Add("Tema", "TEMA");
            dgvReuniones.Columns.Add("Medio", "MEDIO");
            dgvReuniones.Columns.Add("Estado", "ESTADO");
            dgvReuniones.Columns.Add("Acciones", "ACCIONES");

            dgvReuniones.Columns["No"].Width = 50;
            dgvReuniones.Columns["FechaHora"].Width = 120;
            dgvReuniones.Columns["Estudiante"].Width = 165;
            dgvReuniones.Columns["GradoSeccion"].Width = 145;
            dgvReuniones.Columns["Tema"].Width = 120;
            dgvReuniones.Columns["Medio"].Width = 100;
            dgvReuniones.Columns["Estado"].Width = 110;
            dgvReuniones.Columns["Acciones"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvReuniones.Columns["No"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvReuniones.Columns["FechaHora"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
        private void LlenarGrid(DataTable dt)
        {
            dgvReuniones.Rows.Clear();

            if (dt == null || dt.Rows.Count == 0)
            {
                lblRegistros.Text = "Registros del 0 al 0 total de 0 registros";
                return;
            }

            int i = 1;
            foreach (DataRow row in dt.Rows)
            {
                string estado = row["Estado"]?.ToString()?.ToUpper() ?? "";
                string accion = ObtenerAccionSegunEstado(estado);

                string fechaTexto = "";
                if (row["FechaHora"] != DBNull.Value)
                {
                    DateTime fecha = Convert.ToDateTime(row["FechaHora"]);
                    fechaTexto = fecha.ToString("dd/MM hh:mm tt");
                }

                dgvReuniones.Rows.Add(
                    i,
                    fechaTexto,
                    row["Nombre"]?.ToString() ?? "",
                    row["GradoSeccion"]?.ToString() ?? "",
                    row["Tema"]?.ToString() ?? "",
                    row["MedioDifusion"]?.ToString() ?? "",
                    estado,
                    accion
                );

                i++;
            }

            lblRegistros.Text = $"Registros del 1 al {dt.Rows.Count} total de {dt.Rows.Count} registros";
        }

        private string ObtenerAccionSegunEstado(string estado)
        {
            return estado switch
            {
                "REALIZADA" => "PDF",
                "PROGRAMADA" => "EDITAR",
                "CANCELADA" => "--",
                _ => "--"
            };
        }

        // =========================
        // BÚSQUEDA
        // =========================
        private void FiltrarGrid()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            foreach (DataGridViewRow row in dgvReuniones.Rows)
            {
                if (row.IsNewRow) continue;

                string fecha = row.Cells["FechaHora"].Value?.ToString()?.ToLower() ?? "";
                string estudiante = row.Cells["Estudiante"].Value?.ToString()?.ToLower() ?? "";
                string grado = row.Cells["GradoSeccion"].Value?.ToString()?.ToLower() ?? "";
                string tema = row.Cells["Tema"].Value?.ToString()?.ToLower() ?? "";
                string medio = row.Cells["Medio"].Value?.ToString()?.ToLower() ?? "";
                string estado = row.Cells["Estado"].Value?.ToString()?.ToLower() ?? "";

                row.Visible =
                    string.IsNullOrWhiteSpace(filtro) ||
                    fecha.Contains(filtro) ||
                    estudiante.Contains(filtro) ||
                    grado.Contains(filtro) ||
                    tema.Contains(filtro) ||
                    medio.Contains(filtro) ||
                    estado.Contains(filtro);
            }
        }

        // =========================
        // PINTURA PERSONALIZADA
        // =========================
        private void dgvReuniones_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string colName = dgvReuniones.Columns[e.ColumnIndex].Name;

            if (colName == "Estado")
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                string estado = e.FormattedValue?.ToString()?.ToUpper() ?? "";

                DrawingColor backColor = DrawingColor.FromArgb(245, 245, 245);
                DrawingColor foreColor = DrawingColor.FromArgb(90, 90, 90);

                if (estado == "REALIZADA")
                {
                    backColor = DrawingColor.FromArgb(220, 248, 228);
                    foreColor = DrawingColor.FromArgb(22, 163, 74);
                }
                else if (estado == "PROGRAMADA")
                {
                    backColor = DrawingColor.FromArgb(255, 243, 205);
                    foreColor = DrawingColor.FromArgb(180, 125, 0);
                }
                else if (estado == "CANCELADA")
                {
                    backColor = DrawingColor.FromArgb(255, 230, 230);
                    foreColor = DrawingColor.FromArgb(239, 68, 68);
                }

                Rectangle pillRect = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + 11,
                    e.CellBounds.Width - 20,
                    e.CellBounds.Height - 22
                );

                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillEllipse(brush, pillRect.X, pillRect.Y, 18, pillRect.Height);
                    e.Graphics.FillEllipse(brush, pillRect.Right - 18, pillRect.Y, 18, pillRect.Height);
                    e.Graphics.FillRectangle(brush, pillRect.X + 9, pillRect.Y, pillRect.Width - 18, pillRect.Height);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    estado.Length > 10 ? estado.Substring(0, 8) + "..." : estado,
                    new Font("Segoe UI", 8.5F, FontStyle.Regular),
                    pillRect,
                    foreColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                using Pen pen = new Pen(dgvReuniones.GridColor);
                e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
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

                DrawingColor btnBack = DrawingColor.FromArgb(245, 245, 245);
                DrawingColor btnBorder = DrawingColor.FromArgb(210, 210, 210);
                DrawingColor btnText = DrawingColor.FromArgb(90, 90, 90);

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

                using Pen pen2 = new Pen(dgvReuniones.GridColor);
                e.Graphics.DrawLine(pen2, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
            }
        }

        private void dgvReuniones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvReuniones.Columns[e.ColumnIndex].Name != "Acciones")
                return;

            string accion = dgvReuniones.Rows[e.RowIndex].Cells["Acciones"].Value?.ToString() ?? "";
            string estudiante = dgvReuniones.Rows[e.RowIndex].Cells["Estudiante"].Value?.ToString() ?? "";
            string tema = dgvReuniones.Rows[e.RowIndex].Cells["Tema"].Value?.ToString() ?? "";

            if (accion == "PDF")
            {
                string fechaHora = dgvReuniones.Rows[e.RowIndex].Cells["FechaHora"].Value?.ToString() ?? "";
                string gradoSeccion = dgvReuniones.Rows[e.RowIndex].Cells["GradoSeccion"].Value?.ToString() ?? "";
                //string tema = dgvReuniones.Rows[e.RowIndex].Cells["Tema"].Value?.ToString() ?? "";
                string medio = dgvReuniones.Rows[e.RowIndex].Cells["Medio"].Value?.ToString() ?? "";
                string estado = dgvReuniones.Rows[e.RowIndex].Cells["Estado"].Value?.ToString() ?? "";
                string docente = cbDocente.SelectedItem?.ToString() ?? "";
                string anio = cbAnio.SelectedItem?.ToString() ?? "";

                GenerarPdfActa(docente, estudiante, fechaHora, gradoSeccion, tema, medio, estado, anio);
            }
            else if (accion == "EDITAR")
            {
                MessageBox.Show($"No funco todavia :( : {tema}.", "Editar");
            }
        }

        private void GenerarPdfActa(
            string docente,
            string estudiante,
            string fechaHora,
            string gradoSeccion,
            string tema,
            string medio,
            string estado,
            string anio)
        {
            using SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivo PDF (*.pdf)|*.pdf";
            sfd.FileName = $"Acta_{estudiante.Replace(" ", "_")}.pdf";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                using PdfWriter writer = new PdfWriter(sfd.FileName);
                using PdfDocument pdf = new PdfDocument(writer);
                using Document doc = new Document(pdf);

                PdfColor azul = new DeviceRgb(24, 105, 255);
                PdfColor grisClaro = new DeviceRgb(245, 245, 245);
                PdfColor grisBorde = new DeviceRgb(210, 210, 210);

                // Encabezado
                Paragraph escuela = new Paragraph("ATLANTIC ACADEMY BILINGUAL SCHOOL")

                    .SetFontSize(14)
                    .SetMarginBottom(5);

                Paragraph titulo = new Paragraph("ACTA DE REUNIÓN CON PADRE/MADRE DE FAMILIA")

                    .SetFontSize(18)
                    .SetFontColor(azul)
                    .SetMarginBottom(15);

                Paragraph subtitulo = new Paragraph($"Control mensual de reuniones - Año académico {anio}")
                    .SetFontSize(10)
                    .SetMarginBottom(20);

                doc.Add(escuela);
                doc.Add(titulo);
                doc.Add(subtitulo);

                // Tabla de datos
                Table tabla = new Table(2).UseAllAvailableWidth();
                tabla.SetMarginBottom(20);

                void AddRow(string etiqueta, string valor)
                {
                    tabla.AddCell(
                        new Cell()
                            .Add(new Paragraph(etiqueta).SetFontSize(10))
                            .SetBackgroundColor(grisClaro)
                            .SetBorder(new SolidBorder(grisBorde, 1))
                    );

                    tabla.AddCell(
                        new Cell()
                            .Add(new Paragraph(valor).SetFontSize(10))
                            .SetBorder(new SolidBorder(grisBorde, 1))
                    );
                }

                AddRow("Docente", docente);
                AddRow("Estudiante", estudiante);
                AddRow("Fecha y hora", fechaHora);
                AddRow("Grado y sección", gradoSeccion);
                AddRow("Tema", tema);
                AddRow("Medio", medio);
                AddRow("Estado", estado);

                doc.Add(tabla);

                // Cuerpo del acta
                Paragraph cuerpoTitulo = new Paragraph("Detalle del acta")
                    .SetFontSize(12)
                    .SetMarginBottom(10);

                Paragraph cuerpo = new Paragraph(
                    $"En la fecha {fechaHora}, el docente {docente} sostuvo una reunión con el responsable del estudiante {estudiante}, perteneciente a {gradoSeccion}. " +
                    $"El tema tratado fue \"{tema}\" y el medio de comunicación utilizado fue {medio.ToLower()}. " +
                    $"El estado de la reunión se registra como {estado.ToLower()}."
                )
                .SetFontSize(11)
                .SetTextAlignment(TextAlignment.JUSTIFIED)
                .SetMarginBottom(30);

                doc.Add(cuerpoTitulo);
                doc.Add(cuerpo);

                // Firmas
                Table firmas = new Table(2).UseAllAvailableWidth();
                firmas.SetMarginTop(30);

                firmas.AddCell(
                    new Cell()
                        .Add(new Paragraph("\n\n____________________________\nDocente").SetTextAlignment(TextAlignment.CENTER))
                        .SetBorder(Border.NO_BORDER)
                );

                firmas.AddCell(
                    new Cell()
                        .Add(new Paragraph("\n\n____________________________\nPadre / Madre / Encargado").SetTextAlignment(TextAlignment.CENTER))
                        .SetBorder(Border.NO_BORDER)
                );

                doc.Add(firmas);

                MessageBox.Show("PDF generado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al generar el PDF:\n{ex.Message}\n\nDetalle:\n{ex.InnerException?.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
