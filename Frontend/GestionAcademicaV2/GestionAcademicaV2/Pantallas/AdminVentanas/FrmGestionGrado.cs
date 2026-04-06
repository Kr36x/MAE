using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmGestionGrado : Form
    {
        private readonly Conexion conexion = new Conexion();

        private bool _cargandoCombos = false;
        private bool _buscando = false;
        private bool _suspendirSelectionChanged = false;

        private readonly System.Windows.Forms.Timer _timerBusqueda = new System.Windows.Forms.Timer();

        private DataTable _dtGradosCompleto = new DataTable();
        private DataTable _dtGradosFiltrado = new DataTable();

        private int _paginaActual = 1;
        private int _tamanoPagina = 10;
        private int _totalRegistros = 0;
        private int _totalPaginas = 1;

        private string _textoBusqueda = "";

        private int _gradoIdSeleccionado = 0;
        private string _nombreGradoSeleccionado = "";
        private string _nivelSeleccionado = "";

        private FrmNuevaSeccion? _frmSeccionesAbierto;
        private FrmNuevoGrado? _frmNuevoGradoAbierto;

        public FrmGestionGrado()
        {
            InitializeComponent();

            Load += FrmGestionGrado_Load;

            btnNuevaActividad.Click += btnNuevaActividad_Click;
            btnBuscar2.Click += btnBuscar2_Click;
            txtBusqueda.TextChanged += txtBusqueda_TextChanged;
            cbNivel.SelectedIndexChanged += cbNivel_SelectedIndexChanged;

            lblAnterior.Click += lblAnterior_Click;
            lblSiguiente.Click += lblSiguiente_Click;

            dgvNotas.CellClick += dgvNotas_CellClick;
            dgvNotas.SelectionChanged += dgvNotas_SelectionChanged;
            dgvNotas.CellPainting += dgvNotas_CellPainting;
            dgvNotas.CellMouseMove += dgvNotas_CellMouseMove;
            dgvNotas.MouseLeave += (s, e) => dgvNotas.Cursor = Cursors.Default;

            _timerBusqueda.Interval = 350;
            _timerBusqueda.Tick += _timerBusqueda_Tick;
        }

        private void FrmGestionGrado_Load(object? sender, EventArgs e)
        {
            ConfigurarCombos();
            CargarNiveles();
            ConfigurarGridGrados();
            AplicarTemaFinalGrid();

            txtBusqueda.PlaceholderText = "Ingrese dato a buscar";
            btnTexto.Text = "1";

            CargarGradosDesdeBD();
            LimpiarPanelDerecho();
        }

        #region CONFIG

        private void ConfigurarCombos()
        {
            _cargandoCombos = true;
            cbNivel.DropDownStyle = ComboBoxStyle.DropDownList;
            _cargandoCombos = false;
        }

        private void CargarNiveles()
        {
            _cargandoCombos = true;

            DataTable dt = new DataTable();
            dt.Columns.Add("Valor", typeof(string));
            dt.Columns.Add("Texto", typeof(string));

            dt.Rows.Add("", "--SELECCIONE--");
            dt.Rows.Add("PRE-BASICA", "PRE-BASICA");
            dt.Rows.Add("BASICA", "BASICA");
            dt.Rows.Add("MEDIA", "MEDIA");

            cbNivel.DataSource = dt;
            cbNivel.ValueMember = "Valor";
            cbNivel.DisplayMember = "Texto";
            cbNivel.SelectedIndex = 0;

            _cargandoCombos = false;
        }

        private void ConfigurarGridGrados()
        {
            dgvNotas.AutoGenerateColumns = false;
            dgvNotas.Columns.Clear();

            dgvNotas.AllowUserToAddRows = false;
            dgvNotas.AllowUserToDeleteRows = false;
            dgvNotas.AllowUserToResizeRows = false;
            dgvNotas.AllowUserToResizeColumns = false;
            dgvNotas.MultiSelect = false;
            dgvNotas.ReadOnly = true;
            dgvNotas.RowHeadersVisible = false;
            dgvNotas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvNotas.EnableHeadersVisualStyles = false;
            dgvNotas.BorderStyle = BorderStyle.None;
            dgvNotas.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvNotas.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvNotas.BackgroundColor = Color.White;
            dgvNotas.GridColor = Color.FromArgb(220, 220, 220);

            dgvNotas.ColumnHeadersHeight = 42;
            dgvNotas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvNotas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 105, 255);
            dgvNotas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvNotas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvNotas.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvNotas.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(24, 105, 255);
            dgvNotas.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgvNotas.DefaultCellStyle.BackColor = Color.White;
            dgvNotas.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgvNotas.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvNotas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 249, 255);
            dgvNotas.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

            dgvNotas.RowsDefaultCellStyle.BackColor = Color.White;
            dgvNotas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dgvNotas.RowTemplate.Height = 40;
            dgvNotas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "No",
                HeaderText = "N°",
                DataPropertyName = "No",
                Width = 45,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GradoID",
                DataPropertyName = "GradoID",
                Visible = false,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Grado",
                HeaderText = "GRADO",
                DataPropertyName = "NombreGrado",
                Width = 170,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            dgvNotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nivel",
                HeaderText = "NIVEL",
                DataPropertyName = "Nivel",
                Width = 110,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            DataGridViewImageColumn colAgregar = new DataGridViewImageColumn
            {
                Name = "Agregar",
                HeaderText = "ACCIONES",
                Image = Properties.Resources.add_white,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 65
            };

            DataGridViewImageColumn colVista = new DataGridViewImageColumn
            {
                Name = "Vista",
                HeaderText = "",
                Image = Properties.Resources.eye,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 55
            };

            dgvNotas.Columns.Add(colAgregar);
            dgvNotas.Columns.Add(colVista);

            dgvNotas.Columns["No"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvNotas.Columns["Agregar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvNotas.Columns["Vista"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void AplicarTemaFinalGrid()
        {
            dgvNotas.EnableHeadersVisualStyles = false;
            dgvNotas.ColumnHeadersHeight = 42;
            dgvNotas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvNotas.RowTemplate.Height = 40;
            dgvNotas.ReadOnly = true;

            dgvNotas.ThemeStyle.HeaderStyle.Height = 42;
            dgvNotas.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvNotas.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(24, 105, 255);
            dgvNotas.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvNotas.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            dgvNotas.ThemeStyle.RowsStyle.Height = 40;
            dgvNotas.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvNotas.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgvNotas.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(245, 249, 255);
            dgvNotas.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

            dgvNotas.ThemeStyle.ReadOnly = true;
            dgvNotas.GridColor = Color.FromArgb(220, 220, 220);
            dgvNotas.ThemeStyle.GridColor = Color.FromArgb(220, 220, 220);

            dgvNotas.BackgroundColor = Color.White;
            dgvNotas.DefaultCellStyle.BackColor = Color.White;
            dgvNotas.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgvNotas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
        }

        private DataTable CrearEstructuraGrados()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(int));
            dt.Columns.Add("GradoID", typeof(int));
            dt.Columns.Add("NombreGrado", typeof(string));
            dt.Columns.Add("Nivel", typeof(string));
            dt.Columns.Add("Estado", typeof(string));
            return dt;
        }

        #endregion

        #region BD

        private DataTable ObtenerGradosDesdeBD()
        {
            DataTable dt = new DataTable();

            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_ListarGrados", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nivel", cbNivel.SelectedValue?.ToString() ?? "");
            cmd.Parameters.AddWithValue("@Busqueda", _textoBusqueda);

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        private void CargarGradosDesdeBD()
        {
            try
            {
                DataTable dtBD = ObtenerGradosDesdeBD();
                _dtGradosCompleto = CrearEstructuraGrados();

                int correlativo = 1;
                foreach (DataRow row in dtBD.Rows)
                {
                    DataRow nueva = _dtGradosCompleto.NewRow();
                    nueva["No"] = correlativo++;
                    nueva["GradoID"] = Convert.ToInt32(row["GradoID"]);
                    nueva["NombreGrado"] = row["NombreGrado"]?.ToString() ?? "";
                    nueva["Nivel"] = row["Nivel"]?.ToString() ?? "";
                    nueva["Estado"] = row.Table.Columns.Contains("Estado")
                        ? row["Estado"]?.ToString() ?? ""
                        : "ACTIVO";

                    _dtGradosCompleto.Rows.Add(nueva);
                }

                AplicarFiltroLocal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar grados: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarFiltroLocal()
        {
            try
            {
                if (_dtGradosCompleto == null || _dtGradosCompleto.Rows.Count == 0)
                {
                    _dtGradosFiltrado = CrearEstructuraGrados();
                    _totalRegistros = 0;
                    _totalPaginas = 1;
                    _paginaActual = 1;
                    MostrarPagina();
                    return;
                }

                string nivel = (cbNivel.SelectedValue?.ToString() ?? "").Trim().ToLowerInvariant();
                string texto = (_textoBusqueda ?? "").Trim().ToLowerInvariant();

                var filas = _dtGradosCompleto.AsEnumerable().Where(r =>
                {
                    string nivelFila = (r["Nivel"]?.ToString() ?? "").Trim().ToLowerInvariant();
                    string gradoFila = (r["NombreGrado"]?.ToString() ?? "").Trim().ToLowerInvariant();

                    bool cumpleNivel = string.IsNullOrWhiteSpace(nivel) || nivelFila == nivel;
                    bool cumpleTexto = string.IsNullOrWhiteSpace(texto) || gradoFila.Contains(texto);

                    return cumpleNivel && cumpleTexto;
                });

                _dtGradosFiltrado = filas.Any() ? filas.CopyToDataTable() : CrearEstructuraGrados();

                _totalRegistros = _dtGradosFiltrado.Rows.Count;
                _totalPaginas = _totalRegistros == 0 ? 1 : (int)Math.Ceiling((double)_totalRegistros / _tamanoPagina);
                _paginaActual = 1;

                MostrarPagina();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar grados: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarPagina()
        {
            if (_dtGradosFiltrado == null)
                return;

            DataTable dtPagina = _dtGradosFiltrado.Clone();

            var filasPagina = _dtGradosFiltrado.AsEnumerable()
                .Skip((_paginaActual - 1) * _tamanoPagina)
                .Take(_tamanoPagina);

            foreach (var fila in filasPagina)
                dtPagina.ImportRow(fila);

            _suspendirSelectionChanged = true;

            try
            {
                dgvNotas.SuspendLayout();

                dgvNotas.DataSource = null;
                dgvNotas.AutoGenerateColumns = false;
                dgvNotas.DataSource = dtPagina;
                dgvNotas.ClearSelection();

                AplicarTemaFinalGrid();
                ActualizarTextoRegistros();
                ActualizarControlesPaginacion();
            }
            finally
            {
                dgvNotas.ResumeLayout();
                dgvNotas.Refresh();
                _suspendirSelectionChanged = false;
            }
        }

        #endregion

        #region PAGINACION

        private void ActualizarTextoRegistros()
        {
            if (_totalRegistros == 0)
            {
                lblRegistros.Text = "Sin resultados";
                return;
            }

            int desde = ((_paginaActual - 1) * _tamanoPagina) + 1;
            int hasta = Math.Min(_paginaActual * _tamanoPagina, _totalRegistros);

            lblRegistros.Text = $"Registros del {desde} al {hasta} total de {_totalRegistros} registros";
        }

        private void ActualizarControlesPaginacion()
        {
            btnTexto.Text = _paginaActual.ToString();

            lblAnterior.Enabled = _paginaActual > 1;
            lblSiguiente.Enabled = _paginaActual < _totalPaginas;

            lblAnterior.ForeColor = lblAnterior.Enabled ? Color.FromArgb(93, 93, 93) : Color.LightGray;
            lblSiguiente.ForeColor = lblSiguiente.Enabled ? Color.FromArgb(93, 93, 93) : Color.LightGray;
        }

        #endregion

        #region PANEL DERECHO

        private void LimpiarPanelDerecho()
        {
            pnlVariado.Controls.Clear();
            pnlVariado.BorderThickness = 1;
            pnlVariado.BorderColor = Color.FromArgb(217, 217, 217);
        }

        private void AbrirFormularioEnPanel(Form frm)
        {
            pnlVariado.Controls.Clear();

            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;

            pnlVariado.Controls.Add(frm);
            pnlVariado.Tag = frm;
            frm.Show();
        }

        private void MostrarNuevoGrado()
        {
            FrmNuevoGrado frm = new FrmNuevoGrado();
            frm.OperacionRealizada += (s, ev) =>
            {
                CargarGradosDesdeBD();
                LimpiarPanelDerecho();
            };
            frm.Cancelado += (s, ev) => LimpiarPanelDerecho();

            AbrirFormularioEnPanel(frm);
        }

        private void FrmNuevoGradoAbierto_GradoCreado(object? sender, EventArgs e)
        {
            CargarGradosDesdeBD();
            LimpiarPanelDerecho();
        }

        private void MostrarSeccionesDeGrado(int gradoId, string nombreGrado, string nivel)
        {
            FrmNuevaSeccion frm = new FrmNuevaSeccion(gradoId, nombreGrado, nivel);
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;

            pnlVariado.Controls.Clear();
            pnlVariado.Controls.Add(frm);
            frm.Show();
        }

        private void FrmSeccionesAbierto_SeccionCreada(object? sender, EventArgs e)
        {
            if (_gradoIdSeleccionado > 0)
                MostrarSeccionesDeGrado(_gradoIdSeleccionado, _nombreGradoSeleccionado, _nivelSeleccionado);
        }

        #endregion

        #region EVENTOS

        private void btnNuevaActividad_Click(object? sender, EventArgs e)
        {
            MostrarNuevoGrado();
        }

        private void txtBusqueda_TextChanged(object? sender, EventArgs e)
        {
            _textoBusqueda = txtBusqueda.Text.Trim();
            _paginaActual = 1;

            _timerBusqueda.Stop();
            _timerBusqueda.Start();
        }

        private void _timerBusqueda_Tick(object? sender, EventArgs e)
        {
            _timerBusqueda.Stop();
            EjecutarBusqueda();
        }

        private void EjecutarBusqueda()
        {
            try
            {
                _buscando = true;

                if (!string.IsNullOrWhiteSpace(_textoBusqueda) && _textoBusqueda.Length < 2)
                    return;

                AplicarFiltroLocal();
            }
            finally
            {
                _buscando = false;
            }
        }

        private void btnBuscar2_Click(object? sender, EventArgs e)
        {
            _timerBusqueda.Stop();
            _textoBusqueda = txtBusqueda.Text.Trim();
            _paginaActual = 1;
            EjecutarBusqueda();
            txtBusqueda.Focus();
        }

        private void cbNivel_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_cargandoCombos) return;
            _paginaActual = 1;
            CargarGradosDesdeBD();
        }

        private void lblAnterior_Click(object? sender, EventArgs e)
        {
            if (_paginaActual > 1)
            {
                _paginaActual--;
                MostrarPagina();
            }
        }

        private void lblSiguiente_Click(object? sender, EventArgs e)
        {
            if (_paginaActual < _totalPaginas)
            {
                _paginaActual++;
                MostrarPagina();
            }
        }

        private void dgvNotas_SelectionChanged(object? sender, EventArgs e)
        {
            if (_suspendirSelectionChanged) return;
            if (_buscando) return;
            if (dgvNotas.CurrentRow == null) return;
            if (dgvNotas.CurrentRow.Index < 0) return;

            SeleccionarGradoDesdeFila(dgvNotas.CurrentRow.Index);
        }

        private void dgvNotas_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            SeleccionarGradoDesdeFila(e.RowIndex);

            string col = dgvNotas.Columns[e.ColumnIndex].Name;

            if (col == "Vista")
            {
                MostrarSeccionesDeGrado(_gradoIdSeleccionado, _nombreGradoSeleccionado, _nivelSeleccionado);
            }
        }

        private void SeleccionarGradoDesdeFila(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dgvNotas.Rows.Count)
                return;

            DataGridViewRow row = dgvNotas.Rows[rowIndex];

            if (row.DataBoundItem is not DataRowView drv)
                return;

            _gradoIdSeleccionado = Convert.ToInt32(drv["GradoID"]);
            _nombreGradoSeleccionado = drv["NombreGrado"]?.ToString() ?? "";
            _nivelSeleccionado = drv["Nivel"]?.ToString() ?? "";
        }

        #endregion

        #region PINTADO GRID

        private void dgvNotas_CellMouseMove(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                dgvNotas.Cursor = Cursors.Default;
                return;
            }

            string nombreColumna = dgvNotas.Columns[e.ColumnIndex].Name;
            dgvNotas.Cursor = (nombreColumna == "Agregar" || nombreColumna == "Vista")
                ? Cursors.Hand
                : Cursors.Default;
        }

        private void dgvNotas_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string nombreColumna = dgvNotas.Columns[e.ColumnIndex].Name;

            if (nombreColumna == "Agregar")
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                int btnSize = 28;
                int startX = e.CellBounds.X + (e.CellBounds.Width - btnSize) / 2;
                int startY = e.CellBounds.Y + (e.CellBounds.Height - btnSize) / 2;

                Rectangle rect = new Rectangle(startX, startY, btnSize, btnSize);

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(86, 196, 255)))
                    g.FillRectangle(brush, rect);

                DibujarIconoCentrado(g, Properties.Resources.add_white, rect, 6);

                using Pen pen = new Pen(dgvNotas.GridColor);
                g.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);

                return;
            }

            if (nombreColumna == "Vista")
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                int btnSize = 28;
                int startX = e.CellBounds.X + (e.CellBounds.Width - btnSize) / 2;
                int startY = e.CellBounds.Y + (e.CellBounds.Height - btnSize) / 2;

                Rectangle rect = new Rectangle(startX, startY, btnSize, btnSize);

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(101, 191, 104)))
                    g.FillRectangle(brush, rect);

                DibujarIconoCentrado(g, Properties.Resources.eye, rect, 6);

                using Pen pen = new Pen(dgvNotas.GridColor);
                g.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);

                return;
            }
        }

        private void DibujarIconoCentrado(Graphics g, Image icono, Rectangle rect, int padding = 6)
        {
            if (icono == null) return;

            Rectangle rectIcono = new Rectangle(
                rect.X + padding,
                rect.Y + padding,
                rect.Width - (padding * 2),
                rect.Height - (padding * 2)
            );

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(icono, rectIcono);
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        #endregion
    }
}