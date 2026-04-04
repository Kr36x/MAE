using GestionAcademicaV2.Modelos;
using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmRegistroAsistencia : Form
    {
        private readonly int docenteId;
        private readonly Conexion conexion = new Conexion();

        private bool _cargandoCombos = false;

        private DataTable _dtCargas = new DataTable();
        private DataTable _dtAsistenciaCompleto = new DataTable();
        private DataTable _dtAsistenciaFiltrado = new DataTable();

        private int _cargaIdSeleccionada = 0;

        private int _paginaActual = 1;
        private int _tamanoPagina = 5;
        private int _totalRegistros = 0;
        private int _totalPaginas = 1;

        private string _textoBusquedaActual = "";

        public FrmRegistroAsistencia(int docenteId)
        {
            InitializeComponent();
            this.docenteId = docenteId;

            Load += FrmRegistroAsistencia_Load;
            Resize += FrmRegistroAsistencia_Resize;

            cbGrado.SelectedIndexChanged += cbGrado_SelectedIndexChanged;
            cbAsignatura.SelectedIndexChanged += cbAsignatura_SelectedIndexChanged;
            cbSeccion.SelectedIndexChanged += cbSeccion_SelectedIndexChanged;
            cbMostrar.SelectionChangeCommitted += cbMostrar_SelectionChangeCommitted;

            txtBuscar.TextChanged += txtBuscar_TextChanged;
            btnBuscar.Click += btnBuscar2_Click;

            dtpFecha.ValueChanged += dtpFecha_ValueChanged;

            lblAnterior.Click += lblAnterior_Click;
            lblSiguiente.Click += lblSiguiente_Click;
            btnGuardar.Click += btnGuardar_Click;

            dgvAsistencia.CellPainting += dgvAsistencia_CellPainting;
            dgvAsistencia.CellClick += dgvAsistencia_CellClick;
            dgvAsistencia.CellEndEdit += dgvAsistencia_CellEndEdit;
            dgvAsistencia.DataError += dgvAsistencia_DataError;
            dgvAsistencia.CurrentCellDirtyStateChanged += dgvAsistencia_CurrentCellDirtyStateChanged;
        }

        private void FrmRegistroAsistencia_Load(object sender, EventArgs e)
        {
            ConfigurarCombos();
            ConfigurarFecha();
            ConfigurarGrid();
            InicializarFlujoCombos();
            CargarMostrar();
            CargarCargasDocenteDesdeFront();

            txtBuscar.PlaceholderText = "Ingrese el nombre a buscar";
            ActualizarTituloFecha();
            btnTexto.Text = "1";
        }

        private void ActualizarFilaEnTablas(int estudianteId, string estado, string observacion)
        {
            ActualizarFilaEnDataTable(_dtAsistenciaCompleto, estudianteId, estado, observacion);
            ActualizarFilaEnDataTable(_dtAsistenciaFiltrado, estudianteId, estado, observacion);
        }

        private void ActualizarFilaEnDataTable(DataTable dt, int estudianteId, string estado, string observacion)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;

            DataRow[] filas = dt.Select($"EstudianteID = {estudianteId}");
            if (filas.Length == 0)
                return;

            DataRow fila = filas[0];
            fila["EstadoReal"] = estado;
            fila["Observacion"] = observacion ?? "";

            fila["Presente"] = estado == "PRESENTE" ? "●" : "";
            fila["Falta"] = estado == "AUSENTE" ? "X" : "";
            fila["Excusa"] = estado == "JUSTIFICADO" ? "E" : "";
        }

        private void FrmRegistroAsistencia_Resize(object sender, EventArgs e)
        {
            if (_tamanoPagina > 0)
                MostrarPagina();
        }

        private void ConfigurarCombos()
        {
            cbGrado.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAsignatura.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSeccion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMostrar.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void ConfigurarFecha()
        {
            DateTime fechaBase = DateTime.Today;

            if (fechaBase.DayOfWeek == DayOfWeek.Saturday)
                fechaBase = fechaBase.AddDays(2);
            else if (fechaBase.DayOfWeek == DayOfWeek.Sunday)
                fechaBase = fechaBase.AddDays(1);

            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpFecha.Value = fechaBase;
        }

        private void InicializarFlujoCombos()
        {
            _cargandoCombos = true;

            cbAsignatura.DataSource = null;
            cbAsignatura.Items.Clear();
            cbAsignatura.Items.Add("Elegir");
            cbAsignatura.SelectedIndex = 0;
            cbAsignatura.Enabled = false;

            cbSeccion.DataSource = null;
            cbSeccion.Items.Clear();
            cbSeccion.Items.Add("Elegir");
            cbSeccion.SelectedIndex = 0;
            cbSeccion.Enabled = false;

            _cargandoCombos = false;
        }

        private void CargarMostrar()
        {
            _cargandoCombos = true;

            DataTable dt = new DataTable();
            dt.Columns.Add("Valor", typeof(int));
            dt.Columns.Add("Texto", typeof(string));

            dt.Rows.Add(5, "5");
            dt.Rows.Add(10, "10");
            dt.Rows.Add(20, "20");
            dt.Rows.Add(0, "Todos");

            cbMostrar.DataSource = dt;
            cbMostrar.ValueMember = "Valor";
            cbMostrar.DisplayMember = "Texto";
            cbMostrar.SelectedValue = 5;

            _cargandoCombos = false;
        }
        private void GuardarFilaEnMaster(int estudianteId, string estado, string observacion)
        {
            ActualizarFilaEnTablas(estudianteId, estado, observacion);
        }

        private void CargarCargasDocenteDesdeFront()
        {
            try
            {
                _cargandoCombos = true;

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        CA.CargaID,
                        CA.AsignaturaID,
                        A.Nombre AS Asignatura,
                        S.GradoID,
                        G.NombreGrado AS Grado,
                        CA.SeccionID,
                        S.Letra AS Seccion
                    FROM CargaAcademica CA
                    INNER JOIN Asignatura A ON A.AsignaturaID = CA.AsignaturaID
                    INNER JOIN Seccion S ON S.SeccionID = CA.SeccionID
                    INNER JOIN Grado G ON G.GradoID = S.GradoID
                    WHERE CA.DocenteID = @DocenteID
                    ORDER BY G.NombreGrado, S.Letra, A.Nombre;", cn);

                cmd.Parameters.AddWithValue("@DocenteID", docenteId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                _dtCargas = new DataTable();
                da.Fill(_dtCargas);

                LlenarGradosInicial();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las cargas académicas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoCombos = false;
            }
        }

        private DataTable AgregarFilaSeleccione(DataTable dtOriginal, string idCol, string textCol, string texto = "Elegir")
        {
            DataTable dt = dtOriginal.Copy();

            DataRow row = dt.NewRow();
            row[idCol] = 0;
            row[textCol] = texto;
            dt.Rows.InsertAt(row, 0);

            return dt;
        }

        private void LlenarGradosInicial()
        {
            if (_dtCargas == null || _dtCargas.Rows.Count == 0)
                return;

            DataTable dtGrados = _dtCargas.DefaultView.ToTable(true, "GradoID", "Grado");
            dtGrados = AgregarFilaSeleccione(dtGrados, "GradoID", "Grado");

            cbGrado.DataSource = dtGrados;
            cbGrado.ValueMember = "GradoID";
            cbGrado.DisplayMember = "Grado";
            cbGrado.SelectedIndex = 0;
        }

        private int ObtenerValorCombo(ComboBox combo)
        {
            if (combo.SelectedValue == null)
                return 0;

            return int.TryParse(combo.SelectedValue.ToString(), out int valor) ? valor : 0;
        }

        private void CargarAsignaturasPorGrado()
        {
            int gradoId = ObtenerValorCombo(cbGrado);

            _cargandoCombos = true;

            cbSeccion.DataSource = null;
            cbSeccion.Items.Clear();
            cbSeccion.Items.Add("Elegir");
            cbSeccion.SelectedIndex = 0;
            cbSeccion.Enabled = false;

            _cargaIdSeleccionada = 0;

            if (gradoId <= 0)
            {
                cbAsignatura.DataSource = null;
                cbAsignatura.Items.Clear();
                cbAsignatura.Items.Add("Elegir");
                cbAsignatura.SelectedIndex = 0;
                cbAsignatura.Enabled = false;

                _cargandoCombos = false;
                return;
            }

            DataView dv = new DataView(_dtCargas);
            dv.RowFilter = $"GradoID = {gradoId}";

            DataTable dtAsignaturas = dv.ToTable(true, "AsignaturaID", "Asignatura");
            dtAsignaturas = AgregarFilaSeleccione(dtAsignaturas, "AsignaturaID", "Asignatura");

            cbAsignatura.DataSource = dtAsignaturas;
            cbAsignatura.ValueMember = "AsignaturaID";
            cbAsignatura.DisplayMember = "Asignatura";
            cbAsignatura.Enabled = true;
            cbAsignatura.SelectedIndex = 0;

            _cargandoCombos = false;
        }

        private void CargarSeccionesPorGradoYAsignatura()
        {
            int gradoId = ObtenerValorCombo(cbGrado);
            int asignaturaId = ObtenerValorCombo(cbAsignatura);

            _cargandoCombos = true;
            _cargaIdSeleccionada = 0;

            if (gradoId <= 0 || asignaturaId <= 0)
            {
                cbSeccion.DataSource = null;
                cbSeccion.Items.Clear();
                cbSeccion.Items.Add("Elegir");
                cbSeccion.SelectedIndex = 0;
                cbSeccion.Enabled = false;

                _cargandoCombos = false;
                return;
            }

            DataView dv = new DataView(_dtCargas);
            dv.RowFilter = $"GradoID = {gradoId} AND AsignaturaID = {asignaturaId}";

            DataTable dtSecciones = dv.ToTable(true, "SeccionID", "Seccion");

            cbSeccion.Enabled = true;

            if (dtSecciones.Rows.Count == 1)
            {
                cbSeccion.DataSource = dtSecciones;
                cbSeccion.ValueMember = "SeccionID";
                cbSeccion.DisplayMember = "Seccion";
                cbSeccion.SelectedIndex = 0;
            }
            else
            {
                dtSecciones = AgregarFilaSeleccione(dtSecciones, "SeccionID", "Seccion");
                cbSeccion.DataSource = dtSecciones;
                cbSeccion.ValueMember = "SeccionID";
                cbSeccion.DisplayMember = "Seccion";
                cbSeccion.SelectedIndex = 0;
            }

            _cargandoCombos = false;

            ResolverCargaSeleccionada();
            CargarAsistencia();
        }

        private void ResolverCargaSeleccionada()
        {
            _cargaIdSeleccionada = 0;

            int asignaturaId = ObtenerValorCombo(cbAsignatura);
            int gradoId = ObtenerValorCombo(cbGrado);
            int seccionId = ObtenerValorCombo(cbSeccion);

            if (asignaturaId <= 0 || gradoId <= 0 || seccionId <= 0)
                return;

            DataRow[] filas = _dtCargas.Select(
                $"AsignaturaID = {asignaturaId} AND GradoID = {gradoId} AND SeccionID = {seccionId}");

            if (filas.Length > 0)
                _cargaIdSeleccionada = Convert.ToInt32(filas[0]["CargaID"]);
        }

        private void ConfigurarGrid()
        {
            dgvAsistencia.AutoGenerateColumns = false;
            dgvAsistencia.Columns.Clear();

            dgvAsistencia.AllowUserToAddRows = false;
            dgvAsistencia.AllowUserToDeleteRows = false;
            dgvAsistencia.AllowUserToResizeRows = false;
            dgvAsistencia.AllowUserToResizeColumns = false;
            dgvAsistencia.MultiSelect = false;
            dgvAsistencia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAsistencia.RowHeadersVisible = false;
            dgvAsistencia.EnableHeadersVisualStyles = false;
            dgvAsistencia.BorderStyle = BorderStyle.None;
            dgvAsistencia.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvAsistencia.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvAsistencia.BackgroundColor = Color.FromArgb(245, 245, 245);
            dgvAsistencia.GridColor = Color.FromArgb(210, 210, 210);
            dgvAsistencia.Dock = DockStyle.Fill;
            dgvAsistencia.ScrollBars = ScrollBars.Vertical;

            dgvAsistencia.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkGreen;
            dgvAsistencia.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAsistencia.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvAsistencia.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAsistencia.ColumnHeadersHeight = 40;
            dgvAsistencia.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvAsistencia.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvAsistencia.DefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 45);
            dgvAsistencia.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 240, 255);
            dgvAsistencia.DefaultCellStyle.SelectionForeColor = Color.FromArgb(45, 45, 45);
            dgvAsistencia.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgvAsistencia.RowsDefaultCellStyle.BackColor = Color.White;
            dgvAsistencia.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dgvAsistencia.RowTemplate.Height = 48;

            dgvAsistencia.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvAsistencia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Numero",
                HeaderText = "N°",
                DataPropertyName = "Numero",
                ReadOnly = true,
                FillWeight = 8
            });

            dgvAsistencia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EstudianteID",
                DataPropertyName = "EstudianteID",
                Visible = false
            });

            dgvAsistencia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estudiante",
                HeaderText = "ESTUDIANTE",
                DataPropertyName = "Estudiante",
                ReadOnly = true,
                FillWeight = 28
            });

            dgvAsistencia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Presente",
                HeaderText = "PRESENTE",
                DataPropertyName = "Presente",
                ReadOnly = true,
                FillWeight = 13
            });

            dgvAsistencia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Falta",
                HeaderText = "FALTA",
                DataPropertyName = "Falta",
                ReadOnly = true,
                FillWeight = 13
            });

            dgvAsistencia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Excusa",
                HeaderText = "EXCUSA",
                DataPropertyName = "Excusa",
                ReadOnly = true,
                FillWeight = 13
            });

            dgvAsistencia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Observacion",
                HeaderText = "OBSERVACIÓN",
                DataPropertyName = "Observacion",
                ReadOnly = false,
                FillWeight = 25
            });

            dgvAsistencia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EstadoReal",
                DataPropertyName = "EstadoReal",
                Visible = false
            });

            dgvAsistencia.Columns["Numero"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAsistencia.Columns["Presente"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAsistencia.Columns["Falta"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAsistencia.Columns["Excusa"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvAsistencia.Columns["Estudiante"].DefaultCellStyle.Padding = new Padding(10, 0, 6, 0);
            dgvAsistencia.Columns["Observacion"].DefaultCellStyle.Padding = new Padding(10, 0, 6, 0);

            foreach (DataGridViewColumn col in dgvAsistencia.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private DataTable CrearEstructuraAsistencia()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Numero", typeof(int));
            dt.Columns.Add("EstudianteID", typeof(int));
            dt.Columns.Add("Estudiante", typeof(string));
            dt.Columns.Add("Presente", typeof(string));
            dt.Columns.Add("Falta", typeof(string));
            dt.Columns.Add("Excusa", typeof(string));
            dt.Columns.Add("Observacion", typeof(string));
            dt.Columns.Add("EstadoReal", typeof(string));
            return dt;
        }

        private void CargarAsistencia()
        {
            try
            {
                if (_cargaIdSeleccionada <= 0)
                {
                    _dtAsistenciaCompleto = CrearEstructuraAsistencia();
                    _dtAsistenciaFiltrado = CrearEstructuraAsistencia();
                    dgvAsistencia.DataSource = null;
                    _totalRegistros = 0;
                    _totalPaginas = 1;
                    lblRegistros.Text = "Registros: 0";
                    ActualizarControlesPaginacion();
                    return;
                }

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
                    SELECT 
                        E.EstudianteID,
                        E.Nombre AS Estudiante,
                        A.Estado,
                        A.Observacion
                    FROM Estudiante E
                    INNER JOIN Matricula M ON M.EstudianteID = E.EstudianteID
                    INNER JOIN Seccion S ON S.SeccionID = M.SeccionID
                    INNER JOIN CargaAcademica CA ON CA.SeccionID = S.SeccionID
                    LEFT JOIN Asistencia A 
                        ON A.EstudianteID = E.EstudianteID
                       AND A.CargaID = CA.CargaID
                       AND A.Fecha = @Fecha
                    WHERE CA.CargaID = @CargaID
                    ORDER BY E.Nombre;", cn);

                cmd.Parameters.AddWithValue("@CargaID", _cargaIdSeleccionada);
                cmd.Parameters.AddWithValue("@Fecha", dtpFecha.Value.Date);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dtBD = new DataTable();
                da.Fill(dtBD);

                _dtAsistenciaCompleto = CrearEstructuraAsistencia();

                int correlativo = 1;
                foreach (DataRow row in dtBD.Rows)
                {
                    string estado = row["Estado"] == DBNull.Value ? "" : row["Estado"].ToString().Trim().ToUpper();
                    string observacion = row["Observacion"] == DBNull.Value ? "" : row["Observacion"].ToString();

                    DataRow nueva = _dtAsistenciaCompleto.NewRow();
                    nueva["Numero"] = correlativo++;
                    nueva["EstudianteID"] = Convert.ToInt32(row["EstudianteID"]);
                    nueva["Estudiante"] = row["Estudiante"].ToString();
                    nueva["Observacion"] = observacion;
                    nueva["EstadoReal"] = estado;

                    AsignarBotonesVisuales(nueva, estado);

                    _dtAsistenciaCompleto.Rows.Add(nueva);
                }

                _paginaActual = 1;
                _textoBusquedaActual = txtBuscar.Text.Trim();
                AplicarFiltroBusqueda();
                pnFecha.Text = FormatearFechaTitulo(dtpFecha.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar asistencias: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AsignarBotonesVisuales(DataRow fila, string estado)
        {
            fila["Presente"] = estado == "PRESENTE" ? "●" : "";
            fila["Falta"] = estado == "AUSENTE" ? "X" : "";
            fila["Excusa"] = estado == "JUSTIFICADO" ? "E" : "";
        }

        private void AplicarFiltroBusqueda()
        {
            try
            {
                if (_dtAsistenciaCompleto == null || _dtAsistenciaCompleto.Rows.Count == 0)
                {
                    _dtAsistenciaFiltrado = CrearEstructuraAsistencia();
                    _totalRegistros = 0;
                    _totalPaginas = 1;

                    dgvAsistencia.DataSource = null;
                    lblRegistros.Text = "Sin resultados";
                    ActualizarControlesPaginacion();
                    return;
                }

                if (string.IsNullOrWhiteSpace(_textoBusquedaActual))
                {
                    _dtAsistenciaFiltrado = _dtAsistenciaCompleto.Copy();
                }
                else
                {
                    string texto = _textoBusquedaActual.Replace("'", "''");

                    DataRow[] filas = _dtAsistenciaCompleto.Select(
                        $"Estudiante LIKE '%{texto}%'");

                    _dtAsistenciaFiltrado = _dtAsistenciaCompleto.Clone();

                    foreach (DataRow fila in filas)
                        _dtAsistenciaFiltrado.ImportRow(fila);
                }

                _totalRegistros = _dtAsistenciaFiltrado.Rows.Count;

                if (_tamanoPagina == 0)
                    _totalPaginas = 1;
                else
                    _totalPaginas = _totalRegistros == 0 ? 1 :
                        (int)Math.Ceiling((double)_totalRegistros / _tamanoPagina);

                MostrarPagina();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al aplicar búsqueda: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarPagina()
        {
            if (_dtAsistenciaFiltrado == null)
                return;

            DataTable dtPagina = _dtAsistenciaFiltrado.Clone();

            if (_tamanoPagina == 0)
            {
                foreach (DataRow row in _dtAsistenciaFiltrado.Rows)
                    dtPagina.ImportRow(row);

                dgvAsistencia.ScrollBars = ScrollBars.Both;
            }
            else
            {
                var filasPagina = _dtAsistenciaFiltrado.AsEnumerable()
                    .Skip((_paginaActual - 1) * _tamanoPagina)
                    .Take(_tamanoPagina);

                foreach (var fila in filasPagina)
                    dtPagina.ImportRow(fila);

                dgvAsistencia.ScrollBars = ScrollBars.Vertical;
            }

            dgvAsistencia.SuspendLayout();
            dgvAsistencia.DataSource = null;
            dgvAsistencia.Rows.Clear();
            dgvAsistencia.DataSource = dtPagina;
            dgvAsistencia.ClearSelection();
            dgvAsistencia.ResumeLayout();

            AjustarAlturaFilas();
            ActualizarTextoRegistros();
            ActualizarControlesPaginacion();
            dgvAsistencia.Refresh();
        }

        private void AjustarAlturaFilas()
        {
            if (dgvAsistencia.Rows.Count == 0)
            {
                dgvAsistencia.RowTemplate.Height = 48;
                return;
            }

            foreach (DataGridViewRow row in dgvAsistencia.Rows)
                row.Height = 48;
        }

        private void ActualizarTextoRegistros()
        {
            if (_totalRegistros == 0)
            {
                lblRegistros.Text = "Sin resultados";
                return;
            }

            if (_tamanoPagina == 0)
            {
                lblRegistros.Text = $"Registros del 1 al {_totalRegistros} total de {_totalRegistros} registros";
                return;
            }

            int desde = ((_paginaActual - 1) * _tamanoPagina) + 1;
            int hasta = Math.Min(_paginaActual * _tamanoPagina, _totalRegistros);

            lblRegistros.Text = $"Registros del {desde} al {hasta} total de {_totalRegistros} registros";
        }

        private void ActualizarControlesPaginacion()
        {
            btnTexto.Text = _paginaActual.ToString();

            bool mostrarPaginacion = _tamanoPagina != 0;

            lblAnterior.Enabled = mostrarPaginacion && _paginaActual > 1;
            lblSiguiente.Enabled = mostrarPaginacion && _paginaActual < _totalPaginas;

            lblAnterior.Visible = mostrarPaginacion;
            lblSiguiente.Visible = mostrarPaginacion;
            btnTexto.Visible = mostrarPaginacion;

            lblAnterior.ForeColor = lblAnterior.Enabled ? Color.FromArgb(93, 93, 93) : Color.LightGray;
            lblSiguiente.ForeColor = lblSiguiente.Enabled ? Color.FromArgb(93, 93, 93) : Color.LightGray;
        }


        private string FormatearFechaTitulo(DateTime fecha)
        {
            CultureInfo cultura = new CultureInfo("es-HN");
            string texto = fecha.ToString("dddd dd MMMM yyyy", cultura);
            return texto.ToUpper();
        }

        private void AjustarFechaHabil(Guna2DateTimePicker picker)
        {
            DateTime fecha = picker.Value.Date;

            if (fecha.DayOfWeek == DayOfWeek.Saturday)
            {
                picker.Value = fecha.AddDays(2);
                MessageBox.Show("Solo se permiten días hábiles. Se movió la fecha al lunes.",
                    "Fecha no válida", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (fecha.DayOfWeek == DayOfWeek.Sunday)
            {
                picker.Value = fecha.AddDays(1);
                MessageBox.Show("Solo se permiten días hábiles. Se movió la fecha al lunes.",
                    "Fecha no válida", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            _textoBusquedaActual = txtBuscar.Text.Trim();
            _paginaActual = 1;
            AplicarFiltroBusqueda();
        }

        private void btnBuscar2_Click(object sender, EventArgs e)
        {
            txtBuscar.Focus();
        }
        private void cbMostrar_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbMostrar.SelectedValue == null)
                return;

            if (!int.TryParse(cbMostrar.SelectedValue.ToString(), out _tamanoPagina))
                return;

            _paginaActual = 1;
            AplicarFiltroBusqueda();
        }
        private void cbMostrar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;

            if (cbMostrar.SelectedValue == null)
                return;

            if (cbMostrar.SelectedValue is DataRowView)
                return;

            if (!int.TryParse(cbMostrar.SelectedValue.ToString(), out _tamanoPagina))
                return;

            _paginaActual = 1;
            AplicarFiltroBusqueda();
        }

        private void cbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;

            CargarAsignaturasPorGrado();

            dgvAsistencia.DataSource = null;
            lblRegistros.Text = "Registros: 0";
        }

        private void cbAsignatura_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;
            CargarSeccionesPorGradoYAsignatura();
        }

        private void cbSeccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;

            ResolverCargaSeleccionada();
            CargarAsistencia();
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            AjustarFechaHabil(dtpFecha);
            ActualizarTituloFecha();

            if (_cargaIdSeleccionada > 0)
                CargarAsistencia();
        }

        private void lblAnterior_Click(object sender, EventArgs e)
        {
            if (_tamanoPagina == 0) return;

            if (_paginaActual > 1)
            {
                _paginaActual--;
                MostrarPagina();
            }
        }

        private void lblSiguiente_Click(object sender, EventArgs e)
        {
            if (_tamanoPagina == 0) return;

            if (_paginaActual < _totalPaginas)
            {
                _paginaActual++;
                MostrarPagina();
            }
        }

        private void dgvAsistencia_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvAsistencia.IsCurrentCellDirty)
                dgvAsistencia.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvAsistencia_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvAsistencia.Columns[e.ColumnIndex].Name == "Observacion")
            {
                int estudianteId = Convert.ToInt32(dgvAsistencia.Rows[e.RowIndex].Cells["EstudianteID"].Value);
                string estado = dgvAsistencia.Rows[e.RowIndex].Cells["EstadoReal"].Value?.ToString() ?? "";
                string observacion = dgvAsistencia.Rows[e.RowIndex].Cells["Observacion"].Value?.ToString() ?? "";

                GuardarFilaEnMaster(estudianteId, estado, observacion);
            }
        }

        private void dgvAsistencia_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string nombreCol = dgvAsistencia.Columns[e.ColumnIndex].Name;
            if (nombreCol != "Presente" && nombreCol != "Falta" && nombreCol != "Excusa")
                return;

            int estudianteId = Convert.ToInt32(dgvAsistencia.Rows[e.RowIndex].Cells["EstudianteID"].Value);
            string observacion = dgvAsistencia.Rows[e.RowIndex].Cells["Observacion"].Value?.ToString() ?? "";

            string nuevoEstado = "";

            if (nombreCol == "Presente")
                nuevoEstado = "PRESENTE";
            else if (nombreCol == "Falta")
                nuevoEstado = "AUSENTE";
            else if (nombreCol == "Excusa")
                nuevoEstado = "JUSTIFICADO";

            // Actualiza visualmente la fila actual
            dgvAsistencia.Rows[e.RowIndex].Cells["EstadoReal"].Value = nuevoEstado;
            dgvAsistencia.Rows[e.RowIndex].Cells["Presente"].Value = nuevoEstado == "PRESENTE" ? "●" : "";
            dgvAsistencia.Rows[e.RowIndex].Cells["Falta"].Value = nuevoEstado == "AUSENTE" ? "X" : "";
            dgvAsistencia.Rows[e.RowIndex].Cells["Excusa"].Value = nuevoEstado == "JUSTIFICADO" ? "E" : "";

            // Persistir en las tablas fuente
            GuardarFilaEnMaster(estudianteId, nuevoEstado, observacion);

            dgvAsistencia.ClearSelection();
            dgvAsistencia.Refresh();
        }
        private void ActualizarTituloFecha()
        {
            string texto = FormatearFechaTitulo(dtpFecha.Value);

            lblTituloFecha.Text = texto;
            lblTituloFecha.Refresh();
            lblTituloFecha.BringToFront();

            if (lblTituloFecha.Parent != null)
                lblTituloFecha.Parent.Refresh();
        }


        private void dgvAsistencia_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string nombreColumna = dgvAsistencia.Columns[e.ColumnIndex].Name;

            if (nombreColumna != "Presente" && nombreColumna != "Falta" && nombreColumna != "Excusa")
                return;

            e.PaintBackground(e.CellBounds, true);
            e.Paint(e.CellBounds, DataGridViewPaintParts.Border);

            Rectangle rect = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + 9,
                    e.CellBounds.Width - 20,
                    e.CellBounds.Height - 18
                );

            Color borde = Color.FromArgb(205, 205, 205);
            Color textoColor = Color.Gray;
            string texto = "";

            string valor = dgvAsistencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";

            if (nombreColumna == "Presente")
            {
                texto = "●";
                textoColor = valor == "●" ? Color.FromArgb(110, 193, 74) : Color.FromArgb(180, 180, 180);
            }
            else if (nombreColumna == "Falta")
            {
                texto = "X";
                textoColor = valor == "X" ? Color.Red : Color.FromArgb(180, 180, 180);
            }
            else if (nombreColumna == "Excusa")
            {
                texto = "E";
                textoColor = valor == "E" ? Color.Goldenrod : Color.FromArgb(180, 180, 180);
            }

            using (GraphicsPath path = RedondearRectangulo(rect, 5))
            using (Pen pen = new Pen(borde))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(pen, path);
            }

            Font fuente = nombreColumna == "Presente"
                ? new Font("Segoe UI", 12, FontStyle.Bold)
                : new Font("Segoe UI", 10, FontStyle.Bold);

            TextRenderer.DrawText(
                e.Graphics,
                texto,
                fuente,
                rect,
                textoColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );

            e.Handled = true;
        }

        private GraphicsPath RedondearRectangulo(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void dgvAsistencia_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private bool ValidarAntesDeGuardar()
        {
            if (_cargaIdSeleccionada <= 0)
            {
                MessageBox.Show("Seleccione una carga académica válida.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_dtAsistenciaCompleto == null || _dtAsistenciaCompleto.Rows.Count == 0)
            {
                MessageBox.Show("No hay estudiantes para guardar asistencia.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var filasSinEstado = _dtAsistenciaCompleto.AsEnumerable()
                .Where(r => string.IsNullOrWhiteSpace(r["EstadoReal"]?.ToString()))
                .ToList();

            if (filasSinEstado.Count > 0)
            {
                MessageBox.Show("Todos los estudiantes deben tener un estado de asistencia antes de guardar.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarAntesDeGuardar())
                return;

            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                cn.Open();

                foreach (DataRow fila in _dtAsistenciaCompleto.Rows)
                {
                    using SqlCommand cmd = new SqlCommand("sp_Ingresar_Asistencias_v2", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@grado", cbGrado.Text.Trim());
                    cmd.Parameters.AddWithValue("@seccion", cbSeccion.Text.Trim());
                    cmd.Parameters.AddWithValue("@fecha", dtpFecha.Value.Date);
                    cmd.Parameters.AddWithValue("@Asignatura", cbAsignatura.Text.Trim());
                    cmd.Parameters.AddWithValue("@Estado", fila["EstadoReal"].ToString());
                    cmd.Parameters.AddWithValue("@observacion",
                        string.IsNullOrWhiteSpace(fila["Observacion"]?.ToString())
                            ? DBNull.Value
                            : fila["Observacion"].ToString().Trim());
                    cmd.Parameters.AddWithValue("@Estudiante", fila["Estudiante"].ToString());

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Asistencia guardada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarAsistencia();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar asistencia: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}