using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public enum ModoGradoSeccion
    {
        NuevoGradoYSeccion = 1,
        SoloSeccion = 2
    }

    public partial class FrmNuevoGrado : Form
    {
        private readonly Conexion conexion = new Conexion();

        private readonly ModoGradoSeccion _modo;
        private readonly int _gradoId;
        private readonly string _nombreGrado;
        private readonly string _nivel;

        public event EventHandler? OperacionRealizada;
        public event EventHandler? Cancelado;

        // MODO: NUEVO GRADO + SECCION
        public FrmNuevoGrado()
        {
            InitializeComponent();

            _modo = ModoGradoSeccion.NuevoGradoYSeccion;
            _gradoId = 0;
            _nombreGrado = string.Empty;
            _nivel = string.Empty;

            Load += FrmNuevoGrado_Load;
            btnCrear.Click += btnCrear_Click;
            btnCancelar.Click += btnCancelar_Click;
        }

        // MODO: SOLO SECCION
        public FrmNuevoGrado(int gradoId, string nombreGrado, string nivel)
        {
            InitializeComponent();

            _modo = ModoGradoSeccion.SoloSeccion;
            _gradoId = gradoId;
            _nombreGrado = nombreGrado;
            _nivel = nivel;

            Load += FrmNuevoGrado_Load;
            btnCrear.Click += btnCrear_Click;
            btnCancelar.Click += btnCancelar_Click;
        }

        private void FrmNuevoGrado_Load(object? sender, EventArgs e)
        {
            ConfigurarCombos();

            if (_modo == ModoGradoSeccion.NuevoGradoYSeccion)
                PrepararModoNuevoGradoYSeccion();
            else
                PrepararModoSoloSeccion();
        }

        #region CONFIGURACION UI

        private void ConfigurarCombos()
        {
            cbNivelGrado.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSeccion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbTurno.DropDownStyle = ComboBoxStyle.DropDownList;

            cbNivelGrado.Items.Clear();
            cbNivelGrado.Items.Add("--SELECCIONAR--");
            cbNivelGrado.Items.Add("PRE-BASICA");
            cbNivelGrado.Items.Add("BASICA");
            cbNivelGrado.Items.Add("MEDIA");

            cbSeccion.Items.Clear();
            cbSeccion.Items.Add("--SELECCIONAR--");
            cbSeccion.Items.Add("A");
            cbSeccion.Items.Add("B");
            cbSeccion.Items.Add("C");
            cbSeccion.Items.Add("D");

            cbTurno.Items.Clear();
            cbTurno.Items.Add("--SELECCIONAR--");
            cbTurno.Items.Add("MATUTINO");
            cbTurno.Items.Add("VESPERTINO");

            cbNivelGrado.SelectedIndex = 0;
            cbSeccion.SelectedIndex = 0;
            cbTurno.SelectedIndex = 0;
        }

        private void PrepararModoNuevoGradoYSeccion()
        {
            guna2HtmlLabel5.Text = "NUEVO GRADO Y SECCIÓN";
            btnCrear.Text = "CREAR";

            txtGrado.ReadOnly = false;
            txtGrado.Text = "";
            txtGrado.PlaceholderText = "Ingrese nombre del grado";

            cbNivelGrado.Enabled = true;
            cbNivelGrado.SelectedIndex = 0;

            cbSeccion.Enabled = true;
            cbTurno.Enabled = true;

            guna2HtmlLabel15.Text = "CONFIGURACIÓN DEL GRADO";
            guna2HtmlLabel19.Text = "CONFIGURACIÓN DE LA SECCIÓN";
        }

        private void PrepararModoSoloSeccion()
        {
            guna2HtmlLabel5.Text = "NUEVA SECCIÓN";
            btnCrear.Text = "REGISTRAR";

            txtGrado.Text = _nombreGrado;
            txtGrado.ReadOnly = true;

            cbNivelGrado.Items.Clear();
            cbNivelGrado.Items.Add(_nivel);
            cbNivelGrado.SelectedIndex = 0;
            cbNivelGrado.Enabled = false;

            string siguienteLetra = ObtenerSiguienteLetraDesdeBD(_gradoId);

            cbSeccion.Items.Clear();
            cbSeccion.Items.Add(siguienteLetra);
            cbSeccion.SelectedIndex = 0;
            cbSeccion.Enabled = false;

            cbTurno.Enabled = true;
            cbTurno.SelectedItem = ObtenerTurnoSugerido(_gradoId);

            guna2HtmlLabel15.Text = "GRADO SELECCIONADO";
            guna2HtmlLabel19.Text = "CONFIGURACIÓN DE LA SECCIÓN";
        }

        #endregion

        #region EVENTOS

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            Cancelado?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void btnCrear_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_modo == ModoGradoSeccion.NuevoGradoYSeccion)
                    CrearGradoYSeccion();
                else
                    CrearSoloSeccion();

                OperacionRealizada?.Invoke(this, EventArgs.Empty);
                Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error SQL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region LOGICA PRINCIPAL

        private void CrearGradoYSeccion()
        {
            string nombre = txtGrado.Text.Trim();
            string nivel = cbNivelGrado.SelectedIndex > 0 ? cbNivelGrado.Text.Trim() : "";
            string seccion = cbSeccion.SelectedIndex > 0 ? cbSeccion.Text.Trim() : "";
            string turno = cbTurno.SelectedIndex > 0 ? cbTurno.Text.Trim() : "";

            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("Debe ingresar el nombre del grado.");

            if (string.IsNullOrWhiteSpace(nivel))
                throw new Exception("Debe seleccionar el nivel.");

            if (string.IsNullOrWhiteSpace(seccion))
                throw new Exception("Debe seleccionar la sección.");

            if (string.IsNullOrWhiteSpace(turno))
                throw new Exception("Debe seleccionar el turno.");

            int gradoIdCreado = CrearGrado(nombre, nivel);
            CrearSeccion(gradoIdCreado, seccion, turno);

            MessageBox.Show("Grado y sección creados correctamente.",
                "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CrearSoloSeccion()
        {
            string seccion = cbSeccion.Text.Trim();
            string turno = cbTurno.SelectedIndex > 0 ? cbTurno.Text.Trim() : "";

            if (_gradoId <= 0)
                throw new Exception("No se recibió el grado seleccionado.");

            if (string.IsNullOrWhiteSpace(seccion))
                throw new Exception("No se pudo determinar la siguiente sección.");

            if (string.IsNullOrWhiteSpace(turno))
                throw new Exception("Debe seleccionar el turno.");

            CrearSeccion(_gradoId, seccion, turno);

            MessageBox.Show($"Sección {seccion} creada correctamente.",
                "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region BD

        private int CrearGrado(string nombre, string nivel)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_crearGrados", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Nivel", nivel);
            cmd.Parameters.AddWithValue("@Estado", 1);

            cn.Open();
            object? result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                throw new Exception("No se pudo obtener el ID del grado creado.");

            return Convert.ToInt32(result);
        }

        private void CrearSeccion(int gradoId, string letra, string turno)
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_CrearSecciones", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@GradoID", gradoId);
            cmd.Parameters.AddWithValue("@Letra", letra);
            cmd.Parameters.AddWithValue("@Turno", turno);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        private string ObtenerSiguienteLetraDesdeBD(int gradoId)
        {
            DataTable dt = new DataTable();

            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_ListarSeccionesPorGrado", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@GradoID", gradoId);

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var letrasUsadas = dt.AsEnumerable()
                .Select(r => (r["Letra"]?.ToString() ?? "").Trim().ToUpper())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            string[] letrasPermitidas = { "A", "B", "C", "D" };

            foreach (string letra in letrasPermitidas)
            {
                if (!letrasUsadas.Contains(letra))
                    return letra;
            }

            throw new Exception("Este grado ya tiene asignadas todas las secciones permitidas (A, B, C y D).");
        }

        private string ObtenerTurnoSugerido(int gradoId)
        {
            DataTable dt = new DataTable();

            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_ListarSeccionesPorGrado", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@GradoID", gradoId);

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            if (dt.Rows.Count > 0 && dt.Columns.Contains("Turno"))
            {
                string turno = dt.Rows[0]["Turno"]?.ToString()?.Trim().ToUpper() ?? "";
                if (turno == "MATUTINO" || turno == "VESPERTINO")
                    return turno;
            }

            return "--SELECCIONAR--";
        }

        #endregion
    }
}