using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario_Unbound
{
    internal class Fireball : Gameelement
    {
        //PictureBox fireball;
        Panel fireball;
        Color fire;
            public void BuildingFireball(Color Fire)
            {
                _movingspeed = 15;
                _jumpheight = 5;
                Fire = fire;
        }
    }
}
