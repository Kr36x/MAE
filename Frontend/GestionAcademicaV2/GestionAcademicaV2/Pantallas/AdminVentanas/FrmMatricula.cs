using GestionAcademicaV2.Modelos;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Event;
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
using System.Drawing.Imaging;
using System.IO;
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
            private ImageData ObtenerLogoDesdeResources()
            {
                using MemoryStream ms = new MemoryStream();

                // Verifica que el nombre coincida con tu Resources.Designer.cs
                GestionAcademicaV2.Properties.Resources.Logo_expandido.Save(ms, ImageFormat.Png);

                return ImageDataFactory.Create(ms.ToArray());
            }

            private Cell CrearCeldaEtiqueta(string texto, PdfFont boldFont)
            {
                return new Cell()
                    .Add(new Paragraph(texto)
                        .SetFont(boldFont)
                        .SetFontSize(9)
                        .SetFontColor(new DeviceRgb(0, 0, 0)))
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(0)
                    .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
            }
            private Cell CrearCeldaFirma(string texto, PdfFont regularFont)
            {
                return new Cell()
                    .Add(
                        new Paragraph("____________________________\n" + texto)
                            .SetFont(regularFont)
                            .SetFontSize(10)
                            .SetTextAlignment(TextAlignment.CENTER)
                    )
                    .SetBorder(Border.NO_BORDER)
                    .SetPaddingTop(18);
            }

            private Cell CrearCeldaValor(string texto, PdfFont regularFont)
            {
                return new Cell()
                    .Add(new Paragraph(string.IsNullOrWhiteSpace(texto) ? " " : texto)
                        .SetFont(regularFont)
                        .SetFontSize(9)
                        .SetFontColor(new DeviceRgb(0, 0, 0)))
                    .SetBorder(new SolidBorder(new DeviceRgb(80, 80, 80), 0.8f))
                    .SetBorderRadius(new BorderRadius(4))
                    .SetPaddingLeft(8)
                    .SetPaddingRight(8)
                    .SetPaddingTop(5)
                    .SetPaddingBottom(5)
                    .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
            }

            private Cell CrearCeldaValorSpan(string texto, PdfFont regularFont, int colspan)
            {
                return new Cell(1, colspan)
                    .Add(new Paragraph(string.IsNullOrWhiteSpace(texto) ? " " : texto)
                        .SetFont(regularFont)
                        .SetFontSize(9)
                        .SetFontColor(new DeviceRgb(0, 0, 0)))
                    .SetBorder(new SolidBorder(new DeviceRgb(80, 80, 80), 0.8f))
                    .SetBorderRadius(new BorderRadius(4))
                    .SetPaddingLeft(8)
                    .SetPaddingRight(8)
                    .SetPaddingTop(5)
                    .SetPaddingBottom(5)
                    .SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
            }

            private Paragraph CrearBarraTitulo(string texto, PdfFont boldFont)
            {
                return new Paragraph(texto)
                    .SetFont(boldFont)
                    .SetFontSize(11)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetBackgroundColor(ColorConstants.BLACK)
                    .SetPaddingLeft(8)
                    .SetPaddingTop(5)
                    .SetPaddingBottom(5)
                    .SetMarginTop(8)
                    .SetMarginBottom(10);
            }

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
                    using PdfWriter writer = new PdfWriter(rutaSalida);
                    using PdfDocument pdf = new PdfDocument(writer);
                    using Document doc = new Document(pdf);

                    doc.SetMargins(22, 24, 28, 24);

                    PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                    //pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new PageNumberEventHandler(regularFont));

                    // =========================
                    // ENCABEZADO
                    // =========================
                    Table encabezado = new Table(UnitValue.CreatePercentArray(new float[] { 1.1f, 3.9f, 1.4f }))
                        .UseAllAvailableWidth();
                    encabezado.SetBorder(Border.NO_BORDER);
                    encabezado.SetMarginBottom(8);

                    Cell celdaLogo = new Cell().SetBorder(Border.NO_BORDER).SetPadding(0);
                    try
                    {
                        iText.Layout.Element.Image logo = new iText.Layout.Element.Image(ObtenerLogoDesdeResources())
                            .ScaleToFit(95, 95)
                            .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT);

                        celdaLogo.Add(logo);
                    }
                    catch
                    {
                        celdaLogo.Add(new Paragraph(" ").SetFont(regularFont));
                    }

                    Cell celdaCentro = new Cell().SetBorder(Border.NO_BORDER).SetPaddingTop(6);
                    celdaCentro.Add(
                        new Paragraph("ATLANTIC ACADEMY BILINGUAL SCHOOL")
                            .SetFont(regularFont)
                            .SetFontSize(14)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMarginBottom(4)
                    );

                    celdaCentro.Add(
                        new Paragraph("FORMULARIO DE MATRÍCULA")
                            .SetFont(boldFont)
                            .SetFontSize(18)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetMarginTop(0)
                    );

                    Cell celdaFoto = new Cell()
                        .SetHeight(92)
                        .SetBorder(new SolidBorder(new DeviceRgb(90, 90, 90), 0.8f))
                        .SetPadding(0);

                    encabezado.AddCell(celdaLogo);
                    encabezado.AddCell(celdaCentro);
                    encabezado.AddCell(celdaFoto);

                    doc.Add(encabezado);

                    // =========================
                    // INFORMACIÓN DEL ESTUDIANTE
                    // =========================
                    doc.Add(CrearBarraTitulo("INFORMACIÓN DEL ESTUDIANTE", boldFont));

                    Table estudianteTabla = new Table(UnitValue.CreatePercentArray(new float[] { 2.3f, 5.7f, 1.1f, 2.6f }))
                        .UseAllAvailableWidth();
                    estudianteTabla.SetBorder(Border.NO_BORDER);

                    estudianteTabla.AddCell(CrearCeldaEtiqueta("NÚMERO DE IDENTIDAD", boldFont));
                    estudianteTabla.AddCell(CrearCeldaValor(identidad, regularFont));
                    estudianteTabla.AddCell(CrearCeldaEtiqueta("SEXO", boldFont));
                    estudianteTabla.AddCell(CrearCeldaValor(sexo, regularFont));

                    estudianteTabla.AddCell(CrearCeldaEtiqueta("FECHA DE NACIMIENTO", boldFont));
                    estudianteTabla.AddCell(CrearCeldaValor(fechaNacimiento, regularFont));
                    estudianteTabla.AddCell(CrearCeldaEtiqueta("TELÉFONO", boldFont));
                    estudianteTabla.AddCell(CrearCeldaValor(telefono, regularFont));

                    estudianteTabla.AddCell(CrearCeldaEtiqueta("GRADO QUE CURSARÁ", boldFont));
                    estudianteTabla.AddCell(CrearCeldaValor(grado, regularFont));
                    estudianteTabla.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                    estudianteTabla.AddCell(new Cell().SetBorder(Border.NO_BORDER));

                    estudianteTabla.AddCell(CrearCeldaEtiqueta("MANO PARA ESCRIBIR", boldFont));
                    estudianteTabla.AddCell(CrearCeldaValor(mano, regularFont));
                    estudianteTabla.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                    estudianteTabla.AddCell(new Cell().SetBorder(Border.NO_BORDER));

                    estudianteTabla.AddCell(CrearCeldaEtiqueta("ALERGÍAS", boldFont));
                    estudianteTabla.AddCell(CrearCeldaValor(alergias, regularFont));
                    estudianteTabla.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                    estudianteTabla.AddCell(new Cell().SetBorder(Border.NO_BORDER));

                    doc.Add(estudianteTabla);

                    // NOMBRE en fila aparte, más limpio
                    Table nombreTabla = new Table(UnitValue.CreatePercentArray(new float[] { 2.3f, 9.4f }))
                        .UseAllAvailableWidth();
                    nombreTabla.SetMarginTop(6);

                    nombreTabla.AddCell(CrearCeldaEtiqueta("NOMBRE", boldFont));
                    nombreTabla.AddCell(CrearCeldaValor(nombre, regularFont));

                    doc.Add(nombreTabla);

                    // Dirección aparte
                    Table direccionTabla = new Table(UnitValue.CreatePercentArray(new float[] { 2.1f, 8.9f }))
                        .UseAllAvailableWidth();
                    direccionTabla.SetMarginTop(8);

                    direccionTabla.AddCell(CrearCeldaEtiqueta("DIRECCIÓN", boldFont));

                    Cell celdaDireccion = new Cell()
                        .Add(new Paragraph(string.IsNullOrWhiteSpace(direccion) ? " " : direccion)
                            .SetFont(regularFont)
                            .SetFontSize(9))
                        .SetBorder(new SolidBorder(new DeviceRgb(80, 80, 80), 0.8f))
                        .SetPaddingLeft(8)
                        .SetPaddingRight(8)
                        .SetPaddingTop(8)
                        .SetPaddingBottom(8)
                        .SetHeight(70);

                    direccionTabla.AddCell(celdaDireccion);

                    doc.Add(direccionTabla);

                    // =========================
                    // INFORMACIÓN DE LOS PADRES
                    // =========================
                    doc.Add(CrearBarraTitulo("INFORMACIÓN LOS PADRES", boldFont));

                    Table titulosPadres = new Table(UnitValue.CreatePercentArray(new float[] { 1f, 1f }))
                        .UseAllAvailableWidth();
                    titulosPadres.SetMarginBottom(4);

                    titulosPadres.AddCell(
                        new Cell()
                            .Add(new Paragraph("PADRE")
                                .SetFont(boldFont)
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.CENTER))
                            .SetBorder(Border.NO_BORDER)
                    );

                    titulosPadres.AddCell(
                        new Cell()
                            .Add(new Paragraph("MADRE")
                                .SetFont(boldFont)
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.CENTER))
                            .SetBorder(Border.NO_BORDER)
                    );

                    doc.Add(titulosPadres);

                    Table padresTabla = new Table(UnitValue.CreatePercentArray(new float[] { 1f, 1f }))
                        .UseAllAvailableWidth();

                    Table padreInterno = new Table(UnitValue.CreatePercentArray(new float[] { 2.3f, 5.7f }))
                        .UseAllAvailableWidth();
                    padreInterno.SetBorder(Border.NO_BORDER);

                    padreInterno.AddCell(CrearCeldaEtiqueta("NOMBRE", boldFont));
                    padreInterno.AddCell(CrearCeldaValor(nombrePadre, regularFont));
                    padreInterno.AddCell(CrearCeldaEtiqueta("NÚMERO DE IDENTIDAD", boldFont));
                    padreInterno.AddCell(CrearCeldaValor(identidadPadre, regularFont));
                    padreInterno.AddCell(CrearCeldaEtiqueta("TELÉFONO", boldFont));
                    padreInterno.AddCell(CrearCeldaValor(telefonoPadre, regularFont));
                    padreInterno.AddCell(CrearCeldaEtiqueta("LUGAR DE TRABAJO", boldFont));
                    padreInterno.AddCell(CrearCeldaValor(trabajoPadre, regularFont));

                    Table madreInterno = new Table(UnitValue.CreatePercentArray(new float[] { 2.3f, 5.7f }))
                        .UseAllAvailableWidth();
                    madreInterno.SetBorder(Border.NO_BORDER);

                    madreInterno.AddCell(CrearCeldaEtiqueta("NOMBRE", boldFont));
                    madreInterno.AddCell(CrearCeldaValor(nombreMadre, regularFont));
                    madreInterno.AddCell(CrearCeldaEtiqueta("NÚMERO DE IDENTIDAD", boldFont));
                    madreInterno.AddCell(CrearCeldaValor(identidadMadre, regularFont));
                    madreInterno.AddCell(CrearCeldaEtiqueta("TELÉFONO", boldFont));
                    madreInterno.AddCell(CrearCeldaValor(telefonoMadre, regularFont));
                    madreInterno.AddCell(CrearCeldaEtiqueta("LUGAR DE TRABAJO", boldFont));
                    madreInterno.AddCell(CrearCeldaValor(trabajoMadre, regularFont));

                    padresTabla.AddCell(new Cell().Add(padreInterno).SetBorder(Border.NO_BORDER).SetPaddingRight(12));
                    padresTabla.AddCell(new Cell().Add(madreInterno).SetBorder(Border.NO_BORDER).SetPaddingLeft(12));

                    doc.Add(padresTabla);

                    // =========================
                    // FIRMAS
                    // =========================
                    Table firmas = new Table(UnitValue.CreatePercentArray(new float[] { 1f, 1f }))
                        .UseAllAvailableWidth();
                    firmas.SetMarginTop(22);
                    firmas.SetMarginBottom(14);

                    firmas.AddCell(CrearCeldaFirma("Firma del Tutor", regularFont));
                    firmas.AddCell(CrearCeldaFirma("Lugar y Fecha", regularFont));

                    doc.Add(firmas);

                    // =========================
                    // NOTAS
                    // =========================
                    iText.Layout.Element.List notas = new iText.Layout.Element.List()
                        .SetSymbolIndent(8)
                        .SetListSymbol("•")
                        .SetFont(regularFont)
                        .SetFontSize(9)
                        .SetMarginTop(6)
                        .SetMarginBottom(0);

                    notas.Add(new ListItem("Las mensualidades se pagan en el Banco de Occidente, a más tardar el 2 de cada mes, pagos después de esa fecha incurrirán en mora."));
                    notas.Add(new ListItem("La institución no se hace responsable por contagios de COVID u otras enfermedades adquiridas dentro o fuera de la misma."));
                    notas.Add(new ListItem("Al momento de realizar exámenes o pruebas, el alumno/a debe estar al día con las mensualidades."));

                    doc.Add(notas);

                    // =========================
                    // PIE DE PÁGINA
                    // =========================
                    doc.ShowTextAligned(
                        new Paragraph(
                            "Sistema de Gestión Académica MAE\n" +
                            $"Generado: {DateTime.Now:dd/MM/yyyy}\n" +
                            "Página 1 de 1")
                            .SetFont(regularFont)
                            .SetFontSize(9)
                            .SetFontColor(new DeviceRgb(90, 90, 90)),
                        24, 20,
                        TextAlignment.LEFT
                    );
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
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Guardar matrícula en PDF";
                saveFileDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"Matricula_{txtNombreEstudiante.Text.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf";

                // abrir en Descargas por defecto
                saveFileDialog.InitialDirectory = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads"
                );

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string ruta = saveFileDialog.FileName;

                    GenerarMatriculaPDF pdf = new GenerarMatriculaPDF();

                    pdf.CrearPDF(
                        ruta,
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

                    MessageBox.Show("PDF generado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
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

        private void txtAviso2_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
