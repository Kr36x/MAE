using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmReporteDesercionRetencion : Form
    {
        private PantallaAdmin pantallaPrincipal;
        public FrmReporteDesercionRetencion(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }

        private void FrmReporteDesercionRetencion_Load(object sender, EventArgs e)
        {

        }
    }
}
