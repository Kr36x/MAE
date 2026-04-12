using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmGestionVinculacionTutoresVincularEstudiante : Form
    {
        private readonly Conexion conexion = new Conexion();

        private readonly int _tutorId;
        private readonly string _nombreTutor;
        private readonly string _parentescoTutor;

        private int _estudianteIdSeleccionado = 0;
        private string _nombreEstudianteSeleccionado = "";

        private bool _cargandoFormulario = false;
        private bool _seleccionandoSugerencia = false;

        private readonly ListBox lstSugerencias = new ListBox();
        private List<EstudianteSugerencia> _sugerenciasActuales = new List<EstudianteSugerencia>();

        public FrmGestionVinculacionTutoresVincularEstudiante(int tutorId, string nombreTutor, string parentescoTutor)
        {
            InitializeComponent();

            _tutorId = tutorId;
            _nombreTutor = nombreTutor ?? "";
            _parentescoTutor = parentescoTutor ?? "";

            Load += FrmVincularEstudiante_Load;

            txtAsignatura.TextChanged += txtAsignatura_TextChanged;
            txtAsignatura.KeyDown += txtAsignatura_KeyDown;
            txtAsignatura.Leave += txtAsignatura_Leave;

            cbParentesco.SelectedIndexChanged += cbParentesco_SelectedIndexChanged;

            btnVincular.Click += btnVincular_Click;
            btnCancelar.Click += btnCancelar_Click;
            btnBuscar2.Click += btnBuscar2_Click;

            ConfigurarListaSugerencias();
        }

        #region MODELOS INTERNOS

        private class EstudianteSugerencia
        {
            public int EstudianteID { get; set; }
            public string Nombre { get; set; } = "";
            public string Grado { get; set; } = "";
            public string TextoMostrado => string.IsNullOrWhiteSpace(Grado)
                ? Nombre
                : $"{Nombre} - {Grado}";
        }

        #endregion

        #region LOAD / CONFIG

        private void FrmVincularEstudiante_Load(object sender, EventArgs e)
        {
            try
            {
                _cargandoFormulario = true;

                ConfigurarFormulario();
                CargarParentescos();
                CargarDatosTutor();
                LimpiarSeleccionEstudiante();
            }
            finally
            {
                _cargandoFormulario = false;
            }
        }

        private void ConfigurarFormulario()
        {
            Text = "Vinculación de Estudiante";
            StartPosition = FormStartPosition.CenterParent;

            // Ajustes visuales usando los controles que ya tienes en el diseñador
            guna2HtmlLabel1.Text = "VINCULAR ESTUDIANTE A TUTOR";
            guna2HtmlLabel7.Text = "ESTUDIANTE";
            guna2HtmlLabel5.Text = "INGRESE EL NOMBRE DEL ALUMNO";
            guna2HtmlLabel3.Text = "PARENTESCO";
            guna2HtmlLabel4.Text = "EL PARENTESCO SE HEREDA DEL TUTOR SELECCIONADO";

            txtAsignatura.PlaceholderText = "Ej: Luis Pérez";
            txtAsignatura.MaxLength = 150;

            cbParentesco.DropDownStyle = ComboBoxStyle.DropDownList;
            cbParentesco.Enabled = false;

            btnVincular.Text = "VINCULAR";
            btnCancelar.Text = "CANCELAR";

            btnInfo.Visible = false;

            // El panel/lupa decorativo viejo no sirve para esta lógica
            guna2Panel7.Visible = false;
        }

        private void ConfigurarListaSugerencias()
        {
            lstSugerencias.Visible = false;
            lstSugerencias.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lstSugerencias.BorderStyle = BorderStyle.FixedSingle;
            lstSugerencias.IntegralHeight = false;
            lstSugerencias.Height = 120;
            lstSugerencias.BackColor = Color.White;
            lstSugerencias.ForeColor = Color.FromArgb(35, 35, 35);

            lstSugerencias.Click += lstSugerencias_Click;
            lstSugerencias.DoubleClick += lstSugerencias_DoubleClick;
            lstSugerencias.KeyDown += lstSugerencias_KeyDown;

            Controls.Add(lstSugerencias);
            lstSugerencias.BringToFront();
        }

        private void PosicionarListaSugerencias()
        {
            Point punto = txtAsignatura.Parent.PointToScreen(txtAsignatura.Location);
            punto = this.PointToClient(punto);

            lstSugerencias.Left = punto.X;
            lstSugerencias.Top = punto.Y + txtAsignatura.Height + 2;
            lstSugerencias.Width = txtAsignatura.Width;
        }

        private void CargarParentescos()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Valor", typeof(string));
            dt.Columns.Add("Texto", typeof(string));

            dt.Rows.Add("", "--SELECCIONE--");
            dt.Rows.Add("PADRE", "PADRE");
            dt.Rows.Add("MADRE", "MADRE");
            dt.Rows.Add("ABUELO(A)", "ABUELO(A)");
            dt.Rows.Add("TÍO", "TÍO");
            dt.Rows.Add("TÍA", "TÍA");
            dt.Rows.Add("TUTOR LEGAL", "TUTOR LEGAL");
            dt.Rows.Add("ENCARGADO LEGAL", "ENCARGADO LEGAL");
            dt.Rows.Add("OTRO", "OTRO");

            cbParentesco.DataSource = dt;
            cbParentesco.ValueMember = "Valor";
            cbParentesco.DisplayMember = "Texto";
        }

        private void CargarDatosTutor()
        {
            string parentesco = (_parentescoTutor ?? "").Trim().ToUpper();

            if (cbParentesco.DataSource != null)
            {
                cbParentesco.SelectedValue = parentesco;
                if ((cbParentesco.SelectedValue?.ToString() ?? "") != parentesco)
                {
                    cbParentesco.SelectedIndex = 0;
                }
            }

            // Tooltip opcional con el tutor seleccionado
            ToolTip tt = new ToolTip();
            tt.SetToolTip(txtAsignatura, $"Tutor seleccionado: {_nombreTutor}");
        }

        #endregion

        #region BUSQUEDA DE ESTUDIANTE

        private void txtAsignatura_TextChanged(object sender, EventArgs e)
        {
            if (_cargandoFormulario || _seleccionandoSugerencia)
                return;

            _estudianteIdSeleccionado = 0;
            _nombreEstudianteSeleccionado = "";

            string texto = txtAsignatura.Text.Trim();

            if (texto.Length < 2)
            {
                OcultarSugerencias();
                return;
            }

            BuscarEstudiantes(texto);
        }

        private void txtAsignatura_KeyDown(object sender, KeyEventArgs e)
        {
            if (!lstSugerencias.Visible || lstSugerencias.Items.Count == 0)
                return;

            if (e.KeyCode == Keys.Down)
            {
                if (lstSugerencias.SelectedIndex < lstSugerencias.Items.Count - 1)
                    lstSugerencias.SelectedIndex++;

                lstSugerencias.Focus();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (lstSugerencias.SelectedIndex >= 0)
                {
                    SeleccionarSugerencia(lstSugerencias.SelectedIndex);
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                OcultarSugerencias();
            }
        }

        private void txtAsignatura_Leave(object sender, EventArgs e)
        {
            // pequeña espera para permitir click en lista
            System.Windows.Forms. Timer t = new System.Windows.Forms.Timer();
            t.Interval = 180;
            t.Tick += (s, ev) =>
            {
                t.Stop();
                t.Dispose();

                if (!lstSugerencias.Focused)
                    OcultarSugerencias();
            };
            t.Start();
        }

        private void btnBuscar2_Click(object sender, EventArgs e)
        {
            txtAsignatura.Focus();

            string texto = txtAsignatura.Text.Trim();
            if (texto.Length >= 2)
                BuscarEstudiantes(texto);
        }

        private void BuscarEstudiantes(string texto)
        {
            try
            {
                _sugerenciasActuales = ObtenerSugerenciasEstudiantes(texto);

                lstSugerencias.Items.Clear();

                foreach (var item in _sugerenciasActuales)
                    lstSugerencias.Items.Add(item.TextoMostrado);

                if (lstSugerencias.Items.Count > 0)
                {
                    PosicionarListaSugerencias();
                    lstSugerencias.Visible = true;
                    lstSugerencias.SelectedIndex = 0;
                    lstSugerencias.BringToFront();
                }
                else
                {
                    OcultarSugerencias();
                }
            }
            catch (Exception ex)
            {
                OcultarSugerencias();
                MessageBox.Show("Error al buscar estudiantes: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<EstudianteSugerencia> ObtenerSugerenciasEstudiantes(string busqueda)
        {
            List<EstudianteSugerencia> lista = new List<EstudianteSugerencia>();

            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("sp_MAE_BuscarEstudiantesParaVincular", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Busqueda", busqueda);
            cmd.Parameters.AddWithValue("@TutorID", _tutorId);

            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new EstudianteSugerencia
                {
                    EstudianteID = Convert.ToInt32(dr["EstudianteID"]),
                    Nombre = dr["Nombre"]?.ToString() ?? "",
                    Grado = dr["NombreGrado"]?.ToString() ?? ""
                });
            }

            return lista;
        }

        private void lstSugerencias_Click(object sender, EventArgs e)
        {
            if (lstSugerencias.SelectedIndex >= 0)
                SeleccionarSugerencia(lstSugerencias.SelectedIndex);
        }

        private void lstSugerencias_DoubleClick(object sender, EventArgs e)
        {
            if (lstSugerencias.SelectedIndex >= 0)
                SeleccionarSugerencia(lstSugerencias.SelectedIndex);
        }

        private void lstSugerencias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && lstSugerencias.SelectedIndex >= 0)
            {
                SeleccionarSugerencia(lstSugerencias.SelectedIndex);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                OcultarSugerencias();
                txtAsignatura.Focus();
            }
        }

        private void SeleccionarSugerencia(int index)
        {
            if (index < 0 || index >= _sugerenciasActuales.Count)
                return;

            var estudiante = _sugerenciasActuales[index];

            _seleccionandoSugerencia = true;

            _estudianteIdSeleccionado = estudiante.EstudianteID;
            _nombreEstudianteSeleccionado = estudiante.Nombre;
            txtAsignatura.Text = estudiante.Nombre;

            _seleccionandoSugerencia = false;

            OcultarSugerencias();
            txtAsignatura.SelectionStart = txtAsignatura.TextLength;
            txtAsignatura.Focus();
        }

        private void OcultarSugerencias()
        {
            lstSugerencias.Visible = false;
            lstSugerencias.Items.Clear();
            _sugerenciasActuales.Clear();
        }

        private void LimpiarSeleccionEstudiante()
        {
            _estudianteIdSeleccionado = 0;
            _nombreEstudianteSeleccionado = "";
            txtAsignatura.Text = "";
            OcultarSugerencias();
        }

        #endregion

        #region EVENTOS CONTROLES

        private void cbParentesco_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoFormulario) return;
            // intencionalmente vacío, parentesco queda bloqueado
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnVincular_Click(object sender, EventArgs e)
        {
            VincularEstudiante();
        }

        #endregion

        #region GUARDADO

        private void VincularEstudiante()
        {
            try
            {
                if (_tutorId <= 0)
                {
                    MessageBox.Show("No se encontró un tutor válido para realizar la vinculación.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_estudianteIdSeleccionado <= 0)
                {
                    MessageBox.Show("Debe seleccionar un estudiante de la lista de sugerencias.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAsignatura.Focus();
                    return;
                }

                string parentesco = cbParentesco.SelectedValue?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(parentesco))
                {
                    MessageBox.Show("No se pudo determinar el parentesco del tutor seleccionado.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult r = MessageBox.Show(
                    $"¿Desea vincular a {_nombreEstudianteSeleccionado} con el tutor {_nombreTutor}?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (r != DialogResult.Yes)
                    return;

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("sp_MAE_VincularEstudianteATutor", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@tutorID", _tutorId);
                cmd.Parameters.AddWithValue("@estudianteID", _estudianteIdSeleccionado);

                cn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Estudiante vinculado correctamente.",
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
                MessageBox.Show("Error al vincular estudiante: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}