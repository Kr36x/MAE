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

            string[] meses = {"FEBRERO","MARZO","ABRIL","MAYO","JUNIO","JULIO","AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE"};
            double[] meta = { 85, 85, 85, 85, 85 };
            double[] promedio = { 90, 80, 78, 88, 92 };

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
            lbAnio.Text = cbbAnio.Text;
        }
    }
}
