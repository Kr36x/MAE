using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
                chartGrados.Legends.Clear();

                Legend leyenda = new Legend();
                leyenda.Font = new Font("Arial", 7f, FontStyle.Bold);

                leyenda.Docking = Docking.Bottom;
                leyenda.Alignment = StringAlignment.Center;

                chartGrados.Legends.Add(leyenda);

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

                area.InnerPlotPosition = new ElementPosition(8, 5, 100, 65);

                Series sBarras = new Series("PROMEDIO")
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = true,
                    Font = new Font("Arial", 7f, FontStyle.Bold)
                };
                sBarras["PointWidth"] = "0.7";

                Series sMeta = new Series("META")
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.ForestGreen,
                    BorderWidth = 3,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 10
                };

                chartGrados.Series.Add(sBarras);
                chartGrados.Series.Add(sMeta);

                DataTable dt = util.EjecutarSPParametros(
                    "spMAE_RepGlobalRendNivel",
                    new SqlParameter[]
                    {
                        new SqlParameter("@Anio", dtpAnio.Value.Year),
                        new SqlParameter("@Parcial", Convert.ToInt32(cbbParcial.SelectedItem)),
                        new SqlParameter("@Nivel", cbbNivel.Text)
                    }
                );
                if (dt.Rows.Count == 0)
                {
                    lbDatosGrafico.Visible = true;
                    lbDatos.Visible = true;
                }
                else
                {
                    lbDatosGrafico.Visible = false;
                    lbDatos.Visible= false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string grado = dt.Rows[i]["NombreGrado"].ToString().Trim();
                        int promedio = Convert.ToInt32(dt.Rows[i]["PromedioGrado"]);

                        int idx = sBarras.Points.AddXY(i, promedio);
                        sMeta.Points.AddXY(i, 85);

                        sBarras.Points[idx].AxisLabel = grado;
                        sBarras.Points[idx].Label = promedio + "%";
                        //sBarras.Points[idx].Color = Color.CornflowerBlue;
                        if (promedio < 70)
                            sBarras.Points[idx].Color = Color.Tomato;
                        else if (promedio < 90)
                            sBarras.Points[idx].Color = Color.FromArgb(255, 230, 128);   // Amarillo
                        else
                            sBarras.Points[idx].Color = Color.CornflowerBlue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el gráfico: " + ex.Message);
            }
        }
        private void FrmDashboard_Load(object sender, EventArgs e)
        {
            lbAnio.Text = dtpAnio.Text;
            pnlPrincipal.Visible = false;

            CargarParciales();
            CargarPromediosPorGrado2();
            MostrarContenedoresPorNivel();
            CargarGraficoPorGrado();
            txtPromedioPrekinder.BorderColor = Color.FromArgb(217, 221, 226);
            txtExcelenciaPrekinder.BorderColor = Color.FromArgb(217, 221, 226);
            txtPromedioKinder.BorderColor = Color.FromArgb(217, 221, 226);
            txtExcelenciaKinder.BorderColor = Color.FromArgb(217, 221, 226);
            txtPromedioPrepa.BorderColor = Color.FromArgb(217, 221, 226);
            txtExcelenciaPrepa.BorderColor = Color.FromArgb(217, 221, 226);
            txtPromedioPrimero.BorderColor = Color.FromArgb(217, 221, 226);
            txtExcelenciaPrimero.BorderColor = Color.FromArgb(217, 221, 226);
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
            CargarGraficoPorGrado();

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
                new SqlParameter("@Anio", dtpAnio.Value.Year),
                new SqlParameter("@Parcial", Convert.ToInt32(cbbParcial.SelectedItem))
            };

            DataTable dt = util.EjecutarSPParametros("spMAE_RepGlobalRend", p);

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

        private void CargarPromediosPorGrado2()
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                SqlParameter[] p =
                {
                    new SqlParameter("@Anio",dtpAnio.Value.Year),
                    new SqlParameter("@Parcial", Convert.ToInt32(cbbParcial.SelectedItem))
                };

                DataTable dt = util.EjecutarSPParametros("spMAE_RepGlobalRend", p);

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
                    string excelencia = row["Excelencia"].ToString();

                    int valor = Convert.ToInt32(promedio);

                    Action<Guna.UI2.WinForms.Guna2TextBox, Guna.UI2.WinForms.Guna2TextBox> Pintar =
                        (box1, box2) =>
                        {
                            if (valor >= 90)
                            {
                                box1.FillColor = Color.CornflowerBlue;
                                box2.FillColor = Color.CornflowerBlue;
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

                            box1.BorderColor = Color.FromArgb(217, 221, 226);
                            box2.BorderColor = Color.FromArgb(217, 221, 226);
                        };

                    switch (grado.ToUpper())
                    {
                        case "PRE-KINDER":
                            txtPromedioPrekinder.Text = promedio + "%";
                            txtExcelenciaPrekinder.Text = excelencia;
                            lbSeccionPrekinder.Text = "TODAS";
                            Pintar(txtPromedioPrekinder1, txtPromedioPrekinder);
                            break;

                        case "KINDER":
                            txtPromedioKinder.Text = promedio + "%";
                            txtExcelenciaKinder.Text = excelencia;
                            lbSeccionKinder.Text = "TODAS";
                            Pintar(txtPromedioKinder1, txtPromedioKinder);
                            break;

                        case "PREPARATORIA":
                            txtPromedioPrepa.Text = promedio + "%";
                            txtExcelenciaPrepa.Text = excelencia;
                            lbSeccionPrepa.Text = "TODAS";
                            Pintar(txtPromedioPrepa1, txtPromedioPrepa);
                            break;

                        case "PRIMERO":
                            txtPromedioPrimero.Text = promedio + "%";
                            txtExcelenciaPrimero.Text = excelencia;
                            lbSeccionPrimero.Text = "TODAS";
                            Pintar(txtPromedioPrimero1, txtPromedioPrimero);
                            break;

                        case "SEGUNDO":
                            txtPromedioSegundo.Text = promedio + "%";
                            txtExcelenciaSegundo.Text = excelencia;
                            lbSeccionSegundo.Text = "TODAS";
                            Pintar(txtPromedioSegundo1, txtPromedioSegundo);
                            break;

                        case "TERCERO":
                            txtPromedioTercero.Text = promedio + "%";
                            txtExcelenciaTercero.Text = excelencia;
                            lbSeccionTercero.Text = "TODAS";
                            Pintar(txtPromedioTercero1, txtPromedioTercero);
                            break;

                        case "CUARTO":
                            txtPromedioCuarto.Text = promedio + "%";
                            txtExcelenciaCuarto.Text = excelencia;
                            lbSeccionCuarto.Text = "TODAS";
                            Pintar(txtPromedioCuarto1, txtPromedioCuarto);
                            break;

                        case "QUINTO":
                            txtPromedioQuinto.Text = promedio + "%";
                            txtExcelenciaQuinto.Text = excelencia;
                            lbSeccionQuinto.Text = "TODAS";
                            Pintar(txtPromedioQuinto1, txtPromedioQuinto);
                            break;

                        case "SEXTO":
                            txtPromedioSexto.Text = promedio + "%";
                            txtExcelenciaSexto.Text = excelencia;
                            lbSeccionSexto.Text = "TODAS";
                            Pintar(txtPromedioSexto1, txtPromedioSexto);
                            break;

                        case "SÉPTIMO":
                        case "SEPTIMO":
                            txtPromedioSeptimo.Text = promedio + "%";
                            txtExcelenciaSeptimo.Text = excelencia;
                            lbSeccionSeptimo.Text = "TODAS";
                            Pintar(txtPromedioSeptimo1, txtPromedioSeptimo);
                            break;

                        case "OCTAVO":
                            txtPromedioOctavo.Text = promedio + "%";
                            txtExcelenciaOctavo.Text = excelencia;
                            lbSeccionOctavo.Text = "TODAS";
                            Pintar(txtPromedioOctavo1, txtPromedioOctavo);
                            break;

                        case "NOVENO":
                            txtPromedioNoveno.Text = promedio + "%";
                            txtExcelenciaNoveno.Text = excelencia;
                            lbSeccionNoveno.Text = "TODAS";
                            Pintar(txtPromedioNoveno1, txtPromedioNoveno);
                            break;

                        case "DECIMO":
                            txtPromedioDecimo.Text = promedio + "%";
                            txtExcelenciaDecimo.Text = excelencia;
                            lbSeccionDecimo.Text = "TODAS";
                            Pintar(txtPromedioDecimo1, txtPromedioDecimo);
                            break;

                        case "UNDECIMO":
                            txtPromedioUndecimo.Text = promedio + "%";
                            txtExcelenciaUndecimo.Text = excelencia;
                            lbSeccionUndecimo.Text = "TODAS";
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

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
