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
    public interface IUpdatable {
        void MyUpdate();
    }

    [Serializable]
    public class View2 {
        public GameRecords model { get; private set; }
        [NonSerialized]
        public MainForm form;
        [NonSerialized]
        List<IUpdatable> clickForms = new List<IUpdatable>(); 
        
       
        public View2(GameRecords m, MainForm form) {
            this.model = m;
            this.form = form;           
            form.PrepareFormsStartAppearance(model.currOfferedAmus, model.currOfferedPaths, model.currOfferedOthers, model.images);          
        }
        public void CallBeforeDeserialization(){
            if (clickForms != null) foreach (Control item in clickForms) item.Dispose();
        
        }
        [OnDeserialized]
        private void SetValuesAndCheckOnDeserialized(StreamingContext context) {
            if (clickForms != null) foreach (Control item in clickForms) item.Dispose();
            clickForms = new List<IUpdatable>();       
        }
        
        
        public void Action() {            
            ClickDirty();
            UpdateForms();
            form.Refresh();
        }
        private void UpdateForms() {
#warning Toto mozna casem spravit
            try {
                foreach (var item in clickForms) {
                    item.MyUpdate();
                }
            }
            catch { 
            //nothing
            }
        }
        /// <summary>
        /// Removes a detailed IUpdatable item (AmusementsDetailForm).
        /// </summary>
        /// <param name="form"></param>
        public void RemoveClickIUpdatable(IUpdatable form) {
            clickForms.Remove(form);       
        }
        private void ClickDirty() {
           
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
                        if(o.GetType() == typeof(Gate))  f = new GateDetailForm(model, (Gate)o, model.images[o.internTypeID]);
                        else f = new AmusementDetailForm(model, (Amusements)o, model.images[o.internTypeID], this);
                        form.ShowDockingForm(f);
                        clickForms.Add(f);
                    }
                }
            }

        }

    }

   
}
