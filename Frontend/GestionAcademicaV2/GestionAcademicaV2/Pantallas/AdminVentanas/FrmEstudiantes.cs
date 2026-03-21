using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmEstudiantes : Form
    {
        private PantallaAdmin pantallaPrincipal;
        public FrmEstudiantes(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }

        private void FrmEstudiantes_Load(object sender, EventArgs e)
        {

        }
    }
}
