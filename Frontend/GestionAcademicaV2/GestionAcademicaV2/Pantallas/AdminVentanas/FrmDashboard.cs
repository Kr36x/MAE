using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmDashboard : Form
    {
        private PantallaAdmin pantallaPrincipal;
        EjecutarUtilidades util = new EjecutarUtilidades();
        public FrmDashboard(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }

        private void CargarGraficoPorGrado()
        {
            try
            {
                chartGrados.Series.Clear();
                chartGrados.ChartAreas.Clear();
                chartGrados.Titles.Clear();

                chartGrados.Titles.Add(
                    $"PROMEDIO POR GRADO - PARCIAL {cbbParcial.Text} ({dtpAnio.Text})"
                );

                ChartArea area = new ChartArea("MainArea");
                chartGrados.ChartAreas.Add(area);

                area.AxisX.LabelStyle.Font = new Font("Arial", 7f, FontStyle.Bold);
                area.AxisX.LabelStyle.Angle = -30;
                area.AxisX.Interval = 1;
                area.AxisX.MajorGrid.Enabled = false;

                area.AxisY.Minimum = 0;
                area.AxisY.Maximum = 100;
                area.AxisY.LabelStyle.Font = new Font("Arial", 9f, FontStyle.Bold);
                area.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 235, 235);

                area.InnerPlotPosition = new ElementPosition(8, 5, 110, 65);

                Series sBarras = new Series("PROMEDIO")
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = true,
                    Font = new Font("Arial", 8.5f, FontStyle.Bold)
                };
                sBarras["PointWidth"] = "0.7";

                Series sMeta = new Series("META")
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.LimeGreen,
                    BorderWidth = 3,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 10
                };

                chartGrados.Series.Add(sBarras);
                chartGrados.Series.Add(sMeta);

                DataTable dt = util.EjecutarSPParametros(
                    "spMAE_PromedioYExcelenciaPorParcial",
                    new SqlParameter[]
                    {
                new SqlParameter("@Anio", dtpAnio.Text),
                new SqlParameter("@Parcial", cbbParcial.SelectedItem)
                    }
                );

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string grado = dt.Rows[i]["NombreGrado"].ToString().Trim();
                    int promedio = Convert.ToInt32(dt.Rows[i]["PromedioGrado"]);

                    int idx = sBarras.Points.AddXY(i, promedio);
                    sMeta.Points.AddXY(i, 85);

                    sBarras.Points[idx].AxisLabel = grado;
                    sBarras.Points[idx].Label = promedio + "%";
                    sBarras.Points[idx].Color = Color.CornflowerBlue;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el gráfico: " + ex.Message);
            }
        }

        //private void CargarGraficoPorGrado()
        //{
        //    try
        //    {
        //        chartGrados.Series.Clear();
        //        chartGrados.ChartAreas.Clear();
        //        chartGrados.Titles.Clear();

        //        ChartArea area = new ChartArea("MainArea");
        //        chartGrados.ChartAreas.Add(area);

        //        area.AxisX.LabelStyle.Font = new Font("Arial", 7f, FontStyle.Bold);
        //        area.AxisX.LabelStyle.Angle = -30;
        //        area.AxisX.Interval = 1;
        //        area.AxisX.MajorGrid.Enabled = false;

        //        area.AxisY.Minimum = 0;
        //        area.AxisY.Maximum = 100;
        //        area.AxisY.LabelStyle.Font = new Font("Arial", 9f, FontStyle.Bold);
        //        area.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 235, 235);

        //        area.InnerPlotPosition = new ElementPosition(8, 5, 110, 65);

        //        Series sBarras = new Series("PROMEDIO")
        //        {
        //            ChartType = SeriesChartType.Column,
        //            IsValueShownAsLabel = true,
        //            Font = new Font("Arial", 8.5f, FontStyle.Bold)
        //        };
        //        sBarras["PointWidth"] = "0.7";

        //        Series sMeta = new Series("META")
        //        {
        //            ChartType = SeriesChartType.Line,
        //            Color = Color.LimeGreen,
        //            BorderWidth = 3,
        //            MarkerStyle = MarkerStyle.Circle,
        //            MarkerSize = 10
        //        };
        //        chartGrados.Series.Add(sBarras);
        //        chartGrados.Series.Add(sMeta);

        //        DataTable dt = util.EjecutarSP("spMAE_DesempenoPorGradoAnual",
        //                       new SqlParameter[] { new SqlParameter("@Anio", dtpAnio.Text) });

        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            string grado = dt.Rows[i]["NombreGrado"].ToString().Trim();
        //            int promedio = Convert.ToInt32(dt.Rows[i]["PromedioGrado"]);

        //            int idx = sBarras.Points.AddXY(i, promedio);
        //            sMeta.Points.AddXY(i, 85);

        //            sBarras.Points[idx].AxisLabel = grado;
        //            sBarras.Points[idx].Label = promedio + "%";
        //            sBarras.Points[idx].Color = Color.CornflowerBlue;
        //            //// Color Dinámico
        //            //if (promedio<70)
        //            //{
        //            //    sBarras.Points[idx].Color = Color.Yellow;
        //            //} else if (promedio>=70 && promedio <90)
        //            //{
        //            //    sBarras.Points[idx].Color = Color.Green;
        //            //}else if (promedio>=90)
        //            //{
        //            //    sBarras.Points[idx].Color = Color.Blue;
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error al cargar el gráfico: " + ex.Message);
        //    }
        //}

        //private void CargarPromediosPorGrado()
        //{
            //try
            //{
            //    EjecutarUtilidades util = new EjecutarUtilidades();

            //    SqlParameter[] p =
            //    {
            //        new SqlParameter("@Anio",(dtpAnio.Text))
            //    };

            //    DataTable dt = util.EjecutarSP("spMAE_PromedioYExcelenciaPorNivel", p);

            //    foreach (DataRow row in dt.Rows)
            //    {
            //        string grado = row["NombreGrado"].ToString();
            //        string promedio = row["PromedioGrado"].ToString();
            //        string excelencia = row["EstudiantesExcelencia"].ToString();

            //        switch (grado.ToUpper())
            //        {
            //            case "PRE-KINDER":
            //                txtPromedioPrekinder.Text = promedio + "%";
            //                txtExcelenciaPrekinder.Text = excelencia + " EST.";
            //                lbSeccionPrekinder.Text = "A";
            //                int p1 = Convert.ToInt32(promedio);

            //                if (p1 >= 90)
            //                    txtPromedioPrekinder1.FillColor = Color.ForestGreen;
            //                txtPromedioPrekinder.FillColor = Color.ForestGreen;
            //                if (p1 < 90 && p1 >= 70)
            //                    txtPromedioPrekinder1.FillColor = Color.LightYellow;
            //                txtPromedioPrekinder.FillColor = Color.LightYellow;
            //                if (p1 < 70)
            //                    txtPromedioPrekinder1.FillColor = Color.Tomato;
            //                txtPromedioPrekinder.FillColor = Color.Tomato;
            //                break;

            //            case "KINDER":
            //                txtPromedioKinder.Text = promedio + "%";
            //                txtExcelenciaKinder.Text = excelencia + " EST.";
            //                lbSeccionKinder.Text = "A";
            //                int p2 = Convert.ToInt32(promedio);

            //                if (p2 >= 90)
            //                    txtPromedioKinder1.FillColor = Color.ForestGreen;
            //                txtPromedioKinder.FillColor = Color.ForestGreen;
            //                if (p2 < 90 && p2 >= 70)
            //                    txtPromedioKinder1.FillColor = Color.LightYellow;
            //                txtPromedioKinder.FillColor = Color.LightYellow;
            //                if (p2 < 70)
            //                    txtPromedioKinder1.FillColor = Color.Tomato;
            //                txtPromedioKinder.FillColor = Color.Tomato;
            //                break;

            //            case "PREPARATORIA":
            //                txtPromedioPrepa.Text = promedio + "%";
            //                txtExcelenciaPrepa.Text = excelencia + " EST.";
            //                lbSeccionPrepa.Text = "A";
            //                int p3 = Convert.ToInt32(promedio);

            //                if (p3 >= 90)
            //                    txtPromedioPrepa1.FillColor = Color.ForestGreen;
            //                txtPromedioPrepa.FillColor = Color.ForestGreen;
            //                if (p3 < 90 && p3 >= 70)
            //                    txtPromedioPrepa1.FillColor = Color.LightYellow;
            //                txtPromedioPrepa.FillColor = Color.LightYellow;
            //                if (p3 < 70)
            //                    txtPromedioPrepa1.FillColor = Color.Tomato;
            //                txtPromedioPrepa.FillColor = Color.Tomato;
            //                break;

            //            case "PRIMERO":
            //                txtPromedioPrimero.Text = promedio + "%";
            //                txtExcelenciaPrimero.Text = excelencia + " EST.";
            //                lbSeccionPrimero.Text = "A";
            //                int p4 = Convert.ToInt32(promedio);

            //                if (p4 >= 90)
            //                    txtPromedioPrimero1.FillColor = Color.ForestGreen;
            //                txtPromedioPrimero.FillColor = Color.ForestGreen;
            //                if (p4 < 90 && p4 >= 70)
            //                    txtPromedioPrimero1.FillColor = Color.LightYellow;
            //                txtPromedioPrimero.FillColor = Color.LightYellow;
            //                if (p4 < 70)
            //                    txtPromedioPrimero1.FillColor = Color.Tomato;
            //                txtPromedioPrimero.FillColor = Color.Tomato;
            //                break;

            //            case "SEGUNDO":
            //                txtPromedioSegundo.Text = promedio + "%";
            //                txtExcelenciaSegundo.Text = excelencia + " EST.";
            //                lbSeccionSegundo.Text = "A";
            //                int p5 = Convert.ToInt32(promedio);

            //                if (p5 >= 90)
            //                    txtPromedioSegundo1.FillColor = Color.ForestGreen;
            //                txtPromedioSegundo.FillColor = Color.ForestGreen;
            //                if (p5 < 90 && p5 >= 70)
            //                    txtPromedioSegundo1.FillColor = Color.LightYellow;
            //                txtPromedioSegundo.FillColor = Color.LightYellow;
            //                if (p5 < 70)
            //                    txtPromedioSegundo1.FillColor = Color.Tomato;
            //                txtPromedioSegundo.FillColor = Color.Tomato;
            //                break;

            //            case "TERCERO":
            //                txtPromedioTercero.Text = promedio + "%";
            //                txtExcelenciaTercero.Text = excelencia + " EST.";
            //                lbSeccionTercero.Text = "A";
            //                int p6 = Convert.ToInt32(promedio);

            //                if (p6 >= 90)
            //                    txtPromedioTercero1.FillColor = Color.ForestGreen;
            //                txtPromedioTercero.FillColor = Color.ForestGreen;
            //                if (p6 < 90 && p6 >= 70)
            //                    txtPromedioTercero1.FillColor = Color.LightYellow;
            //                txtPromedioTercero.FillColor = Color.LightYellow;
            //                if (p6 < 70)
            //                    txtPromedioTercero1.FillColor = Color.Tomato;
            //                txtPromedioTercero.FillColor = Color.Tomato;
            //                break;

            //            case "CUARTO":
            //                txtPromedioCuarto.Text = promedio + "%";
            //                txtExcelenciaCuarto.Text = excelencia + " EST.";
            //                lbSeccionCuarto.Text = "A";
            //                int p7 = Convert.ToInt32(promedio);

            //                if (p7 >= 90)
            //                    txtPromedioCuarto1.FillColor = Color.ForestGreen;
            //                txtPromedioCuarto.FillColor = Color.ForestGreen;
            //                if (p7 < 90 && p7 >= 70)
            //                    txtPromedioCuarto1.FillColor = Color.LightYellow;
            //                txtPromedioCuarto.FillColor = Color.LightYellow;
            //                if (p7 < 70)
            //                    txtPromedioCuarto1.FillColor = Color.Tomato;
            //                txtPromedioCuarto.FillColor = Color.Tomato;
            //                break;

            //            case "QUINTO":
            //                txtPromedioQuinto.Text = promedio + "%";
            //                txtExcelenciaQuinto.Text = excelencia + " EST.";
            //                lbSeccionQuinto.Text = "A";
            //                int p8 = Convert.ToInt32(promedio);

            //                if (p8 >= 90)
            //                    txtPromedioQuinto1.FillColor = Color.ForestGreen;
            //                txtPromedioQuinto.FillColor = Color.ForestGreen;
            //                if (p8 < 90 && p8 >= 70)
            //                    txtPromedioQuinto1.FillColor = Color.LightYellow;
            //                txtPromedioQuinto.FillColor = Color.LightYellow;
            //                if (p8 < 70)
            //                    txtPromedioQuinto1.FillColor = Color.Tomato;
            //                txtPromedioQuinto.FillColor = Color.Tomato;
            //                break;

            //            case "SEXTO":
            //                txtPromedioSexto.Text = promedio + "%";
            //                txtExcelenciaSexto.Text = excelencia + " EST.";
            //                lbSeccionSexto.Text = "A";
            //                int p9 = Convert.ToInt32(promedio);

            //                if (p9 >= 90)
            //                    txtPromedioSexto1.FillColor = Color.ForestGreen;
            //                txtPromedioSexto.FillColor = Color.ForestGreen;
            //                if (p9 < 90 && p9 >= 70)
            //                    txtPromedioSexto1.FillColor = Color.LightYellow;
            //                txtPromedioSexto.FillColor = Color.LightYellow;
            //                if (p9 < 70)
            //                    txtPromedioSexto1.FillColor = Color.Tomato;
            //                txtPromedioSexto.FillColor = Color.Tomato;
            //                break;

            //            case "SÉPTIMO":
            //            case "SEPTIMO":
            //                txtPromedioSeptimo.Text = promedio + "%";
            //                txtExcelenciaSeptimo.Text = excelencia + " EST.";
            //                lbSeccionSeptimo.Text = "A";
            //                int p10 = Convert.ToInt32(promedio);

            //                if (p10 >= 90)
            //                    txtPromedioSeptimo1.FillColor = Color.ForestGreen;
            //                txtPromedioSeptimo.FillColor = Color.ForestGreen;
            //                if (p10 < 90 && p10 >= 70)
            //                    txtPromedioSeptimo1.FillColor = Color.LightYellow;
            //                txtPromedioSeptimo.FillColor = Color.LightYellow;
            //                if (p10 < 70)
            //                    txtPromedioSeptimo1.FillColor = Color.Tomato;
            //                txtPromedioSeptimo.FillColor = Color.Tomato;
            //                break;

            //            case "OCTAVO":
            //                txtPromedioOctavo.Text = promedio + "%";
            //                txtExcelenciaOctavo.Text = excelencia + " EST.";
            //                lbSeccionOctavo.Text = "A";
            //                int p11 = Convert.ToInt32(promedio);

            //                if (p11 >= 90)
            //                    txtPromedioOctavo1.FillColor = Color.ForestGreen;
            //                txtPromedioOctavo.FillColor = Color.ForestGreen;
            //                if (p11 < 90 && p11 >= 70)
            //                    txtPromedioOctavo1.FillColor = Color.LightYellow;
            //                txtPromedioOctavo.FillColor = Color.LightYellow;
            //                if (p11 < 70)
            //                    txtPromedioOctavo1.FillColor = Color.Tomato;
            //                txtPromedioOctavo.FillColor = Color.Tomato;
            //                break;

            //            case "NOVENO":
            //                txtPromedioNoveno.Text = promedio + "%";
            //                txtExcelenciaNoveno.Text = excelencia + " EST.";
            //                lbSeccionNoveno.Text = "A";
            //                int p12 = Convert.ToInt32(promedio);

            //                if (p12 >= 90)
            //                    txtPromedioNoveno1.FillColor = Color.ForestGreen;
            //                txtPromedioNoveno.FillColor = Color.ForestGreen;
            //                if (p12 < 90 && p12 >= 70)
            //                    txtPromedioNoveno1.FillColor = Color.LightYellow;
            //                txtPromedioNoveno.FillColor = Color.LightYellow;
            //                if (p12 < 70)
            //                    txtPromedioNoveno1.FillColor = Color.Tomato;
            //                txtPromedioNoveno.FillColor = Color.Tomato;
            //                break;

            //            case "DECIMO":
            //                txtPromedioDecimo.Text = promedio + "%";
            //                txtExcelenciaDecimo.Text = excelencia + " EST.";
            //                lbSeccionDecimo.Text = "A";
            //                int p13 = Convert.ToInt32(promedio);

            //                if (p13 >= 90)
            //                    txtPromedioDecimo1.FillColor = Color.ForestGreen;
            //                txtPromedioDecimo.FillColor = Color.ForestGreen;
            //                if (p13 < 90 && p13 >= 70)
            //                    txtPromedioDecimo1.FillColor = Color.LightYellow;
            //                txtPromedioDecimo.FillColor = Color.LightYellow;
            //                if (p13 < 70)
            //                    txtPromedioDecimo1.FillColor = Color.Tomato;
            //                txtPromedioDecimo.FillColor = Color.Tomato;
            //                break;

            //            case "UNDECIMO":
            //                txtPromedioUndecimo.Text = promedio;
            //                txtExcelenciaUndecimo.Text = excelencia + " EST.";
            //                lbSeccionUndecimo.Text = "A";
            //                int p14 = Convert.ToInt32(promedio);

            //                if (p14 >= 90)
            //                    txtPromedioUndecimo1.FillColor = Color.ForestGreen;
            //                txtPromedioUndecimo.FillColor = Color.ForestGreen;
            //                if (p14 < 90 && p14 >= 70)
            //                    txtPromedioUndecimo1.FillColor = Color.LightYellow;
            //                txtPromedioUndecimo.FillColor = Color.LightYellow;
            //                if (p14 < 70)
            //                    txtPromedioUndecimo1.FillColor = Color.Tomato;
            //                txtPromedioUndecimo.FillColor = Color.Tomato;
            //                break;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error al cargar los promedios: " + ex.Message);
            //}
        //}
        private void FrmDashboard_Load(object sender, EventArgs e)
        {
            pnlPrincipal.Visible = false;
            //ctnPrimero.Visible = false;
            //ctnSegundo.Visible = false;
            //ctnTercero.Visible = false;
            //ctnCuarto.Visible = false;
            //ctnQuinto.Visible = false;
            //ctnSexto.Visible = false;
            //ctnSeptimo.Visible = false;
            //ctnOctavo.Visible = false;
            //ctnNoveno.Visible = false;
            //ctnDecimoGrado.Visible = false;
            //ctnUndecimo.Visible = false;
            //CtnPrekinder.Visible = false;
            //ctnKinder.Visible = false;
            //ctnPreparatoria.Visible = false;
            CargarParciales();
            CargarPromediosPorGrado2();
            MostrarContenedoresPorNivel();
            CargarGraficoPorGrado();

        }
        private void CargarParciales()
        {
            cbbParcial.Items.Clear();
            cbbParcial.Items.Add("1");
            cbbParcial.Items.Add("2");
            cbbParcial.Items.Add("3");
            cbbParcial.Items.Add("4");
            cbbParcial.SelectedIndex = 0;
        }
        private void cbbNivel_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlPrincipal.Visible = true;
            MostrarContenedoresPorNivel();
            // Flexibilidad del contenedor primario
            //if (cbbNivel.SelectedIndex == 0)
            //{
            //    ctnPrimero.Visible = false;
            //    ctnSegundo.Visible = false;
            //    ctnTercero.Visible = false;
            //    ctnCuarto.Visible = false;
            //    ctnQuinto.Visible = false;
            //    ctnSexto.Visible = false;
            //    ctnSeptimo.Visible = false;
            //    ctnOctavo.Visible = false;
            //    ctnNoveno.Visible = false;
            //    ctnDecimoGrado.Visible = false;
            //    ctnUndecimo.Visible = false;
            //    CtnPrekinder.Visible = true;
            //    ctnKinder.Visible = true;
            //    ctnPreparatoria.Visible = true;
            //}
            //else if (cbbNivel.SelectedIndex == 1)
            //{
            //    ctnPrimero.Visible = true;
            //    ctnSegundo.Visible = true;
            //    ctnTercero.Visible = true;
            //    ctnCuarto.Visible = true;
            //    ctnQuinto.Visible = true;
            //    ctnSexto.Visible = true;
            //    ctnSeptimo.Visible = true;
            //    ctnOctavo.Visible = true;
            //    ctnNoveno.Visible = true;
            //    ctnDecimoGrado.Visible = false;
            //    ctnUndecimo.Visible = false;
            //    CtnPrekinder.Visible = false;
            //    ctnKinder.Visible = false;
            //    ctnPreparatoria.Visible = false;
            //}
            //else if (cbbNivel.SelectedIndex == 2)
            //{
            //    ctnPrimero.Visible = false;
            //    ctnSegundo.Visible = false;
            //    ctnTercero.Visible = false;
            //    ctnCuarto.Visible = false;
            //    ctnQuinto.Visible = false;
            //    ctnSexto.Visible = false;
            //    ctnSeptimo.Visible = false;
            //    ctnOctavo.Visible = false;
            //    ctnNoveno.Visible = false;
            //    ctnUndecimo.Visible = true;
            //    CtnPrekinder.Visible = false;
            //    ctnKinder.Visible = false;
            //    ctnPreparatoria.Visible = false;
            //    ctnDecimoGrado.Visible = true;
            //}
        }

        private bool HayDatosDelAnio()
        {
            EjecutarUtilidades util = new EjecutarUtilidades();

            SqlParameter[] p =
            {
                new SqlParameter("@Anio", dtpAnio.Text)
                };

            DataTable dt = util.EjecutarSPParametros("spMAE_PromedioYExcelenciaPorNivel", p);

            return dt.Rows.Count > 0;
        }

        private bool HayDatosDelParcial()
        {
            EjecutarUtilidades util = new EjecutarUtilidades();

            SqlParameter[] p =
            {
                new SqlParameter("@Anio", dtpAnio.Text),
                new SqlParameter("@Parcial", cbbParcial.SelectedItem)
            };

            DataTable dt = util.EjecutarSPParametros("spMAE_PromedioYExcelenciaPorParcial", p);

            return dt.Rows.Count > 0;
        }
        private void MostrarContenedoresPorNivel()
        {
            if (!HayDatosDelParcial())
            {
                OcultarContenedores();
                return;
            }

            if (cbbNivel.SelectedIndex == 0)
            {
                ctnPrimero.Visible = false;
                ctnSegundo.Visible = false;
                ctnTercero.Visible = false;
                ctnCuarto.Visible = false;
                ctnQuinto.Visible = false;
                ctnSexto.Visible = false;
                ctnSeptimo.Visible = false;
                ctnOctavo.Visible = false;
                ctnNoveno.Visible = false;
                ctnDecimoGrado.Visible = false;
                ctnUndecimo.Visible = false;

                CtnPrekinder.Visible = true;
                ctnKinder.Visible = true;
                ctnPreparatoria.Visible = true;
            }
            else if (cbbNivel.SelectedIndex == 1)
            {
                ctnPrimero.Visible = true;
                ctnSegundo.Visible = true;
                ctnTercero.Visible = true;
                ctnCuarto.Visible = true;
                ctnQuinto.Visible = true;
                ctnSexto.Visible = true;
                ctnSeptimo.Visible = true;
                ctnOctavo.Visible = true;
                ctnNoveno.Visible = true;

                ctnDecimoGrado.Visible = false;
                ctnUndecimo.Visible = false;

                CtnPrekinder.Visible = false;
                ctnKinder.Visible = false;
                ctnPreparatoria.Visible = false;
            }
            else if (cbbNivel.SelectedIndex == 2)
            {
                ctnPrimero.Visible = false;
                ctnSegundo.Visible = false;
                ctnTercero.Visible = false;
                ctnCuarto.Visible = false;
                ctnQuinto.Visible = false;
                ctnSexto.Visible = false;
                ctnSeptimo.Visible = false;
                ctnOctavo.Visible = false;
                ctnNoveno.Visible = false;

                ctnDecimoGrado.Visible = true;
                ctnUndecimo.Visible = true;

                CtnPrekinder.Visible = false;
                ctnKinder.Visible = false;
                ctnPreparatoria.Visible = false;
            }
        }

        private void cbbAnio_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            MostrarContenedoresPorNivel();
            CargarPromediosPorGrado2();
            lbAnio.Text = dtpAnio.Text;
            CargarGraficoPorGrado();
            pnlPrincipal.Visible = false;
            cbbNivel.SelectedIndex = -1;

        }

        //private void CargarPromediosPorGrado2()
        //{
        //    try
        //    {
        //        EjecutarUtilidades util = new EjecutarUtilidades();

        //        SqlParameter[] p =
        //        {
        //    new SqlParameter("@Anio", dtpAnio.Text)
        //};

        //        DataTable dt = util.EjecutarSP("spMAE_PromedioYExcelenciaPorNivel", p);

        //        if (dt.Rows.Count == 0)
        //        {
        //            OcultarContenedores();
        //            return;
        //        }

        //        MostrarContenedores();

        //        foreach (DataRow row in dt.Rows)
        //        {
        //            string grado = row["NombreGrado"].ToString();
        //            string promedio = row["PromedioGrado"].ToString();
        //            string excelencia = row["EstudiantesExcelencia"].ToString();

        //            switch (grado.ToUpper())
        //            {
        //                case "PRE-KINDER":
        //                    txtPromedioPrekinder.Text = promedio + "%";
        //                    txtExcelenciaPrekinder.Text = excelencia + " EST.";
        //                    lbSeccionPrekinder.Text = "A";
        //                    int p1 = Convert.ToInt32(promedio);

        //                    if (p1 >= 90)
        //                        txtPromedioPrekinder1.FillColor = Color.ForestGreen;
        //                    txtPromedioPrekinder.FillColor = Color.ForestGreen;
        //                    if (p1 < 90 && p1 >= 70)
        //                        txtPromedioPrekinder1.FillColor = Color.Yellow;
        //                    txtPromedioPrekinder.FillColor = Color.Yellow;
        //                    if (p1 < 70)
        //                        txtPromedioPrekinder1.FillColor = Color.Tomato;
        //                    txtPromedioPrekinder.FillColor = Color.Tomato;
        //                    break;

        //                case "KINDER":
        //                    txtPromedioKinder.Text = promedio + "%";
        //                    txtExcelenciaKinder.Text = excelencia + " EST.";
        //                    lbSeccionKinder.Text = "A";
        //                    int p2 = Convert.ToInt32(promedio);

        //                    if (p2 >= 90)
        //                        txtPromedioKinder1.FillColor = Color.ForestGreen;
        //                    txtPromedioKinder.FillColor = Color.ForestGreen;
        //                    if (p2 < 90 && p2 >= 70)
        //                        txtPromedioKinder1.FillColor = Color.Yellow;
        //                    txtPromedioKinder.FillColor = Color.Yellow;
        //                    if (p2 < 70)
        //                        txtPromedioKinder1.FillColor = Color.Tomato;
        //                    txtPromedioKinder.FillColor = Color.Tomato;
        //                    break;

        //                case "PREPARATORIA":
        //                    txtPromedioPrepa.Text = promedio + "%";
        //                    txtExcelenciaPrepa.Text = excelencia + " EST.";
        //                    lbSeccionPrepa.Text = "A";
        //                    int p3 = Convert.ToInt32(promedio);

        //                    if (p3 >= 90)
        //                        txtPromedioPrepa1.FillColor = Color.ForestGreen;
        //                    txtPromedioPrepa.FillColor = Color.ForestGreen;
        //                    if (p3 < 90 && p3 >= 70)
        //                        txtPromedioPrepa1.FillColor = Color.Yellow;
        //                    txtPromedioPrepa.FillColor = Color.Yellow;
        //                    if (p3 < 70)
        //                        txtPromedioPrepa1.FillColor = Color.Tomato;
        //                    txtPromedioPrepa.FillColor = Color.Tomato;
        //                    break;

        //                case "PRIMERO":
        //                    txtPromedioPrimero.Text = promedio + "%";
        //                    txtExcelenciaPrimero.Text = excelencia + " EST.";
        //                    lbSeccionPrimero.Text = "A";
        //                    int p4 = Convert.ToInt32(promedio);

        //                    if (p4 >= 90)
        //                        txtPromedioPrimero1.FillColor = Color.ForestGreen;
        //                    txtPromedioPrimero.FillColor = Color.ForestGreen;
        //                    if (p4 < 90 && p4 >= 70)
        //                        txtPromedioPrimero1.FillColor = Color.Yellow;
        //                    txtPromedioPrimero.FillColor = Color.Yellow;
        //                    if (p4 < 70)
        //                        txtPromedioPrimero1.FillColor = Color.Tomato;
        //                    txtPromedioPrimero.FillColor = Color.Tomato;
        //                    break;

        //                case "SEGUNDO":
        //                    txtPromedioSegundo.Text = promedio + "%";
        //                    txtExcelenciaSegundo.Text = excelencia + " EST.";
        //                    lbSeccionSegundo.Text = "A";
        //                    int p5 = Convert.ToInt32(promedio);

        //                    if (p5 >= 90)
        //                        txtPromedioSegundo1.FillColor = Color.ForestGreen;
        //                    txtPromedioSegundo.FillColor = Color.ForestGreen;
        //                    if (p5 < 90 && p5 >= 70)
        //                        txtPromedioSegundo1.FillColor = Color.Yellow;
        //                    txtPromedioSegundo.FillColor = Color.Yellow;
        //                    if (p5 < 70)
        //                        txtPromedioSegundo1.FillColor = Color.Tomato;
        //                    txtPromedioSegundo.FillColor = Color.Tomato;
        //                    break;

        //                case "TERCERO":
        //                    txtPromedioTercero.Text = promedio + "%";
        //                    txtExcelenciaTercero.Text = excelencia + " EST.";
        //                    lbSeccionTercero.Text = "A";
        //                    int p6 = Convert.ToInt32(promedio);

        //                    if (p6 >= 90)
        //                        txtPromedioTercero1.FillColor = Color.ForestGreen;
        //                    txtPromedioTercero.FillColor = Color.ForestGreen;
        //                    if (p6 < 90 && p6 >= 70)
        //                        txtPromedioTercero1.FillColor = Color.Yellow;
        //                    txtPromedioTercero.FillColor = Color.Yellow;
        //                    if (p6 < 70)
        //                        txtPromedioTercero1.FillColor = Color.Tomato;
        //                    txtPromedioTercero.FillColor = Color.Tomato;
        //                    break;

        //                case "CUARTO":
        //                    txtPromedioCuarto.Text = promedio + "%";
        //                    txtExcelenciaCuarto.Text = excelencia + " EST.";
        //                    lbSeccionCuarto.Text = "A";
        //                    int p7 = Convert.ToInt32(promedio);

        //                    if (p7 >= 90)
        //                        txtPromedioCuarto1.FillColor = Color.ForestGreen;
        //                    txtPromedioCuarto.FillColor = Color.ForestGreen;
        //                    if (p7 < 90 && p7 >= 70)
        //                        txtPromedioCuarto1.FillColor = Color.Yellow;
        //                    txtPromedioCuarto.FillColor = Color.Yellow;
        //                    if (p7 < 70)
        //                        txtPromedioCuarto1.FillColor = Color.Tomato;
        //                    txtPromedioCuarto.FillColor = Color.Tomato;
        //                    break;

        //                case "QUINTO":
        //                    txtPromedioQuinto.Text = promedio + "%";
        //                    txtExcelenciaQuinto.Text = excelencia + " EST.";
        //                    lbSeccionQuinto.Text = "A";
        //                    int p8 = Convert.ToInt32(promedio);

        //                    if (p8 >= 90)
        //                        txtPromedioQuinto1.FillColor = Color.ForestGreen;
        //                    txtPromedioQuinto.FillColor = Color.ForestGreen;
        //                    if (p8 < 90 && p8 >= 70)
        //                        txtPromedioQuinto1.FillColor = Color.Yellow;
        //                    txtPromedioQuinto.FillColor = Color.Yellow;
        //                    if (p8 < 70)
        //                        txtPromedioQuinto1.FillColor = Color.Tomato;
        //                    txtPromedioQuinto.FillColor = Color.Tomato;
        //                    break;

        //                case "SEXTO":
        //                    txtPromedioSexto.Text = promedio + "%";
        //                    txtExcelenciaSexto.Text = excelencia + " EST.";
        //                    lbSeccionSexto.Text = "A";
        //                    int p9 = Convert.ToInt32(promedio);

        //                    if (p9 >= 90)
        //                        txtPromedioSexto1.FillColor = Color.ForestGreen;
        //                    txtPromedioSexto.FillColor = Color.ForestGreen;
        //                    if (p9 < 90 && p9 >= 70)
        //                        txtPromedioSexto1.FillColor = Color.Yellow;
        //                    txtPromedioSexto.FillColor = Color.Yellow;
        //                    if (p9 < 70)
        //                        txtPromedioSexto1.FillColor = Color.Tomato;
        //                    txtPromedioSexto.FillColor = Color.Tomato;
        //                    break;

        //                case "SÉPTIMO":
        //                case "SEPTIMO":
        //                    txtPromedioSeptimo.Text = promedio + "%";
        //                    txtExcelenciaSeptimo.Text = excelencia + " EST.";
        //                    lbSeccionSeptimo.Text = "A";
        //                    int p10 = Convert.ToInt32(promedio);

        //                    if (p10 >= 90)
        //                        txtPromedioSeptimo1.FillColor = Color.ForestGreen;
        //                    txtPromedioSeptimo.FillColor = Color.ForestGreen;
        //                    if (p10 < 90 && p10 >= 70)
        //                        txtPromedioSeptimo1.FillColor = Color.Yellow;
        //                    txtPromedioSeptimo.FillColor = Color.Yellow;
        //                    if (p10 < 70)
        //                        txtPromedioSeptimo1.FillColor = Color.Tomato;
        //                    txtPromedioSeptimo.FillColor = Color.Tomato;
        //                    break;

        //                case "OCTAVO":
        //                    txtPromedioOctavo.Text = promedio + "%";
        //                    txtExcelenciaOctavo.Text = excelencia + " EST.";
        //                    lbSeccionOctavo.Text = "A";
        //                    int p11 = Convert.ToInt32(promedio);

        //                    if (p11 >= 90)
        //                        txtPromedioOctavo1.FillColor = Color.ForestGreen;
        //                    txtPromedioOctavo.FillColor = Color.ForestGreen;
        //                    if (p11 < 90 && p11 >= 70)
        //                        txtPromedioOctavo1.FillColor = Color.Yellow;
        //                    txtPromedioOctavo.FillColor = Color.Yellow;
        //                    if (p11 < 70)
        //                        txtPromedioOctavo1.FillColor = Color.Tomato;
        //                    txtPromedioOctavo.FillColor = Color.Tomato;
        //                    break;

        //                case "NOVENO":
        //                    txtPromedioNoveno.Text = promedio + "%";
        //                    txtExcelenciaNoveno.Text = excelencia + " EST.";
        //                    lbSeccionNoveno.Text = "A";
        //                    int p12 = Convert.ToInt32(promedio);

        //                    if (p12 >= 90)
        //                        txtPromedioNoveno1.FillColor = Color.ForestGreen;
        //                    txtPromedioNoveno.FillColor = Color.ForestGreen;
        //                    if (p12 < 90 && p12 >= 70)
        //                        txtPromedioNoveno1.FillColor = Color.Yellow;
        //                    txtPromedioNoveno.FillColor = Color.Yellow;
        //                    if (p12 < 70)
        //                        txtPromedioNoveno1.FillColor = Color.Tomato;
        //                    txtPromedioNoveno.FillColor = Color.Tomato;
        //                    break;

        //                case "DECIMO":
        //                    txtPromedioDecimo.Text = promedio + "%";
        //                    txtExcelenciaDecimo.Text = excelencia + " EST.";
        //                    lbSeccionDecimo.Text = "A";
        //                    int p13 = Convert.ToInt32(promedio);

        //                    if (p13 >= 90)
        //                        txtPromedioDecimo1.FillColor = Color.ForestGreen;
        //                    txtPromedioDecimo.FillColor = Color.ForestGreen;
        //                    if (p13 < 90 && p13 >= 70)
        //                        txtPromedioDecimo1.FillColor = Color.Yellow;
        //                    txtPromedioDecimo.FillColor = Color.Yellow;
        //                    if (p13 < 70)
        //                        txtPromedioDecimo1.FillColor = Color.Tomato;
        //                    txtPromedioDecimo.FillColor = Color.Tomato;
        //                    break;

        //                case "UNDECIMO":
        //                    txtPromedioUndecimo.Text = promedio;
        //                    txtExcelenciaUndecimo.Text = excelencia + " EST.";
        //                    lbSeccionUndecimo.Text = "A";
        //                    int p14 = Convert.ToInt32(promedio);

        //                    if (p14 >= 90)
        //                        txtPromedioUndecimo1.FillColor = Color.ForestGreen;
        //                    txtPromedioUndecimo.FillColor = Color.ForestGreen;
        //                    if (p14 < 90 && p14 >= 70)
        //                        txtPromedioUndecimo1.FillColor = Color.Yellow;
        //                    txtPromedioUndecimo.FillColor = Color.Yellow;
        //                    if (p14 < 70)
        //                        txtPromedioUndecimo1.FillColor = Color.Tomato;
        //                    txtPromedioUndecimo.FillColor = Color.Tomato;
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error al cargar los promedios: " + ex.Message);
        //    }
        //}

        private void CargarPromediosPorGrado2()
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                SqlParameter[] p =
                {
                    new SqlParameter("@Anio", dtpAnio.Text),
                    new SqlParameter("@Parcial", cbbParcial.SelectedItem)
                };

                DataTable dt = util.EjecutarSPParametros("spMAE_PromedioYExcelenciaPorParcial", p);

                if (dt.Rows.Count == 0)
                {
                    OcultarContenedores();
                    return;
                }

                MostrarContenedores();

                foreach (DataRow row in dt.Rows)
                {
                    string grado = row["NombreGrado"].ToString();
                    string promedio = row["PromedioGrado"].ToString();
                    string excelencia = row["EstudiantesExcelencia"].ToString();

                    int valor = Convert.ToInt32(promedio);

                    Action<Guna.UI2.WinForms.Guna2TextBox, Guna.UI2.WinForms.Guna2TextBox> Pintar =
                        (box1, box2) =>
                        {
                            if (valor >= 90)
                            {
                                box1.FillColor = Color.ForestGreen;
                                box2.FillColor = Color.ForestGreen;
                            }
                            else if (valor >= 70)
                            {
                                box1.FillColor = Color.FromArgb(255, 230, 128);
                                box2.FillColor = Color.FromArgb(255, 230, 128);
                            }
                            else
                            {
                                box1.FillColor = Color.Tomato;
                                box2.FillColor = Color.Tomato;
                            }
                        };

                    switch (grado.ToUpper())
                    {
                        case "PRE-KINDER":
                            txtPromedioPrekinder.Text = promedio + "%";
                            txtExcelenciaPrekinder.Text = excelencia + " EST.";
                            lbSeccionPrekinder.Text = "A";
                            Pintar(txtPromedioPrekinder1, txtPromedioPrekinder);
                            break;

                        case "KINDER":
                            txtPromedioKinder.Text = promedio + "%";
                            txtExcelenciaKinder.Text = excelencia + " EST.";
                            lbSeccionKinder.Text = "A";
                            Pintar(txtPromedioKinder1, txtPromedioKinder);
                            break;

                        case "PREPARATORIA":
                            txtPromedioPrepa.Text = promedio + "%";
                            txtExcelenciaPrepa.Text = excelencia + " EST.";
                            lbSeccionPrepa.Text = "A";
                            Pintar(txtPromedioPrepa1, txtPromedioPrepa);
                            break;

                        case "PRIMERO":
                            txtPromedioPrimero.Text = promedio + "%";
                            txtExcelenciaPrimero.Text = excelencia + " EST.";
                            lbSeccionPrimero.Text = "A";
                            Pintar(txtPromedioPrimero1, txtPromedioPrimero);
                            break;

                        case "SEGUNDO":
                            txtPromedioSegundo.Text = promedio + "%";
                            txtExcelenciaSegundo.Text = excelencia + " EST.";
                            lbSeccionSegundo.Text = "A";
                            Pintar(txtPromedioSegundo1, txtPromedioSegundo);
                            break;

                        case "TERCERO":
                            txtPromedioTercero.Text = promedio + "%";
                            txtExcelenciaTercero.Text = excelencia + " EST.";
                            lbSeccionTercero.Text = "A";
                            Pintar(txtPromedioTercero1, txtPromedioTercero);
                            break;

                        case "CUARTO":
                            txtPromedioCuarto.Text = promedio + "%";
                            txtExcelenciaCuarto.Text = excelencia + " EST.";
                            lbSeccionCuarto.Text = "A";
                            Pintar(txtPromedioCuarto1, txtPromedioCuarto);
                            break;

                        case "QUINTO":
                            txtPromedioQuinto.Text = promedio + "%";
                            txtExcelenciaQuinto.Text = excelencia + " EST.";
                            lbSeccionQuinto.Text = "A";
                            Pintar(txtPromedioQuinto1, txtPromedioQuinto);
                            break;

                        case "SEXTO":
                            txtPromedioSexto.Text = promedio + "%";
                            txtExcelenciaSexto.Text = excelencia + " EST.";
                            lbSeccionSexto.Text = "A";
                            Pintar(txtPromedioSexto1, txtPromedioSexto);
                            break;

                        case "SÉPTIMO":
                        case "SEPTIMO":
                            txtPromedioSeptimo.Text = promedio + "%";
                            txtExcelenciaSeptimo.Text = excelencia + " EST.";
                            lbSeccionSeptimo.Text = "A";
                            Pintar(txtPromedioSeptimo1, txtPromedioSeptimo);
                            break;

                        case "OCTAVO":
                            txtPromedioOctavo.Text = promedio + "%";
                            txtExcelenciaOctavo.Text = excelencia + " EST.";
                            lbSeccionOctavo.Text = "A";
                            Pintar(txtPromedioOctavo1, txtPromedioOctavo);
                            break;

                        case "NOVENO":
                            txtPromedioNoveno.Text = promedio + "%";
                            txtExcelenciaNoveno.Text = excelencia + " EST.";
                            lbSeccionNoveno.Text = "A";
                            Pintar(txtPromedioNoveno1, txtPromedioNoveno);
                            break;

                        case "DECIMO":
                            txtPromedioDecimo.Text = promedio + "%";
                            txtExcelenciaDecimo.Text = excelencia + " EST.";
                            lbSeccionDecimo.Text = "A";
                            Pintar(txtPromedioDecimo1, txtPromedioDecimo);
                            break;

                        case "UNDECIMO":
                            txtPromedioUndecimo.Text = promedio + "%";
                            txtExcelenciaUndecimo.Text = excelencia + " EST.";
                            lbSeccionUndecimo.Text = "A";
                            Pintar(txtPromedioUndecimo1, txtPromedioUndecimo);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los promedios: " + ex.Message);
            }
        }

        private void MostrarContenedores()
        {
            CtnPrekinder.Visible = true;
            ctnKinder.Visible = true;
            ctnPreparatoria.Visible = true;
            ctnPrimero.Visible = true;
            ctnSegundo.Visible = true;
            ctnTercero.Visible = true;
            ctnCuarto.Visible = true;
            ctnQuinto.Visible = true;
            ctnSexto.Visible = true;
            ctnSeptimo.Visible = true;
            ctnOctavo.Visible = true;
            ctnNoveno.Visible = true;
            ctnDecimoGrado.Visible = true;
            ctnUndecimo.Visible = true;
        }
        private void OcultarContenedores()
        {
            CtnPrekinder.Visible = false;
            ctnKinder.Visible = false;
            ctnPreparatoria.Visible = false;
            ctnPrimero.Visible = false;
            ctnSegundo.Visible = false;
            ctnTercero.Visible = false;
            ctnCuarto.Visible = false;
            ctnQuinto.Visible = false;
            ctnSexto.Visible = false;
            ctnSeptimo.Visible = false;
            ctnOctavo.Visible = false;
            ctnNoveno.Visible = false;
            ctnDecimoGrado.Visible = false;
            ctnUndecimo.Visible = false;
        }

        private void cbbParcial_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            CargarPromediosPorGrado2();
            MostrarContenedoresPorNivel();
            CargarGraficoPorGrado();

        }
    }
}
