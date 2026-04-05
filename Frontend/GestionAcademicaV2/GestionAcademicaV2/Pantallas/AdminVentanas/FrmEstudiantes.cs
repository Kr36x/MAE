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

            //try
            //{
            //    EjecutarUtilidades util = new EjecutarUtilidades();
            //    DataTable tabla = util.EjecutarConsulta("SELECT * FROM vMAE_TraeGrados order by GradoID");
            //    cbbGrado.DataSource = tabla;
            //    cbbGrado.DisplayMember = "NombreGrado";
            //    cbbGrado.ValueMember = "GradoID";
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error al cargar los grados: " + ex.Message);
            //}
        }

        private void CargarEstudiantes()
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                string consulta = "SELECT * FROM vMAE_EstudianteGradoAnio";
                DataTable dt = util.EjecutarConsulta(consulta);
                dgvEstudiantes.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los Estudiantes: " + ex.Message);
            }
        }
        private void BuscarConSP()
        {
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

                dgvEstudiantes.DataSource = util.EjecutarSPParametros("spMAE_BuscarEstudiantes", p);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message);
            }

            //try
            //{
            //    EjecutarUtilidades util = new EjecutarUtilidades();
            //    SqlParameter[] p =
            //    {
            //        new SqlParameter("@Nombre", string.IsNullOrWhiteSpace(txtBuscarEstudiante.Text) ? DBNull.Value : txtBuscarEstudiante.Text),
            //        new SqlParameter("@Anio", string.IsNullOrWhiteSpace(dtpAnio.Text) ? DBNull.Value : dtpAnio.Text),
            //        new SqlParameter("@Grado", string.IsNullOrWhiteSpace(cbbGrado.Text) ? DBNull.Value : cbbGrado.Text)
            //    };
            //    dgvEstudiantes.DataSource = util.EjecutarSP("spMAE_BuscarEstudiantes", p);
            //}catch (Exception ex)
            //{
            //    MessageBox.Show("Error al buscar: " + ex.Message);
            //}
        }

        private void FrmEstudiantes_Load(object sender, EventArgs e)
        {
            CargarGrados();
            CargarEstudiantes();
            if (!dgvEstudiantes.Columns.Contains("btnVer"))
            {
                DataGridViewButtonColumn btnVer = new DataGridViewButtonColumn();
                btnVer.Name = "btnVer";
                btnVer.HeaderText = "VER";
                btnVer.Text = "";
                btnVer.UseColumnTextForButtonValue = false;
                dgvEstudiantes.Columns.Add(btnVer);
            }

            if (!dgvEstudiantes.Columns.Contains("btnEditar"))
            {
                DataGridViewButtonColumn btnEditar = new DataGridViewButtonColumn();
                btnEditar.Name = "btnEditar";
                btnEditar.HeaderText = "EDITAR";
                btnEditar.Text = "";
                btnEditar.UseColumnTextForButtonValue = false;
                dgvEstudiantes.Columns.Add(btnEditar);
            }
        }

        private void btBuscarEstudiante_Click(object sender, EventArgs e)
        {
            BuscarConSP();
        }

        private void cbbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuscarConSP();
        }

        private void dtpAnio_ValueChanged(object sender, EventArgs e)
        {
            BuscarConSP();
        }

        private void dgvEstudiantes_DoubleClick(object sender, EventArgs e)
        {

        }

        private void dgvEstudiantes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int estudianteID = Convert.ToInt32(
                    dgvEstudiantes.Rows[e.RowIndex].Cells["EstudianteID"].Value
                );
                FrmFichaMatricula FrmMatriculaVigente = new FrmFichaMatricula(estudianteID);
                FrmMatriculaVigente.Show();
            }

        }

        private void dgvEstudiantes_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                int iconSize = 20;

                if (dgvEstudiantes.Columns[e.ColumnIndex].Name == "btnVer")
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                    Image img = Properties.Resources.ojo_abierto;

                    int x = e.CellBounds.Left + (e.CellBounds.Width - iconSize) / 2;
                    int y = e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2;

                    e.Graphics.DrawImage(img, new Rectangle(x, y, iconSize, iconSize));
                    e.Handled = true;
                }

                if (dgvEstudiantes.Columns[e.ColumnIndex].Name == "btnEditar")
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                    Image img = Properties.Resources.report_blanco;

                    int x = e.CellBounds.Left + (e.CellBounds.Width - iconSize) / 2;
                    int y = e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2;

                    e.Graphics.DrawImage(img, new Rectangle(x, y, iconSize, iconSize));
                    e.Handled = true;
                }
            }


        }
    }
}
