using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class CargaDocente : Form
    {
        private PantallaAdmin pantallaPrincipal;
        public CargaDocente(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }

        private void CargaDocente_Load(object sender, EventArgs e)
        {

        }
    }
}
