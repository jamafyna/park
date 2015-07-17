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
      //  private Path[][] paths;
        private Direction[][][] path;
        private bool pathChanged = false;//,amusDeleted = false;
        private int amusDeletedId = -1;
        private byte widthMap, heightMap;
        private int maxAmusCount;
        

        public Map(byte width, byte height, int maxAmusCount) {
            this.widthMap = width;
            this.heightMap = height;
            this.maxAmusCount = maxAmusCount;
            //----initialize isFree
            isFree = new bool[width][];
            for (int i = 0; i < width; i++) isFree[i] = new bool[height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++) isFree[i][j] = true;
            }
            //----initialize paths
          /*  paths=new Path[width][];
            for (int i = 0; i < width; i++) paths[i] = new Path[height]; */
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
            for (int i = 0; i < widthMap; i++)
            {
                for (int j = 0; j < heightMap; j++)
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
    static class UrceniSmeruCesty ///spocte vzdalenost vsech chodniku k dane atrakci
    {

        //spocte vzdalenost od vstupu dane atrakce ke vsem chodnikum, vlna, a vsem chodnikum nastavi smer nejkratsi cesty k atrakci
        public static void spoctiVzdalenostOdAtrakce(int idAtrakce, int xAtrakce, int yAtrakce, int vyska, int sirka, IntSmer[,] JsouChodniky, Mapa mapaChodniku)
        {
            int sxA = xAtrakce / Program.sizeOfSquare; //indexy pocatecniho policka-vstupu atrakce
            int syA = yAtrakce / Program.sizeOfSquare;
            Fronta<PrvekFronty> fronta = new Fronta<PrvekFronty>(sirka * vyska + 5);
            PocatecniVlozeni(sxA, syA, fronta, JsouChodniky); //kvuli brane - ta muze mit sousedni policko mimo mapu
            while (!fronta.Prazdna())
            {
                PrvekFronty prvek = fronta.VratPrvek();
                //prohlednuti sousedu a prip. pridani do fronty
                //okrajova policka nevadi, nebot na okrajich je plot, tj -1
                //Smer je zrcadlove, z pohledu cloveka je to totiz obracene, tj. z pohledu cloveka takto spravne
                if (NeighbourAction(JsouChodniky, prvek.sx, prvek.sy + 1, prvek.pocetKroku, fronta))
                    JsouChodniky[prvek.sx, prvek.sy + 1].smer = Direction.N;
                if (NeighbourAction(JsouChodniky, prvek.sx, prvek.sy - 1, prvek.pocetKroku, fronta))
                    JsouChodniky[prvek.sx, prvek.sy - 1].smer = Direction.S;
                if (NeighbourAction(JsouChodniky, prvek.sx + 1, prvek.sy, prvek.pocetKroku, fronta))
                    JsouChodniky[prvek.sx + 1, prvek.sy].smer = Direction.W;
                if ((prvek.sx - 1 >= 0) && NeighbourAction(JsouChodniky, prvek.sx - 1, prvek.sy, prvek.pocetKroku, fronta))
                    JsouChodniky[prvek.sx - 1, prvek.sy].smer = Direction.E;
            }
            ZaznamenejDoMapy(vyska, sirka, idAtrakce, JsouChodniky, mapaChodniku);
            NastavHodnotyZpet(vyska, sirka, JsouChodniky, xAtrakce, yAtrakce);
        }

        //vlozi do fronty sousedy startovniho policka (vstup atrakce), v pripade vseho jineho nez brany jsou az 4, v pripade brany prave 1
        private static void PocatecniVlozeni(int sxA, int syA, Fronta<PrvekFronty> fronta, IntSmer[,] JsouChodniky)
        {
            JsouChodniky[sxA, syA].cislo = 0;

            if ((sxA - 1 >= 0) && (NeighbourAction(JsouChodniky, sxA - 1, syA, 1, fronta))) //&& funguje tak, ze se nevyhodnocuje druha cast podminky, pokud je prvni nesplnena, tj. nevadi takovyto zapis
            {
                JsouChodniky[sxA - 1, syA].smer = Direction.E;

            }
            if (NeighbourAction(JsouChodniky, sxA, syA - 1, 1, fronta))
            {
                JsouChodniky[sxA, syA - 1].smer = Direction.S;

            }
            if (NeighbourAction(JsouChodniky, sxA, syA + 1, 1, fronta))
            {
                JsouChodniky[sxA, syA + 1].smer = Direction.N;

            }
            if (NeighbourAction(JsouChodniky, sxA + 1, syA, 1, fronta))
            {
                JsouChodniky[sxA + 1, syA].smer = Direction.W;

            }
        }



        private static bool NeighbourAction(Direction[][][] paths, int i, int j, int pocetKroku, Fronta<PrvekFronty> fronta)
        {
            if (paths[i, j].cislo == int.MaxValue) //tzn. je zde chodnik (kdyby nebyl, je -1) a nebyl jeste projit
            {
                PrvekFronty objekt = new PrvekFronty(i, j, pocetKroku + 1);
                fronta.Vloz(objekt);
                paths[i, j].cislo = pocetKroku;
                return true;
            }
            else return false;
        }
        //prochazi pole a zaznamenat do mapy chodniku a atrakci
        private static void ZaznamenejDoMapy(int vyska, int sirka, int id, IntSmer[,] JsouChodniky, Mapa MapaChodniku)
        {
            for (int i = 0; i < sirka; i++)
            {
                for (int j = 0; j < vyska; j++)
                {
                    MapaChodniku.uloz Smer(i, j, id, JsouChodniky[i, j].smer);
                }
            }
        }
        //upravuje zpet pole
        private static void NastavHodnotyZpet(int vyska, int sirka, IntSmer[,] JsouChodniky, int xAtrakce, int yAtrakce)
        {
            for (int i = 0; i < sirka; i++)
            {
                for (int j = 0; j < vyska; j++)
                {
                    if (JsouChodniky[i, j].cislo >= 0)//resp. je !=-1
                    {
                        JsouChodniky[i, j].cislo = int.MaxValue;
                        JsouChodniky[i, j].smer = Direction.no; //dulezite, pokud bych nenastavila, tak napr. pokud k dalsi atrakci nevede cesta, zustal by nastaven smer z predchozi - proto chodili jen na nejnovejsi
                    }
                }
            }
        }

    }


}
