namespace Mario_Unbound
{
    internal class Character : Gameelement
    {

        PictureBox chosenCharacter;

        //public string _charactername { get; set; }

        
        public void chosenCharacters(PictureBox ausgewählterCharacter, string _charactername) 
        {
            chosenCharacter = ausgewählterCharacter;
            _charactername = _charactername;

            _movingspeed = 10;
            _jumpheight = 20;

        }

        public void Spawn()
        {
            Panel panel = new Panel();

            
        }
    }
}
