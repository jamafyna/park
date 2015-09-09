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
        Image[] images;
        DockPanel dockPanel;

        public AmusementsForm amusform;
        public PathForm pathform;
        public AccessoriesForm accform;


        public View2(Model m, MainForm form, DockPanel mainDockPanel, AmusementsForm amform, PathForm pform, AccessoriesForm oform, Data data) {
            this.model = m;
            this.form = form;
            this.dockPanel = mainDockPanel;
            this.amusform = amform;
            this.pathform = pform;
            this.accform = oform;

            this.images = data.GetImages();
            foreach (var item in data.initialAmus) amusform.CreateNewItem(images[item.internTypeId], item);
            foreach (var item in data.initialPaths) pathform.CreateNewItem(images[item.internTypeId], item);
            foreach (var item in data.initialOthers) accform.CreateNewItem(images[item.internTypeId], item);
            data = null; // due to GC
        }
        public void Action() {
            // DestructDirty();
            //NewDirty();
            //PeopleMove();
            //ClickDirty();
            //UpdateDirty();
            //throw new NotImplementedException();
        }

        public void Click(object sender, EventArgs e) {
            MouseEventArgs mouse = (MouseEventArgs)e;
            int realX = mouse.X;
            int realY = mouse.Y;
            byte mapX = (byte)(mouse.X / MainForm.sizeOfSquare);
            byte mapY = (byte)(mouse.Y / MainForm.sizeOfSquare);
            MapObjects obj = FindObject(realX, realY, mapX, mapY);
            if (obj != null) obj.Click(sender, e);
            else if (mapX != 0 && mapX != model.realWidth - 1 && mapY != 0 && mapY != model.realHeight - 1) { // border
                MapClick(mapX, mapY);
            }
        }
        private void MapClick(byte mapX, byte mapY) {
            if (!model.demolishOn && (model.LastClick != null || model.mustBeEnter || model.mustBeExit)) {
                #region
                //todo: kontrola na co vse mohl uzivatel kliknout, nejspise poslat udalost do modelu spolu se souradnicemi
                //todo: kontrola, zda neni neco rozestavene - mysleno pro free-shaped atrakce
                if (model.mustBeEnter) {
                    model.LastBuiltAmus.CheckEntranceAndBuild(mapX, mapY);
                    return;
                }
                if (model.mustBeExit) {
                    model.LastBuiltAmus.CheckExitAndBuild(mapX, mapY); //if not succeed, no annoing error-text                  
                    return;
                }
                if (model.LastClick.prize > model.GetMoney()) {
                    MessageBox.Show(Notices.cannotBuyNoMoney, Labels.warningMessBox, MessageBoxButtons.OK);
                    return;
                }
                if (model.LastClick.CanBeBuild(mapX, mapY, model)) {
                    model.LastClick.Build(mapX, mapY, model);
                    if (model.LastClick is AmusementsFactory ||
                        model.LastClick.GetType() == typeof(AmusementExitPathFactory) ||
                        model.LastClick.GetType() == typeof(AmusementEnterPathFactory)
                        ) model.SetNullToLastClick();
                }
            }
                #endregion


        }
        private MapObjects FindObject(int realX, int realY, byte mapX, byte mapY) {
            MapObjects obj = null;
            if ((obj = model.maps.GetAmusement(mapX, mapY)) != null) return obj;
            foreach (Person p in model.persList) {
                if (p.IsInside(realX, realY)) return p;
            }
            if ((obj = model.maps.GetPath(mapX, mapY)) != null) return obj;
            return null;
        }

        private void PeopleMove() {
#warning nebude se takto vubec pouzivat
            foreach (Person p in model.persList) {

                if (p.control != null) {
                    p.control.Location = p.GetRealCoordinatesUnsynchronized();
                    p.control.Visible = p.visible;
                }



            }
        }


        private void UpdateDirty() {
            foreach (var item in forms) {
                item.MyUpdate();
            }

        }
    }
}
