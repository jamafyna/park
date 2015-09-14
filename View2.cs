using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using WeifenLuo.WinFormsUI.Docking;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

namespace LunaparkGame {
    [Serializable]
    public class View2 {
        public Model model { get; private set; }
        [NonSerialized]
        public MainForm form;
        [NonSerialized]
        List<IUpdatable> clickForms = new List<IUpdatable>(); 
        public Image[] images;
       
        public ConcurrentBag<AmusementsFactory> currOfferedAmus;
        public ConcurrentBag<PathFactory> currOfferedPaths;
        public ConcurrentBag<MapObjectsFactory> currOfferedOthers;
        public ConcurrentQueue<AmusementsFactory> notOfferedAmus;
        public ConcurrentQueue<PathFactory> notOfferedPaths;
        public ConcurrentQueue<MapObjectsFactory> notOfferedOthers;


        public View2(Model m, MainForm form) {
            Image[] im = {Properties.Images.gate, Properties.Images.enter, Properties.Images.exit} ;
            Data data = new Data(im);
            LoadExternalData(data);
            images = data.GetImages();          
            this.model = m;
            this.form = form;
            
            model.InitializeCurrBuildedItems(data.GetItemsCount());

            currOfferedAmus = new ConcurrentBag<AmusementsFactory>(data.initialAmus);
            currOfferedPaths = new ConcurrentBag<PathFactory>(data.initialPaths);
            currOfferedOthers = new ConcurrentBag<MapObjectsFactory>(data.initialOthers);
            notOfferedAmus = new ConcurrentQueue<AmusementsFactory>(data.otherAmus);
            notOfferedPaths = new ConcurrentQueue<PathFactory>(data.otherPaths);
            notOfferedOthers = new ConcurrentQueue<MapObjectsFactory>(data.otherOthers);

            form.PrepareFormsStartAppearance(data.initialAmus, data.initialPaths, data.initialOthers, images);
           /* foreach (var item in data.initialAmus) form.amusform.CreateNewItem(images[item.internTypeId], item);
            foreach (var item in data.initialPaths) form.pathform.CreateNewItem(images[item.internTypeId], item);
            foreach (var item in data.initialOthers) form.accform.CreateNewItem(images[item.internTypeId], item);*/
            
            data = null; // due to GC
        }
        private void LoadExternalData(Data data) {
            System.IO.StreamReader sr = new System.IO.StreamReader("amusements.txt");
            // System.IO.StreamReader sr = new System.IO.StreamReader("amusementsInitial.txt");   
            data.LoadAmus(sr);
            sr.Close();
            sr = new System.IO.StreamReader("paths.txt");
            data.LoadPaths(sr);
            sr.Close();       
        }
        [OnDeserialized]
        private void SetValuesAndCheckOnDeserialized(StreamingContext context) {
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
                        foreach (Form f in clickForms) {
                            if (f is AmusementDetailForm  && ((AmusementDetailForm)f).a == o) { f.Activate(); break; }
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
