using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LunaparkGame
{
    public interface IActionable {
        void Action();
    
    }
    public class MyDebugException : Exception
    {
        public MyDebugException() : base() { }
        public MyDebugException(string s) : base(s) { }
    }

    public struct Coordinates
    { //todo: Zamyslet se, zda nepouzit System.Drawing.Point
        public int x;
        public int y;
        public Coordinates(int x, int y) { this.x = x; this.y = y; }
    }

    public abstract class MapObjects
    {
        protected int x,y;
        protected readonly Model model;
        public Control control { get; set; } //picture
        virtual protected int value { get; set; } //todo: This must be abstract!!!

        public MapObjects(Model m) {
            this.model = m;
            model.money -= this.value;
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
        public abstract void Click();
        /// <summary>
        /// user action
        /// </summary>
        public abstract void Demolish();
        


    }

    public abstract class Amusements : MapObjects,IActionable
    {
        public int id { get; private set; }
        public AmusementEnterPath entrance { get; protected set; } //todo: je opravdu potreba protected, nestaci private nebo dokonce readonly?
        public AmusementExitPath exit { get; protected set; }
        public int capacity { get; protected set; }
        protected Queue<Person> queue;
        public bool hasEntranceExit { get; protected set; }
        public int waitingPeopleCount
        {
            get
            {
                if (queue != null) return queue.Count;
                else throw new MyDebugException("null v Amusements-queue. Opravdu ma tak byt?");
            }
            private set { }
        }

        public Amusements(Model m) :base(m)       
        {
            throw new NotImplementedException();   
        }
        public Amusements(int x, int y, Model m) :base(m)       
        {
            model.lastBuiltAmus = this;        
           
            model.mustBeEnter = true;
            this.id = model.amusList.ReturnFreeID();//mozna lepe nastavovat az pri pridani do listu
            model.amusList.Add(this);
            throw new NotImplementedException();   
        }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
        public override void Click()
        {
            throw new NotImplementedException();
        }
#warning opravdu abstract Action? Nemela by byt virtual a rovnou naimplementovana?
        public virtual void Action() { throw new NotImplementedException(); }
        public void ChangeId(int id) { this.id = id; }
        //hack: nezkontrolovano
        public abstract bool CheckFreeLocation(int x, int y);
        protected virtual bool CheckFreeLocation(int x, int y, byte width, byte height,bool hasEntranceAndExit=true) { 
            if (x + width > model.map.Length || y + height > model.map[0].Length) return false;
            for (int i = x; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (model.map[i][j] != 0) return false;
                }
            }
            if (hasEntranceAndExit) {
                bool free = false;;
                if (x - 1 > 0) for (int i = y; i < y + height; i++)
                        if (model.map[x - 1][i] == 0) { if (free) return free; else free = true; }
                if (x + 1 < model.map.Length) for (int i = y; i < y + height; i++)
                        if (model.map[x - 1][i] == 0) { if (free) return free; else free = true; }
                if (y - 1 > 0) for (int i = x; i < x + width; i++)
                        if (model.map[i][y - 1] == 0) { if (free) return free; else free = true; }
                if (y + 1 < model.map[0].Length) for (int i = y; i < y + height; i++)
                        if (model.map[i][y - 1] == 0) { if (free) return free; else free = true; }
                return free;
            }
            return true;       
        
        }
        /// <summary>
        /// checks if entrance can be built on this place and if yes -> create it
        /// </summary>
        /// <param name="x">an x coordinate in the inside-map</param>
        /// <param name="y">an x coordinate in the inside-map</param>
        /// <returns>if entrance was successful built on this point</returns>
        public bool CheckEntranceAndBuild(int x,int y) {
            if (IsInsideInAmusement(x - 1, y) ||
                IsInsideInAmusement(x + 1, y) ||
                IsInsideInAmusement(x, y - 1) ||
                IsInsideInAmusement(x, y + 1))
            {
                this.entrance = new AmusementEnterPath(model,x,y);
                model.mustBeEnter = false;
                model.mustBeExit = true;
                return true;
            }
            else return false;        
        }
        /// <summary>
        /// checks if exit can be built on this place and if yes -> create it
        /// </summary>
        /// <param name="x">an x coordinate in the inside-map</param>
        /// <param name="y">an x coordinate in the inside-map</param>
        /// <returns>if exit was successful built on this point</returns>
        public bool CheckExitAndBuild(int x, int y)
        {
            if (IsInsideInAmusement(x - 1, y) ||
                IsInsideInAmusement(x + 1, y) ||
                IsInsideInAmusement(x, y - 1) ||
                IsInsideInAmusement(x, y + 1))
            {
                this.exit = new AmusementExitPath(model, x, y);
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


    }
    public abstract class SquareAmusements : Amusements
    {
        public byte width { get; protected set; }
        public SquareAmusements(Model m) : base(m) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override bool CheckFreeLocation(int x, int y)
        {          
            return base.CheckFreeLocation(x, y, width, width, hasEntranceAndExit: true);
        }
        protected override bool IsInsideInAmusement(int x, int y)
        {
            if (x >= this.x && x < this.x + this.width &&
                y >= this.y && y < this.y + this.width)
                return true;
            else return false;       
        }

    }
    /// <summary>
    /// Class for rectangle, not square, amusements. It can have a different orientation.
    /// </summary>
    public abstract class RectangleAmusements : Amusements
    {
        byte width, height;
        public readonly bool isHorizontalOriented;
        public RectangleAmusements(Model m,bool isHorizontal=true):base(m)
        {
            isHorizontalOriented = isHorizontal;
        }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
           
        }
        public override bool CheckFreeLocation(int x, int y)
        {
            if (isHorizontalOriented) return CheckFreeLocation(x, y, width, height, hasEntranceAndExit: true);
            else return CheckFreeLocation(x,y,height,width,hasEntranceAndExit:true);
        }
        protected override bool IsInsideInAmusement(int x, int y)
        {
            if (x >= this.x && x < this.x + this.width &&
                y >= this.y && y < this.y + this.height)
                return true;
            else return false;
        }
        
        
      
    }

    public abstract class FreeShapedAmusements : Amusements
    {
        public FreeShapedAmusements(Model m) : base(m) { }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
    }



    public class Person : MapObjects,IActionable
    { //todo: Mozna sealed a nebo naopak moznost rozsiritelnosti dal...
        public int id { get; private set; }
        public Person(Model m) : base(m) { }
        public override void Click()
        {
            throw new NotImplementedException();
        }
        public override bool Create(int x, int y)
        {
            throw new NotImplementedException();
        }
        public override void Demolish()
        {
            throw new NotImplementedException();
        }
        public void Action()
        {
            throw new NotImplementedException();
        }


    }

    public abstract class Path : MapObjects
    {
        public Path(Model m, int x, int y) : base(m) { }
        public override bool Create(int x, int y) {
            throw new NotImplementedException();
        }

        public override void Click() {
            //nothing
        }
       
    }

    /// <summary>
    /// list of all amusements, at position i is an amusement with id==i
    /// </summary>
    public class ListOfAmusements
    { //todo: nejspis by mela byt thread-safe
        private List<Amusements> list;
#warning overit, ze v count je opravdu spravne
        public int count { get { return list.Count; } private set { } }
        public ListOfAmusements()
        {
            list = new List<Amusements>();
        }
        public void Add(Amusements a)
        {
            if (a.id == list.Count) list.Add(a);
            else throw new Exception("nesedi id a count v ListOfAmusements-Add()"); //todo: nemelo by se stavat, protoze by vzdy melo jit vytvorit jen jednu atrakci
        }
        public int ReturnFreeID()
        {
            return list.Count;
        }
        public void Remove(Amusements a)
        {
            Amusements b = list[list.Count - 1];
            b.ChangeId(a.id);
            list[a.id] = b;
            list.RemoveAt(list.Count - 1);
        }
        public void ForeachAction()
        {
            foreach (Amusements a in list)
            {
                a.Action();
            }
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
}
