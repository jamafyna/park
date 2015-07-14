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
        public ListOfAmusements amusList = new ListOfAmusements();
        public PersonList persList = new PersonList();
        //todo: ostatni user akce - kliky a formy...
        public int[][] map;//todo: zvazit, zda ne jen bool
        public Amusements lastBuiltAmus;
        public MapObjects lastClick { set; get; }
        public bool mustBeEnter = false;
        public bool mustBeExit = false;
        public bool demolishOn = false;
        public int money;
        
        
        public Model(int height, int width){
            this.height=height;
            this.width=width;
            money = initialMoney;
            //---inicializace mapy - 0 prazdne, -1 chodnik, ostatni id atrakce
            map=new int[width][];
            for (int i = 0; i < width; i++) map[i] = new int[height];
            
            
        }
       
    }



}
