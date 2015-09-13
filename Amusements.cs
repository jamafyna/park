using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization;

namespace LunaparkGame
{
    [Serializable]
    public class Gate : Amusements {

        public const int width = 1;
        public const int height = 3;
        private const int workingPrize = 10;
        public new const int originalFee = 50;
        public int entranceFee;
        [NonSerialized]
        private ProbabilityGenerationPeople peopleGeneration;
        public new Status State {
            get { return status; }
            set { if (value != Status.waitingForPeople) status = value; }
        }
        public Gate(Model m, Coordinates c) {
            this.model = m;
            this.coord = c;
            this.entrance = new AmusementEnterPath(m, new Coordinates(c.x, (byte)(c.y + height / 2)), this, tangible: false);
            this.exit = new AmusementExitPath(m, new Coordinates(c.x, (byte)(c.y + height / 2)), this, tangible: false);
            this.CurrFee = 0;
            this.entranceFee = originalFee;
            m.maps.AddAmus(this);
            peopleGeneration = new ProbabilityGenerationPeople(model);
        }

        public Gate(Model m, Coordinates c, Coordinates entrance, Coordinates exit) {
            this.model = m;
            this.coord = c;
            this.entrance = new AmusementEnterPath(m, entrance, this, tangible: false);
            this.exit = new AmusementExitPath(m, exit, this, tangible: false);
            m.maps.AddAmus(this);
       }
       
        [OnDeserialized]
        private new void SetValuesAndCheckOnDeserialized(StreamingContext context) {           
            base.SetValuesAndCheckOnDeserialized(context);
            peopleGeneration = new ProbabilityGenerationPeople(model);
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
                        int a = (exit.coord.x + 1) * MainForm.sizeOfSquare;
                        int b = exit.coord.y * MainForm.sizeOfSquare;
                        if (!model.maps.IsPath(a, b)) break;
                        if (peopleGeneration.ShouldCreateNewPerson()) {
                            p = new Person(model, a + 1, b + MainForm.sizeOfSquare / 2);
                            model.MoneyAdd(this.entranceFee);
                        }
                        model.MoneyAdd(-workingPrize);
                        #endregion
                    }
                    break;
                case Status.outOfService: //nothing
                    break;
                case Status.runningOut: {
                        // deleting people from the park
                    Person p;
                    while (queue.TryDequeue(out p)) p.Destruct();
                    if (model.CurrPeopleCount == 0) {
                        status = Status.outOfService;
                        model.parkClosed = true;
                    }
                    model.MoneyAdd(-workingPrize);
                }
                    break;
                case Status.disposing: //nothing
                    break;
                default: status = Status.running;
                    break;
            }
           
        }
       
        public override void Destruct() {
            // nothing, the gate cannot be demolished
        }
       
        public override List<Coordinates> GetAllPoints() {
            List<Coordinates> l = new List<Coordinates>();
            l.Add(entrance.coord);
            //l.Add(exit.coord);
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
                Labels.totalVisitorsCount, model.TotalPeopleCount, "\n",
                Labels.longtermContentment, model.GetLongTermContentment(), "\n",
                Labels.currentContentment, model.persList.contenment
                );       
        }
        
        // the method below is irrelevant
        protected override bool IsInsideInAmusement(int x, int y) { return false; }
  
    }

   /// <summary>
    /// Class for rectangle, not square, amusements. It can have a different orientation.
    /// </summary>
    [Serializable]
    public class RectangleAmusements : Amusements {
        public readonly byte sizeA;
        public readonly byte sizeB;
        public readonly bool isHorizontalOriented;


        public RectangleAmusements(Coordinates c, Model m, int prize, int fee, int capacity, int runningTime, string name, bool hasEntranceExit, byte width, byte height, bool isHorizontal, Color color, int typeId, int workingCost, int attractiveness)
        : base (c, m, prize, fee, capacity, runningTime, name, hasEntranceExit, color, typeId, workingCost, attractiveness){
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
    [Serializable]
    public class RectangleAmusementsFactory : AmusementsFactory {
        protected readonly byte  width, height;
        public bool isHorizontal;
        public RectangleAmusementsFactory(int prize, int fee, int capacity, int runningTime, string name, bool hasEntranceExit, byte width, byte height, int workingCost, int attractiveness)
            : base(prize, fee, capacity, runningTime, name, hasEntranceExit, workingCost, attractiveness) {
                this.width = width;
                this.height = height;
        }
        public override bool CanBeBuild(byte x, byte y, Model model) {
            if (isHorizontal) return AmusementsFactory.CheckFreeLocation(x, y, model, width, height, hasSeparatedEnterExit);
            else return AmusementsFactory.CheckFreeLocation(x, y, model, height, width, hasSeparatedEnterExit);
        }
      
        public override MapObjects Build(byte x, byte y, Model model) {
            return new RectangleAmusements(new Coordinates(x,y), model, prize, entranceFee, capacity, runningTime, name, hasSeparatedEnterExit, width, height, isHorizontal, color, internTypeId, workingCost, attractiveness);
        }
        public override string GetInfo() {
            return string.Concat(Labels.prize, prize, "\n",    
                                 Labels.capacity, capacity, "\n",
                                 Labels.size, width, " x ", height, "\n",
                                 Labels.hasEntranceExit, hasSeparatedEnterExit);
                                               
        }
    }
    
    [Serializable]
    public class SquareAmusements : Amusements {
        public readonly byte width;
        public SquareAmusements() { }
       /* public SquareAmusements(Model m, Coordinates c)
            : base(m, c) {
            model.CheckCheapestFee(this.currFee);
        }*/

         public SquareAmusements(Coordinates c, Model m, int prize, int fee, int capacity, int runningTime, string name, bool hasEntranceExit, byte width, Color color, int typeId, int workingCost, int attractiveness)
             : base (c, m, prize, fee, capacity, runningTime, name, hasEntranceExit, color, typeId, workingCost, attractiveness) {
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
    [Serializable]
    public class SquareAmusementsFactory : AmusementsFactory {
        public readonly byte width;

        public SquareAmusementsFactory(int prize, int fee, int capacity, int runningTime, string name, bool hasEntranceExit, byte width, int workingCost, int attractiveness)
             : base(prize, fee, capacity, runningTime, name, hasEntranceExit, workingCost, attractiveness) {
                 this.width = width;        
        }
                
        public override bool CanBeBuild(byte x, byte y, Model model) {
            if (AmusementsFactory.CheckFreeLocation(x, y, model, width, width, hasSeparatedEnterExit)) return true;
            else return false;
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new SquareAmusements(new Coordinates(x, y), model, prize, entranceFee, capacity, runningTime, name, hasSeparatedEnterExit, width, color, internTypeId, workingCost, attractiveness);
        }
        public override string GetInfo() {
            return string.Concat(Labels.prize, prize, "\n",
                                 Labels.capacity, capacity, "\n",
                                 Labels.size, width, " x ", width, "\n",
                                 Labels.hasEntranceExit, hasSeparatedEnterExit);

        }
    }
   
    [Serializable]
    public class FreeShapedAmusements : Amusements {
        public FreeShapedAmusements(Coordinates c, Model m, int prize, int fee, int capacity, int runningTime, string name, Color color, int typeId, int workingCost, int attractiveness)
            : base (c, m, prize, fee, capacity, runningTime, name, false, color, typeId, workingCost ,attractiveness) {
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
    [Serializable]
    public class FreeShapedAmusementsFactory : AmusementsFactory {
        //todo: konstruktor nedokonceny
        public FreeShapedAmusementsFactory(int prize, string name, int workingCost, int attractiveness) : base(prize, name, workingCost, attractiveness) { }
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
    [Serializable]
    public abstract class LittleComplementaryAmusements : Amusements {
        public LittleComplementaryAmusements(Coordinates c, Model m, int prize, int fee, int capacity, int runningTime, string name, Color color, int typeId, int workingCost, int attractiveness) 
          //  : base(c, m, prize, fee, capacity, runningTime, name, hasEntranceExit: false, color: color, typeId: typeId) { }
            : base(c, m, prize, fee, capacity, runningTime, name, false, color, typeId, workingCost, attractiveness) { }
    }
    [Serializable]
    public class LittleComplementaryAmusementsFactory : AmusementsFactory {

        public LittleComplementaryAmusementsFactory(int prize, int fee, int capacity, int runningTime, string name, int workingCost, int attractiveness)
            : base(prize, fee, capacity, runningTime, name, hasEntranceExit: false, workingCost: workingCost, attractiveness: attractiveness) {

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
    [Serializable]
    public class Restaurant : SquareAmusements
    {
        public Restaurant(Coordinates c, Model m, int prize, int foodPrize, int capacity, string name, Color color, int typeId, int workingCost, int attractiveness)
            : base(c, m, prize, foodPrize, capacity, runningTime: 0, name: name, hasEntranceExit: false, width: 1, color: color, typeId: typeId, workingCost: workingCost, attractiveness: attractiveness ) {
            model.mustBeEnter = false; //todo: mozna tyto 2 nejsou potreba
            model.mustBeExit = false;
            this.entrance = new AmusementEnterPath(m, c, this, tangible: false);
            this.exit = new AmusementExitPath(m, c, this,  tangible: false);
            model.maps.AddAmus(this);
            this.status = Status.waitingForPeople;
            model.MarkBackInService(this);
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
    [Serializable]
    public class RestaurantFactory : SquareAmusementsFactory {

        public RestaurantFactory(int prize, int foodPrize, int capacity, string name, int workingCost, int attractiveness)  
          :base(prize, foodPrize, capacity, runningTime: 0, name: name, hasEntranceExit: false, width: 1, workingCost: workingCost, attractiveness: attractiveness){        
        }
       
        public override bool CanBeBuild(byte x, byte y, Model model) {
            if (x > model.playingWidth || y > model.playingHeight) return false;
            return model.maps.isFree(x, y);
        }
        public override MapObjects Build(byte x, byte y, Model model) {
            return new Restaurant(new Coordinates(x, y), model, prize, entranceFee, capacity, name, color, internTypeId, workingCost, attractiveness);
        }
        public override string GetInfo() {
            return string.Concat(Labels.prize, prize, "\n",
                                 Labels.capacity, capacity, "\n",
                                 Labels.size, width, " x ", width, "\n",
                                 Labels.hasEntranceExit, hasSeparatedEnterExit);

        }
        

    
    }

}
