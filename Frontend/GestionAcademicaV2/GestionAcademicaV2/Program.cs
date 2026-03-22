using GestionAcademicaV2.Pantallas;
using GestionAcademicaV2.Pantallas.DocenteVentanas;

namespace GestionAcademicaV2
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
<<<<<<< Updated upstream
            Application.Run(new Login());
=======
            Application.Run(new PantallaAdmin());
>>>>>>> Stashed changes
        }
    }
}