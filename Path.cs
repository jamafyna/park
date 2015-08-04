
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LunaparkGame
{

   public abstract class AmusementPath : Path {
        public readonly Amusements amusement;
        public AmusementPath(Model m, Coordinates c, Amusements a, bool tangible = true)
            : base(m, tangible) //not call base(m,c) because dont want to add to maps
        {
            this.coord = c;
            model.maps.AddEntranceExit(this);
        }

    }
    public class AmusementEnterPath : AmusementPath {

        public override int price {
            get {
                return 0;
            }
            protected set {
                base.price = 0;
            }
        }
        public AmusementEnterPath(Model m, Coordinates c, Amusements a, bool tangible = true)
            : base(m, c, a, tangible) {
        }

        public override void Destruct() {
            model.maps.RemoveEntranceExit(this);
            model.dirtyDestruct.Enqueue(this);
            model.LastBuiltAmus = this.amusement;
            model.mustBeEnter = true;
        }

    }
    public class AmusementExitPath : AmusementPath {
        public AmusementExitPath(Model m, Coordinates c, Amusements a, bool tangible = true) : base(m, c, a, tangible) { }
        public override void Destruct() {
            model.maps.RemoveEntranceExit(this);
            model.dirtyDestruct.Enqueue(this);
            model.LastBuiltAmus = this.amusement;
            if (!model.mustBeEnter) model.mustBeExit = true;

        }

    }
    
    
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
