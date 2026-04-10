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
    public partial class FrmUsuariosPersonal : Form
    {
        private PantallaAdmin pantallaPrincipal;
        public FrmUsuariosPersonal(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }

        private void CargarUsuarios(string rol = "", string busqueda = "")
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                SqlParameter[] p =
                {
                    new SqlParameter("@rol", rol),
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

                dgvUsuarios.DataSource = dt;

                // Ocultar columna Estado real
                if (dgvUsuarios.Columns.Contains("Estado"))
                    dgvUsuarios.Columns["Estado"].Visible = false;

                // Ocultar columna EstadoTexto
                if (dgvUsuarios.Columns.Contains("EstadoTexto"))
                    dgvUsuarios.Columns["EstadoTexto"].Visible = false;

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
            if (!dgvUsuarios.Columns.Contains("colEstado"))
            {
                DataGridViewImageColumn colEstado = new DataGridViewImageColumn();
                colEstado.Name = "colEstado";
                colEstado.HeaderText = "ESTADO";
                colEstado.ImageLayout = DataGridViewImageCellLayout.Zoom;
                dgvUsuarios.Columns.Add(colEstado);
            }

            if (!dgvUsuarios.Columns.Contains("colEditar"))
            {
                DataGridViewImageColumn colEditar = new DataGridViewImageColumn();
                colEditar.Name = "colEditar";
                colEditar.HeaderText = "EDITAR";
                colEditar.ImageLayout = DataGridViewImageCellLayout.Zoom;
                dgvUsuarios.Columns.Add(colEditar);
            }

        }

        private void AplicarFiltros()
        {
            string rol = cbbRol.SelectedIndex > 0 ? cbbRol.Text : "";
            string busqueda = txtBuscar.Text.Trim();

            CargarUsuarios(rol, busqueda);
        }

        private void FrmUsuariosPersonal_Load(object sender, EventArgs e)
        {
            CargarUsuarios();
        }

        private void cbbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void btNuevoUsuario_Click(object sender, EventArgs e)
        {
            Pantallas.AdminVentanas.FrmGestionUsuarios forma = new Pantallas.AdminVentanas.FrmGestionUsuarios();
            forma.Show();
        }

        private void dgvUsuarios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Imagen Editar
            if (dgvUsuarios.Columns[e.ColumnIndex].Name == "colEditar")
            {
                e.Value = Properties.Resources.BotonEditar1;
            }

            // Imagen Estado
            if (dgvUsuarios.Columns[e.ColumnIndex].Name == "colEstado")
            {
                int estado = Convert.ToInt32(dgvUsuarios.Rows[e.RowIndex].Cells["Estado"].Value);

                e.Value = estado == 1
                    ? Properties.Resources.btActivo1
                    : Properties.Resources.btInactivo;
            }
        }

        private void dgvUsuarios_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {

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
        private void dgvUsuarios_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                if (e.RowIndex >= 0 && dgvUsuarios.Columns[e.ColumnIndex].Name == "EstadoTexto")
                {
                    int usuarioID = Convert.ToInt32(dgvUsuarios.Rows[e.RowIndex].Cells["UsuarioID"].Value);
                    string estadoTexto = dgvUsuarios.Rows[e.RowIndex].Cells["EstadoTexto"].Value.ToString();

                    int estadoBit = estadoTexto == "ACTIVO" ? 1 : 0;

                    EjecutarUtilidades util = new EjecutarUtilidades();

                    SqlParameter[] p =
                    {
                        new SqlParameter("@usuarioID", usuarioID),
                        new SqlParameter("@estado", estadoBit)
                    };
                    util.EjecutarSPParametros("spMAE_ActualizarEstadoUsuario", p);

                    dgvUsuarios.InvalidateRow(e.RowIndex);
                    MessageBox.Show("El estado del usuario ha sido actualizado correctamente.",
                                    "Estado actualizado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el estado en la base de datos: " + ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

        }

        private void dgvUsuarios_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvUsuarios.Columns[e.ColumnIndex].Name == "colEstado")
            {
                int usuarioID = Convert.ToInt32(dgvUsuarios.Rows[e.RowIndex].Cells["UsuarioID"].Value);
                int estadoActual = Convert.ToInt32(dgvUsuarios.Rows[e.RowIndex].Cells["Estado"].Value);
                string nombre = dgvUsuarios.Rows[e.RowIndex].Cells["Usuario"].Value.ToString();

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
                    dgvUsuarios.Rows[e.RowIndex].Cells["Estado"].Value = nuevoEstado;

                    // Refrescar imagen
                    dgvUsuarios.InvalidateCell(e.ColumnIndex, e.RowIndex);
                }
            }

            if (e.RowIndex >= 0 && dgvUsuarios.Columns[e.ColumnIndex].Name == "colEditar")
            {
                int usuarioID = Convert.ToInt32(dgvUsuarios.Rows[e.RowIndex].Cells["UsuarioID"].Value);

                Pantallas.AdminVentanas.FrmGestionUsuarios forma = new Pantallas.AdminVentanas.FrmGestionUsuarios(usuarioID);
                forma.ShowDialog();
            }
        }
    }
}
