namespace Mario_Unbound
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            Btn_Start = new Button();
            Bnl_Level = new Button();
            Btn_Team = new Button();
            Btn_Profil = new Button();
            Btn_Schließen = new Button();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // Btn_Start
            // 
            Btn_Start.Location = new Point(37, 190);
            Btn_Start.Name = "Btn_Start";
            Btn_Start.Size = new Size(94, 29);
            Btn_Start.TabIndex = 7;
            Btn_Start.Text = "Start";
            Btn_Start.UseVisualStyleBackColor = true;
            // 
            // Bnl_Level
            // 
            Bnl_Level.Location = new Point(37, 235);
            Bnl_Level.Name = "Bnl_Level";
            Bnl_Level.Size = new Size(94, 29);
            Bnl_Level.TabIndex = 8;
            Bnl_Level.Text = "Level";
            Bnl_Level.UseVisualStyleBackColor = true;
            // 
            // Btn_Team
            // 
            Btn_Team.Location = new Point(37, 282);
            Btn_Team.Name = "Btn_Team";
            Btn_Team.Size = new Size(94, 29);
            Btn_Team.TabIndex = 9;
            Btn_Team.Text = "Team";
            Btn_Team.UseVisualStyleBackColor = true;
            // 
            // Btn_Profil
            // 
            Btn_Profil.Location = new Point(37, 326);
            Btn_Profil.Name = "Btn_Profil";
            Btn_Profil.Size = new Size(94, 29);
            Btn_Profil.TabIndex = 10;
            Btn_Profil.Text = "Profil";
            Btn_Profil.UseVisualStyleBackColor = true;
            Btn_Profil.Click += Btn_Profil_Click;
            // 
            // Btn_Schließen
            // 
            Btn_Schließen.Location = new Point(37, 373);
            Btn_Schließen.Name = "Btn_Schließen";
            Btn_Schließen.Size = new Size(94, 29);
            Btn_Schließen.TabIndex = 11;
            Btn_Schließen.Text = "Schließen";
            Btn_Schließen.UseVisualStyleBackColor = true;
            Btn_Schließen.Click += Btn_Schließen_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(12, -1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(216, 172);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 12;
            pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBox1);
            Controls.Add(Btn_Schließen);
            Controls.Add(Btn_Profil);
            Controls.Add(Btn_Team);
            Controls.Add(Bnl_Level);
            Controls.Add(Btn_Start);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button Btn_Start;
        private Button Bnl_Level;
        private Button Btn_Team;
        private Button Btn_Profil;
        private Button Btn_Schließen;
        private PictureBox pictureBox1;
    }
}
