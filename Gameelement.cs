using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario_Unbound
{
    public class Gameelement :Panel
    {
        protected int _movingspeed;

        public int _jumpheight;

        public void MovingNonHuman()
        {
            Left += _movingspeed;
            Top -= _jumpheight;
        }

        

    }
}
