using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmReporteCalificaciones : Form
    {
        public FrmReporteCalificaciones()
        {
            InitializeComponent();
        }

        private void FrmReporteAsistenciaDiaria_Load(object sender, EventArgs e)
        {
            ConfigurarDataGridViewNotas();
            CargarDatosNotasEjemplo();
        }

        private void ConfigurarDataGridViewNotas()
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

            dgvNotas.ThemeStyle.RowsStyle.BackColor = Color.White;
            dgvNotas.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            dgvNotas.ThemeStyle.RowsStyle.SelectionBackColor = Color.White;
            dgvNotas.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            dgvNotas.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(248, 248, 248);
            dgvNotas.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Black;
            dgvNotas.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.White;
            dgvNotas.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Black;

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

            AgregarColumnaActividad("Act1", "ACTIVIDAD 1\n(15%)", 105);
            AgregarColumnaActividad("Act2", "ACTIVIDAD 2\n(10%)", 105);
            AgregarColumnaActividad("Ex1", "EXAMEN I\n(20%)", 95);
            AgregarColumnaActividad("Act3", "ACTIVIDAD 3\n(10%)", 105);
            AgregarColumnaActividad("Act4", "ACTIVIDAD 4\n(7%)", 105);
            AgregarColumnaActividad("Act5", "ACTIVIDAD 5\n(5%)", 105);
            AgregarColumnaActividad("Act6", "ACTIVIDAD 6\n(8%)", 105);
            AgregarColumnaActividad("Ex2", "EXAMEN II\n(20%)", 95);

            foreach (DataGridViewColumn col in dgvNotas.Columns)
            {
                if (!col.Name.Equals("Nombre"))
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
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

        private void CargarDatosNotasEjemplo()
        {
            dgvNotas.Rows.Clear();

            dgvNotas.Rows.Add("1", "ANDRÉS MORÁN", "", "14.00", "7.00", "19.00", "7.00", "4.50", "5.00", "7.99", "23.40");
            dgvNotas.Rows.Add("2", "ANGIE PARDO", "", "8.00", "9.00", "20.00", "9.00", "7.00", "5.00", "8.10", "14.00");
            dgvNotas.Rows.Add("3", "BAIRON MARTÍNEZ", "", "10.00", "10.00", "15.00", "10.00", "6.50", "3.00", "9.17", "20.00");
            dgvNotas.Rows.Add("4", "BRANDON PAZ", "", "15.00", "10.00", "20.00", "10.00", "6.00", "4.00", "10.40", "20.00");

            CalcularPonderados();
        }

        private void CalcularPonderados()
        {
            foreach (DataGridViewRow fila in dgvNotas.Rows)
            {
                if (fila.IsNewRow) continue;

                double act1 = ConvertirDouble(fila.Cells["Act1"].Value);
                double act2 = ConvertirDouble(fila.Cells["Act2"].Value);
                double ex1 = ConvertirDouble(fila.Cells["Ex1"].Value);
                double act3 = ConvertirDouble(fila.Cells["Act3"].Value);
                double act4 = ConvertirDouble(fila.Cells["Act4"].Value);
                double act5 = ConvertirDouble(fila.Cells["Act5"].Value);
                double act6 = ConvertirDouble(fila.Cells["Act6"].Value);
                double ex2 = ConvertirDouble(fila.Cells["Ex2"].Value);

                double ponderado = act1 + act2 + ex1 + act3 + act4 + act5 + act6 + ex2;
                fila.Cells["Ponderado"].Value = ponderado.ToString("0.##");
            }
        }

        private double ConvertirDouble(object? valor)
        {
            return double.TryParse(valor?.ToString(), out double resultado) ? resultado : 0;
        }
    }
}
