using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario_Unbound
{
    internal class Charakter
    {

        PictureBox ausgewählterCharacter;

        public string _Charaktername { get; set; }

        public void augewählterCharakter(PictureBox ausgewählterCharacter, string _charactername)
        {
            this.ausgewählterCharacter = ausgewählterCharacter;
            _Charaktername = _charactername;
        }
    }
}
