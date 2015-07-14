
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LunaparkGame
{
    public class StonePath : Path
    {
        public StonePath(Model m, int x, int y) : base(m,x,y) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }

        public override void Demolish()
        {
            throw new NotImplementedException();
        }
        
    }
    public class AsphaltPath : Path
    {
        public AsphaltPath(Model m, int x, int y) : base(m, x, y) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
        

    }
    public class SandPath : Path
    {
        public SandPath(Model m, int x, int y) : base(m, x, y) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
        
    }
    public class MarblePath : Path
    {
        public MarblePath(Model m, int x, int y) : base(m, x, y) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
       

    }
    public class AmusementEnterPath : Path
    {
        
        public AmusementEnterPath(Model m, int x, int y) : base(m, x, y) { 
            price=0;
        }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
    }
    public class AmusementExitPath : Path {
        public AmusementExitPath(Model m, int x, int y) : base(m, x, y) {
            price = 0;
        }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
    }
}
