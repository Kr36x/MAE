using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using GestionAcademicaV2.Pantallas.AdminVentanas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmConfigCicloEscolar : Form
    {

        // data para pagination
        private int paginaActual = 1;
        private int registrosPorPagina = 5;
        private int totalRegistrosGlobal = 0;


        public FrmConfigCicloEscolar()
        {
            InitializeComponent();
        }

        private void CargarCiclos()
        {
            EjecutarUtilidades util = new EjecutarUtilidades();
            DataTable dt = util.EjecutarSP("spMAE_ListarCiclosEscolares");

            if (dt.Rows.Count > 0)
            {
                // incluyendo opción "Todos"
                DataRow fila = dt.NewRow();
                fila["CicloEscolar"] = "Todos";
                dt.Rows.InsertAt(fila, 0);

                cbbCicloEscolar.SelectedIndexChanged -= cbbCicloEscolar_SelectedIndexChanged;

                cbbCicloEscolar.DataSource = dt;
                cbbCicloEscolar.DisplayMember = "CicloEscolar";
                cbbCicloEscolar.ValueMember = "CicloEscolar";

                cbbCicloEscolar.SelectedIndexChanged += cbbCicloEscolar_SelectedIndexChanged;
            }
        }

        // llenado del dvg
        private void CargarConfiguracion(string cicloFiltro = "Todos")
        {
            EjecutarUtilidades util = new EjecutarUtilidades();

            SqlParameter pCiclo = new SqlParameter("@CicloEscolar", cicloFiltro);
            SqlParameter pPagina = new SqlParameter("@NumeroPagina", paginaActual);
            SqlParameter pRegistros = new SqlParameter("@RegistrosPorPagina", registrosPorPagina);

            SqlParameter pTotal = new SqlParameter("@TotalRegistros", SqlDbType.Int);
            pTotal.Direction = ParameterDirection.Output;

            SqlParameter[] parametros = new SqlParameter[] { pCiclo, pPagina, pRegistros, pTotal };

            DataTable dt = util.EjecutarSPParametros("spMAE_ListarConfiguracionPaginada", parametros);

            if (pTotal.Value != DBNull.Value)
            {
                totalRegistrosGlobal = Convert.ToInt32(pTotal.Value);
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                dgvConfiguracion.DataSource = null;
                totalRegistrosGlobal = 0;
                ActualizarEstadoPaginacion();
                return;
            }

            dgvConfiguracion.AutoGenerateColumns = false;
            dgvConfiguracion.DataSource = dt;
            ActualizarEstadoPaginacion();

            // ocultamos ConfigID porque bugs y tal
            if (dgvConfiguracion.Columns.Contains("ConfigID"))
                dgvConfiguracion.Columns["ConfigID"].Visible = false;

            // agregamo columna de acciones
            if (!dgvConfiguracion.Columns.Contains("Acciones"))
            {
                DataGridViewImageColumn colAcciones = new DataGridViewImageColumn();
                colAcciones.Name = "Acciones";
                colAcciones.HeaderText = "";
                colAcciones.Width = 30;
                colAcciones.ImageLayout = DataGridViewImageCellLayout.Zoom;

                dgvConfiguracion.Columns.Add(colAcciones);
            }

            // deactivamos que el usuario ordene las columnas
            foreach (DataGridViewColumn col in dgvConfiguracion.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            // ajustamos ancho de columnas
            dgvConfiguracion.Columns["CicloEscolar"].Width = 300;
            dgvConfiguracion.Columns["Periodo"].Width = 100;
            dgvConfiguracion.Columns["FechaInicio"].Width = 300;
            dgvConfiguracion.Columns["FechaFin"].Width = 300;
            dgvConfiguracion.Columns["Activa"].Width = 150;
        }

        // funcion para disenio de dgvConfiguracion
        private void ConfigurarDGV()
        {
            dgvConfiguracion.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvConfiguracion.EnableHeadersVisualStyles = false;
            dgvConfiguracion.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(14, 102, 248);
            dgvConfiguracion.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvConfiguracion.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvConfiguracion.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvConfiguracion.RowTemplate.Height = 50;
            dgvConfiguracion.AllowUserToAddRows = false;
        }

        // para no mostrar true/false sino texto en primera columna
        private string ObtenerEstado(bool activa, DateTime fechaFin)
        {
            DateTime hoy = DateTime.Today;

            if (activa && fechaFin >= hoy)
                return "ACTIVO";
            else if (!activa && fechaFin < hoy)
                return "CERRADO";
            else
                return "PRÓXIMO";
        }

        private void ActualizarEstadoPaginacion()
        {
            int desde = totalRegistrosGlobal == 0 ? 0 : ((paginaActual - 1) * registrosPorPagina) + 1;
            int hasta = Math.Min(paginaActual * registrosPorPagina, totalRegistrosGlobal);
            int totalPaginas = (int)Math.Ceiling((double)totalRegistrosGlobal / registrosPorPagina);
            if (totalPaginas == 0) totalPaginas = 1;

            // cuestiones de disenio y tal porque que dolor de cabeza las propiedade

            Color grisNormal = Color.FromArgb(64, 64, 64);
            Color grisDeshabilitado = Color.FromArgb(220, 220, 220);

            // boton anterior
            btnAnterior.Enabled = (paginaActual > 1);
            btnAnterior.ForeColor = btnAnterior.Enabled ? grisNormal : grisDeshabilitado;

            //  forzamos fondos
            btnAnterior.FillColor = Color.White;
            btnAnterior.BackColor = Color.White;
            btnAnterior.DisabledState.FillColor = Color.White;
            btnAnterior.DisabledState.ForeColor = grisDeshabilitado;
            btnAnterior.DisabledState.BorderColor = Color.White;


            // boton siguiente
            btnSiguiente.Enabled = (paginaActual < totalPaginas);
            btnSiguiente.ForeColor = btnSiguiente.Enabled ? grisNormal : grisDeshabilitado;
            btnSiguiente.FillColor = Color.White;
            btnSiguiente.BackColor = Color.White;
            btnSiguiente.DisabledState.FillColor = Color.White;
            btnSiguiente.DisabledState.ForeColor = grisDeshabilitado;
            btnSiguiente.DisabledState.BorderColor = Color.White;

            // actualizamos label de Informacion al pricipio y tambien numero de pagina
            lblInfoRegistro.Text = $"Registros del {desde}-{hasta}. Total de registros: {totalRegistrosGlobal}";
            lblNumeroPagina.Text = paginaActual.ToString();
        }


        private void btnNuevoCicloEscolar_Click(object sender, EventArgs e)
        {
            FrmCreaEditaConfig frm = new FrmCreaEditaConfig(ModoOperacion.Crear);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                CargarCiclos();          // 🔥 recarga el combo
                CargarConfiguracion();   // 🔥 recarga el dgv
            }
        }

        private void cbbCicloEscolar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbCicloEscolar.SelectedValue == null) return;

            string cicloSeleccionado = cbbCicloEscolar.SelectedValue.ToString();

            CargarConfiguracion(cicloSeleccionado);
        }

        private void FrmConfigCicloEscolar_Load(object sender, EventArgs e)
        {
            CargarCiclos();

            ConfigurarDGV();

            CargarConfiguracion();
        }

        private void dgvConfiguracion_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvConfiguracion.Columns[e.ColumnIndex].Name == "Activa" && e.Value != null)
            {
                if (e.Value == null || e.Value == DBNull.Value) return;

                bool activa = Convert.ToBoolean(e.Value);
                DateTime fechaFin = Convert.ToDateTime(dgvConfiguracion.Rows[e.RowIndex].Cells["FechaFin"].Value);

                string estado = ObtenerEstado(activa, fechaFin);
                e.Value = estado;

                e.FormattingApplied = true;
            }
        }

        private void dgvConfiguracion_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvConfiguracion.Columns[e.ColumnIndex].Name == "Acciones")
            {
                var row = dgvConfiguracion.Rows[e.RowIndex];
                bool activa = Convert.ToBoolean(row.Cells["Activa"].Value);
                DateTime fechaFin = Convert.ToDateTime(row.Cells["FechaFin"].Value);
                string estado = ObtenerEstado(activa, fechaFin);

                if (estado == "CERRADO")
                {
                    MessageBox.Show("No se puede editar un período cerrado.");
                    return;
                }

                int idConfig = Convert.ToInt32(row.Cells["ConfigID"].Value);

                FrmCreaEditaConfig frm = new FrmCreaEditaConfig(ModoOperacion.Editar, idConfig);
                frm.ShowDialog();

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CargarCiclos();         
                    CargarConfiguracion();   
                }

                CargarConfiguracion();
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            paginaActual++;
            CargarConfiguracion(cbbCicloEscolar.SelectedValue?.ToString() ?? "Todos");
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                CargarConfiguracion(cbbCicloEscolar.SelectedValue?.ToString() ?? "Todos");
            }
        }
    }
}
