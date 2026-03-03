using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario_Unbound
{
    internal class Character : Gameelement
    {

        PictureBox chosenCharacter;

        public string _charactername { get; set; }

        

        public void chosenCharacters(PictureBox ausgewählterCharacter, string _charactername) 
        {
            this.chosenCharacter = ausgewählterCharacter;
            this._charactername = _charactername;

            _movingspeed = 10;
            _jumpheight = 20;

        }
    }
}
