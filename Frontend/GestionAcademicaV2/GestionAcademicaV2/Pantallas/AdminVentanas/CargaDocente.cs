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
    public partial class CargaDocente : Form
    {
        private PantallaAdmin pantallaPrincipal;
        EjecutarUtilidades util = new EjecutarUtilidades();
        public CargaDocente(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }

        private void CargarDocentes()
        {
            try
            {
                string consulta = "SELECT * FROM vMAE_CargarDocentes";
                DataTable dt = util.EjecutarConsulta(consulta);
                dgvDocentes.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar docentes: " + ex.Message);
            }
        }

        private void BuscarDocentes()
        {
            try
            {
                SqlParameter[] parametros =
                {
                    new SqlParameter("@Grado",
                    string.IsNullOrWhiteSpace(cbbGrado.Text) ? (object)DBNull.Value : cbbGrado.Text),
                    new SqlParameter("@Seccion",
                    string.IsNullOrWhiteSpace(cbbSeccion.Text) ? (object)DBNull.Value : cbbSeccion.Text),
                    new SqlParameter("@Anio",
                    string.IsNullOrWhiteSpace(dtpAnio.Text) ? (object)DBNull.Value : Convert.ToInt32(dtpAnio.Text)),
                    new SqlParameter("@Nombre",
                    string.IsNullOrWhiteSpace(txtBuscarDocente.Text) ? (object)DBNull.Value : txtBuscarDocente.Text)
                };
                DataTable dt = util.EjecutarSPParametros("spMAE_BuscarDocentesPorGradoSeccionAnio", parametros);
                dgvDocentes.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar docentes: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarAsignaturasDocente(int docenteID)
        {
            try
            {
                string grado = cbbGrado.Text == "TODOS" ? null : cbbGrado.Text;
                string seccion = cbbSeccion.Text == "TODAS" ? null : cbbSeccion.Text;

                SqlParameter[] p =
                {
                    new SqlParameter("@DocenteID", docenteID),
                    new SqlParameter("@Anio", Convert.ToInt32(dtpAnio.Text)),
                    new SqlParameter("@Grado", (object)grado ?? DBNull.Value),
                    new SqlParameter("@Seccion", (object)seccion ?? DBNull.Value)
                };

                DataTable dt = util.EjecutarSPParametros("spMAE_TraeAsignaturasPorDocenteSeccion", p);

                dgvAsignatura.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar asignaturas: " + ex.Message);
            }
        }

        private void CargarGrados2()
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                DataTable tabla = util.EjecutarConsulta("SELECT * FROM vMAE_TraeGrados ORDER BY GradoID");

                DataRow filaTodos = tabla.NewRow();
                filaTodos["GradoID"] = 0;              
                filaTodos["NombreGrado"] = "TODOS";    

                tabla.Rows.InsertAt(filaTodos, 0);

                cbbGrado.DataSource = tabla;
                cbbGrado.DisplayMember = "NombreGrado";
                cbbGrado.ValueMember = "GradoID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al llenar grados: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BuscarDocentes2()
        {
            try
            {
                bool todosLosGrados = cbbGrado.SelectedValue != null &&
                                      cbbGrado.SelectedValue.ToString() == "0";

                bool todasLasSecciones = cbbSeccion.SelectedValue != null &&
                                         cbbSeccion.SelectedValue.ToString() == "0";

                SqlParameter[] parametros =
                {
                    new SqlParameter("@Grado",
                    todosLosGrados ? (object)DBNull.Value : cbbGrado.Text),

                    new SqlParameter("@Seccion",
                    todasLasSecciones ? (object)DBNull.Value : cbbSeccion.Text),

                    new SqlParameter("@Anio",
                    string.IsNullOrWhiteSpace(dtpAnio.Text) ? (object)DBNull.Value : Convert.ToInt32(dtpAnio.Text)),

                    new SqlParameter("@Nombre",
                    string.IsNullOrWhiteSpace(txtBuscarDocente.Text) ? (object)DBNull.Value : txtBuscarDocente.Text)
            };

                DataTable dt = util.EjecutarSPParametros("spMAE_BuscarDocentesPorGradoSeccionAnio", parametros);
                dgvDocentes.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar docentes: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CargarGrados()
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                DataTable tabla = util.EjecutarConsulta("SELECT * FROM vMAE_TraeGrados order by GradoID");
                cbbGrado.DataSource = tabla;
                cbbGrado.DisplayMember = "NombreGrado";
                cbbGrado.ValueMember = "GradoID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al llenar grados: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarSecciones()
        {
            try
            {
                DataTable dt = util.EjecutarSP("spMAE_ObtenerSecciones");

                DataRow filaTodas = dt.NewRow();
                filaTodas["SeccionID"] = 0;
                filaTodas["Letra"] = "TODAS";  

                dt.Rows.InsertAt(filaTodas, 0);

                cbbSeccion.DataSource = dt;
                cbbSeccion.DisplayMember = "Letra";
                cbbSeccion.ValueMember = "SeccionID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar secciones: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CargarGraficoCargaAcademica()
        {
            try
            {
                chartDocentes.Series.Clear();
                chartDocentes.ChartAreas.Clear();
                chartDocentes.Titles.Clear();

                chartDocentes.Titles.Add(
                    $"CARGA ACADÉMICA - {cbbGrado.Text} {cbbSeccion.Text} ({dtpAnio.Text})"
                );

                ChartArea area = new ChartArea("MainArea");
                chartDocentes.ChartAreas.Add(area);

                area.AxisX.ScrollBar.Size = 12;
                area.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
                area.AxisX.ScrollBar.IsPositionedInside = false;
                area.AxisX.ScaleView.Zoomable = true;

                area.AxisX.LabelStyle.Font = new Font("Arial", 6f, FontStyle.Bold);
                area.AxisX.LabelStyle.Angle = -30;
                area.AxisX.Interval = 1;
                area.AxisX.MajorGrid.Enabled = false;

                area.AxisY.Title = "CLASES";
                area.AxisY.TitleFont = new Font("Arial", 8f, FontStyle.Bold);
                area.AxisY.LabelStyle.Font = new Font("Arial", 7f, FontStyle.Bold);
                area.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 235, 235);

                Series sBarras = new Series("CLASES")
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = true,
                    Font = new Font("Arial", 8f, FontStyle.Bold),
                    Color = Color.CornflowerBlue
                };
                sBarras["PointWidth"] = "0.7";
                chartDocentes.Series.Add(sBarras);

                string grado = cbbGrado.Text == "TODOS" ? null : cbbGrado.Text;
                string seccion = cbbSeccion.Text == "TODAS" ? null : cbbSeccion.Text;

                SqlParameter[] p =
                {
            new SqlParameter("@Anio", dtpAnio.Text),
            new SqlParameter("@Grado", (object)grado ?? DBNull.Value),
            new SqlParameter("@Seccion", (object)seccion ?? DBNull.Value)
        };

                DataTable dt = util.EjecutarSPParametros("spMAE_CargaAcademicaDocenteSeccion", p);

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string docente = dt.Rows[i]["Nombre"].ToString().Trim();
                        int totalClases = Convert.ToInt32(dt.Rows[i]["TotalClases"]);

                        int idx = sBarras.Points.AddXY(i, totalClases);
                        sBarras.Points[idx].AxisLabel = docente;
                    }

                    if (dt.Rows.Count > 10)
                    {
                        area.AxisX.ScaleView.Zoom(0, 10);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar el gráfico: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargaDocente_Load(object sender, EventArgs e)
        {
            CargarGrados2();
            CargarSecciones();
            CargarDocentes();
            CargarGraficoCargaAcademica();
        }

        private void cbbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuscarDocentes2();
            CargarGraficoCargaAcademica();
            DataTable dt = (DataTable)dgvAsignatura.DataSource;
            if (dt != null)
            {
                dt.Clear();
            }
        }

        private void cbbSeccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuscarDocentes2();
            CargarGraficoCargaAcademica();
            DataTable dt = (DataTable)dgvAsignatura.DataSource;
            if (dt != null)
            {
                dt.Clear();
            }
        }

        private void dtpAnio_ValueChanged(object sender, EventArgs e)
        {
            BuscarDocentes2();
            CargarGraficoCargaAcademica();
            DataTable dt = (DataTable)dgvAsignatura.DataSource;
            if (dt != null)
            {
                dt.Clear();
            }
        }

        private void btBuscarDocente_Click(object sender, EventArgs e)
        {
            BuscarDocentes2();
        }

        private void btBuscarAnio_Click(object sender, EventArgs e)
        {

        }

        private void dgvDocentes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDocentes.CurrentRow != null)
            {
                int docenteID = Convert.ToInt32(dgvDocentes.CurrentRow.Cells["DocenteID"].Value);

                CargarAsignaturasDocente(docenteID);
            }
        }
    }
}
