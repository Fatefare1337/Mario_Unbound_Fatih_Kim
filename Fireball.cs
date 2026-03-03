using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario_Unbound
{
    internal class Fireball : Gameelement
    {
        PictureBox fireball;

            public void BuildingFireball(PictureBox fireball)
            {
                _movingspeed = 15;
                _jumpheight = 5;
        }
    }
}
