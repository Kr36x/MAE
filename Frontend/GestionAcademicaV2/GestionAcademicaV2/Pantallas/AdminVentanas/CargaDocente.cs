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
                //string consulta = "SELECT * FROM vMAE_CargarDocentes";
                string consulta = "SELECT * FROM vMAE_CargarDocentess";
                DataTable dt = util.EjecutarConsulta(consulta);
                dgvDocentes.DataSource = dt;
                dgvDocentes.Columns["CargaID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar docentes: " + ex.Message);
            }
        }

        //private object ObtenerValorSeguro(ComboBox combo)
        //{
        //    if (combo.SelectedValue == null || combo.SelectedValue is DataRowView)
        //        return DBNull.Value;

        //    return combo.SelectedValue;
        //}
        //private void LlenarDocentes()
        //{
        //    try
        //    {
        //        EjecutarUtilidades util = new EjecutarUtilidades();

        //        SqlParameter[] parametros =
        //        {
        //            new SqlParameter("@Anio", string.IsNullOrWhiteSpace(dtpAnio.Text) ? DBNull.Value : (object)int.Parse(dtpAnio.Text)),
        //            new SqlParameter("@DocenteID", DBNull.Value),
        //            new SqlParameter("@GradoID", ObtenerValorSeguro(cbbGrado)),
        //            new SqlParameter("@SeccionID", ObtenerValorSeguro(cbbSeccion)),
        //            new SqlParameter("@BusquedaDocente", string.IsNullOrWhiteSpace(txtBuscarDocente.Text) ? DBNull.Value : (object)txtBuscarDocente.Text)
        //        };

        //        DataTable dt = util.EjecutarSPParametros("spMAE_ListarCargaAcademica", parametros);

        //        dgvDocentes.DataSource = dt;

        //        if (dgvDocentes.Columns.Contains("CargaID"))
        //            dgvDocentes.Columns["CargaID"].Visible = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error al ejecutar el gráfico: " + ex.Message,
        //                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}


        private void CargarAsignaturasDocente(int? docenteID, int? seccionID)
        {
            // Metodo para llenar el dvgAsignatura al tocar el boton ver
            try
            {
                if (docenteID == null || seccionID == null)
                {
                    MessageBox.Show("Para cargar asignaturas se necesita sección y docente: ");
                }
                else
                {
                    SqlParameter[] p =
                    {
                        new SqlParameter("@DocenteID", docenteID),
                        new SqlParameter("@SeccionID", seccionID)
                    };
                    //DataTable dt = util.EjecutarSPParametros("spMAE_TraeAsignaturasPorDocenteSeccion", p);
                    DataTable dt = util.EjecutarSPParametros("spMAE_ListarCargaAcademicaxDocentexSecc", p);

                    dgvAsignatura.DataSource = dt;
                    ConfigurarColumnas2();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar asignaturas: " + ex.Message);
            }
        }

        private void CargarGrados2()
        {
            // Este metodo carga los datos en el form
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
            // Metodo para poder buscar y aplicar los filtros en el form
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
                dgvDocentes.Columns["CargaID"].Visible = false;
                dgvDocentes.Columns["Estado"].Visible = false;
                dgvDocentes.Columns["SeccionID"].Visible = false;
                dgvDocentes.Columns["AsignaturaID"].Visible = false;

                ConfigurarColumnas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar docentes: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarSecciones()
        {
            // Metodo para cargar secciones en el form
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
            // Metodo para la elaboración del gráfico
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

                //int? grado = cbbGrado.Text == "TODOS" ? (int?)null : Convert.ToInt32(cbbGrado.SelectedValue);
                //int? seccion = cbbSeccion.Text == "TODAS" ? (int?)null : Convert.ToInt32(cbbSeccion.SelectedValue);
                string grado = cbbGrado.Text == "TODOS" ? null : cbbGrado.Text;
                string seccion = cbbSeccion.Text == "TODAS" ? null : cbbSeccion.Text;

                SqlParameter[] p =
                {
                    //new SqlParameter("@Anio", dtpAnio.Value),
                    new SqlParameter("@Anio", dtpAnio.Text),
                    //new SqlParameter("@GradoID", (object)grado ?? DBNull.Value),
                    //new SqlParameter("@SeccionID", (object)seccion ?? DBNull.Value),
                    //new SqlParameter("@BusquedaDocente", (object)txtBuscarDocente.Text ?? DBNull.Value)

                    new SqlParameter("@Grado", (object)grado ?? DBNull.Value),
                    new SqlParameter("@Seccion", (object)seccion ?? DBNull.Value),
                    new SqlParameter("@Docente", (object)txtBuscarDocente.Text ?? DBNull.Value)
                };
                //DataTable dt = util.EjecutarSPParametros("spMAE_ListarCargaAcademica", p);
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
            // Codificacion del Load
            CargarGrados2();
            CargarSecciones();
            //LlenarDocentes();
            //CargarDocentes();
            BuscarDocentes2();
            CargarGraficoCargaAcademica();
        }

        private void cbbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Codificacion del evento selectedindex para cuando se seleccione el grado
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
            // Codificacion del evento selectedindex para cuando se seleccione seccion
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
            // Codificacion para el evento cuando se cambie de año
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
            // Codificacion para evento al predionar buscar 
            BuscarDocentes2();
            /*            LlenarDocentes()*/
            ;
            CargarGraficoCargaAcademica();
        }

        private void btBuscarAnio_Click(object sender, EventArgs e)
        {

        }

        private void dgvDocentes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
            //if (dgvDocentes.CurrentRow != null)
            //{
            //    int docenteID = Convert.ToInt32(dgvDocentes.CurrentRow.Cells["DocenteID"].Value);

            //    //CargarAsignaturasDocente(docenteID);
            //}
        }

        private void btNuevaCarga_Click(object sender, EventArgs e)
        {
            // Codificacion para abrir el form de Asignacion al presionar nueva carga
            Pantallas.AdminVentanas.AsignacionCarga forma = new Pantallas.AdminVentanas.AsignacionCarga();
            forma.Show();
        }

        private void txtBuscarDocente_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validacion de solo letras
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;

                MessageBox.Show("Solo se aceptan letras.",
                                "Validación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void ConfigurarColumnas()
        {
            // Metodo para configurar las columnas
            if (!dgvDocentes.Columns.Contains("colEstado"))
            {
                DataGridViewImageColumn colEstado = new DataGridViewImageColumn();
                colEstado.Name = "colEstado";
                colEstado.HeaderText = "ESTADO";
                colEstado.ImageLayout = DataGridViewImageCellLayout.Zoom;
                dgvDocentes.Columns.Add(colEstado);
            }

            if (!dgvDocentes.Columns.Contains("colVer"))
            {
                DataGridViewImageColumn colVer = new DataGridViewImageColumn();
                colVer.Name = "colVer";
                colVer.HeaderText = "VER";
                colVer.ImageLayout = DataGridViewImageCellLayout.Zoom;
                dgvDocentes.Columns.Add(colVer);
            }

            if (!dgvDocentes.Columns.Contains("colEditar"))
            {
                DataGridViewImageColumn colEditar = new DataGridViewImageColumn();
                colEditar.Name = "colEditar";
                colEditar.HeaderText = "EDITAR";
                colEditar.ImageLayout = DataGridViewImageCellLayout.Zoom;
                dgvDocentes.Columns.Add(colEditar);
            }

        }

        private void dgvDocentes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Codificacion para dar formato a las filas del datagrip Docentes
            if (e.RowIndex < 0) return;

            // Imagen Editar
            if (dgvDocentes.Columns[e.ColumnIndex].Name == "colEditar")
            {
                e.Value = Properties.Resources.BotonEditar1;
            }

            if (dgvDocentes.Columns[e.ColumnIndex].Name == "colVer")
            {
                e.Value = Properties.Resources.btnVer;
            }

            // Imagen Estado
            if (dgvDocentes.Columns[e.ColumnIndex].Name == "colEstado")
            {
                //int estado = Convert.ToInt32(dgvDocentes.Rows[e.RowIndex].Cells["Estado"].Value);
                object valor = dgvDocentes.Rows[e.RowIndex].Cells["Estado"].Value;

                int estado = valor == null || valor == DBNull.Value
                             ? 0   // valor por defecto
                             : Convert.ToInt32(valor);

                e.Value = estado == 1
                    ? Properties.Resources.btActivo1
                    : Properties.Resources.btInactivo;
            }
        }

        private void dgvDocentes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Codificacion para cargar los datos al form Asignacion cuando se de click en el boton editar
            if (e.RowIndex < 0) return;
            if (dgvDocentes.Columns[e.ColumnIndex].Name == "colVer")
            {
                int docenteID = Convert.ToInt32(dgvDocentes.CurrentRow.Cells["DocenteID"].Value);
                int seccionID = Convert.ToInt32(dgvDocentes.CurrentRow.Cells["SeccionID"].Value);

                CargarAsignaturasDocente(docenteID, seccionID);
            }

            if (e.RowIndex < 0) return;
            if (dgvDocentes.Columns[e.ColumnIndex].Name == "colEditar")
            {
                DataGridViewRow row = dgvDocentes.Rows[e.RowIndex];

                AsignacionCarga forma = new AsignacionCarga(
                    Convert.ToInt32(row.Cells["CargaID"].Value),
                    Convert.ToInt32(row.Cells["DocenteID"].Value),
                    row.Cells["Asignaturas"].Value.ToString(),
                    row.Cells["Grados"].Value.ToString(),
                    row.Cells["Secciones"].Value.ToString(),
                    dtpAnio.Value.Year,
                    Convert.ToInt32(row.Cells["Estado"].Value)
                );

                forma.Show();
            }


        }

        private void dgvAsignatura_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Codificacion para dar formato a las columnas del datagrip Asignatura
            if (e.RowIndex < 0) return;

            if (dgvAsignatura.Columns[e.ColumnIndex].Name == "colBorrar")
            {
                e.Value = Properties.Resources.btnBorrar;
            }
        }

        private void ConfigurarColumnas2()
        {
            // Configurar columnas en el datagrip Asignaturas
            if (!dgvAsignatura.Columns.Contains("colBorrar"))
            {
                DataGridViewImageColumn colBorrar = new DataGridViewImageColumn();
                colBorrar.Name = "colBorrar";
                colBorrar.HeaderText = "ELIMINAR";
                colBorrar.ImageLayout = DataGridViewImageCellLayout.Zoom;
                dgvAsignatura.Columns.Add(colBorrar);
            }
        }
    }
}
