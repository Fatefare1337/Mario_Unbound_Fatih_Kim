namespace Mario_Unbound
{
    public partial class Form1 : Form
    {
        bool angemeldet = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Btn_Schlieﬂen_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Btn_Profil_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();

            if (angemeldet == false)
            {
                
                Label lbl_Benutzername = new Label();
                lbl_Benutzername.Text = "Benutzername:";

                Controls.Add(lbl_Benutzername);
                lbl_Benutzername.AutoSize = true;
                lbl_Benutzername.Top = 40;
                lbl_Benutzername.Left = 20;

                TextBox txb_Benutzername = new TextBox();
                
                Controls.Add(txb_Benutzername);
                txb_Benutzername.Size = new Size(140, 20);
                txb_Benutzername.Top = 40;
                txb_Benutzername.Left = 140;

                //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - - - - - - -

                Label lbl_EMail = new Label();
                lbl_EMail.Text = "E-Mail:";

                Controls.Add(lbl_EMail);
                lbl_EMail.AutoSize = true;
                lbl_EMail.Top = 80;
                lbl_EMail.Left = 20;

                TextBox txb_Email = new TextBox();

                Controls.Add(txb_Email);
                txb_Email.Size = new Size(140, 20);
                txb_Email.Top = 80;
                txb_Email.Left = 140;

                //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

                Label lbl_Passwort = new Label();
                lbl_Passwort.Text = "Passwort:";

                Controls.Add(lbl_Passwort);
                lbl_Passwort.AutoSize = true;
                lbl_Passwort.Top = 120;
                lbl_Passwort.Left = 20;

                TextBox txb_Passwort = new TextBox();

                Controls.Add(txb_Passwort);
                txb_Passwort.Size = new Size(140, 20);
                txb_Passwort.Top = 120;
                txb_Passwort.Left = 140;
               
                //- - - - - - - - - - - - - - - - - - - - - -  - - - - - - - -  - - - - - - -  - - - - - - - - - - -

                ComboBox cmb_Avatarbild = new ComboBox();
                Controls.Add(cmb_Avatarbild);

                cmb_Avatarbild.Items.Add("Avatar Frau");
                cmb_Avatarbild.Items.Add("Avatar Mann");
                cmb_Avatarbild.Items.Add("Avatar Dino");

                //PROBLEM: zeigt das Bild noch nicht an!

                    if (cmb_Avatarbild.SelectedIndex == 0)
                    {
                        PictureBox pb_Frau = new PictureBox();
                        pb_Frau.Image = Image.FromFile("Avatar Frau.jpg");
                        Controls.Add(pb_Frau);

                        pb_Frau.SizeMode = PictureBoxSizeMode.StretchImage;
                        pb_Frau.Top = 160;
                        pb_Frau.Left = 50;
                        pb_Frau.Show();

                    
                    }

                    else if (cmb_Avatarbild.SelectedItem == "Avatar Frau")
                    {
                        PictureBox pb_Mann = new PictureBox();
                        pb_Mann.Image = Image.FromFile("user Bild.png");
                        Controls.Add(pb_Mann);

                        pb_Mann.SizeMode = PictureBoxSizeMode.StretchImage;
                        pb_Mann.Top = 160;
                        pb_Mann.Left = 50;
                        pb_Mann.Show();

                    }

                    else
                    {
                        PictureBox pb_Dino = new PictureBox();
                        pb_Dino.Image = Image.FromFile("Avatar Frau.jpg");
                        Controls.Add(pb_Dino);

                        pb_Dino.SizeMode = PictureBoxSizeMode.StretchImage;
                        pb_Dino.Top = 160;
                        pb_Dino.Left = 50;
                        pb_Dino.Show();
                    }

            }
            else
            {
                //wenn angemeldet dann neues Profil anzeigen. 
                //Checken mit textdokument
            }

        }
    }
}
        
