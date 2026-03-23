using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    public partial class FrmSeleccionReportes : Form
    {
        private PantallaDocente pantallaPrincipal;
        private int docenteId;
        private string nombreDocente;
        public FrmSeleccionReportes(PantallaDocente principal, int docenteId)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
            this.docenteId = docenteId;
            nombreDocente = "";
        }

        private void FrmSeleccionReporte_Load(object sender, EventArgs e)
        {
            // Aquí puedes deshabilitar botones si quieres
        }

        private void btnReporteAsistencia_Click(object sender, EventArgs e)
        {
            pantallaPrincipal.MostrarReporteAsistencia(docenteId);
        }

        private void btnCalificaciones_Click(object sender, EventArgs e)
        {
            pantallaPrincipal.MostrarReporteCalificacion(docenteId);
        }

        private void btnConsolidadas_Click(object sender, EventArgs e)
        {
            pantallaPrincipal.MostrarConsolidadoAsignaturas(docenteId);
        }
    }
}
