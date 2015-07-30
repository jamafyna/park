using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Concurrent;

namespace LunaparkGame
{
    
    /// <summary>
    /// Keeps all items of the current game, is thread-safe.
    /// </summary>
    public class Model //todo: Mozna prejmenovat na evidenci
    {     
        private const int initialMoney = 1;
        public const int maxPeopleInPark=1000;
        public readonly byte playingWidth, playingHeight; 
        public readonly byte realWidth, realHeight;
        public readonly int maxAmusementsCount;//todo: nejspis nepocita lavicky
        private int money;
        public int currCheapestFee { private set; get; }//todo: postarat se o vytvoreni
        /// <summary>
        /// use for manipulation with currCheapestFee
        /// </summary>
        private object feeLock=new object();
        public int CurrPeopleCount { get { return persList.GetTotalPeopleCount(); } }
       //---containers---
        public ConcurrentQueue<MapObjects> dirtyNew=new ConcurrentQueue<MapObjects>();
        public ConcurrentQueue<MapObjects> dirtyDestruct=new ConcurrentQueue<MapObjects>();//mozna misto mapObjects staci byt Control
        public ConcurrentQueue<MapObjects> dirtyClick = new ConcurrentQueue<MapObjects>();

        public List<IUpdatable> updatableItems;// todo: sem dat taky hlavni form, udelat thread-safe, spis vlozit do view      
        
        public AmusementsList amusList;// todo:thread-safe
        public PersonList persList = new PersonList(maxPeopleInPark);//todo: thread-safe
        public Map maps;
        public SpecialEffects effects=new SpecialEffects();
       
        //---running fields for Form1
        public Amusements LastBuiltAmus { get; set; }
        public MapObjects LastClick { private set; get; }
        public bool mustBeEnter = false;
        public bool mustBeExit = false;
        public bool demolishOn = false;
             
        public Model(byte height, byte width){
            this.playingHeight=height;
            this.playingWidth=width;
            this.realHeight = (byte)(height + 2);
            this.realWidth = (byte)(width + 2);
            money = initialMoney;
                        
            maxAmusementsCount = playingHeight * playingWidth + 1; // max. count of amusements that can user build, + 1 due to the gate which does not lie on the playing place
            amusList = new AmusementsList(maxAmusementsCount);
            maps=new Map(width,height,this);

        }
        public void MoneyAdd(int value) {
            Interlocked.Add(ref money,value);
        }
        /// <summary>
        /// Returns value of the current money acount.
        /// </summary>
        /// <returns>Int that represents current money.</returns>
        public int GetMoney() { return this.money; }
        public void CheckCheapestFee(int fee) {
            lock(feeLock){
                if(fee<currCheapestFee) currCheapestFee=fee;          
            }        
        }
        public void SetLastClick(MapObjects lastClick) {
            if (!mustBeEnter && !mustBeExit) this.LastClick = lastClick;
            else MessageBox.Show(Notices.unfinishedBuilding, Labels.warningMessBox, MessageBoxButtons.OK);
        }
    }
   

   /// <summary>
   /// Keeps all Amusement items in the current game, is thread-safe.
   /// </summary>
    public class AmusementsList:IActionable
    { 
        static Random rand = new Random();
        private List<Amusements> list; 
        private ConcurrentQueue<int> freeId;
        private List<int> foodIds;
        /// <summary>
        /// use for list and foodIds
        /// </summary>
        private ReaderWriterLockSlim rwLock=new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly Amusements gate;

        public int AmusementsCount { get { return list.Count; } private set { } } //hash: zeptat se, ale nemel by byt problem, maximalne je neaktualni
        public int RestaurantsCount { get { return foodIds.Count; } private set { } }
     
        public AmusementsList(int maxAmusCount)
        {
            //todo:create brana gate
            list = new List<Amusements>();
            foodIds = new List<int>();
           // freeId = new Queue<int>(maxAmusCount);
            freeId = new ConcurrentQueue<int>();
            for (int i = maxAmusCount; i > 0; i--) freeId.Enqueue(i);  //0 is not free, gate.id==0;          
        }
        public void Add(Amusements a)
        {
            /*if (a.id == list.Count) list.Add(a);
            else throw new MyDebugException("nesedi id a count v AmusementsList-Add()"); //todo: nemelo by se stavat, protoze by vzdy melo jit vytvorit jen jednu atrakci
           */
            try {
                rwLock.EnterWriteLock();
                list.Add(a);
                if (a is Restaurant) foodIds.Add(a.id);
            }
            finally {
                rwLock.ExitWriteLock();
            }
        }
        public int GetFreeID()
        {
            try
            {
                int tempId;
                while (!freeId.TryDequeue(out tempId)) { }//hash:overit, zda funguje
                return tempId;
            }
            catch (InvalidOperationException e) {
                throw new MyDebugException("Pocet atrakci prekrocil maximalni povoleny pocet"+e.ToString());
            }
        }
        public void Remove(Amusements a)
        {
            rwLock.EnterWriteLock();
            try {
                list.Remove(a);
                if (a is Restaurant) foodIds.Remove(a.id);
            }
            finally {
                rwLock.ExitWriteLock();
            }

            freeId.Enqueue(a.id);
        }
        /// <summary>
        /// Returns id of an amusement (except of the gate) if it is possible(at least one another amusement), if not - returns the gate's id
        /// </summary>
        /// <returns>A positive int which represents id of an amusement or 0 which represents id of the gate.</returns>
        public int GetRandomAmusement() {
            rwLock.EnterReadLock(); 
            try {
                if (list.Count > 1) return list[rand.Next(1, list.Count)].id;
                else return 0;
            }
            finally{
                rwLock.ExitReadLock();
            }
        }
        /// <summary>
        /// Returns id of an restaurant if it is possible, if not - returns id of the gate.
        /// </summary>
        /// <returns>A positive int which represents id of an restaurant or 0 if there is no restaurant</returns>
        public int GetRandomRestaurant() {
            rwLock.EnterReadLock(); //zbytecne zamykat, list.count vrati cislo, nejhure neaktualni (aktualita se ale stejne dale neda zajistit)
            try {
                if (foodIds.Count == 0) return 0;
                return foodIds[rand.Next(foodIds.Count)];
            }
            finally {
                rwLock.ExitReadLock();
            }
        }
        public void Action()
        {
            rwLock.EnterReadLock();
            try {
                list.ForEach(a => a.Action());
            }
            finally {
                rwLock.ExitReadLock();
            }
        }
        public Coordinates GetGateCoordinate() {
            return gate.coord;       
        }
        public int GetGateId() {
            return gate.id;
        }
        public Amusements[] GetCopyArray() { 
            rwLock.EnterReadLock();
            try {
                return list.ToArray();
            }
            finally {
                rwLock.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PersonList //todo: is TS, ale rozdelit si pole na casti a zamykat vice zamky (napr. %50)
    {
       // private List<Person> list;
        //Mozna je jednodussi pouzit List misto sloziteho pole, nekolikrat se prochazi - ok, add-ok, smazani projde cele pole (tak 400-1000 prvku)
        //zkusit udelat list2 s Listem
        const int maxUniquePeople = 65536;//2^16  //131072; //2^17
        private int[] internChangablePeopleId=new int[maxUniquePeople];
        private Person[] people;
        private int currPeopleCount;
        private int totalPeopleCount;
        /// <summary>
        /// for manipulating s people array (read, write) and currPeopleCount
        /// </summary>
        private object peopleLock = new object();
        /// <summary>
        /// useful only for a very unlikely situation - a person p survived more people then maxUniquePeople and it is called Remove(p) and Add(q) where p.id=q.id in the same time
        /// </summary>
       // private object unlikelyLock = new object();

        public PersonList(int maxPeopleCount) {
            //for (int = 0; i < people.Length; i++) people[i] = null; inicialni hodnota je uz nastavena           
            currPeopleCount = 0;
            totalPeopleCount = 0;
            people = new Person[maxPeopleCount];
            for (int i = 0; i < internChangablePeopleId.Length; i++) internChangablePeopleId[i] = -1;
        }
        public int GetActualPeopleCount() {
            return currPeopleCount;
        }
        public int GetTotalPeopleCount() {
            return totalPeopleCount;
        }
        public int GetFreeId() {//je TS
            int id = Interlocked.Increment(ref totalPeopleCount);
            return id % maxUniquePeople; 
            //it could happen potentially that this id has an another person, never mind - checking in adding person
        }
       
        public void Add(Person p)//ts
        {

            lock (peopleLock) { //todo: povoluje zamek rekurzivni zamykani??? Zde muze za velmi nepst.situace nastat (pri zavolani Remove)
                if (internChangablePeopleId[p.id] != -1) { // very unlikely situation - a person p survived more people then maxUniquePeople; and must be locked - more it is called Remove(p) and Add(q) where p.id=q.id in the same time

#if (DEBUG)
                    throw new MyDebugException("PersList.Add - Is the total number of visitors really bigger then chosen const?");
#else
                    Remove(p);
                    p.DestructWithoutListRemove(); // only p.Destruct cannot be called, it could remove from this new added person
#endif
                }

                // lock is necessary (adding to the array must be done before or in the same time increment countOfPeople

                //  int internId = Interlocked.Increment(ref currPeopleCount); zbytecne, kdyz mam zamceno
                int internId = currPeopleCount++; // e.d. for sure: + 1 is done after setting
                people[internId] = p;
                internChangablePeopleId[p.id] = internId;
            }          
        }

        public void Action()
        {           
            //todo: casem idealne ve vice vlaknech (experimentalne overit, zda je zapotrebi)
            try {
                lock (peopleLock) {
                    for (int i = 0; i < currPeopleCount; i++){
                        people[i].Action();
                    }
                }
            }
            catch (NullReferenceException e) {
                throw new MyDebugException("PersonList.Action - people[i]==null0"+e.ToString());
            }
            
        }
        /// <summary>
        /// a method which should be called only from the class Person
        /// </summary>
        /// <param name="p">the person which will be removed</param>
        public void Remove(Person p)//todo: thread-safe"!!! pri praci s polem se do nej nesmi nic pridavat
        {
            lock (peopleLock) {
                int internId = internChangablePeopleId[p.id];
                internChangablePeopleId[p.id] = -1;

#if (DEBUG)
                if (internId == -1) throw new MyDebugException("PeopleList.Remove - id=-1, tj. p nebyla nejspis v seznamu");
                if (people[internId] != p) throw new MyDebugException("PeopleList.Remove - person[id]!=p: p.id: " + p.id.ToString() + ", p: " + p.ToString());

#else            
            if (internId == -1) return;            
#endif

                if (internId == currPeopleCount - 1) { //p is the last item
                    people[internId] = null;//due to GC
                    currPeopleCount--;
                }
                else {
                    Person q;
                    q = people[currPeopleCount - 1];//last item
                    people[internId] = q;
                    internChangablePeopleId[q.id] = internId;
                    people[currPeopleCount - 1] = null;//due to GC
                    currPeopleCount--;
                }
            }
           
            

          
        }
        
        

    }
     
    public class Map: IActionable 
    {
#warning bude potreba udelat TS - urcite pro vylouceni remove a ostatnich veci, kde se porovnava !=null, RWLockSlim, mozna zvolit vice na casti pole, zamyslet se, jak casto se bude menit (asi spis ne, uzivatel bori malo)
       
        class DirectionItem {
            public Direction dir;
         //   public readonly Coordinates c;
            public readonly byte x;
            public readonly byte y;
            public DirectionItem(byte x, byte y) {
                this.x = x;
                this.y = y;
                dir = Direction.no;
            }
        /*    public DirectionItem(Coordinates c){
                this.c = c;
                dir = Direction.no; 
            }  */     
        }

        private readonly DirectionItem[][] auxPathMap; // null = there isnt path     
        private readonly Amusements[][] amusementMap;
        private readonly Path[][] pathMap;
        private readonly Model model;
        /// <summary>
        /// for setting direction to a new amusement, dequeue can be called only in Action()
        /// </summary>
        private ConcurrentQueue<Amusements> lastAddedAmus=new ConcurrentQueue<Amusements>();
        private volatile bool pathChanged = false;
        /// <summary>
        /// use for adding to queue (due to .Clear) and for "clear"
        /// </summary>
        private object lastAddedAmusLock = new object();
        //todo: misto Lock pouzit aktivni zamek, ale jak? private SpinLock lastAddedAmusSLock = new SpinLock();
        private ReaderWriterLockSlim pathRWLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private ReaderWriterLockSlim amusRWLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

       
       // private int amusDeletedId = -1;
        public readonly byte realWidthMap;
        public readonly byte realHeightMap;
        private int maxAmusCount; //todo: nejspis neni potreba, smazat
        
        /// <summary>
        /// Represents current playing map, includes maps of paths and amusements, provides algorithms for navigation.
        /// </summary>
        /// <param name="width">!!!NAPSAT, JESTLI JDE O REALNOU NEBO HRACI</param>
        /// <param name="height"></param>
        /// <param name="m"></param>
        public Map(byte width, byte height, Model m) {
            this.realWidthMap = width;
            this.realHeightMap = height;
            this.model = m;
            this.maxAmusCount = model.maxAmusementsCount;
             
            //---initialize helpPath
            auxPathMap=new DirectionItem[width][];
            for (int i = 0; i < width; i++) {
                auxPathMap[i] = new DirectionItem[height];
            }
         
            //---initialize pathMap
            pathMap = new Path[width][];
            for (int i = 0; i < width; i++) pathMap[i] = new Path[height];

            //---initialize amusementMap
            amusementMap=new Amusements[width][];
            for (int i = 0; i < width; i++) amusementMap[i] = new Amusements[height];
        }
#warning isFree metody jsou 2
        public bool isFree(byte x, byte y) {
#if (DEBUG)
            if(x==0 || x==realWidthMap-1||y==0||y==realHeightMap-1) throw new MyDebugException("Map.isFree - bad coordination between maps");
#endif
            amusRWLock.EnterReadLock();
            pathRWLock.EnterReadLock();
            try {
                if (amusementMap[x][y] == null && pathMap[x][y] == null) return true;
                else return false;
            }
            finally {
                amusRWLock.ExitReadLock();
                pathRWLock.ExitReadLock();
            }
        }
        public bool isFree(Coordinates c){
#if (DEBUG)
            if (c.x == 0 ||c.x == realWidthMap - 1 || c.y == 0 || c.y == realHeightMap - 1) throw new MyDebugException("Map.isFree - bad coordination between maps");
#endif
            amusRWLock.EnterReadLock();
            pathRWLock.EnterReadLock();
            try {
                if (amusementMap[c.x][c.y] == null && pathMap[c.x][c.y] == null) return true;
                else return false;
            }
            finally {
                amusRWLock.ExitReadLock();
                pathRWLock.ExitReadLock();
            }
        }
       
        public void RemoveAmus(Amusements a) {
            amusRWLock.EnterWriteLock();
            try {
                foreach (var c in a.GetAllPoints()) amusementMap[c.x][c.y] = null;
                if (a.entrance != null) amusementMap[a.entrance.coord.x][a.entrance.coord.y] = null;
                if (a.exit != null) amusementMap[a.entrance.coord.x][a.entrance.coord.y] = null;
                //todo:vyse uvedene by nemelo byt null, entrance a exit musi vzdy existovat, byt to je stejne policko jako atrakce
                //don't set null to pathMap - it made entrance.Destruct()
            }
            finally {
                amusRWLock.ExitReadLock();
            }
        }
        public void AddAmus(Amusements a) {
            amusRWLock.EnterWriteLock();
            pathRWLock.EnterWriteLock();
            try {
                foreach (var c in a.GetAllPoints()) amusementMap[c.x][c.y] = a;
                try {
                    amusementMap[a.entrance.coord.x][a.entrance.coord.y] = a;
                    amusementMap[a.exit.coord.x][a.exit.coord.y] = a;
                    pathMap[a.entrance.coord.x][a.entrance.coord.y] = a.entrance;
                    pathMap[a.exit.coord.x][a.exit.coord.y] = a.exit;
                }
                catch (NullReferenceException e) {//todo: PO OVERENI ODSTRANIT
                    throw new MyDebugException("Entry or exit is null in AddAmus" + e.ToString());
                }
            }
            finally {
                amusRWLock.ExitWriteLock();
                pathRWLock.ExitWriteLock();
            }

            lock (lastAddedAmusLock) {
                lastAddedAmus.Enqueue(a);
            }
            //todo:zamek s aktivnim cekanim - co dela nize uvedene??? Vypada to, ze vyhodi vyjimku - to je opravdu rychlejsi nez odplanovani vlakna???
            /*bool lockTaken = false;
            try {
                lastAddedAmusSLock.Enter(ref lockTaken);
                lastAddedAmus.Enqueue(a);
            }
            finally {
                if (lockTaken) lastAddedAmusSLock.Exit(false);
            } 
            */
            
           
        }
        public void DeleteDirectionToAmusement(int amusId)//TODO: nejspis nepotrebna metoda
        {
            Path p;
            pathRWLock.EnterReadLock();// can be in first forcycle but there is probably better
            try {
                for (int i = 0; i < realWidthMap; i++)
                    for (int j = 0; j < realHeightMap; j++) {
                        if ((p = pathMap[i][j]) != null) p.signpostAmus[amusId] = Direction.no;
                    }
            }
            finally {
                pathRWLock.ExitReadLock();
            }
        }

        public void AddPath(Path p)
        { 
            pathRWLock.EnterWriteLock();
            try {
                pathMap[p.coord.x][p.coord.y] = p;
                
            }
            finally {
                pathRWLock.ExitWriteLock();
            }
            pathChanged = true; //important that it is set after changing pathMap
        }
        public void RemovePath(Path p) {
            pathRWLock.EnterWriteLock();
            try {
                pathMap[p.coord.x][p.coord.y] = null;             
            }                
            finally { pathRWLock.ExitWriteLock(); }
            pathChanged = true; //important that it is set after changing pathMap
        }
        /// <summary>
        /// Set to paths in pathMap calculated directions to the amusement.
        /// </summary>
        /// <param name="a">an amusement to which directions are updated</param>
        /// <param name="queue">an empty instance of Queue</param>
        /// <param name="paths">a new auxilary array of builded paths</param>
        private void UpdateDirectionToAmusement(Amusements a,Queue<DirectionItem> queue, DirectionItem[][] paths) {
            DirectionItem item;
            if (!InitializeQueue(queue, a.entrance.coord, paths)) return;
            while (queue.Count != 0)
            {
                item = queue.Dequeue();
                ProcessQueueItem(item.x,item.y, queue, paths);
            }
            // ---set calculated directions
            pathRWLock.EnterReadLock();
            try {
                for (int i = 0; i < realWidthMap; i++) {
                    for (int j = 0; j < realHeightMap; i++) {
                        if ((item = paths[i][j]) != null && pathMap[i][j] != null) {
                            pathMap[i][j].signpostAmus[a.id] = item.dir;
                        }
                    }
                }
            }
            finally {
                pathRWLock.ExitReadLock();
            }
            
        
        }
       /// <summary>
       /// Set to paths in pathMap calculated directions to the amusement. It should be used only for a particular update (for updating all amusements use UpdateDirectionToAmusement()).
       /// </summary>
       /// <param name="a">An Amusements item</param>
        private void UpdateDirectionToOnlyOneAmusement(Amusements a) {
            Queue<DirectionItem> queue = new Queue<DirectionItem>(realWidthMap*realHeightMap+5);
            for (int i = 0; i < realWidthMap; i++) {
                for (int j = 0; j < realHeightMap; j++) {
                    // here isnt a Lock, auxPathMap is not always actual -> lock here=nonsence
                    if (pathMap[i][j] == null) auxPathMap[i][j] = null;
                    else auxPathMap[i][j] = new DirectionItem((byte)i, (byte)j);
                }
            }
            UpdateDirectionToAmusement(a,queue,auxPathMap);      
        }
       /// <summary>
       /// Updates directions to all amusements. Useful when paths changed.
       /// </summary>
        private void UpdateDirections()
        {
            Amusements[] amusA = model.amusList.GetCopyArray();
            for (int i = 0; i < realWidthMap; i++) {
                for (int j = 0; j < realHeightMap; j++) {
                    // here isnt a Lock, auxPathMap is not always actual -> lock here=nonsence
                    if (pathMap[i][j] == null) auxPathMap[i][j] = null;
                    else auxPathMap[i][j] = new DirectionItem((byte)i, (byte)j);
                }
            }
            Queue<DirectionItem> queue = new Queue<DirectionItem>();
            foreach (var a in amusA)
            {
                UpdateDirectionToAmusement(a,queue,auxPathMap);
                queue.Clear();
                for (int i = 0; i < realWidthMap; i++)
                    for (int j = 0; j < realHeightMap; j++) {
                        if (auxPathMap[i][j] != null) auxPathMap[i][j].dir = Direction.no;
                       //todo:rozhodnout, kterou variantu pouzit
                        /* if (pathMap[i][j] != null) auxPathMap[i][j] = new DirectionItem((byte)i, (byte)j);
                        else auxPathMap[i][j] = null;*/
                    }  
            }
        }
        
        /// <summary>
        /// Set start and get its neighbours to the queue.
        /// </summary>
        /// <param name="queue">An empty queue which has to be initialized.</param>
        /// <param name="start">Coordinates of an entrance of an amusement entrance.</param>
        /// <param name="paths">An auxilary array which represents "copy" of pathMap</param>
        /// <returns>true if the initialization was done successful, otherwise false.</returns>
        private bool InitializeQueue(Queue<DirectionItem> queue, Coordinates start, DirectionItem[][] paths) {
            // dont use pathRWLock (or change it to recursive mode)
            if (paths[start.x][start.y] == null) {
#if(DEBUG)
                throw new MyDebugException("Map.InitializeQueue");
#endif
                return false;
            }
            paths[start.x][start.y].dir = Direction.here; 
            DirectionItem aux;
            if ((start.x - 1 >= 0) && (aux=paths[start.x - 1][start.y]) != null) {             
                aux.dir= Direction.E;
                queue.Enqueue(aux);
            }
            if ((aux=paths[start.x][start.y-1]) != null){
                aux.dir = Direction.S;
                queue.Enqueue(aux);
            }
            if ((aux = paths[start.x][start.y + 1]) != null){
                aux.dir = Direction.N;
                queue.Enqueue(aux);
            }
            if ((aux = paths[start.x+1][start.y]) != null){
                aux.dir = Direction.W;
                queue.Enqueue(aux);
            }
            return true;
        
        }
        /// <summary>
        /// analyse item's neighbours, eventually enque them and set them right direction
        /// </summary>
        /// <param name="item">coordinates of the queue item which is analysed</param>
        /// <param name="queue">queue which realise BFS</param>
        /// <param name="paths">An auxilary array which represents a "copy" of pathMap </param>
        private void ProcessQueueItem(byte x, byte y, Queue<DirectionItem> queue, DirectionItem[][] paths)
        {
            // dont use pathRWLock (or change it to recursive mode)!!
            // border fields (except the gate) don't matter because there is a fence (i.e. null) around the playing map
            // direction is set the other way around because it is correct from a person perspective.
           //todo: pokud nechci, aby mi lidi chodili pres vstupy/vystupy atrakci, nastavit zde a v InitializeQueue, ale DirectionItem by v sobe musela jeste uchovavat typ chodniku (nebo bool isIO)
            DirectionItem aux;
            if ((x - 1 >= 0) && (aux = paths[x - 1][y]) != null && aux.dir == Direction.no) // item.x - 1 >= 0 due to the gate
            {
                aux.dir = Direction.E;
                queue.Enqueue(aux);
            }
            if ((aux = paths[x][y - 1]) != null && aux.dir == Direction.no)
            {
                aux.dir = Direction.S;
                queue.Enqueue(aux);
            }
            if ((aux = paths[x][y + 1]) != null && aux.dir == Direction.no)
            {
                aux.dir = Direction.N;
                queue.Enqueue(aux);
            }
            if ((aux = paths[x + 1][y]) != null && aux.dir == Direction.no)
            {
                aux.dir = Direction.W;
                queue.Enqueue(aux);
            }       
        }   
#warning Odsud nize je TS
    /// <summary>
    /// Updates the navigation, i.e. sets current directions to paths. Only one thread can call this method.
    /// </summary>
        public void Action() {

            if (pathChanged) { // OK, false can be set only here
                // an another thread can potentially set pathChanged=true at this moment, it doesnt matter - an item pathChanged is set after the actual execution changed in the array              
                pathChanged = false;
                lock (lastAddedAmus) {
                    lastAddedAmus = new ConcurrentQueue<Amusements>();
                }
                UpdateDirections(); }
            else
            {
                Amusements a;
                while(!lastAddedAmus.IsEmpty){ //is T-S, dequeu can be done only in this method
                    if (lastAddedAmus.TryDequeue(out a)) UpdateDirectionToOnlyOneAmusement(a);
                }
             }
        }
        /// <summary>
        /// Returns a current direction (only over Paths) to the amusement with id==amusId.
        /// </summary>
        /// <param name="amusId">id of the amusements</param>
        /// <param name="x">the real! x-coordinate</param>
        /// <param name="y">the real! y-coordinate</param>
        /// <returns>A Direction item</returns>
        public Direction GetDirectionToAmusement(int amusId, int x, int y) {
            try {
                return pathMap[x / MainForm.sizeOfSquare][y / MainForm.sizeOfSquare].signpostAmus[amusId];
            }
            catch { 
#if(DEBUG) 
                throw new MyDebugException("Map.GetDirectionToAmusement - there is no path");                
#else
                return Direction.no;
#endif
            }
        }
        /// <summary>
        /// Returns the amusement which lies on the specified coordinates, the coordinates can be also a coordinates of entrance or exit.
        /// </summary>
        /// <param name="x">the real! x-coordinate</param>
        /// <param name="y">the real! y-coordinate</param>
        /// <returns>An Amusements item which stretch to [x,y] or null.</returns>
        public Amusements GetAmusement(int x, int y) { 
            return amusementMap[x / MainForm.sizeOfSquare][y / MainForm.sizeOfSquare];          
        }
        /// <summary>
        /// Determines whether a path is on the point [x,y].
        /// </summary>
        /// <param name="x">the real! x-coordinate</param>
        /// <param name="y">the real! y-coordinate</param>
        /// <returns> true if a path is on the coordinates[x,y]; otherwise, false.</returns>
        public bool IsPath(int x, int y) {
            if (pathMap[x / MainForm.sizeOfSquare][y / MainForm.sizeOfSquare] == null) return false;
            else return true;
        }
    
    }
    
}
