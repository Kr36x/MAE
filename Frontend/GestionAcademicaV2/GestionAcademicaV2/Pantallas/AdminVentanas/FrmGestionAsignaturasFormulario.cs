using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmGestionAsignaturasFormulario : Form
    {
        private readonly Conexion conexion = new Conexion();

        private readonly int _asignaturaId = 0;
        private readonly bool _esEdicion = false;
        private bool _cargandoCombo = false;

        public FrmGestionAsignaturasFormulario()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }

        public FrmGestionAsignaturasFormulario(int asignaturaId)
        {
            InitializeComponent();
            _asignaturaId = asignaturaId;
            _esEdicion = true;
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            Load += FrmNuevaAsignatura_Load;
            btnCancelar.Click += btnCancelar_Click;
            btnCrear.Click += btnCrear_Click;

            txtValor.MaxLength = 60;
            txtDescripcion.MaxLength = 150;

            cbActividad.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void FrmNuevaAsignatura_Load(object sender, EventArgs e)
        {
            CargarAreas();

            if (_esEdicion)
            {
                Text = "Editar Asignatura";
                guna2HtmlLabel1.Text = "EDITAR ASIGNATURA";
                btnCrear.Text = "EDITAR ASIGNATURA";
                CargarAsignaturaPorId();
            }
            else
            {
                Text = "Crear Nueva Asignatura";
                guna2HtmlLabel1.Text = "NUEVA ASIGNATURA";
                btnCrear.Text = "CREAR ASIGNATURA";
            }
        }

        #region CARGA

        private void CargarAreas()
        {
            try
            {
                _cargandoCombo = true;

                DataTable dt = new DataTable();
                dt.Columns.Add("Valor", typeof(string));
                dt.Columns.Add("Texto", typeof(string));

                dt.Rows.Add("", "--SELECCIONE--");

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_ListarAreasAsignatura", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string area = dr["Area"]?.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrWhiteSpace(area))
                        dt.Rows.Add(area, area);
                }

                cbActividad.DataSource = dt;
                cbActividad.ValueMember = "Valor";
                cbActividad.DisplayMember = "Texto";
                cbActividad.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar áreas curriculares: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoCombo = false;
            }
        }

        private void CargarAsignaturaPorId()
        {
            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_ObtenerAsignaturaPorId", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AsignaturaID", _asignaturaId);

                cn.Open();

                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtValor.Text = dr["Nombre"]?.ToString() ?? "";
                    txtDescripcion.Text = dr["Descripcion"]?.ToString() ?? "";

                    string area = dr["Area"]?.ToString()?.Trim() ?? "";

                    if (cbActividad.DataSource is DataTable dt)
                    {
                        bool existe = false;

                        foreach (DataRow row in dt.Rows)
                        {
                            if ((row["Valor"]?.ToString() ?? "") == area)
                            {
                                existe = true;
                                break;
                            }
                        }

                        if (!existe && !string.IsNullOrWhiteSpace(area))
                            dt.Rows.Add(area, area);
                    }

                    cbActividad.SelectedValue = area;
                }
                else
                {
                    MessageBox.Show("No se encontró la asignatura seleccionada.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener la asignatura: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region VALIDACION

        private bool ValidarFormulario()
        {
            string nombre = txtValor.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();
            string area = cbActividad.SelectedValue?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Debe ingresar el nombre de la asignatura.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValor.Focus();
                return false;
            }

            if (nombre.Length > 60)
            {
                MessageBox.Show("El nombre no puede exceder los 60 caracteres.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValor.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(area))
            {
                MessageBox.Show("Debe seleccionar el área curricular.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbActividad.Focus();
                return false;
            }

            if (descripcion.Length > 150)
            {
                MessageBox.Show("La descripción no puede exceder los 150 caracteres.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcion.Focus();
                return false;
            }

            return true;
        }

        #endregion

        #region GUARDAR

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario())
                return;

            if (_esEdicion)
                EditarAsignatura();
            else
                CrearAsignatura();
        }

        private void CrearAsignatura()
        {
            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_CrearAsignatura", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", txtValor.Text.Trim());
                cmd.Parameters.AddWithValue("@Area", cbActividad.SelectedValue?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text.Trim());

                cn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Asignatura creada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message,
                    "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la asignatura: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditarAsignatura()
        {
            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_EditarAsignatura", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AsignaturaID", _asignaturaId);
                cmd.Parameters.AddWithValue("@Nombre", txtValor.Text.Trim());
                cmd.Parameters.AddWithValue("@Area", cbActividad.SelectedValue?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text.Trim());

                cn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Asignatura editada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message,
                    "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar la asignatura: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region EVENTOS

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion
    }
}