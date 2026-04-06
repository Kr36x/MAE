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
            dgvUsuarios.CellValueChanged += dgvUsuarios_CellValueChanged;
            dgvUsuarios.CurrentCellDirtyStateChanged += dgvUsuarios_CurrentCellDirtyStateChanged;
        }
        private void ConfigurarColumnaEstado()
        {
            try
            {
                if (dgvUsuarios.Columns.Contains("EstadoTexto"))
                {
                    int index = dgvUsuarios.Columns["EstadoTexto"].Index;
                    dgvUsuarios.Columns.Remove("EstadoTexto");

                    DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();
                    combo.HeaderText = "ESTADO";
                    combo.Name = "EstadoTexto";
                    combo.DataPropertyName = "EstadoTexto";
                    combo.Items.Add("ACTIVO");
                    combo.Items.Add("INACTIVO");
                    combo.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    combo.Width = 80;

                    dgvUsuarios.Columns.Insert(index, combo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al configurar el estado del usuario: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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

                if (!dt.Columns.Contains("EstadoTexto"))
                    dt.Columns.Add("EstadoTexto", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    bool estado = Convert.ToBoolean(row["Estado"]);
                    row["EstadoTexto"] = estado ? "ACTIVO" : "INACTIVO";
                }

                dgvUsuarios.DataSource = dt;

                if (dgvUsuarios.Columns.Contains("Estado"))
                {
                    dgvUsuarios.Columns["Estado"].Visible = false;
                }

                ConfigurarColumnaEstado();
                AgregarColumna();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void AgregarColumna()
        {
            if (!dgvUsuarios.Columns.Contains("btnEditar"))
            {
                DataGridViewButtonColumn btnEditar = new DataGridViewButtonColumn();
                btnEditar.Name = "btnEditar";
                btnEditar.HeaderText = "EDITAR";
                btnEditar.HeaderCell.Style.BackColor = Color.FromArgb(0, 102, 204);
                btnEditar.Text = "";
                btnEditar.UseColumnTextForButtonValue = false;
                btnEditar.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                btnEditar.Width = 50;

                dgvUsuarios.Columns.Add(btnEditar);

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

        }

        private void dgvUsuarios_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.IsCurrentCellDirty)
                dgvUsuarios.CommitEdit(DataGridViewDataErrorContexts.Commit);
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

            //try
            //{
            //    if (dgvUsuarios.Columns[e.ColumnIndex].Name == "EstadoTexto")
            //    {
            //        int usuarioID = Convert.ToInt32(dgvUsuarios.Rows[e.RowIndex].Cells["UsuarioID"].Value);
            //        string estadoTexto = dgvUsuarios.Rows[e.RowIndex].Cells["EstadoTexto"].Value.ToString();

            //        int estadoBit = estadoTexto == "ACTIVO" ? 1 : 0;

            //        EjecutarUtilidades util = new EjecutarUtilidades();

            //        SqlParameter[] p =
            //        {
            //            new SqlParameter("@usuarioID", usuarioID),
            //            new SqlParameter("@estado", estadoBit)
            //        };

            //        util.EjecutarSPParametros("spMAE_ActualizarEstadoUsuario", p);

            //        dgvUsuarios.InvalidateRow(e.RowIndex);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error al actualizar el estado en la base de datos: " + ex.Message,
            //                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

        }

        private void dgvUsuarios_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                int iconSize = 20;

                if (dgvUsuarios.Columns[e.ColumnIndex].Name == "btnEditar")
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                    Image img = Properties.Resources.report_blanco;

                    int x = e.CellBounds.Left + (e.CellBounds.Width - iconSize) / 2;
                    int y = e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2;

                    e.Graphics.DrawImage(img, new Rectangle(x, y, iconSize, iconSize));
                    e.Handled = true;
                }
            }
        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvUsuarios.Columns[e.ColumnIndex].Name == "btnEditar")
            {
                int usuarioID = Convert.ToInt32(dgvUsuarios.Rows[e.RowIndex].Cells["UsuarioID"].Value);

                Pantallas.AdminVentanas.FrmGestionUsuarios forma = new Pantallas.AdminVentanas.FrmGestionUsuarios(usuarioID);
                forma.ShowDialog();
            }
        }
    }
}
