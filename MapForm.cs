using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LunaparkGame {
    public partial class MapForm : WeifenLuo.WinFormsUI.Docking.DockContent {
        Model model;
        public readonly Control map;

        public MapForm(Model m, View2 view, byte playingWidth, byte playingHeight) {
            InitializeComponent();
            model = m;
            map = new mapCustomControl(model, view);
            map.Parent = this;
           

        }

        public void InitializeAfterDeserialization(Model m, View2 v) {
            model = m;
            ((mapCustomControl)map).InitializeAfterDeserialization(m, v);

        }
        private void MapForm_Load(object sender, EventArgs e) {

        }
        private void map_Click(object sender, EventArgs e) {

            if (!model.demolishOn && (model.LastClick != null || model.mustBeEnter || model.mustBeExit)) {
                #region
                MouseEventArgs mys = (MouseEventArgs)e;
                byte x = (byte)(mys.X / MainForm.sizeOfSquare);
                byte y = (byte)(mys.Y / MainForm.sizeOfSquare);

                //todo: kontrola na co vse mohl uzivatel kliknout, nejspise poslat udalost do modelu spolu se souradnicemi
                //todo: kontrola, zda neni neco rozestavene - mysleno pro free-shaped atrakce
                if (model.mustBeEnter) {
                    model.LastBuiltAmus.CheckEntranceAndBuild(x, y);
                    return;
                }
                if (model.mustBeExit) {
                    model.LastBuiltAmus.CheckExitAndBuild(x, y); //if not succeed, no annoing error-text                  
                    return;
                }
                if (model.LastClick.prize > model.GetMoney()) {
                    MessageBox.Show(Notices.cannotBuyNoMoney, Labels.warningMessBox, MessageBoxButtons.OK);
                    return;
                }
                if (model.LastClick.CanBeBuild(x, y, model)) {
                    model.LastClick.Build(x, y, model);
                    if (model.LastClick is AmusementsFactory ||
                        model.LastClick.GetType() == typeof(AmusementExitPathFactory) ||
                        model.LastClick.GetType() == typeof(AmusementEnterPathFactory)
                        ) model.SetNullToLastClick();
                }
                #endregion

            }

        }
    }
}
