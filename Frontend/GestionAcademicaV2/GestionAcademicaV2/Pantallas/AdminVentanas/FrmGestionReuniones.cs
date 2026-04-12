using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DrawingColor = System.Drawing.Color;


using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.IO.Image;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using System.IO;
using System.Drawing.Imaging;

using PdfColor = iText.Kernel.Colors.Color;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmGestionReuniones : Form
    {
        private readonly Conexion conexion = new Conexion();

        private bool _cargandoCombos = false;

        private DataTable _dtReunionesCompleto = new DataTable();
        private DataTable _dtReunionesFiltrado = new DataTable();

        private int _paginaActual = 1;
        private int _tamanoPagina = 5;
        private int _totalRegistros = 0;
        private int _totalPaginas = 1;

        private string _textoBusquedaActual = "";

        public FrmGestionReuniones()
        {
            InitializeComponent();

            Load += FrmControlReuniones_Load;

            cbDocente.SelectedIndexChanged += cbDocente_SelectedIndexChanged;
            cbMes.SelectedIndexChanged += cbMes_SelectedIndexChanged;
            cbEstado.SelectedIndexChanged += cbEstado_SelectedIndexChanged;
            cbCicloAcademico.SelectedIndexChanged += cbCicloAcademico_SelectedIndexChanged;

            txtBuscar.TextChanged += txtBuscar_TextChanged;
            btnBuscar.Click += btnBuscar_Click;

            lblAnterior.Click += lblAnterior_Click;
            lblSiguiente.Click += lblSiguiente_Click;

            dgvReuniones.CellPainting += dgvReuniones_CellPainting;
            dgvReuniones.CellClick += dgvReuniones_CellClick;

            btnNuevaActividad.Click += btnNuevaActividad_Click;

            dgvReuniones.CellMouseEnter += dgvReuniones_CellMouseEnter;
            dgvReuniones.CellMouseLeave += dgvReuniones_CellMouseLeave;
        }

        private void FrmControlReuniones_Load(object sender, EventArgs e)
        {
            ConfigurarCombos();
            ConfigurarGrid();
            CargarEstados();
            CargarDocentes();
            CargarCiclosAcademicos();
            CargarMesesDisponibles();
            CargarReuniones();

            txtBuscar.PlaceholderText = "Ingresar nombre a buscar";
            btnTexto.Text = "1";
        }

        #region CONFIG

        private void ConfigurarCombos()
        {
            cbDocente.DropDownStyle = ComboBoxStyle.DropDownList;
            cbMes.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCicloAcademico.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void ConfigurarGrid()
        {
            dgvReuniones.AutoGenerateColumns = false;
            dgvReuniones.Columns.Clear();

            dgvReuniones.AllowUserToAddRows = false;
            dgvReuniones.AllowUserToDeleteRows = false;
            dgvReuniones.AllowUserToResizeRows = false;
            dgvReuniones.AllowUserToResizeColumns = false;
            dgvReuniones.MultiSelect = false;
            dgvReuniones.ReadOnly = true;
            dgvReuniones.RowHeadersVisible = false;
            dgvReuniones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvReuniones.EnableHeadersVisualStyles = false;
            dgvReuniones.BorderStyle = BorderStyle.None;
            dgvReuniones.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvReuniones.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvReuniones.BackgroundColor = DrawingColor.White;
            dgvReuniones.GridColor = DrawingColor.FromArgb(220, 220, 220);

            dgvReuniones.ColumnHeadersHeight = 56;
            dgvReuniones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvReuniones.ColumnHeadersDefaultCellStyle.BackColor = DrawingColor.FromArgb(24, 105, 255);
            dgvReuniones.ColumnHeadersDefaultCellStyle.ForeColor = DrawingColor.White;
            dgvReuniones.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvReuniones.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvReuniones.ColumnHeadersDefaultCellStyle.SelectionBackColor = DrawingColor.FromArgb(24, 105, 255);
            dgvReuniones.ColumnHeadersDefaultCellStyle.SelectionForeColor = DrawingColor.White;
            dgvReuniones.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);

            dgvReuniones.DefaultCellStyle.BackColor = DrawingColor.White;
            dgvReuniones.DefaultCellStyle.ForeColor = DrawingColor.FromArgb(35, 35, 35);
            dgvReuniones.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            dgvReuniones.DefaultCellStyle.SelectionBackColor = DrawingColor.FromArgb(245, 249, 255);
            dgvReuniones.DefaultCellStyle.SelectionForeColor = DrawingColor.FromArgb(35, 35, 35);
            dgvReuniones.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvReuniones.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgvReuniones.RowsDefaultCellStyle.BackColor = DrawingColor.White;
            dgvReuniones.AlternatingRowsDefaultCellStyle.BackColor = DrawingColor.FromArgb(248, 248, 248);
            dgvReuniones.RowTemplate.Height = 58;
            dgvReuniones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "No",
                HeaderText = "N°",
                DataPropertyName = "No",
                Width = 50
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ReunionID",
                DataPropertyName = "ReunionID",
                Visible = false
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                HeaderText = "FECHA Y HORA",
                DataPropertyName = "FechaHora",
                Width = 125
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHoraReal",
                DataPropertyName = "FechaHoraReal",
                Visible = false
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estudiante",
                HeaderText = "ESTUDIANTES",
                DataPropertyName = "Estudiante",
                Width = 165
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GradoSeccion",
                HeaderText = "GRADO Y SECCION",
                DataPropertyName = "GradoSeccion",
                Width = 130
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Tema",
                HeaderText = "TEMA",
                DataPropertyName = "Tema",
                Width = 145
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Medio",
                HeaderText = "MEDIO",
                DataPropertyName = "Medio",
                Width = 110
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                HeaderText = "ESTADO",
                DataPropertyName = "Estado",
                Width = 100
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Acciones",
                HeaderText = "ACCIONES",
                DataPropertyName = "Acciones",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvReuniones.Columns["No"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn col in dgvReuniones.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvReuniones.Columns["Tema"].DefaultCellStyle.ForeColor = System.Drawing.Color.DarkSlateGray;
            dgvReuniones.Columns["Tema"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        }

        private DataTable CrearEstructuraReuniones()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(int));
            dt.Columns.Add("ReunionID", typeof(int));
            dt.Columns.Add("FechaHora", typeof(string));
            dt.Columns.Add("FechaHoraReal", typeof(DateTime));
            dt.Columns.Add("Estudiante", typeof(string));
            dt.Columns.Add("GradoSeccion", typeof(string));
            dt.Columns.Add("Tema", typeof(string));
            dt.Columns.Add("Medio", typeof(string));
            dt.Columns.Add("Estado", typeof(string));
            dt.Columns.Add("Acciones", typeof(string));
            return dt;
        }

        #endregion

        #region COMBOS

        private void CargarEstados()
        {
            _cargandoCombos = true;

            DataTable dt = new DataTable();
            dt.Columns.Add("Valor", typeof(string));
            dt.Columns.Add("Texto", typeof(string));

            dt.Rows.Add("", "Todos");
            dt.Rows.Add("PROGRAMADA", "Programada");
            dt.Rows.Add("REALIZADA", "Realizada");
            dt.Rows.Add("CANCELADA", "Cancelada");

            cbEstado.DataSource = dt;
            cbEstado.ValueMember = "Valor";
            cbEstado.DisplayMember = "Texto";
            cbEstado.SelectedIndex = 0;

            _cargandoCombos = false;
        }

        private void CargarDocentes()
        {
            try
            {
                _cargandoCombos = true;

                DataTable dt = new DataTable();
                dt.Columns.Add("DocenteID", typeof(int));
                dt.Columns.Add("Nombre", typeof(string));

                dt.Rows.Add(0, "Todos");

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
                    SELECT DocenteID, Nombre
                    FROM Docente
                    ORDER BY Nombre;", cn);

                cn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    dt.Rows.Add(
                        Convert.ToInt32(dr["DocenteID"]),
                        dr["Nombre"].ToString()
                    );
                }

                cbDocente.DataSource = dt;
                cbDocente.ValueMember = "DocenteID";
                cbDocente.DisplayMember = "Nombre";
                cbDocente.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar docentes: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoCombos = false;
            }
        }

        private void CargarCiclosAcademicos()
        {
            try
            {
                _cargandoCombos = true;

                DataTable dt = new DataTable();
                dt.Columns.Add("AnioInicioCiclo", typeof(int));
                dt.Columns.Add("Texto", typeof(string));

                dt.Rows.Add(0, "Todos");

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
            SELECT DISTINCT
                CASE 
                    WHEN MONTH(FechaHora) >= 8 THEN YEAR(FechaHora)
                    ELSE YEAR(FechaHora) - 1
                END AS AnioInicioCiclo,
                CONCAT(
                    CASE 
                        WHEN MONTH(FechaHora) >= 8 THEN YEAR(FechaHora)
                        ELSE YEAR(FechaHora) - 1
                    END,
                    '-',
                    CASE 
                        WHEN MONTH(FechaHora) >= 8 THEN YEAR(FechaHora) + 1
                        ELSE YEAR(FechaHora)
                    END
                ) AS CicloTexto
            FROM Reunion
            ORDER BY AnioInicioCiclo DESC;", cn);

                cn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    dt.Rows.Add(
                        Convert.ToInt32(dr["AnioInicioCiclo"]),
                        dr["CicloTexto"].ToString()
                    );
                }

                cbCicloAcademico.DataSource = dt;
                cbCicloAcademico.ValueMember = "AnioInicioCiclo";
                cbCicloAcademico.DisplayMember = "Texto";
                cbCicloAcademico.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ciclos académicos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoCombos = false;
            }
        }

        private void CargarMesesDisponibles()
        {
            try
            {
                _cargandoCombos = true;

                DataTable dt = new DataTable();
                dt.Columns.Add("MesNumero", typeof(int));
                dt.Columns.Add("MesNombre", typeof(string));

                dt.Rows.Add(0, "Todos");

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
            SELECT DISTINCT
                MONTH(R.FechaHora) AS MesNumero
            FROM Reunion R
            INNER JOIN Estudiante E
                ON R.EstudianteID = E.EstudianteID
            INNER JOIN Matricula M
                ON M.EstudianteID = E.EstudianteID
               AND M.Anio = CASE 
                                WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora)
                                ELSE YEAR(R.FechaHora) - 1
                            END
            WHERE (@DocenteID = 0 OR R.DocenteID = @DocenteID)
              AND (
                    @AnioInicioCiclo = 0
                    OR
                    CASE 
                        WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora)
                        ELSE YEAR(R.FechaHora) - 1
                    END = @AnioInicioCiclo
                  )
            ORDER BY MesNumero;", cn);

                cmd.Parameters.AddWithValue("@DocenteID", ObtenerDocenteSeleccionado());
                cmd.Parameters.AddWithValue("@AnioInicioCiclo", ObtenerAnioInicioCicloSeleccionado());

                cn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    int mes = Convert.ToInt32(dr["MesNumero"]);
                    dt.Rows.Add(mes, ObtenerNombreMes(mes));
                }

                cbMes.DataSource = dt;
                cbMes.ValueMember = "MesNumero";
                cbMes.DisplayMember = "MesNombre";
                cbMes.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar meses: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoCombos = false;
            }
        }

        private int ObtenerAnioInicioCicloSeleccionado()
        {
            if (cbCicloAcademico.SelectedValue == null)
                return 0;

            return int.TryParse(cbCicloAcademico.SelectedValue.ToString(), out int valor) ? valor : 0;
        }

        private string ObtenerNombreMes(int mes)
        {
            return new CultureInfo("es-HN").DateTimeFormat.GetMonthName(mes).ToUpper();
        }

        private int ObtenerDocenteSeleccionado()
        {
            if (cbDocente.SelectedValue == null)
                return 0;

            return int.TryParse(cbDocente.SelectedValue.ToString(), out int valor) ? valor : 0;
        }

        private int ObtenerMesSeleccionado()
        {
            if (cbMes.SelectedValue == null)
                return 0;

            return int.TryParse(cbMes.SelectedValue.ToString(), out int valor) ? valor : 0;
        }

        private int ObtenerAnioSeleccionado()
        {
            if (cbCicloAcademico.SelectedValue == null)
                return 0;

            return int.TryParse(cbCicloAcademico.SelectedValue.ToString(), out int valor) ? valor : 0;
        }

        private string ObtenerEstadoSeleccionado()
        {
            return cbEstado.SelectedValue?.ToString() ?? "";
        }

        #endregion

        #region CARGA GRID

        private DataTable ObtenerReunionesDesdeFront()
        {
            DataTable dt = new DataTable();

            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand(@"
            SELECT
                R.ReunionID,
                R.FechaHora,
                E.Nombre,
                CONCAT(G.NombreGrado, ' ', S.Letra) AS GradoSeccion,
                R.Tema,
                R.MedioDifusion,
                R.Estado
            FROM Reunion R
            INNER JOIN Estudiante E
                ON R.EstudianteID = E.EstudianteID
            INNER JOIN Matricula M
                ON M.EstudianteID = E.EstudianteID
               AND M.Anio = CASE 
                                WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora)
                                ELSE YEAR(R.FechaHora) - 1
                            END
            INNER JOIN Seccion S
                ON M.SeccionID = S.SeccionID
            INNER JOIN Grado G
                ON S.GradoID = G.GradoID
            WHERE (@DocenteID = 0 OR R.DocenteID = @DocenteID)
              AND (@Mes = 0 OR MONTH(R.FechaHora) = @Mes)
              AND (
                    @AnioInicioCiclo = 0
                    OR
                    CASE 
                        WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora)
                        ELSE YEAR(R.FechaHora) - 1
                    END = @AnioInicioCiclo
                  )
              AND (@Estado = '' OR R.Estado = @Estado)
            ORDER BY R.FechaHora DESC, E.Nombre ASC;", cn);

            cmd.Parameters.AddWithValue("@DocenteID", ObtenerDocenteSeleccionado());
            cmd.Parameters.AddWithValue("@Mes", ObtenerMesSeleccionado());
            cmd.Parameters.AddWithValue("@AnioInicioCiclo", ObtenerAnioInicioCicloSeleccionado());
            cmd.Parameters.AddWithValue("@Estado", ObtenerEstadoSeleccionado());

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        private void CargarReuniones()
        {
            try
            {
                DataTable dtBD = ObtenerReunionesDesdeFront();
                _dtReunionesCompleto = CrearEstructuraReuniones();

                int correlativo = 1;

                foreach (DataRow row in dtBD.Rows)
                {
                    DateTime fechaReal = Convert.ToDateTime(row["FechaHora"]);
                    string estado = row["Estado"]?.ToString()?.Trim().ToUpper() ?? "";
                    string accion = ObtenerAccionSegunEstado(estado, fechaReal);

                    DataRow nueva = _dtReunionesCompleto.NewRow();
                    nueva["No"] = correlativo++;
                    nueva["ReunionID"] = Convert.ToInt32(row["ReunionID"]);
                    nueva["FechaHora"] = fechaReal.ToString("dd/MM hh:mm tt");
                    nueva["FechaHoraReal"] = fechaReal;
                    nueva["Estudiante"] = row["Nombre"]?.ToString() ?? "";
                    nueva["GradoSeccion"] = row["GradoSeccion"]?.ToString() ?? "";
                    nueva["Tema"] = row["Tema"]?.ToString() ?? "";
                    nueva["Medio"] = row["MedioDifusion"]?.ToString() ?? "";
                    nueva["Estado"] = estado;
                    nueva["Acciones"] = accion;

                    _dtReunionesCompleto.Rows.Add(nueva);
                }

                _paginaActual = 1;
                _textoBusquedaActual = txtBuscar.Text.Trim();
                AplicarFiltroBusqueda();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reuniones: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ObtenerAccionSegunEstado(string estado, DateTime fechaReunion)
        {
            return estado switch
            {
                "REALIZADA" => "PDF",
                "CANCELADA" => "--",
                "PROGRAMADA" when fechaReunion.Date <= DateTime.Today => "CREAR ACTA",
                "PROGRAMADA" => "--",
                _ => "--"
            };
        }

        #endregion

        #region FILTRO / BUSQUEDA / PAGINACION

        private void AplicarFiltroBusqueda()
        {
            try
            {
                if (_dtReunionesCompleto == null || _dtReunionesCompleto.Rows.Count == 0)
                {
                    _dtReunionesFiltrado = CrearEstructuraReuniones();
                    _totalRegistros = 0;
                    _totalPaginas = 1;
                    dgvReuniones.DataSource = null;
                    lblRegistros.Text = "Sin resultados";
                    ActualizarControlesPaginacion();
                    return;
                }

                if (string.IsNullOrWhiteSpace(_textoBusquedaActual))
                {
                    _dtReunionesFiltrado = _dtReunionesCompleto.Copy();
                }
                else
                {
                    string texto = _textoBusquedaActual.Trim().ToLower();

                    var filas = _dtReunionesCompleto.AsEnumerable()
                        .Where(r =>
                               (r["FechaHora"]?.ToString() ?? "").ToLower().Contains(texto)
                            || (r["Estudiante"]?.ToString() ?? "").ToLower().Contains(texto)
                            || (r["GradoSeccion"]?.ToString() ?? "").ToLower().Contains(texto)
                            || (r["Tema"]?.ToString() ?? "").ToLower().Contains(texto)
                            || (r["Medio"]?.ToString() ?? "").ToLower().Contains(texto)
                            || (r["Estado"]?.ToString() ?? "").ToLower().Contains(texto));

                    _dtReunionesFiltrado = filas.Any()
                        ? filas.CopyToDataTable()
                        : CrearEstructuraReuniones();
                }

                _totalRegistros = _dtReunionesFiltrado.Rows.Count;
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
            if (_dtReunionesFiltrado == null)
                return;

            DataTable dtPagina = _dtReunionesFiltrado.Clone();

            var filasPagina = _dtReunionesFiltrado.AsEnumerable()
                .Skip((_paginaActual - 1) * _tamanoPagina)
                .Take(_tamanoPagina);

            foreach (var fila in filasPagina)
                dtPagina.ImportRow(fila);

            dgvReuniones.SuspendLayout();
            dgvReuniones.DataSource = null;
            dgvReuniones.Rows.Clear();
            dgvReuniones.DataSource = dtPagina;
            dgvReuniones.ClearSelection();
            dgvReuniones.ResumeLayout();

            AjustarAlturaFilas();
            ActualizarTextoRegistros();
            ActualizarControlesPaginacion();
            dgvReuniones.Refresh();
        }

        private void AjustarAlturaFilas()
        {
            foreach (DataGridViewRow row in dgvReuniones.Rows)
                row.Height = 58;
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

            lblAnterior.ForeColor = lblAnterior.Enabled ? System.Drawing.Color.FromArgb(93, 93, 93) : System.Drawing.Color.LightGray;
            lblSiguiente.ForeColor = lblSiguiente.Enabled ? System.Drawing.Color.FromArgb(93, 93, 93) : System.Drawing.Color.LightGray;
        }

        #endregion

        #region EVENTOS
        private void dgvReuniones_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                dgvReuniones.Cursor = Cursors.Default;
                return;
            }

            string nombreColumna = dgvReuniones.Columns[e.ColumnIndex].Name;

            if (nombreColumna == "Acciones")
            {
                string accion = dgvReuniones.Rows[e.RowIndex].Cells["Acciones"].Value?.ToString() ?? "";
                dgvReuniones.Cursor = accion != "--" ? Cursors.Hand : Cursors.Default;
            }
            else
            {
                dgvReuniones.Cursor = Cursors.Hand;
            }
        }

        private void dgvReuniones_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvReuniones.Cursor = Cursors.Default;
        }
        private void cbDocente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;
            _paginaActual = 1;
            CargarMesesDisponibles();
            CargarReuniones();
        }

        private void cbMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;
            _paginaActual = 1;
            CargarReuniones();
        }

        private void cbEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;
            _paginaActual = 1;
            CargarReuniones();
        }

        private void cbCicloAcademico_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoCombos) return;
            _paginaActual = 1;
            CargarMesesDisponibles();
            CargarReuniones();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            _textoBusquedaActual = txtBuscar.Text.Trim();
            _paginaActual = 1;
            AplicarFiltroBusqueda();
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

        private void btnNuevaActividad_Click(object sender, EventArgs e)
        {
            using FrmGestionReunionesNueva frm = new FrmGestionReunionesNueva();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                CargarMesesDisponibles();
                CargarReuniones();
            }
        }

        #endregion

        #region CELL PAINTING

        private void dgvReuniones_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string colName = dgvReuniones.Columns[e.ColumnIndex].Name;

            if (colName == "Estado")
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                string estado = e.FormattedValue?.ToString()?.ToUpper() ?? "";

                DrawingColor backColor = DrawingColor.FromArgb(245, 245, 245);
                DrawingColor foreColor = DrawingColor.FromArgb(90, 90, 90);

                if (estado == "REALIZADA")
                {
                    backColor = DrawingColor.FromArgb(220, 248, 228);
                    foreColor = DrawingColor.FromArgb(22, 163, 74);
                }
                else if (estado == "PROGRAMADA")
                {
                    backColor = DrawingColor.FromArgb(255, 243, 205);
                    foreColor = DrawingColor.FromArgb(180, 125, 0);
                }
                else if (estado == "CANCELADA")
                {
                    backColor = DrawingColor.FromArgb(255, 230, 230);
                    foreColor = DrawingColor.FromArgb(239, 68, 68);
                }

                Rectangle pillRect = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + 11,
                    e.CellBounds.Width - 20,
                    e.CellBounds.Height - 22
                );

                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillEllipse(brush, pillRect.X, pillRect.Y, 18, pillRect.Height);
                    e.Graphics.FillEllipse(brush, pillRect.Right - 18, pillRect.Y, 18, pillRect.Height);
                    e.Graphics.FillRectangle(brush, pillRect.X + 9, pillRect.Y, pillRect.Width - 18, pillRect.Height);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    estado.Length > 10 ? estado.Substring(0, 8) + "..." : estado,
                    new Font("Segoe UI", 8.5F, FontStyle.Regular),
                    pillRect,
                    foreColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                using Pen pen = new Pen(dgvReuniones.GridColor);
                e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
            }
            else if (colName == "Acciones")
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                string texto = e.FormattedValue?.ToString() ?? "";

                Rectangle btnRect = new Rectangle(
                    e.CellBounds.X + 10,
                    e.CellBounds.Y + 6,
                    e.CellBounds.Width - 20,
                    e.CellBounds.Height - 12
                );

                DrawingColor btnBack = DrawingColor.FromArgb(245, 245, 245);
                DrawingColor btnBorder = DrawingColor.FromArgb(210, 210, 210);
                DrawingColor btnText = DrawingColor.FromArgb(90, 90, 90);

                using (GraphicsPath path = RedondearRectangulo(btnRect, 5))
                using (SolidBrush brush = new SolidBrush(btnBack))
                using (Pen pen = new Pen(btnBorder))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    texto,
                    new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    btnRect,
                    btnText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                using Pen pen2 = new Pen(dgvReuniones.GridColor);
                e.Graphics.DrawLine(pen2, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
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

        #endregion

        #region CELL CLICK

        private void dgvReuniones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string nombreColumna = dgvReuniones.Columns[e.ColumnIndex].Name;
            int reunionId = Convert.ToInt32(dgvReuniones.Rows[e.RowIndex].Cells["ReunionID"].Value ?? 0);

            if (nombreColumna == "Acciones")
            {
                string accion = dgvReuniones.Rows[e.RowIndex].Cells["Acciones"].Value?.ToString() ?? "";

                if (accion == "CREAR ACTA")
                {
                    using FrmGestionReunionesCrearActa frm = new FrmGestionReunionesCrearActa(reunionId);
                    if (frm.ShowDialog() == DialogResult.OK)
                        CargarReuniones();
                }
                else if (accion == "PDF")
                {
                    string fechaHora = dgvReuniones.Rows[e.RowIndex].Cells["FechaHora"].Value?.ToString() ?? "";
                    string estudiante = dgvReuniones.Rows[e.RowIndex].Cells["Estudiante"].Value?.ToString() ?? "";
                    string gradoSeccion = dgvReuniones.Rows[e.RowIndex].Cells["GradoSeccion"].Value?.ToString() ?? "";
                    string tema = dgvReuniones.Rows[e.RowIndex].Cells["Tema"].Value?.ToString() ?? "";
                    string medio = dgvReuniones.Rows[e.RowIndex].Cells["Medio"].Value?.ToString() ?? "";
                    string estado = dgvReuniones.Rows[e.RowIndex].Cells["Estado"].Value?.ToString() ?? "";

                    string docente = ObtenerDocentePorReunionId(reunionId);
                    string anio = cbCicloAcademico.Text.Trim();

                    if (string.IsNullOrWhiteSpace(docente))
                    {
                        MessageBox.Show("No se pudo obtener el docente de la reunión.",
                            "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    GenerarPdfActa(
                        docente,
                        estudiante,
                        fechaHora,
                        gradoSeccion,
                        tema,
                        medio,
                        estado,
                        anio
                    );
                }

                return;
            }

            MostrarDetalleReunion(reunionId);
        }

        private void MostrarDetalleReunion(int reunionId)
        {
            try
            {
                DataTable dt = ObtenerDetalleReunion(reunionId);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontró información de la reunión.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataRow row = dt.Rows[0];

                using FrmGestionReunionesDetalle frm = new FrmGestionReunionesDetalle(
                    row["FechaHoraTexto"]?.ToString() ?? "",
                    row["Docente"]?.ToString() ?? "",
                    row["Estudiante"]?.ToString() ?? "",
                    row["GradoSeccion"]?.ToString() ?? "",
                    row["Tema"]?.ToString() ?? "",
                    row["Medio"]?.ToString() ?? "",
                    row["Estado"]?.ToString() ?? "",
                    TipoVistaDetalleReunion.Admin
                );

                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar detalle de la reunión: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable ObtenerDetalleReunion(int reunionId)
        {
            DataTable dt = new DataTable();

            using SqlConnection cn = conexion.ObtenerConexion();
            using SqlCommand cmd = new SqlCommand(@"
            SELECT
                R.ReunionID,
                FORMAT(R.FechaHora, 'dd/MM/yyyy hh:mm tt', 'es-HN') AS FechaHoraTexto,
                D.Nombre AS Docente,
                E.Nombre AS Estudiante,
                CONCAT(G.NombreGrado, ' ', S.Letra) AS GradoSeccion,
                R.Tema,
                R.MedioDifusion AS Medio,
                R.Estado
            FROM Reunion R
            INNER JOIN Docente D
                ON R.DocenteID = D.DocenteID
            INNER JOIN Estudiante E
                ON R.EstudianteID = E.EstudianteID
            INNER JOIN Matricula M
                ON M.EstudianteID = E.EstudianteID
               AND M.Anio = CASE 
                                WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora)
                                ELSE YEAR(R.FechaHora) - 1
                            END
            INNER JOIN Seccion S
                ON M.SeccionID = S.SeccionID
            INNER JOIN Grado G
                ON S.GradoID = G.GradoID
            WHERE R.ReunionID = @ReunionID;", cn);

            cmd.Parameters.AddWithValue("@ReunionID", reunionId);

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        #endregion

        #region pdf
        private ImageData ObtenerLogoDesdeResources()
        {
            using MemoryStream ms = new MemoryStream();

            GestionAcademicaV2.Properties.Resources.Logo_expandido.Save(ms, ImageFormat.Png);

            return ImageDataFactory.Create(ms.ToArray());
        }
        private string ObtenerDocentePorReunionId(int reunionId)
        {
            try
            {
                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
            SELECT D.Nombre
            FROM Reunion R
            INNER JOIN Docente D
                ON R.DocenteID = D.DocenteID
            WHERE R.ReunionID = @ReunionID;", cn);

                cmd.Parameters.AddWithValue("@ReunionID", reunionId);

                cn.Open();

                object resultado = cmd.ExecuteScalar();
                return resultado?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener docente de la reunión: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }
        private void GenerarPdfActa(
                string docente,
                string estudiante,
                string fechaHora,
                string gradoSeccion,
                string tema,
                string medio,
                string estado,
                string anio)
        {
            using SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Guardar acta en PDF";
            sfd.Filter = "Archivo PDF (*.pdf)|*.pdf";
            sfd.FileName = $"Acta_{estudiante.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf";
            sfd.InitialDirectory = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads"
            );

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                using PdfWriter writer = new PdfWriter(sfd.FileName);
                using PdfDocument pdf = new PdfDocument(writer);
                using Document doc = new Document(pdf);

                doc.SetMargins(30, 35, 30, 35);

                PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                PdfColor azulTitulo = new DeviceRgb(35, 92, 255);
                PdfColor azulOscuro = new DeviceRgb(30, 50, 100);
                PdfColor grisTexto = new DeviceRgb(60, 60, 60);
                PdfColor grisBorde = new DeviceRgb(210, 210, 210);
                PdfColor verde = new DeviceRgb(34, 139, 34);
                PdfColor amarillo = new DeviceRgb(180, 125, 0);
                PdfColor rojo = new DeviceRgb(200, 60, 60);

                // =========================
                // ENCABEZADO
                // =========================
                Table encabezado = new Table(UnitValue.CreatePercentArray(new float[] { 1.25f, 4.75f }))
                    .UseAllAvailableWidth();
                encabezado.SetBorder(Border.NO_BORDER);
                encabezado.SetMarginBottom(8);

                Cell celdaLogo = new Cell().SetBorder(Border.NO_BORDER);
                try
                {
                    iText.Layout.Element.Image logo = new iText.Layout.Element.Image(ObtenerLogoDesdeResources())
                        .ScaleToFit(85, 85)
                        .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT);

                    celdaLogo.Add(logo);
                }
                catch
                {
                    celdaLogo.Add(new Paragraph("").SetFont(regularFont));
                }

                Cell celdaTexto = new Cell().SetBorder(Border.NO_BORDER);
                celdaTexto.Add(
                    new Paragraph("ATLANTIC ACADEMY BILINGUAL SCHOOL")
                        .SetFont(boldFont)
                        .SetFontSize(14)
                        .SetFontColor(azulOscuro)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(4)
                );

                celdaTexto.Add(
                    new Paragraph("ACTA DE REUNIÓN CON PADRE/MADRE DE FAMILIA")
                        .SetFont(boldFont)
                        .SetFontSize(17)
                        .SetFontColor(azulTitulo)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(6)
                        .SetMarginTop(2)
                );

                celdaTexto.Add(
                    new Paragraph($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy}")
                        .SetFont(regularFont)
                        .SetFontSize(9)
                        .SetFontColor(new DeviceRgb(100, 100, 100))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginTop(2)
                );

                celdaTexto.Add(
                    new Paragraph($"Control mensual de reuniones - Año académico {anio}")
                        .SetFont(regularFont)
                        .SetFontSize(10)
                        .SetFontColor(grisTexto)
                        .SetTextAlignment(TextAlignment.CENTER)
                );

                encabezado.AddCell(celdaLogo);
                encabezado.AddCell(celdaTexto);

                doc.Add(encabezado);
                doc.Add(new LineSeparator(new SolidLine(1f)).SetMarginBottom(14));

                // =========================
                // TABLA DE INFORMACIÓN
                // =========================
                Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 2f, 4f }))
                    .UseAllAvailableWidth();
                tabla.SetMarginBottom(18);

                void AddRow(string etiqueta, string valor, bool resaltar = false)
                {
                    tabla.AddCell(
                        new Cell()
                            .Add(
                                new Paragraph(etiqueta)
                                    .SetFont(boldFont)
                                    .SetFontSize(10)
                                    .SetFontColor(grisTexto)
                            )
                            .SetBackgroundColor(new DeviceRgb(240, 240, 240))
                            .SetPaddingTop(7)
                            .SetPaddingBottom(7)
                            .SetPaddingLeft(8)
                            .SetPaddingRight(8)
                            .SetBorder(new SolidBorder(grisBorde, 1))
                    );

                    Paragraph pValor = new Paragraph(valor)
                        .SetFont(regularFont)
                        .SetFontSize(10)
                        .SetFontColor(grisTexto);

                    if (resaltar)
                    {
                        PdfColor colorEstado = estado.ToUpper() switch
                        {
                            "REALIZADA" => verde,
                            "PROGRAMADA" => amarillo,
                            "CANCELADA" => rojo,
                            _ => grisTexto
                        };

                        pValor.SetFont(boldFont);
                        pValor.SetFontColor(colorEstado);
                    }

                    tabla.AddCell(
                        new Cell()
                            .Add(pValor)
                            .SetPaddingTop(7)
                            .SetPaddingBottom(7)
                            .SetPaddingLeft(8)
                            .SetPaddingRight(8)
                            .SetBorder(new SolidBorder(grisBorde, 1))
                    );
                }

                AddRow("Docente", docente);
                AddRow("Estudiante", estudiante);
                AddRow("Fecha y hora", fechaHora);
                AddRow("Grado y sección", gradoSeccion);
                AddRow("Tema", tema);
                AddRow("Medio", medio);
                AddRow("Estado", estado, true);

                doc.Add(tabla);

                // =========================
                // DETALLE DEL ACTA
                // =========================
                doc.Add(
                    new Paragraph("Detalle del acta")
                        .SetFont(boldFont)
                        .SetFontSize(12)
                        .SetFontColor(azulOscuro)
                        .SetMarginBottom(8)
                );

                string detalle =
                    $"En la fecha {fechaHora}, el docente {docente} sostuvo una reunión con el responsable del estudiante {estudiante}, " +
                    $"perteneciente a {gradoSeccion}. El tema tratado fue \"{tema}\" y el medio de comunicación utilizado fue {medio.ToLower()}. " +
                    $"El estado de la reunión se registra como {estado.ToLower()}.";

                doc.Add(
                    new Paragraph(detalle)
                        .SetFont(regularFont)
                        .SetFontSize(11)
                        .SetFontColor(grisTexto)
                        .SetTextAlignment(TextAlignment.JUSTIFIED)
                        .SetMultipliedLeading(1.35f)
                        .SetMarginBottom(34)
                );

                // =========================
                // FIRMAS
                // =========================
                Table firmas = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 }))
                    .UseAllAvailableWidth();

                firmas.SetMarginTop(12);

                firmas.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph("____________________________\nDocente")
                                .SetFont(regularFont)
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.CENTER)
                        )
                        .SetBorder(Border.NO_BORDER)
                        .SetPaddingTop(20)
                );

                firmas.AddCell(
                    new Cell()
                        .Add(
                            new Paragraph("____________________________\nPadre / Madre / Encargado")
                                .SetFont(regularFont)
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.CENTER)
                        )
                        .SetBorder(Border.NO_BORDER)
                        .SetPaddingTop(20)
                );

                doc.Add(firmas);

                doc.ShowTextAligned(
                    new Paragraph(
                        "Sistema de Gestión Académica MAE\n" +
                        $"Generado: {DateTime.Now:dd/MM/yyyy}\n" +
                        "Página 1 de 1")
                        .SetFont(regularFont)
                        .SetFontSize(8)
                        .SetFontColor(new DeviceRgb(110, 110, 110)),
                    35, 25,
                    TextAlignment.LEFT
                );

                MessageBox.Show(
                    "PDF generado correctamente.",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al generar el PDF:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        #endregion
    }
}