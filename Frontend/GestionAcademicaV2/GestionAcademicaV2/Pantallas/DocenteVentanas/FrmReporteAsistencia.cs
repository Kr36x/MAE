using Guna.UI2.WinForms;
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
        public FrmReporteAsistencia()
        {
            InitializeComponent();          
        }

        private void FrmReporteAsistencia_Load(object sender, EventArgs e)
        {
            ConfigurarDataGridView(2026, 3);
            CargarDatosEjemplo(2026, 3);
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

            dgvAsistencia.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 100, 0);
            dgvAsistencia.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAsistencia.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvAsistencia.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvAsistencia.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAsistencia.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvAsistencia.DefaultCellStyle.SelectionBackColor = Color.White;
            dgvAsistencia.DefaultCellStyle.SelectionForeColor = Color.Black;

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

            // DIAS
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
            e.PaintBackground(e.CellBounds, true);
            //e.PaintBorder(e.ClipBounds, true);

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

        private void CargarDatosEjemplo(int anio, int mes)
        {
            dgvAsistencia.Rows.Clear();

            int totalDias = DateTime.DaysInMonth(anio, mes);

            string[] fila = new string[5 + totalDias];

            fila[0] = "1";              // ID
            fila[1] = "JORGE LOPEZ";    // Nombre
            fila[2] = "0";              // TP
            fila[3] = "0";              // TF
            fila[4] = "0";              // TE

            for (int i = 1; i <= totalDias; i++)
            {
                fila[4 + i] = "●";
            }

            fila[5] = "●"; // D1
            fila[6] = "X"; // D2
            fila[7] = "E"; // D3
            fila[8] = "F"; // D4

            dgvAsistencia.Rows.Add(fila);

            CalcularTotalesTodasLasFilas();
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
    }
}
