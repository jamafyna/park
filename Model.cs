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
   

    public class AmusementsList:IActionable
    { 
        static Random rand = new Random();
        private List<Amusements> list; 
        private ConcurrentQueue<int> freeId;
        private List<int> foodIds;
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
        //private bool[][] isFree;
        class DirectionItem {
            public Direction dir;
            public readonly Coordinates c;
            public DirectionItem(Coordinates c){
                this.c = c;
                dir = Direction.no; 
            }       
        }

        private DirectionItem[][] auxPathMap; // null = there isnt path
      
        private Amusements[][] amusementMap;
        private Path[][] pathMap;
        //todo: neni nyni auxPathMap uplne zbytecna???, nestacilo by misto ni pouzivat pathMap? ano stacilo
        private Model model;
        /// <summary>
        /// for setting direction to a new amusement, dequeue can be called only in Action()
        /// </summary>
        private ConcurrentQueue<Amusements> lastAddedAmus=new ConcurrentQueue<Amusements>();

        private bool pathChanged = false;
       // private int amusDeletedId = -1;
        public readonly byte widthMap;
        public readonly byte heightMap;
        private int maxAmusCount;
        
        /// <summary>
        /// Represents current playing map, includes maps of paths and amusements, provides algorithms for navigation.
        /// </summary>
        /// <param name="width">!!!NAPSAT, JESTLI JDE O REALNOU NEBO HRACI</param>
        /// <param name="height"></param>
        /// <param name="m"></param>
        public Map(byte width, byte height, Model m) {
            this.widthMap = width;
            this.heightMap = height;
            this.model = m;
            this.maxAmusCount = model.maxAmusementsCount;
             
            //---initialize helpPath
            auxPathMap=new DirectionItem[width][];
            for (int i = 0; i < width; i++)  auxPathMap[i] = new DirectionItem[height];
         
            //---initialize pathMap
            pathMap = new Path[width][];
            for (int i = 0; i < width; i++) pathMap[i] = new Path[height];

            //---initialize amusementMap
            amusementMap=new Amusements[width][];
            for (int i = 0; i < width; i++) amusementMap[i] = new Amusements[height];
        }
#warning isFree metody jsou 2
        public bool isFree(byte x, byte y) {
            /*if (objectsInMapSquares[x][y] == null) return true;
            else return false;*/
            if (amusementMap[x][y] == null && pathMap[x][y] == null) return true;
            else return false;
        }
        public bool isFree(Coordinates c){
           /* if (objectsInMapSquares[c.x][c.y] == null) return true;
            else return false;*/
            if (amusementMap[c.x][c.y] == null && pathMap[c.x][c.y] == null) return true;
            else return false;
        }
        public void RemoveAmus(Amusements a) {          
            foreach (var c in a.GetAllPoints()) amusementMap[c.x][c.y] = null;
            if (a.entrance != null)  amusementMap[a.entrance.coord.x][a.entrance.coord.y]=null;
            if (a.exit != null)  amusementMap[a.entrance.coord.x][a.entrance.coord.y] = null;
            //todo:vyse uvedene by nemelo byt null, entrance a exit musi vzdy existovat, byt to je stejne policko jako atrakce
            //don't set null to pathMap - it made entrance.Destruct()
            //amusDeletedId = a.id;
        }
        public void AddAmus(Amusements a) {
            
            foreach (var c in a.GetAllPoints()) amusementMap[c.x][c.y] = a;
            try
            {
                amusementMap[a.entrance.coord.x][a.entrance.coord.y] = a;
                amusementMap[a.exit.coord.x][a.exit.coord.y] = a;
                pathMap[a.entrance.coord.x][a.entrance.coord.y] = a.entrance;
                auxPathMap[a.entrance.coord.x][a.entrance.coord.y] = new DirectionItem(a.entrance.coord);
                pathMap[a.exit.coord.x][a.exit.coord.y] = a.exit;
                auxPathMap[a.exit.coord.x][a.exit.coord.y] = new DirectionItem(a.exit.coord);

            }
            catch (NullReferenceException e)
            {
                throw new MyDebugException("Entry or exit is null in AddAmus" + e.ToString());
            }
            
            lastAddedAmus.Enqueue(a);
        }
        public void DeleteDirectionToAmusement(int amusId)//mozna nebude treba, principialne neni spatne, kdyz jde clovek dal
        {
            Path p;
            for (int i = 0; i < widthMap; i++) 
                for (int j = 0; j < heightMap; j++)
                { 
                   if ((p = pathMap[i][j]) != null) p.signpostAmus[amusId] = Direction.no;
                }
            //amusDeletedId = -1;
        }

        public void AddPath(Path p)
        { //todo:TS, kdyby jednotlive radky byly atomicke, staci to? Tj. jak jsou mezi sebou provazane?
            pathMap[p.coord.x][p.coord.y] = p;
            auxPathMap[p.coord.x][p.coord.y] = new DirectionItem(p.coord);
            pathChanged = true;
        }
        public void RemovePath(Path p) {   //todo:TS, kdyby jednotlive radky byly atomicke, staci to? Tj. jak jsou mezi sebou provazane?       
            pathMap[p.coord.x][p.coord.y] = null;
            auxPathMap[p.coord.x][p.coord.y] = null;
            pathChanged = true;
        }
        /// <summary>
        /// Set calculated directions to paths in pathMap
        /// </summary>
        /// <param name="a"></param>
        /// <param name="queue"></param>
        /// <param name="paths"></param>
        private void UpdateDirectionToAmusementPrivate(Amusements a,Queue<DirectionItem> queue, DirectionItem[][] paths) {
            DirectionItem item;
            InitializeQueue(queue, a.entrance.coord, paths);
            while (queue.Count != 0)
            {
                item = queue.Dequeue();
                ProcessQueueItem(item.c, queue, paths);
            }
            //---set calculated directions
            //hash: takto divne kvuli vice vlaknum
           
            for (int i = 0; i < widthMap; i++)
            {
                for (int j = 0; j < heightMap; i++) 
                {
                    if ((item = paths[i][j]) != null)// && (p = objectsInMapSquares[i][j]) is Path)
                    {
                        //paths[][] can't be changed anywhere during running this method -> if is thread-safe
                        pathMap[i][j].signpostAmus[a.id] = item.dir;
                        //((Path)p).signpostAmus[a.id] = item.dir;
                    }
                }
            }
            
        
        }
        public void UpdateDirectionToAmusement(Amusements a) {
            Queue<DirectionItem> queue = new Queue<DirectionItem>(widthMap*heightMap+5);
            DirectionItem[][] paths=(DirectionItem[][])auxPathMap.Clone();
            UpdateDirectionToAmusementPrivate(a,queue,paths);      
        }
        public void UpdateDirections()
        {
            Amusements[] amusA = model.amusList.GetCopyArray();
            DirectionItem[][] paths = (DirectionItem[][])auxPathMap.Clone();
            Queue<DirectionItem> queue = new Queue<DirectionItem>();
            foreach (var a in amusA)
            {
                UpdateDirectionToAmusementPrivate(a,queue,paths);
                queue.Clear();
                for (int i = 0; i < widthMap; i++)
                    for (int j = 0; j < heightMap; j++)                 
                        if (paths[i][j] != null) paths[i][j].dir = Direction.no;                
            }
        }
        
        private void InitializeQueue(Queue<DirectionItem> queue, Coordinates start, DirectionItem[][] paths) {
            paths[start.x][start.y].dir = Direction.here; //uz ne...null - people can't go over entrance
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
        
        }
        /// <summary>
        /// analyse item's neighbours, eventually enque them and set them right direction
        /// </summary>
        /// <param name="item">coordinates of queue item which is analysed</param>
        /// <param name="queue">queue which realise BFS</param>
        /// <param name="paths">a copy of auxPathMap </param>
        private void ProcessQueueItem(Coordinates item, Queue<DirectionItem> queue, DirectionItem[][] paths)
        {
            //border fields (except the gate) don't matter because there is a fence (i.e. null) around the playing map
            //direction is set the other way around because it is correct from a person perspective.
            //todo:neni TS, nejspis vyloucit s odebranim chodniku
            DirectionItem aux;
            if ((item.x - 1 >= 0) && (aux = paths[item.x - 1][item.y]) != null && aux.dir==Direction.no) //item.x-1>=0 due to the gate
            {
                aux.dir = Direction.E;
                queue.Enqueue(aux);
            }
            if ((aux = paths[item.x][item.y - 1]) != null && aux.dir == Direction.no)
            {
                aux.dir = Direction.S;
                queue.Enqueue(aux);
            }
            if ((aux = paths[item.x][item.y + 1]) != null && aux.dir == Direction.no)
            {
                aux.dir = Direction.N;
                queue.Enqueue(aux);
            }
            if ((aux = paths[item.x + 1][item.y]) != null && aux.dir == Direction.no)
            {
                aux.dir = Direction.W;
                queue.Enqueue(aux);
            }       
        }   
#warning Odsud nize je TS
    /// <summary>
    /// Updates the navigation, i.e. sets current directions to paths.
    /// </summary>
        public void Action() {

            if (pathChanged) { // OK, false can be set only here
                pathChanged = false;
                // an another thread can potentially set pathChanged=true in this moment, it can cause a useless call UpdateDirection() in the next Action(); -> I decided, it doesn't matter. 
                UpdateDirections(); }
            else
            {
                Amusements a;
                while(!lastAddedAmus.IsEmpty){ //is T-S, dequeu can be done only here
                    if (lastAddedAmus.TryDequeue(out a)) UpdateDirectionToAmusement(a);
                }
              //todo:  if (amusDeletedId >= 0) { DeleteDirectionToAmusement(amusDeletedId); }//todo: nejspis zbytecne, nepotrebujeme, clovek dojde na misto a zjisti, ze tam nic neni, vybere tedy novou atrakci
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
            //todo: overit, ze toto staci          
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
