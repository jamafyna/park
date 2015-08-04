using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaparkGame
{

    public class Gate : Amusements {

        public int VstupneDoParku { get; set; }
        private new Path exit;
        public const int width = 1;
        public const int height = 3;

        public Gate(Model m, Coordinates c) {
            this.model = m;
            this.coord = c;
            this.entrance = new AmusementEnterPath(m, new Coordinates(c.x, (byte)(c.y + height / 2)), this, tangible: false);
            this.exit = new MarblePath(m, new Coordinates((byte)(c.x + width), entrance.coord.y));
            m.maps.AddAmus(this);
        }

        public Gate(Model m, Coordinates c, Coordinates entrance, Coordinates exit) {
            this.model = m;
            this.coord = c;
            this.entrance = new AmusementEnterPath(m, entrance, this, tangible: false);
            this.exit = new MarblePath(m, exit);
            m.maps.AddAmus(this);
       }
        public override void Action() {
            Person p;
            // deleting people from the park
            while (queue.TryDequeue(out p))
                p.Destruct();
            // a new person can be created
            int a = exit.coord.x * MainForm.sizeOfSquare;
            int b = exit.coord.y * MainForm.sizeOfSquare;

            //todo: if (model.maps.IsPath(a,b) && ShouldCreateNewPerson()) {
            if (ShouldCreateNewPerson()) {
                p = new Person(model, a + 1, b + MainForm.sizeOfSquare / 2);

            }
        }
        public bool ShouldCreateNewPerson() {
            return true;
#warning pozdeji dodelat pstni fci vyroby, pouzit exp.rozd. - pouzit castecne rozdelanou tridu v Program.cs

        }
        public void Click(object sender, EventArgs e) {

        }
        public override void Destruct() {
            // nothing, the gate cannot be demolished
        }
        public override List<Coordinates> GetAllPoints() {
            List<Coordinates> l = new List<Coordinates>();
            l.Add(entrance.coord);
            l.Add(exit.coord);
            return l;
        }
        // the two methods below are irrelevant

        public override bool CheckFreeLocation(byte x, byte y) { return false; }
        protected override bool IsInsideInAmusement(int x, int y) { return false; }

    }

    //todo: 4 tridy nize nejsou abstract + pridat veskere parametry
    public abstract class SquareAmusements : Amusements {
        public byte width { get; protected set; }
        public SquareAmusements() { }
        public SquareAmusements(Model m, Coordinates c)
            : base(m, c) {
            model.CheckCheapestFee(this.currFee);
        }

        public override bool CheckFreeLocation(byte x, byte y) {
            return CheckFreeLocation(x, y, width, width, hasSeparatedEntranceAndExit: true);
        }
        protected override bool IsInsideInAmusement(int x, int y) {
            if (x >= this.coord.x && x < this.coord.x + this.width &&
                y >= this.coord.y && y < this.coord.y + this.width)
                return true;
            else return false;
        }
        public override List<Coordinates> GetAllPoints() {
            List<Coordinates> list = new List<Coordinates>(width * width);
            for (byte i = coord.x; i < coord.x + width; i++) {
                for (byte j = coord.y; j < coord.y + width; j++) list.Add(new Coordinates(i, j));
            }
            return list;
        }
    }
    public class SquareAmusementsFactory : RectangleAmusementsFactory {
        byte width;
       
     //   public SquareAmusementsFactory(): base(prize,model) { }
        
        
        public override bool CanBeBuild(byte x, byte y) {
            if (CheckFreeLocation(x, y, width, width, hasSeparatedEnterExit)) return true;
            else return false;
        }
        public override MapObjects Build() {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Class for rectangle, not square, amusements. It can have a different orientation.
    /// </summary>
    public abstract class RectangleAmusements : Amusements {
        public readonly byte sizeA;
        public readonly byte sizeB;
        public readonly bool isHorizontalOriented;


        public RectangleAmusements(Model m, Coordinates c, bool isHorizontal = true)
            : base(m, c) {
            isHorizontalOriented = isHorizontal;
            model.CheckCheapestFee(this.currFee);
        }

        public RectangleAmusements(Model m, Coordinates c, byte sizeA, byte sizeB, bool isHorizontal = true)
            : this(m, c, isHorizontal) {
            this.sizeA = sizeA;
            this.sizeB = sizeB;
        }


        public override bool CheckFreeLocation(byte x, byte y) {
            if (isHorizontalOriented) return CheckFreeLocation(x, y, sizeA, sizeB, hasSeparatedEntranceAndExit: true);
            else return CheckFreeLocation(x, y, sizeB, sizeA, hasSeparatedEntranceAndExit: true);
        }
        protected override bool IsInsideInAmusement(int x, int y) {
            if (isHorizontalOriented) {
                if (x >= this.coord.x && x < this.coord.x + this.sizeA &&
                    y >= this.coord.y && y < this.coord.y + this.sizeB)
                    return true;
                else return false;
            }
            else {
                if (x >= this.coord.x && x < this.coord.x + this.sizeB &&
                        y >= this.coord.y && y < this.coord.y + this.sizeA)
                    return true;
                else return false;
            }
        }
        public override List<Coordinates> GetAllPoints() {
            List<Coordinates> list = new List<Coordinates>(sizeA * sizeB);
            if (isHorizontalOriented) {
                for (byte i = coord.x; i < coord.x + sizeA; i++) {
                    for (byte j = coord.y; j < coord.y + sizeB; j++) list.Add(new Coordinates(i, j));
                }
            }
            else {
                for (byte i = coord.x; i < coord.x + sizeB; i++) {
                    for (byte j = coord.y; j < coord.y + sizeA; j++) list.Add(new Coordinates(i, j));
                }
            }
            return list;
        }


    }
    public class RectangleAmusementsFactory : AmusementsFactory {
        byte width, height;
        protected bool CheckFreeLocation(byte x, byte y, byte width, byte height, bool hasSeparatedEntranceAndExit = true) {
            if (x + width > model.playingWidth + 1 || y + height > model.playingHeight + 1) return false;
            for (byte i = x; i < x + width; i++) {
                for (byte j = y; j < y + height; j++) {
                    if (!model.maps.isFree(i, j)) return false;
                }
            }
#warning overit, ze to opravdu overuje spravne
            if (hasSeparatedEntranceAndExit) {
                bool free = false;
                if (x - 1 > 0) for (byte i = y; i < y + height; i++)
                        if (model.maps.isFree((byte)(x - 1), i)) { if (free) return free; else free = true; }
                if (x < model.playingWidth) for (byte i = y; i < y + height; i++)
                        if (model.maps.isFree((byte)(x + 1), i)) { if (free) return free; else free = true; }
                if (y - 1 > 0) for (byte i = x; i < x + width; i++)
                        if (model.maps.isFree(i, (byte)(y - 1))) { if (free) return free; else free = true; }
                if (y < model.playingWidth) for (byte i = y; i < y + height; i++)
                        if (model.maps.isFree(i, (byte)(y + 1))) { if (free) return free; else free = true; }
                return free;
            }
            return true;

        }
        public override bool CanBeBuild(byte x, byte y) {
            if (CheckFreeLocation(x, y, width, height, hasSeparatedEnterExit)) return true;
            else return false;
        }
        public override MapObjects Build() {
            throw new NotImplementedException();
        }
    }

    public class FreeShapedAmusements : Amusements {
        public FreeShapedAmusements(Model m, Coordinates c)
            : base(m, c) {
            model.CheckCheapestFee(this.currFee);
        }
        //nejspis v sobe jeste jednu vnorenou tridu reprezentujici kousky atrakce
        public override List<Coordinates> GetAllPoints() {
            throw new NotImplementedException();
        }
        protected override bool IsInsideInAmusement(int x, int y) {
            throw new NotImplementedException();
        }
        
    }
    public class FreeShapedAmusementsFactory : AmusementsFactory {
        public override MapObjects Build() {
            throw new NotImplementedException();
        }
        public override bool CanBeBuild(byte x, byte y) {
            throw new NotImplementedException();
        }
    
    }
    
    /// <summary>
    /// napr. pro lavicky
    /// </summary>
    public abstract class LittleComplementaryAmusements : Amusements {
        public LittleComplementaryAmusements(Model m, Coordinates c) : base(m, c) { }
    }
    
    
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
        protected override void DropPeopleOff() {
            foreach (Person p in peopleInList) {
                p.status = Person.Status.choosesAmus;
                p.visible = true;
                p.Feed();
            }
            peopleInList.Clear();               
        }
    }


}
