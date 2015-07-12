
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LunaparkGame
{
    class StonePath : Path
    {
        public StonePath(Model m) : base(m) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }

        public override void Demolish()
        {
            throw new NotImplementedException();
        }
        
    }
    class AsphaltPath : Path
    {
        public AsphaltPath(Model m) : base(m) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
        

    }
    class SandPath : Path
    {
        public SandPath(Model m) : base(m) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
        
    }
    class MarblePath : Path
    {
        public MarblePath(Model m) : base(m) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
       

    }
    class AmusementEnter : Path {
        public AmusementEnter(Model m) : base(m) { }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
    }
    class AmusementExit : Path {
       public AmusementExit(Model m) : base(m) { }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
    }
}
