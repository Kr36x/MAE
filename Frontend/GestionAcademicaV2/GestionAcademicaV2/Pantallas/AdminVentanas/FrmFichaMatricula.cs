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
        private int veroeditar;
        private int matriculaID;

        public FrmFichaMatricula(int id, int ver)
        {
            InitializeComponent();
            estudianteID = id;
            matriculaID = 0;
            veroeditar = ver;
        }

        private void CargarFichaMatriculaEditar()
        {
            // Metodo para cargar los datos del form al abrir, cuando se da click al editar del form Estudiantes
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                matriculaID = ObtenerMatriculaID();
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
                string sexo = row["Sexo"].ToString();
                if (sexo == "M")
                {
                    txtGenero.Text = "MASCULINO";
                }
                else
                {
                    txtGenero.Text = "FEMENINO";
                }
                txtDireccion.Text = row["Direccion"].ToString();
                txtTelefonoEstudiante.Text = row["Telefono"].ToString();
                dtpFechaNacimiento.Value = Convert.ToDateTime(row["FechaNacimiento"]);
                cbbMano.Text = row["Mano"].ToString();
                txtAlergias.Text = row["Alergia"].ToString();
                cbbGrado.SelectedValue = Convert.ToInt32(row["GradoID"]);
                cbbSeccion.Text = row["Letra"].ToString();
                txtAnio.Text = Convert.ToDateTime(row["Fecha"]).Year.ToString();

                cbbGrado.Enabled = true;
                txtDireccion.Enabled = true;
                cbbMano.Enabled = true;
                txtAlergias.Enabled = true;
                txtTelefonoEstudiante.Enabled = true;
                btEditarMatricula.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ficha: " + ex.Message);
            }
        }
        private void CargarFichaMatricula()
        {
            // Metodo para cargar los datos del form al abrir, cuando se da click al ver del form Estudiantes
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
                string sexo = row["Sexo"].ToString();
                if (sexo == "M")
                {
                    txtGenero.Text = "MASCULINO";
                }
                else
                {
                    txtGenero.Text = "FEMENINO";
                }
                txtDireccion.Text = row["Direccion"].ToString();
                txtTelefonoEstudiante.Text = row["Telefono"].ToString();
                dtpFechaNacimiento.Value = Convert.ToDateTime(row["FechaNacimiento"]);
                cbbMano.Text = row["Mano"].ToString();
                txtAlergias.Text = row["Alergia"].ToString();
                cbbGrado.SelectedValue = Convert.ToInt32(row["GradoID"]);
                cbbSeccion.Text = row["Letra"].ToString();
                txtAnio.Text = Convert.ToDateTime(row["Fecha"]).Year.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ficha: " + ex.Message);
            }
        }

        private int ObtenerMatriculaID()
        {
            // Metodo para obtener la matriculaId Para poder llenar algunos datos del form, que necesitan este parametro
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
            // Este metodo carga la informacion de los tutores
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
                    string Tutor1 = t1["Parentesco"].ToString();
                    lbPadre.Text = Tutor1;
                    txtTrabajoPadre.Text = t1["LugarTrabajo"].ToString();
                    txtCorreoPadre.Text = t1["Correo"].ToString();
                    if (txtCorreoPadre.Text == "")
                    {
                        txtCorreoPadre.Text = " ";
                    }
                }

                if (dt.Rows.Count > 1)
                {
                    DataRow t2 = dt.Rows[1];
                    txtNombreMadre.Text = t2["Nombre"].ToString();
                    txtIdentidadMadre.Text = t2["Identidad"].ToString();
                    txtTelefonoMadre.Text = t2["Telefono"].ToString();
                    string Tutor2 = t2["Parentesco"].ToString();
                    lbMadre.Text = Tutor2;
                    txtTrabajoMadre.Text = t2["LugarTrabajo"].ToString();
                    txtCorreoMadre.Text = t2["Correo"].ToString();
                    if (txtCorreoMadre.Text == "")
                    {
                        txtCorreoMadre.Text = " ";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tutores: " + ex.Message);
            }
        }

        private void CargarGrados()
        {
            // Carga todos los grados al ejecutar el form
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                DataTable tabla = util.EjecutarConsulta("SELECT * FROM vMAE_TraeGrados ORDER BY GradoID");

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

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void FrmFichaMatricula_Load(object sender, EventArgs e)
        {
            // Codificacion del load
            CargarGrados();
            if (veroeditar == 2)
            {
                lbTituloMatricula.Text = "EDITAR MATRICULA DE ESTUDIANTE";
                CargarFichaMatriculaEditar();
            }
            else
            {
                CargarFichaMatricula();
            }
            cbbSeccion.Enabled = false;
            CargarTutores();
        }

        private void FrmFichaMatricula_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void cbbSexo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CargarSeccionesPorGrado(int gradoID)
        {
            //  Metodo para cargar secciones segun el grado seleccionado
            EjecutarUtilidades util = new EjecutarUtilidades();

            SqlParameter[] p =
            {
                new SqlParameter("@GradoID", gradoID)
            };

            DataTable dt = util.EjecutarSPParametros("spMAE_SeccionesPorGrado", p);

            cbbSeccion.DataSource = dt;
            cbbSeccion.DisplayMember = "Letra";
            cbbSeccion.ValueMember = "SeccionID";
        }
        private void btEditarMatricula_Click(object sender, EventArgs e)
        {
            // Codificacion para el boton editar Matricula
            try
            {
                // Validaciones
                if(txtTelefonoEstudiante.TextLength<9)
                {
                    txtTelefonoEstudiante.Focus();
                    MessageBox.Show("El número de teléfono debe tener 8 dígitos.");
                    return;
                }

                EjecutarUtilidades util = new EjecutarUtilidades();
                

                DataTable dtSeccion = util.EjecutarConsulta(
                    "SELECT TOP 1 Letra FROM Seccion WHERE GradoID = " + cbbGrado.SelectedValue);

                if (dtSeccion.Rows.Count == 0)
                {
                    MessageBox.Show("No existe sección para este grado.");
                    return;
                }

                string seccionLetra = dtSeccion.Rows[0]["Letra"].ToString();

                SqlParameter[] p =
                {
                    new SqlParameter("@nombreEst", txtNombreEstudiante.Text),
                    new SqlParameter("@fechaNacimiento", dtpFechaNacimiento.Value),

                    new SqlParameter("@sexo", txtGenero.Text.Substring(0, 1)),
                    new SqlParameter("@dniEst", txtIdentidadEstudiante.Text),
                    new SqlParameter("@direccionEst", txtDireccion.Text),
                    new SqlParameter("@telEst", txtTelefonoEstudiante.Text),
                    new SqlParameter("@mano", cbbMano.Text),
                    new SqlParameter("@alergia", txtAlergias.Text),

                    new SqlParameter("@imagen", DBNull.Value),

                    new SqlParameter("@gradoID", Convert.ToInt32(cbbGrado.SelectedValue)),
                    new SqlParameter("@seccionID", cbbSeccion.Text),

                    new SqlParameter("@nombreTut1", txtNombrePadre.Text),
                    new SqlParameter("@dniTut1", txtIdentidadPadre.Text),
                    new SqlParameter("@telTut1", txtTelefonoPadre.Text),
                    new SqlParameter("@lugTrabTut1", txtTrabajoPadre.Text),
                    new SqlParameter("@correoTut1", txtCorreoPadre.Text),
                    new SqlParameter("@parentescoTut1",string.IsNullOrWhiteSpace(lbPadre.Text)? (object)DBNull.Value: lbPadre.Text),
                    new SqlParameter("@nombreTut2", string.IsNullOrWhiteSpace(txtNombreMadre.Text) ? (object)DBNull.Value : txtNombreMadre.Text),
                    new SqlParameter("@dniTut2", string.IsNullOrWhiteSpace(txtIdentidadMadre.Text) ? (object)DBNull.Value : txtIdentidadMadre.Text),
                    new SqlParameter("@telTut2", string.IsNullOrWhiteSpace(txtTelefonoMadre.Text) ? (object)DBNull.Value : txtTelefonoMadre.Text),
                    new SqlParameter("@lugTrabTut2", string.IsNullOrWhiteSpace(txtTrabajoMadre.Text) ? (object)DBNull.Value : txtTrabajoMadre.Text),
                    new SqlParameter("@correoTut2",txtCorreoMadre.Text),
                    new SqlParameter("@parentescoTut2", string.IsNullOrWhiteSpace(lbMadre.Text) ? (object)DBNull.Value : lbMadre.Text),
                    new SqlParameter("@matriculaID", matriculaID == 0 ? (object)DBNull.Value : matriculaID)
                };

                DataTable dt = util.EjecutarSPParametros("spMAE_Matricular", p);

                if (dt.Rows.Count > 0)
                {
                    matriculaID = Convert.ToInt32(dt.Rows[0][0]);
                }

                MessageBox.Show("La matrícula se edito correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar matrícula: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtTelefonoEstudiante_TextChanged(object sender, EventArgs e)
        {
            // Validacion de formato del text Telefono
            if (txtTelefonoEstudiante.Text.Length == 4 && !txtTelefonoEstudiante.Text.Contains("-"))
            {
                txtTelefonoEstudiante.Text += "-";
                txtTelefonoEstudiante.SelectionStart = txtTelefonoEstudiante.Text.Length;
            }
        }

        private void txtTelefonoEstudiante_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validacion de formato del text Telefono estudiante
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Solo se permiten números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cbbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            // BUsqueda de secciones al seleccionar grado
            if (cbbGrado.SelectedValue == null) return;

            if (cbbGrado.SelectedValue is DataRowView) return;

            int gradoID = Convert.ToInt32(cbbGrado.SelectedValue);
            CargarSeccionesPorGrado(gradoID);
            cbbSeccion.Enabled = true;
        }
    }
}
