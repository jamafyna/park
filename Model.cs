using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LunaparkGame
{
    public class Model //todo: Mozna nahradit Evidenci modelem, aby se musela synchronizovat jen jedna vec
    {
        
        public Queue<MapObjects> dirtyNew;
        public Queue<Control> dirtyDestruct;//pouze pbox.Destruct, mozna jen zadat souradnice
        public ListOfAmusements amusList = new ListOfAmusements();
        public PersonList persList = new PersonList();
        //todo: ostatni user akce - kliky a formy...


    }



}
