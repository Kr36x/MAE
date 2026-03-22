namespace GestionAcademicaV2.Pantallas.DocenteVentanas
{
    partial class FrmSeleccionReportes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnAsistencia = new Guna.UI2.WinForms.Guna2Button();
            guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel3 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnCalificaciones = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.Location = new Point(418, 40);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(180, 27);
            guna2HtmlLabel1.TabIndex = 0;
            guna2HtmlLabel1.Text = "Reportes Disponibles";
            // 
            // btnAsistencia
            // 
            btnAsistencia.CustomizableEdges = customizableEdges1;
            btnAsistencia.DisabledState.BorderColor = Color.DarkGray;
            btnAsistencia.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAsistencia.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAsistencia.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAsistencia.Font = new Font("Segoe UI", 9F);
            btnAsistencia.ForeColor = Color.White;
            btnAsistencia.Location = new Point(119, 109);
            btnAsistencia.Name = "btnAsistencia";
            btnAsistencia.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnAsistencia.Size = new Size(180, 45);
            btnAsistencia.TabIndex = 1;
            btnAsistencia.Text = "Asistencia Diaria";
            btnAsistencia.Click += btnReporteAsistencia_Click;
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.Location = new Point(321, 122);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(365, 17);
            guna2HtmlLabel2.TabIndex = 2;
            guna2HtmlLabel2.Text = "Presentar registro de puntualidad y faltas justificadas o injustificadas. ";
            // 
            // guna2HtmlLabel3
            // 
            guna2HtmlLabel3.BackColor = Color.Transparent;
            guna2HtmlLabel3.Location = new Point(321, 184);
            guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            guna2HtmlLabel3.Size = new Size(291, 17);
            guna2HtmlLabel3.TabIndex = 4;
            guna2HtmlLabel3.Text = "Reporte detallado (con desglose de tareas y conducta).";
            // 
            // btnCalificaciones
            // 
            btnCalificaciones.CustomizableEdges = customizableEdges3;
            btnCalificaciones.DisabledState.BorderColor = Color.DarkGray;
            btnCalificaciones.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCalificaciones.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCalificaciones.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCalificaciones.Font = new Font("Segoe UI", 9F);
            btnCalificaciones.ForeColor = Color.White;
            btnCalificaciones.Location = new Point(119, 171);
            btnCalificaciones.Name = "btnCalificaciones";
            btnCalificaciones.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnCalificaciones.Size = new Size(180, 45);
            btnCalificaciones.TabIndex = 3;
            btnCalificaciones.Text = "Cuadro Calificaciones ";
            btnCalificaciones.Click += btnCalificaciones_Click;
            // 
            // FrmSeleccionReportes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1012, 568);
            Controls.Add(guna2HtmlLabel3);
            Controls.Add(btnCalificaciones);
            Controls.Add(guna2HtmlLabel2);
            Controls.Add(btnAsistencia);
            Controls.Add(guna2HtmlLabel1);
            Name = "FrmSeleccionReportes";
            Text = "FrmReportes";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2Button btnAsistencia;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel3;
        private Guna.UI2.WinForms.Guna2Button btnCalificaciones;
    }
}