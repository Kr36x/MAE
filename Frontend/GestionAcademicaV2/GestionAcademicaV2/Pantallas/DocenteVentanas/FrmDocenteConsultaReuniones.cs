using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using GestionAcademicaV2.Pantallas.AdminVentanas;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmDocenteConsultaReuniones : Form
    {
        private readonly int docenteId;
        private readonly Conexion conexion = new Conexion();

        private bool _cargandoCombos = false;

        private DataTable _dtReunionesCompleto = new DataTable();
        private DataTable _dtReunionesFiltrado = new DataTable();

        private int _paginaActual = 1;
        private int _tamanoPagina = 5;
        private int _totalRegistros = 0;
        private int _totalPaginas = 1;

        private string _textoBusquedaActual = "";

        public FrmDocenteConsultaReuniones(int docenteId)
        {
            InitializeComponent();
            this.docenteId = docenteId;

            Load += FrmVerReunionesDocente_Load;

            cbMes.SelectedIndexChanged += cbMes_SelectedIndexChanged;
            cbCicloAcademico.SelectedIndexChanged += cbCicloAcademico_SelectedIndexChanged;

            txtBuscar.TextChanged += txtBuscar_TextChanged;
            btnBuscar.Click += btnBuscar_Click;

            lblAnterior.Click += lblAnterior_Click;
            lblSiguiente.Click += lblSiguiente_Click;

            dgvReuniones.CellPainting += dgvReuniones_CellPainting;

            dgvReuniones.CellClick += dgvReuniones_CellClick;
            dgvReuniones.CellMouseEnter += dgvReuniones_CellMouseEnter;
            dgvReuniones.CellMouseLeave += dgvReuniones_CellMouseLeave;

            dgvReuniones.Paint += dgvReuniones_Paint;
        }
        private void dgvReuniones_Paint(object sender, PaintEventArgs e)
        {
            if (dgvReuniones.Rows.Count == 0)
            {
                string mensaje = "No hay reuniones programadas para mostrar.";

                using Font font = new Font("Segoe UI", 12F, FontStyle.Bold);
                using SolidBrush brush = new SolidBrush(Color.FromArgb(120, 120, 120));

                SizeF size = e.Graphics.MeasureString(mensaje, font);

                float x = (dgvReuniones.Width - size.Width) / 2;
                float y = (dgvReuniones.Height - size.Height) / 2;

                e.Graphics.DrawString(mensaje, font, brush, x, y);
            }
        }
        private void FrmVerReunionesDocente_Load(object sender, EventArgs e)
        {
            ConfigurarCombos();
            ConfigurarGrid();
            CargarDocenteActual();
            CargarEstados();
            CargarCiclosAcademicos();
            CargarMesesDisponibles();
            CargarReuniones();

            txtBuscar.PlaceholderText = "Buscar por estudiante, grado, tema, medio o estado";
            btnTexto.Text = "1";
        }

        #region CONFIGURACION

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
            dgvReuniones.BackgroundColor = Color.White;
            dgvReuniones.GridColor = Color.FromArgb(220, 220, 220);

            dgvReuniones.ColumnHeadersHeight = 56;
            dgvReuniones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvReuniones.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 0);
            dgvReuniones.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReuniones.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvReuniones.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvReuniones.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 102, 0);
            dgvReuniones.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            dgvReuniones.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);

            dgvReuniones.DefaultCellStyle.BackColor = Color.White;
            dgvReuniones.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgvReuniones.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            dgvReuniones.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 245, 220);
            dgvReuniones.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);
            dgvReuniones.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvReuniones.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgvReuniones.RowsDefaultCellStyle.BackColor = Color.White;
            dgvReuniones.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 250, 245);
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
                HeaderText = "FECHA Y\nHORA",
                DataPropertyName = "FechaHora",
                Width = 120
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estudiante",
                HeaderText = "ESTUDIANTES",
                DataPropertyName = "Estudiante",
                Width = 180
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GradoSeccion",
                HeaderText = "GRADO Y\nSECCION",
                DataPropertyName = "GradoSeccion",
                Width = 150
            });

            dgvReuniones.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Tema",
                HeaderText = "TEMA",
                DataPropertyName = "Tema",
                Width = 130
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
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvReuniones.Columns["No"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn col in dgvReuniones.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private DataTable CrearEstructuraReuniones()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(int));
            dt.Columns.Add("ReunionID", typeof(int));
            dt.Columns.Add("FechaHora", typeof(string));
            dt.Columns.Add("Estudiante", typeof(string));
            dt.Columns.Add("GradoSeccion", typeof(string));
            dt.Columns.Add("Tema", typeof(string));
            dt.Columns.Add("Medio", typeof(string));
            dt.Columns.Add("Estado", typeof(string));
            return dt;
        }

        #endregion

        #region CARGA COMBOS

        private void CargarDocenteActual()
        {
            try
            {
                _cargandoCombos = true;

                DataTable dt = new DataTable();

                using SqlConnection cn = conexion.ObtenerConexion();
                using SqlCommand cmd = new SqlCommand(@"
                    SELECT DocenteID, Nombre
                    FROM Docente
                    WHERE DocenteID = @DocenteID;", cn);

                cmd.Parameters.AddWithValue("@DocenteID", docenteId);

                using SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                cbDocente.DataSource = dt;
                cbDocente.ValueMember = "DocenteID";
                cbDocente.DisplayMember = "Nombre";
                cbDocente.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar docente: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cargandoCombos = false;
            }
        }

        private void CargarEstados()
        {
            _cargandoCombos = true;

            DataTable dt = new DataTable();
            dt.Columns.Add("Valor", typeof(string));
            dt.Columns.Add("Texto", typeof(string));

            dt.Rows.Add("PROGRAMADA", "Programada");

            cbEstado.DataSource = dt;
            cbEstado.ValueMember = "Valor";
            cbEstado.DisplayMember = "Texto";
            cbEstado.SelectedIndex = 0;
            cbEstado.Enabled = false;

            _cargandoCombos = false;
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
                            WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora)
                            ELSE YEAR(R.FechaHora) - 1
                        END AS AnioInicioCiclo,
                        CONCAT(
                            CASE 
                                WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora)
                                ELSE YEAR(R.FechaHora) - 1
                            END,
                            '-',
                            CASE 
                                WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora) + 1
                                ELSE YEAR(R.FechaHora)
                            END
                        ) AS CicloTexto
                    FROM Reunion R
                    WHERE R.DocenteID = @DocenteID
                      AND R.Estado = 'PROGRAMADA'
                    ORDER BY AnioInicioCiclo DESC;", cn);

                cmd.Parameters.AddWithValue("@DocenteID", docenteId);

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
                    WHERE R.DocenteID = @DocenteID
                      AND R.Estado = 'PROGRAMADA'
                      AND (
                            @AnioInicioCiclo = 0
                            OR
                            CASE 
                                WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora)
                                ELSE YEAR(R.FechaHora) - 1
                            END = @AnioInicioCiclo
                          )
                    ORDER BY MesNumero;", cn);

                cmd.Parameters.AddWithValue("@DocenteID", docenteId);
                cmd.Parameters.AddWithValue("@AnioInicioCiclo", ObtenerAnioSeleccionado());

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

        private string ObtenerNombreMes(int mes)
        {
            CultureInfo cultura = new CultureInfo("es-HN");
            return cultura.DateTimeFormat.GetMonthName(mes).ToUpper();
        }

        private int ObtenerAnioSeleccionado()
        {
            if (cbCicloAcademico.SelectedValue == null)
                return 0;

            return int.TryParse(cbCicloAcademico.SelectedValue.ToString(), out int anio) ? anio : 0;
        }

        private int ObtenerMesSeleccionado()
        {
            if (cbMes.SelectedValue == null)
                return 0;

            return int.TryParse(cbMes.SelectedValue.ToString(), out int mes) ? mes : 0;
        }

        #endregion

        #region CARGA REUNIONES

        private DataTable ObtenerReunionesDesdeBD()
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
                WHERE R.DocenteID = @DocenteID
                  AND R.Estado = 'PROGRAMADA'
                  AND (@Mes = 0 OR MONTH(R.FechaHora) = @Mes)
                  AND (
                        @AnioInicioCiclo = 0
                        OR
                        CASE 
                            WHEN MONTH(R.FechaHora) >= 8 THEN YEAR(R.FechaHora)
                            ELSE YEAR(R.FechaHora) - 1
                        END = @AnioInicioCiclo
                      )
                ORDER BY R.FechaHora DESC, E.Nombre ASC;", cn);

            cmd.Parameters.AddWithValue("@DocenteID", docenteId);
            cmd.Parameters.AddWithValue("@Mes", ObtenerMesSeleccionado());
            cmd.Parameters.AddWithValue("@AnioInicioCiclo", ObtenerAnioSeleccionado());

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        private void CargarReuniones()
        {
            try
            {
                DataTable dtBD = ObtenerReunionesDesdeBD();
                _dtReunionesCompleto = CrearEstructuraReuniones();

                int correlativo = 1;

                foreach (DataRow row in dtBD.Rows)
                {
                    string estado = row["Estado"]?.ToString()?.Trim().ToUpper() ?? "";

                    string fechaTexto = "";
                    if (row["FechaHora"] != DBNull.Value)
                    {
                        DateTime fecha = Convert.ToDateTime(row["FechaHora"]);
                        fechaTexto = fecha.ToString("dd/MM hh:mm tt");
                    }

                    DataRow nueva = _dtReunionesCompleto.NewRow();
                    nueva["No"] = correlativo++;
                    nueva["ReunionID"] = row.Table.Columns.Contains("ReunionID")
                        ? Convert.ToInt32(row["ReunionID"])
                        : 0;
                    nueva["FechaHora"] = fechaTexto;
                    nueva["Estudiante"] = row["Nombre"]?.ToString() ?? "";
                    nueva["GradoSeccion"] = row["GradoSeccion"]?.ToString() ?? "";
                    nueva["Tema"] = row["Tema"]?.ToString() ?? "";
                    nueva["Medio"] = row["MedioDifusion"]?.ToString() ?? "";
                    nueva["Estado"] = estado;

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

        #endregion

        #region FILTRO Y PAGINACION

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
            dgvReuniones.Invalidate();
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

            lblAnterior.ForeColor = lblAnterior.Enabled
                ? Color.FromArgb(93, 93, 93)
                : Color.LightGray;

            lblSiguiente.ForeColor = lblSiguiente.Enabled
                ? Color.FromArgb(93, 93, 93)
                : Color.LightGray;
        }

        #endregion

        #region EVENTOS

        private void cbMes_SelectedIndexChanged(object sender, EventArgs e)
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
        private void dgvReuniones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string fechaHora = dgvReuniones.Rows[e.RowIndex].Cells["FechaHora"].Value?.ToString() ?? "";
            string estudiante = dgvReuniones.Rows[e.RowIndex].Cells["Estudiante"].Value?.ToString() ?? "";
            string gradoSeccion = dgvReuniones.Rows[e.RowIndex].Cells["GradoSeccion"].Value?.ToString() ?? "";
            string tema = dgvReuniones.Rows[e.RowIndex].Cells["Tema"].Value?.ToString() ?? "";
            string medio = dgvReuniones.Rows[e.RowIndex].Cells["Medio"].Value?.ToString() ?? "";
            string estado = dgvReuniones.Rows[e.RowIndex].Cells["Estado"].Value?.ToString() ?? "";
            string docente = cbDocente.Text.Trim();

            using FrmGestionReunionesDetalle frm = new FrmGestionReunionesDetalle(
                fechaHora,
                docente,
                estudiante,
                gradoSeccion,
                tema,
                medio,
                estado,
                TipoVistaDetalleReunion.Docente
            );

            frm.ShowDialog();
        }

        private void dgvReuniones_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                dgvReuniones.Cursor = Cursors.Default;
                return;
            }

            dgvReuniones.Cursor = Cursors.Hand;
        }

        private void dgvReuniones_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvReuniones.Cursor = Cursors.Default;
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

        #endregion

        #region PINTADO GRID

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

                Color backColor = Color.FromArgb(245, 245, 245);
                Color foreColor = Color.FromArgb(90, 90, 90);

                if (estado == "PROGRAMADA")
                {
                    backColor = Color.FromArgb(255, 243, 205);
                    foreColor = Color.FromArgb(180, 125, 0);
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
                    estado,
                    new Font("Segoe UI", 8.5F, FontStyle.Regular),
                    pillRect,
                    foreColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                using Pen pen = new Pen(dgvReuniones.GridColor);
                e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
            }
        }

        #endregion

        private void guna2Panel6_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}