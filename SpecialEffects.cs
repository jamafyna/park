using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace LunaparkGame
{
     [Serializable]
    public class SpecialEffects
    {
       //todo: Udelat uzitecne 

         private const int prize_NoMoreResearch = 20000;
         private const int prize_HighContentment = 5000;
         public const int maxPrizeCount = 2;
         private const int highContTimeInPark = 600;

        private double contentment;
        private int currHighContentmentTime = 0;
        private volatile bool contentmentAward;
        
        public int attractiveness = 0;
        public int maxAmusVariety=0;
        public int MaxAmusVariety { set { if (maxAmusVariety == 0) maxAmusVariety = value; } get { return maxAmusVariety; } }
        public int currAmusVariety = 0;
        public int currPrizeCount = 0;
        
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
        GameRecords model;

        delegate void RevealItemDelegate (MapObjectsFactory obj, System.Drawing.Image im);
        [NonSerialized]
         RevealItemDelegate rvDelegate;
         
        public SpecialEffects(GameRecords model, List<LaterShownItem> laterRevealedObjectsList) {
            this.model = model;
            propagateOn = false;
            researchOn = false;
            contentmentAward = false;
            propagation = 0;
            this.laterRevealedObjects = new ConcurrentQueue<LaterShownItem>(laterRevealedObjectsList);
            LaterShownItem l;
            this.laterRevealedObjects.TryDequeue(out l);
            timeToShowNewItem = l.timeToShow;
            followingItemToReveal = l.item;            
        }

        public void Action(MainForm form) {
            // ----- current contentment in the park -----
            if (!contentmentAward) {
                contentment = model.persList.contenment;
                if (contentment > 90) currHighContentmentTime++;
                if (currHighContentmentTime > highContTimeInPark) {
                    contentmentAward = true;
                    ProcessNewPrize(prize_HighContentment, PrizeNotices.highContentment);                   
                }
            }
            else currHighContentmentTime = 0;
                       
            // ----- propagation -----
            if (propagateOn) {
                propagation++; // is thread-safe because Action is called only from one thread
                model.MoneyAdd(- MainForm.propagatePrize);
            }
            else { propagation = Math.Max(0, propagation - 1) ;}
           // ----- researching -----
            if (researchOn) ResearchOnAction(form);
                

        }

        private void ResearchOnAction(MainForm form) {
            model.MoneyAdd(-MainForm.researchPrize);
            timeToShowNewItem--;
            if (timeToShowNewItem == 0) {
                MessageBox.Show(Notices.newRevealedItem + followingItemToReveal.name, Labels.gratulationMessBox, MessageBoxButtons.OK);
                object[] args = { followingItemToReveal, model.images[followingItemToReveal.internTypeId] };
                rvDelegate = new RevealItemDelegate(form.AddNewItemToForm);
                form.BeginInvoke(rvDelegate, args);
                LaterShownItem l;
                this.laterRevealedObjects.TryDequeue(out l);
                if (l != null) {
                    timeToShowNewItem = l.timeToShow;
                    followingItemToReveal = l.item;
                }
                else ProcessNewPrize(prize_NoMoreResearch, PrizeNotices.noMoreResearch);
            }
        }
        private void ProcessNewPrize(int money, string notice) {
            MessageBox.Show(notice + PrizeNotices.cashPrizeText + money, Labels.awardMessBox, MessageBoxButtons.OK);
            this.currPrizeCount++;
            model.MoneyAdd(money);
        }
        
    }
}
