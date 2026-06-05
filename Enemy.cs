using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Mario_Unbound
{
    internal class Enemy : Gameelement
    {

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Health { get; set; }

        public Enemy()
        {
            MoveSpeed = 5;
            JumpHeight = 0;
            Health = 5;
        }
    }

}
