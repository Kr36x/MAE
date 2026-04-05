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
    public partial class FrmFichaMatricula : Form
    {
        private int estudianteID;

        public FrmFichaMatricula(int id)
        {
            InitializeComponent();
            estudianteID = id;
        }

        private void CargarFichaMatricula()
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                // Obtener la matrícula más reciente
                int matriculaID = ObtenerMatriculaID();

                SqlParameter[] p =
                {
                    new SqlParameter("@estudianteID", estudianteID),
                    new SqlParameter("@matriculaID", matriculaID)
                };

                DataTable dt = util.EjecutarSPParametros("spMAE_RepFichaMatricula", p);

                if (dt.Rows.Count == 0)
                    return;

                DataRow row = dt.Rows[0];

                txtNombreEstudiante.Text = row["Nombre"].ToString();
                txtIdentidadEstudiante.Text = row["Identidad"].ToString();
                txtGenero.Text = row["Sexo"].ToString();
                txtDireccion.Text = row["Direccion"].ToString();
                txtTelefonoEstudiante.Text = row["Telefono"].ToString();
                txtFechaNacimiento.Text = Convert.ToDateTime(row["FechaNacimiento"]).ToString("dd/MM/yyyy");
                txtMano.Text = row["Mano"].ToString();
                txtAlergias.Text = row["Alergia"].ToString();

                txtGrado.Text = row["GradoID"].ToString();
                txtSeccion.Text = row["Letra"].ToString();
                txtAnio.Text = Convert.ToDateTime(row["Fecha"]).Year.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ficha: " + ex.Message);
            }
        }

        private int ObtenerMatriculaID()
        {
            EjecutarUtilidades util = new EjecutarUtilidades();

            DataTable dt = util.EjecutarConsulta(
                $"SELECT TOP 1 MatriculaID FROM Matricula WHERE EstudianteID = {estudianteID} ORDER BY Anio DESC"
            );

            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["MatriculaID"]);

            return 0;
        }

        private void CargarTutores()
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                SqlParameter[] p =
                {
                    new SqlParameter("@estudianteID", estudianteID)
                };

                DataTable dt = util.EjecutarSPParametros("spMAE_TraeTutoresxEstudiante", p);

                if (dt.Rows.Count > 0)
                {
                    DataRow t1 = dt.Rows[0];
                    txtNombrePadre.Text = t1["Nombre"].ToString();
                    txtIdentidadPadre.Text = t1["Identidad"].ToString();
                    txtTelefonoPadre.Text = t1["Telefono"].ToString();
                    txtTrabajoPadre.Text = t1["LugarTrabajo"].ToString();
                }

                if (dt.Rows.Count > 1)
                {
                    DataRow t2 = dt.Rows[1];
                    txtNombreMadre.Text = t2["Nombre"].ToString();
                    txtIdentidadMadre.Text = t2["Identidad"].ToString();
                    txtTelefonoMadre.Text = t2["Telefono"].ToString();
                    txtTrabajoMadre.Text = t2["LugarTrabajo"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tutores: " + ex.Message);
            }
        }


        private void CargarInformacionEstudiante()
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                SqlParameter[] p =
                {
                    new SqlParameter("@EstudianteID", estudianteID)
                };
                DataTable dt = util.EjecutarSPParametros("spMAE_DetalleEstudianteCompleto", p);
                if (dt.Rows.Count > 0)
                {
                    txtNombreEstudiante.Text = dt.Rows[0]["NombreEstudiante"].ToString();
                    txtIdentidadEstudiante.Text = dt.Rows[0]["IdentidadEstudiante"].ToString();
                    txtGenero.Text = dt.Rows[0]["Sexo"].ToString();
                    txtDireccion.Text = dt.Rows[0]["Direccion"].ToString();
                    txtTelefonoEstudiante.Text = dt.Rows[0]["TelefonoEstudiante"].ToString();
                    txtFechaNacimiento.Text = Convert.ToDateTime(dt.Rows[0]["FechaNacimiento"]).ToString("dd/MM/yyyy");
                    txtMano.Text = dt.Rows[0]["Mano"].ToString();
                    txtAlergias.Text = dt.Rows[0]["Alergia"].ToString();
                    txtEstado.Text = dt.Rows[0]["Estado"].ToString();

                    txtGrado.Text = dt.Rows[0]["NombreGrado"].ToString();
                    txtAnio.Text = dt.Rows[0]["AnioAcademico"].ToString();

                    txtNombrePadre.Text = dt.Rows[0]["NombrePadre"].ToString();
                    txtIdentidadPadre.Text = dt.Rows[0]["IdentidadPadre"].ToString();
                    txtTelefonoPadre.Text = dt.Rows[0]["TelefonoPadre"].ToString();
                    txtTrabajoPadre.Text = dt.Rows[0]["LugarTrabajoPadre"].ToString();

                    txtNombreMadre.Text = dt.Rows[0]["NombreMadre"].ToString();
                    txtIdentidadMadre.Text = dt.Rows[0]["IdentidadMadre"].ToString();
                    txtTelefonoMadre.Text = dt.Rows[0]["TelefonoMadre"].ToString();
                    txtTrabajoMadre.Text = dt.Rows[0]["LugarTrabajoMadre"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al llenar el formulario: " + ex.Message);
            }
        }
        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void FrmFichaMatricula_Load(object sender, EventArgs e)
        {
            CargarInformacionEstudiante();
            //CargarFichaMatricula();
            //CargarTutores();

        }

        private void FrmFichaMatricula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
