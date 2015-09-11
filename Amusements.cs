using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LunaparkGame
{

    public class Gate : Amusements {

        public const int width = 1;
        public const int height = 3;
        public new Status State {
            get { return status; }
            set { if (value != Status.waitingForPeople) status = value; }
        }
        public Gate(Model m, Coordinates c, Image entrIm, Image exitIm) {
            this.model = m;
            this.coord = c;
            this.entrance = new AmusementEnterPath(m, new Coordinates(c.x, (byte)(c.y + height / 2)), this, entrIm, tangible: false);
            this.exit = new AmusementExitPath(m, new Coordinates((byte)(c.x + width), entrance.coord.y), this, exitIm, tangible: false);
            this.image = Properties.Images.gate;
            m.maps.AddAmus(this);
        }

        public Gate(Model m, Coordinates c, Coordinates entrance, Coordinates exit, Image entrIm, Image exitIm) {
            this.model = m;
            this.coord = c;
            this.entrance = new AmusementEnterPath(m, entrance, this, entrIm, tangible: false);
            this.exit = new AmusementExitPath(m, exit, this, exitIm, tangible: false);
            this.image = Properties.Images.gate;
            m.maps.AddAmus(this);
       }
        public override void Action() {
            switch (status) {

                case Status.running: {
                        #region
                        Person p;
                        // deleting people from the park
                        while (queue.TryDequeue(out p))
                            p.Destruct();
                        // a new person can be created
                        int a = exit.coord.x * MainForm.sizeOfSquare;
                        int b = exit.coord.y * MainForm.sizeOfSquare;

                        //todo: if (model.maps.IsPath(a,b) && ShouldCreateNewPerson()) 
                        if (ShouldCreateNewPerson()) {
                            p = new Person(model, a + 1, b + MainForm.sizeOfSquare / 2);
                            model.MoneyAdd(this.CurrFee);
                        }
                        #endregion
                    }
                    break;
                case Status.outOfService: //nothing
                    break;
                case Status.runningOut: {
                        // deleting people from the park
                    Person p;
                    while (queue.TryDequeue(out p)) p.Destruct();                         
                    if (model.CurrPeopleCount == 0) 
                        status = Status.outOfService;
                }
                    break;
                case Status.disposing: //nothing
                    break;
                default: status = Status.running;
                    break;
            }
           
        }
        public bool ShouldCreateNewPerson() {
            return true;
#warning pozdeji dodelat pstni fci vyroby, pouzit exp.rozd. - pouzit castecne rozdelanou tridu v Program.cs

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
        public override Size GetRealSize() {
            return new Size(Gate.width * MainForm.sizeOfSquare - 1, Gate.height * MainForm.sizeOfSquare - 1);            
        }
        /// <summary>
        /// Determines whether the given coordinates are inside this object or not.
        /// </summary>
        /// <param name="x">An nonnegative integer, represents the game x-coordinate.</param>
        /// <param name="y">An nonnegative integer, represents the game y-coordinate.</param>
        /// <returns></returns>
        public override bool IsInside(int gx, int gy) {
            if (gx >= this.coord.x && gx <= this.coord.x + width && gy >= this.coord.y && gy <= this.coord.y + height) return true;
            else return false;
        }
        
        public string GetInfo() {
            return string.Concat(
                Labels.currVisitorsCount, model.CurrPeopleCount, "\n",
                Labels.totalVisitorsCount, model.TotalPeopleCount
                );       
        }
        
        // the method below is irrelevant
        protected override bool IsInsideInAmusement(int x, int y) { return false; }
  
    }

   /// <summary>
    /// Class for rectangle, not square, amusements. It can have a different orientation.
    /// </summary>
    public class RectangleAmusements : Amusements {
        public readonly byte sizeA;
        public readonly byte sizeB;
        public readonly bool isHorizontalOriented;


        /*public RectangleAmusements(Model m, Coordinates c, bool isHorizontal = true)
            : base(m, c) {
            isHorizontalOriented = isHorizontal;
            model.CheckCheapestFee(this.currFee);
        }

        public RectangleAmusements(Model m, Coordinates c, byte sizeA, byte sizeB, bool isHorizontal = true)
            : this(m, c, isHorizontal) {
            this.sizeA = sizeA;
            this.sizeB = sizeB;
        }*/

        public RectangleAmusements(Coordinates c, Model m, int prize, int fee, int capacity, int runningTime, string name, bool hasEntranceExit, byte width, byte height, bool isHorizontal, Color color, int typeId, Image im, Image entrImage, Image exitImage)
        : base (c, m, prize, fee, capacity, runningTime, name, hasEntranceExit, color, typeId, im, entrImage, exitImage){
            this.sizeA = width;
            this.sizeB = height;
            this.isHorizontalOriented = isHorizontal;
            model.maps.AddAmus(this);
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
        public override Size GetRealSize() {
            if (isHorizontalOriented) {
                return new Size(this.sizeA * MainForm.sizeOfSquare - 1,  this.sizeB * MainForm.sizeOfSquare - 1);              
            }
            else {
                return new Size(this.sizeB * MainForm.sizeOfSquare - 1, this.sizeA * MainForm.sizeOfSquare - 1);                        
            }
        }
        public override bool IsInside(int x, int y) {
            throw new NotImplementedException();
        }
    }
    public class RectangleAmusementsFactory : AmusementsFactory {
        protected readonly byte  width, height;
        public bool isHorizontal;
        public RectangleAmusementsFactory(int prize, int fee, int capacity, int runningTime, string name, bool hasEntranceExit, byte width, byte height, Image image)
            : base(prize, fee, capacity, runningTime, name, hasEntranceExit, image) {
                this.width = width;
                this.height = height;
        }
        public override bool CanBeBuild(byte x, byte y, Model model) {
            if (isHorizontal) return AmusementsFactory.CheckFreeLocation(x, y, model, width, height, hasSeparatedEnterExit);
            else return AmusementsFactory.CheckFreeLocation(x, y, model, height, width, hasSeparatedEnterExit);
        }
      
        public override MapObjects Build(byte x, byte y, Model model) {
            return new RectangleAmusements(new Coordinates(x,y), model, prize, entranceFee, capacity, runningTime, name, hasSeparatedEnterExit, width, height, isHorizontal, color, internTypeId, null, null,null);
        }
        public override string GetInfo() {
            return string.Concat(Labels.prize, prize, "\n",    
                                 Labels.capacity, capacity, "\n",
                                 Labels.size, width, " x ", height, "\n",
                                 Labels.hasEntranceExit, hasSeparatedEnterExit);
                                               
        }
    }
    
    public class SquareAmusements : Amusements {
        public readonly byte width;
        public SquareAmusements() { }
       /* public SquareAmusements(Model m, Coordinates c)
            : base(m, c) {
            model.CheckCheapestFee(this.currFee);
        }*/

         public SquareAmusements(Coordinates c, Model m, int prize, int fee, int capacity, int runningTime, string name, bool hasEntranceExit, byte width, Color color, int typeId)
             : base (c, m, prize, fee, capacity, runningTime, name, hasEntranceExit, color, typeId, null, null, null) {
#warning sirka se nastavuje az pote, co se pridava do mapy a se sirkou se pocita"!!!!!!!!!
             this.width = width;
             model.maps.AddAmus(this);
        }


         public override Size GetRealSize() {
             return new Size(this.width * MainForm.sizeOfSquare - 1, this.width * MainForm.sizeOfSquare - 1);              
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
       
        /// <summary>
        /// Determines whether the given coordinates are inside this object or not.
        /// </summary>
        /// <param name="x">An nonnegative integer, represents the game x-coordinate.</param>
        /// <param name="y">An nonnegative integer, represents the game y-coordinate.</param>
        /// <returns></returns>
        public override bool IsInside(int gx, int gy) {
         /*   byte gx = (byte)(x / MainForm.sizeOfSquare);
            byte gy = (byte)(y / MainForm.sizeOfSquare);
            if (gx >= this.coord.x && gx <= this.coord.x + width && gy >= this.coord.y && gy <= this.coord.y + width) return true;
            else return false;*/
            if (gx >= this.coord.x && gx <= this.coord.x + width && gy >= this.coord.y && gy <= this.coord.y + width) return true;
            else return false;
        }
    }
    public class SquareAmusementsFactory : AmusementsFactory {
        public readonly byte width;

        public SquareAmusementsFactory(int prize, int fee, int capacity, int runningTime, string name, bool hasEntranceExit, byte width, Image image)
             : base(prize, fee, capacity, runningTime, name, hasEntranceExit, image) {
                 this.width = width;        
        }
                
        public override bool CanBeBuild(byte x, byte y, Model model) {
            if (AmusementsFactory.CheckFreeLocation(x, y, model, width, width, hasSeparatedEnterExit)) return true;
            else return false;
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new SquareAmusements(new Coordinates(x, y), model, prize, entranceFee, capacity, runningTime, name, hasSeparatedEnterExit, width, color, internTypeId);
        }
        public override string GetInfo() {
            return string.Concat(Labels.prize, prize, "\n",
                                 Labels.capacity, capacity, "\n",
                                 Labels.size, width, " x ", width, "\n",
                                 Labels.hasEntranceExit, hasSeparatedEnterExit);

        }
    }
   

    public class FreeShapedAmusements : Amusements {
        public FreeShapedAmusements(Coordinates c, Model m, int prize, int fee, int capacity, int runningTime, string name, Color color, int typeId)
            : base (c, m, prize, fee, capacity, runningTime, name, false, color, typeId, null,null,null) {
                model.maps.AddAmus(this);
        }
        //nejspis v sobe jeste jednu vnorenou tridu reprezentujici kousky atrakce
        public override List<Coordinates> GetAllPoints() {
            throw new NotImplementedException();
        }
        protected override bool IsInsideInAmusement(int x, int y) {
            throw new NotImplementedException();
        }
        public override Size GetRealSize() {
            throw new NotImplementedException();
        }
        public override bool IsInside(int x, int y) {
            throw new NotImplementedException();
        }
        
    }
    public class FreeShapedAmusementsFactory : AmusementsFactory {
        //todo: konstruktor nedokonceny
        public FreeShapedAmusementsFactory(int prize, string name, Image image) : base(prize, name, image) { }
        public override MapObjects Build(byte x, byte y, Model model) {
            throw new NotImplementedException();
        }
        public override bool CanBeBuild(byte x, byte y, Model model) {
            throw new NotImplementedException();
        }
        public override string GetInfo() {
            throw new NotImplementedException();
        }
    
    }
    
    /// <summary>
    /// napr. pro lavicky
    /// </summary>
    public abstract class LittleComplementaryAmusements : Amusements {
        public LittleComplementaryAmusements( Coordinates c, Model m, int prize, int fee, int capacity, int runningTime, string name, Color color, int typeId) 
          //  : base(c, m, prize, fee, capacity, runningTime, name, hasEntranceExit: false, color: color, typeId: typeId) { }
            : base(c, m, prize, fee, capacity, runningTime, name, false, color, typeId, null, null, null) { }
    }
    public class LittleComplementaryAmusementsFactory : AmusementsFactory {

        public LittleComplementaryAmusementsFactory(int prize, int fee, int capacity, int runningTime, string name, Image image)
            : base(prize, fee, capacity, runningTime, name, hasEntranceExit: false, image: image) {

        }
        public override MapObjects Build(byte x, byte y, Model model) {
            throw new NotImplementedException();
        }
        public override bool CanBeBuild(byte x, byte y, Model model) {
            throw new NotImplementedException();
        }
        public override string GetInfo() {
            throw new NotImplementedException();
        }

    }  
    public class Restaurant : SquareAmusements
    {

        /*public Restaurant(Model m):base() { 
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
        }*/

        public Restaurant(Coordinates c, Model m, int prize, int foodPrize, int capacity, string name, Color color, int typeId)
            : base(c, m, prize, foodPrize, capacity, runningTime: 0, name: name, hasEntranceExit: false, width: 1, color: color, typeId: typeId ) {
            model.mustBeEnter = false; //todo: mozna tyto 2 nejsou potreba
            model.mustBeExit = false;
            this.entrance = new AmusementEnterPath(m, c, this, entrImage, tangible: false);
            this.exit = new AmusementExitPath(m, c, this, exitImage, tangible: false);
            model.maps.AddAmus(this);
            this.status = Status.waitingForPeople;
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

    public class RestaurantFactory : SquareAmusementsFactory {

        public RestaurantFactory(int prize, int foodPrize, int capacity, string name, Image image)  
          :base(prize, foodPrize, capacity, runningTime: 0, name: name, hasEntranceExit: false, width: 1, image: image){        
        }
       
        public override bool CanBeBuild(byte x, byte y, Model model) {
            if (x > model.playingWidth || y > model.playingHeight) return false;
            return model.maps.isFree(x, y);
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new Restaurant(new Coordinates(x, y), model, prize, entranceFee, capacity, name, color, internTypeId);
        }
        public override string GetInfo() {
            return string.Concat(Labels.prize, prize, "\n",
                                 Labels.capacity, capacity, "\n",
                                 Labels.size, width, " x ", width, "\n",
                                 Labels.hasEntranceExit, hasSeparatedEnterExit);

        }
        

    
    }

}
