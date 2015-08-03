using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaparkGame
{
    public class Restaurant : SquareAmusements
    {

        public Restaurant(Model m):base() { 
         width = 1;
         model = m;
            fixedRunningTime = 0;
            
        }
        public Restaurant(Model m, Coordinates c) : base(m, c) {
            width = 1;
            model.mustBeEnter = false;
            model.mustBeExit = false;
            fixedRunningTime = 0;
            this.entrance = new AmusementEnterPath(m, c, this, tangible:false);
            this.exit = new AmusementExitPath(m,c,this, tangible:false);
        }
        public override bool CheckFreeLocation(byte x, byte y) {
            return CheckFreeLocation(x, y, width, width, hasSeparatedEntranceAndExit: false);
        }
        
    }


}
