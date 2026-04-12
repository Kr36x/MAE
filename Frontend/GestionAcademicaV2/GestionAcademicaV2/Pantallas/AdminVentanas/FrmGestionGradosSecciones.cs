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
    public partial class FrmGestionGradosSecciones : Form
    {
        private readonly Conexion conexion = new Conexion();

        private readonly int _gradoId;
        private readonly string _nombreGrado;
        private readonly string _nivel;

        private DataTable _dtSeccionesCompleto = new DataTable();
        private DataTable _dtSeccionesFiltrado = new DataTable();

        private int _paginaActual = 1;
        private int _tamanoPagina = 5;
        private int _totalRegistros = 0;
        private int _totalPaginas = 1;

        public event EventHandler? SeccionCreada;

        public FrmGestionGradosSecciones(int gradoId, string nombreGrado, string nivel)
        {
            InitializeComponent();

            _gradoId = gradoId;
            _nombreGrado = nombreGrado;
            _nivel = nivel;

            Load += FrmNuevaSeccion_Load;
            btnNuevaSeccion.Click += btnNuevaSeccion_Click;
            lblAnterior.Click += lblAnterior_Click;
            lblSiguiente.Click += lblSiguiente_Click;
            dgvSecciones.CellClick += dgvSecciones_CellClick;
            dgvSecciones.CellPainting += dgvSecciones_CellPainting;
            dgvSecciones.CellMouseMove += dgvSecciones_CellMouseMove;
            dgvSecciones.MouseLeave += (s, e) => dgvSecciones.Cursor = Cursors.Default;
        }

        private void FrmNuevaSeccion_Load(object? sender, EventArgs e)
        {
            ConfigurarGrid();
            AplicarTemaFinalGrid();
            btnTexto.Text = "1";
            CargarSecciones();
        }

        private void ConfigurarGrid()
        {
            dgvSecciones.AutoGenerateColumns = false;
            dgvSecciones.Columns.Clear();

            dgvSecciones.AllowUserToAddRows = false;
            dgvSecciones.AllowUserToDeleteRows = false;
            dgvSecciones.AllowUserToResizeColumns = false;
            dgvSecciones.AllowUserToResizeRows = false;
            dgvSecciones.MultiSelect = false;
            dgvSecciones.ReadOnly = true;
            dgvSecciones.RowHeadersVisible = false;
            dgvSecciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvSecciones.EnableHeadersVisualStyles = false;
            dgvSecciones.BorderStyle = BorderStyle.None;
            dgvSecciones.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvSecciones.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvSecciones.BackgroundColor = Color.White;
            dgvSecciones.GridColor = Color.FromArgb(220, 220, 220);

            dgvSecciones.ColumnHeadersHeight = 40;
            dgvSecciones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvSecciones.RowTemplate.Height = 40;

            dgvSecciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "No",
                HeaderText = "N°",
                DataPropertyName = "No",
                Width = 45,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            dgvSecciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SeccionID",
                DataPropertyName = "SeccionID",
                Visible = false,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            dgvSecciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Seccion",
                HeaderText = "SECCIÓN",
                DataPropertyName = "Letra",
                Width = 120,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            DataGridViewImageColumn colEliminar = new DataGridViewImageColumn
            {
                Name = "Eliminar",
                HeaderText = "ACCIÓN",
                Image = Properties.Resources.delete_white,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 80
            };

            dgvSecciones.Columns.Add(colEliminar);
        }

        private void AplicarTemaFinalGrid()
        {
            dgvSecciones.EnableHeadersVisualStyles = false;
            dgvSecciones.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(101, 191, 104);
            dgvSecciones.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSecciones.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvSecciones.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(101, 191, 104);
            dgvSecciones.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgvSecciones.DefaultCellStyle.BackColor = Color.White;
            dgvSecciones.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgvSecciones.DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 249, 255);
            dgvSecciones.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

            dgvSecciones.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
        }

        private DataTable CrearEstructura()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(int));
            dt.Columns.Add("SeccionID", typeof(int));
            dt.Columns.Add("Letra", typeof(string));
            dt.Columns.Add("Turno", typeof(string));
            return dt;
        }

        private void CargarSecciones()
        {
            try
            {
                _dtSeccionesCompleto = CrearEstructura();

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_ListarSeccionesPorGrado", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GradoID", _gradoId);

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dtBD = new DataTable();
                da.Fill(dtBD);

                int correlativo = 1;
                foreach (DataRow row in dtBD.Rows)
                {
                    DataRow nueva = _dtSeccionesCompleto.NewRow();
                    nueva["No"] = correlativo++;
                    nueva["SeccionID"] = Convert.ToInt32(row["SeccionID"]);
                    nueva["Letra"] = row["Letra"]?.ToString() ?? "";
                    nueva["Turno"] = row["Turno"]?.ToString() ?? "";
                    _dtSeccionesCompleto.Rows.Add(nueva);
                }

                _dtSeccionesFiltrado = _dtSeccionesCompleto.Copy();
                _totalRegistros = _dtSeccionesFiltrado.Rows.Count;
                _totalPaginas = _totalRegistros == 0 ? 1 : (int)Math.Ceiling((double)_totalRegistros / _tamanoPagina);
                _paginaActual = 1;

                MostrarPagina();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar secciones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarPagina()
        {
            DataTable dtPagina = _dtSeccionesFiltrado.Clone();

            var filas = _dtSeccionesFiltrado.AsEnumerable()
                .Skip((_paginaActual - 1) * _tamanoPagina)
                .Take(_tamanoPagina);

            foreach (var fila in filas)
                dtPagina.ImportRow(fila);

            dgvSecciones.DataSource = null;
            dgvSecciones.DataSource = dtPagina;

            ActualizarTextoRegistros();
            ActualizarControlesPaginacion();
        }

        private void ActualizarTextoRegistros()
        {
            if (_totalRegistros == 0)
            {
                lblRegistros.Text = "Sin secciones";
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

        private void btnNuevaSeccion_Click(object? sender, EventArgs e)
        {
            using FrmGestionGradosFormulario frm = new FrmGestionGradosFormulario(_gradoId, _nombreGrado, _nivel);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                CargarSecciones();
            }
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

        private void dgvSecciones_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvSecciones.Columns[e.ColumnIndex].Name != "Eliminar")
                return;

            DataGridViewRow row = dgvSecciones.Rows[e.RowIndex];
            if (row.DataBoundItem is not DataRowView drv)
                return;

            int seccionId = Convert.ToInt32(drv["SeccionID"]);
            string letra = drv["Letra"]?.ToString() ?? "";

            DialogResult r = MessageBox.Show(
                $"¿Desea eliminar la sección {letra}?",
                "Confirmación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r != DialogResult.Yes)
                return;

            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("spMAE_EliminarSeccion", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SeccionID", seccionId);

                cn.Open();
                cmd.ExecuteNonQuery();

                MessageBox.Show("Sección eliminada correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarSecciones();
                SeccionCreada?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar sección: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSecciones_CellMouseMove(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                dgvSecciones.Cursor = Cursors.Default;
                return;
            }

            dgvSecciones.Cursor = dgvSecciones.Columns[e.ColumnIndex].Name == "Eliminar"
                ? Cursors.Hand
                : Cursors.Default;
        }

        private void dgvSecciones_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvSecciones.Columns[e.ColumnIndex].Name != "Eliminar")
                return;

            e.Handled = true;
            e.PaintBackground(e.CellBounds, true);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int btnSize = 28;
            int startX = e.CellBounds.X + (e.CellBounds.Width - btnSize) / 2;
            int startY = e.CellBounds.Y + (e.CellBounds.Height - btnSize) / 2;

            Rectangle rect = new Rectangle(startX, startY, btnSize, btnSize);

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 84, 84)))
                g.FillRectangle(brush, rect);

            DibujarIconoCentrado(g, Properties.Resources.delete_white, rect, 6);

            using Pen pen = new Pen(dgvSecciones.GridColor);
            g.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
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
    }
}