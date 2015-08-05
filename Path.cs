
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LunaparkGame
{

   public abstract class AmusementPath : Path {
        public readonly Amusements amusement;
        public AmusementPath(Model m, Coordinates c, Amusements a, bool tangible = true)
            : base(m, prize: 0, tangible: tangible) //not call base(m,c) because dont want to add to maps
        {
            this.coord = c;
            model.maps.AddEntranceExit(this);
        }
    }
   

    public class AmusementEnterPath : AmusementPath {

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
    public class AmusementEnterPathFactory : PathFactory {
        Amusements a;
        public AmusementEnterPathFactory(Model m, Amusements a) :base(prize:0, m: m, name: "") {
            this.a = a;
        }
#warning nepouzivat ty dve metody (lepe overit, jestli type==AmusEnterPathFactory a pak se ptat atrakce a ta si vytvori sama)
        public override MapObjects Build(byte x, byte y) {
            return new AmusementEnterPath(model, new Coordinates(x, y), a);
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
    public class AmusementExitPathFactory : PathFactory {
        Amusements a;
        public AmusementExitPathFactory(Model m, Amusements a)
            : base(prize: 0, m: m, name: "") {
            this.a = a;
        }
#warning nepouzivat ty dve metody (lepe overit, jestli type==AmusEnterPathFactory a pak se ptat atrakce a ta si vytvori sama)
        public override MapObjects Build(byte x, byte y) {
            return new AmusementEnterPath(model, new Coordinates(x, y), a);
        }
    }
    
    
    public class StonePath : Path
    {
        
        public StonePath(Model m, Coordinates c, int prize, string name) : base(m, c, prize, name) { }
       
        
    }
    public class StonePathFactory : PathFactory {
        public StonePathFactory(Model m, int prize, string name) 
        : base(m, prize, name) {         
        }
        public override MapObjects Build(byte x, byte y) {
            return new StonePath(model, new Coordinates(x, y), prize, name);
        }
    }
    public class AsphaltPath : Path
    {
        public AsphaltPath(Model m, Coordinates c, int prize, string name) : base(m, c, prize, name) { }
        public AsphaltPath() { }
       
        

    }
    public class AsphaltPathFactory : PathFactory {
        public AsphaltPathFactory(Model m, int prize, string name)
            : base(m, prize, name) {
        }
        public override MapObjects Build(byte x, byte y) {
            return new AsphaltPath(model, new Coordinates(x, y), prize, name);
        }
    }
   
    public class SandPath : Path
    {
        public SandPath(Model m, Coordinates c, int prize, string name) : base(m, c, prize, name) { }
        
        
    }
    public class SandPathFactory : PathFactory {
        public SandPathFactory(Model m, int prize, string name)
            : base(m, prize, name) {
        }
        public override MapObjects Build(byte x, byte y) {
            return new SandPath(model, new Coordinates(x, y), prize, name);
        }
    }
   
    public class MarblePath : Path
    {
        public MarblePath(Model m, Coordinates c, int prize, string name) : base(m, c, prize, name) { }
       
    }
    public class MarblePathFactory : PathFactory {
        public MarblePathFactory(Model m, int prize, string name)
            : base(m, prize, name) {
        }
        public override MapObjects Build(byte x, byte y) {
            return new MarblePath(model, new Coordinates(x, y), prize, name);
        }
    }
   
}
