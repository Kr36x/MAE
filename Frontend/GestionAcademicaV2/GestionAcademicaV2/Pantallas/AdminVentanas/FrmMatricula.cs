using GestionAcademicaV2.Modelos;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.XMP.Impl;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Data.SqlClient;
using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public partial class FrmMatricula : Form
    {
        private PantallaAdmin pantallaPrincipal;
        public FrmMatricula(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }

        public class GenerarMatriculaPDF
        {
            public void CrearPDF(
                string rutaSalida,
                string identidad,
                string sexo,
                string nombre,
                string telefono,
                string fechaNacimiento,
                string direccion,
                string grado,
                string mano,
                string alergias,
                string nombrePadre,
                string identidadPadre,
                string telefonoPadre,
                string trabajoPadre,
                string nombreMadre,
                string identidadMadre,
                string telefonoMadre,
                string trabajoMadre
            )
            {
                try
                {
                    PdfWriter writer = new PdfWriter(rutaSalida);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document doc = new Document(pdf);

                    // TÍTULO
                    doc.Add(new Paragraph("FORMULARIO DE MATRÍCULA")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20));

                    doc.Add(new Paragraph("\n"));

                    // INFORMACIÓN DEL ESTUDIANTE
                    doc.Add(new Paragraph("INFORMACIÓN DEL ESTUDIANTE")
                        .SetFontSize(14)
                        .SetMarginBottom(10));

                    Table tablaEst = new Table(2).UseAllAvailableWidth();

                    tablaEst.AddCell(new Cell().Add(new Paragraph("Número de Identidad")).SetBorder(Border.NO_BORDER));
                    tablaEst.AddCell(new Cell().Add(new Paragraph(identidad)).SetBorder(Border.NO_BORDER));

                    tablaEst.AddCell(new Cell().Add(new Paragraph("Sexo")).SetBorder(Border.NO_BORDER));
                    tablaEst.AddCell(new Cell().Add(new Paragraph(sexo)).SetBorder(Border.NO_BORDER));

                    tablaEst.AddCell(new Cell().Add(new Paragraph("Nombre")).SetBorder(Border.NO_BORDER));
                    tablaEst.AddCell(new Cell().Add(new Paragraph(nombre)).SetBorder(Border.NO_BORDER));

                    tablaEst.AddCell(new Cell().Add(new Paragraph("Teléfono")).SetBorder(Border.NO_BORDER));
                    tablaEst.AddCell(new Cell().Add(new Paragraph(telefono)).SetBorder(Border.NO_BORDER));

                    tablaEst.AddCell(new Cell().Add(new Paragraph("Fecha de Nacimiento")).SetBorder(Border.NO_BORDER));
                    tablaEst.AddCell(new Cell().Add(new Paragraph(fechaNacimiento)).SetBorder(Border.NO_BORDER));

                    tablaEst.AddCell(new Cell().Add(new Paragraph("Dirección")).SetBorder(Border.NO_BORDER));
                    tablaEst.AddCell(new Cell().Add(new Paragraph(direccion)).SetBorder(Border.NO_BORDER));

                    tablaEst.AddCell(new Cell().Add(new Paragraph("Grado que cursará")).SetBorder(Border.NO_BORDER));
                    tablaEst.AddCell(new Cell().Add(new Paragraph(grado)).SetBorder(Border.NO_BORDER));

                    tablaEst.AddCell(new Cell().Add(new Paragraph("Mano para escribir")).SetBorder(Border.NO_BORDER));
                    tablaEst.AddCell(new Cell().Add(new Paragraph(mano)).SetBorder(Border.NO_BORDER));

                    tablaEst.AddCell(new Cell().Add(new Paragraph("Alergias")).SetBorder(Border.NO_BORDER));
                    tablaEst.AddCell(new Cell().Add(new Paragraph(alergias)).SetBorder(Border.NO_BORDER));

                    doc.Add(tablaEst);
                    doc.Add(new Paragraph("\n"));

                    doc.Add(new Paragraph("INFORMACIÓN DE LOS PADRES")
                        .SetFontSize(14)
                        .SetMarginBottom(10));

                    Table tablaPadres = new Table(2).UseAllAvailableWidth();

                    tablaPadres.AddCell(new Cell().Add(new Paragraph("Nombre del Padre")).SetBorder(Border.NO_BORDER));
                    tablaPadres.AddCell(new Cell().Add(new Paragraph(nombrePadre)).SetBorder(Border.NO_BORDER));

                    tablaPadres.AddCell(new Cell().Add(new Paragraph("Identidad del Padre")).SetBorder(Border.NO_BORDER));
                    tablaPadres.AddCell(new Cell().Add(new Paragraph(identidadPadre)).SetBorder(Border.NO_BORDER));

                    tablaPadres.AddCell(new Cell().Add(new Paragraph("Teléfono del Padre")).SetBorder(Border.NO_BORDER));
                    tablaPadres.AddCell(new Cell().Add(new Paragraph(telefonoPadre)).SetBorder(Border.NO_BORDER));

                    tablaPadres.AddCell(new Cell().Add(new Paragraph("Lugar de Trabajo del Padre")).SetBorder(Border.NO_BORDER));
                    tablaPadres.AddCell(new Cell().Add(new Paragraph(trabajoPadre)).SetBorder(Border.NO_BORDER));

                    tablaPadres.AddCell(new Cell().Add(new Paragraph("Nombre de la Madre")).SetBorder(Border.NO_BORDER));
                    tablaPadres.AddCell(new Cell().Add(new Paragraph(nombreMadre)).SetBorder(Border.NO_BORDER));

                    tablaPadres.AddCell(new Cell().Add(new Paragraph("Identidad de la Madre")).SetBorder(Border.NO_BORDER));
                    tablaPadres.AddCell(new Cell().Add(new Paragraph(identidadMadre)).SetBorder(Border.NO_BORDER));

                    tablaPadres.AddCell(new Cell().Add(new Paragraph("Teléfono de la Madre")).SetBorder(Border.NO_BORDER));
                    tablaPadres.AddCell(new Cell().Add(new Paragraph(telefonoMadre)).SetBorder(Border.NO_BORDER));

                    tablaPadres.AddCell(new Cell().Add(new Paragraph("Lugar de Trabajo de la Madre")).SetBorder(Border.NO_BORDER));
                    tablaPadres.AddCell(new Cell().Add(new Paragraph(trabajoMadre)).SetBorder(Border.NO_BORDER));

                    doc.Add(tablaPadres);
                    doc.Add(new Paragraph("\n"));

                    // FIRMAS
                    Table firmas = new Table(2).UseAllAvailableWidth();

                    firmas.AddCell(new Cell()
                        .Add(new Paragraph("\n\n______________________________\nFirma del Tutor")
                        .SetTextAlignment(TextAlignment.CENTER))
                        .SetBorder(Border.NO_BORDER));

                    firmas.AddCell(new Cell()
                        .Add(new Paragraph("\n\n______________________________\nLugar y Fecha")
                        .SetTextAlignment(TextAlignment.CENTER))
                        .SetBorder(Border.NO_BORDER));

                    doc.Add(firmas);

                    doc.Add(new Paragraph("\n"));

                    // NOTAS
                    doc.Add(new Paragraph("NOTAS:")
                        .SetFontSize(12));

                    doc.Add(new Paragraph(
                        "• Las mensualidades se pagan en el Banco de Occidente, a más tardar el 2 de cada mes; pagos después de esa fecha incurrirán en mora."
                    ).SetFontSize(11));

                    doc.Add(new Paragraph(
                        "• La institución no se hace responsable por contagios de COVID u otras enfermedades adquiridas dentro o fuera de la misma."
                    ).SetFontSize(11));

                    doc.Add(new Paragraph(
                        "•  Al momento de realizar exámenes o puebas, el alumno/a debe estar al día con las mensualidades. "
                    ).SetFontSize(11));

                    doc.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar el archivo: " + ex.Message);
                }
            }
        }
        private void guna2CheckBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void FrmMatricula_Load(object sender, EventArgs e)
        {
            CargarGrados();
            CargarSexo();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            GenerarMatriculaPDF pdf = new GenerarMatriculaPDF();

            pdf.CrearPDF(
                "C:\\Users\\DELL\\OneDrive\\Desktop\\SistemaGestionAcademicaMAE\\MAE\\Frontend\\GestionAcademicaV2\\Imagenes\\Matricula.pdf",
                txtIdentidadEstudiante.Text,
                cbbGenero.Text,
                txtNombreEstudiante.Text,
                txtTelefono.Text,
                dtpFechaNacimiento.Text,
                txtDireccion.Text,
                cbbGrado.Text,
                cbbMano.Text,
                txtAlergias.Text,
                txtNombrePadre.Text,
                txtIdentidadPadre.Text,
                txtTelefonoPadre.Text,
                txtTrabajoPadre.Text,
                txtNombreMadre.Text,
                txtIdentidadMadre.Text,
                txtTelefonoMadre.Text,
                txtTrabajoMadre.Text
            );

            MessageBox.Show("PDF generado correctamente");
        }

        private void CargarGrados()
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                DataTable tabla = util.EjecutarConsulta("SELECT * FROM vMAE_TraeGrados order by GradoID");
                cbbGrado.DataSource = tabla;
                cbbGrado.DisplayMember = "NombreGrado";
                cbbGrado.ValueMember = "GradoID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al llenar grados: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarSexo()
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                DataTable dt = util.EjecutarSP("spMAE_ObtenerSexo");

                cbbGenero.DataSource = dt;
                cbbGenero.DisplayMember = "Descripcion";
                cbbGenero.ValueMember = "Codigo"; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al llenar genero: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string identidad = txtIdentidadEstudiante.Text.Trim();

                if (string.IsNullOrEmpty(identidad))
                {
                    MessageBox.Show("Ingrese un número de identidad.");
                    return;
                }
                EjecutarUtilidades util = new EjecutarUtilidades();
                DataSet ds = util.EjecutarDataSet(
                    "EXEC spMAE_BuscarFichaMatriculaPorIdentidad '" + identidad + "'");

                if (ds.Tables[0].Columns.Contains("ErrorMensaje"))
                {
                    MessageBox.Show("Error SQL: " + ds.Tables[0].Rows[0]["ErrorMensaje"].ToString());
                    return;
                }

                if (ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("No existe registro del estudiante.");
                    return;
                }

                DataRow est = ds.Tables[0].Rows[0];

                txtNombreEstudiante.Text = est["Nombre"].ToString();
                cbbGenero.SelectedItem = est["Sexo"].ToString();
                dtpFechaNacimiento.Value = Convert.ToDateTime(est["FechaNacimiento"]);
                txtDireccion.Text = est["Direccion"].ToString();
                txtTelefono.Text = est["Telefono"].ToString();
                cbbGrado.SelectedItem = est["NombreGrado"].ToString();
                cbbMano.Text = est["Mano"].ToString();
                txtAlergias.Text = est["Alergia"].ToString();

                txtNombrePadre.Text = "";
                txtIdentidadPadre.Text = "";
                txtTelefonoPadre.Text = "";
                txtTrabajoPadre.Text = "";

                txtNombreMadre.Text = "";
                txtIdentidadMadre.Text = "";
                txtTelefonoMadre.Text = "";
                txtTrabajoMadre.Text = "";

                foreach (DataRow tutor in ds.Tables[1].Rows)
                {
                    string parentesco = tutor["Parentesco"].ToString().ToUpper();

                    if (parentesco == "PADRE")
                    {
                        txtNombrePadre.Text = tutor["Nombre"].ToString();
                        txtIdentidadPadre.Text = tutor["Identidad"].ToString();
                        txtTelefonoPadre.Text = tutor["Telefono"].ToString();
                        txtTrabajoPadre.Text = tutor["LugarTrabajo"].ToString();
                    }
                    else if (parentesco == "MADRE")
                    {
                        txtNombreMadre.Text = tutor["Nombre"].ToString();
                        txtIdentidadMadre.Text = tutor["Identidad"].ToString();
                        txtTelefonoMadre.Text = tutor["Telefono"].ToString();
                        txtTrabajoMadre.Text = tutor["LugarTrabajo"].ToString();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error de base de datos: " + ex.Message);
            }
        }

        private void txtIdentidadEstudiante_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                return;
            }

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}
