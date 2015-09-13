using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using WeifenLuo.WinFormsUI.Docking;

namespace LunaparkGame {

    public class View2 {
        Model model;
        MainForm form;
        List<IUpdatable> forms = new List<IUpdatable>();
        public Image[] images;
        DockPanel dockPanel;
        public AmusementsForm amusform;
        public PathForm pathform;
        public AccessoriesForm accform;

        public View2(Model m, MainForm form, DockPanel mainDockPanel, AmusementsForm amform, PathForm pform, AccessoriesForm oform) {
            Image[] im={Properties.Images.gate, Properties.Images.enter, Properties.Images.exit} ;
            Data data = new Data(im);
            LoadExternalData(data);
            images = data.GetImages();          
            this.model = m;
            this.form = form;
            this.dockPanel = mainDockPanel;
            this.amusform = amform;
            this.pathform = pform;
            this.accform = oform;
            model.CreateCurrBuildedItems(data.GetItemsCount());
                    
            foreach (var item in data.initialAmus) amusform.CreateNewItem(images[item.internTypeId], item);
            foreach (var item in data.initialPaths) pathform.CreateNewItem(images[item.internTypeId], item);
            foreach (var item in data.initialOthers) accform.CreateNewItem(images[item.internTypeId], item);
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
            foreach (var item in forms) {
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
                    forms.Add(new PersonUserControl((Person)o, form.mapform.map));
                }
                if (o is Amusements) {
                    if (o.isClicked) {
                        foreach (Form f in forms) {
                            if (f is AmusementDetailForm  && ((AmusementDetailForm)f).a == o) { f.Activate(); break; }
                        }
                    }
                    else {
                        AmusementDetailForm f;
                        if(o.GetType() == typeof(Gate))  f = new GateDetailForm(model, (Gate)o, images[o.internTypeID]);
                        else f = new AmusementDetailForm(model, (Amusements)o, images[o.internTypeID]);
                        f.Show(dockPanel);
                        forms.Add(f);
                    }
                }
            }

        }

    }
}
