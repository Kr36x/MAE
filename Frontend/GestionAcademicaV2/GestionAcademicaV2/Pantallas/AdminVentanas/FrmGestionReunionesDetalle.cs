using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GestionAcademicaV2.Pantallas.AdminVentanas
{
    public enum TipoVistaDetalleReunion
    {
        Admin,
        Docente
    }

    public partial class FrmGestionReunionesDetalle : Form
    {
        private readonly string _fechaHora;
        private readonly string _docente;
        private readonly string _estudiante;
        private readonly string _gradoSeccion;
        private readonly string _tema;
        private readonly string _medio;
        private readonly string _estado;
        private readonly TipoVistaDetalleReunion _tipoVista;

        public FrmGestionReunionesDetalle(
            string fechaHora,
            string docente,
            string estudiante,
            string gradoSeccion,
            string tema,
            string medio,
            string estado,
            TipoVistaDetalleReunion tipoVista = TipoVistaDetalleReunion.Admin)
        {
            InitializeComponent();

            _fechaHora = fechaHora;
            _docente = docente;
            _estudiante = estudiante;
            _gradoSeccion = gradoSeccion;
            _tema = tema;
            _medio = medio;
            _estado = estado;
            _tipoVista = tipoVista;

            ConstruirVista();
        }

        private void ConstruirVista()
        {
            SuspendLayout();

            Color colorPrincipal;
            Color colorSubtitulo;

            if (_tipoVista == TipoVistaDetalleReunion.Docente)
            {
                colorPrincipal = Color.FromArgb(89, 177, 89);
                colorSubtitulo = Color.FromArgb(220, 245, 220);
            }
            else
            {
                colorPrincipal = Color.FromArgb(24, 105, 255);
                colorSubtitulo = Color.FromArgb(230, 238, 255);
            }

            Text = "Detalle de reunión";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(620, 520);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = colorPrincipal
            };

            Label lblTitulo = new Label
            {
                Text = "DETALLE DE LA REUNIÓN",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(24, 14)
            };

            Label lblSubtitulo = new Label
            {
                Text = "Consulta rápida de la información registrada",
                ForeColor = colorSubtitulo,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(25, 42)
            };

            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Controls.Add(lblSubtitulo);

            Panel card = new Panel
            {
                Location = new Point(22, 88),
                Size = new Size(576, 360),
                BackColor = Color.White
            };

            card.Paint += (s, e) =>
            {
                using Pen pen = new Pen(Color.FromArgb(225, 230, 235));
                Rectangle rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawRectangle(pen, rect);
            };

            int xLabel = 24;
            int xValue = 180;
            int y = 24;
            int salto = 46;

            AgregarCampo(card, "Fecha y hora", ValorSeguro(_fechaHora), xLabel, xValue, y);
            y += salto;

            AgregarCampo(card, "Docente", ValorSeguro(_docente), xLabel, xValue, y);
            y += salto;

            AgregarCampo(card, "Estudiante", ValorSeguro(_estudiante), xLabel, xValue, y);
            y += salto;

            AgregarCampo(card, "Grado y sección", ValorSeguro(_gradoSeccion), xLabel, xValue, y);
            y += salto;

            AgregarCampo(card, "Tema", ValorSeguro(_tema), xLabel, xValue, y, 340);
            y += 62;

            AgregarCampo(card, "Medio", ValorSeguro(_medio), xLabel, xValue, y);
            y += salto;

            AgregarEstado(card, "Estado", ValorSeguro(_estado), xLabel, xValue, y);

            Button btnCerrar = new Button
            {
                Text = "Cerrar",
                Size = new Size(120, 38),
                Location = new Point(478, 462),
                BackColor = colorPrincipal,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => Close();

            Controls.Add(panelHeader);
            Controls.Add(card);
            Controls.Add(btnCerrar);

            ResumeLayout(false);
        }

        private void AgregarCampo(
            Control contenedor,
            string etiqueta,
            string valor,
            int xEtiqueta,
            int xValor,
            int y,
            int anchoValor = 330)
        {
            Label lblEtiqueta = new Label
            {
                Text = etiqueta,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(90, 90, 90),
                AutoSize = true,
                Location = new Point(xEtiqueta, y + 2)
            };

            Label lblValor = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(35, 35, 35),
                AutoSize = false,
                Location = new Point(xValor, y),
                Size = new Size(anchoValor, 36)
            };

            contenedor.Controls.Add(lblEtiqueta);
            contenedor.Controls.Add(lblValor);
        }

        private void AgregarEstado(
            Control contenedor,
            string etiqueta,
            string estado,
            int xEtiqueta,
            int xValor,
            int y)
        {
            Label lblEtiqueta = new Label
            {
                Text = etiqueta,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(90, 90, 90),
                AutoSize = true,
                Location = new Point(xEtiqueta, y + 4)
            };

            Label badge = new Label
            {
                Text = estado.ToUpper(),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(xValor, y),
                Size = new Size(120, 30)
            };

            Color backColor = Color.FromArgb(245, 245, 245);
            Color foreColor = Color.FromArgb(90, 90, 90);

            switch (estado.Trim().ToUpper())
            {
                case "REALIZADA":
                    backColor = Color.FromArgb(220, 248, 228);
                    foreColor = Color.FromArgb(22, 163, 74);
                    break;

                case "PROGRAMADA":
                    backColor = Color.FromArgb(255, 243, 205);
                    foreColor = Color.FromArgb(180, 125, 0);
                    break;

                case "CANCELADA":
                    backColor = Color.FromArgb(255, 230, 230);
                    foreColor = Color.FromArgb(239, 68, 68);
                    break;
            }

            badge.BackColor = backColor;
            badge.ForeColor = foreColor;

            badge.Paint += (s, e) =>
            {
                Label lbl = (Label)s;
                using GraphicsPath path = ObtenerRectanguloRedondeado(
                    new Rectangle(0, 0, lbl.Width - 1, lbl.Height - 1), 14);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using SolidBrush brush = new SolidBrush(lbl.BackColor);
                e.Graphics.FillPath(brush, path);
                TextRenderer.DrawText(
                    e.Graphics,
                    lbl.Text,
                    lbl.Font,
                    new Rectangle(0, 0, lbl.Width, lbl.Height),
                    lbl.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            contenedor.Controls.Add(lblEtiqueta);
            contenedor.Controls.Add(badge);
        }

        private GraphicsPath ObtenerRectanguloRedondeado(Rectangle rect, int radio)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radio * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        private string ValorSeguro(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? "N/A" : valor.Trim();
        }
    }
}