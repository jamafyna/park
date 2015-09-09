
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

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
            if (amusement.State != Amusements.Status.outOfService) {
                MessageBox.Show(Notices.cannotDemolishAmusement, Labels.warningMessBox, MessageBoxButtons.OK);
                return;
            }       
            model.maps.RemoveEntranceExit(this);
            model.dirtyDestruct.Enqueue(this);
            amusement.entrance = null; 
          
        }
    }
    public class AmusementEnterPathFactory : PathFactory {
        Amusements a;
        public AmusementEnterPathFactory(Amusements a, Image image) :base(prize:0, name: "", image: image) {
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
            if (amusement.State != Amusements.Status.outOfService) { 
                MessageBox.Show(Labels.warningMessBox, Notices.cannotDemolishAmusement, MessageBoxButtons.OK);
                return; 
            }
            model.maps.RemoveEntranceExit(this);
            model.dirtyDestruct.Enqueue(this);
            model.mustBeExit = false;
            amusement.exit = null;
        }
    }
    public class AmusementExitPathFactory : PathFactory {
        Amusements a;
        public AmusementExitPathFactory(Amusements a, Image im)
            : base(prize: 0, name: "", image: im) {
            this.a = a;
        }
#warning nepouzivat ty dve metody (lepe overit, jestli type==AmusEnterPathFactory a pak se ptat atrakce a ta si vytvori sama)
        public override MapObjects Build(byte x, byte y, Model model) {
            return new AmusementEnterPath(model, new Coordinates(x, y), a);
        }
    }
    
    
    public class StonePath : Path
    {
        
        public StonePath(Model m, Coordinates c, int prize, string name, int typeId) : base(m, c, prize, name, typeId) { }
       
        
    }
    public class StonePathFactory : PathFactory {
        public StonePathFactory(int prize, string name, Image image) 
        : base(prize, name, image) {         
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new StonePath(model, new Coordinates(x, y), prize, name, internTypeId);
        }
    }
    public class AsphaltPath : Path
    {
        public AsphaltPath(Model m, Coordinates c, int prize, string name, int typeId) : base(m, c, prize, name, typeId) { }
        public AsphaltPath() { }
       
        

    }
    public class AsphaltPathFactory : PathFactory {
        public AsphaltPathFactory(int prize, string name, Image image)
            : base( prize, name, image) {
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new AsphaltPath(model, new Coordinates(x, y), prize, name, internTypeId);
        }
    }
   
    public class SandPath : Path
    {
        public SandPath(Model m, Coordinates c, int prize, string name, int typeId) : base(m, c, prize, name, typeId) { }
        
        
    }
    public class SandPathFactory : PathFactory {
        public SandPathFactory(int prize, string name, Image image)
            : base(prize, name, image) {
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new SandPath(model, new Coordinates(x, y), prize, name, internTypeId);
        }
    }
   
    public class MarblePath : Path
    {
        public MarblePath(Model m, Coordinates c, int prize, string name, int typeId) : base(m, c, prize, name, typeId) { }
       
    }
    public class MarblePathFactory : PathFactory {
        public MarblePathFactory(int prize, string name, Image image)
            : base(prize, name, image) {
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new MarblePath(model, new Coordinates(x, y), prize, name, internTypeId);
        }
    }
   
}
