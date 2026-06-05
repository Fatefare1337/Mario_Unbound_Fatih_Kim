namespace Mario_Unbound
{
    internal class Character : Gameelement
    {
        public string CharacterName { get; private set; }

        public PictureBox CharacterImage { get; private set; }

        public void ChooseCharacter(
            PictureBox selectedCharacter,
            string characterName)
        {
            CharacterImage = selectedCharacter;
            CharacterName = characterName;

            MoveSpeed = 10;
            JumpHeight = 20;
        }
    }
}