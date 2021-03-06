﻿
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace LunaparkGame
{
    [Serializable]
    public abstract class AmusementPath : Path {
       public readonly Amusements amusement;
        public AmusementPath(GameRecords m, Coordinates c, Amusements a, int typeId, bool tangible = true)
            : base(m, prize: 0, typeId: typeId, tangible: tangible) //not call base(m,c) because dont want to add to maps
        {
            this.coord = c;
            this.amusement = a;
            model.maps.AddEntranceExit(this);
        }
    }

    [Serializable]
    public class AmusementEnterPath : AmusementPath {

        public AmusementEnterPath(GameRecords m, Coordinates c, Amusements a, bool tangible = true)
            : base(m, c, a, typeId: 1, tangible: tangible) {
                model.mustBeEnter = false;
                a.entrance = this;
        }

        public override void Destruct() {
            if (amusement.State != Amusements.Status.outOfService) {
                MessageBox.Show(Notices.cannotDemolishAmusement, Labels.warningMessBox, MessageBoxButtons.OK);
                return;
            }       
            model.maps.RemoveEntranceExit(this);
          //  model.dirtyDestruct.Enqueue(this);
            amusement.entrance = null; 
          
        }
    }
    [Serializable]
    public class AmusementEnterPathFactory : PathFactory {
        Amusements a;
        public AmusementEnterPathFactory(Amusements a) :base(prize:0, name: "") {
            this.a = a;
        }
    public override MapObjects Build(byte x, byte y, GameRecords model) {
            return new AmusementEnterPath(model, new Coordinates(x, y), a);
        }
    }
    [Serializable]
    public class AmusementExitPath : AmusementPath {
        public AmusementExitPath(GameRecords m, Coordinates c, Amusements a, bool tangible = true) : base(m, c, a, typeId: 2, tangible: tangible) {
            a.exit = this;
        }
        public override void Destruct() {
            if (amusement.State != Amusements.Status.outOfService) {
                MessageBox.Show(Notices.cannotDemolishAmusement, Labels.warningMessBox, MessageBoxButtons.OK);
                return; 
            }
            model.maps.RemoveEntranceExit(this);
            //model.dirtyDestruct.Enqueue(this);
            model.mustBeExit = false;
            amusement.exit = null;
        }
    }
    [Serializable]
    public class AmusementExitPathFactory : PathFactory {
        Amusements a;
        public AmusementExitPathFactory(Amusements a)
            : base(prize: 0, name: "") {
                this.a = a;
        }
        public override MapObjects Build(byte x, byte y, GameRecords model) {
            return new AmusementExitPath(model, new Coordinates(x, y), a);
        }
    }

    [Serializable]
    public class StonePath : Path
    {
        
        public StonePath(GameRecords m, Coordinates c, int prize, string name, int typeId) : base(m, c, prize, name, typeId) { }
       
        
    }
    
    [Serializable]
    public class StonePathFactory : PathFactory {
        public StonePathFactory(int prize, string name) 
        : base(prize, name) {         
        }
        public override MapObjects Build(byte x, byte y, GameRecords model) {
            return new StonePath(model, new Coordinates(x, y), prize, name, internTypeId);
        }
    }
   
    [Serializable]
    public class AsphaltPath : Path
    {
        public AsphaltPath(GameRecords m, Coordinates c, int prize, string name, int typeId) : base(m, c, prize, name, typeId) { }
        public AsphaltPath() { }
       
        

    }
    [Serializable]
    public class AsphaltPathFactory : PathFactory {
        public AsphaltPathFactory(int prize, string name)
            : base( prize, name) {
        }
        public override MapObjects Build(byte x, byte y, GameRecords model) {
            return new AsphaltPath(model, new Coordinates(x, y), prize, name, internTypeId);
        }
    }

    [Serializable]
    public class SandPath : Path
    {
        public SandPath(GameRecords m, Coordinates c, int prize, string name, int typeId) : base(m, c, prize, name, typeId) { }
        
        
    }
    [Serializable]
    public class SandPathFactory : PathFactory {
        public SandPathFactory(int prize, string name)
            : base(prize, name) {
        }
        public override MapObjects Build(byte x, byte y, GameRecords model) {
            return new SandPath(model, new Coordinates(x, y), prize, name, internTypeId);
        }
    }
    [Serializable]
    public class MarblePath : Path
    {
        public MarblePath(GameRecords m, Coordinates c, int prize, string name, int typeId) : base(m, c, prize, name, typeId) { }
       
    }
    [Serializable]
    public class MarblePathFactory : PathFactory {
        public MarblePathFactory(int prize, string name)
            : base(prize, name) {
        }
        public override MapObjects Build(byte x, byte y, GameRecords model) {
            return new MarblePath(model, new Coordinates(x, y), prize, name, internTypeId);
        }
    }
   
}
