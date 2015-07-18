
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LunaparkGame
{
    public class StonePath : Path
    {
        public StonePath(Model m, Coordinates c) : base(m,c) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }

        public override void Destruct()
        {
            throw new NotImplementedException();
        }
        
    }
    public class AsphaltPath : Path
    {
        public AsphaltPath(Model m, Coordinates c) : base(m,c) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Destruct()
        {
            throw new NotImplementedException();
        }
        

    }
    public class SandPath : Path
    {
        public SandPath(Model m, Coordinates c) : base(m,c) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Destruct()
        {
            throw new NotImplementedException();
        }
        
    }
    public class MarblePath : Path
    {
        public MarblePath(Model m, Coordinates c) : base(m, c) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Destruct()
        {
            throw new NotImplementedException();
        }
       

    }
    public class AmusementEnterPath : Path
    {

        public AmusementEnterPath(Model m, Coordinates c):base(m)//not call base(m,c) because dont want to add to maps
        { 
            price=0;
            //not to add to model.maps
            price = 0;
            control.Click += new EventHandler(Click);
            model.money -= this.price;
            this.coord = coord;
            signpostAmus = new Direction[m.maxAmusementsCount];
            //todo: mozna neni treba, overit
            for (int i = 0; i < signpostAmus.Length; i++) signpostAmus[i] = Direction.no;
           
        }
        public override void Destruct()
        {
            //not to remove from model.maps
        }
    }
    public class AmusementExitPath : Path {
        public AmusementExitPath(Model m, Coordinates c) : base(m) {
            price = 0;
            //not to add to model.maps
            price = 0;
            control.Click += new EventHandler(Click);
            model.money -= this.price;
            this.coord = coord;
            signpostAmus = new Direction[m.maxAmusementsCount];
            //todo: mozna neni treba, overit
            for (int i = 0; i < signpostAmus.Length; i++) signpostAmus[i] = Direction.no;
           
        }
        public override void Destruct()
        {//not to remove from model.maps
        }
    }
}
