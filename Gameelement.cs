using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario_Unbound
{
    public class Gameelement : MovingElements
    {
        

        public virtual void MovingNonHuman() 
        {
            Left += MoveSpeed;
            Top -= JumpHeight;
        }

        

    }
}
