using System;
using System.Collections.Generic;
using System.Text;

namespace GestionAcademicaV2.Modelos
{
    public class SesionUsuario
    {
        public int UsuarioID { get; set; }
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
    }
}
