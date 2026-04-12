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
    public partial class AsignacionCarga : Form
    {
        int carga = 0;
        public AsignacionCarga()
        {
            InitializeComponent();
        }
        
        public AsignacionCarga(int cargaID, int docenteID, string asignatura, string grado, string seccion, int anio,
            int estado)
        {
            InitializeComponent();
            carga = cargaID;
            txtCargaID.Text = cargaID.ToString();

            cbbDocentes.SelectedValue = docenteID;
            cbbAsignatura.Text = asignatura;
            cbbGrado.Text = grado;
            cbbSeccion.Text = seccion;
            cbbAnio.Text = anio.ToString();
            cbbEstado.SelectedValue = estado;
        }

        
        private void CargarDocentes()
        {
            // Aquí se cargan los docentes para poder buscar
            try
            {
                EjecutarUtilidades ejecutar = new EjecutarUtilidades();
                DataTable dt = ejecutar.EjecutarConsulta("spMAE_TraeDocentes");

                cbbDocentes.DataSource = dt;
                cbbDocentes.DisplayMember = "Nombre";
                cbbDocentes.ValueMember = "DocenteID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar docentes: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void CargarSeccionesPorGrado(int gradoID)
        {
            // Aquí se cargan las secciones en el form
            try
            {
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
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar secciones: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AsignacionCarga_Load(object sender, EventArgs e)
        {
            // Códificación del load
            if(carga==0)
            {
                CargarDocentes();
                CargarGrados();
                CargarAsignaturas();
                lbEstado.Visible=false;
                cbbEstado.Visible = false;
                btEditar.Visible = false;
            }
            else
            {
                lbTitulo.Text = "EDITAR CARGA ACADÉMICA";
                cbbEstado.Visible = true;
                btEditar.Visible = true;
                lbEstado.Visible = true;
            }

        }

        private void cbbDocentes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbbDocentes_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBuscarDocente_TextChanged(object sender, EventArgs e)
        {
            // Codificación del textbox de búsqueda
            try
            {
                string filtro = txtBuscarDocente.Text.Trim();

                if (filtro.Length < 1)
                {
                    cbbDocentes.DataSource = null;
                    return;
                }

                EjecutarUtilidades util = new EjecutarUtilidades();

                SqlParameter[] p =
                {
                new SqlParameter("@Filtro", filtro)
            };

                DataTable dt = util.EjecutarSPParametros("spMAE_BuscarDocentes", p);

                cbbDocentes.DataSource = dt;
                cbbDocentes.DisplayMember = "Nombre";
                cbbDocentes.ValueMember = "DocenteID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar docentes: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarGrados()
        {
            // Aquí se cargan los grados en el form
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                DataTable tabla = util.EjecutarConsulta("SELECT * FROM vMAE_TraeGrados order by GradoID");
                cbbGrado.DataSource = tabla;
                cbbGrado.DisplayMember = "NombreGrado";
                cbbGrado.ValueMember = "GradoID";
                cbbGrado.SelectedIndex = -1;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al llenar grados: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btInformacion_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Este formulario sirve para poder asignar una asignaturas a los docentes.",
                                "Información",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
        }

        private void btCancelar_Click(object sender, EventArgs e)
        {
            // Codificación del boton cancelar
            this.Close();
        }

        private void cbbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Mécanica para encontrar las secciones habilitadas para el grado seleccionado
            if (cbbGrado.SelectedValue == null) return;

            if (cbbGrado.SelectedValue is DataRowView) return;

            int gradoID = Convert.ToInt32(cbbGrado.SelectedValue);
            CargarSeccionesPorGrado(gradoID);
        }

        private void CargarAsignaturas()
        {
            // Cargar los datos para el combobox de asignaturas
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                DataTable tabla = util.EjecutarConsulta("spMAE_TraeAsignaturas");

                cbbAsignatura.DataSource = tabla;
                cbbAsignatura.DisplayMember = "Nombre";
                cbbAsignatura.ValueMember = "AsignaturaID";

                cbbAsignatura.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al llenar asignaturas: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBuscarAsignatura_TextChanged(object sender, EventArgs e)
        {
            // Mécanica para buscar asignaturas en el combobox desde el txtBuscarAsignatura
            try
            {
                string filtro = txtBuscarAsignatura.Text.Trim();

                if (filtro.Length < 1)
                {
                    cbbAsignatura.DataSource = null;
                    return;
                }

                EjecutarUtilidades util = new EjecutarUtilidades();

                SqlParameter[] p =
                {
                new SqlParameter("@Filtro", filtro)
                };

                DataTable dt = util.EjecutarSPParametros("spMAE_BuscarAsignaturas", p);

                cbbAsignatura.DataSource = dt;
                cbbAsignatura.DisplayMember = "Nombre";
                cbbAsignatura.ValueMember = "AsignaturaID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar asignaturas: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBuscarDocente_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validación que solo deje ingresar letras.
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

        private void txtBuscarAsignatura_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validación para que solo deje ingresar letras.
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

        private void btAsignar_Click(object sender, EventArgs e)
        {
            // Codificación para poder asignar los datos y almacenarlos en la base de datos
            try
            {
                if (cbbDocentes.SelectedValue == null ||
                    cbbGrado.SelectedValue == null ||
                    cbbSeccion.SelectedValue == null ||
                    cbbAsignatura.SelectedValue == null)
                {
                    MessageBox.Show("Debe completar todos los campos.");
                    return;
                }

                int docenteID = Convert.ToInt32(cbbDocentes.SelectedValue);
                int gradoID = Convert.ToInt32(cbbGrado.SelectedValue);
                int seccionID = Convert.ToInt32(cbbSeccion.SelectedValue);
                int asignaturaID = Convert.ToInt32(cbbAsignatura.SelectedValue);
                int anio = Convert.ToInt32(cbbAnio.Text);

                SqlParameter[] p =
                {
                    new SqlParameter("@DocenteID", docenteID),
                    new SqlParameter("@GradoID", gradoID),
                    new SqlParameter("@SeccionID", seccionID),
                    new SqlParameter("@AsignaturaID", asignaturaID),
                    new SqlParameter("@Anio", anio)
                };

                EjecutarUtilidades util = new EjecutarUtilidades();
                object result = util.EjecutarSPScalar("spMAE_AgregarCargaAcademica", p);

                if (result != null)
                {
                    int nuevoID = Convert.ToInt32(result);
                    MessageBox.Show("Carga académica asignada correctamente. ID: " + nuevoID);
                }

                CargarDocentes();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
