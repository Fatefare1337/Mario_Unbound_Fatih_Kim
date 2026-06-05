using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Mario_Unbound
{
    public abstract class MovingElements : Panel
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MoveSpeed { get; protected set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int JumpHeight { get; protected set; }
    }
}
