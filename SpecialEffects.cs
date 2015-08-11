using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LunaparkGame
{
    public class SpecialEffects:IActionable
    {
       //todo: propagace a research nejspis dat do modelu nebo do FORm, sem davat ruzne kontroly na pocet lidi, pocet atrakci..., tj. o zviditelneni dalsi polozky se stara nekdo jiny (idealne hlavni form)
        public int contentment = 100;
        public int attractiveness = 0;
        public int propagate = 0;
        public int research = 0;
        public Queue<int> newItemWaitingTime { get; private set; }
        private int[] newItemWTArray = { 300 , 200};
        public SpecialEffects() {
            this.newItemWaitingTime = new Queue<int>(newItemWTArray);
        }
        public void Action() {
        //todo 1x za min projit vsechny lidi a vzit prumer jejich spokojenosti
        
        }
       
        
    }
}
