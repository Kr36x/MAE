using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmDocenteReporteSemanalCalificaciones : Form
    {
        private readonly Conexion conexion = new Conexion();
        private readonly int docenteId;
        private string _mensajeSinDatos = "";
        public FrmDocenteReporteSemanalCalificaciones(int docenteId)
        {
            InitializeComponent();
            this.docenteId = docenteId;
        }

        private void FrmReporteSemanal_Load(object sender, EventArgs e)
        {
            CargarGrados();

            cbParcial.Items.Clear();
            cbParcial.Items.Add("1");
            cbParcial.Items.Add("2");
            cbParcial.Items.Add("3");
            cbParcial.Items.Add("4");
            cbParcial.SelectedIndex = 0;

            cbAsignatura.DrawMode = DrawMode.Normal;
            cbAsignatura.DropDownStyle = ComboBoxStyle.DropDownList;
            cbAsignatura.BackColor = Color.White;
            cbAsignatura.ForeColor = Color.Black;

            lblAsignatura.AutoSize = false;
            lblAsignatura.Location = new Point(0, 0);
            lblAsignatura.Size = plVerder.ClientSize;
            lblAsignatura.TextAlignment = ContentAlignment.MiddleCenter;
            lblAsignatura.Margin = new Padding(0);
            lblAsignatura.Padding = new Padding(0);
            dgvNotas.Paint += dgvNotas_Paint;

            ConfigurarDatePickerSemana();
        }
        private void dgvNotas_Paint(object sender, PaintEventArgs e)
        {
            if (dgvNotas.Rows.Count == 0 && !string.IsNullOrWhiteSpace(_mensajeSinDatos))
            {
                using Font tituloFont = new Font("Segoe UI", 12F, FontStyle.Bold);
                using Font mensajeFont = new Font("Segoe UI", 10.5F, FontStyle.Regular);

                using SolidBrush tituloBrush = new SolidBrush(Color.DarkGreen);
                using SolidBrush mensajeBrush = new SolidBrush(Color.FromArgb(110, 110, 110));

                string titulo = "INFORMACIÓN";
                string mensaje = _mensajeSinDatos;

                Rectangle rect = dgvNotas.ClientRectangle;

                SizeF sizeTitulo = e.Graphics.MeasureString(titulo, tituloFont);
                SizeF sizeMensaje = e.Graphics.MeasureString(mensaje, mensajeFont);

                float xTitulo = (rect.Width - sizeTitulo.Width) / 2;
                float xMensaje = (rect.Width - sizeMensaje.Width) / 2;

                float yTitulo = (rect.Height / 2f) - 30;
                float yMensaje = yTitulo + 28;

                e.Graphics.DrawString(titulo, tituloFont, tituloBrush, xTitulo, yTitulo);
                e.Graphics.DrawString(mensaje, mensajeFont, mensajeBrush, xMensaje, yMensaje);
            }
        }

        private void ConfigurarDatePickerSemana()
        {
            dtpSemana.Format = DateTimePickerFormat.Short;

            DateTime hoy = DateTime.Today;
            DateTime lunes = ObtenerLunesDeLaSemana(hoy);

            dtpSemana.ValueChanged -= dtpSemana_ValueChanged;
            dtpSemana.Value = lunes;
            dtpSemana.ValueChanged += dtpSemana_ValueChanged;
        }

        private DateTime ObtenerLunesDeLaSemana(DateTime fecha)
        {
            int diferencia = fecha.DayOfWeek == DayOfWeek.Sunday
                ? 6
                : ((int)fecha.DayOfWeek - 1);

            return fecha.Date.AddDays(-diferencia);
        }

        private void dtpSemana_ValueChanged(object sender, EventArgs e)
        {
            DateTime lunes = ObtenerLunesDeLaSemana(dtpSemana.Value);

            if (dtpSemana.Value.Date != lunes.Date)
            {
                dtpSemana.ValueChanged -= dtpSemana_ValueChanged;
                dtpSemana.Value = lunes;
                dtpSemana.ValueChanged += dtpSemana_ValueChanged;
            }

            CargarReporte();
        }

        private void CargarGrados()
        {
            cbGrado.Items.Clear();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                select distinct G.NombreGrado
                from CargaAcademica CA
                inner join Seccion S on CA.SeccionID = S.SeccionID
                inner join Grado G on S.GradoID = G.GradoID
                inner join Docente D on CA.DocenteID = D.DocenteID
                where D.UsuarioID = @Docente
                order by G.NombreGrado", cn))
            {
                cmd.Parameters.AddWithValue("@Docente", docenteId);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cbGrado.Items.Add(dr["NombreGrado"].ToString());
                    }
                }
            }

            if (cbGrado.Items.Count > 0)
                cbGrado.SelectedIndex = 0;
        }

        private void CargarSecciones()
        {
            cbSeccion.Items.Clear();

            if (cbGrado.SelectedItem == null)
                return;

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                select distinct S.Letra
                from CargaAcademica CA
                inner join Seccion S on CA.SeccionID = S.SeccionID
                inner join Grado G on S.GradoID = G.GradoID
                inner join Docente D on CA.DocenteID = D.DocenteID
                where D.UsuarioID = @Docente
                  and G.NombreGrado = @Grado
                order by S.Letra", cn))
            {
                cmd.Parameters.AddWithValue("@Docente", docenteId);
                cmd.Parameters.AddWithValue("@Grado", cbGrado.SelectedItem.ToString());

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cbSeccion.Items.Add(dr["Letra"].ToString());
                    }
                }
            }

            if (cbSeccion.Items.Count > 0)
                cbSeccion.SelectedIndex = 0;
        }

        private void AjustarAnchoCombo(ComboBox combo)
        {
            int maxWidth = combo.DropDownWidth;

            using (Graphics g = combo.CreateGraphics())
            {
                foreach (var item in combo.Items)
                {
                    int width = (int)g.MeasureString(item.ToString(), combo.Font).Width;

                    if (width > maxWidth)
                        maxWidth = width;
                }
            }

            combo.DropDownWidth = maxWidth + 50;
        }

        private void CargarAsignaturas()
        {
            cbAsignatura.Items.Clear();

            if (cbGrado.SelectedItem == null || cbSeccion.SelectedItem == null)
                return;

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                select distinct ltrim(rtrim(Asi.Nombre)) as Nombre
                from CargaAcademica CA
                inner join Seccion S on CA.SeccionID = S.SeccionID
                inner join Grado G on S.GradoID = G.GradoID
                inner join Docente D on CA.DocenteID = D.DocenteID
                inner join Asignatura Asi on CA.AsignaturaID = Asi.AsignaturaID
                where D.UsuarioID = @Docente
                  and G.NombreGrado = @Grado
                  and S.Letra = @Seccion
                order by Nombre", cn))
            {
                cmd.Parameters.AddWithValue("@Docente", docenteId);
                cmd.Parameters.AddWithValue("@Grado", cbGrado.SelectedItem.ToString().Trim());
                cmd.Parameters.AddWithValue("@Seccion", cbSeccion.SelectedItem.ToString().Trim());

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    HashSet<string> materias = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    while (dr.Read())
                    {
                        string nombre = dr["Nombre"].ToString().Trim();

                        if (!string.IsNullOrWhiteSpace(nombre) && materias.Add(nombre))
                        {
                            cbAsignatura.Items.Add(nombre);
                        }
                    }
                }
            }

            if (cbAsignatura.Items.Count > 0)
            {
                cbAsignatura.SelectedIndex = 0;
                cbAsignatura.Enabled = cbAsignatura.Items.Count > 1;
            }
            else
            {
                cbAsignatura.Enabled = false;
            }

            AjustarAnchoCombo(cbAsignatura);
            cbAsignatura.IntegralHeight = false;
            cbAsignatura.DropDownHeight = 300;
            cbAsignatura.MaxDropDownItems = 12;
        }

        private void cbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarSecciones();
        }

        private void cbSeccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarAsignaturas();
        }

        private void cbAsignatura_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAsignatura.SelectedItem == null)
                return;

            CargarReporte();
            lblAsignatura.Text = cbAsignatura.SelectedItem.ToString().ToUpper();
            CentrarLabelMateria();
        }

        private void cbParcial_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private DataTable ObtenerCalificaciones(
            string grado,
            int parcial,
            string seccion,
            string asignatura,
            DateTime fechaInicial,
            DateTime fechaFinal,
            string estudiante)
        {
            DataTable dt = new DataTable();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("spMAE_Calificaciones_semanales_experimental", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Grado", grado);
                cmd.Parameters.AddWithValue("@parcial", parcial);
                cmd.Parameters.AddWithValue("@Seccion", seccion);
                cmd.Parameters.AddWithValue("@Asignatura", asignatura);
                cmd.Parameters.AddWithValue("@fecha_inicial", fechaInicial.Date);
                cmd.Parameters.AddWithValue("@fecha_final", fechaFinal.Date);

                if (string.IsNullOrWhiteSpace(estudiante))
                    cmd.Parameters.AddWithValue("@Estudiante", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@Estudiante", estudiante.Trim());

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        private void CargarReporte()
        {
            if (cbGrado.SelectedItem == null ||
                cbSeccion.SelectedItem == null ||
                cbAsignatura.SelectedItem == null ||
                cbParcial.SelectedItem == null)
                return;

            string grado = cbGrado.SelectedItem.ToString();
            string seccion = cbSeccion.SelectedItem.ToString();
            string asignatura = cbAsignatura.SelectedItem.ToString();
            int parcial = Convert.ToInt32(cbParcial.SelectedItem);
            string estudiante = txtBuscar.Text.Trim();

            DateTime fechaInicial = ObtenerLunesDeLaSemana(dtpSemana.Value);
            DateTime fechaFinal = fechaInicial.AddDays(4); // lunes a viernes

            DataTable dt = ObtenerCalificaciones(
                grado,
                parcial,
                seccion,
                asignatura,
                fechaInicial,
                fechaFinal,
                estudiante);

            lblTitulo.Text = $"CUADRO DE CALIFICACIONES SEMANALES: {fechaInicial:dd/MM/yyyy} - {fechaFinal:dd/MM/yyyy}";

            if (dt == null || dt.Rows.Count == 0)
            {
                MostrarMensajeSinDatos("No se encontraron calificaciones para los filtros seleccionados.");
                return;
            }
            _mensajeSinDatos = "";
            ConfigurarDataGridViewNotas(dt);
            LlenarGridNotas(dt);

            foreach (DataGridViewColumn col in dgvNotas.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void ConfigurarDataGridViewNotas(DataTable dt)
        {
            dgvNotas.Columns.Clear();
            dgvNotas.Rows.Clear();

            dgvNotas.AllowUserToAddRows = false;
            dgvNotas.AllowUserToDeleteRows = false;
            dgvNotas.AllowUserToResizeRows = false;
            dgvNotas.ReadOnly = true;
            dgvNotas.RowHeadersVisible = false;
            dgvNotas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNotas.MultiSelect = false;
            dgvNotas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNotas.BackgroundColor = Color.White;
            dgvNotas.BorderStyle = BorderStyle.None;
            dgvNotas.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvNotas.EnableHeadersVisualStyles = false;
            dgvNotas.ColumnHeadersHeight = 45;
            dgvNotas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvNotas.RowTemplate.Height = 40;
            dgvNotas.ScrollBars = ScrollBars.Both;
            dgvNotas.GridColor = Color.LightGray;
            dgvNotas.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            dgvNotas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(92, 184, 92);
            dgvNotas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvNotas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvNotas.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvNotas.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgvNotas.DefaultCellStyle.BackColor = Color.White;
            dgvNotas.DefaultCellStyle.ForeColor = Color.Black;
            dgvNotas.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvNotas.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvNotas.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvNotas.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvNotas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            DataGridViewTextBoxColumn colNo = new DataGridViewTextBoxColumn();
            colNo.Name = "No";
            colNo.HeaderText = "No";
            colNo.FillWeight = 30;
            dgvNotas.Columns.Add(colNo);

            DataGridViewTextBoxColumn colAlumno = new DataGridViewTextBoxColumn();
            colAlumno.Name = "Alumno";
            colAlumno.HeaderText = "ALUMNO";
            colAlumno.MinimumWidth = 200;
            colAlumno.FillWeight = 200;
            dgvNotas.Columns.Add(colAlumno);

            DataGridViewTextBoxColumn colPonderado = new DataGridViewTextBoxColumn();
            colPonderado.Name = "Ponderado";
            colPonderado.HeaderText = "PONDERADO";
            colPonderado.FillWeight = 80;
            dgvNotas.Columns.Add(colPonderado);

            var actividades = dt.AsEnumerable()
                .Select(x => new
                {
                    Descripcion = LimpiarDescripcion(x["Descripcion"].ToString()),
                    Valor = Convert.ToDecimal(x["Valor"])
                })
                .Distinct()
                .OrderBy(x => x.Descripcion)
                .ToList();

            foreach (var act in actividades)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.Name = act.Descripcion;
                col.HeaderText = $"{act.Descripcion}\n{act.Valor:0.##}%";
                col.FillWeight = 80;

                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.WrapMode = DataGridViewTriState.True;

                dgvNotas.Columns.Add(col);
            }
        }

        private string LimpiarDescripcion(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return "";

            // cortar todo lo que venga después de "-"
            int index = descripcion.IndexOf("-");

            if (index > 0)
                return descripcion.Substring(0, index).Trim();

            return descripcion.Trim();
        }

        private void LlenarGridNotas(DataTable dt)
        {
            dgvNotas.Rows.Clear();

            var estudiantes = dt.AsEnumerable()
                .GroupBy(x => new
                {
                    EstudianteID = x["EstudianteID"],
                    Nombre = x["Estudiante"].ToString()
                })
                .OrderBy(x => x.Key.Nombre)
                .ToList();

            var actividades = dt.AsEnumerable()
                .Select(x => new
                {
                    Descripcion = x["Descripcion"].ToString(),
                    Valor = Convert.ToDecimal(x["Valor"])
                })
                .Distinct()
                .OrderBy(x => x.Descripcion)
                .ToList();

            int numero = 1;

            foreach (var est in estudiantes)
            {
                List<string> fila = new List<string>
                {
                    numero.ToString(),
                    est.Key.Nombre
                };

                decimal ponderado = 0m;

                foreach (var act in actividades)
                {
                    var reg = est.FirstOrDefault(x =>
                        x["Descripcion"].ToString() == act.Descripcion &&
                        Convert.ToDecimal(x["Valor"]) == act.Valor);

                    if (reg != null)
                    {
                        decimal nota = Convert.ToDecimal(reg["calificacion"]);
                        fila.Add(nota.ToString("0.##"));
                        ponderado += nota;
                    }
                    else
                    {
                        fila.Add("");
                    }
                }

                fila.Insert(2, ponderado.ToString("0.##"));

                dgvNotas.Rows.Add(fila.ToArray());
                numero++;
            }

            int total = dgvNotas.Rows.Count;
            lblRegistros.Text = total == 0
                ? "Sin registros"
                : $"Registros del 1 al {total} total de {total} registros";
        }

        private void MostrarMensajeSinDatos(string mensaje)
        {
            _mensajeSinDatos = mensaje;

            dgvNotas.Columns.Clear();
            dgvNotas.Rows.Clear();
            dgvNotas.DataSource = null;

            dgvNotas.AllowUserToAddRows = false;
            dgvNotas.AllowUserToDeleteRows = false;
            dgvNotas.AllowUserToResizeRows = false;
            dgvNotas.ReadOnly = true;
            dgvNotas.RowHeadersVisible = false;
            dgvNotas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNotas.MultiSelect = false;
            dgvNotas.BackgroundColor = Color.White;
            dgvNotas.BorderStyle = BorderStyle.None;
            dgvNotas.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvNotas.EnableHeadersVisualStyles = false;
            dgvNotas.ScrollBars = ScrollBars.None;

            lblRegistros.Text = "Sin registros";
            dgvNotas.Invalidate();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void plVerder_Resize(object sender, EventArgs e)
        {
            lblAsignatura.Location = new Point(0, 0);
            lblAsignatura.Size = plVerder.ClientSize;
        }

        private void CentrarLabelMateria()
        {
            lblAsignatura.Location = new Point(0, 0);
            lblAsignatura.Size = plVerder.ClientSize;
            lblAsignatura.TextAlignment = ContentAlignment.MiddleCenter;
        }
    }
}