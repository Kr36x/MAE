using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmDocenteRegistroCalificaciones : Form
    {
        private readonly int docenteId;
        private readonly Conexion conexion = new Conexion();

        private int _cargaIdSeleccionada = 0;
        private int _parcialSeleccionado = 0;

        private bool _cargandoCombos = false;

        private DataTable _dtCargas = new DataTable();
        private DataTable _dtNotasCompleto = new DataTable();
        private DataTable _dtNotasFiltrado = new DataTable();

        private int _paginaActual = 1;
        private int _tamanoPagina = 5;
        private int _totalRegistros = 0;
        private int _totalPaginas = 1;
        private decimal _valorMaximoActividad = 0m;
        private string _textoBusquedaActual = "";
        private TextBox _txtNotaEditando;
        public FrmDocenteRegistroCalificaciones(int docenteId)
        {
            InitializeComponent();
            this.docenteId = docenteId;

            Load += FrmRegistroCalificaciones_Load;
            Resize += FrmRegistroCalificaciones_Resize;

            cbGrado.SelectedIndexChanged += cbGrado_SelectedIndexChanged;
            cbAsignatura.SelectedIndexChanged += cbAsignatura_SelectedIndexChanged;
            cbSeccion.SelectedIndexChanged += cbSeccion_SelectedIndexChanged;
            cbParcial.SelectedIndexChanged += cbParcial_SelectedIndexChanged;
            cbActividad.SelectedIndexChanged += cbActividad_SelectedIndexChanged;

            txtBuscar.TextChanged += txtBuscar_TextChanged;
            btnGuardar.Click += btnGuardar_Click;

            dgvNotas.EditingControlShowing += dgvNotas_EditingControlShowing;
            dgvNotas.CellValidating += dgvNotas_CellValidating;
            dgvNotas.DataError += dgvNotas_DataError;
            dgvNotas.CellPainting += dgvNotas_CellPainting;
        }
        private void dgvNotas_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvNotas.Columns[e.ColumnIndex].Name != "Nota") return;

            e.PaintBackground(e.CellBounds, true);
            e.PaintContent(e.CellBounds);

            Rectangle rect = new Rectangle(
                e.CellBounds.X + 8,
                e.CellBounds.Y + 6,
                e.CellBounds.Width - 16,
                e.CellBounds.Height - 12
            );

            using (Pen pen = new Pen(Color.Silver))
            {
                e.Graphics.DrawRectangle(pen, rect);
            }

            e.Handled = true;
        }
        private void FrmRegistroCalificaciones_Load(object sender, EventArgs e)
        {
            ConfigurarCombos();
            ConfigurarGridNotas();
            InicializarFlujoCombos();
            CargarParciales();
            CargarCicloAcademico();
            CargarCargasDocenteDesdeFront();
            CalcularTamanoPagina();

            lblAnterior.Click += lblAnterior_Click;
            lblSiguiente.Click += lblSiguiente_Click;
            dgvNotas.CellEndEdit += dgvNotas_CellEndEdit;
            txtBuscar.PlaceholderText = "Ingrese el nombre a buscar";
            lblAsignatura.Text = "Asignatura";
            lblRegistros.Text = "Registros: 0";
        }
        private void lblAnterior_Click(object sender, EventArgs e)
        {
            if (_paginaActual > 1)
            {
                GuardarDatosPaginaEnTablaCompleta();
                _paginaActual--;
                MostrarPagina();
            }
        }
        private void lblSiguiente_Click(object sender, EventArgs e)
        {
            if (_paginaActual < _totalPaginas)
            {
                GuardarDatosPaginaEnTablaCompleta();
                _paginaActual++;
                MostrarPagina();
            }
        }
        private void FrmRegistroCalificaciones_Resize(object sender, EventArgs e)
        {
            CalcularTamanoPagina();
            MostrarPagina();
        }

        private void ConfigurarCombos()
        {
            cbAsignatura.DropDownStyle = ComboBoxStyle.DropDownList;
            cbGrado.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSeccion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbParcial.DropDownStyle = ComboBoxStyle.DropDownList;
            cbActividad.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCicloAcademico.DropDownStyle = ComboBoxStyle.DropDownList;

            cbActividad.DropDownWidth = 500;
            cbAsignatura.DropDownWidth = 260;
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

            cbActividad.DataSource = null;
            cbActividad.Items.Clear();
            cbActividad.Items.Add("Elegir");
            cbActividad.SelectedIndex = 0;
            cbActividad.Enabled = false;

            _cargandoCombos = false;
        }

        private void ConfigurarGridNotas()
        {
            dgvNotas.AutoGenerateColumns = false;
            dgvNotas.Columns.Clear();

            dgvNotas.AllowUserToAddRows = false;
            dgvNotas.AllowUserToDeleteRows = false;
            dgvNotas.AllowUserToResizeRows = false;
            dgvNotas.AllowUserToResizeColumns = false;
            dgvNotas.ReadOnly = false;
            dgvNotas.MultiSelect = false;
            dgvNotas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNotas.RowHeadersVisible = false;
            dgvNotas.EnableHeadersVisualStyles = false;
            dgvNotas.BorderStyle = BorderStyle.None;
            dgvNotas.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvNotas.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvNotas.BackgroundColor = Color.FromArgb(245, 245, 245);
            dgvNotas.GridColor = Color.FromArgb(210, 210, 210);
            dgvNotas.Dock = DockStyle.Fill;
            dgvNotas.ScrollBars = ScrollBars.None;

            dgvNotas.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkGreen;
            dgvNotas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvNotas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvNotas.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvNotas.ColumnHeadersHeight = 45;
            dgvNotas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvNotas.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvNotas.DefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 45);
            dgvNotas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 240, 255);
            dgvNotas.DefaultCellStyle.SelectionForeColor = Color.FromArgb(45, 45, 45);
            dgvNotas.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgvNotas.RowsDefaultCellStyle.BackColor = Color.White;
            dgvNotas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dgvNotas.RowTemplate.Height = 46;

            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EstudianteID",
                DataPropertyName = "EstudianteID",
                Visible = false
            });

            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CalificacionID",
                DataPropertyName = "CalificacionID",
                Visible = false
            });

            var colNum = new DataGridViewTextBoxColumn
            {
                Name = "Num",
                HeaderText = "N°",
                DataPropertyName = "Num",
                ReadOnly = true
            };
            colNum.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colEstudiante = new DataGridViewTextBoxColumn
            {
                Name = "Estudiante",
                HeaderText = "ESTUDIANTE",
                DataPropertyName = "Estudiante",
                ReadOnly = true
            };

            var colActividad = new DataGridViewTextBoxColumn
            {
                Name = "Actividad",
                HeaderText = "ACTIVIDAD",
                DataPropertyName = "Actividad",
                ReadOnly = true
            };

            var colNota = new DataGridViewTextBoxColumn
            {
                Name = "Nota",
                HeaderText = "NOTA",
                DataPropertyName = "Nota"
            };

            colNota.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNota.DefaultCellStyle.Format = "N2";
            colNota.DefaultCellStyle.BackColor = Color.White;
            colNota.DefaultCellStyle.ForeColor = Color.Black;
            colNota.DefaultCellStyle.SelectionBackColor = Color.White;
            colNota.DefaultCellStyle.SelectionForeColor = Color.Black;
            colNota.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);

            var colFecha = new DataGridViewTextBoxColumn
            {
                Name = "Fecha",
                HeaderText = "FECHA",
                DataPropertyName = "Fecha",
                ReadOnly = true
            };
            colFecha.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colFecha.DefaultCellStyle.Format = "yyyy-MM-dd";

            dgvNotas.Columns.Add(colNum);
            dgvNotas.Columns.Add(colEstudiante);
            dgvNotas.Columns.Add(colActividad);
            dgvNotas.Columns.Add(colNota);
            dgvNotas.Columns.Add(colFecha);
            dgvNotas.EditMode = DataGridViewEditMode.EditOnEnter;

            dgvNotas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvNotas.Columns["Num"].FillWeight = 10;
            dgvNotas.Columns["Estudiante"].FillWeight = 34;
            dgvNotas.Columns["Actividad"].FillWeight = 34;
            dgvNotas.Columns["Nota"].FillWeight = 12;
            dgvNotas.Columns["Fecha"].FillWeight = 15;

            foreach (DataGridViewColumn col in dgvNotas.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void CalcularTamanoPagina()
        {
            _tamanoPagina = 5;
            //if (dgvNotas.Height <= 0) return;

            //int altoDisponible = dgvNotas.Height - dgvNotas.ColumnHeadersHeight - 8;
            //int altoFila = Math.Max(46, dgvNotas.RowTemplate.Height);

            //int filasVisibles = altoDisponible / altoFila;
            //_tamanoPagina = Math.Max(1, filasVisibles);
        }

        private void CargarParciales()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Nombre", typeof(string));

            dt.Rows.Add(0, "Elegir");
            dt.Rows.Add(1, "I");
            dt.Rows.Add(2, "II");
            dt.Rows.Add(3, "III");
            dt.Rows.Add(4, "IV");

            cbParcial.DataSource = dt;
            cbParcial.ValueMember = "Id";
            cbParcial.DisplayMember = "Nombre";
            cbParcial.SelectedIndex = 0;
        }

        private void CargarCicloAcademico()
        {
            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
            SELECT TOP 1
                CicloEscolar
            FROM Configuracion
            WHERE Activa = 1
            ORDER BY ConfigID DESC;", cn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Agregar columna calculada para el año base
                dt.Columns.Add("AnioBase", typeof(int));

                foreach (DataRow row in dt.Rows)
                {
                    string ciclo = row["CicloEscolar"].ToString(); // "2025-2026"

                    if (!string.IsNullOrEmpty(ciclo) && ciclo.Contains("-"))
                    {
                        string anioInicio = ciclo.Split('-')[0]; // "2025"
                        row["AnioBase"] = int.Parse(anioInicio);
                    }
                    else
                    {
                        row["AnioBase"] = 0;
                    }
                }

                cbCicloAcademico.DataSource = dt;
                cbCicloAcademico.ValueMember = "AnioBase";      // 👉 2025
                cbCicloAcademico.DisplayMember = "CicloEscolar"; // 👉 2025-2026
                cbCicloAcademico.SelectedIndex = dt.Rows.Count > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ciclo académico: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                MessageBox.Show("Error al cargar cargas del docente: " + ex.Message,
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

        private void CargarAsignaturasPorGrado()
        {
            int gradoId = ObtenerValorCombo(cbGrado);

            _cargandoCombos = true;

            cbSeccion.DataSource = null;
            cbSeccion.Items.Clear();
            cbSeccion.Items.Add("Elegir");
            cbSeccion.SelectedIndex = 0;
            cbSeccion.Enabled = false;

            cbActividad.DataSource = null;
            cbActividad.Items.Clear();
            cbActividad.Items.Add("Elegir");
            cbActividad.SelectedIndex = 0;
            cbActividad.Enabled = false;

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

            cbActividad.DataSource = null;
            cbActividad.Items.Clear();
            cbActividad.Items.Add("Elegir");
            cbActividad.SelectedIndex = 0;
            cbActividad.Enabled = false;

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
            CargarActividadesCombo();
        }

        private int ObtenerValorCombo(ComboBox combo)
        {
            if (combo.SelectedValue == null)
                return 0;

            return int.TryParse(combo.SelectedValue.ToString(), out int valor) ? valor : 0;
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
            {
                _cargaIdSeleccionada = Convert.ToInt32(filas[0]["CargaID"]);
            }
        }

        private void CargarActividadesCombo()
        {
            try
            {
                if (_cargaIdSeleccionada <= 0 || _parcialSeleccionado <= 0)
                {
                    cbActividad.DataSource = null;
                    cbActividad.Enabled = false;
                    return;
                }

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_ListarActividadesPorParcial", cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocenteID", docenteId);
                cmd.Parameters.AddWithValue("@CargaID", _cargaIdSeleccionada);
                cmd.Parameters.AddWithValue("@Parcial", _parcialSeleccionado);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (!dt.Columns.Contains("Descripcion") || !dt.Columns.Contains("ActividadID"))
                    throw new Exception("El SP no devolvió las columnas esperadas.");

                dt = AgregarFilaSeleccione(dt, "ActividadID", "Descripcion");

                cbActividad.DataSource = dt;
                cbActividad.ValueMember = "ActividadID";
                cbActividad.DisplayMember = "Descripcion";
                cbActividad.Enabled = true;
                cbActividad.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar actividades: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string ObtenerNombreActividadLimpio(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "";

            if (texto.Contains("-"))
            {
                var partes = texto.Split('-');
                return partes.Length > 1 ? partes[1].Trim() : texto;
            }

            return texto;
        }
        private void CargarRegistroCalificaciones()
        {
            try
            {
                if (_cargaIdSeleccionada <= 0 || _parcialSeleccionado <= 0 || ObtenerValorCombo(cbActividad) <= 0)
                {
                    _dtNotasCompleto = new DataTable();
                    dgvNotas.DataSource = null;
                    _totalRegistros = 0;
                    _totalPaginas = 1;
                    lblRegistros.Text = "Registros: 0";
                    return;
                }

                DataTable dtEstudiantes = ObtenerEstudiantesDelGrupo();
                DataTable dtCalificaciones = ObtenerCalificacionesExistentes();

                //MessageBox.Show(
                //    $"Estudiantes: {dtEstudiantes.Rows.Count}\nCalificaciones: {dtCalificaciones.Rows.Count}",
                //    "Debug");

                _dtNotasCompleto = CrearEstructuraTablaNotas();

                int correlativo = 1;
                string actividadNombre = ObtenerNombreActividadLimpio(cbActividad.Text.Trim());

                foreach (DataRow alumno in dtEstudiantes.Rows)
                {
                    int estudianteId = Convert.ToInt32(alumno["EstudianteID"]);
                    string nombre = alumno["Estudiante"].ToString();

                    DataRow notaExistente = dtCalificaciones.AsEnumerable()
                        .FirstOrDefault(x => Convert.ToInt32(x["EstudianteID"]) == estudianteId);

                    DataRow nueva = _dtNotasCompleto.NewRow();
                    nueva["Num"] = correlativo++;
                    nueva["EstudianteID"] = estudianteId;
                    nueva["Estudiante"] = nombre;
                    nueva["Actividad"] = actividadNombre;

                    if (notaExistente != null)
                    {
                        decimal notaActual = notaExistente["Nota"] == DBNull.Value ? 0m : Convert.ToDecimal(notaExistente["Nota"]);
                        DateTime fechaActual = notaExistente["Fecha"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(notaExistente["Fecha"]);

                        nueva["Nota"] = notaActual;
                        nueva["Fecha"] = fechaActual;
                        nueva["CalificacionID"] = notaExistente["CalificacionID"] == DBNull.Value ? DBNull.Value : notaExistente["CalificacionID"];

                        nueva["NotaOriginal"] = notaActual;
                        nueva["FechaOriginal"] = fechaActual;
                        nueva["FueEditado"] = false;
                    }
                    else
                    {
                        nueva["Nota"] = 0m;
                        nueva["Fecha"] = DateTime.Today;
                        nueva["CalificacionID"] = DBNull.Value;

                        nueva["NotaOriginal"] = 0m;
                        nueva["FechaOriginal"] = DateTime.Today;
                        nueva["FueEditado"] = false;
                    }

                    _dtNotasCompleto.Rows.Add(nueva);
                }

                //MessageBox.Show($"Tabla final: {_dtNotasCompleto.Rows.Count}", "Debug");

                lblAsignatura.Text = $"{cbAsignatura.Text.Trim()}  |   PUNTAJE MÁXIMO: {_valorMaximoActividad:N2}";
                _paginaActual = 1;
                _textoBusquedaActual = txtBuscar.Text.Trim();
                AplicarFiltroBusqueda();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar registro de calificaciones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgvNotas_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvNotas.Columns[e.ColumnIndex].Name != "Nota") return;

            if (dgvNotas.DataSource is not DataTable dtPagina) return;

            DataRow rowPagina = dtPagina.Rows[e.RowIndex];
            int estudianteId = Convert.ToInt32(rowPagina["EstudianteID"]);
            decimal notaNueva = Convert.ToDecimal(rowPagina["Nota"]);

            DataRow rowCompleta = _dtNotasCompleto.AsEnumerable()
                .FirstOrDefault(r => Convert.ToInt32(r["EstudianteID"]) == estudianteId);

            if (rowCompleta != null)
            {
                decimal notaOriginal = Convert.ToDecimal(rowCompleta["NotaOriginal"]);
                rowCompleta["Nota"] = notaNueva;
                rowCompleta["FueEditado"] = (notaNueva != notaOriginal);
            }
        }
        private DataTable ObtenerEstudiantesDelGrupo()
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    E.EstudianteID,
                    E.Nombre AS Estudiante
                FROM Matricula M
                INNER JOIN Estudiante E ON E.EstudianteID = M.EstudianteID
                WHERE M.SeccionID = @SeccionID
                ORDER BY E.Nombre;", cn);

            cmd.Parameters.AddWithValue("@SeccionID", ObtenerValorCombo(cbSeccion));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        private DataTable ObtenerCalificacionesExistentes()
        {
            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_ListarCalificaciones", cn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DocenteID", docenteId);
            cmd.Parameters.AddWithValue("@Anio", ObtenerValorCombo(cbCicloAcademico));
            cmd.Parameters.AddWithValue("@GradoID", ObtenerValorCombo(cbGrado));
            cmd.Parameters.AddWithValue("@SeccionID", ObtenerValorCombo(cbSeccion));
            cmd.Parameters.AddWithValue("@ActividadID", ObtenerValorCombo(cbActividad));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        private DataTable CrearEstructuraTablaNotas()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Num", typeof(int));
            dt.Columns.Add("EstudianteID", typeof(int));
            dt.Columns.Add("CalificacionID", typeof(int));
            dt.Columns.Add("Estudiante", typeof(string));
            dt.Columns.Add("Actividad", typeof(string));
            dt.Columns.Add("Nota", typeof(decimal));
            dt.Columns.Add("Fecha", typeof(DateTime));

            dt.Columns.Add("NotaOriginal", typeof(decimal));
            dt.Columns.Add("FechaOriginal", typeof(DateTime));
            dt.Columns.Add("FueEditado", typeof(bool));

            return dt;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            _textoBusquedaActual = txtBuscar.Text.Trim();
            _paginaActual = 1;
            AplicarFiltroBusqueda();
        }

        private void AplicarFiltroBusqueda()
        {
            try
            {
                if (_dtNotasCompleto == null || _dtNotasCompleto.Rows.Count == 0)
                {
                    _dtNotasFiltrado = CrearEstructuraTablaNotas();
                    _totalRegistros = 0;
                    _totalPaginas = 1;

                    dgvNotas.DataSource = null;
                    lblRegistros.Text = "Sin resultados";
                    return;
                }

                string texto = (_textoBusquedaActual ?? "").Trim();

                if (string.IsNullOrWhiteSpace(texto))
                {
                    _dtNotasFiltrado = _dtNotasCompleto.Copy();
                }
                else
                {
                    _dtNotasFiltrado = _dtNotasCompleto.Clone();

                    var filas = _dtNotasCompleto.AsEnumerable()
                        .Where(r => r["Estudiante"].ToString()
                        .IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0);

                    foreach (var fila in filas)
                    {
                        _dtNotasFiltrado.ImportRow(fila);
                    }
                }

                _totalRegistros = _dtNotasFiltrado.Rows.Count;
                _totalPaginas = _totalRegistros == 0
                    ? 1
                    : (int)Math.Ceiling((double)_totalRegistros / _tamanoPagina);

                _paginaActual = 1;

                MostrarPagina();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al aplicar búsqueda: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //MessageBox.Show(
            //$"_dtNotasCompleto: {_dtNotasCompleto.Rows.Count}\n" +
            //$"_dtNotasFiltrado: {_dtNotasFiltrado.Rows.Count}\n" +
            //$"_totalRegistros: {_totalRegistros}",
            //"Debug filtro");
        }
        private void MostrarPagina()
        {
            try
            {
                DataTable dtBase;

                if (_dtNotasFiltrado != null && _dtNotasFiltrado.Rows.Count > 0)
                    dtBase = _dtNotasFiltrado;
                else if (_dtNotasCompleto != null && _dtNotasCompleto.Rows.Count > 0)
                    dtBase = _dtNotasCompleto;
                else
                    dtBase = CrearEstructuraTablaNotas();

                _totalRegistros = dtBase.Rows.Count;
                _totalPaginas = _totalRegistros == 0
                    ? 1
                    : (int)Math.Ceiling((double)_totalRegistros / _tamanoPagina);

                // por seguridad, si la página actual se pasa, la corriges
                if (_paginaActual > _totalPaginas)
                    _paginaActual = _totalPaginas;

                if (_paginaActual < 1)
                    _paginaActual = 1;

                DataTable dtPagina = dtBase.Clone();

                if (dtBase.Rows.Count > 0)
                {
                    var filasPagina = dtBase.AsEnumerable()
                        .Skip((_paginaActual - 1) * _tamanoPagina)
                        .Take(_tamanoPagina);

                    foreach (var fila in filasPagina)
                    {
                        dtPagina.ImportRow(fila);
                    }
                }

                dgvNotas.SuspendLayout();
                dgvNotas.AutoGenerateColumns = false;
                dgvNotas.DataSource = null;
                dgvNotas.DataSource = dtPagina;
                dgvNotas.ClearSelection();
                dgvNotas.ResumeLayout();

                AjustarAlturaFilas();

                if (dtBase.Rows.Count == 0)
                {
                    lblRegistros.Text = "Sin resultados";
                }
                else
                {
                    int desde = ((_paginaActual - 1) * _tamanoPagina) + 1;
                    int hasta = Math.Min(_paginaActual * _tamanoPagina, dtBase.Rows.Count);
                    lblRegistros.Text = $"Registros del {desde} al {hasta} de {dtBase.Rows.Count} registros";
                }

                btnTexto.Text = $"{_paginaActual} / {_totalPaginas}";

                lblAnterior.Enabled = _paginaActual > 1;
                lblSiguiente.Enabled = _paginaActual < _totalPaginas;

                lblAnterior.ForeColor = lblAnterior.Enabled ? Color.Black : Color.Gray;
                lblSiguiente.ForeColor = lblSiguiente.Enabled ? Color.Black : Color.Gray;

                dgvNotas.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar página: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AjustarAlturaFilas()
        {
            if (dgvNotas.Rows.Count == 0)
                return;

            int cantidadFilasObjetivo = 5;

            int altoDisponible = dgvNotas.ClientSize.Height - dgvNotas.ColumnHeadersHeight - 2;

            int altoFila = altoDisponible / cantidadFilasObjetivo;

            // evitar que queden demasiado pequeñas
            altoFila = Math.Max(36, altoFila);

            foreach (DataGridViewRow row in dgvNotas.Rows)
            {
                row.Height = altoFila;
            }
        }

        private void ActualizarTextoRegistros()
        {
            if (_totalRegistros == 0)
            {
                lblRegistros.Text = "Sin resultados";
                return;
            }

            int desde = ((_paginaActual - 1) * _tamanoPagina) + 1;
            int hasta = Math.Min(_paginaActual * _tamanoPagina, _totalRegistros);

            lblRegistros.Text = $"Registros del {desde} al {hasta} de {_totalRegistros} registros";
        }

        private void cbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;

            CargarAsignaturasPorGrado();

            dgvNotas.DataSource = null;
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
            CargarActividadesCombo();
        }

        private void cbParcial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;

            _parcialSeleccionado = ObtenerValorCombo(cbParcial);
            CargarActividadesCombo();
        }

        private void cbActividad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;

            if (cbActividad.SelectedItem is DataRowView drv)
            {
                if (drv.Row.Table.Columns.Contains("Valor"))
                {
                    _valorMaximoActividad = drv["Valor"] == DBNull.Value
                        ? 0m
                        : Convert.ToDecimal(drv["Valor"]);
                }
                else
                {
                    _valorMaximoActividad = 0m;
                }
            }

            if (dgvNotas.Columns.Contains("Nota"))
            {
                dgvNotas.Columns["Nota"].HeaderText = "NOTA";
            }

            CargarRegistroCalificaciones();
        }

        private void dgvNotas_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvNotas.CurrentCell == null) return;

            if (dgvNotas.CurrentCell.OwningColumn.Name == "Nota" && e.Control is TextBox tb)
            {
                _txtNotaEditando = tb;

                tb.KeyPress -= SoloDecimal_KeyPress;
                tb.KeyPress += SoloDecimal_KeyPress;

                tb.TextChanged -= TbNota_TextChanged;
                tb.TextChanged += TbNota_TextChanged;
            }
        }
        private void TbNota_TextChanged(object sender, EventArgs e)
        {
            if (sender is not TextBox tb) return;

            if (string.IsNullOrWhiteSpace(tb.Text))
                return;

            if (decimal.TryParse(tb.Text, out decimal nota))
            {
                if (_valorMaximoActividad > 0 && nota > _valorMaximoActividad)
                {
                    tb.BackColor = Color.MistyRose;
                }
                else
                {
                    tb.BackColor = Color.White;
                }
            }
            else
            {
                tb.BackColor = Color.MistyRose;
            }
        }
        private void SoloDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            TextBox tb = sender as TextBox;

            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;

            if (e.KeyChar == '.' && tb.Text.Contains("."))
                e.Handled = true;
        }

        private void dgvNotas_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvNotas.Columns[e.ColumnIndex].Name != "Nota")
                return;

            string valor = Convert.ToString(e.FormattedValue)?.Trim();

            if (string.IsNullOrWhiteSpace(valor))
            {
                dgvNotas.Rows[e.RowIndex].Cells["Nota"].Value = 0m;
                return;
            }

            if (!decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal nota) &&
                !decimal.TryParse(valor, out nota))
            {
                MessageBox.Show("Ingrese una nota válida.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            if (nota < 0)
            {
                MessageBox.Show("La nota no puede ser menor que 0.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            if (_valorMaximoActividad > 0 && nota > _valorMaximoActividad)
            {
                MessageBox.Show($"La nota no puede ser mayor que {_valorMaximoActividad:N2}.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }
        }

        private void dgvNotas_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ObtenerValorCombo(cbActividad) <= 0)
                {
                    MessageBox.Show("Seleccione una actividad válida.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                GuardarDatosPaginaEnTablaCompleta();

                using SqlConnection cn = conexion.ObtenerConexion();
                cn.Open();

                var filasEditadas = _dtNotasCompleto.AsEnumerable()
                 .Where(r => r["FueEditado"] != DBNull.Value && Convert.ToBoolean(r["FueEditado"]));

                foreach (DataRow row in filasEditadas)
                {
                    using SqlCommand cmd = new SqlCommand("spMAE_GuardarCalificacion", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EstudianteID", Convert.ToInt32(row["EstudianteID"]));
                    cmd.Parameters.AddWithValue("@ActividadID", ObtenerValorCombo(cbActividad));
                    cmd.Parameters.AddWithValue("@Nota", Convert.ToDecimal(row["Nota"]));
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Today);
                    cmd.Parameters.AddWithValue("@DocenteID", docenteId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Calificaciones guardadas correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarRegistroCalificaciones();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar calificaciones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GuardarDatosPaginaEnTablaCompleta()
        {
            if (dgvNotas.DataSource is not DataTable dtPagina || dtPagina.Rows.Count == 0)
                return;

            foreach (DataRow rowPagina in dtPagina.Rows)
            {
                int estudianteId = Convert.ToInt32(rowPagina["EstudianteID"]);

                DataRow rowCompleta = _dtNotasCompleto.AsEnumerable()
                    .FirstOrDefault(r => Convert.ToInt32(r["EstudianteID"]) == estudianteId);

                if (rowCompleta != null)
                {
                    rowCompleta["Nota"] = rowPagina["Nota"];
                    rowCompleta["Fecha"] = rowPagina["Fecha"];
                    rowCompleta["CalificacionID"] = rowPagina["CalificacionID"];
                }
            }
        }

        //DEBUGG
        private void DebugFiltrosActuales()
        {
            try
            {
                int gradoId = ObtenerValorCombo(cbGrado);
                int asignaturaId = ObtenerValorCombo(cbAsignatura);
                int seccionId = ObtenerValorCombo(cbSeccion);
                int parcial = ObtenerValorCombo(cbParcial);
                int anio = ObtenerValorCombo(cbCicloAcademico);
                int actividadId = ObtenerValorCombo(cbActividad);

                string gradoTxt = cbGrado.Text?.Trim() ?? "";
                string asignaturaTxt = cbAsignatura.Text?.Trim() ?? "";
                string seccionTxt = cbSeccion.Text?.Trim() ?? "";
                string parcialTxt = cbParcial.Text?.Trim() ?? "";
                string cicloTxt = cbCicloAcademico.Text?.Trim() ?? "";
                string actividadTxt = cbActividad.Text?.Trim() ?? "";

                string msg =
                    "===== DEBUG FILTROS =====\n\n" +
                    $"DocenteID: {docenteId}\n" +
                    $"GradoID: {gradoId} | Texto: {gradoTxt}\n" +
                    $"AsignaturaID: {asignaturaId} | Texto: {asignaturaTxt}\n" +
                    $"SeccionID: {seccionId} | Texto: {seccionTxt}\n" +
                    $"Parcial: {parcial} | Texto: {parcialTxt}\n" +
                    $"Año(ValueMember): {anio} | Texto visible: {cicloTxt}\n" +
                    $"CargaID seleccionada: {_cargaIdSeleccionada}\n" +
                    $"ActividadID: {actividadId} | Texto: {actividadTxt}\n" +
                    $"_dtCargas rows: {_dtCargas?.Rows.Count ?? 0}\n";

                MessageBox.Show(msg, "Debug Filtros");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en debug de filtros: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DebugCoincidenciasCarga()
        {
            try
            {
                int gradoId = ObtenerValorCombo(cbGrado);
                int asignaturaId = ObtenerValorCombo(cbAsignatura);
                int seccionId = ObtenerValorCombo(cbSeccion);

                if (_dtCargas == null || _dtCargas.Rows.Count == 0)
                {
                    MessageBox.Show("_dtCargas está vacío.", "Debug Carga");
                    return;
                }

                DataRow[] filas = _dtCargas.Select(
                    $"AsignaturaID = {asignaturaId} AND GradoID = {gradoId} AND SeccionID = {seccionId}");

                if (filas.Length == 0)
                {
                    string detalle = "No se encontró coincidencia en _dtCargas.\n\n";
                    detalle += "Contenido actual:\n";

                    foreach (DataRow row in _dtCargas.Rows)
                    {
                        detalle += $"CargaID={row["CargaID"]}, " +
                                   $"AsignaturaID={row["AsignaturaID"]}, " +
                                   $"GradoID={row["GradoID"]}, " +
                                   $"SeccionID={row["SeccionID"]}\n";
                    }

                    MessageBox.Show(detalle, "Debug Coincidencias Carga");
                    return;
                }

                string msg = "Coincidencias encontradas en _dtCargas:\n\n";
                foreach (DataRow row in filas)
                {
                    msg += $"CargaID={row["CargaID"]}, " +
                           $"AsignaturaID={row["AsignaturaID"]}, " +
                           $"GradoID={row["GradoID"]}, " +
                           $"SeccionID={row["SeccionID"]}\n";
                }

                MessageBox.Show(msg, "Debug Coincidencias Carga");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al depurar carga: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DebugEstudiantesConYSinAnio()
        {
            try
            {
                int seccionId = ObtenerValorCombo(cbSeccion);
                int anio = ObtenerValorCombo(cbCicloAcademico);

                using SqlConnection cn = conexion.ObtenerConexion();
                cn.Open();

                // Con año
                using SqlCommand cmd1 = new SqlCommand(@"
            SELECT COUNT(*)
            FROM Matricula M
            WHERE M.SeccionID = @SeccionID
              AND M.Anio = @Anio;", cn);

                cmd1.Parameters.AddWithValue("@SeccionID", seccionId);
                cmd1.Parameters.AddWithValue("@Anio", anio);

                int conAnio = Convert.ToInt32(cmd1.ExecuteScalar());

                // Sin año
                using SqlCommand cmd2 = new SqlCommand(@"
            SELECT COUNT(*)
            FROM Matricula M
            WHERE M.SeccionID = @SeccionID;", cn);

                cmd2.Parameters.AddWithValue("@SeccionID", seccionId);

                int sinAnio = Convert.ToInt32(cmd2.ExecuteScalar());

                MessageBox.Show(
                    $"SeccionID = {seccionId}\n" +
                    $"Anio = {anio}\n\n" +
                    $"Matrículas con año: {conAnio}\n" +
                    $"Matrículas sin filtrar año: {sinAnio}",
                    "Debug Matrícula");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error debug matrícula: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}