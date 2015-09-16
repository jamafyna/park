using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace LunaparkGame
{
     [Serializable]
    public class SpecialEffects:IActionable
    {
       //todo: Udelat uzitecne + spravnou serializaci
        private double contentment;      
        public int attractiveness = 0;
        public int maxAmusVariety=0;
        public int MaxAmusVariety { set { if (maxAmusVariety == 0) maxAmusVariety = value; } get { return maxAmusVariety; } }
        public int currAmusVariety = 0;
        public int awardsCount;
        public const int maxAwardsCount = 5;
        public bool propagateOn;
        public bool researchOn;
        /// <summary>
        /// Represents park advertising, is not thread safe -> for manipulating use atomic operations
        /// </summary>
        public int propagation { get; private set; }
        /// <summary>
        /// Represents researching, is not thread safe -> for manipulating use atomic operations
        /// </summary>
        private int timeToShowNewItem;
        private MapObjectsFactory followingItemToReveal;
        ConcurrentQueue<LaterShownItem> laterRevealedObjects;
       
        Model model;
        MainForm form;
        //delegate
         
         public SpecialEffects(Model model, List<LaterShownItem> laterRevealedObjectsList, MainForm form) {
            this.model = model;
            this.form = form;
            propagateOn = false;
            researchOn = false;
            propagation = 0;
            this.laterRevealedObjects = new ConcurrentQueue<LaterShownItem>(laterRevealedObjectsList);
            LaterShownItem l;
            this.laterRevealedObjects.TryDequeue(out l);
            timeToShowNewItem = l.timeToShow;
            followingItemToReveal = l.item;            
        }

        public void Action() {
            contentment = model.persList.contenment;
            // ----- propagation -----
            if (propagateOn) {
                propagation++; // is thread-safe because Action is called only from one thread
                model.MoneyAdd(- MainForm.propagatePrize);
            }
            else { propagation = Math.Max(0, propagation - 1) ;}
           // ----- researching -----
            if (researchOn) {
                model.MoneyAdd(- MainForm.researchPrize);
                timeToShowNewItem--;
                if (timeToShowNewItem == 0) {
                  //  object[] args = {}
                   // form.BeginInvoke(AddNewItemToForm, args);
                }
                 // public void AddNewItemToForm(MapObjectsFactory item, Image im) {
            }

        }
        
       
        
    }
}
