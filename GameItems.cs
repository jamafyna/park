using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LunaparkGame
{
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
        public Control control { get; set; }
        public int value { get; protected set; }
        /// <summary>
        /// Create an instance and show it in the game map
        /// </summary>
        /// <param name="x">the left coordinate</param>
        /// <param name="y">the top coordinate</param>
        public abstract void Create(int x, int y); //todo mozna system.drawing.point
        /// <summary>
        /// user action
        /// </summary>
        public abstract void Click();
        /// <summary>
        /// user action
        /// </summary>
        public abstract void Demolish();



    }

    public abstract class Amusements : MapObjects
    {
        public int id { get; private set; }
       
        public Coordinates entrance { get; protected set; } //todo: je opravdu potreba protected, nestaci private nebo dokonce readonly?
        public Coordinates exit { get; protected set; }
        public int capacity { get; protected set; }
        protected Queue<Person> queue;

        public int waitingPeopleCount
        {
            get
            {
                if (queue != null) return queue.Count;
                else throw new MyDebugException("null v Amusements-queue. Opravdu ma tak byt?");
            }
            private set { }
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


        /// <summary>
        /// create an Item in AtrakceForm and set it (e.g. set visible=false)
        /// </summary>
        //  public abstract static void Initialize();


    }
    public abstract class SquareAmusements : Amusements
    {
        public override void Create(int x, int y)
        {
            throw new NotImplementedException();
        }


    }
    /// <summary>
    /// Class for rectangle, not square, amusements. It can have a different orientation.
    /// </summary>
    public abstract class RectangleAmusements : Amusements
    {
        public readonly bool isHorizontalOriented;
        public RectangleAmusements(bool isHorizontal)
        {
            isHorizontalOriented = isHorizontal;
        }

    }

    public abstract class FreeShapedAmusements : Amusements
    {
        public override void Create(int x, int y)
        {
            throw new NotImplementedException();
        }
    }



    public class Person : MapObjects
    { //todo: Mozna sealed a nebo naopak moznost rozsiritelnosti dal...
        public int id { get; private set; }
        public override void Click()
        {
            throw new NotImplementedException();
        }
        public override void Create(int x, int y)
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
        public override void Create(int x, int y) { }

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
