using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LunaparkGame
{
    public enum Direction {no, N, S, W, E };
   
    public interface IActionable {
        void Action();    
    }
    public class MyDebugException : Exception
    {
        public MyDebugException() : base() { }
        public MyDebugException(string s) : base(s) { }
        
    }

    public struct Coordinates
    {
        public byte x;
        public byte y;
        public Coordinates(byte x, byte y) { this.x = x; this.y = y; }
    }

    public abstract class MapObjects
    {
        public Coordinates coord { protected set; get; }
        protected readonly Model model;
        public Control control { get; set; } //picture
        virtual protected int price { get; set; } //todo: This must be abstract!!!

        public MapObjects(Model m) {
            control.Click += new EventHandler(Click);
            this.model = m;
            model.money -= this.price;
        }
        /// <summary>
        /// Create an instance and show it in the game map
        /// </summary>
        /// <param name="x">the left coordinate</param>
        /// <param name="y">the top coordinate</param>
        public abstract bool Create(int x, int y); //todo mozna system.drawing.point
        /// <summary>
        /// user action
        /// </summary>
        protected abstract void Click(object sender, EventArgs e);
        /// <summary>
        /// user action
        /// </summary>
        public abstract void Demolish();
        


    }

    public abstract class Amusements : MapObjects,IActionable
    {
        #region
        public enum Status { waitingForPeople, running, outOfService, runningOut }
        public int id { get; private set; }
        public AmusementEnterPath entrance { get; protected set; } //todo: je opravdu potreba protected, nestaci private nebo dokonce readonly?
        public AmusementExitPath exit { get; protected set; }
        protected Queue<Person> queue;
        protected List<Person> peopleInList;
        private Status status;
        public int countOfWaitingPeople
        {
            get
            {
                if (queue != null) return queue.Count;
                else throw new MyDebugException("null v Amusements-queue. Opravdu ma tak byt?");
            }
            private set { }
        }
        protected int waitingTime = 0, actRunningTime=0;
        public int visitPrice;
        protected bool isRunningOut=false;
        protected bool isRunning = false;
        //-------popisove vlastnosti
        public int capacity { get; protected set; }
        public int workingPrice { get; protected set; }//provozniCena
        protected int maxWaitingTime, fixedRunningTime;
        public bool hasEntranceExit { get; protected set; }
        protected readonly int initialVisitPrice;
        protected int refundCoef;
        //----------------------------
        #endregion

        public Amusements(Model m) :base(m)       
        {
            throw new NotImplementedException();   
        }
        public Amusements(Coordinates coord, Model m) :base(m)       
        {
            model.lastBuiltAmus = this;                 
            model.mustBeEnter = true;
            this.id = model.amusList.ReturnFreeID();//mozna lepe nastavovat az pri pridani do listu
            model.amusList.Add(this);
            peopleInList = new List<Person>(capacity);
            queue = new Queue<Person>();
            this.coord = coord;
            //todo: dodelat
                       
           // mimoProvoz = true;
           // klikForm = new KlikNaAtrakciForm(this, (LSSAtrakce)form.evidence.atrakceLSS, form);
           // zacatek = true;
           // stav = Stav.mimoProvoz;
        }
        
        public override void Demolish()
        {
            entrance.Demolish();
            exit.Demolish();
            model.money += refundCoef * price;//refund money
            model.amusList.Remove(this);
            //todo:odstranit z mapy
        } 
        
        protected void PickUpPeople(int n) { //naloz lidi
            Person p;           
            for (int i = 1; i <= n; i++)
            {
                p = queue.Dequeue(); 
                try //melo by jit jen o zbytecne ujisteni
                {
                    p.status = Person.Status.inAmus;
                    peopleInList.Add(p);
                    p.money -= this.visitPrice;              
                    //todo: p.Zneviditelni();     Control.visible=false; - co takhle zde vytvorit metodu a pak predat delegata?              
                }
                catch (NullReferenceException e) {
                    throw new MyDebugException("Not expected null in pickUpPeople: " + e.ToString());
                }
                catch (ArgumentNullException e) {
                    throw new MyDebugException("Not expected null in pickUpPeople: " + e.ToString());
                }
            }
            model.money += n * this.visitPrice;        
        }
        protected void DropPeopleOff(){
            foreach (Person p in peopleInList)
	        {
		        p.status=Person.Status.choosesAmus;
               //todo: p.coord=this.exit.coord;
                //todo:clovek.Premisti(vystupX,vystupY);
                //todo:clovek.Zviditelni();
	        }
            peopleInList.Clear();       
        }
        protected virtual void WaitingForPeopleAction() {
            int temp = queue.Count;
            if (temp >= 0.8 * capacity || (waitingTime >= maxWaitingTime && temp > 0))
            {
                waitingTime = 0;
                actRunningTime = 0; //aktualni doba toceni atrakce
                temp = Math.Min(temp, capacity);
                PickUpPeople(temp); //vlozi lidi z fronty do atrakce, zmeni jim stav na VAtrakci a zneviditelni je
                status = Status.running;
                isRunning = true;
            }
            else waitingTime++;
            model.money -= this.workingPrice; //provozni cena        
        }
        protected virtual void RunningAction() {
            if (actRunningTime < fixedRunningTime) actRunningTime++;
            else
            {
                DropPeopleOff();
                if (isRunningOut) status = Status.runningOut;
                else status = Status.waitingForPeople;
                isRunning = false;
                actRunningTime = 0;
            }
            model.money -= workingPrice;
        }
        protected virtual void RunningOutAction()
        {//todo: neni lepsi, aby se lide cekajici ve fronte museli vratit? tj. nikdo dalsi se nesveze, snizi se spokojenost, ale asi neni proveditelne, protoze to pak lide stale mohou vybirat tuto atrakci
            model.money -= workingPrice;
            if (queue.Count == 0 && (!isRunning)) //!bezi-aby se nezmenilo, kdyz uzivatel klikne uprostred behu
            {
                status = Status.outOfService;
                isRunningOut = false;
            }
            else
            {
                isRunningOut = true;
                if (isRunning) status = Status.running;
                else status = Status.waitingForPeople;             
            }       
        }
        public virtual void Action() {
            switch (status)
            {
                case Status.waitingForPeople: WaitingForPeopleAction();
                    break;
                case Status.running: RunningAction();
                    break;
                case Status.runningOut: RunningOutAction();
                    break;
                case Status.outOfService: //nothing
                    break;
                default:
                    break;
            }                     
        }
        public void QueuesPerson(Person p){
            queue.Enqueue(p);  
        }
        public void DeletePersonFromQueue(Person p)
        {
            Person q;
            int c = queue.Count;
            Person[] atemp = queue.ToArray();//due to keep queue in its original order
            queue.Clear();           
            for (int i = c; i >= 0; i--)
			{
                if ((q=atemp[i]) != p) queue.Enqueue(q);
            }           
        }
        public void ChangeId(int id) { this.id = id; }
        public abstract bool CheckFreeLocation(int x, int y);  //hack: nezkontrolovano
        protected virtual bool CheckFreeLocation(int x, int y, byte width, byte height,bool hasEntranceAndExit=true) { 
            if (x + width > model.map.Length || y + height > model.map[0].Length) return false;
            for (int i = x; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (model.map[i][j] != 0) return false;
                }
            }
            if (hasEntranceAndExit) {
                bool free = false;;
                if (x - 1 > 0) for (int i = y; i < y + height; i++)
                        if (model.map[x - 1][i] == 0) { if (free) return free; else free = true; }
                if (x + 1 < model.map.Length) for (int i = y; i < y + height; i++)
                        if (model.map[x - 1][i] == 0) { if (free) return free; else free = true; }
                if (y - 1 > 0) for (int i = x; i < x + width; i++)
                        if (model.map[i][y - 1] == 0) { if (free) return free; else free = true; }
                if (y + 1 < model.map[0].Length) for (int i = y; i < y + height; i++)
                        if (model.map[i][y - 1] == 0) { if (free) return free; else free = true; }
                return free;
            }
            return true;       
        
        }
        /// <summary>
        /// checks if entrance can be built on this place and if yes -> create it
        /// </summary>
        /// <param name="x">an x coordinate in the inside-map</param>
        /// <param name="y">an x coordinate in the inside-map</param>
        /// <returns>if entrance was successful built on this point</returns>
        public bool CheckEntranceAndBuild(int x,int y) {
            if (IsInsideInAmusement(x - 1, y) ||
                IsInsideInAmusement(x + 1, y) ||
                IsInsideInAmusement(x, y - 1) ||
                IsInsideInAmusement(x, y + 1))
            {
                this.entrance = new AmusementEnterPath(model,x,y);
                model.mustBeEnter = false;
                model.mustBeExit = true;
                return true;
            }
            else return false;        
        }
        /// <summary>
        /// checks if exit can be built on this place and if yes -> create it
        /// </summary>
        /// <param name="x">an x coordinate in the inside-map</param>
        /// <param name="y">an x coordinate in the inside-map</param>
        /// <returns>if exit was successful built on this point</returns>
        public bool CheckExitAndBuild(int x, int y)
        {
            if (IsInsideInAmusement(x - 1, y) ||
                IsInsideInAmusement(x + 1, y) ||
                IsInsideInAmusement(x, y - 1) ||
                IsInsideInAmusement(x, y + 1))
            {
                this.exit = new AmusementExitPath(model, x, y);
                model.mustBeExit = false;
                return true;
            }
            else return false;
        }
        protected abstract bool IsInsideInAmusement(int x, int y);
        /// <summary>
        /// create an Item in AtrakceForm and set it (e.g. set visible=false)
        /// </summary>
        //  public abstract static void Initialize();
        public abstract List<Coordinates> GetAllPoints();
        protected override void Click(object sender, EventArgs e)
        {
           // if (!zacatek)
          //  {
                if (model.demolishOn)
                {
                    if (status == Status.outOfService)
                    {
                        Demolish();
                        model.lastClick = null; //v tomto se lisi od funkce predka; v pripade, ze se nejprve rusil vstup a pak atrakce
                    }
                    else
                    {  //todo: MessageBox.Show("Nelze zbořit, dokud atrakce běží.", "Upozornění", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    // todo: HlaskaPoKliknuti();
                }
           // }
        }

    }
    public abstract class SquareAmusements : Amusements
    {
        public byte width { get; protected set; }
        public SquareAmusements(Model m) : base(m) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override bool CheckFreeLocation(int x, int y)
        {          
            return base.CheckFreeLocation(x, y, width, width, hasEntranceAndExit: true);
        }
        protected override bool IsInsideInAmusement(int x, int y)
        {
            if (x >= this.coord.x && x < this.coord.x + this.width &&
                y >= this.coord.y && y < this.coord.y + this.width)
                return true;
            else return false;       
        }
        public override List<Coordinates> GetAllPoints()
        {
            List<Coordinates> list=new List<Coordinates>(width*width);
            for (byte i = coord.x; i < coord.x+width; i++)
            {
                for (byte j = coord.y; j < coord.y + width; j++) list.Add(new Coordinates(i, j));               
            }
            return list;
        }
    }
    /// <summary>
    /// Class for rectangle, not square, amusements. It can have a different orientation.
    /// </summary>
    public abstract class RectangleAmusements : Amusements
    {
        public abstract byte width{get;protected set;}
        public abstract byte height { get; protected set; }
        public readonly bool isHorizontalOriented;


        public RectangleAmusements(Model m,bool isHorizontal=true):base(m)
        {
            isHorizontalOriented = isHorizontal;
        }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
           
        }
        public override bool CheckFreeLocation(int x, int y)
        {
            if (isHorizontalOriented) return CheckFreeLocation(x, y, width, height, hasEntranceAndExit: true);
            else return CheckFreeLocation(x,y,height,width,hasEntranceAndExit:true);
        }
        protected override bool IsInsideInAmusement(int x, int y)
        {
            if (x >= this.coord.x && x < this.coord.x + this.width &&
                y >= this.coord.y && y < this.coord.y + this.height)
                return true;
            else return false;
        }
        public override List<Coordinates> GetAllPoints()
        {
            List<Coordinates> list = new List<Coordinates>(width * height);
            for (byte i = coord.x; i < coord.x + width; i++)
            {
                for (byte j = coord.y; j < coord.y + height; j++) list.Add(new Coordinates(i, j));
            }
            return list;
        }
        
      
    }

    public abstract class FreeShapedAmusements : Amusements
    {
        public FreeShapedAmusements(Model m) : base(m) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
    }



    public class Person : MapObjects,IActionable
    { //todo: Mozna sealed a nebo naopak moznost rozsiritelnosti dal...
        public enum Status { walking, onCrossroad, inAmusQueue, inAmus,choosesAmus, end, initialWalking  }
        public int money;
        public Coordinates coord;
        public int id { get; private set; }
        public Status status { set; private get; }
        public Person(Model m) : base(m) {
            //must set money
        }
        protected override void Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
        public void Action()
        {
            throw new NotImplementedException();
        }


    }

    public abstract class Path : MapObjects
    {
        public Direction[] signpostAmus;//rozcestnik
        public Path(Model m, int x, int y) : base(m) { 
            signpostAmus=new Direction[m.maxAmusementsCount];
            //todo: mozna neni treba, overit
            for (int i = 0; i < signpostAmus.Length; i++) signpostAmus[i] = Direction.no;
          
        }
        public override bool Create(int x, int y) {
            throw new NotImplementedException();
        }

        protected override void Click(object sender, EventArgs e)
        {
            //nothing
        }
       
    }

    
}
