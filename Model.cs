using System;
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
        private List<Amusements> list;
        private Queue<int> freeId;
#warning overit, ze v count je opravdu spravne
        public int count { get { return list.Count; } private set { } }
        private int reallyId=1;
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
            freeId.Enqueue(a.id);
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
        public Amusements[] GetCopyArray() { 
            Amusements[] a=new Amusements[list.Count];
            list.CopyTo(a);
            return a;
        }
    }

    public class PersonList
    {
       // private List<Person> list;
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
        public void Add(Person p)
        {
            if (interChangablePeopleId[p.id] != -1) {  }//todo: smaz tuto osobu - preziva moc dlouho  -rucni smazani+smazani cloveka-pozor, nesmi odstranovat z mapy
           //vyse uvedene je Thread-safe, protoze pri porovnani se nestihne novy cyklus (tj. vytvorit nekolik tisic osob)
            //a je odjinud zaruceno, ze jinak nez novym cyklem nedostaneme stejne id          
#warning zeptat se Jezka, jestli  to nize je opravdu atomicke, jestli to nekazi aktPeopleCount+1
            int interId = Interlocked.Exchange(ref countOfPeopleArray, countOfPeopleArray + 1);//misto:  int interId = aktPeopleCount++;//todo: ulozeni a zvyseni musi probehnout atomicky!!
            interChangablePeopleId[p.id] = interId;
            //todo: dodelat, dost chybi

        }
        public void Action()//todo: thread-safe - zamek people pro cteni
        {
            Person p;
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
            int newId;
            //DEBUG check
            if (people[id] != p) throw new MyDebugException("PeopleList.Remove - person[id]!=p: p.id: "+p.id.ToString()+", p: "+p.ToString());
            if(id==countOfPeopleArray-1){ //p is the last item
                people[id]=null;//due to GC
                countOfPeopleArray--;
            }
            else {
                people[id] = people[countOfPeopleArray - 1];
                people[countOfPeopleArray - 1] = null;//due to GC
                countOfPeopleArray--;
            }
            

            /*Person p = list.Find(q => q.id == id);
            p.Destruct();//todo: mozna v opacnem smeru, tj. list.demolish vola person.demolish
            throw new NotImplementedException();*/
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
                //not add to auxPathMap, because I want to people can't go over entrance               
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
            //paths[start.x][start.y].dir = Direction.here; ...null - people can't go over entrance
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
        
    }
    
}
