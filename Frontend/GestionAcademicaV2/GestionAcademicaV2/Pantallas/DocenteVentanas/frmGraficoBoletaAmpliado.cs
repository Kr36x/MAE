using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class frmGraficoBoletaAmpliado : Form
    {
        private readonly DataTable dtGrafico;

        public frmGraficoBoletaAmpliado(DataTable dt)
        {
            InitializeComponent();

            dtGrafico = dt?.Copy() ?? new DataTable();

            this.Text = "Gráfico ampliado";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;
            this.DoubleBuffered = true;
            this.Paint += FrmGraficoBoletaAmpliado_Paint;
        }

        private void FrmGraficoBoletaAmpliado_Paint(object sender, PaintEventArgs e)
        {
            DibujarGraficoHorizontal(e.Graphics, this.ClientRectangle);
        }

        private List<DataRow> ObtenerTodasLasFilas()
        {
            List<DataRow> filas = new List<DataRow>();

            if (dtGrafico == null || dtGrafico.Rows.Count == 0)
                return filas;

            foreach (DataRow row in dtGrafico.Rows)
                filas.Add(row);

            return filas;
        }

        private string ObtenerPrimerNombreAsignatura(string asignatura)
        {
            if (string.IsNullOrWhiteSpace(asignatura))
                return "N/A";

            return asignatura.Split('/')[0].Trim().ToUpper();
        }

        private void DibujarGraficoHorizontal(Graphics g, Rectangle areaTotal)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            var filas = ObtenerTodasLasFilas();

            if (filas.Count == 0)
            {
                TextRenderer.DrawText(
                    g,
                    "No hay datos para mostrar",
                    new Font("Segoe UI", 14, FontStyle.Regular),
                    areaTotal,
                    Color.Gray,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
                return;
            }

            int margenIzq = 260;
            int margenDer = 80;
            int margenSup = 70;
            int margenInf = 50;

            Rectangle areaGrafico = new Rectangle(
                areaTotal.X + margenIzq,
                areaTotal.Y + margenSup,
                areaTotal.Width - margenIzq - margenDer,
                areaTotal.Height - margenSup - margenInf
            );

            using Pen penGrid = new Pen(Color.FromArgb(220, 220, 220), 1);
            using Font fontEje = new Font("Segoe UI", 9);
            using Font fontAsignatura = new Font("Segoe UI", 9, FontStyle.Bold);
            using Font fontValor = new Font("Segoe UI", 9, FontStyle.Bold);

            // Líneas verticales del eje X
            for (int v = 0; v <= 100; v += 10)
            {
                int x = areaGrafico.Left + (int)(areaGrafico.Width * (v / 100f));
                g.DrawLine(penGrid, x, areaGrafico.Top, x, areaGrafico.Bottom);

                Rectangle rectLabel = new Rectangle(x - 15, areaGrafico.Bottom + 5, 30, 20);
                TextRenderer.DrawText(
                    g,
                    v.ToString(),
                    fontEje,
                    rectLabel,
                    Color.Black,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
            }

            int cantidad = filas.Count;
            int espacio = 10;
            int altoDisponible = areaGrafico.Height - ((cantidad - 1) * espacio);
            int altoBarra = Math.Max(24, altoDisponible / cantidad);

            for (int i = 0; i < cantidad; i++)
            {
                DataRow row = filas[i];

                string asignatura = row["Asignatura"]?.ToString() ?? "";
                string etiqueta = ObtenerPrimerNombreAsignatura(asignatura);

                decimal promedio = row["PromedioClase"] == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(row["PromedioClase"]);

                string estado = row["Estado"]?.ToString()?.ToUpper() ?? "";

                Color colorBarra = Color.FromArgb(255, 77, 79);
                if (estado == "EXCELENTE")
                    colorBarra = Color.FromArgb(92, 184, 92);
                else if (estado == "MEDIO")
                    colorBarra = Color.Goldenrod;

                int anchoBarra = (int)(areaGrafico.Width * ((float)promedio / 100f));
                int y = areaGrafico.Top + i * (altoBarra + espacio);

                Rectangle rectTexto = new Rectangle(
                    20,
                    y,
                    margenIzq - 30,
                    altoBarra
                );

                TextRenderer.DrawText(
                    g,
                    etiqueta,
                    fontAsignatura,
                    rectTexto,
                    Color.Black,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
                );

                Rectangle rectBarra = new Rectangle(
                    areaGrafico.Left,
                    y,
                    anchoBarra,
                    altoBarra
                );

                using (SolidBrush brush = new SolidBrush(colorBarra))
                    g.FillRectangle(brush, rectBarra);

                Rectangle rectValor = new Rectangle(
                    rectBarra.Right + 8,
                    y,
                    40,
                    altoBarra
                );

                TextRenderer.DrawText(
                    g,
                    promedio.ToString("0"),
                    fontValor,
                    rectValor,
                    Color.Black,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                );
            }

            Rectangle rectTitulo = new Rectangle(0, 20, areaTotal.Width, 30);
            TextRenderer.DrawText(
                g,
                "INDICADOR DE RENDIMIENTO ACADÉMICO POR ASIGNATURA",
                new Font("Segoe UI", 16, FontStyle.Bold),
                rectTitulo,
                Color.FromArgb(24, 105, 255),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );

            Rectangle rectTituloX = new Rectangle(areaGrafico.Left, areaTotal.Bottom - 30, areaGrafico.Width, 20);
            TextRenderer.DrawText(
                g,
                "PROMEDIO (%)",
                new Font("Segoe UI", 9, FontStyle.Regular),
                rectTituloX,
                Color.Black,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );
        }
    }
}