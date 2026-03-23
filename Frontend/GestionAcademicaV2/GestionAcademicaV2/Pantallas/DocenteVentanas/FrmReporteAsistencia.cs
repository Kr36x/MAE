using GestionAcademicaV2.Modelos;
using Guna.UI2.WinForms;
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
    public partial class FrmReporteAsistencia : Form
    {
        private readonly int docenteId;
        private readonly Conexion conexion = new Conexion();
        private int anioActual;
        private int mesActual;
        public FrmReporteAsistencia(int docenteId)
        {
            InitializeComponent();
            this.docenteId = docenteId;

        }

        private void FrmReporteAsistencia_Load(object sender, EventArgs e)
        {
            DateTime hoy = DateTime.Today;
            anioActual = hoy.Year;
            mesActual = hoy.Month;
            lblDocente.Text = ObtenerNombreDocente();

            CargarGrados();
            ConfigurarDataGridView(anioActual, mesActual);

            if (cbGrado.Items.Count > 0)
            {
                CargarReporte();
                cbGrado.SelectedIndex = 0;
            }
        }

        private void ActualizarLabelRegistros()
        {
            int total = dgvAsistencia.Rows.Count;

            // Si tienes fila nueva habilitada
            if (dgvAsistencia.AllowUserToAddRows)
                total--;

            if (total < 0) total = 0;

            if (total == 0)
            {
                lblRegistros.Text = "Sin registros";
            }
            else
            {
                lblRegistros.Text = $"Registros del 1 al {total} total de {total} registros";
            }
        }
        private void CargarGrados()
        {
            cbGrado.Items.Clear();
            cbSeccion.Items.Clear();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
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
                where D.UsuarioID = 4
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
            {
                cbSeccion.SelectedIndex = 0;
                CargarReporte();
            }
        }
        public class MesItem
        {
            public int Anio { get; set; }
            public int Mes { get; set; }

            public override string ToString()
            {
                return new DateTime(Anio, Mes, 1)
                    .ToString("MMMM yyyy")
                    .ToUpper();
            }
        }
        private void CargarMeses()
        {
            cbMes.Items.Clear();

            if (cbGrado.SelectedItem == null || cbSeccion.SelectedItem == null)
                return;

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                select distinct
                    year(A.Fecha) as Anio,
                    month(A.Fecha) as Mes
                from Asistencia A
                inner join CargaAcademica CA on A.CargaID = CA.CargaID
                inner join Seccion S on CA.SeccionID = S.SeccionID
                inner join Grado G on S.GradoID = G.GradoID
                inner join Docente D on CA.DocenteID = D.DocenteID
                where D.UsuarioID = @Docente
                  and G.NombreGrado = @Grado
                  and S.Letra = @Seccion
            order by Anio, Mes", cn))
            {
                cmd.Parameters.AddWithValue("@Docente", docenteId);
                cmd.Parameters.AddWithValue("@Grado", cbGrado.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@Seccion", cbSeccion.SelectedItem.ToString());

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cbMes.Items.Add(new MesItem
                        {
                            Anio = Convert.ToInt32(dr["Anio"]),
                            Mes = Convert.ToInt32(dr["Mes"])
                        });
                    }
                }
            }

            if (cbMes.Items.Count > 0)
                cbMes.SelectedIndex = cbMes.Items.Count - 1; // último mes (más reciente)
        }

        private void cbMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMes.SelectedItem is MesItem mes)
            {
                anioActual = mes.Anio;
                mesActual = mes.Mes;

                CargarReporte();
            }
        }
        private string ObtenerNombreDocente()
        {
            string nombre = "";

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand(@"
                select Nombre
                from Docente
                where UsuarioID = @Docente", cn))
            {
                cmd.Parameters.AddWithValue("@Docente", docenteId);
                cn.Open();

                object resultado = cmd.ExecuteScalar();

                if (resultado != null)
                    nombre = resultado.ToString();
            }

            return nombre;
        }

        private void cbGrado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarSecciones();

            if (cbSeccion.Items.Count > 0)
                cbSeccion.SelectedIndex = 0;
        }

        private void cbSeccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarReporte();
            CargarMeses();
        }
        private void CargarReporte()
        {
            if (cbGrado.SelectedItem == null || cbSeccion.SelectedItem == null)
                return;

            DateTime fechaInicial = new DateTime(anioActual, mesActual, 1);
            DateTime fechaFinal = fechaInicial.AddMonths(1).AddDays(-1);

            string estudiante = txtBuscar.Text.Trim();
            string grado = cbGrado.SelectedItem.ToString();
            string seccion = cbSeccion.SelectedItem.ToString();

            DataTable dt = ObtenerAsistencias(
                fechaInicial,
                fechaFinal,
                docenteId,
                estudiante,
                grado,
                seccion
            );

            ConfigurarDataGridView(anioActual, mesActual);
            LlenarGridDesdeDataTable(dt, anioActual, mesActual);
            ActualizarLabelRegistros();

            ActualizarTituloMes();
        }

        private void ActualizarTituloMes()
        {
            DateTime fecha = new DateTime(anioActual, mesActual, 1);
            lblMes.Text = fecha.ToString("MMMM yyyy").ToUpper();
            //lblFecha.Text = fecha.ToString("dd/MM/yyyy");
        }

        private DataTable ObtenerAsistencias(
             DateTime fechaInicial,
             DateTime fechaFinal,
             int docente,
             string estudiante,
             string grado,
             string seccion)
        {
            DataTable dt = new DataTable();

            using (SqlConnection cn = conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("spMAE_Asistencias_por_Grado", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@fecha_inicial", fechaInicial);
                cmd.Parameters.AddWithValue("@fecha_final", fechaFinal);
                cmd.Parameters.AddWithValue("@Docente", docente);
                cmd.Parameters.AddWithValue("@Grado", grado);
                cmd.Parameters.AddWithValue("@Seccion", seccion);

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
        //private void btnAnterior_Click(object sender, EventArgs e)
        //{
        //    DateTime fecha = new DateTime(anioActual, mesActual, 1).AddMonths(-1);
        //    anioActual = fecha.Year;
        //    mesActual = fecha.Month;
        //    CargarReporte();
        //}

        //private void btnSiguiente_Click(object sender, EventArgs e)
        //{
        //    DateTime fecha = new DateTime(anioActual, mesActual, 1).AddMonths(1);
        //    anioActual = fecha.Year;
        //    mesActual = fecha.Month;
        //    CargarReporte();
        //}
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarReporte();
        }
        private void lblTitulo_Click(object sender, EventArgs e)
        {
            this.Text = "Reporte de Asistencia";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(239, 239, 239);
            this.WindowState = FormWindowState.Maximized;
        }

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void dvgAsistencia_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void ConfigurarDataGridView(int anio, int mes)
        {
            dgvAsistencia.Columns.Clear();
            dgvAsistencia.Rows.Clear();

            int totalDias = DateTime.DaysInMonth(anio, mes);

            dgvAsistencia.AllowUserToAddRows = false;
            dgvAsistencia.AllowUserToDeleteRows = false;
            dgvAsistencia.AllowUserToResizeRows = false;
            dgvAsistencia.ReadOnly = true;
            dgvAsistencia.RowHeadersVisible = false;
            dgvAsistencia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAsistencia.MultiSelect = false;
            dgvAsistencia.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvAsistencia.BackgroundColor = Color.White;
            dgvAsistencia.BorderStyle = BorderStyle.None;
            dgvAsistencia.EnableHeadersVisualStyles = false;
            dgvAsistencia.ColumnHeadersHeight = 40;
            dgvAsistencia.RowTemplate.Height = 40;
            dgvAsistencia.ScrollBars = ScrollBars.Both;

            // 👇 esto agrega separación visible entre celdas
            dgvAsistencia.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvAsistencia.GridColor = Color.FromArgb(210, 210, 210);

            dgvAsistencia.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvAsistencia.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;

            dgvAsistencia.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 100, 0);
            dgvAsistencia.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAsistencia.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvAsistencia.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvAsistencia.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAsistencia.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvAsistencia.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvAsistencia.DefaultCellStyle.SelectionForeColor = Color.Black;

            // 👇 diferencia visual entre filas
            dgvAsistencia.DefaultCellStyle.BackColor = Color.White;
            dgvAsistencia.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            // opcional: quita ese fondo azul de selección si haces click
            dgvAsistencia.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 245, 245);
            dgvAsistencia.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 235, 235);

            // FIJAS
            var colId = new DataGridViewTextBoxColumn();
            colId.Name = "ID";
            colId.HeaderText = "N°";
            colId.Width = 50;
            colId.Frozen = true;
            dgvAsistencia.Columns.Add(colId);

            var colNombre = new DataGridViewTextBoxColumn();
            colNombre.Name = "Nombre";
            colNombre.HeaderText = "ESTUDIANTES";
            colNombre.Width = 220;
            colNombre.Frozen = true;
            colNombre.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvAsistencia.Columns.Add(colNombre);

            var colTP = new DataGridViewTextBoxColumn();
            colTP.Name = "TP";
            colTP.HeaderText = "TP";
            colTP.Width = 50;
            colTP.Frozen = true;
            colTP.HeaderCell.Style.BackColor = Color.FromArgb(120, 220, 80);
            colTP.HeaderCell.Style.ForeColor = Color.White;
            dgvAsistencia.Columns.Add(colTP);

            var colTF = new DataGridViewTextBoxColumn();
            colTF.Name = "TF";
            colTF.HeaderText = "TF";
            colTF.Width = 50;
            colTF.Frozen = true;
            colTF.HeaderCell.Style.BackColor = Color.Red;
            colTF.HeaderCell.Style.ForeColor = Color.White;
            dgvAsistencia.Columns.Add(colTF);

            var colTE = new DataGridViewTextBoxColumn();
            colTE.Name = "TE";
            colTE.HeaderText = "TE";
            colTE.Width = 50;
            colTE.Frozen = true;
            colTE.HeaderCell.Style.BackColor = Color.Gold;
            colTE.HeaderCell.Style.ForeColor = Color.White;
            dgvAsistencia.Columns.Add(colTE);

            for (int dia = 1; dia <= totalDias; dia++)
            {
                var colDia = new DataGridViewTextBoxColumn();
                colDia.Name = $"D{dia}";
                colDia.HeaderText = dia.ToString();
                colDia.Width = 42;
                colDia.HeaderCell.Style.BackColor = Color.FromArgb(92, 184, 92);
                colDia.HeaderCell.Style.ForeColor = Color.White;
                dgvAsistencia.Columns.Add(colDia);
            }

            dgvAsistencia.CellFormatting -= dgvAsistencia_CellFormatting;
            dgvAsistencia.CellFormatting += dgvAsistencia_CellFormatting;

            dgvAsistencia.CellPainting -= dgvAsistencia_CellPainting;
            dgvAsistencia.CellPainting += dgvAsistencia_CellPainting;
        }
        private void dgvAsistencia_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string nombreColumna = dgvAsistencia.Columns[e.ColumnIndex].Name;

            if (!nombreColumna.StartsWith("D")) return;

            string valor = dgvAsistencia.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";

            Color? colorTexto = null;
            Font fuente = new Font("Segoe UI", 10, FontStyle.Bold);

            if (valor == "●")
            {
                colorTexto = Color.FromArgb(110, 193, 74);
                fuente = new Font("Segoe UI", 14, FontStyle.Bold);
            }
            else if (valor == "X")
            {
                colorTexto = Color.Red;
            }
            else if (valor == "E")
            {
                colorTexto = Color.Goldenrod;
            }
            else if (valor == "F" || valor == "I" || valor == "N" || valor == "D")
            {
                colorTexto = Color.Black;
            }

            if (colorTexto == null) return;

            e.Handled = true;

            // 👇 pinta fondo + borde
            e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

            TextRenderer.DrawText(
                e.Graphics,
                valor,
                fuente,
                e.CellBounds,
                colorTexto.Value,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );
        }
        private void dgvAsistencia_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            string valor = e.Value.ToString() ?? "";

            // SOLO PARA COLUMNAS DE DÍAS
            if (dgvAsistencia.Columns[e.ColumnIndex].Name.StartsWith("D"))
            {
                // PRESENTE (● VERDE)
                if (valor == "●")
                {
                    e.Value = "●";
                    e.CellStyle.ForeColor = Color.FromArgb(110, 193, 74); // verde bonito
                    e.CellStyle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                    e.FormattingApplied = true;
                }

                // FALTA (X ROJA)
                else if (valor == "X")
                {
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }

                // EXCUSA (E AMARILLA)
                else if (valor == "E")
                {
                    e.CellStyle.ForeColor = Color.Goldenrod;
                    e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }

                // OTROS (F, I, N, D)
                else if (valor == "F" || valor == "I" || valor == "N" || valor == "D")
                {
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
            }

            // 🎯 COLORES DE TOTALES
            if (dgvAsistencia.Columns[e.ColumnIndex].Name == "TP")
            {
                e.CellStyle.BackColor = Color.FromArgb(120, 220, 80);
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            if (dgvAsistencia.Columns[e.ColumnIndex].Name == "TF")
            {
                e.CellStyle.BackColor = Color.Red;
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            if (dgvAsistencia.Columns[e.ColumnIndex].Name == "TE")
            {
                e.CellStyle.BackColor = Color.Gold;
                e.CellStyle.ForeColor = Color.Black;
                e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }
        }

        private void LlenarGridDesdeDataTable(DataTable dt, int anio, int mes)
        {
            dgvAsistencia.Rows.Clear();

            if (dt == null || dt.Rows.Count == 0)
                return;

            int totalDias = DateTime.DaysInMonth(anio, mes);

            var grupos = dt.AsEnumerable()
                .GroupBy(r => r["Estudiante"].ToString())
                .OrderBy(g => g.Key)
                .ToList();

            int correlativo = 1;

            foreach (var grupo in grupos)
            {
                string[] fila = new string[5 + totalDias];

                fila[0] = correlativo.ToString();
                fila[1] = grupo.Key;
                fila[2] = "0";
                fila[3] = "0";
                fila[4] = "0";

                for (int i = 1; i <= totalDias; i++)
                {
                    fila[4 + i] = "";
                }

                foreach (var item in grupo
                             .Where(x => x["Fecha"] != DBNull.Value && x["Estado"] != DBNull.Value)
                             .GroupBy(x => Convert.ToDateTime(x["Fecha"]).Day)
                             .Select(g => g.First()))
                {
                    DateTime fecha = Convert.ToDateTime(item["Fecha"]);

                    if (fecha.Year != anio || fecha.Month != mes)
                        continue;

                    int dia = fecha.Day;
                    string estado = item["Estado"].ToString().Trim().ToUpper();

                    fila[4 + dia] = MapearEstadoASimbolo(estado);
                }

                dgvAsistencia.Rows.Add(fila);
                correlativo++;
            }

            CalcularTotalesTodasLasFilas();
        }
        private string MapearEstadoASimbolo(string estado)
        {
            switch (estado)
            {
                case "P":
                case "PRESENTE":
                case "ASISTIO":
                case "ASISTIÓ":
                    return "●";

                case "E":
                case "EXCUSA":
                case "EXCUSADO":
                    return "E";

                case "F":
                case "FALTA":
                case "AUSENTE":
                    return "X";

                case "I":
                    return "I";

                case "N":
                    return "N";

                case "D":
                    return "D";

                default:
                    return "";
            }
        }
        private void CalcularTotalesTodasLasFilas()
        {
            foreach (DataGridViewRow fila in dgvAsistencia.Rows)
            {
                if (!fila.IsNewRow)
                    CalcularTotalesFila(fila);
            }
        }

        private void CalcularTotalesFila(DataGridViewRow fila)
        {
            int tp = 0;
            int tf = 0;
            int te = 0;

            foreach (DataGridViewColumn col in dgvAsistencia.Columns)
            {
                if (col.Name.StartsWith("D"))
                {
                    string valor = fila.Cells[col.Name].Value?.ToString() ?? "";

                    if (valor == "●")
                        tp++;
                    else if (valor == "X" || valor == "F" || valor == "I" || valor == "N" || valor == "D")
                        tf++;
                    else if (valor == "E")
                        te++;
                }
            }

            fila.Cells["TP"].Value = tp;
            fila.Cells["TF"].Value = tf;
            fila.Cells["TE"].Value = te;
        }

        private void guna2HtmlLabel7_Click(object sender, EventArgs e)
        {

        }
    }

}
