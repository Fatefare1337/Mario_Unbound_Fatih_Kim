using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Devices;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Windows.Gaming.Input;
using Windows.Services.Maps.Guidance;
using static Mario_Unbound.Controller;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Timer = System.Windows.Forms.Timer;

namespace Mario_Unbound
{

    /*
    * Kim stunden: ca. 16 Stunden
    * Fatih stunden: ca. 17,5 Stunde
    *
    *neue probleme: WICHTIG:
    *Level2 Enemy2 kann nich angeschossen werden
    *wenn coin berührt muss zähler noch hochzählen
    *sollen wir eine endflag machen oder einfach wenn er das ende berührt?
    *
    *probleme:
    *
    * 
     */
    public partial class Form1 : Form
    {

        List <Panel> allCoins = new List<Panel>();
        List<Panel> allNpcs = new List<Panel>();
        private PS4Controller _ps4Controller;
        private Timer _controllerTimer;

        // Deadzone für den Stick 
        private const double Deadzone = 0.15;
        private const double StickMitte = 0.5;


        

        bool talking = false;
        bool _signedIn = false;
        ComboBox cmb_Profilpicture;
        PictureBox picture;
        PictureBox Logo;
        PictureBox Endflag;
        Button Btn_Start; Button Btn_Team; Button Btn_Profil; Button Btn_Closing;
        public string _profilUsername, _profilEmail, _profiPassword;
        TextBox txb_Username, txb_Email, txb_Password;
        PictureBox pb_Mario, pb_Luigi, pb_Toad, pb_Waluigi;
        //für Levelaufbau

        public List<Panel> flyingBlocks = new List<Panel>();
        
        public List<Panel> waterPanels = new List<Panel>();
        public List<Panel> enemyShots = new List<Panel>();
        //public List<Enemy> e = new List<Enemy>();




        public int coinsCollected = 0;
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
        
        private Timer gameTimer;
        private Timer enemyFireTimer;
        Enemy enemyE1 = new Enemy();
        Enemy enemyE2 = new Enemy();
        Enemy enemyE3 = new Enemy();
        Enemy enemyE4 = new Enemy();
        Enemy enemyE5 = new Enemy();

        private Dictionary<Panel, PointF> enemyShotVelocities = new Dictionary<Panel, PointF>();
        // player projectiles
        private List<Panel> playerShots = new List<Panel>();
        private Dictionary<Panel, PointF> playerShotVelocities = new Dictionary<Panel, PointF>();

        // enemy health and UI
        private int enemyE1Health = 5;
        private int enemyE2Health = 3;
        private int enemyE3Health = 3;
        private Label enemyHealthLabel;
        // player shot cooldown
        private DateTime _lastPlayerShot = DateTime.MinValue;
        private readonly TimeSpan _playerShotCooldown = TimeSpan.FromMilliseconds(100);
        private bool _goLeft = false;
        private bool _goRight = false;
        private int _verticalMovement = 0;
        private int _jumpForce = 18;
        private int _gravity = 1;
        private int _playerSpeed = 10;
        private bool _canJump = false;
        // -1 = blocked moving left, 1 = blocked moving right, 0 = not blocked
        private int _blockedDirection = 0;
        // width of gap at end of main floor (pixels)
        


        public Form1()
        {
            InitializeComponent();
            KeyPreview = true; //gemini : damit Form1 auch KeyEvents empfängt, wenn ein anderes Steuerelement den Fokus hat (z.B. TextBox)
            _ps4Controller = new PS4Controller();

            Text = "Mario Unbound - F to talk - E to shoot, Collected coins: " + coinsCollected;

            _controllerTimer = new Timer();
            _controllerTimer.Interval = 20;
            _controllerTimer.Tick += _ControllerTimer_Tick; 
            _controllerTimer.Start();
            


            DoubleBuffered = true; // Verhindert Flackern
            ClientSize = new Size(800, 500);
            //hier titel wo sagt e für schießen, j für reden, komm bis zum ende /zur flagge

            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;

            pb_Luigi = new PictureBox();
            pb_Toad = new PictureBox();
            pb_Mario = new PictureBox();
            pb_Waluigi = new PictureBox();
            Homepage();

            #region Charaktere

            pb_Mario.Image = Image.FromFile("MarioAuswahl.png");
            // setzt das Idle-Bild auf das Auswahlbild, damit es angezeigt wird,
            // wenn keine Animationen verfügbar sind oder abgespielt werden
            idleImage = pb_Mario.Image;


            try
            {
                runningGif = Image.FromFile("Mario_running_full_life.gif");
            }
            catch (Exception)
            {
                runningGif = null;
            }
            // Move player shots and check collisions with enemy
            if (playerShots.Count > 0)
            {
                foreach (var playerShot in playerShots.ToList())
                {
                    if (!playerShotVelocities.TryGetValue(playerShot, out PointF pvel))
                    {
                        Controls.Remove(playerShot);
                        playerShots.Remove(playerShot);
                        playerShotVelocities.Remove(playerShot);
                        continue;
                    }

                    playerShot.Left += (int)pvel.X;
                    playerShot.Top += (int)pvel.Y;

                    // entfernen, wenn außerhalb des Bildschirms
                    if (playerShot.Right < 0 || playerShot.Left > ClientSize.Width || playerShot.Bottom < 0 || playerShot.Top > ClientSize.Height)
                    {
                        Controls.Remove(playerShot);
                        playerShots.Remove(playerShot);
                        playerShotVelocities.Remove(playerShot);
                        continue;
                    }

                    // wenn der Spieler den "Boss" trifft.
                    if (enemyE1 != null && playerShot.Bounds.IntersectsWith(enemyE1.Bounds))
                    {
                        // Projektil entfernen
                        Controls.Remove(playerShot);
                        playerShots.Remove(playerShot);
                        playerShotVelocities.Remove(playerShot);

                        // Gegnerische Gesundheit reduzieren
                        enemyE1Health -= 1;
                        if (enemyHealthLabel != null)
                            enemyHealthLabel.Text = $"Enemy HP: {enemyE1Health}";

                        //if (enemyE1Health <= 0)
                        //{
                        //    // Der gegner ist besiegt worden
                        //    gameTimer?.Stop();
                        //    enemyFireTimer?.Stop();
                        Controls.Remove(enemyE1);
                        //    if (enemyHealthLabel != null)
                        //        Controls.Remove(enemyHealthLabel);
                        //    enemyE1 = null;
                        //    MessageBox.Show("Du hast gewonnen! Du hast den 'Boss' besigt! Wir hoffen, das Dir das spiel gefallen hat!", "SIEG!!!!!!", MessageBoxButtons.OK);
                        //    Homepage();
                        //}

                    }
                }
            }
            // Move player shots and check collisions with enemy
            if (playerShots.Count > 0)
            {
                foreach (var playerShot in playerShots.ToList())
                {
                    if (!playerShotVelocities.TryGetValue(playerShot, out PointF pvel))
                    {
                        Controls.Remove(playerShot);
                        playerShots.Remove(playerShot);
                        playerShotVelocities.Remove(playerShot);
                        continue;
                    }

                    playerShot.Left += (int)pvel.X;
                    playerShot.Top += (int)pvel.Y;

                    // entfernen, wenn außerhalb des Bildschirms
                    if (playerShot.Right < 0 || playerShot.Left > ClientSize.Width || playerShot.Bottom < 0 || playerShot.Top > ClientSize.Height)
                    {
                        Controls.Remove(playerShot);
                        playerShots.Remove(playerShot);
                        playerShotVelocities.Remove(playerShot);
                        continue;
                    }

                    // wenn der Spieler den "Boss" trifft.
                    if (enemyE1 != null && playerShot.Bounds.IntersectsWith(enemyE1.Bounds))
                    {
                        // Projektil entfernen
                        Controls.Remove(playerShot);
                        playerShots.Remove(playerShot);
                        playerShotVelocities.Remove(playerShot);

                        // Gegnerische Gesundheit reduzieren
                        enemyE1Health -= 1;
                        if (enemyHealthLabel != null)
                            enemyHealthLabel.Text = $"Enemy HP: {enemyE1Health}";

                        if (enemyE1Health <= 0)
                        {
                            //// Der gegner ist besiegt worden
                            //gameTimer?.Stop();
                            //enemyFireTimer?.Stop();
                            //Controls.Remove(enemyE1);
                            //if (enemyHealthLabel != null)
                            //    Controls.Remove(enemyHealthLabel);
                            //enemyE1 = null;
                            //MessageBox.Show("Du hast gewonnen! Du hast den 'Boss' besigt! Wir hoffen, das Dir das spiel gefallen hat!", "SIEG!!!!!!", MessageBoxButtons.OK);
                            //Homepage();
                        }
                    }
                }
            }
            
            

            try
            {
                runningGifLeft = Image.FromFile("Mario_running_full_life_left.gif");
            }
            catch (Exception)
            {
                // Wenn das Laden des linken GIFs fehlschlägt, versuchen wir,
                // es aus dem rechten GIF zu erstellen, indem wir es horizontal spiegeln
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


            if (_signedIn == false)
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

            Button Btn_Reset = new Button();
            Btn_Reset.BackColor = Color.White;
            Btn_Reset.ForeColor = Color.Black;
            Btn_Reset.Size = new Size(100, 30);
            Btn_Reset.Text = "Reset Fortschritt";
            Btn_Reset.Top = 400;
            Btn_Reset.Left = ClientSize.Width - Btn_Reset.Width;
            Controls.Add(Btn_Reset);
            Btn_Reset.Click += Btn_Reset_Click;

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

            if (_signedIn == false)
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

        private void Btn_Reset_Click(object? sender, EventArgs e)
        {
            _currentLevel = 1;
            coinsCollected = 0;
            
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

        protected void SpielSeite()
        {
            Controls.Clear();

            BackToPage();

            Label lbl_titel = new Label();
            lbl_titel.Text = "Mach dich auf eine spannend Reise gefasst!";

            Controls.Add(lbl_titel);
            lbl_titel.Size = new Size(200, 100);
            lbl_titel.Top = 30;
            lbl_titel.Left = (ClientSize.Width - lbl_titel.Width) / 2;
            lbl_titel.Font = new Font(lbl_titel.Font, FontStyle.Bold);

            Label lbl_Tutorial = new Label();
            lbl_Tutorial.Text = "Deine Reise beginnt hier! Bist du bereit, Leben zu retten?\n" +
                "Steuerung: WASD zum bewegen, E zum schießen, F zum reden\n" +
                "Für PS4 Kontroller: joystick zum bewegen, X zum springen, Kreis zum schießen, Dreieck zum reden\n" +
                "Klicke auf Mario, um deine Reise zu beginnen!";

            Controls.Add(lbl_Tutorial);
            lbl_Tutorial.Size = new Size(410,100);
            lbl_Tutorial.Top = 150;
            lbl_Tutorial.Left = (ClientSize.Width - lbl_Tutorial.Width) / 2;

            pb_Mario = new PictureBox();
            Controls.Add(pb_Mario);

            pb_Mario.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_Mario.Image = Image.FromFile("MarioAuswahl.png");
            pb_Mario.Size = new Size(180, 300);
            pb_Mario.Top = 100;
            pb_Mario.Left = 10;

            pb_Mario.Click += Pb_MarioAuswahl_Click;

           

           
        } 



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

            _signedIn = true;
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
            SpielSeite();



        } //In bearbeitung



        #endregion



        #endregion

        #region Mitgame

        private void CreateEnemy(Enemy enemy, int x, int y, int width, int height)
        {
            enemy.BackColor = Color.IndianRed;
            enemy.Size = new Size(width, height);
            enemy.Location = new Point(x, y);

            Controls.Add(enemy);
        }
        private Panel CreateNpc(int x, int y, string dialog)
        {
            Panel npc = new Panel();

            npc.BackColor = Color.GreenYellow;
            npc.Size = new Size(40, 60);
            npc.Tag = dialog; // Speichern des Dialogs im Tag-Property für späteren Zugriff
            npc.Location = new Point(x, y);

            Controls.Add(npc);

            return npc;
        }
        private void CreateWater(int x, int y, int width)
        {
            Panel water = new Panel();

            water.BackColor = Color.LightBlue;
            water.Size = new Size(width, 25);
            water.Location = new Point(x, y);

            Controls.Add(water);

            water.BringToFront();
            waterPanels.Add(water);
        }
        private Panel CreateFlyingBlock(int x, int y, int width)
        {
            Panel block = new Panel();

            block.BackColor = Color.RosyBrown;
            block.Size = new Size(width, 30);
            block.Location = new Point(x, y);

            Controls.Add(block);
            flyingBlocks.Add(block);

            return block;
        }
        private Panel CreateCoin(int x, int y)
        {
            Panel coin = new Panel();

            coin.BackColor = Color.Gold;
            coin.Size = new Size(40, 40);
            coin.Location = new Point(x, y);

            allCoins.Add(coin);

            Controls.Add(coin);

            return coin;
        }
        private Label CreateEnemyHealthLabel(Enemy enemy, int health)
        {
            Label label = new Label();

            label.AutoSize = true;
            label.ForeColor = Color.White;
            label.BackColor = Color.Transparent;
            label.Top = enemy.Top - 20;
            label.Left = enemy.Left;

            label.Font = new Font(
                label.Font.FontFamily,
                10,
                FontStyle.Bold);

            label.Text = $"Enemy HP: {health}";

            Controls.Add(label);

            return label;

        }
        private void CreateEndFlag()
        {

            Endflag = new PictureBox();
            Endflag.Image = Image.FromFile("Fahne.jpg");
            Endflag.SizeMode = PictureBoxSizeMode.Zoom;
            Endflag.Location = new Point(ClientSize.Width - Endflag.Width, floor.Top - Endflag.Height * 2);
            Endflag.Size = new Size(50, 100);
            Controls.Add(Endflag);
        }
        private void CreatePlayer()
        {
            player = new Panel();

            player.Size = new Size(40, 60);
            player.Location = new Point(
                50,
                floor.Top - player.Height);

            Controls.Add(player);

            pb.Dock = DockStyle.Fill;
            pb.SizeMode = PictureBoxSizeMode.Zoom;

            if (pb.Image == null)
                pb.Image = idleImage;

            player.Controls.Add(pb);
        }
        private void CreateFloor()
        {
            floor = new Panel();

            floor.BackColor = Color.Green;
            floor.Size = new Size(ClientSize.Width, 50);
            floor.Location = new Point(0, ClientSize.Height - floor.Height);

            Controls.Add(floor);
        }
        private void BossFireBall()
        {
            // enemy shooting timer: every 2.5 seconds spawn a red projectile aimed at player
            if (enemyFireTimer == null)
            {
                enemyFireTimer = new Timer();
                enemyFireTimer.Interval = 1000; // 2.5 seconds
                enemyFireTimer.Tick += EnemyFireTimer_Tick;
            }
            enemyFireTimer.Start();
        }
        private void TimerGame()
        {
            if (gameTimer == null)
            {
                gameTimer = new Timer();
                gameTimer.Interval = 20; // ~50 FPS
                gameTimer.Tick += GameTimer_Tick;
            }
            

        }
        private void PlayerTouchedEndFlag()
        {
            if (player.Location.X == Endflag.Location.X)
            {
                if (enemyE1Health <= 0)
                {
                    MessageBox.Show("Du hast das Level geschafft! Weiter zum nächsten Level!");
                    _currentLevel += 1;

                    if (_currentLevel == 2)
                        AufbauLevel2();
                    else if (_currentLevel == 3)
                        AufbauLevel3();
                    else if (_currentLevel > 3)
                        MessageBox.Show("Du hast alle Level geschafft! Herzlichen Glückwunsch!");
                }
                else
                {
                    MessageBox.Show("Du musst den Boss besiegen, um das Level zu beenden!");
                }
            }
        }

        private void _ControllerTimer_Tick(object? sender, EventArgs e)
        {


            var state = _ps4Controller.GetState();

            if (state.HasValue)
            {
                bool[] buttons = state.Value.Buttons;
                double[] axes = state.Value.Axes;

                // ----------------------------------------------------
                // 1. LAUFEN (Linker Stick - Horizontale Achse)
                // ----------------------------------------------------
                // Index 0 ist in der Regel die X-Achse des linken Sticks
                double linkerStickX = axes.Length > 0 ? axes[0] : StickMitte;

                // Berechnung, wie weit der Stick aus der Mitte bewegt wurde
                double abweichung = linkerStickX - StickMitte;

                if (abweichung > Deadzone)
                {
                    // Stick nach RECHTS gedrückt (Wert geht Richtung 1.0)
                    _goRight = true;
                }
                else if (abweichung < -Deadzone)
                {
                    // Stick nach LINKS gedrückt (Wert geht Richtung 0.0)
                    _goLeft = true;
                }
                else
                {
                    _goRight = false;
                    _goLeft = false;
                }


                // ----------------------------------------------------
                // 2. SPRINGEN (Kreuz / "X" Taste)
                // ----------------------------------------------------

                bool kreuzGedrueckt = buttons.Length > 1 ? buttons[1] : false;
                bool kreisGedrueckt = buttons.Length > 2 ? buttons[2] : false;
                bool dreieckGedrueckt = buttons.Length > 3 ? buttons[3] : false;

                if (kreuzGedrueckt)
                {
                    if (IsOnGround())
                    {
                        _verticalMovement = -_jumpForce;
                        _canJump = false;
                    }
                }
                if (kreisGedrueckt)
                {
                    SpawnPlayerFireball();
                }
                if (dreieckGedrueckt)
                {
                    foreach (Panel npc in allNpcs)
                    {
                        if (player.Bounds.IntersectsWith(npc.Bounds))
                        {
                            talking = true;
                            ShowDialog(npc);
                            break;
                        }
                    }
                   
                }
            }
        }

        private void ResetLevel()
        {
            
            Controls.Clear();

            ClientSize = new Size(1600, 500);


            flyingBlocks.Clear();
            waterPanels.Clear();
            enemyShots.Clear();
            playerShots.Clear();
            enemyShotVelocities.Clear();
            playerShotVelocities.Clear();
            enemyE1Health = 5;
            enemyE2Health = 3;
            enemyE3Health = 4;  
            
        }

        public void AufbauLevel1()
        {
            ResetLevel();
            CreateFloor();
            CreateEndFlag();
            CreateWater(600, 450, 300);
            CreateWater(1000, 450, 100);
            CreateFlyingBlock(180, 300, 150);
            CreateFlyingBlock(800, 300, 300);
            Panel block1 = CreateFlyingBlock(180, 300, 150);
            

            allNpcs.Add(
            CreateNpc(200, 300 - block1.Height, "Zum Glück bist du da! Das Monster frisst uns alle!!!!!"));
            allNpcs.Add(
            CreateNpc(400, 420, "Jetzt beruhig dich doch bitte. Er wird uns schon helfen. Spieler, schieß das Monster mit E ab"));
            allNpcs.Add(
            CreateNpc(450, 420, "Ich hab so Angst. Bitte hilf uns"));
            allNpcs.Add(
            CreateNpc(950, 420,"Ich wurde umzingelt!"));
            CreateEnemy(enemyE1, 1400, 300, 200, 200);
            CreateEnemyHealthLabel(enemyE1, enemyE1Health);
            BossFireBall();
            CreatePlayer();
            TimerGame();
            PlayerTouchedEndFlag();
            gameTimer.Start();

           

        }

        

        public void AufbauLevel2()
        {
            
            ResetLevel();
            CreateFloor();
            CreatePlayer();
            CreateEndFlag();
            //Wasser erstellen 
            CreateWater(400, 450, 100);
            CreateWater(600, 450, 300);
            //die fleigenden Blöcke erstelln
            CreateFlyingBlock(800, 300, 300); 
            //NPC panel
           CreateFlyingBlock(180, 300, 150);
            allNpcs.Add(CreateNpc(200,400,"Wow! Du hast uns schon viel geholfen. Diese Monster haben schon unsere Vorräte geklaut und meinen Sohn verletzt! Kümmer dich um sie!"));
           ;
            Panel block2 = CreateFlyingBlock(180, 300, 150);
            allNpcs.Add(
            CreateNpc(200, 300 - block2.Height, "Hast du keine Angst?"));  
            //gegner
            CreateEnemy(enemyE1, 1400, 300, 200, 200);
            CreateEnemyHealthLabel(enemyE1, enemyE1Health);
            CreateEnemy(enemyE2, 500, 350, 100, 100);
            CreateEnemyHealthLabel(enemyE2, enemyE2Health);
            
           
            CreateCoin(300, 400);
            
            BossFireBall();
            TimerGame();
            PlayerTouchedEndFlag();
            gameTimer.Start();

        }

        public void AufbauLevel3()
        {
            ResetLevel();
            CreateFloor();
            CreatePlayer();
            CreateEndFlag();
            CreateWater(400, 450, 100);

            //die fleigenden Blöcke erstelln
            CreateFlyingBlock(800, 300, 300);

            Panel block3 = CreateFlyingBlock(180, 300, 150);
            allNpcs.Add(
            CreateNpc(200, 300 - block3.Height,"Das ist die letzte Stufe! Du schaffst das. Danach sind wir frei!"));
            allNpcs.Add(
            CreateNpc(200, 400,"Ich seh mein Ende schon..."));
            allNpcs.Add(
            CreateNpc(500, 420,"Ich wusste wir können auf dich zählen"));

            

            //coins
            
            CreateCoin(300, 400);
            
            CreateCoin(700, 400);
            //Hier noch wenn coin berührt zähler + 1
            //gegner

            CreateEnemy(enemyE1, 1400, 300, 200, 200);
            CreateEnemyHealthLabel(enemyE1, enemyE1Health);

            CreateEnemy(enemyE2, 500, 350, 100, 100);
            CreateEnemyHealthLabel(enemyE2, enemyE2Health);

            CreateEnemy(enemyE3, 900, 350, 100, 100);
            CreateEnemyHealthLabel(enemyE3, enemyE3Health)  ;
            BossFireBall();
            TimerGame();
            PlayerTouchedEndFlag();



            gameTimer.Start();
        }



        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            if (player == null)
                return;

            //  Spieler erreicht Endflagge: 
            if (Endflag != null && player.Bounds.IntersectsWith(Endflag.Bounds))
            {
                gameTimer?.Stop();
                enemyFireTimer?.Stop();

                // win condition: enemy defeated or removed
                if (enemyE1 == null || enemyE1Health <= 0)
                {
                    _currentLevel += 1;

                    MessageBox.Show("Du hast das Level geschafft! Weiter zum nächsten Level!");

                    // reset movement state so player spawns clean in next level
                    _goLeft = false;
                    _goRight = false;
                    _verticalMovement = 0;
                    _blockedDirection = 0;

                    if (_currentLevel == 1) AufbauLevel1();
                    else if (_currentLevel == 2) AufbauLevel2();
                    else if (_currentLevel == 3) AufbauLevel3();
                    else
                    {
                        MessageBox.Show("Du hast alle Level geschafft! Herzlichen Glückwunsch!");
                        Homepage();
                    }
                }
                else
                {
                    MessageBox.Show("Du musst den Boss besiegen, um das Level zu beenden!");
                    // ggf. weiterlaufen lassen oder respawn etc.
                    gameTimer?.Start();
                    enemyFireTimer?.Start();
                }

                return;
            }

            // Horizontalbewegung
            bool wasMovingHorizontally = _goLeft || _goRight; // wenn einer der beiden true ist, dann bewegt sich der Spieler horizontal

            // setzt die horizontale Bewegung um, aber respektiert die blockierte Richtung:
            // wenn nach links blockiert (-1), ignoriere die linke Eingabe; wenn nach rechts blockiert (1), ignoriere die rechte Eingabe. So wird verhindert, dass der Spieler in die Richtung weiterläuft, in die er gerade kollidiert ist.
            int appliedDeltaX = 0;
            if (_goLeft && _blockedDirection != -1) appliedDeltaX -= _playerSpeed;
            if (_goRight && _blockedDirection != 1) appliedDeltaX += _playerSpeed;

            player.Left += appliedDeltaX;

            // inerhalb der Fenstergrenzen bleiben
            if (player.Left < 0)
            {
                player.Left = 0;
            }
            if (player.Right > ClientSize.Width)
            {
                player.Left = ClientSize.Width - player.Width;
            }

            // animation bassierend auf die horizontale Bewegung setzen.
            if (player.Controls.Contains(pb))
            {
                if (_goLeft)
                {

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

            // Kollision mit Boden: prüfe zuerst den unteren Hauptboden (`floor`) (der eine Lücke am rechten Ende haben kann),
            // ansonsten prüfe das mittlere `floorbetween`.
            if (floor != null && player.Bottom >= floor.Top)
            {
                // Wenn der Spieler über der Lücke am rechten Ende des Bodens steht, soll er fallen.
                int gapLeft = floor.Left + floor.Width;
                bool overGap = player.Right > gapLeft;

                if (!overGap)
                {
                    player.Top = floor.Top - player.Height;
                    _verticalMovement = 0;
                }
            }
            // floorbetween removed: no additional snap logic here

            // Verhindere, dass Spieler aus dem Fenster nach oben verschwindet
            if (player.Top < 0)
            {
                player.Top = 0;
                _verticalMovement = 0;
            }

            //Auf den panels bleiben


            // mit hilfe von copilot gemacht: Kollisionserkennung mit den fliegenden Blöcken, inklusive seitlicher Kollisionen, um das Durchdringen der Blöcke zu verhindern. Anhand der vorherigen vertikalen Position erkennen, ob der Spieler auf einem Block landet oder von unten gegen den Block stößt,
            // und entsprechend positionieren und vertikale Geschwindigkeit auf Null setzen, um das Durchdringen zu verhindern.

            // Anhand der vorherigen vertikalen Position erkennen, ob der Spieler auf einem Block landet
            int prevTop = player.Top - _verticalMovement; // Ausgangsposition vor der aktuellen Bewegung
            int prevBottom = prevTop + player.Height;
            // Vorherige horizontale Position berechnen, um seitliche Kollisionen zu erkennen
            int prevLeft = player.Left - appliedDeltaX;
            int prevRight = prevLeft + player.Width;




            foreach (Panel block in flyingBlocks)
            {
                //  überprüft zuerst die horizontale Überlappung, um unnötige vertikale Kollisionstests zu vermeiden
                bool horizontallyOverlapping = player.Right > block.Left && player.Left < block.Right;

                //  als erstes die horizontale Überlappung überprüft, um unnötige vertikale Kollisionstests zu vermeiden. Wenn keine horizontale Überlappung vorliegt, kann der Spieler nicht auf dem Block landen oder von unten gegen den Block stoßen, daher werden vertikale Kollisionstests übersprungen. Wenn eine horizontale Überlappung vorliegt, wird dann die vertikale Überlappung überprüft,
                //  festzustellen, ob der Spieler tatsächlich mit dem Block kollidiert.
                bool verticallyOverlapping = player.Top < block.Bottom && player.Bottom > block.Top;

                // bewegung rechts in den Block
                if (prevRight <= block.Left && player.Right >= block.Left && verticallyOverlapping)
                {
                    // player wird direkt neben dem Block positioniert, um das Durchdringen zu verhindern
                    player.Left = block.Left - player.Width;
                    appliedDeltaX = 0;
                    _blockedDirection = 1; // die rechte bewegung blockieren
                }
                // bewegung links in den Block
                if (prevLeft >= block.Right && player.Left <= block.Right && verticallyOverlapping)
                {
                    player.Left = block.Right;
                    appliedDeltaX = 0;
                    _blockedDirection = -1; // die linke bewegung blockieren
                }
                else if (player.Bounds.IntersectsWith(block.Bounds))
                {
                    // vertikale bewegung kollidiert mit Block: entweder von oben drauf landen oder von unten gegen den Block stoßen. Anhand der vorherigen vertikalen Position erkennen, ob der Spieler auf einem Block landet oder von unten gegen den Block stößt,
                    // und entsprechend positionieren und vertikale Geschwindigkeit auf Null setzen, um das Durchdringen zu verhindern.
                    if (prevBottom <= block.Top && player.Bottom >= block.Top)
                    {
                        player.Top = block.Top - player.Height;
                        _verticalMovement = 0;
                    }
                    else if (prevTop >= block.Bottom && player.Top <= block.Bottom)
                    {
                        player.Top = block.Bottom;
                        _verticalMovement = 0;
                    }
                    else
                    {
                        int overlapTop = player.Bottom - block.Top;
                        int overlapBottom = block.Bottom - player.Top;
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

            // mit hilfe von copilot gemacht:

            // nach der Kollisionserkennung mit den Blöcken, überprüfe, ob der Spieler immer noch einen Block berührt, um die Blockierung der Bewegung aufrechtzuerhalten. Wenn nicht mehr berührt, Blockierung aufheben.
            int touching = 0; // -1 linke berührung, 1 rechte berührung, 0 keine berührung
            foreach (Panel block in flyingBlocks)
            {
                bool vertOverlap = player.Top < block.Bottom && player.Bottom > block.Top;
                if (!vertOverlap) continue;

                if (Math.Abs(player.Right - block.Left) <= 2)
                {
                    touching = 1;
                    break;
                }
                if (Math.Abs(player.Left - block.Right) <= 2)
                {
                    touching = -1;
                    break;
                }
            }
            if (touching == 0)
            {
                _blockedDirection = 0;
            }

            // Verhindere, dass Spieler oben aus dem Bild fliegt
            if (player.Top < 0)
            {
                player.Top = 0;
                _verticalMovement = 0;
            }


            _canJump = IsOnGround();

            // Mit hilfe vom Copilot gemacht: Bewegung der gegnerischen Schüsse und Kollisionserkennung mit Spieler
            if (enemyShots.Count > 0)
            {
                // iterate over a copy to allow removal
                foreach (var shot in enemyShots.ToList())
                {
                    if (!enemyShotVelocities.TryGetValue(shot, out PointF vel))
                    {
                        // keine Infomationen zur Geschwindigkeit des Schusses, also entfernen
                        Controls.Remove(shot);
                        enemyShots.Remove(shot);
                        enemyShotVelocities.Remove(shot);
                        continue;
                    }

                    // move
                    shot.Left += (int)vel.X;
                    shot.Top += (int)vel.Y;

                    // löschen, wenn außerhalb des Bildschirms
                    if (shot.Right < 0 || shot.Left > ClientSize.Width || shot.Bottom < 0 || shot.Top > ClientSize.Height)
                    {
                        Controls.Remove(shot);
                        enemyShots.Remove(shot);
                        enemyShotVelocities.Remove(shot);
                        continue;
                    }

                    // wenn Spieler getroffen, dann Game Over (Fürs erste)
                    if (player != null && shot.Bounds.IntersectsWith(player.Bounds))
                    {

                        gameTimer?.Stop();
                        enemyFireTimer?.Stop();

                        MessageBox.Show("Game Over! DU HAST UNS ENTÄUSCHT.");
                        Close(); // Fenster schließen, sonst problem
                        // löschen aller gegnerischen Schüsse
                        foreach (var s in enemyShots.ToList())
                        {
                            Controls.Remove(s);
                            enemyShotVelocities.Remove(s);
                        }
                        enemyShots.Clear();

                        Homepage();
                        return;
                    }
                }

                for (int i = allCoins.Count - 1; i >= 0; i--)
                {
                    Panel coin = allCoins[i];

                    if (player != null && player.Bounds.IntersectsWith(coin.Bounds))
                    {
                        coinsCollected++;
                        Controls.Remove(coin);
                        Text = "Mario Unbound - F to talk -E to shoot, Collected coins: " + coinsCollected ;

                        allCoins.RemoveAt(i);

                       
                       
                    }
                }

                // Move player shots and check collisions with enemyE1
                for (int i = playerShots.Count - 1; i >= 0; i--)
                {
                    var playerShot = playerShots[i];
                    if (!playerShotVelocities.TryGetValue(playerShot, out PointF pvel))
                    {
                        Controls.Remove(playerShot);
                        playerShotVelocities.Remove(playerShot);
                        playerShots.RemoveAt(i);
                        continue;
                    }

                    playerShot.Left += (int)pvel.X;
                    playerShot.Top += (int)pvel.Y;

                    // remove if out of bounds
                    if (playerShot.Right < 0 || playerShot.Left > ClientSize.Width || playerShot.Bottom < 0 || playerShot.Top > ClientSize.Height)
                    {
                        Controls.Remove(playerShot);
                        playerShotVelocities.Remove(playerShot);
                        playerShots.RemoveAt(i);
                        continue;
                    }

                    // collision with enemyE1
                    if (enemyE1 != null && playerShot.Bounds.IntersectsWith(enemyE1.Bounds))
                    {
                        Controls.Remove(playerShot);
                        playerShotVelocities.Remove(playerShot);
                        playerShots.RemoveAt(i);

                        enemyE1Health -= 1;
                        if (enemyHealthLabel != null)
                            enemyHealthLabel.Text = $"Enemy HP: {enemyE1Health}";

                        if (enemyE1Health <= 0)
                        {

                            enemyFireTimer?.Stop();
                            Controls.Remove(enemyE1);
                            if (enemyHealthLabel != null) Controls.Remove(enemyHealthLabel);

                        }








                    }
                }




            }

        }


        private void EnemyFireTimer_Tick(object? sender, EventArgs e)
        {
            if (enemyE1 == null)
                return;

            // erstellt einen neuen feuerball, positioniert ihn in der Mitte des "Bosses".
            Panel enemyFireball = new Panel();
            enemyFireball.Size = new Size(20, 20);
            enemyFireball.BackColor = Color.Red;
            int spawnX = enemyE1.Left + enemyE1.Width / 2 - enemyFireball.Width / 2;
            int spawnY = enemyE1.Top + enemyE1.Height / 2 - enemyFireball.Height / 2;
            enemyFireball.Location = new Point(spawnX, spawnY);
            Controls.Add(enemyFireball);



            // VON COPLOT GEMACHT!!!!!!!!!


            // Berechne die Geschwindigkeit in Richtung des aktuellen Spielerzentrums
            float velocityX = 0, velocityY = 0;
            float speed = 8; // pixels pro tick // eigentlich kommt nach der 8 ein "f", damit es als float erkannt wird und somit optimierter wird. fürs erste egal
            if (player != null)
            {
                float targetX = player.Left + player.Width / 2f;
                float targetY = player.Top + player.Height / 2f;
                float directionX = targetX - (spawnX + enemyFireball.Width / 2f);
                float directionY = targetY - (spawnY + enemyFireball.Height / 2f);
                float distansToPlayer = (float)Math.Sqrt(directionX * directionX + directionY * directionY);

                // normalisiere die Richtung und multipliziere mit der gewünschten Geschwindigkeit, um die Geschwindigkeitskomponenten zu erhalten
                if (distansToPlayer > 0.001)
                {
                    velocityX = directionX / distansToPlayer * speed;
                    velocityY = directionY / distansToPlayer * speed;
                }
            }

            enemyShots.Add(enemyFireball);
            enemyShotVelocities[enemyFireball] = new PointF(velocityX, velocityY);
        }

        private void SpawnPlayerFireball()
        {
            if (player == null)
                return;

            // enforce cooldown
            if (DateTime.UtcNow - _lastPlayerShot < _playerShotCooldown)
                return;
            _lastPlayerShot = DateTime.UtcNow;

            Panel playerFireball = new Panel();
            playerFireball.Size = new Size(16, 16);
            playerFireball.BackColor = Color.OrangeRed;
            
            int spawnX = _wasLeftMovement ? player.Left - playerFireball.Width : player.Right;
            int spawnY = player.Top + player.Height / 2 - playerFireball.Height / 2;
            playerFireball.Location = new Point(spawnX, spawnY);
            Controls.Add(playerFireball);
            playerFireball.BringToFront();

            float speed = 12f;
            float vx = _wasLeftMovement ? -speed : speed;
            float vy = 0f;

            playerShots.Add(playerFireball);
            playerShotVelocities[playerFireball] = new PointF(vx, vy);
        }


        // Checkt, ob der Spieler auf einem Boden (Boden oder fliegender Block) steht, um Springen zu ermöglichen
        private bool IsOnGround()
        {
            if (player == null) return false;

            // prefer the main floor for ground checks; floorbetween is no longer required
            if (floor != null && Math.Abs(player.Bottom - floor.Top) <= 3)
                return true;

            // Check collision with flying blocks (platforms)
            foreach (Panel block in flyingBlocks)
            {
                if (Math.Abs(player.Bottom - block.Top) <= 3 && player.Right > block.Left + 5 && player.Left < block.Right - 5)
                    return true;
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

            // Springen: Space oder W (Up wird weiterhin unterstützt)
            if (player != null && (e.KeyCode == Keys.Space || e.KeyCode == Keys.W || e.KeyCode == Keys.Up))
            {
                if (IsOnGround())
                {
                    _verticalMovement = -_jumpForce;
                    _canJump = false;
                }
            }

            
            if (e.KeyCode == Keys.E)
            {
                SpawnPlayerFireball();
                
            }

            
            if (e.KeyCode == Keys.F)
            {
                    
                foreach (Panel npc in allNpcs)
                {
                        if (player.Bounds.IntersectsWith(npc.Bounds))
                        {
                            talking = true;
                            ShowDialog(npc);
                            break;
                        }
                }
                
            }

            if (e.KeyCode == Keys.Escape)
            {
                gameTimer?.Stop();
                enemyFireTimer?.Stop();
                var result = MessageBox.Show("Möchtest du wirklich zum Hauptmenü zurückkehren? ", "Bestätigung", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Homepage();
                }
                else
                {
                    gameTimer?.Start();
                    enemyFireTimer?.Start();
                }
            }
        }

        public void ShowDialog(Panel getroffenerNpc)
        {
            gameTimer.Stop(); // Spiel kurz pausieren

            // Sie holt sich den Text genau von diesem NPC
            string text = getroffenerNpc.Tag.ToString();
            MessageBox.Show(text, "NPC sagt:");

            gameTimer.Start(); // Spiel weiterlaufen lassen
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
            switch (_currentLevel)
            {
                case 1:
                    AufbauLevel1();
                    break;
                case 2:
                    AufbauLevel2();
                    break;
                case 3:
                    AufbauLevel3();
                    break;
               

            }

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

            Mario.ChooseCharacter(pb_Mario, "Mario");
            //charakter auf eine Panel machen mit pb bild
            
        }

        private void Pb_Luigi_Click(object? sender, EventArgs e)
        {
            Luigi.ChooseCharacter(pb_Luigi, "Luigi");
            
        }

        private void Pb_Toad_Click(object? sender, EventArgs e)
        {
            Toad.ChooseCharacter(pb_Toad, "Toad");
        }

        private void Pb_Waluigi_Click(object? sender, EventArgs e)
        {
            Waluigi.ChooseCharacter(pb_Waluigi, "Waluigi");
            // Wenn Waluigi ausgewählt wird, wird es schwere
            enemyE1Health = 20;

        }
        #endregion

    }
}

