using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Concurrent;


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
        private Control control;
        public Control Control
        {
            get { return control; }
            set
            {              
                control = value;
                control.Click += new EventHandler(Click); //hash: overit, ze se opravdu nastavi
        } } 
        public bool isClicked = false;
        virtual public int price { get; protected set; } //todo: This must be abstract!!! - az budu znat ceny
        public MapObjects() { }
        protected MapObjects(Model m) {
            this.model = m;
#warning toto nejspis nefunguje, protoze price bude vzdy 0, nastavuje se az po volani konstruktoru.        
            model.MoneyAdd(-this.price);
            model.dirtyNew.Enqueue(this);
            Control.Click += new EventHandler(Click);

        }
        public MapObjects(Model m,Coordinates coord):this(m)
        {
            this.coord = coord;
        }
       
        /// <summary>
        /// user action
        /// </summary>
        protected abstract void Click(object sender, EventArgs e);
        /// <summary>
        /// user action
        /// </summary>
        public virtual void Destruct() {
            model.dirtyDestruct.Enqueue(this);
        }
        


    }


    /// <summary>
    /// ts,pokud se pouzivaji fce Destruct/Click...z hlavniho vlakna
    /// </summary>
    public abstract class Amusements : MapObjects,IActionable
    {
        #region
        public enum Status { waitingForPeople, running, outOfService, runningOut }
       
        public int id { get; private set; }
        public AmusementEnterPath entrance { get; protected set; } //todo: je opravdu potreba protected, nestaci private nebo dokonce readonly?
        public AmusementExitPath exit { get; protected set; }
        protected  ConcurrentQueue<Person> queue=new ConcurrentQueue<Person>();
        protected object queueDeleteLock = new object();
        protected ReaderWriterLockSlim queueAddRWLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        /// <summary>
        /// Contains people who are in the amusement now.
        /// </summary>
        protected List<Person> peopleInList;
        private Status status=Status.outOfService;
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
        protected int currFee;//todo: odnekud zpocatku nacist
        //public abstract int originalVisitPrice{get;protected set;}//todo:patri jinam
        protected bool isRunningOut=false;
        protected bool isRunning = false;
        //-------popisove vlastnosti
        public readonly int capacity;
        protected readonly int maxWaitingTime, fixedRunningTime;
        
        public int WorkingPrice { get; protected set; }//todo: mozna nebude treba a pevne se vzdy urci procenta z provozu nebo tak nejak
        //protected readonly int initialVisitPrice; nebude potreba, ziska se odnekud
        protected int refundCoef;//todo: maybe zvolit pevna procenta vzdy stejna - napr. Amusement mit static polozku ebo tak nejak
        //----------------------------
        #endregion

         public Amusements( Model m, Coordinates c) :base(m,c)       
        {
            model.LastBuiltAmus = this;                 
            model.mustBeEnter = true;
            this.id = model.amusList.GetFreeID();
           
            peopleInList = new List<Person>(capacity);
            
            this.coord = coord;
            model.amusList.Add(this);
            model.maps.AddAmus(this);         
                       
           // mimoProvoz = true;
           // zacatek = true;
         
        }

        public int GetEntranceFee() {
            return currFee;
        }
        public void SetEntranceFee(int value){
            if (value > 0) currFee = value;
            //todo: bylo if(value>0)Interlocked.Exchange(ref currFee, value) - proc Interlocked? overit
        }
        /// <summary>
        /// Demolish the amusement, remove it from the maps and the amusList, refund money.
        /// </summary>
        public override void Destruct()
        {
            //todo: smi je volat pouze 1x, tj.pokud by nekdy v budoucnu se mela volat i z jineho duvodu, nez user akce, nutno pridat omezeni - napr.nejaky unikatni zamek a pokud je zamcen, return
#warning entrance a exit by mely byt nejspis nejak zabezpeceny
            if(entrance != null) entrance.Destruct();
            if(exit !=null) exit.Destruct();
            model.MoneyAdd(refundCoef * price);//refund money
            model.amusList.Remove(this);
            model.maps.RemoveAmus(this);
            model.dirtyDestruct.Enqueue(this);
            
        } 
        /// <summary>
        /// Puts n waiting people from queue to the amusment. It is assumed that nobody can dequeu while running this method (queue can only growth)!
        /// </summary>
        /// <param name="n">count of items to be relocated, assumed that n isn't bigger than queue.Count anywhere</param>
        protected void PickUpPeople(int n) { 
            Person p;           
            for (int i = 1; i <= n; i++)
            {
                if (!queue.TryDequeue(out p)) throw new MyDebugException("Amusements.PickUpPeople: TryDequeu neuspelo, i kdyz by melo" ); 
                try //melo by jit jen o zbytecne ujisteni
                {
                    p.visible = false;
                    p.status = Person.Status.inAmus;
                    peopleInList.Add(p);
                    p.AddMoney (- this.currFee);                    
                }
                catch (NullReferenceException e) {
                    throw new MyDebugException("Not expected null in pickUpPeople: " + e.ToString());
                }
                catch (ArgumentNullException e) {
                    throw new MyDebugException("Not expected null in pickUpPeople: " + e.ToString());
                }
            }
            model.MoneyAdd(n * this.currFee);        
        }
        protected void DropPeopleOff(){
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
            model.MoneyAdd(-this.WorkingPrice);         
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
            model.MoneyAdd( - this.WorkingPrice);
        }
        protected virtual void RunningOutAction()
        {//todo: neni lepsi, aby se lide cekajici ve fronte museli vratit? tj. nikdo dalsi se nesveze, snizi se spokojenost, ale asi neni proveditelne, protoze to pak lide stale mohou vybirat tuto atrakci
            model.MoneyAdd(-WorkingPrice);
            queueAddRWLock.EnterWriteLock();
            try {
                if (queue.IsEmpty && (!isRunning)) // !isRunning - aby se nezmenilo, kdyz uzivatel klikne uprostred behu
                 {
                    status = Status.outOfService;
                    isRunningOut = false;
                }
                else {
                    isRunningOut = true;
                    if (isRunning) status = Status.running;
                    else status = Status.waitingForPeople;
                }
            }
            finally{queueAddRWLock.ExitWriteLock();}
                  
        }
        /// <summary>
        /// Simmulate an activity of the amusement. 
        /// </summary>
        public virtual void Action() {
            // Cannot use anything from AmusementsList (it could create an cycle)!
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
            queueAddRWLock.EnterReadLock();
            try {
                queue.Enqueue(p);
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
            }
        }
        public abstract bool CheckFreeLocation(byte x,byte y);  
        protected bool CheckFreeLocation(byte x,byte y, byte width, byte height,bool hasSeparatedEntranceAndExit=true) { 
            if (x + width > model.playingWidth + 1 || y + height > model.playingHeight + 1) return false;
            for (byte i = x; i < x + width; i++)
            {
                for (byte j = y; j < y + height; j++)
                {
                    if (!model.maps.isFree(i,j)) return false;
                }
            }
#warning overit, ze to opravdu overuje spravne
            if (hasSeparatedEntranceAndExit) {
                bool free = false;
                if (x - 1 > 0) for (byte i = y; i < y + height; i++)
                        if (model.maps.isFree((byte)(x-1),i)) { if (free) return free; else free = true; }
                if (x < model.playingWidth) for (byte i = y; i < y + height; i++)
                        if (model.maps.isFree((byte)(x + 1), i)) { if (free) return free; else free = true; }
                if (y - 1 > 0) for (byte i = x; i < x + width; i++)
                        if (model.maps.isFree(i,(byte)(y-1))) { if (free) return free; else free = true; }
                if (y < model.playingWidth) for (byte i = y; i < y + height; i++)
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
        
        /// <summary>
        /// Returns all points on which the amusement lies except the entrance and the exit.
        /// </summary>
        /// <returns></returns>
        public abstract List<Coordinates> GetAllPoints();
        protected override void Click(object sender, EventArgs e)
        {
           // if (!zacatek)
          //  { 
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

    }
    public abstract class SquareAmusements : Amusements
    {
        public abstract byte width { get; protected set; }
        public SquareAmusements(Model m, Coordinates c) : base(m,c) {
            model.CheckCheapestFee(this.currFee);
        }
        
        public override bool CheckFreeLocation(byte x, byte y)
        {          
            return CheckFreeLocation(x, y, width, width, hasSeparatedEntranceAndExit: true);
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
            for (byte i = coord.x; i < coord.x + width; i++) { 
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
        public readonly byte sizeA;
        public readonly byte sizeB;
        public readonly bool isHorizontalOriented;


        public RectangleAmusements(Model m,Coordinates c,bool isHorizontal=true):base(m,c)
        {
            isHorizontalOriented = isHorizontal;
            model.CheckCheapestFee(this.currFee);
        }
       
        public RectangleAmusements(Model m, Coordinates c, byte sizeA, byte sizeB, bool isHorizontal = true) : this(m, c, isHorizontal) {
            this.sizeA = sizeA;
            this.sizeB = sizeB;
        }
        

        public override bool CheckFreeLocation(byte x, byte y)
        {
            if (isHorizontalOriented) return CheckFreeLocation(x, y, sizeA, sizeB, hasSeparatedEntranceAndExit: true);
            else return CheckFreeLocation(x, y, sizeB, sizeA, hasSeparatedEntranceAndExit: true);
        }
        protected override bool IsInsideInAmusement(int x, int y)
        {
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
        public override List<Coordinates> GetAllPoints()
        {
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

    public abstract class FreeShapedAmusements : Amusements
    {
        public FreeShapedAmusements(Model m, Coordinates c) : base(m,c) {
            model.CheckCheapestFee(this.currFee);
        }
       //nejspis v sobe jeste jednu vnorenou tridu reprezentujici kousky atrakce
    }
    /// <summary>
    /// napr. pro lavicky
    /// </summary>
    public abstract class LittleComplementaryAmusements : Amusements {
        public LittleComplementaryAmusements(Model m, Coordinates c) : base(m, c) { }
    }
#warning po sem je Thread safe
  
    public class Person : MapObjects,IActionable
    { //todo: Mozna sealed a nebo naopak moznost rozsiritelnosti dal...
        private static Random rand = new Random();
        const int minMoney = 200, maxMoney = 2000, minPatience=1, maxPatience=10;
        public enum Status {initialWalking, walking, onCrossroad, inAmusQueue, inAmus,choosesAmus, end }

        private int money; 
        private readonly int patience;
        public readonly int id;
        public readonly int maxAcceptablePrice;//max price which he is willing to pay per an amusement
        public bool visible { get; set; }
        //----provozni hodnoty-----
        private int remainingStepsCount=0;//pocet zbyvajicich kroku
        private int waitingTimeInQueue = 0, startingWalkingTime=2*MainForm.sizeOfSquare;
        private int contentment = 0;//spokojenost
        private int hunger=0;
        protected int x, y; //instead of coord //todo: casem s tim neco udelat, napr. 2 abstract tridy od MapObjects apod.
        protected object xyLock = new object(); //use for every manipulation with x and y together
        private Direction currDirection = Direction.no;
        private int currAmusId;
        private Amusements currAmus;
        public Status status { set; protected get; }
        
        public Person(Model m, byte x, byte y) : base(m) {
            
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
            hunger = Math.Min(hunger+1,2000);
            switch (status)
            {
                case Status.walking:{
                    #region
                    if (remainingStepsCount > 0) {
                        remainingStepsCount++;
                        switch (currDirection)
                        {
                            case Direction.N: Interlocked.Decrement(ref y);
                                break;
                            case Direction.S: Interlocked.Increment(ref y);
                                break;
                            case Direction.W: Interlocked.Decrement(ref x);
                                break;
                            case Direction.E: Interlocked.Increment(ref x);
                                break;
                            case Direction.no: { status = Status.choosesAmus;}
                                break;
                            case Direction.here: {
                                
                                lock (xyLock) {  // lock is not necessary here, only an amusement changes x and y together and the person is not in an amusement, yet.
                                    currAmus = model.maps.GetAmusement(x, y);
                                }
                                if (currAmus == null) {
                                    AddContentment(-20); //todo: ubrat spokojenost
                                    status = Status.choosesAmus;
                                }
                                if (currAmus.id != currAmusId) throw new MyDebugException("Person.Action - lisi se ocekavane id");
                                if (currAmus.GetEntranceFee() > maxAcceptablePrice){
                                    AddContentment(-40);//todo: nastavit poradne
                                    status = Status.choosesAmus;
                                }
                                else if (currAmus.GetEntranceFee() > money) {
                                    AddContentment(-10);//todo: nastavit poradne
                                    status = Status.choosesAmus;
                                
                                }
                                else {
                                    status = Status.inAmusQueue;
                                    waitingTimeInQueue = 0;
                                    currAmus.QueuesPerson(this);
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
                            AddContentment(-10);//todo: mozna udelat: odejde, pokud prekroci patience a vzdy jindy se drobne snizi spokojenost
                            currAmus.DeletePersonFromQueue(this);
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
                    currAmusId = ChooseAmusement();
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
                                if (model.maps.IsPath(x + 1, y)) { x++;
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
            if(money<model.currCheapestFee) return model.amusList.GetGateId();//person cannot afford pay any amusement
            if (contentment == 0) return model.amusList.GetGateId();
            if (hunger > 1800) //2000=100 %, i.e. 2000*0.9
                return model.amusList.GetRandomRestaurant(); //kdyz je hlad > 90%, vybira obcerstveni
           
            int number = rand.Next(101); //0-100
            //---- less contenment increases the probability of leaving the park
            if (number > contentment) return model.amusList.GetGateId(); 
            //---- more hunger -> bigger probability of visiting a restaurant
            if (number < hunger / 20) return model.amusList.GetRandomRestaurant();
            //---- go to an amusement
            return model.amusList.GetRandomAmusement();
        }
        public void SetRealCoordinates(int x, int y) {
            lock (xyLock) {
                this.x = x;
                this.y = y;
            }
        }
        public System.Drawing.Point GetRealCoordinates() {
            lock (xyLock) {
                return new System.Drawing.Point(x, y);
            }
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
        
        protected override void Click(object sender, EventArgs e)
        {
           /* if (!isClicked)
            {*/
                model.dirtyClick.Enqueue(this);
            /*    isClicked = true;
            }*/
        }
       

       
        public override void Destruct()
        {
            model.persList.Remove(this);
            model.dirtyDestruct.Enqueue(this);
        }
        public void DestructWithoutListRemove() {
            model.dirtyDestruct.Enqueue(this);
        }


    }
 
    public abstract class Path : MapObjects
    {
        public Direction[] signpostAmus;//rozcestnik
        protected Path(Model m):base(m){
            signpostAmus = new Direction[m.maxAmusementsCount];
            //todo: mozna neni treba, overit
            for (int i = 0; i < signpostAmus.Length; i++) signpostAmus[i] = Direction.no;
        }
        public Path(Model m, Coordinates c) : base(m,c) {
            signpostAmus = new Direction[m.maxAmusementsCount];
            //todo: mozna neni treba, overit
            for (int i = 0; i < signpostAmus.Length; i++) signpostAmus[i] = Direction.no;
            model.maps.AddPath(this);
        }
        public Path() { }
        protected override void Click(object sender, EventArgs e)
        {
            if (model.demolishOn) Destruct();
        }
        public override void Destruct()
        {
            model.maps.RemovePath(this);
            model.dirtyDestruct.Enqueue(this);
        }
       
    }
    public abstract class AmusementPath : Path {
        public readonly Amusements amusement;
        public AmusementPath(Model m, Coordinates c, Amusements a)
            : base(m) //not call base(m,c) because dont want to add to maps
        {
            this.coord = coord;
            model.maps.AddEntranceExit(this);
        }
    
    }
    public class AmusementEnterPath : AmusementPath {

        public override int price {
            get {
                return 0;
            }
            protected set {
                base.price = 0;
            }
        }
        public AmusementEnterPath(Model m, Coordinates c, Amusements a) : base(m, c, a)  {
        }
        public override void Destruct() {
            model.maps.RemoveEntranceExit(this);
            model.dirtyDestruct.Enqueue(this);
            model.LastBuiltAmus = this.amusement;
            model.mustBeEnter = true;
        }

    }
    public class AmusementExitPath : AmusementPath {
        public AmusementExitPath (Model m, Coordinates c, Amusements a) : base(m, c, a)  {  }
        public override void Destruct() {           
            model.maps.RemoveEntranceExit(this);
            model.dirtyDestruct.Enqueue(this);
            model.LastBuiltAmus = this.amusement;
            if(!model.mustBeEnter) model.mustBeExit = true;
        
        }

    }
    
}
