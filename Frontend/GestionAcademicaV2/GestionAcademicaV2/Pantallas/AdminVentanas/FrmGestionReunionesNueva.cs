using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmGestionReunionesNueva : Form
    {
        private readonly Conexion conexion = new Conexion();
        private bool _actualizandoHorario = false;
        private class OpcionCombo
        {
            public int Valor { get; set; }
            public string Texto { get; set; } = "";
        }
        public FrmGestionReunionesNueva()
        {
            InitializeComponent();

            Load += FrmNuevaReunion_Load;
            btnCancelar.Click += btnCancelar_Click;
            btnAgendar.Click += btnAgendar_Click;
        }

        private void FrmNuevaReunion_Load(object sender, EventArgs e)
        {
            ConfigurarControles();
            CargarDocentes();
            CargarMedios();

            CargarHorasBase();
            CargarPeriodosBase();

            SeleccionarPrimerHorarioValido();
            //cbDocente.DrawItem += Combo_DrawItem;
            // cbEstudiante.DrawItem += Combo_DrawItem;
            dtpFechaHora.ValueChanged += dtpFechaHora_ValueChanged;
            cbHora.SelectedIndexChanged += cbHora_SelectedIndexChanged;
            cbDocente.SelectedIndexChanged += cbDocente_SelectedIndexChanged;
            cbDocente.SelectedValue = 0;
        }
        private void cbDocente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDocente.SelectedValue == null)
                return;

            if (int.TryParse(cbDocente.SelectedValue.ToString(), out int docenteID))
            {
                CargarEstudiantesPorDocente(docenteID);
            }
        }
        #region CONFIGURACION

        private void ConfigurarControles()
        {
            cbDocente.DropDownStyle = ComboBoxStyle.DropDown;
            cbEstudiante.DropDownStyle = ComboBoxStyle.DropDown;
            cbMedio.DropDownStyle = ComboBoxStyle.DropDownList;

            cbHora.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMinuto.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPeriodo.DropDownStyle = ComboBoxStyle.DropDownList;

            cbDocente.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbDocente.AutoCompleteSource = AutoCompleteSource.CustomSource;

            cbEstudiante.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbEstudiante.AutoCompleteSource = AutoCompleteSource.CustomSource;

            cbDocente.IntegralHeight = false;
            cbEstudiante.IntegralHeight = false;

            cbDocente.DropDownHeight = 220;
            cbEstudiante.DropDownHeight = 220;

            cbPeriodo.Enabled = false;

            dtpFechaHora.Format = DateTimePickerFormat.Short;
            dtpFechaHora.MinDate = DateTime.Today;
            dtpFechaHora.Value = DateTime.Today;
        }


        private void CargarHorasBase()
        {
            cbHora.Items.Clear();

            cbHora.Items.Add("07");
            cbHora.Items.Add("08");
            cbHora.Items.Add("09");
            cbHora.Items.Add("10");
            cbHora.Items.Add("11");
            cbHora.Items.Add("12");
            cbHora.Items.Add("01");
            cbHora.Items.Add("02");
            cbHora.Items.Add("03");
            cbHora.Items.Add("04");
            cbHora.Items.Add("05");
        }

        private void CargarPeriodosBase()
        {
            cbPeriodo.Items.Clear();
            cbPeriodo.Items.Add("AM");
            cbPeriodo.Items.Add("PM");
        }

        private void CargarMedios()
        {
            cbMedio.Items.Clear();
            cbMedio.Items.Add("--SELECCIONAR--");
            cbMedio.Items.Add("PRESENCIAL");
            cbMedio.Items.Add("LLAMADA");
            cbMedio.Items.Add("VIDEOLLAMADA");
            cbMedio.SelectedIndex = 0;
        }

        #endregion

        #region CARGA DE DATOS

        private void CargarDocentes()
        {
            try
            {
                List<OpcionCombo> lista = new List<OpcionCombo>();

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
            SELECT DocenteID, Nombre
            FROM Docente
            ORDER BY Nombre;", cn);

                cn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new OpcionCombo
                    {
                        Valor = Convert.ToInt32(dr["DocenteID"]),
                        Texto = dr["Nombre"].ToString() ?? ""
                    });
                }

                cbDocente.DataSource = null;
                cbDocente.Items.Clear();

                cbDocente.DisplayMember = "Texto";
                cbDocente.ValueMember = "Valor";
                cbDocente.DataSource = lista;

                AutoCompleteStringCollection autoDocentes = new AutoCompleteStringCollection();
                foreach (var item in lista)
                    autoDocentes.Add(item.Texto);

                cbDocente.AutoCompleteCustomSource = autoDocentes;

                cbDocente.SelectedIndex = lista.Count > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar docentes: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CargarEstudiantesPorDocente(int docenteID)
        {
            try
            {
                List<OpcionCombo> lista = new List<OpcionCombo>();

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
            SELECT DISTINCT E.EstudianteID, E.Nombre
            FROM Estudiante E
            INNER JOIN Matricula M 
                ON E.EstudianteID = M.EstudianteID
            INNER JOIN Seccion S 
                ON M.SeccionID = S.SeccionID
            INNER JOIN CargaAcademica CA 
                ON CA.SeccionID = S.SeccionID
            WHERE CA.DocenteID = @docenteID
            ORDER BY E.Nombre;", cn);

                cmd.Parameters.AddWithValue("@docenteID", docenteID);

                cn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new OpcionCombo
                    {
                        Valor = Convert.ToInt32(dr["EstudianteID"]),
                        Texto = dr["Nombre"].ToString() ?? ""
                    });
                }

                cbEstudiante.DataSource = null;
                cbEstudiante.Items.Clear();

                cbEstudiante.DisplayMember = "Texto";
                cbEstudiante.ValueMember = "Valor";
                cbEstudiante.DataSource = lista;

                AutoCompleteStringCollection autoEstudiantes = new AutoCompleteStringCollection();
                foreach (var item in lista)
                    autoEstudiantes.Add(item.Texto);

                cbEstudiante.AutoCompleteCustomSource = autoEstudiantes;

                cbEstudiante.SelectedIndex = lista.Count > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar estudiantes: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region HORARIO

        private void SeleccionarPrimerHorarioValido()
        {
            _actualizandoHorario = true;

            try
            {
                cbHora.SelectedIndex = -1;
                cbMinuto.Items.Clear();
                cbMinuto.SelectedIndex = -1;
                cbPeriodo.SelectedIndex = -1;

                DateTime fecha = dtpFechaHora.Value.Date;
                DateTime ahora = DateTime.Now;

                DateTime inicio = fecha.AddHours(7);   // 7:00 AM
                DateTime fin = fecha.AddHours(17);     // 5:00 PM

                DateTime? primeraOpcion = null;

                while (inicio <= fin)
                {
                    if (fecha > DateTime.Today || inicio > ahora)
                    {
                        primeraOpcion = inicio;
                        break;
                    }

                    inicio = inicio.AddMinutes(15);
                }

                if (primeraOpcion == null)
                {
                    MessageBox.Show("Ya no hay horarios disponibles para hoy.",
                        "Horario no disponible", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DateTime slot = primeraOpcion.Value;

                string hora12 = slot.ToString("hh");
                string minuto = slot.ToString("mm");

                if (cbHora.Items.Contains(hora12))
                    cbHora.SelectedItem = hora12;

                AutoSeleccionarPeriodoDesdeHora();
                CargarMinutosSegunSeleccion(minuto);
            }
            finally
            {
                _actualizandoHorario = false;
            }
        }

        private void AutoSeleccionarPeriodoDesdeHora()
        {
            if (cbHora.SelectedItem == null)
            {
                cbPeriodo.SelectedIndex = -1;
                return;
            }

            string hora = cbHora.SelectedItem.ToString();

            // En horario escolar:
            // 07-11 => AM
            // 12-05 => PM
            if (hora == "07" || hora == "08" || hora == "09" || hora == "10" || hora == "11")
                cbPeriodo.SelectedItem = "AM";
            else
                cbPeriodo.SelectedItem = "PM";
        }

        private void CargarMinutosSegunSeleccion(string minutoSugerido = null)
        {
            cbMinuto.Items.Clear();

            if (cbHora.SelectedItem == null || cbPeriodo.SelectedItem == null)
                return;

            DateTime fecha = dtpFechaHora.Value.Date;
            DateTime ahora = DateTime.Now;

            int hora24 = ObtenerHora24DesdeSeleccion();
            int[] minutosPermitidos = { 0, 15, 30, 45 };

            foreach (int minuto in minutosPermitidos)
            {
                DateTime opcion = new DateTime(
                    fecha.Year,
                    fecha.Month,
                    fecha.Day,
                    hora24,
                    minuto,
                    0
                );

                if (fecha > DateTime.Today || opcion > ahora)
                    cbMinuto.Items.Add(minuto.ToString("00"));
            }

            if (cbMinuto.Items.Count == 0)
            {
                MoverASiguienteHoraDisponible();
                return;
            }

            if (!string.IsNullOrWhiteSpace(minutoSugerido) && cbMinuto.Items.Contains(minutoSugerido))
                cbMinuto.SelectedItem = minutoSugerido;
            else
                cbMinuto.SelectedIndex = 0;
        }

        private void MoverASiguienteHoraDisponible()
        {
            if (cbHora.SelectedIndex < 0)
                return;

            int indiceActual = cbHora.SelectedIndex;

            for (int i = indiceActual + 1; i < cbHora.Items.Count; i++)
            {
                cbHora.SelectedIndex = i;
                AutoSeleccionarPeriodoDesdeHora();

                cbMinuto.Items.Clear();

                DateTime fecha = dtpFechaHora.Value.Date;
                DateTime ahora = DateTime.Now;
                int hora24 = ObtenerHora24DesdeSeleccion();
                int[] minutosPermitidos = { 0, 15, 30, 45 };

                foreach (int minuto in minutosPermitidos)
                {
                    DateTime opcion = new DateTime(
                        fecha.Year,
                        fecha.Month,
                        fecha.Day,
                        hora24,
                        minuto,
                        0
                    );

                    if (fecha > DateTime.Today || opcion > ahora)
                        cbMinuto.Items.Add(minuto.ToString("00"));
                }

                if (cbMinuto.Items.Count > 0)
                {
                    cbMinuto.SelectedIndex = 0;
                    return;
                }
            }

            cbHora.SelectedIndex = -1;
            cbPeriodo.SelectedIndex = -1;
            cbMinuto.Items.Clear();
            cbMinuto.SelectedIndex = -1;

            MessageBox.Show("Ya no hay horarios disponibles para hoy.",
                "Horario no disponible", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int ObtenerHora24DesdeSeleccion()
        {
            if (cbHora.SelectedItem == null)
                throw new Exception("Seleccione la hora.");

            string horaTexto = cbHora.SelectedItem.ToString();
            string periodo = cbPeriodo.SelectedItem?.ToString() ?? "";

            int hora12 = int.Parse(horaTexto);

            if (periodo == "AM")
            {
                if (hora12 == 12)
                    return 0;

                return hora12;
            }

            // PM
            if (hora12 == 12)
                return 12;

            return hora12 + 12;
        }

        private DateTime ObtenerFechaHoraSeleccionada()
        {
            if (cbHora.SelectedItem == null)
                throw new Exception("Seleccione la hora.");

            if (cbMinuto.SelectedItem == null)
                throw new Exception("Seleccione los minutos.");

            if (cbPeriodo.SelectedItem == null)
                throw new Exception("No se pudo determinar AM o PM.");

            DateTime fecha = dtpFechaHora.Value.Date;
            int hora24 = ObtenerHora24DesdeSeleccion();
            int minuto = int.Parse(cbMinuto.SelectedItem.ToString());

            return new DateTime(fecha.Year, fecha.Month, fecha.Day, hora24, minuto, 0);
        }

        #endregion

        #region VALIDACIONES

        private int ObtenerDocenteId()
        {
            if (cbDocente.SelectedValue == null)
                return 0;

            return int.TryParse(cbDocente.SelectedValue.ToString(), out int id) ? id : 0;
        }

        private int ObtenerEstudianteId()
        {
            if (cbEstudiante.SelectedValue == null)
                return 0;

            return int.TryParse(cbEstudiante.SelectedValue.ToString(), out int id) ? id : 0;
        }
        private bool ValidarSeleccionCombo(ComboBox combo, string nombreCampo)
        {
            if (combo.SelectedIndex < 0 || combo.SelectedValue == null)
            {
                MessageBox.Show($"Seleccione un {nombreCampo} válido de la lista.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                combo.Focus();
                return false;
            }

            return true;
        }
        private bool ValidarFechaHora()
        {
            if (dtpFechaHora.Value.Date < DateTime.Today)
            {
                MessageBox.Show("No se puede seleccionar una fecha anterior a hoy.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DateTime fechaHoraSeleccionada;

            try
            {
                fechaHoraSeleccionada = ObtenerFechaHoraSeleccionada();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (fechaHoraSeleccionada <= DateTime.Now)
            {
                MessageBox.Show("La fecha y hora de la reunión debe ser posterior a la actual.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            TimeSpan horaMinima = new TimeSpan(7, 0, 0);
            TimeSpan horaMaxima = new TimeSpan(17, 0, 0);

            if (fechaHoraSeleccionada.TimeOfDay < horaMinima || fechaHoraSeleccionada.TimeOfDay > horaMaxima)
            {
                MessageBox.Show("La hora debe estar entre 7:00 AM y 5:00 PM.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool ValidarFormulario()
        {
            if (!ValidarSeleccionCombo(cbDocente, "docente"))
                return false;

            if (!ValidarSeleccionCombo(cbEstudiante, "estudiante"))
                return false;

            if (string.IsNullOrWhiteSpace(txtTema.Text))
            {
                MessageBox.Show("Ingrese el tema de la reunión.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cbMedio.SelectedIndex <= 0)
            {
                MessageBox.Show("Seleccione el medio de difusión.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!ValidarFechaHora())
                return false;

            return true;
        }

        #endregion

        #region GUARDAR

        private void GuardarReunion()
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            cn.Open();

            using SqlCommand cmd = new SqlCommand("spMAE_CrearReunion", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@docenteID", ObtenerDocenteId());
            cmd.Parameters.AddWithValue("@estudianteID", ObtenerEstudianteId());
            cmd.Parameters.AddWithValue("@fechaHora", ObtenerFechaHoraSeleccionada());
            cmd.Parameters.AddWithValue("@tema", txtTema.Text.Trim().ToUpper());
            cmd.Parameters.AddWithValue("@medioDifusion", cbMedio.Text.Trim().ToUpper());

            cmd.ExecuteScalar();
        }

        #endregion

        #region EVENTOS

        private void dtpFechaHora_ValueChanged(object sender, EventArgs e)
        {
            if (_actualizandoHorario) return;
            SeleccionarPrimerHorarioValido();
        }

        private void cbHora_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_actualizandoHorario) return;
            if (cbHora.SelectedItem == null) return;

            _actualizandoHorario = true;
            try
            {
                AutoSeleccionarPeriodoDesdeHora();
                CargarMinutosSegunSeleccion();
            }
            finally
            {
                _actualizandoHorario = false;
            }
        }

        private void btnAgendar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario())
                return;

            try
            {
                GuardarReunion();

                MessageBox.Show("Reunión programada correctamente.",
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
                MessageBox.Show("Error al agendar reunión: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion


    }
}