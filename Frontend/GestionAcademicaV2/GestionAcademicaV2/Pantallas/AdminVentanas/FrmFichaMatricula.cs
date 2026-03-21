using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmFichaMatricula : Form
    {
        private PantallaAdmin pantallaPrincipal;
        public FrmFichaMatricula(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void FrmFichaMatricula_Load(object sender, EventArgs e)
        {

        }
    }
}
