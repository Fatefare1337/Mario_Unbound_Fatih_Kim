using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario_Unbound
{
    internal class Fireball : Gameelement
    {
        public Color FireColor { get; private set; }

        public Fireball(Color color)
        {
            MoveSpeed = 15;
            JumpHeight = 5;
            FireColor = color;
        }
    }
    
}
