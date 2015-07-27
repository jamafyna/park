﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace LunaparkGame
{
    
    public class Model //todo: Mozna nahradit Evidenci modelem, aby se musela synchronizovat jen jedna vec
    {       
        private const int initialMoney = 1;
        public readonly int width = 10, height = 15; //todo: virtual metoda u atrakci pocita s tim, ze jsou to ty viditelne uzivatelem
        public Queue<MapObjects> dirtyNew;
        public Queue<Control> dirtyDestruct;//pouze pbox.Destruct, mozna jen zadat souradnice
        public ListOfAmusements amusList;
        public PersonList persList = new PersonList();
        public Map maps;
        //todo: ostatni user akce - kliky a formy...
        public Amusements lastBuiltAmus;
        public MapObjects lastClick { set; get; }
        public bool mustBeEnter = false;
        public bool mustBeExit = false;
        public bool demolishOn = false;
        public int money;
        public readonly int maxAmusementsCount;
        
        
        public Model(byte height, byte width){
            this.height=height;
            this.width=width;
            money = initialMoney;
           
            maxAmusementsCount = height * width+1;//todo: pokud zde bude uz myslenkove height+2, upravit, ale nezapomenout na +1 za branu
            amusList = new ListOfAmusements(maxAmusementsCount);
            maps=new Map(width,height,this);
        }
        
       
    }
    /// <summary>
    /// list of all amusements, at position i is an amusement with id==i
    /// </summary>
    public class ListOfAmusements
    { //todo: nejspis by mela byt thread-safe
        static Random rand = new Random();
        private List<Amusements> list;
        private Queue<int> freeId;
        private List<int> foodIds;
#warning overit, ze v count je opravdu spravne
        public int amusementsCount { get { return list.Count; } private set { } }
        public int restaurantsCount { get { return foodIds.Count; } private set { } }
       // private int reallyId=1;
        public ListOfAmusements(int maxAmusCount)
        {
            //todo:create brana
            list = new List<Amusements>();
            freeId = new Queue<int>(maxAmusCount);
            for (int i = maxAmusCount; i > 0; i--) freeId.Enqueue(i);            
        }
        public void Add(Amusements a)
        {
            /*if (a.id == list.Count) list.Add(a);
            else throw new MyDebugException("nesedi id a count v ListOfAmusements-Add()"); //todo: nemelo by se stavat, protoze by vzdy melo jit vytvorit jen jednu atrakci
           */
            list.Add(a);
            if (a is Restaurant) foodIds.Add(a.id);
        }
        public int GetFreeID()
        {
            //return list.Count;
            try
            {
                return freeId.Dequeue();
            }
            catch (InvalidOperationException e) {
                throw new MyDebugException("Pocet atrakci prekrocil maximalni povoleny pocet"+e.ToString());
            }
        }
        public void Remove(Amusements a)
        {
           /* Amusements b = list[list.Count - 1];
            b.ChangeId(a.id);
            list[a.id] = b;
            reallyIdArray[a.id] = reallyIdArray[list.Count - 1];
            reallyIdArray[list.Count - 1] = 0;
            list.RemoveAt(list.Count - 1);*/
            list.Remove(a);
            if(a is Restaurant) foodIds.Remove(a.id);
            freeId.Enqueue(a.id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns id of an amusement (except of the gate) if it is possible, if not - returns the gate's id</returns>
        public int GetRandomAmusement() {
            if (list.Count > 1) return list[rand.Next(1, list.Count)].id;
            else return 0;
        }
        public int GetRandomRestaurant() { 
            return foodIds[rand.Next(foodIds.Count)];
        }
        public void ForeachAction()
        {
            list.ForEach(a=>a.Action());
           /* foreach (Amusements a in list)
            {
                a.Action();
            }*/
        }
        public Coordinates GetGateCoordinate() {
            return list[0].coord;       
        }
        public int GetGateId() {
            return 0;
        }
        public Amusements[] GetCopyArray() { 
            Amusements[] a=new Amusements[list.Count];
            list.CopyTo(a);
            return a;
        }
    }

    public class PersonList
    {
       // private List<Person> list;
        //Mozna je jednodussi pouzit List misto sloziteho pole, nekolikrat se prochazi - ok, add-ok, smazani projde cele pole (tak 400-1000 prvku)
        //zkusit udelat list2 s Listem
        const int max = 65536;//2^16  //131072; //2^17
        const int maxPeopleCountInPark=1000;
        private int[] interChangablePeopleId=new int[max];
        private Person[] people=new Person[maxPeopleCountInPark];
        private int countOfPeopleArray;
        private int totalPeopleCount;
       
        public PersonList() {
            //for (int = 0; i < people.Length; i++) people[i] = null; inicialni hodnota je uz nastavena
            for (int i = 0; i < interChangablePeopleId.Length; i++) interChangablePeopleId[i] = -1;
            countOfPeopleArray = 0;
            totalPeopleCount = 0;
        }
        public int GetActualPeopleCount() {
            return countOfPeopleArray;
        }
        public int GetTotalPeopleCount() {
            return totalPeopleCount;
        }
        public int GetFreeId() {
            //todo: musi byt thread-safe!!!
            return totalPeopleCount++ % max; //Ed. for sure: after all it incremets 
            //it could happen potentially that this id has an another person, never mind - checking in adding person
        }
        public void Add(Person p)//todo:thread-safe
        {
            if (interChangablePeopleId[p.id] != -1)  {
                //todo: smaz tuto osobu - preziva moc dlouho  -rucni smazani+smazani cloveka-pozor, nesmi odstranovat z mapy
            }
                //vyse uvedene je Thread-safe, protoze pri porovnani se nestihne novy cyklus (tj. vytvorit nekolik tisic osob)
            //a je odjinud zaruceno, ze jinak nez novym cyklem nedostaneme stejne id          
#warning zeptat se Jezka, jestli  to nize je opravdu atomicke, jestli to nekazi aktPeopleCount+1            
            int interId = countOfPeopleArray++;//todo: ulozeni a zvyseni musi probehnout atomicky!! //mozna:int interId = Interlocked.Exchange(ref countOfPeopleArray, countOfPeopleArray + 1);
         
            interChangablePeopleId[p.id] = interId;
            people[interId] = p;
            

        }
        public void Action()//todo: thread-safe - zamek people pro cteni
        {           
            //todo: casem idealne ve vice vlaknech (experimentalne overit, zda je zapotrebi)
            try
            {
                for (int i = 0; i < countOfPeopleArray; i++)//hash: jak funguje threading?, tj. co se stane, kdyz se jinde zmeni countOfPeopleArray
                {
                    people[i].Action(); 
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
            int id=interChangablePeopleId[p.id];
            interChangablePeopleId[p.id]=-1;
            Person temp;
            //DEBUG check
            if (id == -1) throw new MyDebugException("PeopleList.Remove - id=-1, tj. p nebyla nejspis v seznamu");//pozdeji nedat nebo ignorovat
            //DEBUG check
            if (people[id] != p) throw new MyDebugException("PeopleList.Remove - person[id]!=p: p.id: "+p.id.ToString()+", p: "+p.ToString());
            if(id==countOfPeopleArray-1){ //p is the last item
                people[id]=null;//due to GC
                countOfPeopleArray--;
            }
            else {
                temp = people[countOfPeopleArray - 1];//last item
                people[id] = temp;
                interChangablePeopleId[temp.id]=id;
                people[countOfPeopleArray - 1] = null;//due to GC
                countOfPeopleArray--;
            }
            

          
        }
        
        

    }
    public class Map: IActionable 
    {
        //private bool[][] isFree;
        class DirectionItem {
            public Direction dir;
            public readonly Coordinates c;
            public DirectionItem(Coordinates c){
                this.c = c;
                dir = Direction.no; 
            }       
        }

        private DirectionItem[][] auxPathMap;//null = there isnt path
        private MapObjects[][] objectsInMapSquares;
        private Amusements[][] amusementMap;
        private Path[][] pathMap;
        private Model model;
        private Queue<Amusements> lastAddedAmus=new Queue<Amusements>();

       // private Direction[][][] path;
        private bool pathChanged = false, amusAdd=false;//,amusDeleted = false;
        private int amusDeletedId = -1;
        public byte widthMap { get; private set; }
        public byte heightMap{get;private set;}
        private int maxAmusCount;
        

        public Map(byte width, byte height, Model m) {
            this.widthMap = width;
            this.heightMap = height;
            this.model = m;
            this.maxAmusCount = model.maxAmusementsCount;
            
           
            /*//----initialize isFree
            isFree = new bool[width][];
            for (int i = 0; i < width; i++) isFree[i] = new bool[height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++) isFree[i][j] = true;
            }
         
            //----initialize path
            path = new Direction[width][][];
            for (int i = 0; i < width; i++) path[i] = new Direction[height][];
            for (int i = 0; i < width; i++)//nechci, chci aby byl null, kde neni chodnik
            {
                for (int j = 0; j < height; j++)
                {
                    path[i][j]=new Direction[maxAmusCount];//hash:zkontrolovat, jestli opravdu .no je inicialni
                }
            }   */
      
            //---initialize helpPath
            auxPathMap=new DirectionItem[width][];
            for (int i = 0; i < width; i++)  auxPathMap[i] = new DirectionItem[height];
           
            //---initialize objectsInMap
            objectsInMapSquares=new MapObjects[width][];
            for (int i = 0; i < width; i++) objectsInMapSquares[i] = new MapObjects[height];
            
        }
#warning isFree metody jsou 2
        public bool isFree(byte x, byte y) {
            if (objectsInMapSquares[x][y] == null) return true;
            else return false;
        }
        public bool isFree(Coordinates c){
            if (objectsInMapSquares[c.x][c.y] == null) return true;
            else return false;
        /*    if (amusementMap[c.x][c.y] == null && pathMap[c.x][c.y] == null) return true;
            else return false;*/
        }
        public void RemoveAmus(Amusements a) {          
            foreach (var c in a.GetAllPoints()) objectsInMapSquares[c.x][c.y] = null;
            amusDeletedId = a.id;
          /*  if (a.entrance != null) { isFree[a.entrance.coord.x][a.entrance.coord.y]=true;}//mel by udelat chodnik
            if (a.exit != null) { isFree[a.entrance.coord.x][a.entrance.coord.y] = true; }*/
        }
        public void AddAmus(Amusements a) {
            
            foreach (var c in a.GetAllPoints()) objectsInMapSquares[c.x][c.y] = a;
            try
            {
                objectsInMapSquares[a.entrance.coord.x][a.entrance.coord.y] = a.entrance;
                //not add to auxPathMap, because I want to people can't go over entrance-UZ NEPLATNE 
                auxPathMap[a.entrance.coord.x][a.entrance.coord.y] = new DirectionItem(a.entrance.coord);
                objectsInMapSquares[a.exit.coord.x][a.exit.coord.y] = a.exit;
                auxPathMap[a.exit.coord.x][a.exit.coord.y] = new DirectionItem(a.exit.coord);

            }
            catch (NullReferenceException e)
            {
                throw new MyDebugException("Entry or exit is null in AddAmus" + e.ToString());
            }
            amusAdd = true;
            lastAddedAmus.Enqueue(a);
        }
        public void DeleteDirectionToAmusement(int amusId)//mozna nebude treba, principialne neni spatne, kdyz jde clovek dal
        {
           /* Direction[] temp;
            
            for (int i = 0; i < widthMap; i++)
            {
                for (int j = 0; j < heightMap; j++)
                    if ((temp = path[i][j]) != null) temp[amusId] = Direction.no;
            }
           */
            Path p;
            for (int i = 0; i < widthMap; i++) 
                for (int j = 0; j < heightMap; j++)
                { 
                    p=objectsInMapSquares[i][j] as Path;
                    if (p!=null) p.signpostAmus[amusId] = Direction.no;
                }
            amusDeletedId = -1;
        }

        public void AddPath(Path p) { 
            /*path[p.coord.x][p.coord.y]=new Direction[maxAmusCount];
            isFree[p.coord.x][p.coord.y] = false;
            pathChanged = true;*/

            objectsInMapSquares[p.coord.x][p.coord.y] = p;
            auxPathMap[p.coord.x][p.coord.y] = new DirectionItem(p.coord);
            pathChanged = true;
        }
        public void RemovePath(Path p) {
           /* path[p.coord.x][p.coord.y] = null;
            isFree[p.coord.x][p.coord.y] = true;
            pathChanged = true;
            */
            objectsInMapSquares[p.coord.x][p.coord.y] = null;
            auxPathMap[p.coord.x][p.coord.y] = null;
            pathChanged = true;
        }
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
            MapObjects p;
            for (int i = 0; i < widthMap; i++)
            {
                for (int j = 0; j < heightMap; i++)
                {
                    if ((item = paths[i][j]) != null && (p = objectsInMapSquares[i][j]) is Path)
                    {
                        ((Path)p).signpostAmus[a.id] = item.dir;
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
            paths[start.x][start.y].dir = Direction.here; //...null - people can't go over entrance
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
        public void Action() {

            if (pathChanged) { pathChanged = false; UpdateDirections(); }
            else
            {
                if (lastAddedAmus.Count > 0)
                {
                    Amusements[] a = new Amusements[lastAddedAmus.Count];
                    lastAddedAmus.CopyTo(a, 0);
                    lastAddedAmus.Clear();
                    foreach (var item in a)
                    {
                        UpdateDirectionToAmusement(item);
                    }
                }
                if (amusDeletedId >= 0) { DeleteDirectionToAmusement(amusDeletedId); }
            }
        }
        /// <summary>
        /// return direction to the amusement with id==amusId
        /// </summary>
        /// <param name="amusId">id of the amusements</param>
        /// <param name="x">the real! x-coordinate</param>
        /// <param name="y">the real! y-coordinate</param>
        /// <returns></returns>
        public Direction GetDirectionToAmusement(int amusId, int x, int y) {
            Path p = objectsInMapSquares[x / MainForm.sizeOfSquare][y / MainForm.sizeOfSquare] as Path;
            return p.signpostAmus[amusId];
        //todo: misto as pouzivat opravdu 2 mapy - 1 atrakce, 2. chodniky
        }
        /// <summary>
        /// return the amusement which lies on the specified coordinates
        /// </summary>
        /// <param name="x">the real! x-coordinate</param>
        /// <param name="y">the real! y-coordinate</param>
        /// <returns></returns>
        public Amusements GetAmusement(int x, int y) { //x,y jsou nejspis souradnice vstupu, tj. chce to samostastne pole amusements, kam se ulozi i na vstup a vystup dana atrakce
            
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">the real! x-coordinate</param>
        /// <param name="y">the real! y-coordinate</param>
        /// <returns>returns true if on the point [x,y] is a path, otherwise false </returns>
        public bool IsPath(int x, int y) {
            if (auxPathMap[x / MainForm.sizeOfSquare][y / MainForm.sizeOfSquare] == null) return false;
            else return true;
        }
    
    }
    
}
