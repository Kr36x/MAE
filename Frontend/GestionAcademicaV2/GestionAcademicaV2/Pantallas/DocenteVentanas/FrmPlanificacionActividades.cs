using GestionAcademicaV2.Modelos;
using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using static iText.Commons.Utils.PlaceHolderTextUtil;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmPlanifacacionActividades : Form
    {
        private readonly int docenteId;
        private readonly Conexion conexion = new Conexion();

        private int _cargaIdSeleccionada = 0;
        private int _parcialSeleccionado = 0;

        private bool _cargandoCombos = false;

        private DataTable _dtCargas = new DataTable();
        private DataTable _dtActividadesCompleto = new DataTable();

        private int _paginaActual = 1;
        private int _tamanoPagina = 5;
        private int _totalRegistros = 0;
        private int _totalPaginas = 1;

        private DataTable _dtActividadesFiltrado = new DataTable();
        private string _textoBusquedaActual = "";

        public FrmPlanifacacionActividades(int docenteId)
        {
            InitializeComponent();
            this.docenteId = docenteId;

            Load += FrmPlanifacacionActividades_Load;
            Resize += FrmPlanifacacionActividades_Resize;

            cbGrado.SelectedIndexChanged += cbGrado_SelectedIndexChanged;
            cbAsignatura.SelectedIndexChanged += cbAsignatura_SelectedIndexChanged;
            cbSeccion.SelectedIndexChanged += cbSeccion_SelectedIndexChanged;
            cbParcial.SelectedIndexChanged += cbParcial_SelectedIndexChanged;

            btnNuevaActividad.Click += btnNuevaActividad_Click;
            lblAnterior.Click += lblAnterior_Click;
            lblSiguiente.Click += lblSiguiente_Click;

            dgvNotas.CellPainting += dgvNotas_CellPainting;
            dgvNotas.CellClick += dgvNotas_CellClick;
            dgvNotas.DataError += dgvNotas_DataError;
            txtBusqueda.TextChanged += txtBusqueda_TextChanged;
        }
        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            _textoBusquedaActual = txtBusqueda.Text.Trim();
            _paginaActual = 1;
            AplicarFiltroBusqueda();
        }

        private void AplicarFiltroBusqueda()
        {
            try
            {
                if (_dtActividadesCompleto == null || _dtActividadesCompleto.Rows.Count == 0)
                {
                    _dtActividadesFiltrado = new DataTable();
                    _totalRegistros = 0;
                    _totalPaginas = 1;

                    dgvNotas.DataSource = null;
                    lblRegistros.Text = "Registros: 0";
                    ActualizarLblTotal(0m);
                    ActualizarControlesPaginacion();
                    return;
                }

                // Si la búsqueda está vacía, volver a cargar todo
                if (string.IsNullOrWhiteSpace(_textoBusquedaActual))
                {
                    _dtActividadesFiltrado = _dtActividadesCompleto.Copy();
                }
                else
                {
                    string texto = _textoBusquedaActual.Replace("'", "''");

                    DataRow[] filas = _dtActividadesCompleto.Select(
                        $"Descripcion LIKE '%{texto}%'");

                    _dtActividadesFiltrado = _dtActividadesCompleto.Clone();

                    foreach (DataRow fila in filas)
                    {
                        _dtActividadesFiltrado.ImportRow(fila);
                    }
                }

                _totalRegistros = _dtActividadesFiltrado.Rows.Count;
                _totalPaginas = _totalRegistros == 0 ? 1 : (int)Math.Ceiling((double)_totalRegistros / _tamanoPagina);

                decimal totalAcumulado = 0m;
                if (_dtActividadesFiltrado.Rows.Count > 0)
                {
                    totalAcumulado = _dtActividadesFiltrado.AsEnumerable()
                        .Sum(r => r.Field<decimal>("Valor"));
                }

                ActualizarLblTotal(totalAcumulado);
                MostrarPagina();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al aplicar búsqueda: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FrmPlanifacacionActividades_Load(object sender, EventArgs e)
        {
            ConfigurarCombos();
            ConfigurarGridActividades();
            InicializarFlujoCombos();
            CargarParciales();
            CargarCargasDocenteDesdeFront();
            CalcularTamanoPagina();
            ConfigurarLblTotal();

            BeginInvoke(new Action(() =>
            {
                ActualizarLblTotal(0m);
                guna2Panel4.PerformLayout();
                lblTotal.Refresh();
            }));

            txtBusqueda.PlaceholderText = "Ingrese nombre a buscar";
        }
        private void ConfigurarLblTotal()
        {
            lblTotal.AutoSize = false;
            lblTotal.TextAlignment = ContentAlignment.TopLeft;
            lblTotal.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            lblTotal.Left = 12;
            lblTotal.Width = guna2Panel4.ClientSize.Width - 24;
            lblTotal.Height = 42;
            lblTotal.Padding = new Padding(6, 3, 5, 3);
            //lblTotal.BorderStyle = BorderStyle.FixedSingle;
            //lblTotal.BackColor = Color.FromArgb(232, 240, 255);
            lblTotal.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        }
        private void FrmPlanifacacionActividades_Resize(object sender, EventArgs e)
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

        private void ConfigurarGridActividades()
        {
            dgvNotas.AutoGenerateColumns = false;
            dgvNotas.Columns.Clear();

            dgvNotas.AllowUserToAddRows = false;
            dgvNotas.AllowUserToDeleteRows = false;
            dgvNotas.AllowUserToResizeRows = false;
            dgvNotas.AllowUserToResizeColumns = false;
            dgvNotas.ReadOnly = true;
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
            dgvNotas.ScrollBars = ScrollBars.Vertical;

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

            var colActividadId = new DataGridViewTextBoxColumn
            {
                Name = "ActividadID",
                DataPropertyName = "ActividadID",
                Visible = false
            };

            var colNum = new DataGridViewTextBoxColumn
            {
                Name = "Num",
                HeaderText = "N°",
                DataPropertyName = "Num"
            };
            colNum.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colDescripcion = new DataGridViewTextBoxColumn
            {
                Name = "Descripcion",
                HeaderText = "DESCRIPCIÓN DE LA ACTIVIDAD",
                DataPropertyName = "Descripcion"
            };
            colDescripcion.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            colDescripcion.DefaultCellStyle.Padding = new Padding(12, 0, 8, 0);

            var colValor = new DataGridViewTextBoxColumn
            {
                Name = "Valor",
                HeaderText = "VALOR (%)",
                DataPropertyName = "Valor"
            };
            colValor.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colValor.DefaultCellStyle.Format = "N2";
            colValor.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colEditar = new DataGridViewButtonColumn
            {
                Name = "Editar",
                HeaderText = "",
                Text = "",
                UseColumnTextForButtonValue = false
            };
            colEditar.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colEliminar = new DataGridViewButtonColumn
            {
                Name = "Eliminar",
                HeaderText = "",
                Text = "",
                UseColumnTextForButtonValue = false
            };
            colEliminar.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvNotas.Columns.Add(colActividadId);
            dgvNotas.Columns.Add(colNum);
            dgvNotas.Columns.Add(colDescripcion);
            dgvNotas.Columns.Add(colValor);
            dgvNotas.Columns.Add(colEditar);
            dgvNotas.Columns.Add(colEliminar);

            dgvNotas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvNotas.Columns["Num"].FillWeight = 10;
            dgvNotas.Columns["Descripcion"].FillWeight = 58;
            dgvNotas.Columns["Valor"].FillWeight = 14;
            dgvNotas.Columns["Editar"].FillWeight = 9;
            dgvNotas.Columns["Eliminar"].FillWeight = 9;

            foreach (DataGridViewColumn col in dgvNotas.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // importante para que el panel/grid usen mejor el espacio
            //guna2Panel6.Dock = DockStyle.Fill;
            guna2Panel4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void CalcularTamanoPagina()
        {
            if (dgvNotas.Height <= 0) return;

            int altoDisponible = dgvNotas.Height - dgvNotas.ColumnHeadersHeight - 8;
            int altoFila = Math.Max(46, dgvNotas.RowTemplate.Height);

            int filasVisibles = altoDisponible / altoFila;
            _tamanoPagina = Math.Max(1, filasVisibles);
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
                MessageBox.Show("Error al cargar las asignaturas del docente: " + ex.Message,
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
            CargarActividades();
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

        private void CargarActividades()
        {
            try
            {
                if (_cargaIdSeleccionada <= 0 || _parcialSeleccionado <= 0)
                {
                    _dtActividadesCompleto = new DataTable();
                    dgvNotas.DataSource = null;
                    _totalRegistros = 0;
                    _totalPaginas = 1;

                    lblRegistros.Text = "Registros: 0";
                    if (lblTotal != null) lblTotal.Text = "TOTAL ACUMULADO: 0.00%";

                    ActualizarControlesPaginacion();
                    return;
                }

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_ListarActividadesPorParcial", cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocenteID", docenteId);
                cmd.Parameters.AddWithValue("@CargaID", _cargaIdSeleccionada);
                cmd.Parameters.AddWithValue("@Parcial", _parcialSeleccionado);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                _dtActividadesCompleto = new DataTable();
                da.Fill(_dtActividadesCompleto);

                _paginaActual = 1;
                _textoBusquedaActual = txtBusqueda.Text.Trim();
                AplicarFiltroBusqueda();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar actividades: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarLblTotal(decimal totalAcumulado)
        {
            decimal meta = 100m;
            decimal restante = meta - totalAcumulado;

            if (restante < 0)
                restante = 0;

            BeginInvoke(new Action(() =>
            {
                lblTotal.Width = guna2Panel4.ClientSize.Width - 24;
                lblTotal.Left = 12;

                if (totalAcumulado >= 100)
                {
                    lblTotal.Text = $"TOTAL ACUMULADO: {totalAcumulado:N2} de 100.00 · COMPLETADO";
                    //lblTotal.BackColor = Color.FromArgb(223, 240, 216);
                    lblTotal.ForeColor = Color.DarkGreen;
                }
                else
                {
                    lblTotal.Text = $"TOTAL ACUMULADO: {totalAcumulado:N2} de 100.00 · RESTAN {restante:N2} PUNTOS";
                    //lblTotal.BackColor = Color.FromArgb(232, 240, 255);
                    lblTotal.ForeColor = Color.FromArgb(33, 37, 41);
                }

                lblTotal.BringToFront();
                lblTotal.Refresh();
            }));
        }

        private void MostrarPagina()
        {
            if (dgvNotas.Columns.Count == 0)
                return;

            DataTable dtBase = new DataTable();

            if (_dtActividadesFiltrado != null)
                dtBase = _dtActividadesFiltrado;
            else
                dtBase = _dtActividadesCompleto;

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
            dgvNotas.DataSource = null;   // 🔥 importante
            dgvNotas.Rows.Clear();        // 🔥 fuerza limpieza visual
            dgvNotas.DataSource = dtPagina;
            dgvNotas.ClearSelection();
            dgvNotas.ResumeLayout();

            AjustarAlturaFilas();
            ActualizarTextoRegistros();
            ActualizarControlesPaginacion();
            dgvNotas.Refresh();
        }

        private void AjustarAlturaFilas()
        {
            if (dgvNotas.Rows.Count == 0)
            {
                dgvNotas.RowTemplate.Height = 46;
                return;
            }

            int cantidadFilas = dgvNotas.Rows.Count;

            int alturaNormal = 55;

            // 👇 NUEVA CONDICIÓN
            if (cantidadFilas <= 3)
            {
                foreach (DataGridViewRow row in dgvNotas.Rows)
                {
                    row.Height = alturaNormal;
                }
                return;
            }

            // 👇 lógica original (solo cuando hay más de 3 filas)
            int altoDisponible = dgvNotas.ClientSize.Height - dgvNotas.ColumnHeadersHeight - 2;

            int altoCalculado = altoDisponible / cantidadFilas;

            // puedes limitar un poco para que no se vea exagerado
            int altoFinal = Math.Max(alturaNormal, Math.Min(altoCalculado, 65));

            foreach (DataGridViewRow row in dgvNotas.Rows)
            {
                row.Height = altoFinal;
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
        private void btnBuscar2_Click(object sender, EventArgs e)
        {
            txtBusqueda.Focus();
        }
        private void ActualizarControlesPaginacion()
        {
            btnTexto.Text = _paginaActual.ToString();

            lblAnterior.Enabled = _paginaActual > 1;
            lblSiguiente.Enabled = _paginaActual < _totalPaginas;

            lblAnterior.ForeColor = lblAnterior.Enabled ? Color.FromArgb(93, 93, 93) : Color.LightGray;
            lblSiguiente.ForeColor = lblSiguiente.Enabled ? Color.FromArgb(93, 93, 93) : Color.LightGray;
        }

        private void btnNuevaActividad_Click(object sender, EventArgs e)
        {
            if (_cargaIdSeleccionada <= 0)
            {
                MessageBox.Show("Seleccione una carga académica válida antes de crear una actividad.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_parcialSeleccionado <= 0)
            {
                MessageBox.Show("Seleccione un parcial válido.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (FrmNuevaActividad frm = new FrmNuevaActividad(
                        _cargaIdSeleccionada,
                        _parcialSeleccionado,
                        _dtActividadesCompleto))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CargarActividades();
                }
            }
        }

        private void lblAnterior_Click(object sender, EventArgs e)
        {
            if (_paginaActual > 1)
            {
                _paginaActual--;
                MostrarPagina();
            }
        }

        private void lblSiguiente_Click(object sender, EventArgs e)
        {
            if (_paginaActual < _totalPaginas)
            {
                _paginaActual++;
                MostrarPagina();
            }
        }

        private void cbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;

            CargarAsignaturasPorGrado();

            dgvNotas.DataSource = null;
            lblRegistros.Text = "Registros: 0";
            if (lblTotal != null) lblTotal.Text = "TOTAL ACUMULADO: 0.00%";
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
            CargarActividades();
        }

        private void cbParcial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;

            _parcialSeleccionado = ObtenerValorCombo(cbParcial);
            CargarActividades();
        }

        private void dgvNotas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == dgvNotas.Columns["Editar"].Index)
            {
                int actividadId = Convert.ToInt32(dgvNotas.Rows[e.RowIndex].Cells["ActividadID"].Value);
                EditarActividad(actividadId);
            }
            else if (e.ColumnIndex == dgvNotas.Columns["Eliminar"].Index)
            {
                int actividadId = Convert.ToInt32(dgvNotas.Rows[e.RowIndex].Cells["ActividadID"].Value);
                EliminarActividad(actividadId);
            }
        }

        private void dgvNotas_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == dgvNotas.Columns["Editar"].Index ||
                e.ColumnIndex == dgvNotas.Columns["Eliminar"].Index)
            {
                e.PaintBackground(e.CellBounds, true);

                using (Pen pen = new Pen(Color.FromArgb(220, 220, 220)))
                {
                    e.Graphics.DrawRectangle(
                        pen,
                        e.CellBounds.X,
                        e.CellBounds.Y,
                        e.CellBounds.Width - 1,
                        e.CellBounds.Height - 1
                    );
                }

                Color backColor = e.ColumnIndex == dgvNotas.Columns["Editar"].Index
                    ? Color.FromArgb(92, 184, 92)
                    : Color.FromArgb(255, 82, 82);

                Rectangle rect = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + 8,
                    e.CellBounds.Width - 20,
                    e.CellBounds.Height - 16
                );

                using (SolidBrush brush = new SolidBrush(backColor))
                using (GraphicsPath path = RedondearRectangulo(rect, 6))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                }

                string texto = e.ColumnIndex == dgvNotas.Columns["Editar"].Index ? "✎" : "🗑";

                TextRenderer.DrawText(
                    e.Graphics,
                    texto,
                    new Font("Segoe UI Emoji", 11, FontStyle.Bold),
                    rect,
                    Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            }
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

        private void dgvNotas_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void EliminarActividad(int actividadId)
        {
            DialogResult r = MessageBox.Show(
                "¿Desea eliminar esta actividad?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_EliminarActividad", cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActividadID", actividadId);
                cmd.Parameters.AddWithValue("@DocenteID", docenteId);

                cn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Actividad eliminada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarActividades();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar actividad: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditarActividad(int actividadId)
        {
            try
            {
                DataRow[] filas = _dtActividadesCompleto.Select($"ActividadID = {actividadId}");

                if (filas.Length == 0)
                {
                    MessageBox.Show("No se encontró la actividad seleccionada.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow fila = filas[0];

                string descripcionActual = fila["Descripcion"]?.ToString() ?? "";
                decimal valorActual = fila["Valor"] != DBNull.Value
                    ? Convert.ToDecimal(fila["Valor"])
                    : 0m;

                using (FrmEditarActividad frm = new FrmEditarActividad(
                    actividadId,
                    descripcionActual,
                    valorActual,
                    docenteId,
                    _dtActividadesCompleto))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        CargarActividades();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir edición: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}