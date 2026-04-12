using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmGestionReunionesCrearActa : Form
    {
        private readonly int reunionId;
        private readonly Conexion conexion = new Conexion();

        public FrmGestionReunionesCrearActa(int reunionId)
        {
            InitializeComponent();
            this.reunionId = reunionId;

            Load += FrmActaReunion_Load;
            btnGuardar.Click += btnGuardar_Click;
            btnCancelar.Click += btnCancelar_Click;
        }

        private void FrmActaReunion_Load(object sender, EventArgs e)
        {
            ConfigurarCampos();
            CargarDetalleReunion();
        }

        private void ConfigurarCampos()
        {
            txtDocente.ReadOnly = true;
            txtEstudiante.ReadOnly = true;
            txtFechayHora.ReadOnly = true;
            txtGrado.ReadOnly = true;
            txtSeccion.ReadOnly = true;
            txtTema.ReadOnly = true;
            txtMedio.ReadOnly = true;

            txtDocente.TabStop = false;
            txtEstudiante.TabStop = false;
            txtFechayHora.TabStop = false;
            txtGrado.TabStop = false;
            txtSeccion.TabStop = false;
            txtTema.TabStop = false;
            txtMedio.TabStop = false;

            txtAcuerdos.ReadOnly = false;
            txtObservaciones.ReadOnly = false;

            txtAcuerdos.Multiline = true;
            txtObservaciones.Multiline = true;
        }

        private void CargarDetalleReunion()
        {
            try
            {
                DataTable dt = new DataTable();

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
            SELECT
                D.Nombre AS Docente,
                E.Nombre AS Estudiante,
                R.FechaHora,
                G.NombreGrado,
                S.Letra,
                R.Tema,
                R.MedioDifusion,
                A.Acuerdos,
                A.Observaciones,
                R.Estado
            FROM Reunion R
            INNER JOIN Docente D
                ON D.DocenteID = R.DocenteID
            INNER JOIN Estudiante E
                ON E.EstudianteID = R.EstudianteID
            INNER JOIN Matricula M
                ON M.EstudianteID = R.EstudianteID
               AND M.Anio = CASE 
                                WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora)
                                ELSE YEAR(R.FechaHora) - 1
                            END
            INNER JOIN Seccion S
                ON S.SeccionID = M.SeccionID
            INNER JOIN Grado G
                ON G.GradoID = S.GradoID
            LEFT JOIN Acta A
                ON A.ReunionID = R.ReunionID
            WHERE R.ReunionID = @ReunionID;", cn);

                cmd.Parameters.AddWithValue("@ReunionID", reunionId);

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontró información de la reunión.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }

                DataRow row = dt.Rows[0];
                DateTime fechaHora = Convert.ToDateTime(row["FechaHora"]);

                txtDocente.Text = row["Docente"]?.ToString() ?? "";
                txtEstudiante.Text = row["Estudiante"]?.ToString() ?? "";
                txtFechayHora.Text = fechaHora.ToString("dd/MM/yyyy hh:mm tt");
                txtGrado.Text = row["NombreGrado"]?.ToString() ?? "";
                txtSeccion.Text = row["Letra"]?.ToString() ?? "";
                txtTema.Text = row["Tema"]?.ToString() ?? "";
                txtMedio.Text = row["MedioDifusion"]?.ToString() ?? "";
                txtAcuerdos.Text = row["Acuerdos"] == DBNull.Value ? "" : row["Acuerdos"].ToString();
                txtObservaciones.Text = row["Observaciones"] == DBNull.Value ? "" : row["Observaciones"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalle de acta: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private bool ValidarAntesGuardar()
        {
            if (string.IsNullOrWhiteSpace(txtAcuerdos.Text))
            {
                MessageBox.Show("Ingrese los acuerdos o conclusiones de la reunión.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtObservaciones.Text))
            {
                MessageBox.Show("Ingrese las observaciones de la reunión.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void GuardarActa()
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("spMAE_CrearActa", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@reunionID", reunionId);
            cmd.Parameters.AddWithValue("@fechaActa", DateTime.Now);
            cmd.Parameters.AddWithValue("@acuerdos", txtAcuerdos.Text.Trim().ToUpper());
            cmd.Parameters.AddWithValue("@observaciones", txtObservaciones.Text.Trim().ToUpper());

            cmd.ExecuteScalar();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarAntesGuardar())
                return;

            try
            {
                GuardarActa();

                MessageBox.Show("Acta guardada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message,
                    "SQL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar acta: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}