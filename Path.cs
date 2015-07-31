
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LunaparkGame
{
    public class StonePath : Path
    {
        public StonePath(Model m, Coordinates c) : base(m,c) { }
       

        public override void Destruct()
        {
            throw new NotImplementedException();
        }
        
    }
    public class AsphaltPath : Path
    {
        public AsphaltPath(Model m, Coordinates c) : base(m,c) { }
        public AsphaltPath() { }
        public override void Destruct()
        {
            throw new NotImplementedException();
        }
        

    }
    public class SandPath : Path
    {
        public SandPath(Model m, Coordinates c) : base(m,c) { }
        
        public override void Destruct()
        {
            throw new NotImplementedException();
        }
        
    }
    public class MarblePath : Path
    {
        public MarblePath(Model m, Coordinates c) : base(m, c) { }
        
        public override void Destruct()
        {
            throw new NotImplementedException();
        }
       

    }
   

}
