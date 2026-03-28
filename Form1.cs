using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Timer = System.Windows.Forms.Timer;

namespace Mario_Unbound
{
    /*
    * Kim stunden: ca. 11,5 Stunden
    * Fatih stunden: ca. 13,5 Stunde
    *
    *probleme:
    *- bei email kann man kein @ dazuschreiben
    *Level abspeichern
    *idee: man kann level nicht auswählen, aber es gibt eine zurücksetzten button wo man seinen
    *fortschritt zurücksetzt
    *alles muss auf englisch gemacht werden
    *abspeichern current level on account
    *man kann von unten durch panel durchspringen, das sollte nicht sein
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
        //für Levelaufbau
        
        public List <Panel> flyingBlocks = new List <Panel>();
        bool touchfloor = true;
        public List<Panel> waterPanels = new List<Panel>();
        public List <Panel> enemyShots = new List <Panel>();


        
        public int _currentLevel = 1;
        PictureBox pb = new PictureBox();
        
        // new: animation images
        private Image runningGif;
        private Image runningGifLeft; 
        private Image idleImage; 
        private string _currentAnimation = "";
        private bool _wasLeftMovement = false;

        private string _file = "proildaten.txt";
        Character Mario = new Character();
        Character Luigi = new Character();
        Character Toad = new Character();
        Character Waluigi = new Character();

        // Spieler- und Bewegungsfelder
        private Panel player;
        private Panel floor;
        private Panel water;
        private Timer gameTimer;
        private bool _goLeft = false;
        private bool _goRight = false;
        // replaced jumpSpeed/jumping with vertical velocity
        private int _verticalMovement = 0;
        private int _jumpForce = 18;
        private int _gravity = 1; 
        private int _playerSpeed = 10;
        private bool _canJump = false;

        public Form1()
        {
            InitializeComponent();
            ClientSize = new Size(800, 500);

            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;

            pb_Luigi = new PictureBox();
            pb_Toad = new PictureBox();
            pb_Mario = new PictureBox();
            pb_Waluigi = new PictureBox();
            Homepage();

            #region Charaktere

            pb_Mario.Image = Image.FromFile("MarioAuswahl.png");
            // set idleImage to a smaller standing sprite if available; fallback to pb_Mario.Image
            idleImage = pb_Mario.Image;

            // try to load running gif and left-running gif (or create flipped clone)
            try
            {
                runningGif = Image.FromFile("Mario_running_full_life.gif");
            }
            catch (Exception)
            {
                runningGif = null;
            }

            try
            {
                runningGifLeft = Image.FromFile("Mario_running_full_life_left.gif");
            }
            catch (Exception)
            {
                // if explicit left gif not found, create flipped clone of runningGif if possible
                try
                {
                    if (runningGif != null)
                    {
                        runningGifLeft = (Image)runningGif.Clone();
                        runningGifLeft.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else
                    {
                        runningGifLeft = null;
                    }
                }
                catch
                {
                    runningGifLeft = null;
                }
            }

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
            Close();
        }

        protected void Profilpage()
        {
            Controls.Clear();
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

        } //in bearbeitung, Fortschritt zurücksetzen button

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
            Btn_Start.Top = 260;
            Btn_Start.Left = 30;
            Controls.Add(Btn_Start);

            Btn_Start.Click += Btn_Start_Click;

           

            
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
            Controls.Clear();

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
            if (File.Exists(_file))
            {

                var zeilen = File.ReadAllLines(_file); // ReadAllLines liest alle Zeilen der Textdatei und gibt sie als Array zurück.
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

            

            File.AppendAllText(_file, $"{_profilUsername}|{_profilEmail}|{_profiPassword}"); // AppendAllText ==> erstellt die Datei, falls sie noch nicht ertellt wurde, und fügt die Daten am Ende der Datei hinzu. So werden bestehende Daten nicht überschrieben.

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

        public void AufbauLevel1()
        {
            Controls.Clear();
            ClientSize = new Size(1600, 500);



            // Boden erstellen bzw. wiederverwenden
            
            floor = new Panel();
            floor.BackColor = Color.Green;
            if (!Controls.Contains(floor)) 
                Controls.Add(floor);
            floor.Size = new Size(ClientSize.Width, 50);
            floor.Location = new Point(0, ClientSize.Height - floor.Height);

            //Wasser erstellen 

            Panel water = new Panel();
            water.BackColor = Color.LightBlue;
            water.Size = new Size(100, 25);
            water.Location = new Point(200, 450);
            this.Controls.Add(water);
            water.BringToFront();
            waterPanels.Add(water);

            Panel water2 = new Panel();
            water2.BackColor = Color.LightBlue;
            water2.Size = new Size(300, 25);
            water2.Location = new Point(600, 450);
            this.Controls.Add(water2);
            water2.BringToFront();
            waterPanels.Add(water2);

            Panel water3 = new Panel();
            water3.BackColor = Color.LightBlue;
            water3.Size = new Size(100, 25);
            water3.Location = new Point(1000, 450);
            this.Controls.Add(water3);
            water3.BringToFront();
            waterPanels.Add(water3);

            //die fleigenden Blöcke erstelln

            Panel block1 = new Panel();
            block1.BackColor = Color.RosyBrown;
            block1.Size = new Size(150, 30);
            block1.Location = new Point(180, 300);
            this.Controls.Add(block1);
            flyingBlocks.Add(block1);

            Panel block2 = new Panel();
            block2.BackColor = Color.RosyBrown;
            block2.Size = new Size(300, 30);
            block2.Location = new Point(800, 300);
            this.Controls.Add(block2);
            flyingBlocks.Add(block2);

            //NPC panel
            Panel npc1 = new Panel();
            npc1.BackColor = Color.GreenYellow;
            npc1.Size = new Size(40, 60);
            npc1.Location = new Point(200, 300 - block1.Height); //sollte noch dynamisch werden
            Controls.Add(npc1);

            Panel npc2 = new Panel();
            npc2.BackColor = Color.GreenYellow;
            npc2.Size = new Size(40, 60);
            npc2.Location = new Point(400, 420); //sollte noch dynamisch werden
            Controls.Add(npc2);

            Panel npc3 = new Panel();
            npc3.BackColor = Color.GreenYellow;
            npc3.Size = new Size(40, 60);
            npc3.Location = new Point(450, 420); //sollte noch dynamisch werden
            Controls.Add(npc3);

            Panel npc4 = new Panel();
            npc4.BackColor = Color.GreenYellow;
            npc4.Size = new Size(40, 60);
            npc4.Location = new Point(950, 420); //sollte noch dynamisch werden
            Controls.Add(npc4);

            //gegner

            Panel e1 = new Panel();
            e1.BackColor = Color.IndianRed;
            e1.Size = new Size(200, 200);
            e1.Location = new Point(1400, 300); //sollte noch dynamisch werden
            Controls.Add(e1);

            //fireball

            Fireball fireballE = new Fireball();
            fireballE.BuildingFireball(Color.Orange);
            fireballE.Location = new Point(e1.Left, e1.Top + (e1.Height / 2));
            Controls.Add(fireballE);

            // Spielerpanel erstellen

            player = new Panel();

            pb.SizeMode = PictureBoxSizeMode.Zoom;
            player.Size = new Size(40, 60);
            player.Location = new Point(50, floor.Top - player.Height);
            if (!Controls.Contains(player))
                Controls.Add(player);

            // Game Timer 
            if (gameTimer == null)
            {
                gameTimer = new Timer();
                gameTimer.Interval = 20; // ~50 FPS
                gameTimer.Tick += GameTimer_Tick;
            }
            gameTimer.Start();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            if (player == null || floor == null) 
                return;

            // Horizontalbewegung
            bool wasMovingHorizontally = _goLeft || _goRight;

            if (_goLeft)
            {
                player.Left -= _playerSpeed;
            }
            if (_goRight)
            {
                player.Left += _playerSpeed;

            }
            if (player.Left < 0)
            {
                player.Left = 0;
            }
            if (player.Right > ClientSize.Width)
            {
                player.Left = ClientSize.Width - player.Width;
            }

            // set animations based on horizontal movement
            if (player.Controls.Contains(pb))
            {
                if (_goLeft)
                {
                    // play left-running gif when moving left
                    if (runningGifLeft != null && _currentAnimation != "run_left")
                    {
                        pb.Image = runningGifLeft;
                        _currentAnimation = "run_left";
                        _wasLeftMovement = true;
                    }
                    else if (runningGifLeft == null && _currentAnimation != "idle" && _wasLeftMovement)
                    {
                        pb.Image = idleImage;
                        _currentAnimation = "idle";
                    }
                }
                else if (_goRight)
                {
                    // play original gif when moving right
                    if (runningGif != null && _currentAnimation != "run_right")
                    {
                        pb.Image = runningGif;
                        _currentAnimation = "run_right";
                        _wasLeftMovement = false;
                    }
                    else if (runningGif == null && _currentAnimation != "idle" && _wasLeftMovement != true)
                    {
                        pb.Image = idleImage;
                        _currentAnimation = "idle";
                    }
                }
                else
                {
                    if (_currentAnimation != "idle")
                    {
                        pb.Image = idleImage;
                        _currentAnimation = "idle";
                    }
                }
            }

            // Vertikale Bewegung: wende vertikale Geschwindigkeit und Gravitation an
            player.Top += _verticalMovement;
            _verticalMovement += _gravity;

            // Kollision mit Boden: wenn unter oder auf Boden, setze auf Boden und Null Velocity
            if (player.Bottom >= floor.Top)
            {
                player.Top = floor.Top - player.Height;
                _verticalMovement = 0;
            }

            // Verhindere, dass Spieler aus dem Fenster nach oben verschwindet
            if (player.Top < 0)
            {
                player.Top = 0;
                _verticalMovement = 0;
            }

            //Auf den panels bleiben

            // Use previous vertical position to detect whether the player is landing on a block
            int prevTop = player.Top - _verticalMovement; // position before applying current movement
            int prevBottom = prevTop + player.Height;

            foreach (Panel block in flyingBlocks)
            {
                // check horizontal overlap first
                bool horizontallyOverlapping = player.Right > block.Left && player.Left < block.Right;
                if (!horizontallyOverlapping) continue;

                // if bounding boxes intersect, resolve depending on where the player came from
                if (player.Bounds.IntersectsWith(block.Bounds))
                {
                    // Landing on top of the block: previously below or at/above top, now overlapping its top
                    if (prevBottom <= block.Top && player.Bottom >= block.Top)
                    {
                        player.Top = block.Top - player.Height;
                        _verticalMovement = 0;
                    }
                    // Hitting the block from below: previously below block bottom and now overlapping
                    else if (prevTop >= block.Bottom && player.Top <= block.Bottom)
                    {
                        // place player just below the block and stop upward movement
                        player.Top = block.Bottom;
                        _verticalMovement = 0;
                    }
                    else
                    {
                        // fallback: if still intersecting, separate vertically based on minimal penetration
                        int overlapTop = player.Bottom - block.Top; // positive if overlapping from top
                        int overlapBottom = block.Bottom - player.Top; // positive if overlapping from bottom
                        if (overlapTop > 0 && (overlapTop <= overlapBottom))
                        {
                            player.Top = block.Top - player.Height;
                            _verticalMovement = 0;
                        }
                        else if (overlapBottom > 0)
                        {
                            player.Top = block.Bottom;
                            _verticalMovement = 0;
                        }
                    }
                }
            }

            // Verhindere, dass Spieler oben aus dem Bild fliegt
            if (player.Top < 0)
            {
                player.Top = 0;
                _verticalMovement = 0;
            }

            // Update jump ability based on contact with floor or blocks
            _canJump = IsOnGround();

            //im wasser versinken - NOCH GEPLANT

            //feuerball gegener schießt
           

        }

        // Hilfsmethode, damit der GameTimer übersichtlich bleibt
        private void UpdateAnimations()
        {
            if (!player.Controls.Contains(pb)) return;

            if (_goLeft)
            {
                if (runningGifLeft != null && _currentAnimation != "run_left")
                {
                    pb.Image = runningGifLeft;
                    _currentAnimation = "run_left";
                    _wasLeftMovement = true;
                }
            }
            else if (_goRight)
            {
                if (runningGif != null && _currentAnimation != "run_right")
                {
                    pb.Image = runningGif;
                    _currentAnimation = "run_right";
                    _wasLeftMovement = false;
                }
            }
            else
            {
                if (_currentAnimation != "idle")
                {
                    pb.Image = idleImage;
                    _currentAnimation = "idle";
                }
            }
        }

        // Helper to determine if the player is standing on floor or any flying block
        private bool IsOnGround()
        {
            if (player == null || floor == null) return false;

            // Check floor contact (allow small tolerance)
            if (Math.Abs(player.Bottom - floor.Top) <= 3) return true;

            // Check flying blocks contact (feet overlap with block top and horizontally overlapping)
            foreach (Panel block in flyingBlocks)
            {
                if (Math.Abs(player.Bottom - block.Top) <= 3 && player.Right > block.Left + 5 && player.Left < block.Right - 5)
                {
                    return true;
                }
            }

            return false;
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                _goLeft = true;
            }
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                _goRight = true;
            }

            // Nur springen, wenn Spieler Kontakt mit Boden oder einem fliegenden Block hat
            if ((e.KeyCode == Keys.Space || e.KeyCode == Keys.Up || e.KeyCode == Keys.W) && player != null)
            {
                if (IsOnGround())
                {
                    _verticalMovement = -_jumpForce;
                    _canJump = false; // will be re-enabled by GameTimer when contact is detected again
                }
            }
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                _goLeft = false;
            }
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                _goRight = false;
            }
        }

        private void Pb_MarioAuswahl_Click(object? sender, EventArgs e)
        {
           
            AufbauLevel1();

            // pb vorbereiten (Bild und Layout)
            if (pb_Mario?.Image != null)
            {
                pb.Image = pb_Mario.Image;
                pb.SizeMode = PictureBoxSizeMode.Zoom; // PictureBoxSizeMode.Zoom sorgt dafür, dass das Bild im PictureBox skaliert wird, ohne das Seitenverhältnis zu verzerren.
                pb.Dock = DockStyle.Fill; // DockStyle.Fill sorgt dafür, dass das Bild den gesamten Bereich des PictureBox ausfüllt, unabhängig von der Größe des PictureBox.
            }

            
            if (player != null)
            {
                player.Controls.Add(pb);
            }

            Mario.chosenCharacters(pb_Mario, "Mario");
            //charakter auf eine Panel machen mit pb bild
            Mario.Spawn();
        }

        private void Pb_Luigi_Click(object? sender, EventArgs e)
        {
            Luigi.chosenCharacters(pb_Luigi, "Luigi");
        }

        private void Pb_Toad_Click(object? sender, EventArgs e)
        {
            Toad.chosenCharacters(pb_Toad, "Toad");
        }

        private void Pb_Waluigi_Click(object? sender, EventArgs e)
        {
            Waluigi.chosenCharacters(pb_Waluigi, "Waluigi");
        }
        #endregion

    }
}

