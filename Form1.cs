using Microsoft.VisualBasic.ApplicationServices;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Mario_Unbound
{
    /*
    * Kim stunden: ca. 5 Stunden
    *Fatih stunden: ca. 4 1/2 Stunde
    *
    *probleme:
    *- bei email kann man kein @ dazuschreiben
    *Zweiter Button ntig für anmelden
     */
    public partial class Form1 : Form
    {
        bool angemeldet = false;
        ComboBox cmb_Avatarbild;
        PictureBox picture;
        PictureBox Logo;
        Button Btn_Start; Button Btn_Level; Button Btn_Team; Button Btn_Profil; Button Btn_Schließen;
        public string _profilBenutzername, _profilEmail, _profiPasswort;
        TextBox txb_Benutzername, txb_Email, txb_Passwort;
        PictureBox pb_Mario, pb_Luigi, pb_Toad, pb_Waluigi;

        private string dateiPfad = "proildaten.txt";

        public Form1()
        {
            InitializeComponent();
            ClientSize = new Size(800, 500);

            pb_Luigi = new PictureBox();
            pb_Toad = new PictureBox();
            pb_Mario = new PictureBox();
            pb_Waluigi = new PictureBox();
            Startseite();

            #region Charaktere

            Charakter Mario = new Charakter();
            Mario.augewählterCharakter(pb_Mario, "Mario");

            Charakter Luig = new Charakter();
            Luig.augewählterCharakter(pb_Luigi, "Luigi");

            Charakter Toad = new Charakter();
            Toad.augewählterCharakter(pb_Toad, "Toad");

            Charakter Waluigi = new Charakter();
            Waluigi.augewählterCharakter(pb_Waluigi, "Waluigi");

            #endregion

        }

        #region OhneGame

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
        }

        #region Methoden
        protected void Schließen()
        {
            this.Close();
        }

        protected void Profilseite()
        {
            this.Controls.Clear();
            ClientSize = new Size(800, 500);

            //Zurück Pfeil hinzufügen


            if (angemeldet == false)
            {


                Label lbl_Benutzername = new Label();
                lbl_Benutzername.Text = "Benutzername:";

                Controls.Add(lbl_Benutzername);
                lbl_Benutzername.AutoSize = true;
                lbl_Benutzername.Top = 60;
                lbl_Benutzername.Left = 20;

                txb_Benutzername = new TextBox();


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


                txb_Passwort = new TextBox();


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

                Button Btn_Registrieren = new Button();

                Btn_Registrieren.BackColor = Color.White;
                Btn_Registrieren.ForeColor = Color.Black;
                Btn_Registrieren.Size = new Size(100, 30);
                Btn_Registrieren.Text = "Registrieren";
                Btn_Registrieren.Top = 400;
                Btn_Registrieren.Left = (ClientSize.Width - Btn_Registrieren.Width) / 2;
                Controls.Add(Btn_Registrieren);

                Btn_Registrieren.Click += Registrieren_Click;


            }

            else
            {
                //wenn angemeldet dann neues Profil anzeigen. 
                //Checken mit textdokument
            }

        } //in bearbeitung

        private void Registrieren_Click(object? sender, EventArgs e)
        {
            _profiPasswort = txb_Passwort.Text;
            _profilBenutzername = txb_Benutzername.Text;
            _profilEmail = txb_Email.Text;

            // Prüftt, ob die erforderlichen Felder ausgefüllt sind
            if (string.IsNullOrEmpty(_profilBenutzername) || string.IsNullOrEmpty(_profilEmail) || string.IsNullOrEmpty(_profiPasswort))
            {
                MessageBox.Show("Bitte Name, Passwort und E-mail eingeben!");
                return;
            }
            //gemini code
            // Prüft, ob der Benutzername oder die E-Mail bereits in der Textdatei existiert
            if (File.Exists(dateiPfad))
            {

                var zeilen = File.ReadAllLines(dateiPfad);
                foreach (var zeile in zeilen)
                {
                    var benutzerDaten = zeile.Split('|'); // die Daten in der Textdatei sollten durch '|' getrennt sein, z.B. "Benutzername|Email|Passwort"; macht alles übersichtlicher
                    if (benutzerDaten.Length >= 3)
                    {
                        // Index [0] ist _profilBenutzername, Index [1] ist _profilEmail
                        if (benutzerDaten[0].ToLower() == _profilBenutzername.ToLower())
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

            //wenn existent kommt anmeldung erfolgreich, sonst das
            string benutzerdaten = $"{_profilBenutzername}|{_profilEmail}|{_profiPasswort}{Environment.NewLine}";
            File.AppendAllText(dateiPfad, benutzerdaten);
            MessageBox.Show("Registrierung erfolgreich!");

            txb_Passwort.Clear();
            txb_Email.Clear();
            txb_Benutzername.Clear();



            angemeldet = true;

        } //in bearbeitung


        protected void Startseite()
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

            Btn_Schließen = new Button();
            Controls.Add(Btn_Schließen);

            Btn_Schließen.BackColor = Color.White;
            Btn_Schließen.ForeColor = Color.Black;
            Btn_Schließen.Size = new Size(100, 30);
            Btn_Schließen.Text = "Schließen";
            Btn_Schließen.Top = 380;
            Btn_Schließen.Left = 30;
            Controls.Add(Btn_Schließen);

            Btn_Schließen.Click += Btn_Schließen_Click1;

            //- - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - - - - - - - - - - - - - - - - - - - - - 

            if (angemeldet == false)
            {
                Label lbl_Warnung = new Label();
                lbl_Warnung.Text = "Bitte melden Sie sich an, um Ihren Stand zu speichern!";
                Controls.Add(lbl_Warnung);
                lbl_Warnung.AutoSize = true;
                lbl_Warnung.Top = 30;
                lbl_Warnung.Left = 400;
                lbl_Warnung.ForeColor = Color.Red;
                lbl_Warnung.Font = new Font(lbl_Warnung.Font, FontStyle.Bold);
            }
        }

        protected void Teamseite()
        {
            Controls.Clear();

            //TODO:
            //Bilder /Character der Teammitglieder hinzufügen.
            //Zurück Pfeil hinzufügen

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

            Label Stundenanzahl = new Label();
            Stundenanzahl.Text = "Gearbeitete Stunden: ";

            Controls.Add(Stundenanzahl);
            Stundenanzahl.AutoSize = true;
            Stundenanzahl.Top = 330;
            Stundenanzahl.Left = 500;

            //- - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - 

            Label StundenanzahlK = new Label();
            StundenanzahlK.Text = "Gearbeitete Stunden: ";

            Controls.Add(StundenanzahlK);
            StundenanzahlK.AutoSize = true;
            StundenanzahlK.Top = 330;
            StundenanzahlK.Left = 100;

            //- - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - 

            Label Bereich = new Label();
            Bereich.Text = "Gearbeitete Stunden: ";

            Controls.Add(Bereich);
            Bereich.AutoSize = true;
            Bereich.Top = 360;
            Bereich.Left = 500;

            //- - - - - -- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  - - - - - - 

            Label BereichK = new Label();
            BereichK.Text = "Gearbeitete Stunden: ";

            Controls.Add(BereichK);
            BereichK.AutoSize = true;
            BereichK.Top = 360;
            BereichK.Left = 100;
        } //in bearbeitung

        protected void Charakterübersicht()
        {
            this.Controls.Clear();

            Label lbl_Charakterwahl = new Label();
            lbl_Charakterwahl.Text = "Choose your Character!";

            Controls.Add(lbl_Charakterwahl);
            lbl_Charakterwahl.Size = new Size(200, 30);
            lbl_Charakterwahl.Top = 30;
            lbl_Charakterwahl.Left = (ClientSize.Width - lbl_Charakterwahl.Width) / 2;
            lbl_Charakterwahl.Font = new Font(lbl_Charakterwahl.Font, FontStyle.Bold);

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








        #endregion

        #region Buttons
        private void Btn_Schließen_Click1(object? sender, EventArgs e)
        {
            Schließen();
        }

        private void Btn_Profil_Click1(object? sender, EventArgs e)
        {
            Profilseite();
        }

        private void Btn_Team_Click(object? sender, EventArgs e)
        {
            Teamseite();
        }

        private void Btn_Level_Click(object? sender, EventArgs e) //in bearbeitung
        {

        }

        private void Registrieren_Click(object? sender, EventArgs e)
        {
            _profiPasswort = txb_Passwort.Text;
            _profilBenutzername = txb_Benutzername.Text;
            _profilEmail = txb_Email.Text;

            // Prüftt, ob die erforderlichen Felder ausgefüllt sind
            if (string.IsNullOrEmpty(_profilBenutzername) || string.IsNullOrEmpty(_profilEmail) || string.IsNullOrEmpty(_profiPasswort))
            {
                MessageBox.Show("Bitte Name, Passwort und E-mail eingeben!");
                return;
            }
            //gemini code
            // Prüft, ob der Benutzername oder die E-Mail bereits in der Textdatei existiert
            if (File.Exists(dateiPfad))
            {

                var zeilen = File.ReadAllLines(dateiPfad);
                foreach (var zeile in zeilen)
                {
                    var benutzerDaten = zeile.Split('|'); // die Daten in der Textdatei sollten durch '|' getrennt sein, z.B. "Benutzername|Email|Passwort"; macht alles übersichtlicher
                    if (benutzerDaten.Length >= 3)
                    {
                        // Index [0] ist _profilBenutzername, Index [1] ist _profilEmail
                        if (benutzerDaten[0].ToLower() == _profilBenutzername.ToLower())
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

            //wenn existent kommt anmeldung erfolgreich, sonst das

            File.AppendAllText(dateiPfad, $"{_profilBenutzername}|{_profilEmail}|{_profiPasswort}{Environment.NewLine}");
            MessageBox.Show("Registrierung erfolgreich!");

            txb_Passwort.Clear();
            txb_Email.Clear();
            txb_Benutzername.Clear();



            angemeldet = true;

        } //in bearbeitung


        private void Btn_Start_Click(object? sender, EventArgs e)
        {
            Charakterübersicht();



        } //In bearbeitung



        #endregion



        #endregion

        #region Mitgame


        private void Pb_MarioAuswahl_Click(object? sender, EventArgs e)
        {

        }

        private void Pb_Luigi_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Pb_Toad_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Pb_Waluigi_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        //TODO: Profil
        //Zurück Button

    }
}

