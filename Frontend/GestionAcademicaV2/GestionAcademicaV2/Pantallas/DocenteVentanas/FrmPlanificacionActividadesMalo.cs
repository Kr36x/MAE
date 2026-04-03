using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmPlanificacionActividades : Form
    {
        private readonly int docenteId;
        private readonly string nombreDocente;
        private readonly Conexion conexion = new Conexion();

        public FrmPlanificacionActividades(int docenteId)
        {
            InitializeComponent();
            this.docenteId = docenteId;

        }

        private void FrmReporteAsistenciaDiaria_Load(object sender, EventArgs e)
        {
            //lblDocente.Text = nombreDocente;
            //txtAnio.Text = DateTime.Today.Year.ToString();
            CargarAnios();
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

            //lblAsignatura.AutoSize = false;
            //lblAsignatura.Width = plVerder.Width;
            //lblAsignatura.Height = plVerder.Height;
            //lblAsignatura.TextAlignment = ContentAlignment.MiddleCenter;
          

        }
        private void CentrarLabelMateria()
        {
           
        }
        private void CargarGrados()
        {
            cbGrado.Items.Clear();
            cbSeccion.Items.Clear();
            cbAsignatura.Items.Clear();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                select NombreGrado
                from
                (
                    select distinct
                        G.NombreGrado,
                        case
                            when G.NombreGrado = 'KINDER' then 1
                            when G.NombreGrado = 'PREBASICA' then 2
                            when G.NombreGrado = 'PRIMERO' then 3
                            when G.NombreGrado = 'SEGUNDO' then 4
                            when G.NombreGrado = 'TERCERO' then 5
                            when G.NombreGrado = 'CUARTO' then 6
                            when G.NombreGrado = 'QUINTO' then 7
                            when G.NombreGrado = 'SEXTO' then 8
                            when G.NombreGrado = 'SEPTIMO' then 9
                            when G.NombreGrado = 'OCTAVO' then 10
                            when G.NombreGrado = 'NOVENO' then 11
                            when G.NombreGrado = 'DECIMO' then 12
                            when G.NombreGrado = 'UNDECIMO' then 13
                            else 99
                        end as OrdenGrado
                    from CargaAcademica CA
                    inner join Seccion S on CA.SeccionID = S.SeccionID
                    inner join Grado G on S.GradoID = G.GradoID
                    inner join Docente D on CA.DocenteID = D.DocenteID
                    where D.UsuarioID = @Docente
                ) X
                order by OrdenGrado;", cn))
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
            cbAsignatura.Items.Clear();

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

            combo.DropDownWidth = maxWidth + 50; // margen
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

            //MessageBox.Show("Asignaturas cargadas: " + cbAsignatura.Items.Count);
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
            CargarReporte();
  
            CentrarLabelMateria();
        }

        private void cbParcial_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarReporte();
        }
        private void cbAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarReporte();
        }
        private DataTable ObtenerCalificaciones(string grado, int parcial, string seccion, string asignatura, int anio, string estudiante)
        {
            DataTable dt = new DataTable();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("spMAE_Calificaciones_semanlaes", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Grado", grado);
                cmd.Parameters.AddWithValue("@parcial", parcial);
                cmd.Parameters.AddWithValue("@Seccion", seccion);
                cmd.Parameters.AddWithValue("@Asignatura", asignatura);
                cmd.Parameters.AddWithValue("@anio", anio);

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
            dgvNotas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvNotas.BackgroundColor = Color.White;
            dgvNotas.BorderStyle = BorderStyle.None;
            dgvNotas.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvNotas.EnableHeadersVisualStyles = false;
            dgvNotas.ColumnHeadersHeight = 60;
            dgvNotas.RowTemplate.Height = 40;
            dgvNotas.ScrollBars = ScrollBars.Both;
            dgvNotas.GridColor = Color.LightGray;

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

            dgvNotas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            var colNo = new DataGridViewTextBoxColumn();
            colNo.Name = "No";
            colNo.HeaderText = "N°";
            colNo.Width = 50;
            colNo.Frozen = true;
            colNo.HeaderCell.Style.BackColor = Color.FromArgb(18, 125, 216);
            colNo.HeaderCell.Style.ForeColor = Color.White;
            dgvNotas.Columns.Add(colNo);

            var colNombre = new DataGridViewTextBoxColumn();
            colNombre.Name = "Nombre";
            colNombre.HeaderText = "ESTUDIANTES";
            colNombre.Width = 160;
            colNombre.Frozen = true;
            colNombre.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            colNombre.HeaderCell.Style.BackColor = Color.FromArgb(0, 100, 0);
            colNombre.HeaderCell.Style.ForeColor = Color.White;
            dgvNotas.Columns.Add(colNombre);

            var colPonderado = new DataGridViewTextBoxColumn();
            colPonderado.Name = "Ponderado";
            colPonderado.HeaderText = "PONDERADO";
            colPonderado.Width = 140;
            colPonderado.Frozen = true;
            colPonderado.HeaderCell.Style.BackColor = Color.FromArgb(0, 100, 0);
            colPonderado.HeaderCell.Style.ForeColor = Color.White;
            dgvNotas.Columns.Add(colPonderado);

            var actividades = dt.AsEnumerable()
                .Select(r => new
                {
                    Descripcion = r["Descripcion"].ToString(),
                    Valor = Convert.ToDecimal(r["Valor"])
                })
                .Distinct()
                .ToList();

            int i = 1;
            foreach (var act in actividades)
            {
                string descripcion = act.Descripcion;

                if (descripcion.Contains("-"))
                {
                    descripcion = descripcion.Split('-')[0].Trim();
                }

                AgregarColumnaActividad(
                    $"Act{i}",
                    $"{descripcion}\n({act.Valor:0.##}%)",
                    105
                );

                i++;
            }

            foreach (DataGridViewColumn col in dgvNotas.Columns)
            {
                if (!col.Name.Equals("Nombre"))
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void LlenarGridNotas(DataTable dt)
        {
            dgvNotas.Rows.Clear();

            if (dt == null || dt.Rows.Count == 0)
            {
                lblRegistros.Text = "Sin registros";
                return;
            }

            var actividades = dt.AsEnumerable()
                .Select(r => new
                {
                    Descripcion = r["Descripcion"].ToString(),
                    Valor = Convert.ToDecimal(r["Valor"])
                })
                .Distinct()
                .ToList();

            var estudiantes = dt.AsEnumerable()
                .GroupBy(r => new
                {
                    Id = Convert.ToInt32(r["EstudianteID"]),
                    Nombre = r["Estudiante"].ToString()
                })
                .OrderBy(g => g.Key.Nombre)
                .ToList();

            int numero = 1;

            foreach (var est in estudiantes)
            {
                List<string> fila = new List<string>();
                fila.Add(numero.ToString());
                fila.Add(est.Key.Nombre);

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

        private void CargarAnios()
        {
            cbAnio.Items.Clear();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                select distinct year(C.Fecha) as Anio
                from Calificacion C
                inner join Actividad A on C.ActividadID = A.ActividadID
                inner join CargaAcademica CA on A.CargaID = CA.CargaID
                inner join Seccion S on CA.SeccionID = S.SeccionID
                inner join Grado G on S.GradoID = G.GradoID
                inner join Docente D on CA.DocenteID = D.DocenteID
                where D.UsuarioID = @Docente
                order by Anio desc", cn))
            {
                cmd.Parameters.AddWithValue("@Docente", docenteId);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cbAnio.Items.Add(dr["Anio"].ToString());
                    }
                }
            }

            if (cbAnio.Items.Count > 0)
                cbAnio.SelectedIndex = 0; // año más reciente
        }
        private void MostrarMensajeSinDatos(string mensaje)
        {
            dgvNotas.Columns.Clear();
            dgvNotas.Rows.Clear();

            dgvNotas.AllowUserToAddRows = false;
            dgvNotas.AllowUserToDeleteRows = false;
            dgvNotas.ReadOnly = true;
            dgvNotas.RowHeadersVisible = false;
            dgvNotas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNotas.MultiSelect = false;
            dgvNotas.BackgroundColor = Color.White;
            dgvNotas.BorderStyle = BorderStyle.None;
            dgvNotas.EnableHeadersVisualStyles = false;
            dgvNotas.ColumnHeadersHeight = 45;
            dgvNotas.RowTemplate.Height = 40;

            var colMensaje = new DataGridViewTextBoxColumn();
            colMensaje.Name = "Mensaje";
            colMensaje.HeaderText = "INFORMACIÓN";
            colMensaje.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvNotas.Columns.Add(colMensaje);

            int rowIndex = dgvNotas.Rows.Add();
            dgvNotas.Rows[rowIndex].Cells["Mensaje"].Value = mensaje;
            dgvNotas.Rows[rowIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvNotas.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
            dgvNotas.Rows[rowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Italic);

            lblRegistros.Text = "Sin registros";
        }
        private void CargarReporte()
        {
            if (cbGrado.SelectedItem == null ||
                cbSeccion.SelectedItem == null ||
                cbAsignatura.SelectedItem == null ||
                cbParcial.SelectedItem == null ||
                cbAnio.SelectedItem == null)
                return;

            string grado = cbGrado.SelectedItem.ToString();
            string seccion = cbSeccion.SelectedItem.ToString();
            string asignatura = cbAsignatura.SelectedItem.ToString();
            int parcial = Convert.ToInt32(cbParcial.SelectedItem);
            int anio = Convert.ToInt32(cbAnio.SelectedItem);
            string estudiante = txtBuscar.Text.Trim();

            DataTable dt = ObtenerCalificaciones(grado, parcial, seccion, asignatura, anio, estudiante);

            lblTitulo.Text = $"CUADRO DE CALIFICACIONES: PARCIAL {parcial}";
            if (dt == null || dt.Rows.Count == 0)
            {
                MostrarMensajeSinDatos("No hay calificaciones registradas para este filtro.");
                return;
            }
            ConfigurarDataGridViewNotas(dt);
            LlenarGridNotas(dt);

            foreach (DataGridViewColumn col in dgvNotas.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void AgregarColumnaActividad(string name, string header, int width)
        {
            var col = new DataGridViewTextBoxColumn();
            col.Name = name;
            col.HeaderText = header;
            col.Width = width;
            col.HeaderCell.Style.BackColor = Color.FromArgb(92, 184, 92);
            col.HeaderCell.Style.ForeColor = Color.White;
            dgvNotas.Columns.Add(col);
        }

        private void guna2Panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void plVerder_Resize(object sender, EventArgs e)
        {
            lblAsignatura.Location = new Point(0, 0);
            lblAsignatura.Size = plVerder.ClientSize;
        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }

        private void lblRegistros_Click(object sender, EventArgs e)
        {

        }

        //private void CargarDatosNotasEjemplo()
        //{
        //    dgvNotas.Rows.Clear();

        //    dgvNotas.Rows.Add("1", "ANDRÉS MORÁN", "", "14.00", "7.00", "19.00", "7.00", "4.50", "5.00", "7.99", "23.40");
        //    dgvNotas.Rows.Add("2", "ANGIE PARDO", "", "8.00", "9.00", "20.00", "9.00", "7.00", "5.00", "8.10", "14.00");
        //    dgvNotas.Rows.Add("3", "BAIRON MARTÍNEZ", "", "10.00", "10.00", "15.00", "10.00", "6.50", "3.00", "9.17", "20.00");
        //    dgvNotas.Rows.Add("4", "BRANDON PAZ", "", "15.00", "10.00", "20.00", "10.00", "6.00", "4.00", "10.40", "20.00");

        //    CalcularPonderados();
        //}

        //private void CalcularPonderados()
        //{
        //    foreach (DataGridViewRow fila in dgvNotas.Rows)
        //    {
        //        if (fila.IsNewRow) continue;

        //        double act1 = ConvertirDouble(fila.Cells["Act1"].Value);
        //        double act2 = ConvertirDouble(fila.Cells["Act2"].Value);
        //        double ex1 = ConvertirDouble(fila.Cells["Ex1"].Value);
        //        double act3 = ConvertirDouble(fila.Cells["Act3"].Value);
        //        double act4 = ConvertirDouble(fila.Cells["Act4"].Value);
        //        double act5 = ConvertirDouble(fila.Cells["Act5"].Value);
        //        double act6 = ConvertirDouble(fila.Cells["Act6"].Value);
        //        double ex2 = ConvertirDouble(fila.Cells["Ex2"].Value);

        //        double ponderado = act1 + act2 + ex1 + act3 + act4 + act5 + act6 + ex2;
        //        fila.Cells["Ponderado"].Value = ponderado.ToString("0.##");
        //    }
        //}

        //private double ConvertirDouble(object? valor)
        //{
        //    return double.TryParse(valor?.ToString(), out double resultado) ? resultado : 0;
        //}
    }
}
