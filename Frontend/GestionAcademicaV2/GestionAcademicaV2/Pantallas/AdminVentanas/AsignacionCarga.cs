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
    public partial class AsignacionCarga : Form
    {
        bool bloqueado = false;
        public AsignacionCarga()
        {
            InitializeComponent();
            cbbDocentes.TextChanged += cbbDocentes_TextChanged;
        }

        private void CargarDocentes()
        {
            try
            {
                bloqueado = true;

                EjecutarUtilidades util = new EjecutarUtilidades();
                DataTable dt = util.EjecutarConsulta("spMAE_TraeDocentes");

                cbbDocentes.DataSource = dt;
                cbbDocentes.DisplayMember = "Nombre";
                cbbDocentes.ValueMember = "DocenteID";

                cbbDocentes.DropDownStyle = ComboBoxStyle.DropDown;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar docentes." + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                bloqueado = false;
            }
        }

        private void AsignacionCarga_Load(object sender, EventArgs e)
        {
            CargarDocentes();
        }

        private void cbbDocentes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbbDocentes_TextChanged(object sender, EventArgs e)
        {
            if (bloqueado) return;

            try
            {
                string filtro = cbbDocentes.Text.Trim();

                if (filtro.Length < 1)
                {
                    cbbDocentes.DroppedDown = false;
                    return;
                }

                bloqueado = true;

                EjecutarUtilidades util = new EjecutarUtilidades();

                SqlParameter[] p =
                {
            new SqlParameter("@Filtro", filtro)
        };

                DataTable dt = util.EjecutarSPParametros("spMAE_BuscarDocentes", p);

                cbbDocentes.DataSource = dt;
                cbbDocentes.DisplayMember = "Nombre";
                cbbDocentes.ValueMember = "DocenteID";

                cbbDocentes.DroppedDown = true;

                cbbDocentes.Text = filtro;
                cbbDocentes.SelectionStart = filtro.Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar docentes." + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                bloqueado = false;
            }
        }
    }
}
