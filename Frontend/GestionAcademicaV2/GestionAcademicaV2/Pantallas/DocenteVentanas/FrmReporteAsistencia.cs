using GestionAcademicaV2.Modelos;
using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmReporteAsistencia : Form
    {
        private readonly int docenteId;
        private readonly Conexion conexion = new Conexion();
        private int anioActual;
        private int mesActual;

        private readonly System.Windows.Forms.Timer timerBusqueda = new System.Windows.Forms.Timer();
        private const int MinimoCaracteresBusqueda = 3;
        private bool formularioCargado = false;

        private bool ocultarFinesDeSemana = true;
        public FrmReporteAsistencia(int docenteId)
        {
            InitializeComponent();
            this.docenteId = docenteId;
            timerBusqueda.Interval = 350;
            timerBusqueda.Tick += TimerBusqueda_Tick;

        }

        private void FrmReporteAsistencia_Load(object sender, EventArgs e)
        {
            DateTime hoy = DateTime.Today;
            anioActual = hoy.Year;
            mesActual = hoy.Month;
            lblDocente.Text = ObtenerNombreDocente();

            CargarGrados();
            ConfigurarDataGridView(anioActual, mesActual);

            if (cbGrado.Items.Count > 0)
            {
                CargarReporte();
                cbGrado.SelectedIndex = 0;
            }

            txtBuscar.TextChanged += txtBuscar_TextChanged;
            btnPDF.Click += btnDescargarPdf_Click;
            formularioCargado = true;

            chkMostrarFinesDeSemana.CheckedChanged += chkMostrarFinesDeSemana_CheckedChanged;
            chkMostrarFinesDeSemana.Checked = false;

            chkMostrarFinesDeSemana.Text = "Mostrar fines de semana";
            chkMostrarFinesDeSemana.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkMostrarFinesDeSemana.ForeColor = System.Drawing.Color.FromArgb(20, 20, 20);
            chkMostrarFinesDeSemana.BackColor = System.Drawing.Color.Transparent;

            chkMostrarFinesDeSemana.CheckedState.BorderColor = System.Drawing.Color.FromArgb(0, 100, 0);
            chkMostrarFinesDeSemana.CheckedState.FillColor = System.Drawing.Color.FromArgb(0, 128, 0);
            chkMostrarFinesDeSemana.CheckedState.BorderRadius = 2;
            chkMostrarFinesDeSemana.CheckedState.BorderThickness = 1;

            chkMostrarFinesDeSemana.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(150, 150, 150);
            chkMostrarFinesDeSemana.UncheckedState.FillColor = System.Drawing.Color.White;
            chkMostrarFinesDeSemana.UncheckedState.BorderRadius = 2;
            chkMostrarFinesDeSemana.UncheckedState.BorderThickness = 1;
        }

        private void chkMostrarFinesDeSemana_CheckedChanged(object sender, EventArgs e)
        {
            ocultarFinesDeSemana = !chkMostrarFinesDeSemana.Checked;
            CargarReporte();
        }

        private List<DateTime> ObtenerFechasVisiblesDelMes(int anio, int mes)
        {
            List<DateTime> fechas = new List<DateTime>();
            int totalDias = DateTime.DaysInMonth(anio, mes);

            for (int dia = 1; dia <= totalDias; dia++)
            {
                DateTime fecha = new DateTime(anio, mes, dia);

                if (ocultarFinesDeSemana &&
                    (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday))
                    continue;

                fechas.Add(fecha);
            }

            return fechas;
        }
        private void txtBuscar_TextChanged(object? sender, EventArgs e)
        {
            if (!formularioCargado) return;

            timerBusqueda.Stop();
            timerBusqueda.Start();
        }
        private void btnDescargarPdf_Click(object sender, EventArgs e)
        {
            if (dgvAsistencia.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog save = new SaveFileDialog())
            {
                save.Filter = "Archivo PDF (*.pdf)|*.pdf";
                save.FileName = $"Reporte_Asistencia_{cbGrado.Text}_{cbSeccion.Text}_{anioActual}_{mesActual:D2}.pdf";

                if (save.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportarReporteAsistenciaPdf(save.FileName);
                        MessageBox.Show("PDF generado correctamente.", "PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ocurrió un error al generar el PDF:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private Cell CrearCeldaInfo(string titulo, string valor, PdfFont font, PdfFont fontBold)
        {
            Paragraph p = new Paragraph()
                .Add(new Text(titulo + "\n").SetFont(fontBold).SetFontSize(10))
                .Add(new Text(valor).SetFont(font).SetFontSize(10));

            return new Cell()
                .Add(p)
                .SetBorder(new SolidBorder(new DeviceRgb(210, 210, 210), 1))
                .SetPadding(6);
        }
        private void ExportarReporteAsistenciaPdf(string rutaArchivo)
        {
            DateTime fechaInicial = new DateTime(anioActual, mesActual, 1);
            DateTime fechaFinal = fechaInicial.AddMonths(1).AddDays(-1);

            using (PdfWriter writer = new PdfWriter(rutaArchivo))
            using (PdfDocument pdf = new PdfDocument(writer))
            using (Document document = new Document(pdf, PageSize.A4.Rotate()))
            {
                document.SetMargins(20, 20, 20, 20);

                string rutaSegoe = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                "segoeui.ttf"
            );

                string rutaSegoeBold = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                    "segoeuib.ttf"
                );

                PdfFont font = PdfFontFactory.CreateFont(
                    rutaSegoe,
                    iText.IO.Font.PdfEncodings.IDENTITY_H
                );

                PdfFont fontBold = PdfFontFactory.CreateFont(
                    rutaSegoeBold,
                    iText.IO.Font.PdfEncodings.IDENTITY_H
                );

                // Colores
                DeviceRgb verdeOscuro = new DeviceRgb(0, 100, 0);
                DeviceRgb verdeClaro = new DeviceRgb(92, 184, 92);
                DeviceRgb azul = new DeviceRgb(0, 102, 204);
                DeviceRgb verdeTP = new DeviceRgb(120, 220, 80);
                DeviceRgb rojoTF = new DeviceRgb(255, 0, 0);
                DeviceRgb amarilloTE = new DeviceRgb(255, 215, 0);
                DeviceRgb grisBorde = new DeviceRgb(210, 210, 210);

                // Encabezado
                document.Add(
                    new Paragraph("ATLANTIC ACADEMY BILINGUAL SCHOOL")
                        .SetFont(font)
                        .SetFontSize(14)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(2)
                );

                document.Add(
                    new Paragraph("REPORTE DE ASISTENCIA DIARIA POR SECCIÓN")
                        .SetFont(fontBold)
                        .SetFontSize(18)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(10)
                );

                // Datos filtro
                Table tablaInfo = new Table(UnitValue.CreatePercentArray(new float[] { 2, 2, 2, 3 }))
                    .UseAllAvailableWidth();

                tablaInfo.AddCell(CrearCeldaInfo("DOCENTE", lblDocente.Text, font, fontBold));
                tablaInfo.AddCell(CrearCeldaInfo("GRADO", cbGrado.Text, font, fontBold));
                tablaInfo.AddCell(CrearCeldaInfo("SECCIÓN", cbSeccion.Text, font, fontBold));
                tablaInfo.AddCell(CrearCeldaInfo("FECHA", $"{fechaInicial:dd/MM/yyyy} AL {fechaFinal:dd/MM/yyyy}", font, fontBold));

                document.Add(tablaInfo);

                // Barra verde
                document.Add(
                    new Paragraph($"{fechaInicial.ToString("MMMM yyyy").ToUpper()} DEL {fechaInicial:dd} AL {fechaFinal:dd}")
                        .SetFont(fontBold)
                        .SetFontSize(12)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetBackgroundColor(verdeOscuro)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(6)
                        .SetMarginTop(8)
                        .SetMarginBottom(8)
                );

                // Tabla principal
                int totalColumnas = dgvAsistencia.Columns.Count;
                float[] widths = new float[totalColumnas];

                for (int i = 0; i < totalColumnas; i++)
                {
                    string nombre = dgvAsistencia.Columns[i].Name;

                    if (nombre == "ID") widths[i] = 35;
                    else if (nombre == "Nombre") widths[i] = 180;
                    else if (nombre == "TP" || nombre == "TF" || nombre == "TE") widths[i] = 40;
                    else widths[i] = 28;
                }

                Table tabla = new Table(UnitValue.CreatePointArray(widths));
                tabla.SetWidth(UnitValue.CreatePercentValue(100));

                // Headers
                foreach (DataGridViewColumn col in dgvAsistencia.Columns)
                {
                    DeviceRgb fondo = verdeOscuro;
                    iText.Kernel.Colors.Color colorTexto = ColorConstants.WHITE;

                    if (col.Name == "ID")
                        fondo = azul;
                    else if (col.Name == "TP")
                        fondo = verdeTP;
                    else if (col.Name == "TF")
                        fondo = rojoTF;
                    else if (col.Name == "TE")
                    {
                        fondo = amarilloTE;
                        colorTexto = ColorConstants.BLACK;
                    }
                    else if (col.Name.StartsWith("D"))
                        fondo = verdeClaro;

                    tabla.AddHeaderCell(
                        new Cell()
                            .Add(new Paragraph(col.HeaderText).SetFont(fontBold).SetFontSize(9))
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                            .SetBackgroundColor(fondo)
                            .SetFontColor(colorTexto)
                            .SetBorder(new SolidBorder(grisBorde, 1))
                            .SetPadding(4)
                    );
                }

                // Filas
                foreach (DataGridViewRow row in dgvAsistencia.Rows)
                {
                    if (row.IsNewRow) continue;

                    foreach (DataGridViewColumn col in dgvAsistencia.Columns)
                    {
                        string valor = row.Cells[col.Name].Value?.ToString() ?? "";
                        DeviceRgb? fondo = null;
                        iText.Kernel.Colors.Color colorTexto = ColorConstants.BLACK;
                        PdfFont fontActual = font;

                        if (col.Name == "TP")
                        {
                            fondo = verdeTP;
                            colorTexto = ColorConstants.WHITE;
                            fontActual = fontBold;
                        }
                        else if (col.Name == "TF")
                        {
                            fondo = rojoTF;
                            colorTexto = ColorConstants.WHITE;
                            fontActual = fontBold;
                        }
                        else if (col.Name == "TE")
                        {
                            fondo = amarilloTE;
                            colorTexto = ColorConstants.BLACK;
                            fontActual = fontBold;
                        }
                        else if (col.Name.StartsWith("D"))
                        {
                            if (valor == "●")
                            {
                                colorTexto = new DeviceRgb(110, 193, 74);
                                fontActual = fontBold;
                            }
                            else if (valor == "X")
                            {
                                colorTexto = ColorConstants.RED;
                                fontActual = fontBold;
                            }
                            else if (valor == "E")
                            {
                                colorTexto = new DeviceRgb(184, 134, 11);
                                fontActual = fontBold;
                            }
                            else if (valor == "F" || valor == "I" || valor == "N" || valor == "D")
                            {
                                colorTexto = ColorConstants.BLACK;
                                fontActual = fontBold;
                            }
                        }

                        Paragraph p = new Paragraph(valor).SetFont(fontActual).SetFontSize(9);

                        Cell cell = new Cell()
                            .Add(p)
                            .SetBorder(new SolidBorder(grisBorde, 1))
                            .SetPadding(4)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE);

                        if (col.Name == "Nombre")
                            cell.SetTextAlignment(TextAlignment.LEFT);
                        else
                            cell.SetTextAlignment(TextAlignment.CENTER);

                        if (fondo != null)
                            cell.SetBackgroundColor(fondo);

                        cell.SetFontColor(colorTexto);

                        tabla.AddCell(cell);
                    }
                }

                document.Add(tabla);

                document.Add(
                    new Paragraph(lblRegistros.Text)
                        .SetFont(font)
                        .SetFontSize(9)
                        .SetMarginTop(8)
                );
            }
        }

        private void TimerBusqueda_Tick(object? sender, EventArgs e)
        {
            timerBusqueda.Stop();

            string texto = txtBuscar.Text.Trim();

            // Si está vacío, recarga todo
            if (string.IsNullOrWhiteSpace(texto))
            {
                CargarReporte();
                return;
            }

            // Si no llega al mínimo, no consultes todavía
            if (texto.Length < MinimoCaracteresBusqueda)
                return;

            CargarReporte();
        }
        private void ActualizarLabelRegistros()
        {
            int total = dgvAsistencia.Rows.Count;

            // Si tienes fila nueva habilitada
            if (dgvAsistencia.AllowUserToAddRows)
                total--;

            if (total < 0) total = 0;

            if (total == 0)
            {
                lblRegistros.Text = "Sin registros";
            }
            else
            {
                lblRegistros.Text = $"Registros del 1 al {total} total de {total} registros";
            }
        }
        private void CargarGrados()
        {
            cbGrado.Items.Clear();
            cbSeccion.Items.Clear();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                    select distinct
                    G.NombreGrado,
                    case
                        when G.NombreGrado = 'KINDER' then 1
                        when G.NombreGrado = 'PREBASICA' then 2
                        when G.NombreGrado = 'PRIMERO' then 3
                        when G.NombreGrado = 'SEGUNDO' then 4
                        when G.NombreGrado = 'TERCERO' then 5
                        when G.NombreGrado = 'CUARTO' then 6
                        when G.NombreGrado = 'QUINTO' then 7
                        when G.NombreGrado = 'SEXTO' then 8
                        when G.NombreGrado = 'SEPTIMO' then 9
                        when G.NombreGrado = 'OCTAVO' then 10
                        when G.NombreGrado = 'NOVENO' then 11
                        when G.NombreGrado = 'DECIMO' then 12
                        when G.NombreGrado = 'UNDECIMO' then 13
                        else 99
                    end as OrdenGrado
                from CargaAcademica CA
                inner join Seccion S on CA.SeccionID = S.SeccionID
                inner join Grado G on S.GradoID = G.GradoID
                inner join Docente D on CA.DocenteID = D.DocenteID
                where D.UsuarioID = @Docente
                order by OrdenGrado;", cn))
            {
                cmd.Parameters.AddWithValue("@Docente", docenteId);
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
                select distinct S.Letra
                from CargaAcademica CA
                inner join Seccion S on CA.SeccionID = S.SeccionID
                inner join Grado G on S.GradoID = G.GradoID
                inner join Docente D on CA.DocenteID = D.DocenteID
                where D.UsuarioID = @Docente
                  and G.NombreGrado = @Grado
                order by S.Letra", cn))
            {
                cmd.Parameters.AddWithValue("@Docente", docenteId);
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
            {
                cbSeccion.SelectedIndex = 0;
                CargarReporte();
            }
        }
        public class MesItem
        {
            public int Anio { get; set; }
            public int Mes { get; set; }

            public override string ToString()
            {
                return new DateTime(Anio, Mes, 1)
                    .ToString("MMMM yyyy")
                    .ToUpper();
            }
        }
        private void CargarMeses()
        {
            cbMes.Items.Clear();

            if (cbGrado.SelectedItem == null || cbSeccion.SelectedItem == null)
                return;

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                select distinct
                    year(A.Fecha) as Anio,
                    month(A.Fecha) as Mes
                from Asistencia A
                inner join CargaAcademica CA on A.CargaID = CA.CargaID
                inner join Seccion S on CA.SeccionID = S.SeccionID
                inner join Grado G on S.GradoID = G.GradoID
                inner join Docente D on CA.DocenteID = D.DocenteID
                where D.UsuarioID = @Docente
                  and G.NombreGrado = @Grado
                  and S.Letra = @Seccion
            order by Anio, Mes", cn))
            {
                cmd.Parameters.AddWithValue("@Docente", docenteId);
                cmd.Parameters.AddWithValue("@Grado", cbGrado.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@Seccion", cbSeccion.SelectedItem.ToString());

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cbMes.Items.Add(new MesItem
                        {
                            Anio = Convert.ToInt32(dr["Anio"]),
                            Mes = Convert.ToInt32(dr["Mes"])
                        });
                    }
                }
            }

            if (cbMes.Items.Count > 0)
                cbMes.SelectedIndex = cbMes.Items.Count - 1; // último mes (más reciente)
        }

        private void cbMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMes.SelectedItem is MesItem mes)
            {
                anioActual = mes.Anio;
                mesActual = mes.Mes;

                CargarReporte();
            }
        }
        private string ObtenerNombreDocente()
        {
            string nombre = "";

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                select Nombre
                from Docente
                where UsuarioID = @Docente", cn))
            {
                cmd.Parameters.AddWithValue("@Docente", docenteId);
                cn.Open();

                object resultado = cmd.ExecuteScalar();

                if (resultado != null)
                    nombre = resultado.ToString();
            }

            return nombre;
        }

        private void cbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarSecciones();

            if (cbSeccion.Items.Count > 0)
                cbSeccion.SelectedIndex = 0;
        }

        private void cbSeccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarMeses();
        }
        private void CargarReporte()
        {
            if (cbGrado.SelectedItem == null || cbSeccion.SelectedItem == null)
                return;

            DateTime fechaInicial = new DateTime(anioActual, mesActual, 1);
            DateTime fechaFinal = fechaInicial.AddMonths(1).AddDays(-1);

            string estudiante = txtBuscar.Text.Trim();
            string grado = cbGrado.SelectedItem.ToString();
            string seccion = cbSeccion.SelectedItem.ToString();

            DataTable dt = ObtenerAsistencias(
                fechaInicial,
                fechaFinal,
                docenteId,
                estudiante,
                grado,
                seccion
            );

            ConfigurarDataGridView(anioActual, mesActual);
            LlenarGridDesdeDataTable(dt, anioActual, mesActual);
            ActualizarLabelRegistros();

            ActualizarTituloMes();

            foreach (DataGridViewColumn col in dgvAsistencia.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void ActualizarTituloMes()
        {
            DateTime fechaInicial = new DateTime(anioActual, mesActual, 1);
            DateTime fechaFinal = fechaInicial.AddMonths(1).AddDays(-1);

            if (ocultarFinesDeSemana)
                lblMes.Text = $"{fechaInicial.ToString("MMMM yyyy").ToUpper()} SOLO DÍAS HÁBILES";
            else
                lblMes.Text = $"{fechaInicial.ToString("MMMM yyyy").ToUpper()} DEL {fechaInicial:dd} AL {fechaFinal:dd}";
        }

        private DataTable ObtenerAsistencias(
             DateTime fechaInicial,
             DateTime fechaFinal,
             int docente,
             string estudiante,
             string grado,
             string seccion)
        {
            DataTable dt = new DataTable();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("spMAE_Asistencias_por_Grado", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@fecha_inicial", fechaInicial);
                cmd.Parameters.AddWithValue("@fecha_final", fechaFinal);
                cmd.Parameters.AddWithValue("@Docente", docente);
                cmd.Parameters.AddWithValue("@Grado", grado);
                cmd.Parameters.AddWithValue("@Seccion", seccion);

                if (string.IsNullOrWhiteSpace(estudiante))
                    cmd.Parameters.AddWithValue("@Estudiante", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@Estudiante", estudiante.Trim());

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }
        //private void btnAnterior_Click(object sender, EventArgs e)
        //{
        //    DateTime fecha = new DateTime(anioActual, mesActual, 1).AddMonths(-1);
        //    anioActual = fecha.Year;
        //    mesActual = fecha.Month;
        //    CargarReporte();
        //}

        //private void btnSiguiente_Click(object sender, EventArgs e)
        //{
        //    DateTime fecha = new DateTime(anioActual, mesActual, 1).AddMonths(1);
        //    anioActual = fecha.Year;
        //    mesActual = fecha.Month;
        //    CargarReporte();
        //}
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string texto = txtBuscar.Text.Trim();

            if (!string.IsNullOrWhiteSpace(texto) && texto.Length < MinimoCaracteresBusqueda)
            {
                MessageBox.Show(
                    $"Ingrese al menos {MinimoCaracteresBusqueda} caracteres para buscar.",
                    "Búsqueda",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            CargarReporte();
        }
        private void lblTitulo_Click(object sender, EventArgs e)
        {
            this.Text = "Reporte de Asistencia";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
            this.WindowState = FormWindowState.Maximized;
        }

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void dvgAsistencia_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void ConfigurarDataGridView(int anio, int mes)
        {
            dgvAsistencia.Columns.Clear();
            dgvAsistencia.Rows.Clear();

            List<DateTime> fechasVisibles = ObtenerFechasVisiblesDelMes(anio, mes);

            dgvAsistencia.AllowUserToAddRows = false;
            dgvAsistencia.AllowUserToDeleteRows = false;
            dgvAsistencia.AllowUserToResizeRows = false;
            dgvAsistencia.ReadOnly = true;
            dgvAsistencia.RowHeadersVisible = false;
            dgvAsistencia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAsistencia.MultiSelect = false;
            dgvAsistencia.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvAsistencia.BackgroundColor = System.Drawing.Color.White;
            dgvAsistencia.BorderStyle = BorderStyle.None;
            dgvAsistencia.EnableHeadersVisualStyles = false;
            dgvAsistencia.ColumnHeadersHeight = 40;
            dgvAsistencia.RowTemplate.Height = 40;
            dgvAsistencia.ScrollBars = ScrollBars.Both;

            dgvAsistencia.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvAsistencia.GridColor = System.Drawing.Color.FromArgb(210, 210, 210);

            dgvAsistencia.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvAsistencia.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;

            dgvAsistencia.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(0, 100, 0);
            dgvAsistencia.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvAsistencia.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvAsistencia.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvAsistencia.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAsistencia.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvAsistencia.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
            dgvAsistencia.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

            dgvAsistencia.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvAsistencia.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 248, 248);

            dgvAsistencia.RowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            dgvAsistencia.AlternatingRowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(235, 235, 235);

            var colId = new DataGridViewTextBoxColumn();
            colId.Name = "ID";
            colId.HeaderText = "N°";
            colId.Width = 50;
            colId.Frozen = true;
            dgvAsistencia.Columns.Add(colId);

            var colNombre = new DataGridViewTextBoxColumn();
            colNombre.Name = "Nombre";
            colNombre.HeaderText = "ESTUDIANTES";
            colNombre.Width = 220;
            colNombre.Frozen = true;
            colNombre.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvAsistencia.Columns.Add(colNombre);

            var colTP = new DataGridViewTextBoxColumn();
            colTP.Name = "TP";
            colTP.HeaderText = "TP";
            colTP.Width = 50;
            colTP.Frozen = true;
            colTP.HeaderCell.Style.BackColor = System.Drawing.Color.FromArgb(120, 220, 80);
            colTP.HeaderCell.Style.ForeColor = System.Drawing.Color.White;
            dgvAsistencia.Columns.Add(colTP);

            var colTF = new DataGridViewTextBoxColumn();
            colTF.Name = "TF";
            colTF.HeaderText = "TF";
            colTF.Width = 50;
            colTF.Frozen = true;
            colTF.HeaderCell.Style.BackColor = System.Drawing.Color.Red;
            colTF.HeaderCell.Style.ForeColor = System.Drawing.Color.White;
            dgvAsistencia.Columns.Add(colTF);

            var colTE = new DataGridViewTextBoxColumn();
            colTE.Name = "TE";
            colTE.HeaderText = "TE";
            colTE.Width = 50;
            colTE.Frozen = true;
            colTE.HeaderCell.Style.BackColor = System.Drawing.Color.Gold;
            colTE.HeaderCell.Style.ForeColor = System.Drawing.Color.White;
            dgvAsistencia.Columns.Add(colTE);

            foreach (DateTime fecha in fechasVisibles)
            {
                var colDia = new DataGridViewTextBoxColumn();
                colDia.Name = $"D{fecha.Day}";
                colDia.HeaderText = fecha.Day.ToString();
                colDia.Width = 42;
                colDia.HeaderCell.Style.BackColor = System.Drawing.Color.FromArgb(92, 184, 92);
                colDia.HeaderCell.Style.ForeColor = System.Drawing.Color.White;
                dgvAsistencia.Columns.Add(colDia);
            }

            dgvAsistencia.CellFormatting -= dgvAsistencia_CellFormatting;
            dgvAsistencia.CellFormatting += dgvAsistencia_CellFormatting;

            dgvAsistencia.CellPainting -= dgvAsistencia_CellPainting;
            dgvAsistencia.CellPainting += dgvAsistencia_CellPainting;
        }
        private void dgvAsistencia_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string nombreColumna = dgvAsistencia.Columns[e.ColumnIndex].Name;

            if (!nombreColumna.StartsWith("D")) return;

            string valor = dgvAsistencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";

            System.Drawing.Color? colorTexto = null;
            Font fuente = new Font("Segoe UI", 10, FontStyle.Bold);

            if (valor == "●")
            {
                colorTexto = System.Drawing.Color.FromArgb(110, 193, 74);
                fuente = new Font("Segoe UI", 14, FontStyle.Bold);
            }
            else if (valor == "X")
            {
                colorTexto = System.Drawing.Color.Red;
            }
            else if (valor == "E")
            {
                colorTexto = System.Drawing.Color.Goldenrod;
            }
            else if (valor == "F" || valor == "I" || valor == "N" || valor == "D")
            {
                colorTexto = System.Drawing.Color.Black;
            }

            if (colorTexto == null) return;

            e.Handled = true;

            // 👇 pinta fondo + borde
            e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

            TextRenderer.DrawText(
                e.Graphics,
                valor,
                fuente,
                e.CellBounds,
                colorTexto.Value,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );
        }
        private void dgvAsistencia_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            string valor = e.Value.ToString() ?? "";

            // SOLO PARA COLUMNAS DE DÍAS
            if (dgvAsistencia.Columns[e.ColumnIndex].Name.StartsWith("D"))
            {
                // PRESENTE (● VERDE)
                if (valor == "●")
                {
                    e.Value = "●";
                    e.CellStyle.ForeColor = System.Drawing.Color.FromArgb(110, 193, 74); // verde bonito
                    e.CellStyle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                    e.FormattingApplied = true;
                }

                // FALTA (X ROJA)
                else if (valor == "X")
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.Red;
                    e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }

                // EXCUSA (E AMARILLA)
                else if (valor == "E")
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.Goldenrod;
                    e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }

                // OTROS (F, I, N, D)
                else if (valor == "F" || valor == "I" || valor == "N" || valor == "D")
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.Black;
                    e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
            }

            // 🎯 COLORES DE TOTALES
            if (dgvAsistencia.Columns[e.ColumnIndex].Name == "TP")
            {
                e.CellStyle.BackColor = System.Drawing.Color.FromArgb(120, 220, 80);
                e.CellStyle.ForeColor = System.Drawing.Color.White;
                e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            if (dgvAsistencia.Columns[e.ColumnIndex].Name == "TF")
            {
                e.CellStyle.BackColor = System.Drawing.Color.Red;
                e.CellStyle.ForeColor = System.Drawing.Color.White;
                e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            if (dgvAsistencia.Columns[e.ColumnIndex].Name == "TE")
            {
                e.CellStyle.BackColor = System.Drawing.Color.Gold;
                e.CellStyle.ForeColor = System.Drawing.Color.Black;
                e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }
        }

        private void LlenarGridDesdeDataTable(DataTable dt, int anio, int mes)
        {
            dgvAsistencia.Rows.Clear();

            if (dt == null || dt.Rows.Count == 0)
                return;

            List<DateTime> fechasVisibles = ObtenerFechasVisiblesDelMes(anio, mes);

            var grupos = dt.AsEnumerable()
                .GroupBy(r => r["Estudiante"].ToString())
                .OrderBy(g => g.Key)
                .ToList();

            int correlativo = 1;

            foreach (var grupo in grupos)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvAsistencia);

                row.Cells[dgvAsistencia.Columns["ID"].Index].Value = correlativo.ToString();
                row.Cells[dgvAsistencia.Columns["Nombre"].Index].Value = grupo.Key;
                row.Cells[dgvAsistencia.Columns["TP"].Index].Value = "0";
                row.Cells[dgvAsistencia.Columns["TF"].Index].Value = "0";
                row.Cells[dgvAsistencia.Columns["TE"].Index].Value = "0";

                foreach (DateTime fecha in fechasVisibles)
                {
                    string nombreColumna = $"D{fecha.Day}";
                    if (dgvAsistencia.Columns.Contains(nombreColumna))
                        row.Cells[dgvAsistencia.Columns[nombreColumna].Index].Value = "";
                }

                foreach (var item in grupo
                             .Where(x => x["Fecha"] != DBNull.Value && x["Estado"] != DBNull.Value)
                             .GroupBy(x => Convert.ToDateTime(x["Fecha"]).Date)
                             .Select(g => g.First()))
                {
                    DateTime fecha = Convert.ToDateTime(item["Fecha"]).Date;

                    if (fecha.Year != anio || fecha.Month != mes)
                        continue;

                    if (ocultarFinesDeSemana &&
                        (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday))
                        continue;

                    string nombreColumna = $"D{fecha.Day}";
                    if (!dgvAsistencia.Columns.Contains(nombreColumna))
                        continue;

                    string estado = item["Estado"]?.ToString()?.Trim().ToUpper() ?? "";
                    row.Cells[dgvAsistencia.Columns[nombreColumna].Index].Value = MapearEstadoASimbolo(estado);
                }

                dgvAsistencia.Rows.Add(row);
                correlativo++;
            }

            CalcularTotalesTodasLasFilas();
        }
        private string MapearEstadoASimbolo(string estado)
        {
            switch (estado)
            {
                case "P":
                case "PRESENTE":
                case "ASISTIO":
                case "ASISTIÓ":
                    return "●";

                case "E":
                case "EXCUSA":
                case "EXCUSADO":
                case "JUSTIFICADO":
                case "TARDE":
                    return "E";

                case "F":
                case "FALTA":
                case "AUSENTE":
                    return "X";

                case "I":
                    return "I";

                case "N":
                    return "N";

                case "D":
                    return "D";

                default:
                    return "";
            }
        }
        private void CalcularTotalesTodasLasFilas()
        {
            foreach (DataGridViewRow fila in dgvAsistencia.Rows)
            {
                if (!fila.IsNewRow)
                    CalcularTotalesFila(fila);
            }
        }

        private void CalcularTotalesFila(DataGridViewRow fila)
        {
            int tp = 0;
            int tf = 0;
            int te = 0;

            foreach (DataGridViewColumn col in dgvAsistencia.Columns)
            {
                if (col.Name.StartsWith("D"))
                {
                    string valor = fila.Cells[col.Name].Value?.ToString() ?? "";

                    if (valor == "●")
                        tp++;
                    else if (valor == "X" || valor == "F" || valor == "I" || valor == "N" || valor == "D")
                        tf++;
                    else if (valor == "E")
                        te++;
                }
            }

            fila.Cells["TP"].Value = tp;
            fila.Cells["TF"].Value = tf;
            fila.Cells["TE"].Value = te;
        }

        private void guna2HtmlLabel7_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

}
