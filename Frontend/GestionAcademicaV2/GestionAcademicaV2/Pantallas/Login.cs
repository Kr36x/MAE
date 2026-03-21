using GestionAcademicaV2.Modelos;
using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private string ValidarLogin(string usuario, string clave)
        {
            Conexion conexion = new Conexion();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string query = @"
            SELECT Rol
            FROM Usuario
            WHERE Usuario = @usuario
              AND [Password] = @clave
              AND Estado = 1";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@clave", clave);

                    cn.Open();

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                        return resultado.ToString();

                    return null;
                }
            }
        }
        private bool mostrarPassword = false;
        private void Login_Load(object sender, EventArgs e)
        {
            Conexion conexion = new Conexion();

            //Login 
            txtContrasenia.UseSystemPasswordChar = true;
            txtContrasenia.PasswordChar = '●';
           pbMostrarContrasenia.Image = Properties.Resources.ojo_cerrado;
    pbMostrarContrasenia.HoverState.Image = null;
    pbMostrarContrasenia.PressedState.Image = null;
    pbMostrarContrasenia.CheckedState.Image = null;


            using (var cn = conexion.ObtenerConexion())
            {
                try
                {
                    cn.Open();
                    MessageBox.Show("✅ Conexión exitosa a la base de datos");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Error de conexión:\n" + ex.Message);
                }
            }
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {
           
        }

        private void passwordVisible_Click(object sender, EventArgs e)
        {
            mostrarPassword = !mostrarPassword;

            txtContrasenia.UseSystemPasswordChar = !mostrarPassword;

            if (mostrarPassword)
                {
                    txtContrasenia.PasswordChar = '\0';
                    pbMostrarContrasenia.Image = Properties.Resources.ojo_abierto;
                    pbMostrarContrasenia.PressedState.Image = Properties.Resources.ojo_abierto;
            }
            else
                {
                    txtContrasenia.PasswordChar = '*'; 
                    pbMostrarContrasenia.Image = Properties.Resources.ojo_cerrado1;
            }

            pbMostrarContrasenia.Refresh();
        }

        private void btnConexion_Click(object sender, EventArgs e)
        {

        }

        private void LoginBoton_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string clave = txtContrasenia.Text.Trim();

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
            {
                MessageBox.Show("Ingrese usuario y contraseña.");
                return;
            }

            try
            {
                string rol = ValidarLogin(usuario, clave);

                if (rol == null)
                {
                    MessageBox.Show("Usuario o contraseña incorrectos, o usuario inactivo.");
                    return;
                }

                MessageBox.Show("Bienvenido. Rol: " + rol);

                if (rol == "Administrador")
                {
                    PantallaAdmin pantallaAdmin = new PantallaAdmin();
                    pantallaAdmin.Show();
                    this.Hide();
                }
                else if (rol == "Docente")
                {
                    PantallaDocente pantallaDocente = new PantallaDocente();
                    pantallaDocente.Show();
                    this.Hide();
                }
                else if (rol == "Tutor")
                {
                    MessageBox.Show("Login correcto, pero aún no existe una pantalla para Tutor.");
                }
                else
                {
                    MessageBox.Show("Rol no reconocido: " + rol);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar sesión: " + ex.Message);
            }
        }
    }
}
