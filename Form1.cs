using Microsoft.VisualBasic.ApplicationServices;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Mario_Unbound
{
    /*
    * Kim stunden: ca. 7 Stunden
    *Fatih stunden: ca. 7 Stunde
    *
    *probleme:
    *- bei email kann man kein @ dazuschreiben
     */
    public partial class Form1 : Form
    {
        bool SignedIn = false;
        ComboBox cmb_Profilpicture;
        PictureBox picture;
        PictureBox Logo;
        Button Btn_Start; Button Btn_Level; Button Btn_Team; Button Btn_Profil; Button Btn_Closing;
        public string _profilUsername, _profilEmail, _profiPassword;
        TextBox txb_Username, txb_Email, txb_Password;
        PictureBox pb_Mario, pb_Luigi, pb_Toad, pb_Waluigi;
        public int _currentLevel = 1;

        private string file = "proildaten.txt";

        public Form1()
        {
            InitializeComponent();
            ClientSize = new Size(800, 500);

            pb_Luigi = new PictureBox();
            pb_Toad = new PictureBox();
            pb_Mario = new PictureBox();
            pb_Waluigi = new PictureBox();
            Homepage();

            #region Charaktere

            Character Mario = new Character();
            Mario.chosenCharacters(pb_Mario, "Mario");

            Character Luigi = new Character();
            Luigi.chosenCharacters(pb_Luigi, "Luigi");

            Character Toad = new Character();
            Toad.chosenCharacters(pb_Toad, "Toad");

            Character Waluigi = new Character(); //ER STIRBT
            Waluigi.chosenCharacters(pb_Waluigi, "Waluigi");

            #endregion


            #region Gegner
            Enemy enemy1 = new Enemy();
            enemy1.BuildingEnemies(1);
            enemy1.MovingNonHuman();

            #endregion
        }

        #region OhneGame

        public void BackToPage()
        {
            Button Btn_Zurück = new Button();
            Btn_Zurück.BackColor = Color.Red;
            Btn_Zurück.ForeColor = Color.White;
            Btn_Zurück.Size = new Size(100, 30);
            Btn_Zurück.Text = "Zurück";
            Btn_Zurück.Top = 10;
            Btn_Zurück.Left = 10;
            Controls.Add(Btn_Zurück);

            Btn_Zurück.Click += Btn_Back_Click;
        }

        private void Cmb_ProfilePicture_SelectedIndexChanged(object? sender, EventArgs e)
        {

            if (cmb_Profilpicture.SelectedIndex == 0)
            {
                Controls.Remove(picture);
                picture = new PictureBox();
                picture.Image = Image.FromFile("Frau_Avatar.png");
                Controls.Add(picture);


                picture.Size = new Size(200, 200);
                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.Top = 100;
                picture.Left = 30;
                picture.Show();


            }

            else if (cmb_Profilpicture.SelectedIndex == 1)
            {
                Controls.Remove(picture);
                picture = new PictureBox();
                picture.Image = Image.FromFile("Mann_Avatar.png");
                Controls.Add(picture);

                picture.Size = new Size(200, 200);
                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.Top = 100;
                picture.Left = 30;
                picture.Show();

            }

            else
            {
                Controls.Remove(picture);
                picture = new PictureBox();
                picture.Image = Image.FromFile("Dino_Avatar.png");
                Controls.Add(picture);

                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.Size = new Size(200, 200);
                picture.Top = 100;
                picture.Left = 30;
                picture.Show();
            }
        } 

        #region Methoden
        protected void Closing()
        {
            this.Close();
        }

        protected void Profilpage()
        {
            this.Controls.Clear();
            ClientSize = new Size(800, 500);

            BackToPage();


            if (SignedIn == false)
            {


                Label lbl_Benutzername = new Label();
                lbl_Benutzername.Text = "Benutzername:";

                Controls.Add(lbl_Benutzername);
                lbl_Benutzername.AutoSize = true;
                lbl_Benutzername.Top = 60;
                lbl_Benutzername.Left = 20;

                txb_Username = new TextBox();


                Controls.Add(txb_Username);
                txb_Username.Size = new Size(140, 20);
                txb_Username.Top = 60;
                txb_Username.Left = 140;



                //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - - - - - - -

                Label lbl_EMail = new Label();
                lbl_EMail.Text = "E-Mail:";

                Controls.Add(lbl_EMail);
                lbl_EMail.AutoSize = true;
                lbl_EMail.Top = 120;
                lbl_EMail.Left = 20;

                txb_Email = new TextBox();


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


                txb_Password = new TextBox();


                Controls.Add(txb_Password);
                txb_Password.Size = new Size(140, 20);
                txb_Password.Top = 180;
                txb_Password.Left = 140;
                txb_Password.UseSystemPasswordChar = true;


                

               

                //- - - - -  - - -  - - - - - - - - -  - - - - -  - - -   - - - - - - - - - - - - - - - - - - - - - - - - - - -  - -- - - -

                Button Btn_Registrieren = new Button();

                Btn_Registrieren.BackColor = Color.White;
                Btn_Registrieren.ForeColor = Color.Black;
                Btn_Registrieren.Size = new Size(100, 30);
                Btn_Registrieren.Text = "Registrieren";
                Btn_Registrieren.Top = 400;
                Btn_Registrieren.Left = 350;
                Controls.Add(Btn_Registrieren);

                Btn_Registrieren.Click += SignUp_Click;

                //- - - - -  - - -  - - - - - - - - -  - - - - -  - - -   - - - - - - - - - - - - - - - - - - - - - - - - - - -  - -- - - -


                

                Button Btn_Anmelden = new Button();

                Btn_Anmelden.BackColor = Color.White;
                Btn_Anmelden.ForeColor = Color.Black;
                Btn_Anmelden.Size = new Size(100, 30);
                Btn_Anmelden.Text = "Anmelden";
                Btn_Anmelden.Top = 400;
                Btn_Anmelden.Left = 450;
                Controls.Add(Btn_Anmelden);

                Btn_Anmelden.Click += Btn_SignIn_Click;
            }

            else
            {
                cmb_Profilpicture = new ComboBox();
                Controls.Add(cmb_Profilpicture);

                cmb_Profilpicture.Items.Add("Avatar Frau");
                cmb_Profilpicture.Items.Add("Avatar Mann");
                cmb_Profilpicture.Items.Add("Avatar Dino");

                cmb_Profilpicture.Top = 40;
                cmb_Profilpicture.Left = 300;
                cmb_Profilpicture.SelectedIndexChanged += Cmb_ProfilePicture_SelectedIndexChanged;

                cmb_Profilpicture.SelectedIndex = 2;

                //- - - - - - - - - - - - - - - - - - - - - - - -  - - - - -  - - - - - - - - - - - - - - - - - -

                Label lbl_gespeicherterBenutzername = new Label();
                lbl_gespeicherterBenutzername.Text = $"Benutzername: {_profilUsername}";

                Controls.Add(lbl_gespeicherterBenutzername);
                lbl_gespeicherterBenutzername.AutoSize = true;
                lbl_gespeicherterBenutzername.Top = 100;
                lbl_gespeicherterBenutzername.Left = 300;
                lbl_gespeicherterBenutzername.Font = new Font(lbl_gespeicherterBenutzername.Font, FontStyle.Bold);

                //- - - - - - - - - - - - - - - - - - - - - - - -  - - - - -  - - - - - - - - - - - - - - - - - -

                Label lbl_gespeicherteEmail = new Label();
                lbl_gespeicherteEmail.Text = $"Email: {_profilEmail}";

                Controls.Add(lbl_gespeicherteEmail);
                lbl_gespeicherteEmail.AutoSize = true;
                lbl_gespeicherteEmail.Top = 150;
                lbl_gespeicherteEmail.Left = 300;
                lbl_gespeicherteEmail.Font = new Font(lbl_gespeicherteEmail.Font, FontStyle.Bold);

                //- - - - - - - - - - - - - - - - - - - - - - - -  - - - - -  - - - - - - - - - - - - - - - - - -

                //Foto von map unten oder impressum oder so 
            }

        } //in bearbeitung

        protected void Homepage()
        {

            Controls.Clear();

            Logo = new PictureBox();
            Controls.Add(Logo);
            Logo.Image = Image.FromFile("Mario Logo.png");
            Logo.Size = new Size(200, 200);
            Logo.SizeMode = PictureBoxSizeMode.Zoom;


            Btn_Start = new Button();
            Controls.Add(Btn_Start);

            Btn_Start.BackColor = Color.White;
            Btn_Start.ForeColor = Color.Black;
            Btn_Start.Size = new Size(100, 30);
            Btn_Start.Text = "Start";
            Btn_Start.Top = 220;
            Btn_Start.Left = 30;
            Controls.Add(Btn_Start);

            Btn_Start.Click += Btn_Start_Click;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - - - - - - - - - -  - - - - - -

            Btn_Level = new Button();
            Controls.Add(Btn_Level);

            Btn_Level.BackColor = Color.White;
            Btn_Level.ForeColor = Color.Black;
            Btn_Level.Size = new Size(100, 30);
            Btn_Level.Text = "Level";
            Btn_Level.Top = 260;
            Btn_Level.Left = 30;
            Controls.Add(Btn_Level);

            Btn_Level.Click += Btn_Level_Click;
            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

            Btn_Team = new Button();
            Controls.Add(Btn_Team);

            Btn_Team.BackColor = Color.White;
            Btn_Team.ForeColor = Color.Black;
            Btn_Team.Size = new Size(100, 30);
            Btn_Team.Text = "Team";
            Btn_Team.Top = 300;
            Btn_Team.Left = 30;
            Controls.Add(Btn_Team);

            Btn_Team.Click += Btn_Team_Click;

            //- - - - - - - - - - - - - - - -  - - - - - - - - - - - -  -- - -  - - - - - -  - - - - - -  - - - 

            Btn_Profil = new Button();
            Controls.Add(Btn_Profil);

            Btn_Profil.BackColor = Color.White;
            Btn_Profil.ForeColor = Color.Black;
            Btn_Profil.Size = new Size(100, 30);
            Btn_Profil.Text = "Profil";
            Btn_Profil.Top = 340;
            Btn_Profil.Left = 30;
            Controls.Add(Btn_Profil);

            Btn_Profil.Click += Btn_Profil_Click1;

            //- - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - - - - - - - - - - - - - - - - - - - - - 

            Btn_Closing = new Button();
            Controls.Add(Btn_Closing);

            Btn_Closing.BackColor = Color.White;
            Btn_Closing.ForeColor = Color.Black;
            Btn_Closing.Size = new Size(100, 30);
            Btn_Closing.Text = "Schließen";
            Btn_Closing.Top = 380;
            Btn_Closing.Left = 30;
            Controls.Add(Btn_Closing);

            Btn_Closing.Click += Btn_Closing_Click1;

            //- - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - - - - - - - - - - - - - - - - - - - - - 

            if (SignedIn == false)
            {
                Label lbl_Warning = new Label();
                lbl_Warning.Text = "Bitte melden Sie sich an, um Ihren Stand zu speichern!";
                Controls.Add(lbl_Warning);
                lbl_Warning.AutoSize = true;
                lbl_Warning.Top = 30;
                lbl_Warning.Left = 400;
                lbl_Warning.ForeColor = Color.Red;
                lbl_Warning.Font = new Font(lbl_Warning.Font, FontStyle.Bold);
            }
        }

        protected void Teamseite()
        {
            Controls.Clear();

            //TODO:
            //Bilder /Character der Teammitglieder hinzufügen.
            
            BackToPage();

            Label NameKim = new Label();
            NameKim.Text = "Kimberly Heinzl";

            Controls.Add(NameKim);
            NameKim.AutoSize = true;
            NameKim.Top = 300;
            NameKim.Left = 100;

            //- - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - 

            Label NameFatih = new Label();
            NameFatih.Text = "Fatih (Nachname)";

            Controls.Add(NameFatih);
            NameFatih.AutoSize = true;
            NameFatih.Top = 300;
            NameFatih.Left = 500;

            //- - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - 

            Label HourCountF = new Label();
            HourCountF.Text = "Gearbeitete Stunden: ";

            Controls.Add(HourCountF);
            HourCountF.AutoSize = true;
            HourCountF.Top = 330;
            HourCountF.Left = 500;

            //- - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - 

            Label HourCountK = new Label();
            HourCountK.Text = "Gearbeitete Stunden: ";

            Controls.Add(HourCountK);
            HourCountK.AutoSize = true;
            HourCountK.Top = 330;
            HourCountK.Left = 100;

            //- - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - 

            Label AreaF = new Label();
            AreaF.Text = "Gearbeitete Bereich: ";

            Controls.Add(AreaF);
            AreaF.AutoSize = true;
            AreaF.Top = 360;
            AreaF.Left = 500;

            //- - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - 

            Label AreaK = new Label();
            AreaK.Text = "Gearbeitete Bereich: ";

            Controls.Add(AreaK);
            AreaK.AutoSize = true;
            AreaK.Top = 360;
            AreaK.Left = 100;
        } //Ende Projekt stunden und fotos

        protected void Characters()
        {
            this.Controls.Clear();

            BackToPage();

            Label lbl_CharakterChoosing = new Label();
            lbl_CharakterChoosing.Text = "Choose your Character!";

            Controls.Add(lbl_CharakterChoosing);
            lbl_CharakterChoosing.Size = new Size(200, 30);
            lbl_CharakterChoosing.Top = 30;
            lbl_CharakterChoosing.Left = (ClientSize.Width - lbl_CharakterChoosing.Width) / 2;
            lbl_CharakterChoosing.Font = new Font(lbl_CharakterChoosing.Font, FontStyle.Bold);

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - - - -

            pb_Mario = new PictureBox();
            Controls.Add(pb_Mario);

            pb_Mario.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_Mario.Image = Image.FromFile("MarioAuswahl.png");
            pb_Mario.Size = new Size(180, 300);
            pb_Mario.Top = 100;
            pb_Mario.Left = 10;

            pb_Mario.Click += Pb_MarioAuswahl_Click;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - - - -

            pb_Luigi = new PictureBox();
            Controls.Add(pb_Luigi);

            pb_Luigi.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_Luigi.Image = Image.FromFile("MarioAuswahl.png");
            pb_Luigi.Size = new Size(180, 300);
            pb_Luigi.Top = 100;
            pb_Luigi.Left = 200;

            pb_Luigi.Click += Pb_Luigi_Click;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - - - -

            pb_Toad = new PictureBox();
            Controls.Add(pb_Toad);

            pb_Toad.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_Toad.Image = Image.FromFile("MarioAuswahl.png");
            pb_Toad.Size = new Size(180, 300);
            pb_Toad.Top = 100;
            pb_Toad.Left = 400;

            pb_Toad.Click += Pb_Toad_Click;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - - - -

            pb_Waluigi = new PictureBox();
            Controls.Add(pb_Waluigi);

            pb_Waluigi.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_Waluigi.Image = Image.FromFile("MarioAuswahl.png");
            pb_Waluigi.Size = new Size(180, 300);
            pb_Waluigi.Top = 100;
            pb_Waluigi.Left = 600;

            pb_Waluigi.Click += Pb_Waluigi_Click;
        } //Charakterbilder ändern dann fertig

        protected void Levelseite()
        {
            Controls.Clear();
            BackToPage();
        } //in bearbeitung

        #endregion

        #region Buttons
        private void Btn_Closing_Click1(object? sender, EventArgs e)
        {
            Closing();
        }

        private void Btn_Profil_Click1(object? sender, EventArgs e)
        {
            Profilpage();
        }

        private void Btn_Team_Click(object? sender, EventArgs e)
        {
            Teamseite();
        }

        private void Btn_Level_Click(object? sender, EventArgs e) //in bearbeitung
        {
            
        }

        private void SignUp_Click(object? sender, EventArgs e)
        {
            _profiPassword = txb_Password.Text;
            _profilUsername = txb_Username.Text;
            _profilEmail = txb_Email.Text;

            // Prüftt, ob die erforderlichen Felder ausgefüllt sind
            if (string.IsNullOrEmpty(_profilUsername) || string.IsNullOrEmpty(_profilEmail) || string.IsNullOrEmpty(_profiPassword))
            {
                MessageBox.Show("Bitte Name, Passwort und E-mail eingeben!");
                return;
            }
            //gemini code
            // Prüft, ob der Benutzername oder die E-Mail bereits in der Textdatei existiert
            if (File.Exists(file))
            {

                var zeilen = File.ReadAllLines(file);
                foreach (var zeile in zeilen)
                {
                    var benutzerDaten = zeile.Split('|'); // die Daten in der Textdatei sollten durch '|' getrennt sein, z.B. "Benutzername|Email|Passwort"; macht alles übersichtlicher
                    if (benutzerDaten.Length >= 3)
                    {
                        // Index [0] ist _profilBenutzername, Index [1] ist _profilEmail
                        if (benutzerDaten[0].ToLower() == _profilUsername.ToLower())
                        {
                            MessageBox.Show("Dieser Benutzername ist bereits vergeben!");
                            return;
                        }
                        if (benutzerDaten[1].ToLower() == _profilEmail.ToLower())
                        {
                            MessageBox.Show("Diese E-Mail wird bereits verwendet!");
                            return;
                        }
                    }
                }
            }

            

            File.AppendAllText(file, $"{_profilUsername}|{_profilEmail}|{_profiPassword}{Environment.NewLine}");
            MessageBox.Show("Registrierung erfolgreich!");

            txb_Password.Clear();
            txb_Email.Clear();
            txb_Username.Clear();



            SignedIn = true;
            Profilpage();

        }  

        private void Btn_SignIn_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_profilUsername) || string.IsNullOrEmpty(_profilEmail) || string.IsNullOrEmpty(_profiPassword))
            {
                MessageBox.Show("Bitte Name, Passwort und E-mail eingeben!");
                return;

                //wenn passwort, email und benutzername passen, dann angemeldet auf true.
                //Hier muss noch ein zweiter button hin
            }


        } //in anmeldung

        private void Btn_Back_Click(object? sender, EventArgs e)
        {
            Homepage();
        }


        private void Btn_Start_Click(object? sender, EventArgs e)
        {
            Characters();



        } //In bearbeitung



        #endregion



        #endregion

        #region Mitgame

        
        private void Pb_MarioAuswahl_Click(object? sender, EventArgs e)
        {

        }

        private void Pb_Luigi_Click(object? sender, EventArgs e)
        {
            
        }

        private void Pb_Toad_Click(object? sender, EventArgs e)
        {
            
        }

        private void Pb_Waluigi_Click(object? sender, EventArgs e)
        {
            
        }
        #endregion

       

    }
}

