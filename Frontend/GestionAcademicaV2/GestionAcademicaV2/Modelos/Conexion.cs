using Microsoft.Data.SqlClient;

namespace GestionAcademicaV2.Modelos
{
    public class Conexion
    {
        private string cadenaConexion = "Server=3.128.144.165;Database=AgroLinkDB;User Id=josue.varela;Password=JV20222000646;Encrypt=True;TrustServerCertificate=True;";

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}

