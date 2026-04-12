    using GestionAcademicaV2.Modelos;
    using Microsoft.Data.SqlClient;
    using System;
    using System.Data;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Linq;
    using System.Windows.Forms;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;

    namespace GestionAcademicaV2.Pantallas.AdminVentanas
    {
        public partial class FrmGestionVinculacionTutores : Form
        {
            private readonly Conexion conexion = new Conexion();

            private bool _cargandoCombos = false;

            // GRID TUTORES
            private DataTable _dtTutoresCompleto = new DataTable();
            private DataTable _dtTutoresFiltrado = new DataTable();

            private int _paginaActualTutores = 1;
            private int _tamanoPaginaTutores = 5;
            private int _totalRegistrosTutores = 0;
            private int _totalPaginasTutores = 1;

            private string _textoBusquedaTutor = "";

            // GRID VINCULOS
            private DataTable _dtVinculosCompleto = new DataTable();
            private DataTable _dtVinculosFiltrado = new DataTable();

            private int _paginaActualVinculos = 1;
            private int _tamanoPaginaVinculos = 5;
            private int _totalRegistrosVinculos = 0;
            private int _totalPaginasVinculos = 1;

            // Tutor seleccionado
            private int _tutorIdSeleccionado = 0;
            private string _nombreTutorSeleccionado = "";
            private string _parentescoTutorSeleccionado = "";

            private readonly System.Windows.Forms. Timer _timerBusqueda = new System.Windows.Forms.Timer();
            private bool _suspendirSelectionChangedTutores = false;
            private bool _buscandoTutores = false;


            public FrmGestionVinculacionTutores()
            {
                InitializeComponent();

                Load += FrmGestionVinculacionTutores_Load;

                cbParentesco.SelectedIndexChanged += cbParentesco_SelectedIndexChanged;
                cbRegistros.SelectedIndexChanged += cbRegistros_SelectedIndexChanged;

                txtBuscar.TextChanged += txtBuscar_TextChanged;
                btBuscarDocente.Click += btBuscarDocente_Click;

                lblAnterior.Click += lblAnterior_Click;
                lblSiguiente.Click += lblSiguiente_Click;

                lblAnteriorVinculo.Click += lblAnteriorVinculo_Click;
                lblSiguienteVinculo.Click += lblSiguienteVinculo_Click;

                dgvTutores.SelectionChanged += dgvTutores_SelectionChanged;
                dgvTutores.CellClick += dgvTutores_CellClick;

                btnVincularEstudiante.Click += btnVincularEstudiante_Click;
                dgvVinculos.CellClick += dgvVinculos_CellClick;

                dgvTutores.CellPainting += dgvTutores_CellPainting;
                dgvVinculos.CellPainting += dgvVinculos_CellPainting;

                dgvTutores.CellMouseMove += dgvTutores_CellMouseMove;
                dgvTutores.MouseLeave += (s, e) => dgvTutores.Cursor = Cursors.Default;

                dgvVinculos.CellMouseMove += dgvVinculos_CellMouseMove;
                dgvVinculos.MouseLeave += (s, e) => dgvVinculos.Cursor = Cursors.Default;

                _timerBusqueda.Interval = 350;
                _timerBusqueda.Tick += TimerBusqueda_Tick;
            }

            private void dgvTutores_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    dgvTutores.Cursor = Cursors.Default;
                    return;
                }

                string nombreColumna = dgvTutores.Columns[e.ColumnIndex].Name;

                dgvTutores.Cursor = (nombreColumna == "Estado" || nombreColumna == "Editar")
                    ? Cursors.Hand
                    : Cursors.Default;
            }
            private void dgvTutores_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                string nombreColumna = dgvTutores.Columns[e.ColumnIndex].Name;

                // BADGE DE ESTADO
                if (nombreColumna == "Estado")
                {
                    e.Handled = true;
                    e.PaintBackground(e.CellBounds, true);

                    object valor = dgvTutores.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    string estado = valor?.ToString() ?? "";

                    bool activo = estado.Equals("ACTIVO", StringComparison.OrdinalIgnoreCase);

                    Color fondo = activo
                        ? Color.FromArgb(222, 243, 228)
                        : Color.FromArgb(255, 230, 230);

                    Color texto = activo
                        ? Color.FromArgb(46, 161, 85)
                        : Color.FromArgb(210, 70, 70);

                    Rectangle rect = e.CellBounds;
                    int badgeWidth = 78;
                    int badgeHeight = 24;
                    int x = rect.X + (rect.Width - badgeWidth) / 2;
                    int y = rect.Y + (rect.Height - badgeHeight) / 2;

                    Rectangle badgeRect = new Rectangle(x, y, badgeWidth, badgeHeight);

                    using (GraphicsPath path = RedondearRectangulo(badgeRect, 10))
                    using (SolidBrush brush = new SolidBrush(fondo))
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.FillPath(brush, path);
                    }

                    string textoMostrar = activo ? "ACTIVO" : "INACTIVO";
                    using (SolidBrush textBrush = new SolidBrush(texto))
                    using (Font font = new Font("Segoe UI", 9F, FontStyle.Bold))
                    {
                        SizeF textSize = e.Graphics.MeasureString(textoMostrar, font);
                        float textX = badgeRect.X + 8;
                        float textY = badgeRect.Y + (badgeRect.Height - textSize.Height) / 2;

                        e.Graphics.DrawString(textoMostrar, font, textBrush, textX, textY);
                    }

                    // flechita
                    Point p1 = new Point(badgeRect.Right - 14, badgeRect.Y + 9);
                    Point p2 = new Point(badgeRect.Right - 8, badgeRect.Y + 9);
                    Point p3 = new Point(badgeRect.Right - 11, badgeRect.Y + 13);

                    using (SolidBrush arrowBrush = new SolidBrush(texto))
                    {
                        e.Graphics.FillPolygon(arrowBrush, new[] { p1, p2, p3 });
                    }

                    using Pen pen = new Pen(dgvTutores.GridColor);
                    e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);

                    return;
                }

                // BOTÓN EDITAR
                if (nombreColumna == "Editar")
                {
                    e.Handled = true;
                    e.PaintBackground(e.CellBounds, true);

                    Graphics g = e.Graphics;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    int btnSize = 28;
                    int startX = e.CellBounds.X + (e.CellBounds.Width - btnSize) / 2;
                    int startY = e.CellBounds.Y + (e.CellBounds.Height - btnSize) / 2;

                    Rectangle rectEditar = new Rectangle(startX, startY, btnSize, btnSize);

                    using (GraphicsPath pathEditar = RedondearRectangulo(rectEditar, 4))
                    using (SolidBrush brushEditar = new SolidBrush(Color.FromArgb(16, 57, 129)))
                    {
                        g.FillPath(brushEditar, pathEditar);
                    }

                    DibujarIconoCentrado(g, Properties.Resources.edit_white, rectEditar, 6);

                    using Pen pen = new Pen(dgvTutores.GridColor);
                    g.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);

                    return;
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
            private void dgvVinculos_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                if (dgvVinculos.Columns[e.ColumnIndex].Name != "Eliminar")
                    return;

                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                int btnSize = 28;
                int startX = e.CellBounds.X + (e.CellBounds.Width - btnSize) / 2;
                int startY = e.CellBounds.Y + (e.CellBounds.Height - btnSize) / 2;

                Rectangle rectEliminar = new Rectangle(startX, startY, btnSize, btnSize);

                using (SolidBrush brushEliminar = new SolidBrush(Color.FromArgb(255, 84, 84)))
                {
                    g.FillRectangle(brushEliminar, rectEliminar);
                }

                DibujarIconoCentrado(g, Properties.Resources.delete_white, rectEliminar, 6);

                using Pen pen = new Pen(dgvVinculos.GridColor);
                g.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
            }

            private void dgvVinculos_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    dgvVinculos.Cursor = Cursors.Default;
                    return;
                }

                dgvVinculos.Cursor = dgvVinculos.Columns[e.ColumnIndex].Name == "Eliminar"
                    ? Cursors.Hand
                    : Cursors.Default;
            }
            private void FrmGestionVinculacionTutores_Load(object sender, EventArgs e)
            {
                ConfigurarCombos();

                ConfigurarGridTutores();
                AplicarTemaFinalGridTutores();

                ConfigurarGridVinculos();
                AplicarTemaFinalGridVinculos();

                CargarParentescos();
                CargarRegistrosPorPagina();

                txtBuscar.PlaceholderText = "Ingrese nombre o identidad del tutor";
                btnTexto.Text = "1";
                btnTextoVinculo.Text = "1";

                CargarTutoresDesdeBD();
                LimpiarVinculos();
            }

            #region CONFIGURACION

            private void ConfigurarCombos()
            {
                cbParentesco.DropDownStyle = ComboBoxStyle.DropDownList;
                cbRegistros.DropDownStyle = ComboBoxStyle.DropDownList;
            }

            private void ConfigurarGridTutores()
            {
                dgvTutores.AutoGenerateColumns = false;
                dgvTutores.Columns.Clear();

                dgvTutores.AllowUserToAddRows = false;
                dgvTutores.AllowUserToDeleteRows = false;
                dgvTutores.AllowUserToResizeRows = false;
                dgvTutores.AllowUserToResizeColumns = false;
                dgvTutores.MultiSelect = false;
                dgvTutores.ReadOnly = true;
                dgvTutores.RowHeadersVisible = false;
                dgvTutores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                dgvTutores.EnableHeadersVisualStyles = false;
                dgvTutores.BorderStyle = BorderStyle.None;
                dgvTutores.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                dgvTutores.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgvTutores.BackgroundColor = Color.White;
                dgvTutores.GridColor = Color.FromArgb(220, 220, 220);

                dgvTutores.ColumnHeadersHeight = 42;
                dgvTutores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dgvTutores.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 105, 255);
                dgvTutores.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvTutores.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                dgvTutores.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dgvTutores.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(24, 105, 255);
                dgvTutores.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

                dgvTutores.DefaultCellStyle.BackColor = Color.White;
                dgvTutores.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
                dgvTutores.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
                dgvTutores.DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 249, 255);
                dgvTutores.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

                dgvTutores.RowsDefaultCellStyle.BackColor = Color.White;
                dgvTutores.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
                dgvTutores.RowTemplate.Height = 40;
                dgvTutores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                dgvTutores.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "No",
                    HeaderText = "N°",
                    DataPropertyName = "No",
                    Width = 45,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                dgvTutores.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TutorID",
                    DataPropertyName = "TutorID",
                    Visible = false,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });
                dgvTutores.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UsuarioID",
                    DataPropertyName = "UsuarioID",
                    Visible = false,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                dgvTutores.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Nombre",
                    HeaderText = "NOMBRE",
                    DataPropertyName = "Nombre",
                    Width = 170,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                dgvTutores.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Identidad",
                    HeaderText = "IDENTIDAD",
                    DataPropertyName = "Identidad",
                    Width = 130,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                dgvTutores.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Telefono",
                    HeaderText = "TELÉFONO",
                    DataPropertyName = "Telefono",
                    Width = 100,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                dgvTutores.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Parentesco",
                    HeaderText = "PARENTESCO",
                    DataPropertyName = "Parentesco",
                    Width = 105,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                dgvTutores.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LugarTrabajo",
                    HeaderText = "LUGAR DE TRABAJO",
                    DataPropertyName = "LugarTrabajo",
                    Width = 230,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                dgvTutores.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Estado",
                    HeaderText = "ESTADO",
                    DataPropertyName = "Estado",
                    Width = 92,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });


                dgvTutores.Columns["No"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                DataGridViewImageColumn colEditar = new DataGridViewImageColumn();
                colEditar.Name = "Editar";
                colEditar.HeaderText = "ACCIONES";
                colEditar.Image = Properties.Resources.edit_white; // tu icono
                colEditar.ImageLayout = DataGridViewImageCellLayout.Zoom;
                colEditar.Width = 80;

                dgvTutores.Columns.Add(colEditar);
            }

            private void ConfigurarGridVinculos()
            {
                dgvVinculos.AutoGenerateColumns = false;
                dgvVinculos.Columns.Clear();

                dgvVinculos.AllowUserToAddRows = false;
                dgvVinculos.AllowUserToDeleteRows = false;
                dgvVinculos.AllowUserToResizeRows = false;
                dgvVinculos.AllowUserToResizeColumns = false;
                dgvVinculos.MultiSelect = false;
                dgvVinculos.ReadOnly = true;
                dgvVinculos.RowHeadersVisible = false;
                dgvVinculos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                dgvVinculos.EnableHeadersVisualStyles = false;
                dgvVinculos.BorderStyle = BorderStyle.None;
                dgvVinculos.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                dgvVinculos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgvVinculos.BackgroundColor = Color.White;
                dgvVinculos.GridColor = Color.FromArgb(220, 220, 220);

                dgvVinculos.ColumnHeadersHeight = 40;
                dgvVinculos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dgvVinculos.RowTemplate.Height = 40;

                dgvVinculos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                dgvVinculos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(14, 102, 248);
                dgvVinculos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvVinculos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                dgvVinculos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dgvVinculos.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(14, 102, 248);
                dgvVinculos.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
                dgvVinculos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(14, 102, 248);
                dgvVinculos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvVinculos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                dgvVinculos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dgvVinculos.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(14, 102, 248);
                dgvVinculos.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

                dgvVinculos.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TutorID",
                    DataPropertyName = "TutorID",
                    Visible = false,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                dgvVinculos.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "EstudianteID",
                    DataPropertyName = "EstudianteID",
                    Visible = false,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                dgvVinculos.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Estudiante",
                    HeaderText = "ESTUDIANTE",
                    DataPropertyName = "Nombre",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                dgvVinculos.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Grado",
                    HeaderText = "GRADO",
                    DataPropertyName = "NombreGrado",
                    Width = 95,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                dgvVinculos.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Parentesco",
                    HeaderText = "PARENTESCO",
                    DataPropertyName = "Parentesco",
                    Width = 100,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

                DataGridViewImageColumn colEliminar = new DataGridViewImageColumn
                {
                    Name = "Eliminar",
                    HeaderText = "ACCIÓN",
                    Image = Properties.Resources.delete_white,
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Width = 70
                };

                dgvVinculos.Columns.Add(colEliminar);

                dgvVinculos.Columns["Eliminar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            private DataTable CrearEstructuraTutores()
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("No", typeof(int));
                dt.Columns.Add("TutorID", typeof(int));
                dt.Columns.Add("UsuarioID", typeof(int));
                dt.Columns.Add("Nombre", typeof(string));
                dt.Columns.Add("Identidad", typeof(string));
                dt.Columns.Add("Telefono", typeof(string));
                dt.Columns.Add("Parentesco", typeof(string));
                dt.Columns.Add("LugarTrabajo", typeof(string));
                dt.Columns.Add("Estado", typeof(string));
                return dt;
            }

            private DataTable CrearEstructuraVinculos()
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("TutorID", typeof(int));
                dt.Columns.Add("EstudianteID", typeof(int));
                dt.Columns.Add("Nombre", typeof(string));
                dt.Columns.Add("NombreGrado", typeof(string));
                dt.Columns.Add("Parentesco", typeof(string));
                return dt;
            }

            #endregion

            #region COMBOS

            private void CargarParentescos()
            {
                try
                {
                    _cargandoCombos = true;

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Valor", typeof(string));
                    dt.Columns.Add("Texto", typeof(string));

                    dt.Rows.Add("", "--SELECCIONE--");
                    dt.Rows.Add("PADRE", "PADRE");
                    dt.Rows.Add("MADRE", "MADRE");
                    dt.Rows.Add("ABUELO(A)", "ABUELO(A)");
                    dt.Rows.Add("TÍA", "TÍA");
                    dt.Rows.Add("TÍO", "TÍO");
                    dt.Rows.Add("TUTOR LEGAL", "TUTOR LEGAL");
                    dt.Rows.Add("ENCARGADO LEGAL", "ENCARGADO LEGAL");
                    dt.Rows.Add("OTRO", "OTRO");

                    cbParentesco.DataSource = dt;
                    cbParentesco.ValueMember = "Valor";
                    cbParentesco.DisplayMember = "Texto";
                    cbParentesco.SelectedIndex = 0;
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

                cbRegistros.DataSource = dt;
                cbRegistros.ValueMember = "Valor";
                cbRegistros.DisplayMember = "Texto";
                cbRegistros.SelectedValue = 5;

                _tamanoPaginaTutores = 5;
                _tamanoPaginaVinculos = 5;

                _cargandoCombos = false;
            }

            private string ObtenerParentescoSeleccionado()
            {
                return cbParentesco.SelectedValue?.ToString() ?? "";
            }

            #endregion

            #region CARGA TUTORES

            private DataTable ObtenerTutoresDesdeBD()
            {
                DataTable dt = new DataTable();

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand("sp_MAE_ListarTutores", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Carga base. Si tu SP soporta parentesco, lo mandamos.
                cmd.Parameters.AddWithValue("@Parentesco", ObtenerParentescoSeleccionado());

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return dt;
            }

            private void CargarTutoresDesdeBD()
            {
                try
                {
                    DataTable dtBD = ObtenerTutoresDesdeBD();
                    _dtTutoresCompleto = CrearEstructuraTutores();

                    int correlativo = 1;

                    foreach (DataRow row in dtBD.Rows)
                    {
                        DataRow nueva = _dtTutoresCompleto.NewRow();
                        nueva["No"] = correlativo++;
                        nueva["TutorID"] = Convert.ToInt32(row["TutorID"]);
                        nueva["UsuarioID"] = row.Table.Columns.Contains("UsuarioID")
                        ? Convert.ToInt32(row["UsuarioID"])
                        : 0;
                        nueva["Nombre"] = row["Nombre"]?.ToString() ?? "";
                        nueva["Identidad"] = row["Identidad"]?.ToString() ?? "";
                        nueva["Telefono"] = row["Telefono"]?.ToString() ?? "";
                        nueva["Parentesco"] = row["Parentesco"]?.ToString() ?? "";
                        nueva["LugarTrabajo"] = row.Table.Columns.Contains("LugarTrabajo")
                            ? row["LugarTrabajo"]?.ToString() ?? ""
                            : "";
                        string estadoTexto = "";

                        if (row.Table.Columns.Contains("Estado"))
                        {
                            string valorEstado = row["Estado"]?.ToString()?.Trim().ToLower() ?? "";

                            if (valorEstado == "1" || valorEstado == "true" || valorEstado == "activo")
                                estadoTexto = "ACTIVO";
                            else
                                estadoTexto = "INACTIVO";
                        }

                        nueva["Estado"] = estadoTexto;

                        _dtTutoresCompleto.Rows.Add(nueva);
                    }

                    AplicarFiltroLocalTutores();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar tutores: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void AplicarFiltroLocalTutores()
            {
                try
                {
                    if (_dtTutoresCompleto == null || _dtTutoresCompleto.Rows.Count == 0)
                    {
                        _dtTutoresFiltrado = CrearEstructuraTutores();
                        _totalRegistrosTutores = 0;
                        _totalPaginasTutores = 1;
                        _paginaActualTutores = 1;
                        MostrarPaginaTutores();
                        return;
                    }

                    string parentesco = ObtenerParentescoSeleccionado().Trim().ToLowerInvariant();
                    string texto = _textoBusquedaTutor.Trim().ToLowerInvariant();

                    var filas = _dtTutoresCompleto.AsEnumerable()
                    .Where(r =>
                    {
                        string parentescoFila = (r["Parentesco"]?.ToString() ?? "").Trim().ToLowerInvariant();
                        string nombreFila = (r["Nombre"]?.ToString() ?? "").Trim().ToLowerInvariant();
                        string identidadFila = (r["Identidad"]?.ToString() ?? "").Trim().ToLowerInvariant();

                        bool cumpleParentesco =
                            string.IsNullOrWhiteSpace(parentesco) || parentescoFila == parentesco;

                        bool cumpleTexto =
                            string.IsNullOrWhiteSpace(texto) ||
                            nombreFila.Contains(texto) ||
                            identidadFila.Contains(texto);

                        return cumpleParentesco && cumpleTexto;
                    });

                    _dtTutoresFiltrado = filas.Any()
                        ? filas.CopyToDataTable()
                        : CrearEstructuraTutores();

                    _totalRegistrosTutores = _dtTutoresFiltrado.Rows.Count;
                    _totalPaginasTutores = _totalRegistrosTutores == 0
                        ? 1
                        : (int)Math.Ceiling((double)_totalRegistrosTutores / _tamanoPaginaTutores);

                    _paginaActualTutores = 1;
                    MostrarPaginaTutores();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al aplicar filtros de tutores: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void MostrarPaginaTutores()
            {
                if (_dtTutoresFiltrado == null)
                    return;

                DataTable dtPagina = _dtTutoresFiltrado.Clone();

                var filasPagina = _dtTutoresFiltrado.AsEnumerable()
                    .Skip((_paginaActualTutores - 1) * _tamanoPaginaTutores)
                    .Take(_tamanoPaginaTutores);

                foreach (var fila in filasPagina)
                    dtPagina.ImportRow(fila);

                _suspendirSelectionChangedTutores = true;

                try
                {
                    dgvTutores.SuspendLayout();

                    bool requiereConfigurarColumnas = dgvTutores.Columns.Count == 0;

                    dgvTutores.DataSource = null;

                    if (requiereConfigurarColumnas)
                        ConfigurarGridTutores();

                    dgvTutores.AutoGenerateColumns = false;
                    dgvTutores.DataSource = dtPagina;
                    dgvTutores.ClearSelection();

                    AplicarTemaFinalGridTutores();

                    ActualizarTextoRegistrosTutores();
                    ActualizarControlesPaginacionTutores();
                }
                finally
                {
                    dgvTutores.ResumeLayout();
                    dgvTutores.Refresh();
                    _suspendirSelectionChangedTutores = false;
                }

                if (dgvTutores.Rows.Count > 0)
                {
                    dgvTutores.Rows[0].Selected = true;

                    if (!_buscandoTutores)
                    {
                        dgvTutores.CurrentCell = dgvTutores.Rows[0].Cells["Nombre"];
                        SeleccionarTutorDesdeFila(0);
                    }
                }
                else
                {
                    _tutorIdSeleccionado = 0;
                    _nombreTutorSeleccionado = "";
                    _parentescoTutorSeleccionado = "";
                    LimpiarVinculos();
                }
            }

            private void ActualizarTextoRegistrosTutores()
            {
                if (_totalRegistrosTutores == 0)
                {
                    lblRegistros.Text = "Sin resultados";
                    return;
                }

                int desde = ((_paginaActualTutores - 1) * _tamanoPaginaTutores) + 1;
                int hasta = Math.Min(_paginaActualTutores * _tamanoPaginaTutores, _totalRegistrosTutores);

                lblRegistros.Text = $"Registros del {desde} al {hasta} total de {_totalRegistrosTutores} registros";
            }

            private void ActualizarControlesPaginacionTutores()
            {
                btnTexto.Text = _paginaActualTutores.ToString();

                lblAnterior.Enabled = _paginaActualTutores > 1;
                lblSiguiente.Enabled = _paginaActualTutores < _totalPaginasTutores;

                lblAnterior.ForeColor = lblAnterior.Enabled ? Color.FromArgb(93, 93, 93) : Color.LightGray;
                lblSiguiente.ForeColor = lblSiguiente.Enabled ? Color.FromArgb(93, 93, 93) : Color.LightGray;
            }

            #endregion

            #region CARGA VINCULOS

            private void CargarVinculosTutor(int tutorId)
            {
                try
                {
                    _dtVinculosCompleto = CrearEstructuraVinculos();

                    using SqlConnection cn = conexion.ObtenerConexion();
                    using SqlCommand cmd = new SqlCommand("sp_MAE_EstudiantesVinculados", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@tutorID", tutorId);

                    using SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dtBD = new DataTable();
                    da.Fill(dtBD);

                    foreach (DataRow row in dtBD.Rows)
                    {
                        DataRow nueva = _dtVinculosCompleto.NewRow();
                        nueva["TutorID"] = Convert.ToInt32(row["TutorID"]);
                        nueva["EstudianteID"] = Convert.ToInt32(row["EstudianteID"]);
                        nueva["Nombre"] = row["Nombre"]?.ToString() ?? "";
                        nueva["NombreGrado"] = row["NombreGrado"]?.ToString() ?? "";
                        nueva["Parentesco"] = row["Parentesco"]?.ToString() ?? "";

                        _dtVinculosCompleto.Rows.Add(nueva);
                    }

                    _dtVinculosFiltrado = _dtVinculosCompleto.Copy();
                    _totalRegistrosVinculos = _dtVinculosFiltrado.Rows.Count;
                    _totalPaginasVinculos = _totalRegistrosVinculos == 0
                        ? 1
                        : (int)Math.Ceiling((double)_totalRegistrosVinculos / _tamanoPaginaVinculos);

                    _paginaActualVinculos = 1;
                    MostrarPaginaVinculos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar vínculos del tutor: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void LimpiarVinculos()
            {
                _dtVinculosCompleto = CrearEstructuraVinculos();
                _dtVinculosFiltrado = _dtVinculosCompleto.Copy();
                _totalRegistrosVinculos = 0;
                _totalPaginasVinculos = 1;
                _paginaActualVinculos = 1;
                MostrarPaginaVinculos();
            }

            private void MostrarPaginaVinculos()
            {
                if (_dtVinculosFiltrado == null)
                    return;

                DataTable dtPagina = _dtVinculosFiltrado.Clone();

                var filasPagina = _dtVinculosFiltrado.AsEnumerable()
                    .Skip((_paginaActualVinculos - 1) * _tamanoPaginaVinculos)
                    .Take(_tamanoPaginaVinculos);

                foreach (var fila in filasPagina)
                    dtPagina.ImportRow(fila);

                dgvVinculos.SuspendLayout();

                dgvVinculos.DataSource = null;
                dgvVinculos.Columns.Clear();

                ConfigurarGridVinculos();   // volver a crear columnas SIEMPRE

                dgvVinculos.AutoGenerateColumns = false;
                dgvVinculos.DataSource = dtPagina;
                dgvVinculos.ClearSelection();

                AplicarTemaFinalGridVinculos();

                dgvVinculos.ResumeLayout();
                dgvVinculos.Refresh();

                ActualizarTextoRegistrosVinculos();
                ActualizarControlesPaginacionVinculos();
            }
            private void AplicarTemaFinalGridVinculos()
            {
                dgvVinculos.EnableHeadersVisualStyles = false;
                dgvVinculos.ColumnHeadersHeight = 40;
                dgvVinculos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dgvVinculos.RowTemplate.Height = 40;
                dgvVinculos.ReadOnly = true;

                dgvVinculos.ThemeStyle.HeaderStyle.Height = 40;
                dgvVinculos.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dgvVinculos.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(24, 105, 255);
                dgvVinculos.ThemeStyle.HeaderStyle.ForeColor = Color.White;
                dgvVinculos.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

                dgvVinculos.ThemeStyle.RowsStyle.Height = 40;
                dgvVinculos.ThemeStyle.RowsStyle.BackColor = Color.White;
                dgvVinculos.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(35, 35, 35);
                dgvVinculos.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(245, 249, 255);
                dgvVinculos.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

                dgvVinculos.ThemeStyle.ReadOnly = true;
                dgvVinculos.GridColor = Color.FromArgb(220, 220, 220);
                dgvVinculos.ThemeStyle.GridColor = Color.FromArgb(220, 220, 220);

                dgvVinculos.BackgroundColor = Color.White;
                dgvVinculos.DefaultCellStyle.BackColor = Color.White;
                dgvVinculos.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
                dgvVinculos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            }
            private void AplicarTemaFinalGridTutores()
            {
                dgvTutores.EnableHeadersVisualStyles = false;
                dgvTutores.ColumnHeadersHeight = 42;
                dgvTutores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dgvTutores.RowTemplate.Height = 40;
                dgvTutores.ReadOnly = true;

                dgvTutores.ThemeStyle.HeaderStyle.Height = 42;
                dgvTutores.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dgvTutores.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(24, 105, 255);
                dgvTutores.ThemeStyle.HeaderStyle.ForeColor = Color.White;
                dgvTutores.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

                dgvTutores.ThemeStyle.RowsStyle.Height = 40;
                dgvTutores.ThemeStyle.RowsStyle.BackColor = Color.White;
                dgvTutores.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(35, 35, 35);
                dgvTutores.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(245, 249, 255);
                dgvTutores.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

                dgvTutores.ThemeStyle.ReadOnly = true;
                dgvTutores.GridColor = Color.FromArgb(220, 220, 220);
                dgvTutores.ThemeStyle.GridColor = Color.FromArgb(220, 220, 220);

                dgvTutores.BackgroundColor = Color.White;
                dgvTutores.DefaultCellStyle.BackColor = Color.White;
                dgvTutores.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
                dgvTutores.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            }
            private void ActualizarTextoRegistrosVinculos()
            {
                if (_totalRegistrosVinculos == 0)
                {
                    lblRegistroVinculos.Text = "Sin vínculos";
                    return;
                }

                int desde = ((_paginaActualVinculos - 1) * _tamanoPaginaVinculos) + 1;
                int hasta = Math.Min(_paginaActualVinculos * _tamanoPaginaVinculos, _totalRegistrosVinculos);

                lblRegistroVinculos.Text = $"Registros del {desde} al {hasta} total de {_totalRegistrosVinculos} registros";
            }

            private void ActualizarControlesPaginacionVinculos()
            {
                btnTextoVinculo.Text = _paginaActualVinculos.ToString();

                lblAnteriorVinculo.Enabled = _paginaActualVinculos > 1;
                lblSiguienteVinculo.Enabled = _paginaActualVinculos < _totalPaginasVinculos;

                lblAnteriorVinculo.ForeColor = lblAnteriorVinculo.Enabled ? Color.FromArgb(93, 93, 93) : Color.LightGray;
                lblSiguienteVinculo.ForeColor = lblSiguienteVinculo.Enabled ? Color.FromArgb(93, 93, 93) : Color.LightGray;
            }

            #endregion

            #region SELECCION

            private void SeleccionarTutorDesdeFila(int rowIndex)
            {
                if (rowIndex < 0 || rowIndex >= dgvTutores.Rows.Count)
                    return;

                DataGridViewRow row = dgvTutores.Rows[rowIndex];

                object tutorIdObj = ObtenerValorCelda(row, "TutorID");
                if (tutorIdObj == null || tutorIdObj == DBNull.Value)
                    return;

                _tutorIdSeleccionado = Convert.ToInt32(tutorIdObj);
                _nombreTutorSeleccionado = Convert.ToString(ObtenerValorCelda(row, "Nombre")) ?? "";
                _parentescoTutorSeleccionado = Convert.ToString(ObtenerValorCelda(row, "Parentesco")) ?? "";

                CargarVinculosTutor(_tutorIdSeleccionado);
            }

            private object? ObtenerValorCelda(DataGridViewRow row, string nombreColumna)
            {
                // 1. Intentar por nombre real de columna
                if (row.DataGridView.Columns.Contains(nombreColumna))
                    return row.Cells[nombreColumna].Value;

                // 2. Intentar localizarla por DataPropertyName
                foreach (DataGridViewColumn col in row.DataGridView.Columns)
                {
                    if (string.Equals(col.DataPropertyName, nombreColumna, StringComparison.OrdinalIgnoreCase))
                        return row.Cells[col.Index].Value;
                }

                // 3. Intentar desde el DataBoundItem
                if (row.DataBoundItem is DataRowView drv && drv.Row.Table.Columns.Contains(nombreColumna))
                    return drv[nombreColumna];

                return null;
            }

            #endregion

            #region EVENTOS

            private void cbParentesco_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (_cargandoCombos) return;

                _paginaActualTutores = 1;
                CargarTutoresDesdeBD();
            }

            private void cbRegistros_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (_cargandoCombos) return;
                if (cbRegistros.SelectedValue == null) return;
                if (!int.TryParse(cbRegistros.SelectedValue.ToString(), out int valor)) return;

                _tamanoPaginaTutores = valor;
                _tamanoPaginaVinculos = valor;

                _paginaActualTutores = 1;
                _paginaActualVinculos = 1;

                MostrarPaginaTutores();
                MostrarPaginaVinculos();
            }

            private void txtBuscar_TextChanged(object sender, EventArgs e)
            {
                _textoBusquedaTutor = txtBuscar.Text.Trim();
                _paginaActualTutores = 1;

                _timerBusqueda.Stop();
                _timerBusqueda.Start();
            }
            private void TimerBusqueda_Tick(object? sender, EventArgs e)
            {
                _timerBusqueda.Stop();
                EjecutarBusquedaTutores();
            }
            private void EjecutarBusquedaTutores()
            {
                try
                {
                    _buscandoTutores = true;

                    string texto = (_textoBusquedaTutor ?? "").Trim();

                    if (!string.IsNullOrWhiteSpace(texto) && texto.Length < 2)
                        return;

                    AplicarFiltroLocalTutores();
                }
                finally
                {
                    _buscandoTutores = false;
                }
            }
            private void btBuscarDocente_Click(object sender, EventArgs e)
            {
                _timerBusqueda.Stop();
                _textoBusquedaTutor = txtBuscar.Text.Trim();
                _paginaActualTutores = 1;
                EjecutarBusquedaTutores();
                txtBuscar.Focus();
            }

            private void lblAnterior_Click(object sender, EventArgs e)
            {
                if (_paginaActualTutores > 1)
                {
                    _paginaActualTutores--;
                    MostrarPaginaTutores();
                }
            }

            private void lblSiguiente_Click(object sender, EventArgs e)
            {
                if (_paginaActualTutores < _totalPaginasTutores)
                {
                    _paginaActualTutores++;
                    MostrarPaginaTutores();
                }
            }

            private void lblAnteriorVinculo_Click(object sender, EventArgs e)
            {
                if (_paginaActualVinculos > 1)
                {
                    _paginaActualVinculos--;
                    MostrarPaginaVinculos();
                }
            }

            private void lblSiguienteVinculo_Click(object sender, EventArgs e)
            {
                if (_paginaActualVinculos < _totalPaginasVinculos)
                {
                    _paginaActualVinculos++;
                    MostrarPaginaVinculos();
                }
            }
            private void dgvVinculos_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                if (dgvVinculos.Columns[e.ColumnIndex].Name != "Eliminar")
                    return;

                DesvincularEstudianteDesdeFila(e.RowIndex);
            }
            private void DesvincularEstudianteDesdeFila(int rowIndex)
            {
                if (rowIndex < 0 || rowIndex >= dgvVinculos.Rows.Count)
                    return;

                DataGridViewRow row = dgvVinculos.Rows[rowIndex];

                if (row.DataBoundItem is not DataRowView drv)
                    return;

                if (!drv.Row.Table.Columns.Contains("TutorID") || !drv.Row.Table.Columns.Contains("EstudianteID"))
                    return;

                int tutorId = Convert.ToInt32(drv["TutorID"]);
                int estudianteId = Convert.ToInt32(drv["EstudianteID"]);
                string nombreEstudiante = drv.Row.Table.Columns.Contains("Nombre")
                    ? drv["Nombre"]?.ToString() ?? ""
                    : "este estudiante";

                DialogResult r = MessageBox.Show(
                    $"¿Desea desvincular a {nombreEstudiante} de este tutor?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (r != DialogResult.Yes)
                    return;

                try
                {
                    using SqlConnection cn = conexion.ObtenerConexion();
                    using SqlCommand cmd = new SqlCommand("sp_MAE_DesvincularEstudiante", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@tutorID", tutorId);
                    cmd.Parameters.AddWithValue("@estudianteID", estudianteId);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Estudiante desvinculado correctamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // recargar vínculos del tutor actual
                    CargarVinculosTutor(tutorId);

                    // opcional: refrescar tutores también
                    // CargarTutoresDesdeBD();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error SQL al desvincular: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al desvincular estudiante: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            private void dgvTutores_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

                string col = dgvTutores.Columns[e.ColumnIndex].Name;

                if (col == "Editar")
                {
                    EditarTutorDesdeFila(e.RowIndex);
                    return;
                }

                if (col == "Estado")
                {
                    CambiarEstadoTutorDesdeFila(e.RowIndex);
                    return;
                }

                SeleccionarTutorDesdeFila(e.RowIndex);
            }

            private void CambiarEstadoTutorDesdeFila(int rowIndex)
            {
                if (rowIndex < 0 || rowIndex >= dgvTutores.Rows.Count)
                    return;

                DataGridViewRow row = dgvTutores.Rows[rowIndex];

                if (row.DataBoundItem is not DataRowView drv)
                    return;

                int usuarioId = drv.Row.Table.Columns.Contains("UsuarioID")
                    ? Convert.ToInt32(drv["UsuarioID"])
                    : 0;

                if (usuarioId <= 0)
                {
                    MessageBox.Show("No se encontró el UsuarioID del tutor.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string nombre = drv.Row.Table.Columns.Contains("Nombre")
                    ? drv["Nombre"]?.ToString() ?? "este tutor"
                    : "este tutor";

                string estadoActual = drv.Row.Table.Columns.Contains("Estado")
                    ? drv["Estado"]?.ToString() ?? ""
                    : "";

                bool estaActivo = estadoActual.Equals("ACTIVO", StringComparison.OrdinalIgnoreCase);
                string accion = estaActivo ? "desactivar" : "activar";

                DialogResult r = MessageBox.Show(
                    $"¿Desea {accion} a {nombre}?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (r != DialogResult.Yes)
                    return;

                try
                {
                    using SqlConnection cn = conexion.ObtenerConexion();
                    using SqlCommand cmd = new SqlCommand("spMAE_CambiarEstadoUsuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@usuarioID", usuarioId);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show(
                        $"Tutor {(estaActivo ? "desactivado" : "activado")} correctamente.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    int tutorActual = _tutorIdSeleccionado;

                    CargarTutoresDesdeBD();

                    // intentar volver a seleccionar el tutor actual
                    if (tutorActual > 0)
                    {
                        foreach (DataGridViewRow fila in dgvTutores.Rows)
                        {
                            object valor = ObtenerValorCelda(fila, "TutorID");
                            if (valor != null && Convert.ToInt32(valor) == tutorActual)
                            {
                                fila.Selected = true;
                                dgvTutores.CurrentCell = fila.Cells["Nombre"];
                                SeleccionarTutorDesdeFila(fila.Index);
                                break;
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error SQL al cambiar el estado: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cambiar el estado del tutor: " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void EditarTutorDesdeFila(int rowIndex)
            {
                if (rowIndex < 0 || rowIndex >= dgvTutores.Rows.Count)
                    return;

                DataGridViewRow row = dgvTutores.Rows[rowIndex];

                if (row.DataBoundItem is not DataRowView drv)
                    return;

                int tutorId = Convert.ToInt32(drv["TutorID"]);
                int usuarioId = Convert.ToInt32(drv["UsuarioID"]);

                using FrmGestionVinculacionTutoresEditarTutor frm = new FrmGestionVinculacionTutoresEditarTutor(usuarioId, tutorId);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    int tutorActual = _tutorIdSeleccionado;

                    CargarTutoresDesdeBD();

                    if (tutorActual > 0)
                    {
                        foreach (DataGridViewRow fila in dgvTutores.Rows)
                        {
                            object valor = ObtenerValorCelda(fila, "TutorID");
                            if (valor != null && Convert.ToInt32(valor) == tutorActual)
                            {
                                fila.Selected = true;
                                dgvTutores.CurrentCell = fila.Cells["Nombre"];
                                SeleccionarTutorDesdeFila(fila.Index);
                                break;
                            }
                        }
                    }
                }
            }

            private void dgvTutores_SelectionChanged(object sender, EventArgs e)
            {
                if (_suspendirSelectionChangedTutores) return;
                if (_buscandoTutores) return;
                if (dgvTutores.CurrentRow == null) return;
                if (dgvTutores.CurrentRow.Index < 0) return;

                SeleccionarTutorDesdeFila(dgvTutores.CurrentRow.Index);
            }
            private void btnVincularEstudiante_Click(object sender, EventArgs e)
            {
                if (_tutorIdSeleccionado <= 0)
                {
                    MessageBox.Show("Debe seleccionar un tutor antes de vincular un estudiante.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Aquí asumo que crearás un constructor nuevo en FrmVincularEstudiante
                // para recibir tutor id, nombre y parentesco.
                using FrmGestionVinculacionTutoresVincularEstudiante frm = new FrmGestionVinculacionTutoresVincularEstudiante(
                    _tutorIdSeleccionado,
                    _nombreTutorSeleccionado,
                    _parentescoTutorSeleccionado
                );

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CargarTutoresDesdeBD();
                    CargarVinculosTutor(_tutorIdSeleccionado);
                }
            }

            #endregion
        }
    }