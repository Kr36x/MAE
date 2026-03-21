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

        private SesionUsuario ValidarLogin(string acceso, string clave)
        {
            Conexion conexion = new Conexion();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string query = @"
            SELECT UsuarioID, Usuario, Correo, Rol
            FROM Usuario
            WHERE (Usuario = @acceso OR Correo = @acceso)
              AND [Password] = @clave
              AND Estado = 1";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@acceso", acceso);
                    cmd.Parameters.AddWithValue("@clave", clave);

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new SesionUsuario
                            {
                                UsuarioID = Convert.ToInt32(dr["UsuarioID"]),
                                Usuario = dr["Usuario"].ToString(),
                                Correo = dr["Correo"].ToString(),
                                Rol = dr["Rol"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        private bool mostrarPassword = false;
        private void Login_Load(object sender, EventArgs e)
        {
            Conexion conexion = new Conexion();

            //Login 
            txtContrasenia.UseSystemPasswordChar = true;
            txtContrasenia.PasswordChar = '●';
            pbMostrarContrasenia.Image = Properties.Resources.ojo_cerrado;
            //pbMostrarContrasenia.HoverState.Image = null;
            //pbMostrarContrasenia.PressedState.Image = null;
            //pbMostrarContrasenia.CheckedState.Image = null;


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

        private void btnLogin_Click(object sender, EventArgs e)
        {
            
        }

        private void LoginBoton_Click(object sender, EventArgs e)
        {
            string acceso = txtUsuario.Text.Trim();
            string clave = txtContrasenia.Text.Trim();

            if (string.IsNullOrWhiteSpace(acceso) || string.IsNullOrWhiteSpace(clave))
            {
                MessageBox.Show("Ingrese usuario o correo, y contraseña.");
                return;
            }

            try
            {
                SesionUsuario usuarioActual = ValidarLogin(acceso, clave);

                if (usuarioActual == null)
                {
                    MessageBox.Show("Credenciales incorrectas o usuario inactivo.");
                    return;
                }

                if (usuarioActual.Rol == "Administrador")
                {
                    PantallaAdmin admin = new PantallaAdmin(usuarioActual);
                    admin.Show();
                    this.Hide();
                }
                else if (usuarioActual.Rol == "Docente")
                {
                    PantallaDocente docente = new PantallaDocente(usuarioActual);
                    docente.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Rol no reconocido: " + usuarioActual.Rol);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar sesión: " + ex.Message);
            }
        }
    }
}
