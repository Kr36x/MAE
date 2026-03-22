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
            EjecutarUtilidades util = new EjecutarUtilidades();
            DataTable tabla = util.EjecutarConsulta("SELECT * FROM vMAE_TraeGrados order by GradoID GO");
            cbbGrado.DataSource = tabla;
            cbbGrado.DisplayMember = "NombreGrado";
            cbbGrado.ValueMember = "GradoID";
        }
        private void CargarEstudiantes()
        {
            EjecutarUtilidades util = new EjecutarUtilidades();
            string consulta = "SELECT * FROM vMAE_EstudianteGradoAnio";
            DataTable dt = util.EjecutarConsulta(consulta);
            dgvEstudiantes.DataSource = dt;
        }
        private void BuscarConSP()
        {
            EjecutarUtilidades util = new EjecutarUtilidades();
            SqlParameter[] p =
            {
                new SqlParameter("@Nombre", string.IsNullOrWhiteSpace(txtBuscarEstudiante.Text) ? DBNull.Value : txtBuscarEstudiante.Text),
                new SqlParameter("@Anio", string.IsNullOrWhiteSpace(dtpAnio.Text) ? DBNull.Value : dtpAnio.Text),
                new SqlParameter("@Grado", string.IsNullOrWhiteSpace(cbbGrado.Text) ? DBNull.Value : cbbGrado.Text)
            };
            dgvEstudiantes.DataSource = util.EjecutarSP("spMAE_BuscarEstudiantes", p);
        }

        private void FrmEstudiantes_Load(object sender, EventArgs e)
        {
            CargarGrados();
            CargarEstudiantes();
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
                // Obtener el ID del estudiante desde la fila seleccionada
                int estudianteID = Convert.ToInt32(
                    dgvEstudiantes.Rows[e.RowIndex].Cells["EstudianteID"].Value
                );

                // Abrir el formulario de Ficha enviando el ID
                FrmFichaMatricula FrmMatriculaVigente = new FrmFichaMatricula(estudianteID);
                FrmMatriculaVigente.Show();
            }

        }
    }
}
