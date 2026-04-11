using GestionAcademicaV2.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmReporteDesercionRetencion : Form
    {
        private PantallaAdmin pantallaPrincipal;
        EjecutarUtilidades util = new EjecutarUtilidades();
        public FrmReporteDesercionRetencion(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }
        private void CargarGridsDetalle()
        {
            try
            {
                DataTable dtTop = util.EjecutarConsulta("SELECT EstudianteID FROM vMAE_RepProyDesercionGen");
                int total = Math.Min(dtTop.Rows.Count, 10);
                for (int i = 0; i < total; i++)
                {
                    int estudianteID = Convert.ToInt32(dtTop.Rows[i]["EstudianteID"]);
                    DataTable dtDet = util.EjecutarConsulta(
                        "SELECT Asignatura, Inasistencias FROM vMAE_RepProyDesercionDet WHERE EstudianteID = " + estudianteID);
                    var grid = this.Controls.Find("dgvTop" + (i + 1), true)
                        .FirstOrDefault() as Guna.UI2.WinForms.Guna2DataGridView;
                    if (grid != null)
                    {
                        grid.DataSource = dtDet;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los detalles: " + ex.Message);
            }
        }

        private void CargarContenedoresDesercion()
        {
            try
            {
                DataTable dt = util.EjecutarConsulta("SELECT * FROM vMAE_RepProyDesercionGen");
                int total = Math.Min(dt.Rows.Count, 10);
                for (int i = 0; i < total; i++)
                {
                    DataRow row = dt.Rows[i];
                    var lblNombre = this.Controls.Find("lbNombreTop" + (i + 1), true)
                        .FirstOrDefault() as Guna.UI2.WinForms.Guna2HtmlLabel;

                    var lblPromedio = this.Controls.Find("lbPromedioTop" + (i + 1), true)
                        .FirstOrDefault() as Guna.UI2.WinForms.Guna2HtmlLabel;

                    var lblGrado = this.Controls.Find("lbGradoTop" + (i + 1), true)
                        .FirstOrDefault() as Guna.UI2.WinForms.Guna2HtmlLabel;

                    var lblSeccion = this.Controls.Find("lbSeccionTop" + (i + 1), true)
                        .FirstOrDefault() as Guna.UI2.WinForms.Guna2HtmlLabel;

                    if (lblNombre != null) lblNombre.Text = row["Nombre"].ToString();
                    if (lblPromedio != null) lblPromedio.Text = row["PromedioAnual"].ToString() + "%";
                    if (lblGrado != null) lblGrado.Text = row["Grado"].ToString();
                    if (lblSeccion != null) lblSeccion.Text = row["Seccion"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar contenedores: " + ex.Message);
            }
        }
        private void FrmReporteDesercionRetencion_Load(object sender, EventArgs e)
        {
            CargarContenedoresDesercion();
            CargarGridsDetalle();
            txtAnio.Text = (DateTime.Now.Year - 1).ToString();
            //txtAnio.Text = DateTime.Now.Year.ToString();
        }
    }
}
