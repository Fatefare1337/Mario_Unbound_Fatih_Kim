using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mario_Unbound
{
    internal class Enemy : Gameelement
    {
        int _enemytype;
         
        public void BuildingEnemies(int enemytype)
        {
            _enemytype = enemytype;

            _movingspeed = 5;
            _jumpheight = 0;
        }
    }
}
