using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class ReporteDocentes : Form
    {
        public ReporteDocentes()
        {
            InitializeComponent();

        }

        private void AplicarFiltros()
        {
            string rol = cbbEspecialidad.SelectedIndex > 0 ? cbbEspecialidad.Text : "";
            string busqueda = txtBuscarDocente.Text.Trim();

            CargarUsuarios(rol, busqueda);
        }
        private void ActualizarEstadoUsuario(int usuarioID, int nuevoEstado)
        {
            EjecutarUtilidades util = new EjecutarUtilidades();

            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@usuarioID", usuarioID),
                new SqlParameter("@estado", nuevoEstado)
            };

            util.EjecutarSPParametros("spMAE_ActualizarEstadoUsuario", parametros);
        }

        private void CargarUsuarios(string rol = "", string busqueda = "")
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                SqlParameter[] p =
                {
                    new SqlParameter("@rol", "DOCENTE"),
                    new SqlParameter("@usuario", busqueda),
                    new SqlParameter("@correo", busqueda)
                };

                DataTable dt = util.EjecutarSPParametros("spMAE_TraeUsuarios", p);

                // Crear columna de texto para mostrar ACTIVO / INACTIVO
                if (!dt.Columns.Contains("EstadoTexto"))
                    dt.Columns.Add("EstadoTexto", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    bool estado = Convert.ToBoolean(row["Estado"]);
                    row["EstadoTexto"] = estado ? "ACTIVO" : "INACTIVO";
                }

                dgvDocentes.DataSource = dt;

                // Ocultar columna Estado real
                if (dgvDocentes.Columns.Contains("Estado"))
                    dgvDocentes.Columns["Estado"].Visible = false;

                // Ocultar columna EstadoTexto
                if (dgvDocentes.Columns.Contains("EstadoTexto"))
                    dgvDocentes.Columns["EstadoTexto"].Visible = false;

                // Configurar columnas de imagen
                ConfigurarColumnas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConfigurarColumnas()
        {
            if (!dgvDocentes.Columns.Contains("colEstado"))
            {
                DataGridViewImageColumn colEstado = new DataGridViewImageColumn();
                colEstado.Name = "colEstado";
                colEstado.HeaderText = "ESTADO";
                colEstado.ImageLayout = DataGridViewImageCellLayout.Zoom;
                dgvDocentes.Columns.Add(colEstado);
            }

            if (!dgvDocentes.Columns.Contains("colEditar"))
            {
                DataGridViewImageColumn colEditar = new DataGridViewImageColumn();
                colEditar.Name = "colEditar";
                colEditar.HeaderText = "EDITAR";
                colEditar.ImageLayout = DataGridViewImageCellLayout.Zoom;
                dgvDocentes.Columns.Add(colEditar);
            }

        }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e)
        {

        }

        private void ReporteDocentes_Load(object sender, EventArgs e)
        {
            CargarUsuarios();
        }

        private void dgvDocentes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Imagen Editar
            if (dgvDocentes.Columns[e.ColumnIndex].Name == "colEditar")
            {
                e.Value = Properties.Resources.BotonEditar1;
            }

            // Imagen Estado
            if (dgvDocentes.Columns[e.ColumnIndex].Name == "colEstado")
            {
                int estado = Convert.ToInt32(dgvDocentes.Rows[e.RowIndex].Cells["Estado"].Value);

                e.Value = estado == 1
                    ? Properties.Resources.btActivo1
                    : Properties.Resources.btInactivo;
            }

        }

        private void dgvDocentes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvDocentes.Columns[e.ColumnIndex].Name == "colEstado")
            {
                int usuarioID = Convert.ToInt32(dgvDocentes.Rows[e.RowIndex].Cells["UsuarioID"].Value);
                int estadoActual = Convert.ToInt32(dgvDocentes.Rows[e.RowIndex].Cells["Estado"].Value);
                string nombre = dgvDocentes.Rows[e.RowIndex].Cells["Usuario"].Value.ToString();

                DialogResult r = MessageBox.Show(
                    $"¿Está seguro que desea cambiar el estado del usuario {nombre}?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (r == DialogResult.Yes)
                {
                    int nuevoEstado = estadoActual == 1 ? 0 : 1;

                    // Actualizar en BD
                    ActualizarEstadoUsuario(usuarioID, nuevoEstado);

                    // Actualizar en DataGridView
                    dgvDocentes.Rows[e.RowIndex].Cells["Estado"].Value = nuevoEstado;

                    // Refrescar imagen
                    dgvDocentes.InvalidateCell(e.ColumnIndex, e.RowIndex);
                }
            }

            if (e.RowIndex >= 0 && dgvDocentes.Columns[e.ColumnIndex].Name == "colEditar")
            {
                int usuarioID = Convert.ToInt32(dgvDocentes.Rows[e.RowIndex].Cells["UsuarioID"].Value);

                Pantallas.AdminVentanas.FrmGestionUsuarios forma = new Pantallas.AdminVentanas.FrmGestionUsuarios(usuarioID);
                forma.ShowDialog();
            }

        }

        private void cbbEspecialidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void txtBuscarDocente_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }
    }
}
