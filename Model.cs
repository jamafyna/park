﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.IO;

namespace LunaparkGame {

    /// <summary>
    /// Keeps all items of the current game, is thread-safe.
    /// </summary>
    [Serializable]
    public class GameRecords
    {
        private const int initialMoney = 10000000;
        //private const int initialMoney = 4000;
        public const int maxPeopleInPark = 10000;
        public readonly byte playingWidth, playingHeight;
        public readonly byte internalWidth, internalHeight;
        public readonly int maxAmusementsCount; // count of big amusements, it doesnt include littleComplementaryAmusements
        public bool parkClosed;// = true;
        private int money;

        /// <summary>
        /// ReaderWriterLockSlim item. Use for manipulation with longtermContentment.
        /// </summary>
        [NonSerialized]
        ReaderWriterLockSlim longContentmentRWL = new ReaderWriterLockSlim();
        private int longtermContentment = 100;

        /// <summary>
        /// use for manipulation with currCheapestFee
        /// </summary>
        [NonSerialized]
        private object feeLock = new object();
        public int currCheapestFee { private set; get; }//todo: postarat se o vytvoreni

        public int CurrPeopleCount { get { return persList.GetActualPeopleCount(); } }
        public int TotalPeopleCount { get { return persList.GetTotalPeopleCount(); } }
        //---containers---
        [NonSerialized]
        public ConcurrentQueue<MapObjects> dirtyClick = new ConcurrentQueue<MapObjects>();
        public readonly AmusementsList amusList;
        public readonly PersonList persList = new PersonList(maxPeopleInPark);
        public readonly Map maps;
        public int[] currBuildedItems { private set; get; }


        //---running fields for MainForm
        public Amusements LastBuiltAmus { get; set; }
        public MapObjectsFactory LastClick { private set; get; }
        public bool mustBeEnter = false;
        public bool mustBeExit = false;
        public bool demolishOn = false;
        public Gate gate;

        public readonly System.Drawing.Image[] images;
        public ConcurrentBag<AmusementsFactory> currOfferedAmus;
        public ConcurrentBag<PathFactory> currOfferedPaths;
        public ConcurrentBag<MapObjectsFactory> currOfferedOthers;
        public ConcurrentQueue<LaterShownItem> laterAddedObjects;
        public readonly SpecialEffects effects;

        public GameRecords(byte playingHeight, byte playingWidth, string InitAmusFilename, string initPathFilename, string initAccFilename, string RevealingRulesFilename) {
            // ----- setting variables -----
            parkClosed = true;
            this.playingHeight = playingHeight;
            this.playingWidth = playingWidth;
            this.internalHeight = (byte)(playingHeight + 2);
            this.internalWidth = (byte)(playingWidth + 2);
            maxAmusementsCount = playingHeight * playingWidth + 1; // max. count of amusements that can user build, + 1 due to the gate which does not lie on the playing place        
            money = initialMoney;

            // ----- creating containers -----
            maps = new Map(internalWidth, internalHeight, this);
            gate = new Gate(this, new Coordinates(0, (byte)(new Random()).Next(1, internalHeight - Gate.height - 1)));
            amusList = new AmusementsList(maxAmusementsCount, gate);

            // ----- loading initialization data -----
            System.Drawing.Image[] im = { Properties.Images.gate, Properties.Images.enter, Properties.Images.exit };
            Data data = new Data(im);
            LoadExternalData(data, InitAmusFilename, initPathFilename, initAccFilename, RevealingRulesFilename);
            images = data.GetImages();
            InitializeCurrBuildedItemsArray(data.GetItemsCount());
            effects = new SpecialEffects(this, data.laterShowedItems);
            data = null; // due to GC

        }
        [OnDeserialized]
        private void SetValuesAndCheckOnDeserialized(StreamingContext context) {
            feeLock = new object();
            longContentmentRWL = new ReaderWriterLockSlim();
            dirtyClick = new ConcurrentQueue<MapObjects>();
            // parkClosed = false; 
            //todo: jakmile zjistim, proc je vzdy true po deserializaci a spravim to, toto odstranit
        }
        private void LoadExternalData(Data data, string initAFilename, string initPFilename, string initAccFilename, string rulesFilename) {
            StreamReader srAmus = new StreamReader(initAFilename);
            StreamReader srPath = new StreamReader(initPFilename);
            StreamReader srAcc = new StreamReader(initAccFilename);
            StreamReader srAddition = new StreamReader(rulesFilename);

            data.LoadAll(srAmus, srPath, srAcc, srAddition);
            srAmus.Close();
            srPath.Close();
            srAcc.Close();
            srAddition.Close();

            currOfferedAmus = new ConcurrentBag<AmusementsFactory>(data.initialAmus);
            currOfferedPaths = new ConcurrentBag<PathFactory>(data.initialPaths);
            currOfferedOthers = new ConcurrentBag<MapObjectsFactory>(data.initialOthers);
            laterAddedObjects = new ConcurrentQueue<LaterShownItem>(data.laterShowedItems);
        }




        public void ChangeLongtermContentment(int contentment) {
            if (contentment > 100 || contentment < 0) return;
            try {
                longContentmentRWL.EnterWriteLock();
                longtermContentment = (longtermContentment + contentment) / 2;
            }
            finally {
                longContentmentRWL.ExitWriteLock();
            }
        }
        public int GetLongTermContentment() {
            try {
                longContentmentRWL.EnterReadLock();
                return longtermContentment;
            }
            finally {
                longContentmentRWL.ExitReadLock();
            }

        }




        public void MoneyAdd(int value) {
            Interlocked.Add(ref money, value);
        }
        /// <summary>
        /// Returns value of the current money acount.
        /// </summary>
        /// <returns>Int that represents current money.</returns>
        public int GetMoney() { return this.money; }
        public void CheckCheapestFee(int fee) {
            lock (feeLock) {
                if (fee < currCheapestFee) currCheapestFee = fee;
            }
        }
        /// <summary>
        /// Set MapObjectsFactory value to lastClick.
        /// </summary>
        /// <param name="lastClick">MapObjectsFactory item, but not null!</param>
        public void SetLastClick(MapObjectsFactory lastClick) {
            if (!mustBeEnter && !mustBeExit) this.LastClick = lastClick;
            else MessageBox.Show(Notices.unfinishedBuilding, Labels.warningMessBox, MessageBoxButtons.OK);
        }
        public void SetNullToLastClick() {
            this.LastClick = null;
        }
        public void MarkOutOfService(Amusements a) {
            Interlocked.Decrement(ref currBuildedItems[a.internTypeID]);
        }
        public void MarkBackInService(Amusements a) {
            Interlocked.Increment(ref currBuildedItems[a.internTypeID]);
        }

        public void InitializeCurrBuildedItemsArray(int maxItemsVariety) {
            currBuildedItems = new int[maxItemsVariety];
        }
    }


    /// <summary>
    /// Keeps all Amusement items in the current game, is thread-safe.
    /// </summary>
    [Serializable]
    public class AmusementsList : IActionable {
        [NonSerialized]
        static Random rand = new Random();
        private List<Amusements> list;
        [NonSerialized]
        private ConcurrentQueue<int> freeId;
        [NonSerialized]
        private List<int> foodIds;
        private readonly int maxAmusCount;
        // <summary>
        /// use for list and foodIds
        /// </summary>
        [NonSerialized]
        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly Amusements gate;

        public int AmusementsCount { get { return list.Count; } private set { } } //hash: zeptat se, ale nemel by byt problem, maximalne je neaktualni
        public int RestaurantsCount { get { return foodIds.Count; } private set { } }

        public AmusementsList(int maxAmusCount, Gate gate) {
            this.gate = gate;
            list = new List<Amusements>();
            list.Add(gate);
            foodIds = new List<int>();
            this.maxAmusCount = maxAmusCount;
            freeId = new ConcurrentQueue<int>();
            for (int i = 1; i < maxAmusCount; i++) freeId.Enqueue(i);  //0 is not free, gate.id==0;          
        }

        [OnDeserialized]
        private void SetValuesAndCheckOnDeserialized(StreamingContext context) {
            rand = new Random();
            rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            if (list.Count == 0) throw new MyDebugException("Wrong deseralization in AmusementList, a gate is not in the list.");
            if (list[0] != gate) throw new MyDebugException("Wrong deseralization in AmusementList, a gate is not at the first position or has a wrong value.");
            List<int> newFreeId = new List<int>();
            for (int i = 0; i < maxAmusCount; i++) {
                if (list.Find(a => a.Id == i) == null) newFreeId.Add(i);
            }
            freeId = new ConcurrentQueue<int>(newFreeId);
            foodIds = new List<int>();
            foreach (Amusements a in list) {
                if (a is Restaurant) foodIds.Add(a.Id);
            }

        }

        public void Add(Amusements a) {
            /*if (a.id == list.Count) list.Add(a);
            else throw new MyDebugException("nesedi id a count v AmusementsList-Add()"); //todo: nemelo by se stavat, protoze by vzdy melo jit vytvorit jen jednu atrakci
           */
            try {
                rwLock.EnterWriteLock();
                list.Add(a);
                if (a is Restaurant) foodIds.Add(a.Id);
            }
            finally {
                rwLock.ExitWriteLock();
            }
        }
        public int GetFreeID() {
            try {
                int tempId;
                while (!freeId.TryDequeue(out tempId)) { }//hash:overit, zda funguje
                return tempId;
            }
            catch (InvalidOperationException e) {
                throw new MyDebugException("Pocet atrakci prekrocil maximalni povoleny pocet" + e.ToString());
            }
        }
        public void Remove(Amusements a) {
            rwLock.EnterWriteLock();
            try {
                list.Remove(a);
                if (a is Restaurant) foodIds.Remove(a.Id);
            }
            finally {
                rwLock.ExitWriteLock();
            }

            freeId.Enqueue(a.Id);
        }
        /// <summary>
        /// Returns id of an amusement (except of the gate) if it is possible(at least one another amusement), if not - returns the gate's id
        /// </summary>
        /// <returns>A positive int which represents id of an amusement or 0 which represents id of the gate.</returns>
        public int GetRandomAmusement() {
            rwLock.EnterReadLock();
            try {
                if (list.Count > 1) return list[rand.Next(1, list.Count)].Id;
                else return 0;
            }
            finally {
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

        public void Action() {
            rwLock.EnterReadLock();
            try {
                //  list.ForEach(a => a.Action());
                Parallel.ForEach(list, a => a.Action());
            }
            finally {
                rwLock.ExitReadLock();
            }
        }
        public Coordinates GetGateCoordinate() {
            return gate.coord;
        }
        public int GetGateId() {
            return gate.Id;
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
        public List<Amusements> GetAmusementsUnsynchronized() {
            return this.list;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PersonList : System.Collections.IEnumerable //todo: is TS, ale rozdelit si pole na casti a zamykat vice zamky (napr. %50)
    {
        // private List<Person> list;
        //Mozna je jednodussi pouzit List misto sloziteho pole, nekolikrat se prochazi - ok, add-ok, smazani projde cele pole (tak 400-1000 prvku)
        //zkusit udelat list2 s Listem
        const int maxUniquePeople = 65536;//2^16  //131072; //2^17
        private int[] internChangablePeopleId = new int[maxUniquePeople];
        private Person[] people;
        private int currPeopleCount;
        private int totalPeopleCount;
        public double contenment { protected set; get; }
        /// <summary>
        /// for manipulating with people array (read, write) and currPeopleCount
        /// </summary>
        [NonSerialized]
        private object peopleLock = new object();
        //  private ReaderWriterLockSlim peopleRWLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        /// <summary>
        /// useful only for a very unlikely situation - a person p survived more people then maxUniquePeople and it is called Remove(p) and Add(q) where p.id=q.id in the same time
        /// </summary>
        // private object unlikelyLock = new object();

        public PersonList(int maxPeopleCount) {
            //for (int = 0; i < people.Length; i++) people[i] = null; inicialni hodnota je uz nastavena           
            currPeopleCount = 0;
            totalPeopleCount = 0;
            contenment = 100;
            people = new Person[maxPeopleCount];
            for (int i = 0; i < internChangablePeopleId.Length; i++) internChangablePeopleId[i] = -1;
        }
        [OnDeserialized]
        private void SetValuesAndCheckOnDeserialized(StreamingContext context) {
            int count = 0;
            for (int i = 0; i < people.Length; i++) {
                if (people[i] != null) count++;
            }
            if (count != currPeopleCount) throw new MyDeserializationException("Wrong deseralization in PeopleList, wrong count of people.");
            if (contenment < 0 || contenment > 100) throw new MyDeserializationException("in PeopleList, contentment out of bounds.");
            peopleLock = new object();
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

            lock (peopleLock) {
                if (currPeopleCount == people.Length)
#if (DEBUG) 
                    throw new MyDebugException("More people than a determined value");
#else
                    return;
#endif
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

        public void Action() {
            //todo: casem idealne ve vice vlaknech (experimentalne overit, zda je zapotrebi)
            int i = 0;
            //   try {

            lock (peopleLock) {

                /* for (i = 0; i < currPeopleCount; i++) {
                     people[i].Action();
                 }
                */
                //Parallel.ForEach(source: this, p => p.Action());
                //Parallel.ForEach(source: (IEnumerable<Person>)this, body: p => p.Action());
                int newContenm = 0;

                Parallel.ForEach(people, p => { if (p != null) { p.Action(); Interlocked.Add(ref newContenm, p.GetContentment()); } });
#warning Jak tady pockam na dokonceni vsech vlaken?
                
                try {
                    contenment = newContenm / currPeopleCount;
                }
                catch (DivideByZeroException) {
                    contenment = 100;
                }

            }

            //   }
            /* catch (NullReferenceException e) {
                 throw new MyDebugException("PersonList.Action - people["+i+"]==null0"+e.ToString());
             }*/

        }

        public System.Collections.IEnumerator GetEnumerator() {
            //it is not synchronized, not use in multi-thread!!

            for (int i = 0; i < currPeopleCount; i++) {
                yield return people[i];
            }

        }
        /// <summary>
        /// a method which should be called only from the class Person
        /// </summary>
        /// <param name="p">the person which will be removed</param>
        public void Remove(Person p) {
            lock (peopleLock) {
                int internId = internChangablePeopleId[p.id];
                internChangablePeopleId[p.id] = -1;

#if (DEBUG)
                if (internId == -1) throw new MyDebugException("PeopleList.Remove - id=-1, tj. p nebyla nejspis v seznamu");
                if (people[internId] != p) throw new MyDebugException("PeopleList.Remove - person[id]!=p: p.id: " + p.id.ToString() + ", p: " + p.ToString());

#else
                if (internId == -1) return;
#endif
                if (internId == currPeopleCount - 1) { // p is the last item
                    people[internId] = null; //due to GC
                    currPeopleCount--;
                }
                else {
                    Person q;
                    q = people[currPeopleCount - 1]; // last item
                    people[internId] = q;
                    internChangablePeopleId[q.id] = internId;
                    people[currPeopleCount - 1] = null; // due to GC
                    currPeopleCount--;
                }
            }





        }



    }

    /// <summary>
    /// Keeps information about how are amusements and paths placed on the playing map. Takes care of updating directions.
    /// </summary>
    [Serializable()]
    public class Map : IActionable, ISerializable {
        class DirectionItem {
            public Direction dir;

            public readonly byte x;
            public readonly byte y;
            public bool isEnterExit;
            public DirectionItem(byte x, byte y, Type typeOfPath) {
                this.x = x;
                this.y = y;
                dir = Direction.no;
                if (typeOfPath == typeof(AmusementEnterPath) || typeOfPath == typeof(AmusementExitPath)) isEnterExit = true;
                else isEnterExit = false;
            }

        }
        private readonly DirectionItem[][] auxPathMap; // null = there isnt path  
        private readonly Amusements[][] amusementMap;
        private readonly Path[][] pathMap;
        private readonly GameRecords gameRec;
        private volatile bool pathChanged = false;
        /// <summary>
        /// for setting direction to a new amusement, dequeue can be called only in Action()
        /// </summary>
        private ConcurrentQueue<Amusements> lastAddedAmus = new ConcurrentQueue<Amusements>();

        /// <summary>
        /// use for adding to queue (due to .Clear) and for "clear"
        /// </summary>
        [NonSerialized]
        private object lastAddedAmusLock = new object();
        [NonSerialized]
        private ReaderWriterLockSlim pathRWLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        [NonSerialized]
        private ReaderWriterLockSlim amusRWLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public readonly byte internalWidthMap;
        public readonly byte internalHeightMap;

        /// <summary>
        /// Represents the current playing map, includes maps of paths and amusements, provides algorithms for navigation.
        /// </summary>
        /// <param name="width">The internal width of the map.</param>
        /// <param name="height">The internal height of the map.</param>
        /// <param name="m"></param>
        public Map(byte width, byte height, GameRecords m) {
            this.internalWidthMap = width;
            this.internalHeightMap = height;
            this.gameRec = m;


            //---initialize helpPath
            auxPathMap = new DirectionItem[width][];
            for (int i = 0; i < width; i++) {
                auxPathMap[i] = new DirectionItem[height];
            }

            //---initialize pathMap
            pathMap = new Path[width][];
            for (int i = 0; i < width; i++) pathMap[i] = new Path[height];

            //---initialize amusementMap
            amusementMap = new Amusements[width][];
            for (int i = 0; i < width; i++) amusementMap[i] = new Amusements[height];
        }

        public Map(SerializationInfo si, StreamingContext sc) {
            gameRec = (GameRecords)si.GetValue("gameRec", typeof(GameRecords));
            amusementMap = (Amusements[][])si.GetValue("amusementMap", typeof(Amusements[][]));
            pathMap = (Path[][])si.GetValue("pathMap", typeof(Path[][])); //todo: Bylo by rychlejsi, kdybych si vytvorila seznam a teprve ten pak prochazela?
            internalWidthMap = gameRec.internalWidth;
            internalHeightMap = gameRec.internalHeight;

            auxPathMap = new DirectionItem[internalWidthMap][];
            for (int i = 0; i < internalWidthMap; i++) {
                auxPathMap[i] = new DirectionItem[internalHeightMap];
            }
            lastAddedAmusLock = new object();
            pathRWLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            amusRWLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            pathChanged = true; // due to calling UpdateDirection()
            lastAddedAmus = new ConcurrentQueue<Amusements>();


        }
        public void GetObjectData(SerializationInfo si, StreamingContext sc) {
            si.AddValue("pathMap", pathMap);
            si.AddValue("amusementMap", amusementMap); //todo: neni treba, pokud uz touhle dobou bude vyplneny AmusementList
            si.AddValue("gameRec", gameRec);
        }





#warning nejspis se nebude pouzivat, tak pak smazat
        public List<Path> GetPathsUnsynchronized() {
            List<Path> list = new List<Path>();
            int height = pathMap[0].Length;
            Path p;
            for (int i = 0; i < pathMap.Length; i++) {
                for (int j = 0; j < height; j++) {
                    if ((p = pathMap[i][j]) != null) list.Add(p);
                }
            }
            return list;
        }
#warning nejspis se nebude pouzivat
        public System.Collections.IEnumerator GetEnumerator() {
            int height = pathMap[0].Length;
            Path p;
            for (int i = 0; i < pathMap.Length; i++) {
                for (int j = 0; j < height; j++) {
                    if ((p = pathMap[i][j]) != null) { yield return p; }
                }
            }
        }
        /// <summary>
        /// Not synchronized.
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerator GetPathEnumerator() {
            int height = pathMap[0].Length;
            Path p;
            for (int i = 0; i < pathMap.Length; i++) {
                for (int j = 0; j < height; j++) {
                    if ((p = pathMap[i][j]) != null) { yield return p; }
                }
            }
        }

#warning isFree metody jsou 2
        public bool isFree(byte x, byte y) {
#if (DEBUG)
            if(x==0 || x==internalWidthMap-1||y==0||y==internalHeightMap-1) throw new MyDebugException("Map.isFree - bad coordination between maps");
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
        public bool isFree(Coordinates c) {
#if (DEBUG)
            if (c.x == 0 ||c.x == internalWidthMap - 1 || c.y == 0 || c.y == internalHeightMap - 1) throw new MyDebugException("Map.isFree - bad coordination between maps");
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
            }
            finally {
                amusRWLock.ExitWriteLock();
            }
        }
        public void AddAmus(Amusements a) {
            amusRWLock.EnterWriteLock();

            try {
                foreach (var c in a.GetAllPoints()) amusementMap[c.x][c.y] = a;
            }
            finally {
                amusRWLock.ExitWriteLock();

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
                for (int i = 0; i < internalWidthMap; i++)
                    for (int j = 0; j < internalHeightMap; j++) {
                        if ((p = pathMap[i][j]) != null) p.signpostAmus[amusId] = Direction.no;
                    }
            }
            finally {
                pathRWLock.ExitReadLock();
            }
        }

        public void AddEntranceExit(AmusementPath p) {
            pathRWLock.EnterWriteLock();
            amusRWLock.EnterWriteLock();
            try {
                pathMap[p.coord.x][p.coord.y] = p;
                amusementMap[p.coord.x][p.coord.y] = p.amusement;
            }
            finally {
                pathRWLock.ExitWriteLock();
                amusRWLock.ExitWriteLock();
            }
            pathChanged = true; //important that it is set after changing pathMap
        }
        public void AddPath(Path p) {
            pathRWLock.EnterWriteLock();
            try {
                pathMap[p.coord.x][p.coord.y] = p;

            }
            finally {
                pathRWLock.ExitWriteLock();
            }
            pathChanged = true; //important that it is set after changing pathMap
        }
        public void RemoveEntranceExit(AmusementPath p) {
            pathRWLock.EnterWriteLock();
            amusRWLock.EnterWriteLock();
            try {
                pathMap[p.coord.x][p.coord.y] = null;
                amusementMap[p.coord.x][p.coord.y] = null;
                if (pathMap[p.coord.x - 1][p.coord.y] != null) pathMap[p.coord.x - 1][p.coord.y].signpostAmus[p.amusement.Id] = Direction.no;
                if (pathMap[p.coord.x + 1][p.coord.y] != null) pathMap[p.coord.x + 1][p.coord.y].signpostAmus[p.amusement.Id] = Direction.no;
                if (pathMap[p.coord.x][p.coord.y - 1] != null) pathMap[p.coord.x][p.coord.y - 1].signpostAmus[p.amusement.Id] = Direction.no;
                if (pathMap[p.coord.x][p.coord.y + 1] != null) pathMap[p.coord.x][p.coord.y + 1].signpostAmus[p.amusement.Id] = Direction.no;
            }
            finally {
                pathRWLock.ExitWriteLock();
                amusRWLock.ExitWriteLock();
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
        private void UpdateDirectionToAmusement(Amusements a, Queue<DirectionItem> queue, DirectionItem[][] paths) {
            DirectionItem item;
            AmusementPath entrance = a.entrance;
            if (entrance == null) return;
            if (!InitializeQueue(queue, entrance.coord, paths)) return;
            while (queue.Count != 0) {
                item = queue.Dequeue();
                ProcessQueueItem(item, queue, paths);
            }
            // ---set calculated directions
            pathRWLock.EnterReadLock();
            try {
                for (int i = 0; i < internalWidthMap; i++) {
                    for (int j = 0; j < internalHeightMap; j++) {
                        if ((item = paths[i][j]) != null && pathMap[i][j] != null) {
                            pathMap[i][j].signpostAmus[a.Id] = item.dir;
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
            Queue<DirectionItem> queue = new Queue<DirectionItem>(internalWidthMap * internalHeightMap + 5);
            for (int i = 0; i < internalWidthMap; i++) {
                for (int j = 0; j < internalHeightMap; j++) {
                    // here isnt a Lock, auxPathMap is not always actual -> lock here=nonsence
                    if (pathMap[i][j] == null) auxPathMap[i][j] = null;
                    else auxPathMap[i][j] = new DirectionItem((byte)i, (byte)j, (pathMap[i][j]).GetType());
                }
            }
            UpdateDirectionToAmusement(a, queue, auxPathMap);
        }
        /// <summary>
        /// Updates directions to all amusements. Useful when paths changed.
        /// </summary>
        private void UpdateDirections() {
            for (int i = 0; i < internalWidthMap; i++) {
                for (int j = 0; j < internalHeightMap; j++) {
                    // here isnt a Lock, auxPathMap is not always actual -> lock here=nonsence
                    if (pathMap[i][j] == null) auxPathMap[i][j] = null;
                    else auxPathMap[i][j] = new DirectionItem((byte)i, (byte)j, (pathMap[i][j]).GetType());
                }
            }
            Queue<DirectionItem> queue = new Queue<DirectionItem>();
            Amusements[] amusA = gameRec.amusList.GetCopyArray();
            foreach (var a in amusA) {
                UpdateDirectionToAmusement(a, queue, auxPathMap);
                queue.Clear();
                for (int i = 0; i < internalWidthMap; i++)
                    for (int j = 0; j < internalHeightMap; j++) {
                        if (auxPathMap[i][j] != null) auxPathMap[i][j].dir = Direction.no;

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
                return false; // it can happen if the entrance is demolished
            }
            paths[start.x][start.y].dir = Direction.here;
            DirectionItem aux;
            if ((start.x - 1 >= 0) && (aux = paths[start.x - 1][start.y]) != null) {
                aux.dir = Direction.E;
                // if(!aux.isEnterExit) 
                queue.Enqueue(aux); //cannot go over these paths (without stop in an amusement)
            }
            if ((aux = paths[start.x][start.y - 1]) != null) {
                aux.dir = Direction.S;
                //if (!aux.isEnterExit)
                queue.Enqueue(aux);
            }
            if ((aux = paths[start.x][start.y + 1]) != null) {
                aux.dir = Direction.N;
                //if (!aux.isEnterExit)
                queue.Enqueue(aux);
            }
            if ((aux = paths[start.x + 1][start.y]) != null) {
                aux.dir = Direction.W;
                // if (!aux.isEnterExit)
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
        private void ProcessQueueItem(DirectionItem item, Queue<DirectionItem> queue, DirectionItem[][] paths) {
            // dont use pathRWLock (or change it to recursive mode)!!
            // border fields (except the gate) don't matter because there is a fence (i.e. null) around the playing map
            // direction is set the other way around because it is correct from a person perspective.
            byte x = item.x, y = item.y;

            DirectionItem aux;
            if ((x - 1 >= 0) && (aux = paths[x - 1][y]) != null && aux.dir == Direction.no) // item.x - 1 >= 0 due to the gate
            {
                aux.dir = Direction.E;
                if (!aux.isEnterExit) queue.Enqueue(aux); // cannot go over these paths (without stop in an amusement)
            }
            if ((aux = paths[x][y - 1]) != null && aux.dir == Direction.no) {
                aux.dir = Direction.S;
                if (!aux.isEnterExit) queue.Enqueue(aux);
            }
            if ((aux = paths[x][y + 1]) != null && aux.dir == Direction.no) {
                aux.dir = Direction.N;
                if (!aux.isEnterExit) queue.Enqueue(aux);
            }
            if ((aux = paths[x + 1][y]) != null && aux.dir == Direction.no) {
                aux.dir = Direction.W;
                if (!aux.isEnterExit) queue.Enqueue(aux);
            }
        }

        /// <summary>
        /// Updates the navigation, i.e. sets current directions to paths. Only one thread can call this method.
        /// </summary>
        public void Action() {

            if (pathChanged) { // OK, false can be set only here
                // an another thread can potentially set pathChanged=true at this moment, it doesnt matter - an item pathChanged is set after the actual execution changed in the array              
                pathChanged = false;
                lock (lastAddedAmusLock) {
                    lastAddedAmus = new ConcurrentQueue<Amusements>();
                }
#warning pouzit jedine tehdy, pokud se puvodni task ukonci bez cekani na tohoto - to prave nechci kvuli cekani na dokonceni vsech tasku
                // Task.Factory.StartNew(UpdateDirections,TaskCreationOptions.LongRunning);
                UpdateDirections();
            }

            else {
                // Task.Factory.StartNew(UpdateDirectionToNotUpdatedAmusements, TaskCreationOptions.LongRunning);
                UpdateDirectionToNotUpdatedAmusements();
            }
        }
        /// <summary>
        /// It can be called only from the method Action()
        /// </summary>
        private void UpdateDirectionToNotUpdatedAmusements() {
            Amusements a;
            while (!lastAddedAmus.IsEmpty) { //is T-S, dequeu can be done only in this method and Action
                if (lastAddedAmus.TryDequeue(out a)) UpdateDirectionToOnlyOneAmusement(a);
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
            pathRWLock.EnterReadLock();
            try {
                Path p;
                if ((p = pathMap[x / MainForm.sizeOfSquare][y / MainForm.sizeOfSquare]) != null)
                    return p.signpostAmus[amusId];
                else return Direction.no;  // could happen if user demolished path when people
            }
            finally {
                pathRWLock.ExitReadLock();
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
        /// <summary>
        /// Returns a path which lies on the given coordinates or null.
        /// </summary>
        /// <param name="x">The map x-coordinate.</param>
        /// <param name="y">The map y-coordinate.</param>
        /// <returns></returns>
        public Path GetPath(byte x, byte y) {
            return pathMap[x][y];
        }
        public Amusements GetAmusement(byte x, byte y) {
            Path p;
            if ((p = pathMap[x][y]) == null || !p.tangible) return amusementMap[x][y];
            else return null;

        }

    }

}
