using GestionAcademicaV2.Modelos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public enum ModoOperacion
    {
        Crear,
        Editar
    }

    public partial class FrmCreaEditaConfig : Form
    {
        private ModoOperacion modo; // creando o editando
        private int idConfigSeleccionado; // ID de la fila que se edita

        public FrmCreaEditaConfig(ModoOperacion modoOperacion, int idConfig = 0)
        {
            InitializeComponent();
            CargarParciales();

            modo = modoOperacion;
            idConfigSeleccionado = idConfig;

            if (modo == ModoOperacion.Editar)
            {
                CargarDatosParaEditar(idConfigSeleccionado);
            }
        }


        private void CargarDatosParaEditar(int idConfig)
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                string consulta = $"SELECT CicloEscolar, Periodo, FechaInicio, FechaFin FROM Configuracion WHERE ConfigID = {idConfig}";
                DataTable dt = util.EjecutarConsulta(consulta);

                if (dt.Rows.Count > 0)
                {
                    txtCicloEscolar.Text = dt.Rows[0]["CicloEscolar"].ToString();
                    cbbPeriodo.SelectedItem = dt.Rows[0]["Periodo"].ToString();
                    dtpFechaInicio.Value = Convert.ToDateTime(dt.Rows[0]["FechaInicio"]);
                    dtpFechaFin.Value = Convert.ToDateTime(dt.Rows[0]["FechaFin"]);
                }
                else
                {
                    MessageBox.Show("No se encontraron datos para editar.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void CargarParciales()
        {
            cbbPeriodo.Items.Clear();
            cbbPeriodo.Items.Add("1");
            cbbPeriodo.Items.Add("2");
            cbbPeriodo.Items.Add("3");
            cbbPeriodo.Items.Add("4");
            cbbPeriodo.SelectedIndex = 0;
        }

        private bool ValidarCicloEscolar()
        {
            string ciclo = txtCicloEscolar.Text.Trim();
            if (!Regex.IsMatch(ciclo, @"^\d{4}-\d{4}$"))
            {
                MessageBox.Show("El ciclo escolar debe tener formato AAAA-AAAA");
                return false;
            }
            string[] partes = ciclo.Split('-');
            int anio1 = int.Parse(partes[0]);
            int anio2 = int.Parse(partes[1]);
            if (anio2 != anio1 + 1)
            {
                MessageBox.Show("El segundo año debe ser consecutivo al primero");
                return false;
            }
            return true;
        }

        private bool ValidarFechas()
        {
            DateTime inicio = dtpFechaInicio.Value;
            DateTime fin = dtpFechaFin.Value;
            if (fin <= inicio)
            {
                MessageBox.Show("La fecha fin debe ser mayor que la fecha inicio");
                return false;
            }
            return true;
        }

        private int ObtenerUltimoParcial(string ciclo)
        {
            EjecutarUtilidades util = new EjecutarUtilidades();

            try
            {
                string consulta = $"SELECT ISNULL(MAX(Periodo),0) FROM Configuracion WHERE CicloEscolar = '{ciclo}'";
                DataTable dt = util.EjecutarConsulta(consulta);
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al consultar los periodos: " + ex.Message);
                return 0; // Retornamos 0 para indicar que no hay parciales aún
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
                return 0;
            }
        }

        private void LimpiarCampos()
        {
            txtCicloEscolar.Clear();
            cbbPeriodo.SelectedIndex = 0;
            dtpFechaInicio.Value = DateTime.Now;
            dtpFechaFin.Value = DateTime.Now;
        }

        private void btnAperturarCicloEscolar_Click(object sender, EventArgs e)
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();

                string ciclo = txtCicloEscolar.Text.Trim();
                int periodo = int.Parse(cbbPeriodo.SelectedItem.ToString());

                if (!ValidarCicloEscolar() || !ValidarFechas())
                    return;

                SqlParameter[] parametros = new SqlParameter[]
                {
                    new SqlParameter("@CicloEscolar", ciclo),
                    new SqlParameter("@Periodo", periodo),
                    new SqlParameter("@FechaInicio", dtpFechaInicio.Value),
                    new SqlParameter("@FechaFin", dtpFechaFin.Value)
                };

                if (modo == ModoOperacion.Crear)
                {
                    util.EjecutarSPParametros("spMAE_CrearConfiguracion", parametros);
                    MessageBox.Show($"Periodo {periodo} del ciclo {ciclo} creado correctamente!");
                }
                else if (modo == ModoOperacion.Editar)
                {
                    SqlParameter[] parametrosEditar = new SqlParameter[]
                    {
                        new SqlParameter("@ConfigID", idConfigSeleccionado),
                        new SqlParameter("@FechaInicio", dtpFechaInicio.Value),
                        new SqlParameter("@FechaFin", dtpFechaFin.Value)
                    };

                    util.EjecutarSPParametros("spMAE_EditarConfiguracion", parametrosEditar);
                    MessageBox.Show($"Periodo {periodo} del ciclo {ciclo} actualizado correctamente!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los datos: " + ex.Message);
            }
        }

        private void btnCancelarApertura_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
                "¿Está seguro que desea cancelar y cerrar el formulario?",
                "Confirmar Cancelación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                // limpia campos
                LimpiarCampos();

                // cierra el formulario
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}
