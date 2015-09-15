using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Concurrent;
using System.Drawing;
using System.Runtime.Serialization;


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
    public interface IClickable { 
    
    }
    public interface IButtonCreatable {
       
    }
   

    public class MyDebugException : Exception
    {
        public MyDebugException() : base() { }
        public MyDebugException(string s) : base(s) { }
        
    }
    public class MyDeserializationException : MyDebugException {
        public MyDeserializationException() : base() { }
        public MyDeserializationException(string s) : base(s) { }
    }
   
    [Serializable]
    public struct Coordinates
    {
        public byte x;
        public byte y;
        public Coordinates(byte x, byte y) { this.x = x; this.y = y; }
    }
    [Serializable]
    public abstract class MapObjectsFactory {
        public int internTypeId;
        public readonly string name;
        public readonly int prize;
        public MapObjectsFactory(int prize, string name) {
            this.prize = prize;
            this.name = name;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The intern x-coordinate of the playing map.</param>
        /// <param name="y">The intern x-coordinate of the playing map.</param>
        /// <returns>true if all conditions of building are satisfied, otherwise false.</returns>
        public abstract bool CanBeBuild(byte x, byte y, Model model);
        public abstract MapObjects Build(byte x, byte y, Model model);
    }

    [Serializable]
    public abstract class MapObjects
    {      
        /// <summary>
        /// Determines view order, value: 0 (closest) - 10 (furthest)
        /// </summary>
        public int zIndex { protected set ; get; }
        public Coordinates coord { protected set; get; }
        protected Model model;
        [NonSerialized]
        public bool isClicked = false;
        public readonly int prize;
        public readonly int internTypeID;
        public readonly bool tangible;
        public MapObjects() { }
        protected MapObjects(Model m, int prize, int typeID, bool tangible=true) {
            this.model = m;
            this.prize = prize;
            this.tangible = tangible;
            model.MoneyAdd(-this.prize);
            this.internTypeID = typeID;
        }
        public MapObjects(Model m,Coordinates coord, int prize, int typeID, bool tangible=true):this(m, prize, typeID, tangible)
        {            
            this.coord = coord;
            this.internTypeID = typeID;
        }

        [OnDeserialized]
        protected void SetValuesAndCheckOnDeserialized(StreamingContext context) {
            isClicked = false;       
        }

        /// <summary>
        /// user action
        /// </summary>
        public abstract void Click(object sender, EventArgs e);
        /// <summary>
        /// user action
        /// </summary>
        public virtual void Destruct() {
           // model.dirtyDestruct.Enqueue(this);
        }
        public abstract Size GetRealSize();
        public abstract Point GetRealCoordinates();
       /// <summary>
       /// Determines whether the given coordinates are inside this object or not.
       /// </summary>
       /// <param name="x">An nonnegative integer, represents the real or game x-coordinate.</param>
       /// <param name="y">An nonnegative integer, represents the real or game y-coordinate.</param>
       /// <returns></returns>
        public abstract bool IsInside(int x, int y);
    }

   
    /// <summary>
    /// ts,pokud se pouzivaji fce Destruct/Click...z hlavniho vlakna
    /// </summary>
    [Serializable]
    public abstract class Amusements : MapObjects, IActionable//, IButtonCreatable
    {
        #region
        public enum Status { waitingForPeople, running, outOfService, runningOut, disposing }
       
      //  public int InternTypeId { get { return typeId; } set { } }
        private int id;
        public int Id { get { return id; } private set { id = value; } }
        public AmusementPath entrance { get;  set; } 
        public AmusementPath exit { get; set; }
        protected  ConcurrentQueue<Person> queue=new ConcurrentQueue<Person>();
        [NonSerialized]
        protected object queueDeleteLock = new object();
        [NonSerialized]
        protected ReaderWriterLockSlim queueAddRWLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        /// <summary>
        /// Contains people who are in the amusement now.
        /// </summary>
        protected List<Person> peopleInList;
        public Status State { get { return status; } set { status = value; } }
        protected volatile Status status=Status.outOfService;
        public int CountOfWaitingPeople
        {
            get
            {
                if (queue != null) return queue.Count;
                else throw new MyDebugException("null v Amusements-queue. Opravdu ma tak byt?");
            }
            private set { }
        }
        protected int waitingTime = 0, actRunningTime=0;
        public bool IsRunningOut { get { return isRunningOut; } }
        protected bool isRunningOut = false;
        protected bool isRunning = false;
        /// <summary>
        /// 600 = 100 %
        /// </summary>
        protected int crashnessPercent = 0;
        public int GetCrashnessPercent {
            private set { } 
            get {
                if(crashnessPercent > 600) return 100;
                else return crashnessPercent/6;
                }
        }
        public bool Crashed { 
            private set{} 
            get { if (crashnessPercent >= 600) return true; else return false; } 
        }
        //-------characteristics-------------
       // public readonly int typeId;
        public readonly int capacity, originalFee;
        private int currFee;
        public int CurrFee { get { return currFee; } set { if (value > 0) currFee = value; } }
        protected readonly int maxWaitingTime, fixedRunningTime;        
        public readonly string name;
        public readonly int attractiveness;
        protected readonly bool hasSeparatedEnterExit;
        public readonly Color color;
        
        public int WorkingCost { get; protected set; }

        //----------------------------
        #endregion
        public Amusements(){}
       
        public Amusements(Coordinates c, Model m, int prize, int fee, int capacity, int runningTime, string name, bool hasEntranceExit, Color color, int typeId, int workingPrice, int attractiveness)
            : base(m, c, prize, typeId)       
        {
            this.zIndex = 0;
            this.originalFee = fee;
            this.currFee = fee;
            this.capacity = capacity;
            this.WorkingCost = workingPrice;
            this.attractiveness = attractiveness;
                     
            this.fixedRunningTime = runningTime;
            this.maxWaitingTime = (int)(runningTime*1.5);
            this.name = name;
            this.hasSeparatedEnterExit = hasEntranceExit;
            this.color = color;
            //this.typeId = typeId;
            peopleInList = new List<Person>(capacity);

           model.LastBuiltAmus = this;                 
            if(hasEntranceExit) model.mustBeEnter = true;
            this.Id = model.amusList.GetFreeID();                                 
            model.amusList.Add(this);
            model.maps.AddAmus(this);
            model.CheckCheapestFee(this.CurrFee);
           
       
           // mimoProvoz = true;
           // zacatek = true;
         
        }
        [OnDeserialized]
        private new void SetValuesAndCheckOnDeserialized(StreamingContext context) {
            base.SetValuesAndCheckOnDeserialized(context);
            if (this.id < 0 || this.waitingTime < 0|| this.maxWaitingTime < 0) throw new MyDeserializationException("Wrong deserialization in Amusements, values < 0");
            queueAddRWLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            queueDeleteLock = new object();

        }

        /// <summary>
        /// Demolish the amusement, remove it from the maps and the amusList, refund money.
        /// </summary>
        public override void Destruct()
        {
            try { if (entrance != null) entrance.Destruct(); }
            catch (NullReferenceException) { 
                // nothing, everything is OK
            }
            try {  if (exit != null) exit.Destruct();}               
            catch (NullReferenceException) {
                // nothing, everything is OK
            }
           
            this.status = Status.disposing;
            model.MoneyAdd((int)((1-crashnessPercent/600.0)*0.9*prize));//refund money
            
             model.amusList.Remove(this);
            model.maps.RemoveAmus(this);
           
            
        }
        /// <summary>
        /// Repairs the amusement and decreases count of money of the repairing value, if there is no money, shows a warning MessageBox
        /// </summary>
        public void RepairWhole() { 
            // it isnt 100% correct because of not atomic money asking, but never mind, there can be a small debt
            double repairPrize = crashnessPercent / 600.0 * prize * 0.8;
            if (model.GetMoney() >= repairPrize) { crashnessPercent = 0; model.MoneyAdd(-(int)repairPrize); }
            else MessageBox.Show(Labels.warningMessBox, Notices.cannotRepairNoMoney, MessageBoxButtons.OK);
        }
        /// <summary>
        /// Tries to repair the given percent part of the amusement and if it is succesfull, decreases count of money
        /// </summary>
        /// <param name="percent">Byte number, count of percent of which the amusement should be repaired.</param>
        public bool TryRepairPart(int percent) {
            // it isnt 100% correct because of not atomic money asking, but never mind, there can be a small debt
            percent = Math.Min(percent * 6, crashnessPercent);
            double repairPrize = percent / 600.0 * prize * 0.1;
            if (model.GetMoney() >= repairPrize) {
                Interlocked.Add(ref crashnessPercent, -1 * percent);
                return true;
            }
            else return false;        
        }
        
        /// <summary>
        /// Puts n waiting people from queue to the amusement. It is assumed that nobody can dequeu while running this method (queue can only growth)!
        /// </summary>
        /// <param name="n">count of items to be relocated, assumed that n isn't bigger than queue.Count anywhere</param>
        protected virtual void PickUpPeople(int n) { 
            Person p;           
            for (int i = 1; i <= n; i++)
            {
                if (!queue.TryDequeue(out p)) throw new MyDebugException("Amusements.PickUpPeople: TryDequeu neuspelo, i kdyz by melo" ); 
                try //melo by jit jen o zbytecne ujisteni
                {
                    p.visible = false;
                    p.status = Person.Status.inAmus;
                    peopleInList.Add(p);
                    p.AddMoney (- this.CurrFee);                    
                }
                catch (NullReferenceException e) {
                    throw new MyDebugException("Not expected null in pickUpPeople: " + e.ToString());
                }
                catch (ArgumentNullException e) {
                    throw new MyDebugException("Not expected null in pickUpPeople: " + e.ToString());
                }
            }
            model.MoneyAdd(n * this.CurrFee);        
        }
        protected virtual void DropPeopleOff(){
            int quarter=MainForm.sizeOfSquare/4;
            int i = 0;
            int j = 0;
            
            foreach (Person p in peopleInList)
	        {
		        p.status=Person.Status.choosesAmus;
                p.SetRealCoordinates(exit.coord.x*MainForm.sizeOfSquare+2*quarter+i,exit.coord.y*MainForm.sizeOfSquare+2*quarter+j);
                i = -(i + 1) % quarter;
                j = -(j + 2) % quarter;
                p.visible = true;
	        }
            peopleInList.Clear();       
        }
        protected virtual void WaitingForPeopleAction() {
            lock(queueDeleteLock) {
                int count = queue.Count;
                if (count >= 0.8 * capacity || (waitingTime >= maxWaitingTime && count > 0)) {
                    waitingTime = 0;
                    actRunningTime = 0;
                    count = Math.Min(count, capacity);
                    PickUpPeople(count);
                    status = Status.running;
                    isRunning = true;
                }
                else waitingTime++;
            }
            model.MoneyAdd(-this.WorkingCost / 64);         
        }
        protected virtual void RunningAction() {
            if (actRunningTime < fixedRunningTime) {
                actRunningTime++;
                crashnessPercent++;
            }
            else {
                DropPeopleOff();
                if (isRunningOut) { status = Status.runningOut; Interlocked.Decrement(ref model.currBuildedItems[internTypeID]); }
                else status = Status.waitingForPeople;
                isRunning = false;
                actRunningTime = 0;
                
            }
           model.MoneyAdd( - this.WorkingCost);
        }
        protected virtual void RunningOutAction() {
        
            model.MoneyAdd(-WorkingCost);
            
                if (!isRunning) // it is used if user clicked while people are in amus ( amus is running )
                 {
                    status = Status.outOfService;
                    model.MarkOutOfService(this);
                    this.entrance.signpostAmus[this.Id] = Direction.no;
#warning zde nesmi delat prekladac optimalizace, musi byt v tomto poradi
                    DeleteAllPeopleFromQueue(); 
                    isRunningOut = false;
                }
                else {
                    isRunningOut = true;
                    status = Status.running;
                }                  
        }
        /// <summary>
        /// Simmulate an activity of the amusement. 
        /// </summary>
        public virtual void Action() {
            // Cannot use anything from AmusementsList (it could create an cycle)!
            if (crashnessPercent >= 590) {
                status = Status.runningOut;
                Interlocked.Decrement(ref model.currBuildedItems[internTypeID]);      
      
            }
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
        public bool TryQueuesPerson(Person p){
            queueAddRWLock.EnterReadLock();
            try {
                if (this.status == Status.outOfService || isRunningOut) return false;
                queue.Enqueue(p);
                return true;
            }
            finally { queueAddRWLock.ExitReadLock(); }
        }
       
        public void DeletePersonFromQueue(Person p)
        {
            queueAddRWLock.EnterWriteLock();
            try {
                lock (queueDeleteLock) {
                    ConcurrentQueue<Person> newQueue = new ConcurrentQueue<Person>();
                    Person q;

                    while (!queue.IsEmpty) {
                        if (queue.TryDequeue(out q) && q != p) newQueue.Enqueue(q);
                    }
                    queue = newQueue;
                }
            }
            finally { queueAddRWLock.ExitWriteLock(); }
        }
        public void DeletePeopleFromQueue(List<Person> l) {
            queueAddRWLock.EnterWriteLock();
            try {
                lock (queueDeleteLock) {
                    ConcurrentQueue<Person> newQueue = new ConcurrentQueue<Person>();
                    Person q;

                    while (!queue.IsEmpty) {
                        if (queue.TryDequeue(out q) && !l.Contains(q)) newQueue.Enqueue(q);
                    }
                    queue = newQueue;
                }
            }
            finally { queueAddRWLock.ExitWriteLock(); }
            foreach (var q in l) {
                q.SetContentment(-20);
                q.status = Person.Status.choosesAmus;
            }
        }
        public void DeleteAllPeopleFromQueue() {
            queueAddRWLock.EnterUpgradeableReadLock();
            try {
                foreach (Person p in queue) {
                    p.SetContentment(-20);
                    p.status = Person.Status.choosesAmus;
                }
                queueAddRWLock.EnterWriteLock();
                lock (queueDeleteLock) {
                    queue = new ConcurrentQueue<Person>();
                }
                queueAddRWLock.ExitWriteLock();
            }
            finally { queueAddRWLock.ExitUpgradeableReadLock(); }
            
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
                this.entrance = new AmusementEnterPath(model, new Coordinates((byte)x,(byte)y),this);
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
                this.exit = new AmusementExitPath(model,new Coordinates((byte)x,(byte)y),this);
                status = Status.waitingForPeople;
                model.mustBeExit = false;
                return true;
            }
            else return false;
        }
        protected abstract bool IsInsideInAmusement(int x, int y);
        public override Point GetRealCoordinates() {
            return new Point (this.coord.x * MainForm.sizeOfSquare + 1, this.coord.y * MainForm.sizeOfSquare + 1);
        }
        /// <summary>
        /// Returns all points on which the amusement lies except the entrance and the exit.
        /// </summary>
        /// <returns></returns>
        public abstract List<Coordinates> GetAllPoints();
        public override void Click(object sender, EventArgs e)
        {
               if (model.demolishOn) // it is called in the same thread as changing demolishOn -> OK
                {
                    if (status == Status.outOfService)
                    {
                        Destruct();
                        model.SetLastClick(null); //must be there due to automaticall set lastClick=enter when enter is destructed
                    }
                    else
                    {  
                        MessageBox.Show(Notices.cannotDemolishAmusement, Labels.warningMessBox, MessageBoxButtons.OK);
                    }
                }
                else
                {
                   model.dirtyClick.Enqueue(this);                   
                }
           // }
        }
        public bool isDemolishedEntrance() {
            return (hasSeparatedEnterExit && entrance == null);
        }
        public bool isDemolishedExit() {
            return (hasSeparatedEnterExit && exit == null);
        }
    }
    [Serializable]
    public abstract class AmusementsFactory : MapObjectsFactory {
        public readonly int entranceFee, capacity, runningTime;
        public readonly bool hasSeparatedEnterExit;
        public Color color;
        public int attractiveness;
        public int workingCost;

        public AmusementsFactory(int prize, int fee, int capacity, int runningTime, string name, bool hasEntranceExit, int workingCost, int attractiveness) : base(prize, name) {
            this.entranceFee = fee;
            this.capacity = capacity;
            this.runningTime = runningTime;
            this.hasSeparatedEnterExit = hasEntranceExit;
            this.color = Color.Yellow;
            this.workingCost = workingCost;
            this.attractiveness = attractiveness;
        }
        public AmusementsFactory(int prize, string name, int workingCost, int attractiveness):base(prize, name){}
        protected static bool CheckFreeLocation(byte x, byte y, Model model, byte width, byte height, bool hasSeparatedEntranceAndExit = true) {
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
        public abstract string GetInfo();
    }

    [Serializable]
    public abstract class Path : MapObjects//, IButtonCreatable 
    {
        public Direction[] signpostAmus;//rozcestnik
        public readonly string name;
        public readonly int typeId;
        public int InternTypeId { get { return typeId; } set { } }
        
        protected Path(Model m, int prize, int typeId, bool tangible = true)
            : base(m, prize, typeId, tangible) {
            this.zIndex = 10;
            signpostAmus = new Direction[m.maxAmusementsCount];
            for (int i = 0; i < signpostAmus.Length; i++) signpostAmus[i] = Direction.no;
        }
        public Path(Model m, Coordinates c, int prize, string name, int typeId)
            : base(m, c, prize, typeId) {
            this.name = name;
            this.typeId = typeId;
            signpostAmus = new Direction[m.maxAmusementsCount];
           for (int i = 0; i < signpostAmus.Length; i++) signpostAmus[i] = Direction.no;
          
            model.maps.AddPath(this);
        }
        public Path() { }
        public override void Click(object sender, EventArgs e) {
            if (model.demolishOn) Destruct();
        }
        public override void Destruct() {
            model.maps.RemovePath(this);
            //model.dirtyDestruct.Enqueue(this);
        }
        public override Size GetRealSize() {
            return new Size(MainForm.sizeOfSquare - 1, MainForm.sizeOfSquare - 1);              
        }
        public override Point GetRealCoordinates() {
            return new Point (this.coord.x * MainForm.sizeOfSquare + 1, this.coord.y * MainForm.sizeOfSquare + 1);
        }
        /// <summary>
        /// Determines whether the given coordinates are inside this object or not.
        /// </summary>
        /// <param name="x">An nonnegative integer, represents the game x-coordinate.</param>
        /// <param name="y">An nonnegative integer, represents the game y-coordinate.</param>
        /// <returns></returns>
        public override bool IsInside(int gx, int gy) {
#warning overit, jestli opravdu funguje
            if (gx == this.coord.x && gy == this.coord.y) return true;
            else return false;
        }
    }
    [Serializable]
    public abstract class PathFactory: MapObjectsFactory{
        public PathFactory(int prize, string name)
        : base(prize, name) {
           
        }
        public override bool CanBeBuild(byte x, byte y, Model model) {
            return true;
        }
    }
    
    [Serializable]
    public class Person : MapObjects,IActionable
    { //todo: Mozna sealed a nebo naopak moznost rozsiritelnosti dal...
        private static Random rand = new Random();
        const int minMoney = 200, maxMoney = 2000, minPatience=10, maxPatience=100;
        public const int width = MainForm.sizeOfSquare / 7, height = MainForm.sizeOfSquare / 2;
        public enum Status {initialWalking, walking, onCrossroad, inAmusQueue, inAmus,choosesAmus, disposing }

        private int money; 
        private readonly int patience;
        public readonly int id;
        public readonly double maxAcceptablePrice;//max price which he is willing to pay per an amusement
        public bool visible { get; set; }
        public readonly System.Drawing.Color color;

        //----provozni hodnoty-----
        private int remainingStepsCount=0;//pocet zbyvajicich kroku
        private int waitingTimeInQueue = 0, startingWalkingTime=2*MainForm.sizeOfSquare;
        private int contentment;//spokojenost
        private int hunger = 0;

        protected int realX, realY; //instead of coord 
        [NonSerialized]
        protected object xyLock = new object(); //use for every manipulation with x and y together
        private Direction currDirection = Direction.no;
        public int CurrAmusId { get; private set; }
        private Amusements currAmus;
        public Status status { set; get; } //protected get; }
        
        public Person(Model m, int x, int y) : base(m, prize: 0, typeID: -1) {

            this.contentment = 100;
            this.zIndex = 1;
            this.id = m.persList.GetFreeId();
            this.status = Status.initialWalking;
            this.money = rand.Next(Person.minMoney,Person.maxMoney);
            this.patience = rand.Next(Person.minPatience, Person.maxPatience);
            this.maxAcceptablePrice = rand.Next(1000,3000) / 1000.0;
            this.realX = x;
            this.realY = y;
            this.visible = true;
            this.color = System.Drawing.Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));

            m.persList.Add(this);

        }

        [OnDeserialized]
        private new void SetValuesAndCheckOnDeserialized(StreamingContext context) {
            base.SetValuesAndCheckOnDeserialized(context);
            xyLock = new object();
        }
        public void Action()
        {
            hunger = Math.Min(hunger+1,2000);
            switch (status)
            {
                case Status.walking:{
                    #region
                    if (remainingStepsCount > 0) {
                        remainingStepsCount--;
                        switch (currDirection)
                        {
                            case Direction.N: Interlocked.Decrement(ref realY);
                                break;
                            case Direction.S: Interlocked.Increment(ref realY);
                                break;
                            case Direction.W: Interlocked.Decrement(ref realX);
                                break;
                            case Direction.E: Interlocked.Increment(ref realX);
                                break;
                            case Direction.no: { status = Status.choosesAmus;}
                                break;
                            case Direction.here: {
                                
                                lock (xyLock) {  // lock is not necessary here, only an amusement changes x and y together and the person is not in an amusement, yet.
                                    currAmus = model.maps.GetAmusement(realX, realY);
                                }
                                if (currAmus == null) {
                                    AddContentment(-10); //todo: ubrat spokojenost
                                    status = Status.choosesAmus; return;
                                }
                                if (currAmus.Id != CurrAmusId) throw new MyDebugException("Person.Action - lisi se ocekavane id");
                                if (currAmus.CurrFee > maxAcceptablePrice * currAmus.originalFee){
                                    AddContentment(-25);//todo: nastavit poradne
                                    status = Status.choosesAmus;
                                }
                                else if (currAmus.CurrFee > money) {                                  
                                    status = Status.choosesAmus;                              
                                }
                                else {
                                    status = Status.inAmusQueue;
                                    waitingTimeInQueue = 0;
                                    if (!currAmus.TryQueuesPerson(this)) {
                                        AddContentment(-10);
                                        status = Status.choosesAmus;
                                    }
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
                    currDirection=model.maps.GetDirectionToAmusement(CurrAmusId,realX,realY);
                    status = Status.walking;
                    remainingStepsCount = MainForm.sizeOfSquare;
                    #endregion
                }                 
                    break;
                case Status.inAmusQueue:{
                    #region
                        if (waitingTimeInQueue > patience) {
                            AddContentment(-10);
                            currAmus.DeletePersonFromQueue(this);
                            status = Status.choosesAmus;
                        }
                        else waitingTimeInQueue++;
                    #endregion
                     }
                    break;
                case Status.inAmus:{
                        AddContentment(2);
                    }
                    break;
                case Status.choosesAmus: {
                    CurrAmusId = ChooseAmusement();
                    status = Status.onCrossroad;
                }
                    break;
                case Status.initialWalking:
                    {
                    #region
                    if (startingWalkingTime > 0)
                    {
                        startingWalkingTime--;
                                                                    
                        switch (currDirection)//doesnt matter that it is not "thread-safe", nobody else couldnt change x,y while initialWalkingTime
                        {
                            case Direction.E:  
                                if (model.maps.IsPath(realX + 1, realY)) { realX++;
                                }
                                else currDirection=(Direction)rand.Next(1,5);
                                break;
                            case Direction.N:
                                if (model.maps.IsPath(realX , realY-1)) { realY--; }
                                else currDirection = (Direction)rand.Next(1, 5);
                                break;
                            case Direction.S: 
                                if (model.maps.IsPath(realX , realY+1)) { realY++; }
                                else currDirection = (Direction)rand.Next(1, 5);
                                break;
                            case Direction.W: 
                               if (model.maps.IsPath(realX -1, realY)) { realX--; }
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
                case Status.disposing:// nothing
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

            if (model.gate.State == Amusements.Status.runningOut) {
                AddContentment(-20);
                return model.amusList.GetGateId(); 
            }
            if(money < model.currCheapestFee || model.gate.State == Amusements.Status.runningOut) return model.amusList.GetGateId();//person cannot afford pay any amusement
            if (contentment == 0) return model.amusList.GetGateId();
            if (hunger > 1800) //2000=100 %, i.e. 2000*0.9
                return model.amusList.GetRandomRestaurant(); //kdyz je hlad > 90%, vybira obcerstveni
           
            int number = rand.Next(101); //0-100
            //---- less contenment increases the probability of leaving the park
            if (number > contentment + 50) return model.amusList.GetGateId(); 
            //---- more hunger -> bigger probability of visiting a restaurant
            if (number < hunger / 20) return model.amusList.GetRandomRestaurant();
            //---- go to an amusement      
            return model.amusList.GetRandomAmusement();
        }

        public void SetRealCoordinates(int x, int y) {
            lock (xyLock) {
                this.realX = x;
                this.realY = y;
            }
        }
       
        public System.Drawing.Point GetRealCoordinatesUnsynchronized() {
            return new System.Drawing.Point(realX,realY);
        }
        public override Point GetRealCoordinates() {
            lock (xyLock) {
                return new System.Drawing.Point(realX, realY);
            }
        }
        /// <summary>
        /// Determines whether the given coordinates are inside this object or not.
        /// </summary>
        /// <param name="x">An nonnegative integer, represents the real x-coordinate.</param>
        /// <param name="y">An nonnegative integer, represents the real y-coordinate.</param>
        /// <returns></returns>
        public override bool IsInside(int x, int y) {
            if (x >= this.realX && x <= this.realX + width && y >= this.realY - height && y <= this.realY) return true;
            else return false;
        }
        public int GetContentment() {
            return contentment;
        }
        public int GetHunger() {
            return hunger / 20;
        }
        public int GetMoney() {
            return this.money;
        }
        public void AddMoney(int addedValue) {
            Interlocked.Add(ref money, addedValue);
        }
        /// <summary>
        /// Decreases hunger.
        /// </summary>
        public void Feed() {
            hunger = 0;
        } 
       /// <summary>
       /// Sets the contenment to param value.
       /// </summary>
       /// <param name="percent">A nonnegtive int that represents count of percent.</param>
        public void SetContentment(int percent){
            if(percent >= 0 && percent <= 100) contentment=percent;
        }
        /// <summary>
        /// Increments the contentment of the desired count of percent (or set 100 % or 0 %).
        /// </summary>
        /// <param name="percentCount">An integer between -100 and 100 which represents the percent.</param>
        public void AddContentment(int percentCount) {
            //It doesnt matter that it is not thread-safe, it leaves the contenment to chance a little bit
            if (percentCount >= -100 && percentCount <= 0) 
                contentment = Math.Max(0, contentment+percentCount);
            else if(percentCount > 0 && percentCount <= 100)
                contentment = Math.Min(contentment+percentCount,100);            
        }
        
        public override void Click(object sender, EventArgs e)
        {           
            model.dirtyClick.Enqueue(this);         
        }


        public override Size GetRealSize() {
            return new Size(MainForm.sizeOfSquare / 7, MainForm.sizeOfSquare / 2);              
        }
        public override void Destruct()
        {
            model.persList.Remove(this);
            //model.dirtyDestruct.Enqueue(this);
            this.status = Status.disposing;
            model.ChangeLongtermContentment(this.contentment);
        }
        public void DestructWithoutListRemove() {
           // model.dirtyDestruct.Enqueue(this);
            this.status = Status.disposing;
            model.ChangeLongtermContentment(this.contentment);
        }


    }
 
}
