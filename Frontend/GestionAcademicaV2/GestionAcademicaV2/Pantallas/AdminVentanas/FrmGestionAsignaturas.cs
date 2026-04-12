using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using DrawingColor = System.Drawing.Color;

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmGestionAsignaturas : Form
    {
        private readonly Conexion conexion = new Conexion();

        private bool _cargandoCombos = false;

        private DataTable _dtAsignaturasCompleto = new DataTable();
        private DataTable _dtAsignaturasFiltrado = new DataTable();

        private int _paginaActual = 1;
        private int _tamanoPagina = 5;
        private int _totalRegistros = 0;
        private int _totalPaginas = 1;

        private string _textoBusquedaActual = "";

        public FrmGestionAsignaturas()
        {
            InitializeComponent();

            Load += FrmGestionAsignaturas_Load;

            cbArea.SelectedIndexChanged += cbArea_SelectedIndexChanged;
            cbRegistros.SelectedIndexChanged += cbRegistros_SelectedIndexChanged;

            txtBuscar.TextChanged += txtBuscar_TextChanged;


            lblAnterior.Click += lblAnterior_Click;
            lblSiguiente.Click += lblSiguiente_Click;

            btnNuevaAsignatura.Click += btnNuevaAsignatura_Click;

            dgvAsignaturas.CellPainting += dgvAsignaturas_CellPainting;
            dgvAsignaturas.CellClick += dgvAsignaturas_CellClick;

            dgvAsignaturas.CellMouseMove += dgvAsignaturas_CellMouseMove;
            dgvAsignaturas.MouseLeave += dgvAsignaturas_MouseLeave;
        }

        private void FrmGestionAsignaturas_Load(object sender, EventArgs e)
        {
            ConfigurarCombos();
            ConfigurarGrid();
            CargarAreas();
            CargarRegistrosPorPagina();
            CargarAsignaturas();

            txtBuscar.PlaceholderText = "Ingrese nombre, área o descripción a buscar";
            btnTexto.Text = "1";
        }

        #region CONFIG
        private void dgvAsignaturas_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                dgvAsignaturas.Cursor = Cursors.Default;
                return;
            }

            if (dgvAsignaturas.Columns[e.ColumnIndex].Name != "Acciones")
            {
                dgvAsignaturas.Cursor = Cursors.Default;
                return;
            }

            Rectangle rectCelda = dgvAsignaturas.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            Point mouse = dgvAsignaturas.PointToClient(Cursor.Position);

            int btnSize = 28;
            int separacion = 8;
            int totalWidth = (btnSize * 2) + separacion;
            int startX = rectCelda.X + (rectCelda.Width - totalWidth) / 2;
            int startY = rectCelda.Y + (rectCelda.Height - btnSize) / 2;

            Rectangle rectEditar = new Rectangle(startX, startY, btnSize, btnSize);
            Rectangle rectEliminar = new Rectangle(startX + btnSize + separacion, startY, btnSize, btnSize);

            dgvAsignaturas.Cursor =
                rectEditar.Contains(mouse) || rectEliminar.Contains(mouse)
                ? Cursors.Hand
                : Cursors.Default;
        }

        private void dgvAsignaturas_MouseLeave(object sender, EventArgs e)
        {
            dgvAsignaturas.Cursor = Cursors.Default;
        }
        private void ConfigurarCombos()
        {
            cbArea.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRegistros.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void ConfigurarGrid()
        {
            dgvAsignaturas.AutoGenerateColumns = false;
            dgvAsignaturas.Columns.Clear();

            dgvAsignaturas.AllowUserToAddRows = false;
            dgvAsignaturas.AllowUserToDeleteRows = false;
            dgvAsignaturas.AllowUserToResizeRows = false;
            dgvAsignaturas.AllowUserToResizeColumns = false;
            dgvAsignaturas.MultiSelect = false;
            dgvAsignaturas.ReadOnly = true;
            dgvAsignaturas.RowHeadersVisible = false;
            dgvAsignaturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvAsignaturas.EnableHeadersVisualStyles = false;
            dgvAsignaturas.BorderStyle = BorderStyle.None;
            dgvAsignaturas.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvAsignaturas.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvAsignaturas.BackgroundColor = DrawingColor.White;
            dgvAsignaturas.GridColor = DrawingColor.FromArgb(220, 220, 220);

            dgvAsignaturas.ColumnHeadersHeight = 42;
            dgvAsignaturas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.BackColor = DrawingColor.FromArgb(24, 105, 255);
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.ForeColor = DrawingColor.White;
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.SelectionBackColor = DrawingColor.FromArgb(24, 105, 255);
            dgvAsignaturas.ColumnHeadersDefaultCellStyle.SelectionForeColor = DrawingColor.White;

            dgvAsignaturas.DefaultCellStyle.BackColor = DrawingColor.White;
            dgvAsignaturas.DefaultCellStyle.ForeColor = DrawingColor.FromArgb(35, 35, 35);
            dgvAsignaturas.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            dgvAsignaturas.DefaultCellStyle.SelectionBackColor = DrawingColor.FromArgb(245, 249, 255);
            dgvAsignaturas.DefaultCellStyle.SelectionForeColor = DrawingColor.FromArgb(35, 35, 35);

            dgvAsignaturas.RowsDefaultCellStyle.BackColor = DrawingColor.White;
            dgvAsignaturas.AlternatingRowsDefaultCellStyle.BackColor = DrawingColor.FromArgb(248, 248, 248);
            dgvAsignaturas.RowTemplate.Height = 40;
            dgvAsignaturas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dgvAsignaturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "No",
                HeaderText = "N°",
                DataPropertyName = "No",
                Width = 45
            });

            dgvAsignaturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AsignaturaID",
                DataPropertyName = "AsignaturaID",
                Visible = false
            });

            dgvAsignaturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Asignatura",
                HeaderText = "ASIGNATURA",
                DataPropertyName = "Nombre",
                Width = 190
            });

            dgvAsignaturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Area",
                HeaderText = "ÁREA",
                DataPropertyName = "Area",
                Width = 210
            });

            dgvAsignaturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Descripcion",
                HeaderText = "DESCRIPCIÓN",
                DataPropertyName = "Descripcion",
                Width = 290
            });

            dgvAsignaturas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Acciones",
                HeaderText = "ACCIONES",
                DataPropertyName = "Acciones",
                Width = 115
            });
            dgvAsignaturas.Columns["No"].Width = 45;
            dgvAsignaturas.Columns["Asignatura"].Width = 220;
            dgvAsignaturas.Columns["Area"].Width = 210;
            dgvAsignaturas.Columns["Descripcion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvAsignaturas.Columns["Acciones"].Width = 115;
            dgvAsignaturas.Columns["No"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn col in dgvAsignaturas.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private DataTable CrearEstructuraAsignaturas()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(int));
            dt.Columns.Add("AsignaturaID", typeof(int));
            dt.Columns.Add("Nombre", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("Descripcion", typeof(string));
            dt.Columns.Add("Acciones", typeof(string));
            return dt;
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

        #endregion

        #region COMBOS

        private void CargarAreas()
        {
            try
            {
                _cargandoCombos = true;

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
                    string area = dr["Area"]?.ToString() ?? "";
                    dt.Rows.Add(area, area);
                }

                cbArea.DataSource = dt;
                cbArea.ValueMember = "Valor";
                cbArea.DisplayMember = "Texto";
                cbArea.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar áreas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoCombos = false;
            }
        }

        private void CargarRegistrosPorPagina()
        {
            _cargandoCombos = true;

            DataTable dt = new DataTable();
            dt.Columns.Add("Valor", typeof(int));
            dt.Columns.Add("Texto", typeof(string));

            dt.Rows.Add(5, "5");
            dt.Rows.Add(10, "10");
            dt.Rows.Add(15, "15");
            dt.Rows.Add(20, "20");
            dt.Rows.Add(-1, "OTROS...");

            cbRegistros.DataSource = dt;
            cbRegistros.ValueMember = "Valor";
            cbRegistros.DisplayMember = "Texto";
            cbRegistros.SelectedValue = 5;

            _tamanoPagina = 5;
            _cargandoCombos = false;
        }

        private string ObtenerAreaSeleccionada()
        {
            return cbArea.SelectedValue?.ToString() ?? "";
        }

        #endregion

        #region CARGA

        private DataTable ObtenerAsignaturasDesdeBD()
        {
            DataTable dt = new DataTable();

            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand("spMAE_ListarAsignaturas", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Area", ObtenerAreaSeleccionada());
            cmd.Parameters.AddWithValue("@Busqueda", _textoBusquedaActual);

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        private void CargarAsignaturas()
        {
            try
            {
                DataTable dtBD = ObtenerAsignaturasDesdeBD();
                _dtAsignaturasCompleto = CrearEstructuraAsignaturas();

                int correlativo = 1;

                foreach (DataRow row in dtBD.Rows)
                {
                    DataRow nueva = _dtAsignaturasCompleto.NewRow();
                    nueva["No"] = correlativo++;
                    nueva["AsignaturaID"] = Convert.ToInt32(row["AsignaturaID"]);
                    nueva["Nombre"] = row["Nombre"]?.ToString() ?? "";
                    nueva["Area"] = row["Area"]?.ToString() ?? "";
                    nueva["Descripcion"] = row["Descripcion"]?.ToString() ?? "";
                    nueva["Acciones"] = "EDITAR|ELIMINAR";

                    _dtAsignaturasCompleto.Rows.Add(nueva);
                }

                _dtAsignaturasFiltrado = _dtAsignaturasCompleto.Copy();
                _totalRegistros = _dtAsignaturasFiltrado.Rows.Count;
                _totalPaginas = _totalRegistros == 0
                    ? 1
                    : (int)Math.Ceiling((double)_totalRegistros / _tamanoPagina);

                _paginaActual = 1;
                MostrarPagina();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar asignaturas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region BUSQUEDA Y PAGINACION

        private void AplicarFiltroBusquedaLocal()
        {
            try
            {
                if (_dtAsignaturasCompleto == null || _dtAsignaturasCompleto.Rows.Count == 0)
                {
                    _dtAsignaturasFiltrado = CrearEstructuraAsignaturas();
                    _totalRegistros = 0;
                    _totalPaginas = 1;
                    dgvAsignaturas.DataSource = null;
                    lblRegistros.Text = "Sin resultados";
                    ActualizarControlesPaginacion();
                    return;
                }

                if (string.IsNullOrWhiteSpace(_textoBusquedaActual))
                {
                    _dtAsignaturasFiltrado = _dtAsignaturasCompleto.Copy();
                }
                else
                {
                    string texto = _textoBusquedaActual.Trim().ToLower();

                    var filas = _dtAsignaturasCompleto.AsEnumerable()
                        .Where(r =>
                            (r["Nombre"]?.ToString() ?? "").ToLower().Contains(texto) ||
                            (r["Area"]?.ToString() ?? "").ToLower().Contains(texto) ||
                            (r["Descripcion"]?.ToString() ?? "").ToLower().Contains(texto));

                    _dtAsignaturasFiltrado = filas.Any()
                        ? filas.CopyToDataTable()
                        : CrearEstructuraAsignaturas();
                }

                _totalRegistros = _dtAsignaturasFiltrado.Rows.Count;
                _totalPaginas = _totalRegistros == 0
                    ? 1
                    : (int)Math.Ceiling((double)_totalRegistros / _tamanoPagina);

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
            if (_dtAsignaturasFiltrado == null)
                return;

            DataTable dtPagina = _dtAsignaturasFiltrado.Clone();

            var filasPagina = _dtAsignaturasFiltrado.AsEnumerable()
                .Skip((_paginaActual - 1) * _tamanoPagina)
                .Take(_tamanoPagina);

            foreach (var fila in filasPagina)
                dtPagina.ImportRow(fila);

            dgvAsignaturas.SuspendLayout();
            dgvAsignaturas.DataSource = null;
            dgvAsignaturas.Rows.Clear();
            dgvAsignaturas.DataSource = dtPagina;
            dgvAsignaturas.ClearSelection();
            dgvAsignaturas.ResumeLayout();

            ActualizarTextoRegistros();
            ActualizarControlesPaginacion();
            dgvAsignaturas.Refresh();
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

        #region EVENTOS

        private void cbArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;
            _paginaActual = 1;
            CargarAsignaturas();
        }

        private void cbRegistros_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;

            if (cbRegistros.SelectedValue == null)
                return;

            if (!int.TryParse(cbRegistros.SelectedValue.ToString(), out int valor))
                return;

            if (valor == -1)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox(
                    "Ingrese la cantidad de registros por página:",
                    "Cantidad personalizada",
                    _tamanoPagina.ToString());

                if (int.TryParse(input, out int cantidad) && cantidad > 0)
                {
                    _tamanoPagina = cantidad;
                }
                else
                {
                    cbRegistros.SelectedValue = 5;
                    _tamanoPagina = 5;
                }
            }
            else
            {
                _tamanoPagina = valor;
            }

            _paginaActual = 1;
            MostrarPagina();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            _textoBusquedaActual = txtBuscar.Text.Trim();
            _paginaActual = 1;
            CargarAsignaturas(); // búsqueda tiempo real desde BD
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            txtBuscar.Focus();
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

        private void btnNuevaAsignatura_Click(object sender, EventArgs e)
        {
            using FrmGestionAsignaturasFormulario frm = new FrmGestionAsignaturasFormulario();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                CargarAreas();
                CargarAsignaturas();
            }
        }

        #endregion

        #region GRID ACCIONES

        private void dgvAsignaturas_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvAsignaturas.Columns[e.ColumnIndex].Name != "Acciones")
                return;

            e.Handled = true;
            e.PaintBackground(e.CellBounds, true);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int btnSize = 28;
            int separacion = 8;

            int totalWidth = (btnSize * 2) + separacion;
            int startX = e.CellBounds.X + (e.CellBounds.Width - totalWidth) / 2;
            int startY = e.CellBounds.Y + (e.CellBounds.Height - btnSize) / 2;

            Rectangle rectEditar = new Rectangle(startX, startY, btnSize, btnSize);
            Rectangle rectEliminar = new Rectangle(startX + btnSize + separacion, startY, btnSize, btnSize);

            Color colorEditar = Color.FromArgb(16, 57, 129);
            Color colorEliminar = Color.FromArgb(255, 84, 84);

            using (GraphicsPath pathEditar = RedondearRectangulo(rectEditar, 4))
            using (SolidBrush brushEditar = new SolidBrush(colorEditar))
            {
                g.FillPath(brushEditar, pathEditar);
            }

            using (GraphicsPath pathEliminar = RedondearRectangulo(rectEliminar, 4))
            using (SolidBrush brushEliminar = new SolidBrush(colorEliminar))
            {
                g.FillPath(brushEliminar, pathEliminar);
            }

            DibujarIconoCentrado(g, Properties.Resources.edit_white, rectEditar, 6);
            DibujarIconoCentrado(g, Properties.Resources.delete_white, rectEliminar, 6);

            using Pen pen = new Pen(dgvAsignaturas.GridColor);
            g.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
        }

        private void dgvAsignaturas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvAsignaturas.Columns[e.ColumnIndex].Name != "Acciones")
                return;

            int asignaturaId = Convert.ToInt32(dgvAsignaturas.Rows[e.RowIndex].Cells["AsignaturaID"].Value);
            Rectangle rectCelda = dgvAsignaturas.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

            Point mouse = dgvAsignaturas.PointToClient(Cursor.Position);

            int btnSize = 28;
            int separacion = 8;

            int totalWidth = (btnSize * 2) + separacion;
            int startX = rectCelda.X + (rectCelda.Width - totalWidth) / 2;
            int startY = rectCelda.Y + (rectCelda.Height - btnSize) / 2;

            Rectangle rectEditar = new Rectangle(startX, startY, btnSize, btnSize);
            Rectangle rectEliminar = new Rectangle(startX + btnSize + separacion, startY, btnSize, btnSize);

            if (rectEditar.Contains(mouse))
            {
                using FrmGestionAsignaturasFormulario frm = new FrmGestionAsignaturasFormulario(asignaturaId);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CargarAreas();
                    CargarAsignaturas();
                }
            }
            else if (rectEliminar.Contains(mouse))
            {
                EliminarAsignatura(asignaturaId);
            }
        }

        private void EliminarAsignatura(int asignaturaId)
        {
            DialogResult r = MessageBox.Show(
                "¿Desea eliminar esta asignatura?",
                "Confirmación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_EliminarAsignatura", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AsignaturaID", asignaturaId);

                cn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Asignatura eliminada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarAreas();
                CargarAsignaturas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar asignatura: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}