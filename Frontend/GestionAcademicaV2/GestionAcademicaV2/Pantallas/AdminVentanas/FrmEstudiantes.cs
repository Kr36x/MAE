using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmEstudiantes : Form
    {
        private PantallaAdmin pantallaPrincipal;
        public FrmEstudiantes(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }
        private void CargarGrados()
        {
            // Metodo para cargar grados al ejecutar el form
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
        private void BuscarConSP()
        {
            // Metodo para buscar y aplicar filtos en el form
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                string grado = cbbGrado.Text == "TODOS" ? null : cbbGrado.Text;

                SqlParameter[] p =
                {
                    new SqlParameter("@Nombre",
                    string.IsNullOrWhiteSpace(txtBuscarEstudiante.Text)
                    ? DBNull.Value
                    : txtBuscarEstudiante.Text),

                    new SqlParameter("@Anio",
                    string.IsNullOrWhiteSpace(dtpAnio.Text)
                    ? DBNull.Value
                    : dtpAnio.Text),

                    new SqlParameter("@Grado",
                    (object)grado ?? DBNull.Value)
                };

                //dgvEstudiantes.DataSource = util.EjecutarSPParametros("spMAE_BuscarEstudiantes", p);
                DataTable dt = util.EjecutarSPParametros("spMAE_BuscarEstudiantes", p);

                // VALIDACIÓN DE DATOS
                if (dt.Rows.Count == 0)
                {
                    dgvEstudiantes.Visible=false;
                    lbDatosEstudiantes.Visible = true;
                }
                else
                {
                    dgvEstudiantes.Visible = true;
                    lbDatosEstudiantes.Visible = false;
                    dgvEstudiantes.DataSource = dt;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message);
            }
        }

        private void FrmEstudiantes_Load(object sender, EventArgs e)
        {
            // Codificación del Load
            CargarGrados();
            BuscarConSP();
            ConfigurarColumnas();
        }

        private void btBuscarEstudiante_Click(object sender, EventArgs e)
        {

        }

        private void cbbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Aplicar filtro al seleccionar grados
            BuscarConSP();
        }

        private void dtpAnio_ValueChanged(object sender, EventArgs e)
        {
            // Aplicar filtro al cambiar año
            BuscarConSP();
        }

        private void dgvEstudiantes_DoubleClick(object sender, EventArgs e)
        {

        }

        private void dgvEstudiantes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvEstudiantes_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void dgvEstudiantes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Codificacion para abrir el form Ficha Matricula al dar click en los botones del grip
            if (e.RowIndex < 0) return;

            string col = dgvEstudiantes.Columns[e.ColumnIndex].Name;

            int estudianteID = Convert.ToInt32(
                dgvEstudiantes.Rows[e.RowIndex].Cells["EstudianteID"].Value
            );

            if (col == "btnVer")
            {
                FrmFichaMatricula frm = new FrmFichaMatricula(estudianteID,1);
                frm.Show();
                return;
            }

            if (col == "btnEditar")
            {
                FrmFichaMatricula frm = new FrmFichaMatricula(estudianteID,2);
                frm.Show();
                return;
            }

        }

        private void ConfigurarColumnas()
        {
            // Metodo para configurar columnas
            if (!dgvEstudiantes.Columns.Contains("btnVer"))
            {
                DataGridViewImageColumn colEstado = new DataGridViewImageColumn();
                colEstado.Name = "btnVer";
                colEstado.HeaderText = "VER";
                colEstado.ImageLayout = DataGridViewImageCellLayout.Zoom;
                dgvEstudiantes.Columns.Add(colEstado);
            }

            if (!dgvEstudiantes.Columns.Contains("btnEditar"))
            {
                DataGridViewImageColumn colEditar = new DataGridViewImageColumn();
                colEditar.Name = "btnEditar";
                colEditar.HeaderText = "EDITAR";
                colEditar.ImageLayout = DataGridViewImageCellLayout.Zoom;
                dgvEstudiantes.Columns.Add(colEditar);
            }
        }


        private void txtBuscarEstudiante_TextChanged(object sender, EventArgs e)
        {
            BuscarConSP();
        }

        private void guna2ContainerControl1_Click(object sender, EventArgs e)
        {

        }

        private void dgvEstudiantes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Establecer formato para las columnas del datagrip Estudiante
            if (e.RowIndex < 0) return;

            // Imagen Editar
            if (dgvEstudiantes.Columns[e.ColumnIndex].Name == "btnEditar")
            {
                e.Value = Properties.Resources.BotonEditar1;
            }

            // Imagen Ver
            if (dgvEstudiantes.Columns[e.ColumnIndex].Name == "btnVer")
            {
                e.Value = Properties.Resources.btnVer;
            }

        }
    }
}
