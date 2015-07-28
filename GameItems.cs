using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LunaparkGame
{
    public enum Direction { no, N = 1, E = 2, S = 3, W = 4, here };
   
    public interface IActionable {
        void Action();    
    }
    public interface IUnmoving {
        Coordinates coord{get;set;}
    }
    public interface IMoving {
        byte x { get; set; }
        byte y { get; set; }
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
        virtual protected int price { get; set; } //todo: This must be abstract!!! - az budu znat ceny

        protected MapObjects(Model m) {
            this.model = m;
            control.Click += new EventHandler(Click);
            model.money -= this.price;
        }
        public MapObjects(Model m,Coordinates coord):this(m)
        {
            this.coord = coord;
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
        public abstract void Destruct();
        


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
        private Status status=Status.outOfService;
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
        public int currVisitPrice;
        //public abstract int originalVisitPrice{get;protected set;}//todo:patri jinam
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

         public Amusements( Model m, Coordinates c) :base(m,c)       
        {
            model.lastBuiltAmus = this;                 
            model.mustBeEnter = true;
            this.id = model.amusList.GetFreeID();//mozna lepe nastavovat az pri pridani do listu
           
            peopleInList = new List<Person>(capacity);
            queue = new Queue<Person>();
            this.coord = coord;
            model.amusList.Add(this);
            model.maps.AddAmus(this);
            //todo: dodelat
                       
           // mimoProvoz = true;
           // klikForm = new KlikNaAtrakciForm(this, (LSSAtrakce)form.evidence.atrakceLSS, form);
           // zacatek = true;
           // stav = Stav.mimoProvoz;
        }
        
        public override void Destruct()
        {
            entrance.Destruct();
            exit.Destruct();
            model.money += refundCoef * price;//refund money
            model.amusList.Remove(this);
            model.maps.RemoveAmus(this);
            //todo:odstranit z mapy
        } 
        
        protected void PickUpPeople(int n) { //naloz lidi
            Person p;           
            for (int i = 1; i <= n; i++)
            {
                p = queue.Dequeue(); 
                try //melo by jit jen o zbytecne ujisteni
                {
                    p.visible = false;
                    p.status = Person.Status.inAmus;
                    peopleInList.Add(p);
                    p.money -= this.currVisitPrice;              
                    //todo: p.Zneviditelni();     Control.visible=false; - co takhle zde vytvorit metodu a pak predat delegata?              
                }
                catch (NullReferenceException e) {
                    throw new MyDebugException("Not expected null in pickUpPeople: " + e.ToString());
                }
                catch (ArgumentNullException e) {
                    throw new MyDebugException("Not expected null in pickUpPeople: " + e.ToString());
                }
            }
            model.money += n * this.currVisitPrice;        
        }
        protected void DropPeopleOff(){
            foreach (Person p in peopleInList)
	        {
		        p.status=Person.Status.choosesAmus;
                p.visible = true;
                p.SetCoordinates(this.exit.coord);
               
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
       // public void ChangeId(int id) { this.id = id; }
        public abstract bool CheckFreeLocation(byte x,byte y);  //hack: nezkontrolovano
        protected virtual bool CheckFreeLocation(byte x,byte y, byte width, byte height,bool hasEntranceAndExit=true) { 
            if (x + width > model.width || y + height > model.height) return false;
            for (byte i = x; i < x+width; i++)
            {
                for (byte j = y; j < y+height; j++)
                {
                    if (!model.maps.isFree(i,j)) return false;
                }
            }
            if (hasEntranceAndExit) {
                bool free = false;
                if (x - 1 > 0) for (byte i = y; i < y + height; i++)
                        if (model.maps.isFree((byte)(x-1),i)) { if (free) return free; else free = true; }
                if (x + 1 < model.width) for (byte i = y; i < y + height; i++)
                        if (model.maps.isFree((byte)(x + 1), i)) { if (free) return free; else free = true; }
                if (y - 1 > 0) for (byte i = x; i < x + width; i++)
                        if (model.maps.isFree(i,(byte)(y-1))) { if (free) return free; else free = true; }
                if (y + 1 < model.width) for (byte i = y; i < y + height; i++)
                        if (model.maps.isFree(i,(byte)(y+1))) { if (free) return free; else free = true; }
                return free;
            }
            return true;       
        
        }
        /// <summary>
        /// checks if the entrance can be built on the given place and creates it if yes
        /// </summary>
        /// <param name="x">an x coordinate in the inside-map</param>
        /// <param name="y">an x coordinate in the inside-map</param>
        /// <returns>true if the entrance was successful built on this point, otherwise false.</returns>
        public bool CheckEntranceAndBuild(int x,int y) {
            if (IsInsideInAmusement(x - 1, y) ||
                IsInsideInAmusement(x + 1, y) ||
                IsInsideInAmusement(x, y - 1) ||
                IsInsideInAmusement(x, y + 1))
            {
                this.entrance = new AmusementEnterPath(model, new Coordinates((byte)x,(byte)y));
                model.mustBeEnter = false;
                model.mustBeExit = true;
                return true;
            }
            else return false;        
        }
        /// <summary>
        /// checks if the exit can be built on this place and creates it if yes.
        /// </summary>
        /// <param name="x">an x coordinate in the inside-map</param>
        /// <param name="y">an x coordinate in the inside-map</param>
        /// <returns>true if exit was successful built on the given point, otherwise false</returns>
        public bool CheckExitAndBuild(int x, int y)
        {
            if (IsInsideInAmusement(x - 1, y) ||
                IsInsideInAmusement(x + 1, y) ||
                IsInsideInAmusement(x, y - 1) ||
                IsInsideInAmusement(x, y + 1))
            {
                this.exit = new AmusementExitPath(model,new Coordinates((byte)x,(byte)y));
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
                        Destruct();
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
        public SquareAmusements(Model m, Coordinates c) : base(m,c) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override bool CheckFreeLocation(byte x, byte y)
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


        public RectangleAmusements(Model m,Coordinates c,bool isHorizontal=true):base(m,c)
        {
            isHorizontalOriented = isHorizontal;
        }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
           
        }
        public override bool CheckFreeLocation(byte x, byte y)
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
        public FreeShapedAmusements(Model m, Coordinates c) : base(m,c) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
    }



    public class Person : MapObjects,IActionable
    { //todo: Mozna sealed a nebo naopak moznost rozsiritelnosti dal...
        private static Random rand = new Random();
        const int minMoney = 200, maxMoney = 2000, minPatience=1, maxPatience=10;
        public enum Status {initialWalking, walking, onCrossroad, inAmusQueue, inAmus,choosesAmus, end }
 
        public int money { get; set; } //{ if (value >= 0) return value; } } - spatna syntaxe
        private readonly int patience;
        public readonly int id;
        public readonly int maxAcceptablePrice;//max price which he is willing to pay per an amusement
        public bool visible { get; set; }
        //----provozni hodnoty-----
        private int remainingStepsCount=0;//pocet zbyvajicich kroku
        private int waitingTimeInQueue = 0, initialWalkingTime=2*MainForm.sizeOfSquare;
        private int contenment = 0;//spokojenost
        private int hunger=0;
        protected int x, y; //instead of coord //todo: casem s tim neco udelat, napr. 2 abstract tridy od MapObjects apod.
        private Direction currDirection = Direction.no;
        private int currAmusId;
        public Status status { set; protected get; }
        
        public Person(Model m, byte x, byte y) : base(m) {
            //must set money
            this.id = m.persList.GetFreeId();
            this.status = Status.initialWalking;
            this.money = rand.Next(Person.minMoney,Person.maxMoney);
            this.patience = rand.Next(Person.minPatience, Person.maxPatience);
            this.maxAcceptablePrice = 100;//todo: nastavit nejak podle vstupnich cen vstupu na atrakce
            this.price = 0;
            this.x = x;
            this.y = y;
            this.visible = true;

            m.persList.Add(this);

        }
        public void Action()
        {
            switch (status)
            {
                case Status.walking:{
                    #region
                    if (remainingStepsCount > 0) {
                        remainingStepsCount++;
                        switch (currDirection)
                        {
                            case Direction.N: y--;//todo: add to move fronta in model
                                break;
                            case Direction.S: y++;//todo: add to move fronta in model
                                break;
                            case Direction.W: x--;//todo: add to move fronta in model
                                break;
                            case Direction.E: x++;//todo: add to move fronta in model
                                break;
                            case Direction.no: { status = Status.choosesAmus;}
                                break;
                            case Direction.here: { 
                                Amusements a=model.maps.GetAmusement(x,y);
                                if (a == null) { 
                                    //todo: ubrat spokojenost
                                    status = Status.choosesAmus;
                                }
                                if (a.id != currAmusId) throw new MyDebugException("Person.Action - lisi se ocekavane id");
                                if (a.currVisitPrice > maxAcceptablePrice || a.currVisitPrice > money)
                                {
                                    contenment = contenment - 10;//todo: nastavit poradne
                                    status = Status.choosesAmus;
                                }
                                else
                                {
                                    status = Status.inAmusQueue;
                                    waitingTimeInQueue = 0;
                                    a.QueuesPerson(this);
                                }
                            }
                                break;
                        }
                    }
                    else {
                        status = Status.onCrossroad;
                    }
                    #endregion
                }
                    break;
                case Status.onCrossroad: {
                    #region               
                    currDirection=model.maps.GetDirectionToAmusement(currAmusId,x,y);
                    status = Status.walking;
                    remainingStepsCount = MainForm.sizeOfSquare;
                    #endregion
                }                 
                    break;
                case Status.inAmusQueue:{
                    #region
                        if (waitingTimeInQueue > patience) {
                             contenment = contenment - 10;//todo: mozna udelat: odejde, pokud prekroci patience a vzdy jindy se drobne snizi spokojenost
                        }
                        else waitingTimeInQueue++;
                    #endregion
                     }
                    break;
                case Status.inAmus:{
                        contenment = Math.Min(contenment+1,100);//todo: lepe zbavit se konstant a mit nekde lepe rozmyslene
                    }
                    break;
                case Status.choosesAmus: {
                    currAmusId = ChooseAmusement();
                    status = Status.onCrossroad;
                }
                    break;
                case Status.initialWalking:
                    {
                    #region
                    if (initialWalkingTime > 0)
                    {
                        initialWalkingTime--;
                                                                    
                        switch (currDirection)
                        {
                            case Direction.E:  
                                if (model.maps.IsPath(x + 1, y)) { x++; //todo: Zaradit do move fronty
                                }
                                else currDirection=(Direction)rand.Next(1,5);
                                break;
                            case Direction.N:
                                if (model.maps.IsPath(x , y-1)) { y--; }
                                else currDirection = (Direction)rand.Next(1, 5);
                                break;
                            case Direction.S: 
                                if (model.maps.IsPath(x , y+1)) { y++; }
                                else currDirection = (Direction)rand.Next(1, 5);
                                break;
                            case Direction.W: 
                               if (model.maps.IsPath(x -1, y)) { x--; }
                               else currDirection = (Direction)rand.Next(1, 5);
                                break;
                            default: currDirection = (Direction)rand.Next(1,5);
                                break;
                        }
                        if (rand.Next(1, 12) % 7 == 0) currDirection = (Direction)rand.Next(1, 4);  
                    }
                    else status = Status.choosesAmus;
                    #endregion
                    }

                    break;               
                case Status.end://todo: je vubec k necemu???
                    break;
                default:
                    break;
            }
        }
              
        /// <summary>
        /// Returns the id of the chosen amusement.
        /// </summary>
        /// <returns>An nonnegative int, which represents the id of an amusement</returns>
        public int ChooseAmusement() {
            //todo: mam mene penez nez stoji nejlevnejsi atrakce -> jdu domu
            if (contenment == 0) return model.amusList.GetGateId();
            if (hunger > 1800) //2000=100 %, i.e. 2000*0.9
                return model.amusList.GetRandomRestaurant(); //kdyz je hlad 90%, vybira obcerstveni
           
            int temp = rand.Next(101); //0-100
            //---- less contenment increases the probability of leaving the park
            if (temp > contenment) return model.amusList.GetGateId(); 
            //---- more hunger -> bigger probability of visiting a restaurant
            if (temp < hunger / 20) return model.amusList.GetRandomRestaurant();
            //---- go to an amusement
            return model.amusList.GetRandomAmusement();
        }
        public void SetCoordinates(Coordinates c) {
            this.x = c.x;
            this.y = c.y;
        }
        protected override void Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
       
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Destruct()
        {
            //todo: vlozit do destruct fronty v modelu
            model.persList.Remove(this);
        }
       


    }
 
    public abstract class Path : MapObjects
    {
        public Direction[] signpostAmus;//rozcestnik
        protected Path(Model m):base(m){ }
        public Path(Model m, Coordinates c) : base(m,c) { 
            signpostAmus=new Direction[m.maxAmusementsCount];
            //todo: mozna neni treba, overit
            for (int i = 0; i < signpostAmus.Length; i++) signpostAmus[i] = Direction.no;
            model.maps.AddPath(this);
        }
        public override bool Create(int x, int y) {
            throw new NotImplementedException();
        }

        protected override void Click(object sender, EventArgs e)
        {
            //nothing
        }
        public override void Destruct()
        {
            model.maps.RemovePath(this);
        }
       
    }

    
}
