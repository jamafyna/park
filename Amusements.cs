using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaparkGame
{
    public abstract class Restaurant : SquareAmusements
    {//todo: nema byt abstract, ale virtual, akorat ted nechci vyplnovat
        public Restaurant(Model m, Coordinates c) : base(m, c) { }
    }


}
