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

        private void CargarInformacionEstudiante()
        {
            try {
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
