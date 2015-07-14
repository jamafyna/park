using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LunaparkGame
{
    
    public class Model //todo: Mozna nahradit Evidenci modelem, aby se musela synchronizovat jen jedna vec
    {
        private const int initialMoney = 1;
        public readonly int width=10,height=15;
        public Queue<MapObjects> dirtyNew;
        public Queue<Control> dirtyDestruct;//pouze pbox.Destruct, mozna jen zadat souradnice
        public ListOfAmusements amusList;
        public PersonList persList = new PersonList();
        //todo: ostatni user akce - kliky a formy...
        public int[][] map;//todo: zvazit, zda ne jen bool
        public Amusements lastBuiltAmus;
        public MapObjects lastClick { set; get; }
        public bool mustBeEnter = false;
        public bool mustBeExit = false;
        public bool demolishOn = false;
        public int money;
        public readonly int maxAmusementsCount;
        
        
        public Model(int height, int width){
            this.height=height;
            this.width=width;
            money = initialMoney;
            //---inicializace mapy - 0 prazdne, -1 chodnik, ostatni id atrakce
            map=new int[width][];
            for (int i = 0; i < width; i++) map[i] = new int[height];
            maxAmusementsCount = height * width;
            amusList = new ListOfAmusements(maxAmusementsCount);
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
        public int ReturnFreeID()
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
    }

    public class PersonList
    {
        private List<Person> list;
        public void Action()
        {
            foreach (var p in list)
            {
                p.Action();//todo: casem idealne ve vice vlaknech
            }
        }
        public void Demolish(int id)
        {
            Person p = list.Find(q => q.id == id);
            p.Demolish();//todo: mozna v opacnem smeru, tj. list.demolish vola person.demolish
            throw new NotImplementedException();
        }
        public void Add(Person p)
        {
            throw new NotImplementedException();
        }
        private int GetFreeID()
        {
            throw new NotImplementedException();
        }

    }
    public class Map: IActionable 
    {
        private bool[][] isFree;
        private Path[][] paths;
        private Direction[][][] path;
        private bool pathChanged = false;//,amusDeleted = false;
        private int amusDeletedId = -1;
        private byte width, height;
        private int maxAmusCount;
        

        public Map(byte width, byte height, int maxAmusCount) {
            this.width = width;
            this.height = height;
            this.maxAmusCount = maxAmusCount;
            //----initialize isFree
            isFree = new bool[width][];
            for (int i = 0; i < width; i++) isFree[i] = new bool[height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++) isFree[i][j] = true;
            }
            //----initialize paths
            paths=new Path[width][];
            for (int i = 0; i < width; i++) paths[i] = new Path[height]; 
            //----initialize path
            path = new Direction[width][][];
            for (int i = 0; i < width; i++) path[i] = new Direction[height][];
            /*for (int i = 0; i < width; i++)//nechci, chci aby byl null, kde neni chodnik
            {
                for (int j = 0; j < height; j++)
                {
                    path[i][j]=new Direction[maxAmusCount];//hash:zkontrolovat, jestli opravdu .no je inicialni
                }
            } */        
        }
        public void RemoveAmus(Amusements a) {
            amusDeletedId = a.id;
            foreach (var c in a.GetAllPoints()) isFree[c.x][c.y] = true;
          /*  if (a.entrance != null) { isFree[a.entrance.coord.x][a.entrance.coord.y]=true;}//mel by udelat chodnik
            if (a.exit != null) { isFree[a.entrance.coord.x][a.entrance.coord.y] = true; }*/
        }
        public void AddAmus(Amusements a) {
            foreach (var c in a.GetAllPoints()) isFree[c.x][c.y] = false;
           /* if (a.entrance != null) { isFree[a.entrance.coord.x][a.entrance.coord.y] = true; }
            if (a.exit != null) { isFree[a.entrance.coord.x][a.entrance.coord.y] = true; }*/
        }
        public void DeleteDirectionToAmusement(int amusId)//mozna nebude treba, principialne neni spatne, kdyz jde clovek dal
        {
            Direction[] temp;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                    if ((temp = path[i][j]) != null) temp[amusId] = Direction.no;
            }
            amusDeletedId = -1;
        }

        public void AddPath(Path p) { 
            path[p.coord.x][p.coord.y]=new Direction[maxAmusCount];
            isFree[p.coord.x][p.coord.y] = false;
            pathChanged = true;
        }
        public void RemovePath(Path p) {
            path[p.coord.x][p.coord.y] = null;
            isFree[p.coord.x][p.coord.y] = true;
            pathChanged = true;
        }
        public void UpdateDirectionToAmusement(int amusId) {           
             throw new NotImplementedException();       
        }
        public void UpdatePathsDirection() {
            throw new NotImplementedException();
        }
        public void Action() {
            if (amusDeletedId >= 0) {DeleteDirectionToAmusement(amusDeletedId);}
            if (pathChanged) UpdatePathsDirection();
        }
        
    }


}
