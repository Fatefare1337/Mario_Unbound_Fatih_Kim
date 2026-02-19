using System.Security.Cryptography.X509Certificates;

namespace Mario_Unbound
{
    public partial class Form1 : Form
    {

        /*
         * Kim stunden: ca. 2 Stunden
         *Fatih stunden: ca. 
         */

        bool angemeldet = false;
        ComboBox cmb_Avatarbild;
        PictureBox picture;
        public Form1()
        {

            InitializeComponent();
            ClientSize = new Size(800, 500);

        }

        private void Btn_Schließen_Click(object sender, EventArgs e)
        {
            this.Close();
        } //Fertig

        private void Btn_Profil_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            ClientSize = new Size(800, 500);

            if (angemeldet == false)
            {

                Label label = new Label();
                Label lbl_Benutzername = new Label();
                lbl_Benutzername.Text = "Benutzername:";

                Controls.Add(lbl_Benutzername);
                lbl_Benutzername.AutoSize = true;
                lbl_Benutzername.Top = 60;
                lbl_Benutzername.Left = 20;

                TextBox txb_Benutzername = new TextBox();

                Controls.Add(txb_Benutzername);
                txb_Benutzername.Size = new Size(140, 20);
                txb_Benutzername.Top = 60;
                txb_Benutzername.Left = 140;

                //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - - - - - - -

                Label lbl_EMail = new Label();
                lbl_EMail.Text = "E-Mail:";

                Controls.Add(lbl_EMail);
                lbl_EMail.AutoSize = true;
                lbl_EMail.Top = 120;
                lbl_EMail.Left = 20;

                TextBox txb_Email = new TextBox();

                Controls.Add(txb_Email);
                txb_Email.Size = new Size(140, 20);
                txb_Email.Top = 120;
                txb_Email.Left = 140;

                //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

                Label lbl_Passwort = new Label();
                lbl_Passwort.Text = "Passwort:";

                Controls.Add(lbl_Passwort);
                lbl_Passwort.AutoSize = true;
                lbl_Passwort.Top = 180;
                lbl_Passwort.Left = 20;
                

                TextBox txb_Passwort = new TextBox();

                Controls.Add(txb_Passwort);
                txb_Passwort.Size = new Size(140, 20);
                txb_Passwort.Top = 180;
                txb_Passwort.Left = 140;
                txb_Passwort.UseSystemPasswordChar = true;

                //- - - - - - - - - - - - - - - - - - - - - -  - - - - - - - -  - - - - - - -  - - - - - - - - - - -

                cmb_Avatarbild = new ComboBox();
                Controls.Add(cmb_Avatarbild);

                cmb_Avatarbild.Items.Add("Avatar Frau");
                cmb_Avatarbild.Items.Add("Avatar Mann");
                cmb_Avatarbild.Items.Add("Avatar Dino");

                cmb_Avatarbild.Top = 30;
                cmb_Avatarbild.Left = 500;
                cmb_Avatarbild.SelectedIndexChanged += Cmb_Avatarbild_SelectedIndexChanged;

                cmb_Avatarbild.SelectedIndex = 2;

            //- - - - -  - - -  - - - - - - - - -  - - - - -  - - -   - - - - - - - - - - - - - - - - - - - - - - - - - - -  - -- - - -
            
                Button Registrieren = new Button();

                Registrieren.BackColor = Color.White;
                Registrieren.ForeColor = Color.Black;
                Registrieren.Size = new Size(100, 30);
                Registrieren.Text = "Registrieren";
                Registrieren.Top = 400;
                Registrieren.Left = (ClientSize.Width - Registrieren.Width) / 2;
                Controls.Add(Registrieren);

                Registrieren.Click += Registrieren_Click;
            }
            else
            {
                //wenn angemeldet dann neues Profil anzeigen. 
                //Checken mit textdokument
            }

        } //in bearbeitung

        private void Registrieren_Click(object? sender, EventArgs e)
        {
            //ins Textformular 
        } //in bearbeitung

        private void Cmb_Avatarbild_SelectedIndexChanged(object? sender, EventArgs e)
        {
            
            if (cmb_Avatarbild.SelectedIndex == 0)
            {
                Controls.Remove(picture);
                picture = new PictureBox();
                picture.Image = Image.FromFile("Frau_Avatar.png");
                Controls.Add(picture);


                picture.Size = new Size(200, 200);
                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.Top = 100;
                picture.Left = 500;
                picture.Show();


            }

            else if (cmb_Avatarbild.SelectedIndex == 1)
            {
                Controls.Remove(picture);
                picture = new PictureBox();
                picture.Image = Image.FromFile("Mann_Avatar.png");
                Controls.Add(picture);

                picture.Size = new Size(200, 200);
                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.Top = 100;
                picture.Left = 500;
                picture.Show();

            }

            //TODO:
            //abspeichern der ergebnisse wenn speichern / registieren geklickt wird.

            else
            {
                Controls.Remove(picture);
                picture = new PictureBox();
                picture.Image = Image.FromFile("Dino_Avatar.png");
                Controls.Add(picture);

                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.Size = new Size(200, 200);
                picture.Top = 100;
                picture.Left = 500;
                picture.Show();
            }
        } //fertig

        private void Btn_Team_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();

            

        }


        //TODO: Profil
        //Zurück Button
        //Passwort vielleich verschleiern
    }
}
        
