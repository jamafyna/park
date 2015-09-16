using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using WeifenLuo.WinFormsUI.Docking;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.IO;

namespace LunaparkGame {
    [Serializable]
    public class View2 {
        public Model model { get; private set; }
        [NonSerialized]
        public MainForm form;
        [NonSerialized]
        List<IUpdatable> clickForms = new List<IUpdatable>(); 
        public readonly Image[] images;
       
        public ConcurrentBag<AmusementsFactory> currOfferedAmus;
        public ConcurrentBag<PathFactory> currOfferedPaths;
        public ConcurrentBag<MapObjectsFactory> currOfferedOthers;
     //   public ConcurrentQueue<AmusementsFactory> notOfferedAmus;
     //   public ConcurrentQueue<PathFactory> notOfferedPaths;
     //   public ConcurrentQueue<MapObjectsFactory> notOfferedOthers;
        public ConcurrentQueue<LaterShownItem> laterAddedObjects;


        public View2(Model m, MainForm form) {
            this.model = m;
            this.form = form;  
            
            Image[] im = {Properties.Images.gate, Properties.Images.enter, Properties.Images.exit} ;
            Data data = new Data(im);
            LoadExternalData(data);
            images = data.GetImages();                              
            model.InitializeCurrBuildedItems(data.GetItemsCount());
            //form.PrepareFormsStartAppearance(data.initialAmus, data.initialPaths, data.initialOthers, images);
            form.PrepareFormsStartAppearance(currOfferedAmus, currOfferedPaths, currOfferedOthers, images);
           
            data = null; // due to GC
        }
        public void CallBeforeDeserialization(){
            if (clickForms != null) foreach (Control item in clickForms) item.Dispose();
        
        }
        private void LoadExternalData(Data data) {
            System.IO.StreamReader sr = new System.IO.StreamReader("amusements.txt");
            // System.IO.StreamReader sr = new System.IO.StreamReader("amusementsInitial.txt");   
            StreamReader srAmus = new StreamReader("amusements.txt");
            StreamReader srPath = new StreamReader("paths.txt");
            StreamReader srAddition = new StreamReader("additionRules.txt");
            data.LoadAll(srAmus, srPath, null, srAddition);
            srAmus.Close();
            srPath.Close();
            srAddition.Close();
            /* data.LoadAmus(sr);
            sr.Close();
            sr = new System.IO.StreamReader("paths.txt");
            data.LoadPaths(sr);
            sr.Close();    */
            currOfferedAmus = new ConcurrentBag<AmusementsFactory>(data.initialAmus);
            currOfferedPaths = new ConcurrentBag<PathFactory>(data.initialPaths);
            currOfferedOthers = new ConcurrentBag<MapObjectsFactory>(data.initialOthers);
            /*  notOfferedAmus = new ConcurrentQueue<AmusementsFactory>(data.otherAmus);
              notOfferedPaths = new ConcurrentQueue<PathFactory>(data.otherPaths);
              notOfferedOthers = new ConcurrentQueue<MapObjectsFactory>(data.otherOthers);*/
            laterAddedObjects = new ConcurrentQueue<LaterShownItem>(data.laterShowedItems);
        }
        [OnDeserialized]
        private void SetValuesAndCheckOnDeserialized(StreamingContext context) {
            if (clickForms != null) foreach (Control item in clickForms) item.Dispose();
            clickForms = new List<IUpdatable>();       
        }
        
        
        public void Action() {
            // DestructDirty();
            //NewDirty();
            //PeopleMove();
            ClickDirty();
            //UpdateDirty();
            //throw new NotImplementedException();
            UpdateForms();
            form.Refresh();
        }
        private void UpdateForms() {
            foreach (var item in clickForms) {
                item.MyUpdate();
            }
        }
        private void ClickDirty() {
            //if(.GetType()==typeof(Gate)) mozna, mozna staci amus
            //else if(is Amusements) atd.
            //nezapomenout zmenit spravne click v danem objektu
#warning pouze provizorni
            MapObjects o;
            while (model.dirtyClick.TryDequeue(out o)) {
                if (o is Person) {
                    clickForms.Add(new PersonUserControl((Person)o, form.mapform.map));
                }
                if (o is Amusements) {
                    if (o.isClicked) {
                        foreach (var f in clickForms) {
                            if (f is AmusementDetailForm  && ((AmusementDetailForm)f).a == o) { 
                                ((AmusementDetailForm)f).Activate();
                                 break; }
                        }
                    }
                    else {
                        AmusementDetailForm f;
                        if(o.GetType() == typeof(Gate))  f = new GateDetailForm(model, (Gate)o, images[o.internTypeID]);
                        else f = new AmusementDetailForm(model, (Amusements)o, images[o.internTypeID]);
                        form.ShowDockingForm(f);
                        clickForms.Add(f);
                    }
                }
            }

        }

    }
}
