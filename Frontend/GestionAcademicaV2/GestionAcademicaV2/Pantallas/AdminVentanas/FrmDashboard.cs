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
        public FrmDashboard(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }

        private void CargarPromediosPorGrado()
        {
            EjecutarUtilidades util = new EjecutarUtilidades();

            SqlParameter[] p =
            {
        new SqlParameter("@Anio", Convert.ToInt32(dtpAnio.Value))
    };

            DataTable dt = util.EjecutarSP("spPromedioYExcelenciaPorNivel", p);

            foreach (DataRow row in dt.Rows)
            {
                string grado = row["NombreGrado"].ToString();
                string promedio = row["PromedioGrado"].ToString();
                string excelencia = row["EstudiantesExcelencia"].ToString();

                switch (grado.ToUpper())
                {
                    case "PRE-KINDER":
                        txtPromedioPrekinder.Text = promedio;
                        txtExcelenciaPrekinder.Text = excelencia;
                        lbSeccionPrekinder.Text = "A";
                        int p1 = Convert.ToInt32(promedio);

                        if (p1 >= 90)
                            txtPromedioPrekinder1.BackColor = Color.ForestGreen;
                            txtPromedioPrekinder.BackColor = Color.ForestGreen;
                        if (p1 < 90 && p1 >= 70)
                            txtPromedioPrekinder1.BackColor = Color.Yellow;
                            txtPromedioPrekinder.BackColor = Color.Yellow;
                        if (p1 < 70)
                            txtPromedioPrekinder1.BackColor = Color.Tomato;
                            txtPromedioPrekinder.BackColor = Color.Tomato;
                        break;

                    case "KINDER":
                        txtPromedioKinder.Text = promedio;
                        txtExcelenciaKinder.Text = excelencia;
                        lbSeccionKinder.Text = "A";
                        int p2 = Convert.ToInt32(promedio);

                        if (p2 >= 90)
                            txtPromedioKinder1.BackColor = Color.ForestGreen;
                        txtPromedioKinder.BackColor = Color.ForestGreen;
                        if (p2 < 90 && p2 >= 70)
                            txtPromedioKinder1.BackColor = Color.Yellow;
                        txtPromedioKinder.BackColor = Color.Yellow;
                        if (p2 < 70)
                            txtPromedioKinder1.BackColor = Color.Tomato;
                        txtPromedioKinder.BackColor = Color.Tomato;
                        break;

                    case "PREPARATORIA":
                        txtPromedioPrepa.Text = promedio;
                        txtExcelenciaPrepa.Text = excelencia;
                        lbSeccionPrepa.Text = "A";
                        int p3 = Convert.ToInt32(promedio);

                        if (p3 >= 90)
                            txtPromedioPrepa1.BackColor = Color.ForestGreen;
                        txtPromedioPrepa.BackColor = Color.ForestGreen;
                        if (p3 < 90 && p3 >= 70)
                            txtPromedioPrepa1.BackColor = Color.Yellow;
                            txtPromedioPrepa.BackColor = Color.Yellow;
                        if (p3 < 70)
                            txtPromedioPrepa1.BackColor = Color.Tomato;
                            txtPromedioPrepa.BackColor = Color.Tomato;
                        break;

                    case "PRIMERO":
                        txtPromedioPrimero.Text = promedio;
                        txtExcelenciaPrimero.Text = excelencia;
                        lbSeccionPrimero.Text = "A";
                        int p4 = Convert.ToInt32(promedio);

                        if (p4 >= 90)
                            txtPromedioPrimero1.BackColor = Color.ForestGreen;
                            txtPromedioPrimero.BackColor = Color.ForestGreen;
                        if (p4 < 90 && p4 >= 70)
                            txtPromedioPrimero1.BackColor = Color.Yellow;
                            txtPromedioPrimero.BackColor = Color.Yellow;
                        if (p4 < 70)
                            txtPromedioPrimero1.BackColor = Color.Tomato;
                            txtPromedioPrimero.BackColor = Color.Tomato;
                        break;

                    case "SEGUNDO":
                        txtPromedioSegundo.Text = promedio;
                        txtExcelenciaSegundo.Text = excelencia;
                        lbSeccionSegundo.Text = "A";
                        int p5 = Convert.ToInt32(promedio);

                        if (p5 >= 90)
                            txtPromedioSegundo1.BackColor = Color.ForestGreen;
                            txtPromedioSegundo.BackColor = Color.ForestGreen;
                        if (p5 < 90 && p5 >= 70)
                            txtPromedioSegundo1.BackColor = Color.Yellow;
                            txtPromedioSegundo.BackColor = Color.Yellow;
                        if (p5 < 70)
                            txtPromedioSegundo1.BackColor = Color.Tomato;
                            txtPromedioSegundo.BackColor = Color.Tomato;
                        break;

                    case "TERCERO":
                        txtPromedioTercero.Text = promedio;
                        txtExcelenciaTercero.Text = excelencia;
                        lbSeccionTercero.Text = "A";
                        int p6 = Convert.ToInt32(promedio);

                        if (p6 >= 90)
                            txtPromedioTercero1.BackColor = Color.ForestGreen;
                            txtPromedioTercero.BackColor = Color.ForestGreen;
                        if (p6 < 90 && p6 >= 70)
                            txtPromedioTercero1.BackColor = Color.Yellow;
                            txtPromedioTercero.BackColor = Color.Yellow;
                        if (p6 < 70)
                            txtPromedioSegundo1.BackColor = Color.Tomato;
                            txtPromedioSegundo.BackColor = Color.Tomato;
                        break;

                    case "CUARTO":
                        txtPromedioCuarto.Text = promedio;
                        txtExcelenciaCuarto.Text = excelencia;
                        lbSeccionCuarto.Text = "A";
                        break;

                    case "QUINTO":
                        txtPromedioQuinto.Text = promedio;
                        txtExcelenciaQuinto.Text = excelencia;
                        lbSeccionQuinto.Text = "A";
                        break;

                    case "SEXTO":
                        txtPromedioSexto.Text = promedio;
                        txtExcelenciaSexto.Text = excelencia;
                        lbSeccionSexto.Text = "A";
                        break;

                    case "SÉPTIMO":
                    case "SEPTIMO":
                        txtPromedioSeptimo.Text = promedio;
                        txtExcelenciaSeptimo.Text = excelencia;
                        lbSeccionSeptimo.Text = "A";
                        break;

                    case "OCTAVO":
                        txtPromedioOctavo.Text = promedio;
                        txtExcelenciaOctavo.Text = excelencia;
                        lbSeccionOctavo.Text = "A";
                        break;

                    case "NOVENO":
                        txtPromedioNoveno.Text = promedio;
                        txtExcelenciaNoveno.Text = excelencia;
                        lbSeccionNoveno.Text = "A";
                        break;

                    case "DECIMO":
                        txtPromedioDecimo.Text = promedio;
                        lbSeccionDecimo.Text = "A";
                        break;

                    case "UNDECIMO":
                        txtPromedioUndecimo.Text = promedio;
                        lbSeccionUndecimo.Text = "A";
                        break;
                }
            }
        }
        private void FrmDashboard_Load(object sender, EventArgs e)
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
            CtnPrekinder.Visible = false;
            ctnKinder.Visible = false;
            ctnPreparatoria.Visible = false;

            // Gráfico Desempeño Escolar
            chartDesempenoEscolar.Series.Clear();
            chartDesempenoEscolar.ChartAreas.Clear();

            ChartArea area = new ChartArea("Area1");
            chartDesempenoEscolar.ChartAreas.Add(area);

            // Serie META
            Series serieMeta = new Series("DESEMPEÑO META");
            serieMeta.ChartType = SeriesChartType.Line;
            serieMeta.Color = Color.Green;
            serieMeta.BorderWidth = 3;
            serieMeta.MarkerStyle = MarkerStyle.Circle;
            serieMeta.MarkerSize = 8;

            // Serie PROMEDIO
            Series seriePromedio = new Series("PROMEDIO ALCANZADO");
            seriePromedio.ChartType = SeriesChartType.Line;
            seriePromedio.Color = Color.RoyalBlue;
            seriePromedio.BorderWidth = 3;
            seriePromedio.MarkerStyle = MarkerStyle.Circle;
            seriePromedio.MarkerSize = 8;

            string[] meses = { "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE" };
            double[] meta = { 85, 85, 85, 85, 85, 85, 85, 85, 85, 85 };
            double[] promedio = { 90, 80, 78, 88, 92, 67, 78, 90, 89, 69 };

            for (int i = 0; i < meses.Length; i++)
            {
                serieMeta.Points.AddXY(meses[i], meta[i]);
                seriePromedio.Points.AddXY(meses[i], promedio[i]);
            }

            chartDesempenoEscolar.Series.Add(serieMeta);
            chartDesempenoEscolar.Series.Add(seriePromedio);

            var area1 = chartDesempenoEscolar.ChartAreas["Area1"];
            area1.AxisY.Minimum = 65;
            area1.AxisY.Maximum = 100;
            area1.AxisY.Interval = 5;
            area1.AxisY.LabelStyle.Format = "0'%'";
            area1.AxisX.Interval = 1;

            chartDesempenoEscolar.Titles.Clear();
            chartDesempenoEscolar.Legends[0].Docking = Docking.Bottom;
        }

        private void cbbNivel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Flexibilidad del contenedor primario
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
                ctnUndecimo.Visible = true;
                CtnPrekinder.Visible = false;
                ctnKinder.Visible = false;
                ctnPreparatoria.Visible = false;
                ctnDecimoGrado.Visible = true;
            }
        }

        private void cbbAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void guna2DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            lbAnio.Text = dtpAnio.Text;
        }
    }
}
