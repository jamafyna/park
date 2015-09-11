using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace LunaparkGame
{
    public partial class MainForm : Form, IUpdatable {
        
        public const int sizeOfSquare = 40;
        private const int propagatePrize = 10;
        private const int researchPrize = 10;

        Model model;
        View2 view;
        public readonly MapForm mapform;
        int timerTicks = 0;


        public MainForm(byte playingWidth, byte playingHeight) {
            InitializeComponent();
            IsMdiContainer = true;
            model = new Model(playingHeight, playingWidth);

            AmusementsForm amusform = new AmusementsForm(model, mainDockPanel, amusementsToolStripMenuItem);
            PathForm pathform = new PathForm(model, pathToolStripMenuItem);
            AccessoriesForm otherform = new AccessoriesForm(model, accessoriesToolStripMenuItem);
            view = new View2(model, this, mainDockPanel, amusform, pathform, otherform);
            mapform = new MapForm(model, view, playingWidth, playingHeight);

            amusform.Show(mainDockPanel);
            pathform.Show(mainDockPanel);
            accessoriesToolStripMenuItem.Checked = false;
            mapform.Show(mainDockPanel);
            timer.Enabled = true;

        }

        private void MyInitialize() {
        }

        private void MainForm_Load(object sender, EventArgs e) {

        }

        private void amusementsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (amusementsToolStripMenuItem.Checked) {
                view.amusform.Hide();
                amusementsToolStripMenuItem.Checked = false;
            }
            else {
                view.amusform.Show(mainDockPanel);
                amusementsToolStripMenuItem.Checked = true;
            }
        }

        private void pathToolStripMenuItem_Click(object sender, EventArgs e) {
            if (pathToolStripMenuItem.Checked) {
                view.pathform.Hide();
                pathToolStripMenuItem.Checked = false;
            }
            else {
                view.pathform.Show(mainDockPanel);
                pathToolStripMenuItem.Checked = true;
            }
        }
        
        private void accessoriesToolStripMenuItem_Click(object sender, EventArgs e) {
            if (accessoriesToolStripMenuItem.Checked) {
                view.accform.Hide();
                accessoriesToolStripMenuItem.Checked = false;
            }
            else {
                view.accform.Show(mainDockPanel);
                accessoriesToolStripMenuItem.Checked = true;
            }
        }

        private void timer_Tick(object sender, EventArgs e) {

#warning work with Person in view only when no another thread is running
            //todo: rozvrstvit do vlaken
            //---actions
            Task.Factory.StartNew(model.persList.Action);

            // model.persList.Action();
            if (timerTicks >= 10) {
                Task.Factory.StartNew(model.amusList.Action);
                Task.Factory.StartNew(model.maps.Action);

                timerTicks = 0;
                //   model.amusList.Action();
                //    model.effects.Action();
                //    model.maps.Action();
                if (model.propagateOn) {
                    Interlocked.Increment(ref model.propagation);
                    model.MoneyAdd(- propagatePrize);
                }
                if (model.researchOn) {
                    model.MoneyAdd(- researchPrize);
                    Interlocked.Decrement(ref model.timeToShowNewItem);
                }

            }

            //---visual
            //model.dirtyClick
            Task.WaitAll();
            view.Action();
            MyUpdate();
            timerTicks++;
        }

        public void MyUpdate() {
            this.moneyCount_toolStripMenuItem.Text = model.GetMoney().ToString();
            this.peopleCount_toolStripMenuItem.Text = model.CurrPeopleCount.ToString();
            if (model.demolishOn) {
                demolish_toolStripMenuItem.Text = Labels.demolishing;
                demolish_toolStripMenuItem.ForeColor = Color.Red;
            }
            else {
                demolish_toolStripMenuItem.Text = Labels.demolishStart;
                demolish_toolStripMenuItem.ForeColor = Color.Black;
            }
        }

        private void demolish_toolStripMenuItem_Click(object sender, EventArgs e) {
          /*  if (model.demolishOn) {
                model.demolishOn = false;
                demolish_toolStripMenuItem.Text = Labels.demolishStart;
                demolish_toolStripMenuItem.ForeColor = Color.Black;
            }
            else {
                model.demolishOn = true;
                demolish_toolStripMenuItem.Text = Labels.demolishing;
                demolish_toolStripMenuItem.ForeColor = Color.Red;

            }*/
            model.demolishOn = !model.demolishOn;
        }

        private void propagate_toolStripMenuItem_Click(object sender, EventArgs e) {
            if (model.propagateOn) {
                model.propagateOn = false;
                propagate_toolStripMenuItem.Text = Labels.advertiseStart;
                propagate_toolStripMenuItem.ForeColor = Color.Black;
            }
            else {
                model.propagateOn = true;
                propagate_toolStripMenuItem.Text = Labels.advertising;
                propagate_toolStripMenuItem.ForeColor = Color.DarkGreen;
            }
        }

        private void research_toolStripMenuItem_Click(object sender, EventArgs e) {
            if (model.researchOn) {
                model.researchOn = false;
                research_toolStripMenuItem.Text = Labels.researchStart;
                research_toolStripMenuItem.ForeColor = Color.Black;
            }
            else {
                model.researchOn = true;
                research_toolStripMenuItem.Text = Labels.researching;
                research_toolStripMenuItem.ForeColor = Color.DarkGreen;

            }
        }

        
    }   
    public partial class MapForm : WeifenLuo.WinFormsUI.Docking.DockContent {
        Model model;
        public readonly Control map;
        
        public MapForm(Model m, View2 view, byte playingWidth, byte playingHeight) {
            InitializeComponent();
            model = m;
            map = new mapCustomControl(model, view);
            map.Parent = this;
           //hash: map = view.CreateVisualMap(playingWidth + 2, playingHeight + 2, MainForm.sizeOfSquare);
          //hash:  map.Parent = this;
          //hash:  this.map.Click += new System.EventHandler(this.map_Click);
          //hash:  view.CreateGate(model.gate);
            
        }

        private void MapForm_Load(object sender, EventArgs e) {

        }
        private void map_Click(object sender, EventArgs e) {

            if (!model.demolishOn && (model.LastClick != null || model.mustBeEnter || model.mustBeExit)) {
                #region
                MouseEventArgs mys = (MouseEventArgs)e;
                // byte x = (byte)(mys.X - mys.X % sizeOfSquare);
                // byte y = (byte)(mys.Y - mys.Y % sizeOfSquare);
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
