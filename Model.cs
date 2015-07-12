using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LunaparkGame
{
    public class Model //todo: Mozna nahradit Evidenci modelem, aby se musela synchronizovat jen jedna vec
    {
        int width=10,height=15;
        public Queue<MapObjects> dirtyNew;
        public Queue<Control> dirtyDestruct;//pouze pbox.Destruct, mozna jen zadat souradnice
        public ListOfAmusements amusList = new ListOfAmusements();
        public PersonList persList = new PersonList();
        //todo: ostatni user akce - kliky a formy...
        public int[][] map;//todo: zvazit, zda ne jen bool
        
        public Model(int height, int width){
            this.height=height;
            this.width=width;  
            //---inicializace mapy - 0 prazdne, -1 chodnik, ostatni id atrakce
            map=new int[width][];
            for (int i = 0; i < width; i++) map[i] = new int[height];
            
            
        }
        //hack: nezkontrolovano
        public bool CheckFreeLocation(int x, int y, int width, int height, bool hasEntranceAndExit=true) {
            if (x + width > map.Length || y + height > map[0].Length) return false;
            for (int i = x; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (map[i][j] != 0) return false;
                }
            }
            if (hasEntranceAndExit) {
                bool free = false;;
                if (x - 1 > 0) for (int i = y; i < y + height; i++) 
                    if (map[x - 1][i] == 0) {if (free) return free; else free = true;}
                if (x + 1 < map.Length) for (int i = y; i < y + height; i++) 
                    if (map[x - 1][i] == 0) { if (free) return free; else free = true; }
                if (y - 1 > 0) for (int i = x; i < x+width; i++)
                        if (map[i][y-1] == 0) { if (free) return free; else free = true; }
                if (y + 1 < map[0].Length) for (int i = y; i < y + height; i++)
                        if (map[i][y - 1] == 0) { if (free) return free; else free = true; }
                return free;
            }
            return true;
        }
    }



}
