
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
            this.amusement = a;
            model.maps.AddEntranceExit(this);
        }
    }
   

    public class AmusementEnterPath : AmusementPath {

        public AmusementEnterPath(Model m, Coordinates c, Amusements a, bool tangible = true)
            : base(m, c, a, tangible) {
                model.mustBeEnter = false;
        }

        public override void Destruct() {
            model.maps.RemoveEntranceExit(this);
            model.dirtyDestruct.Enqueue(this);
          //  model.LastBuiltAmus = this.amusement;
         //   model.mustBeEnter = true;
        }
    }
    public class AmusementEnterPathFactory : PathFactory {
        Amusements a;
        public AmusementEnterPathFactory(Amusements a) :base(prize:0, name: "") {
            this.a = a;
        }
#warning nepouzivat ty dve metody (lepe overit, jestli type==AmusEnterPathFactory a pak se ptat atrakce a ta si vytvori sama)
        public override MapObjects Build(byte x, byte y, Model model) {
            return new AmusementEnterPath(model, new Coordinates(x, y), a);
        }
    }

    public class AmusementExitPath : AmusementPath {
        public AmusementExitPath(Model m, Coordinates c, Amusements a, bool tangible = true) : base(m, c, a, tangible) { }
        public override void Destruct() {
            model.maps.RemoveEntranceExit(this);
            model.dirtyDestruct.Enqueue(this);
            model.mustBeExit = false;
         //   model.LastBuiltAmus = this.amusement;
         //   if (!model.mustBeEnter) model.mustBeExit = true;

        }
    }
    public class AmusementExitPathFactory : PathFactory {
        Amusements a;
        public AmusementExitPathFactory(Amusements a)
            : base(prize: 0, name: "") {
            this.a = a;
        }
#warning nepouzivat ty dve metody (lepe overit, jestli type==AmusEnterPathFactory a pak se ptat atrakce a ta si vytvori sama)
        public override MapObjects Build(byte x, byte y, Model model) {
            return new AmusementEnterPath(model, new Coordinates(x, y), a);
        }
    }
    
    
    public class StonePath : Path
    {
        
        public StonePath(Model m, Coordinates c, int prize, string name) : base(m, c, prize, name) { }
       
        
    }
    public class StonePathFactory : PathFactory {
        public StonePathFactory(int prize, string name) 
        : base(prize, name) {         
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new StonePath(model, new Coordinates(x, y), prize, name);
        }
    }
    public class AsphaltPath : Path
    {
        public AsphaltPath(Model m, Coordinates c, int prize, string name) : base(m, c, prize, name) { }
        public AsphaltPath() { }
       
        

    }
    public class AsphaltPathFactory : PathFactory {
        public AsphaltPathFactory(int prize, string name)
            : base( prize, name) {
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new AsphaltPath(model, new Coordinates(x, y), prize, name);
        }
    }
   
    public class SandPath : Path
    {
        public SandPath(Model m, Coordinates c, int prize, string name) : base(m, c, prize, name) { }
        
        
    }
    public class SandPathFactory : PathFactory {
        public SandPathFactory(int prize, string name)
            : base(prize, name) {
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new SandPath(model, new Coordinates(x, y), prize, name);
        }
    }
   
    public class MarblePath : Path
    {
        public MarblePath(Model m, Coordinates c, int prize, string name) : base(m, c, prize, name) { }
       
    }
    public class MarblePathFactory : PathFactory {
        public MarblePathFactory(int prize, string name)
            : base(prize, name) {
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new MarblePath(model, new Coordinates(x, y), prize, name);
        }
    }
   
}
