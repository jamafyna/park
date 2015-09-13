using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LunaparkGame
{
     [Serializable]
    public class SpecialEffects:IActionable
    {
       //todo: Udelat uzitecne + spravnou serializaci
         //todo: propagace a research nejspis dat do modelu nebo do FORm, sem davat ruzne kontroly na pocet lidi, pocet atrakci..., tj. o zviditelneni dalsi polozky se stara nekdo jiny (idealne hlavni form)
        private double contentment;
        private int research;        
        public int attractiveness = 0;
        public int maxAmusVariety=0;
        public int MaxAmusVariety { set { if (maxAmusVariety == 0) maxAmusVariety = value; } get { return maxAmusVariety; } }
        public int currAmusVariety = 0;
        public int awardsCount;
        public const int maxAwardsCount = 5;

        public Queue<int> newItemWaitingTime { get; private set; }
        Model model;
        private int[] newItemWTArray = { 300 , 200};
        public SpecialEffects(Model model) {
            this.newItemWaitingTime = new Queue<int>(newItemWTArray);
            this.model = model;
        }
        public void Action() {
        //todo 1x za min projit vsechny lidi a vzit prumer jejich spokojenosti
            contentment = model.persList.contenment;
        }
        
       
        
    }
}
