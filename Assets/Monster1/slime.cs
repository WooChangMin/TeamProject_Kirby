using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Monster1
{
    internal class slime
    {
        private string name;
        private int hp;
        private int mp;

        public slime()
        {
            hp = 20;
            mp = 10;
        }

        private void Hit()
        {
            Console.WriteLine( "몬스터 작업");
        }

    }
}
