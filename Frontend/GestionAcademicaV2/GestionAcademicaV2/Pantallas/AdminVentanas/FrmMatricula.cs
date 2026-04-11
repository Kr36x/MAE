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
        private int estudianteID = 0;
        private int matriculaID = 0;
        public FrmMatricula(PantallaAdmin principal)
        {
            InitializeComponent();
            pantallaPrincipal = principal;
        }
        public FrmMatricula(PantallaAdmin principal, int id) : this(principal)
        {
            estudianteID = id;
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
                    doc.Add(CrearBarraTitulo("INFORMACIÓN DE LOS TUTORES", boldFont));

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

        //private void CargarDatosParaEditar(int id)
        //{
        //    try
        //    {
        //        EjecutarUtilidades util = new EjecutarUtilidades();

        //        SqlParameter[] p =
        //        {
        //                new SqlParameter("@EstudianteID", id)
        //            };

        //        DataTable dt = util.EjecutarSPParametros("spMAE_TraeMatriculaPorEstudiante", p);

        //        if (dt.Rows.Count == 0) return;

        //        DataRow row = dt.Rows[0];

        //        // ============================
        //        // ESTUDIANTE
        //        // ============================
        //        txtIdentidadEstudiante.Text = row["DniEst"].ToString();
        //        txtNombreEstudiante.Text = row["NombreEst"].ToString();
        //        txtTelefono.Text = row["TelEst"].ToString();
        //        txtDireccion.Text = row["Direccion"].ToString();
        //        dtpFechaNacimiento.Value = Convert.ToDateTime(row["FechaNacimiento"]);
        //        string sexo = row["Sexo"].ToString();
        //        if (sexo == "M")
        //            cbbGenero.Text = "MASCULINO";
        //        else if (sexo == "F")
        //            cbbGenero.Text = "FEMENINO";
        //        else
        //            cbbGenero.Text = "";
        //        cbbMano.Text = row["Mano"].ToString();
        //        txtAlergias.Text = row["Alergia"].ToString();
        //        cbbGrado.SelectedValue = Convert.ToInt32(row["GradoID"]);
        //        // ============================
        //        // TUTOR 1
        //        // ============================
        //        txtNombrePadre.Text = row["NombreTut1"].ToString();
        //        txtIdentidadPadre.Text = row["DniTut1"].ToString();
        //        txtTelefonoPadre.Text = row["TelTut1"].ToString();
        //        txtCorreoPadre.Text = row["CorreoTut1"].ToString();
        //        txtTrabajoPadre.Text = row["LugarTrabTut1"].ToString();
        //        cbbParentescoPadre.Text = row["ParentescoTut1"].ToString();
        //        // ============================
        //        // TUTOR 2
        //        // ============================
        //        txtNombreMadre.Text = row["NombreTut2"].ToString();
        //        txtIdentidadMadre.Text = row["DniTut2"].ToString();
        //        txtTelefonoMadre.Text = row["TelTut2"].ToString();
        //        txtCorreoMadre.Text = row["CorreoTut2"].ToString();
        //        txtTrabajoMadre.Text = row["LugarTrabTut2"].ToString();
        //        cbbParentescoMadre.Text = row["ParentescoTut2"].ToString();
        //        // ============================
        //        // MATRÍCULA
        //        // ============================
        //        matriculaID = Convert.ToInt32(row["MatriculaID"]);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error al cargar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void FrmMatricula_Load(object sender, EventArgs e)
        {
            CargarGrados();
            CargarSexo();
            //if (estudianteID > 0)
            //{
            //    CargarDatosParaEditar(estudianteID);
            //    txtNombreMadre.Enabled = false;
            //    txtNombrePadre.Enabled = false;
            //    txtIdentidadPadre.Enabled = false;
            //    txtIdentidadMadre.Enabled = false;
            //    txtCorreoPadre.Enabled = false;
            //    txtCorreoMadre.Enabled = false;
            //    txtTelefonoPadre.Enabled = false;
            //    txtTelefonoMadre.Enabled = false;
            //    cbbParentescoPadre.Enabled = false;
            //    cbbParentescoMadre.Enabled = false;
            //    txtTrabajoMadre.Enabled = false;
            //    txtTrabajoPadre.Enabled = false;
            //}
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
                MessageBox.Show("Solo se permiten números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtAviso2_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void btMatricular_Click(object sender, EventArgs e)
        {
            try
            {
                EjecutarUtilidades util = new EjecutarUtilidades();
                // Validaciones
                string correo = txtCorreoPadre.Text.Trim();

                if (!correo.Contains("@") || !correo.Contains("."))
                {
                    MessageBox.Show("Ingrese un correo válido. Ejemplo: usuario@dominio.com",
                                    "Validación",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);

                    txtCorreoPadre.Focus();
                }

                if (string.IsNullOrWhiteSpace(txtNombreEstudiante.Text))
                {
                    MessageBox.Show("Debe ingresar el nombre del estudiante.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtIdentidadEstudiante.Text))
                {
                    MessageBox.Show("Debe ingresar la identidad del estudiante.");
                    return;
                }

                if (cbbGrado.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe seleccionar un grado.");
                    return;
                }

                DataTable dtSeccion = util.EjecutarConsulta(
                    "SELECT TOP 1 Letra FROM Seccion WHERE GradoID = " + cbbGrado.SelectedValue);

                if (dtSeccion.Rows.Count == 0)
                {
                    MessageBox.Show("No existe sección para este grado.");
                    return;
                }

                string seccionLetra = dtSeccion.Rows[0]["Letra"].ToString();

                SqlParameter[] p =
                {
                    new SqlParameter("@nombreEst", txtNombreEstudiante.Text),
                    new SqlParameter("@fechaNacimiento", dtpFechaNacimiento.Value),
                    new SqlParameter("@sexo", cbbGenero.SelectedValue),
                    new SqlParameter("@dniEst", txtIdentidadEstudiante.Text),
                    new SqlParameter("@direccionEst", txtDireccion.Text),
                    new SqlParameter("@telEst", txtTelefono.Text),
                    new SqlParameter("@mano", cbbMano.Text),
                    new SqlParameter("@alergia", txtAlergias.Text),

                    new SqlParameter("@imagen", DBNull.Value),

                    new SqlParameter("@gradoID", Convert.ToInt32(cbbGrado.SelectedValue)),
                    new SqlParameter("@seccionID", seccionLetra),

                    new SqlParameter("@nombreTut1", txtNombrePadre.Text),
                    new SqlParameter("@dniTut1", txtIdentidadPadre.Text),
                    new SqlParameter("@telTut1", txtTelefonoPadre.Text),
                    new SqlParameter("@lugTrabTut1", txtTrabajoPadre.Text),
                    new SqlParameter("@correoTut1", txtCorreoPadre.Text),
                    new SqlParameter("@parentescoTut1", cbbParentescoPadre.Text),

                    new SqlParameter("@nombreTut2", string.IsNullOrWhiteSpace(txtNombreMadre.Text) ? (object)DBNull.Value : txtNombreMadre.Text),
                    new SqlParameter("@dniTut2", string.IsNullOrWhiteSpace(txtIdentidadMadre.Text) ? (object)DBNull.Value : txtIdentidadMadre.Text),
                    new SqlParameter("@telTut2", string.IsNullOrWhiteSpace(txtTelefonoMadre.Text) ? (object)DBNull.Value : txtTelefonoMadre.Text),
                    new SqlParameter("@lugTrabTut2", string.IsNullOrWhiteSpace(txtTrabajoMadre.Text) ? (object)DBNull.Value : txtTrabajoMadre.Text),
                    new SqlParameter("@correoTut2", string.IsNullOrWhiteSpace(txtCorreoMadre.Text) ? (object)DBNull.Value : txtCorreoMadre.Text),
                    new SqlParameter("@parentescoTut2", string.IsNullOrWhiteSpace(cbbParentescoMadre.Text) ? (object)DBNull.Value : cbbParentescoMadre.Text),

                    new SqlParameter("@matriculaID", matriculaID == 0 ? (object)DBNull.Value : matriculaID)
                };

                DataTable dt = util.EjecutarSPParametros("spMAE_Matricular", p);

                if (dt.Rows.Count > 0)
                {
                    matriculaID = Convert.ToInt32(dt.Rows[0][0]);
                }

                MessageBox.Show("Matrícula guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar matrícula: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtIdentidadEstudiante_TextChanged(object sender, EventArgs e)
        {
            int cursor = txtIdentidadEstudiante.SelectionStart;

            string limpio = new string(txtIdentidadEstudiante.Text.Where(char.IsDigit).ToArray());

            if (limpio.Length > 13)
                limpio = limpio.Substring(0, 13);

            string formateado = limpio;

            if (limpio.Length > 4)
                formateado = limpio.Insert(4, "-");


            if (limpio.Length > 8)
                formateado = formateado.Insert(9, "-");

            int diff = formateado.Length - txtIdentidadEstudiante.Text.Length;

            txtIdentidadEstudiante.TextChanged -= txtIdentidadEstudiante_TextChanged;
            txtIdentidadEstudiante.Text = formateado;
            txtIdentidadEstudiante.TextChanged += txtIdentidadEstudiante_TextChanged;

            txtIdentidadEstudiante.SelectionStart = Math.Max(0, cursor + diff);


        }

        private void txtNombreEstudiante_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;

                MessageBox.Show("Solo se aceptan letras.",
                                "Validación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }

        }

        private void txtAlergias_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;

                MessageBox.Show("Solo se aceptan letras.",
                                "Validación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void txtNombrePadre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;

                MessageBox.Show("Solo se aceptan letras.",
                                "Validación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void txtNombreMadre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNombreMadre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;

                MessageBox.Show("Solo se aceptan letras.",
                                "Validación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void txtTrabajoPadre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;

                MessageBox.Show("Solo se aceptan letras.",
                                "Validación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void txtTrabajoMadre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;

                MessageBox.Show("Solo se aceptan letras.",
                                "Validación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void txtTelefono_TextChanged(object sender, EventArgs e)
        {
            if (txtTelefono.Text.Length == 4 && !txtTelefono.Text.Contains("-"))
            {
                txtTelefono.Text += "-";
                txtTelefono.SelectionStart = txtTelefono.Text.Length;
            }
        }

        private void txtTelefonoPadre_TextChanged(object sender, EventArgs e)
        {
            if (txtTelefonoPadre.Text.Length == 4 && !txtTelefono.Text.Contains("-"))
            {
                txtTelefonoPadre.Text += "-";
                txtTelefonoPadre.SelectionStart = txtTelefono.Text.Length;
            }
        }

        private void txtTelefonoMadre_TextChanged(object sender, EventArgs e)
        {
            if (txtTelefonoMadre.Text.Length == 4 && !txtTelefono.Text.Contains("-"))
            {
                txtTelefonoMadre.Text += "-";
                txtTelefonoMadre.SelectionStart = txtTelefono.Text.Length;
            }
        }

        private void txtIdentidadPadre_TextChanged(object sender, EventArgs e)
        {
            int cursor = txtIdentidadPadre.SelectionStart;

            string limpio = new string(txtIdentidadPadre.Text.Where(char.IsDigit).ToArray());

            if (limpio.Length > 13)
                limpio = limpio.Substring(0, 13);

            string formateado = limpio;

            if (limpio.Length > 4)
                formateado = limpio.Insert(4, "-");


            if (limpio.Length > 8)
                formateado = formateado.Insert(9, "-");

            int diff = formateado.Length - txtIdentidadPadre.Text.Length;

            txtIdentidadPadre.TextChanged -= txtIdentidadPadre_TextChanged;
            txtIdentidadPadre.Text = formateado;
            txtIdentidadPadre.TextChanged += txtIdentidadPadre_TextChanged;

            txtIdentidadPadre.SelectionStart = Math.Max(0, cursor + diff);
        }

        private void txtIdentidadMadre_TextChanged(object sender, EventArgs e)
        {
            int cursor = txtIdentidadMadre.SelectionStart;

            string limpio = new string(txtIdentidadMadre.Text.Where(char.IsDigit).ToArray());

            if (limpio.Length > 13)
                limpio = limpio.Substring(0, 13);

            string formateado = limpio;

            if (limpio.Length > 4)
                formateado = limpio.Insert(4, "-");


            if (limpio.Length > 8)
                formateado = formateado.Insert(9, "-");

            int diff = formateado.Length - txtIdentidadMadre.Text.Length;

            txtIdentidadMadre.TextChanged -= txtIdentidadMadre_TextChanged;
            txtIdentidadMadre.Text = formateado;
            txtIdentidadMadre.TextChanged += txtIdentidadMadre_TextChanged;

            txtIdentidadMadre.SelectionStart = Math.Max(0, cursor + diff);
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Solo se permiten números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void txtTelefonoPadre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Solo se permiten números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void txtTelefonoMadre_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validación para que en el textbox TeléfonoMadre solo puedan ingresarse números.
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Solo se permiten números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void dtpFechaNacimiento_ValueChanged(object sender, EventArgs e)
        {
            // Validación para verificar que el estudiante no tenga menos de 4 años al ingresar a la institución.
            DateTime fechaNac = dtpFechaNacimiento.Value;
            int edad = DateTime.Now.Year - fechaNac.Year;

            if (fechaNac.Date > DateTime.Now.AddYears(-edad))
                edad--;

            if (edad < 4)
            {
                dtpFechaNacimiento.Focus();
                MessageBox.Show("El estudiante debe tener al menos 5 años.",
                                "Validación",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }

        }

        private void txtCorreoMadre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtIdentidadPadre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Solo se permiten números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtIdentidadMadre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
                return;

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Solo se permiten números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}