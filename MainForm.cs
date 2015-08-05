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
    public partial class MainForm : Form, IUpdatable
    {
        //public Type lastClick{get; private set;}
        /// <summary>
        /// contains an unique instance for each type
        /// </summary>
        
       
        public const int sizeOfSquare = 40;
        AmusementsForm amusform;
        PathForm pathform;
        public Control map;
        Model model;
        View view;
        MapForm mapform;
        int timerTime = 0;
        
          
        public MainForm(byte playingWidth, byte playingHeight)
        {
            InitializeComponent();
            IsMdiContainer = true;
            model = new Model(playingHeight,playingWidth);
            view = new View(model,this);
            mapform = new MapForm(model, view, playingWidth, playingHeight);
            amusform = new AmusementsForm(model, mainDockPanel, amusementsToolStripMenuItem);
            pathform = new PathForm(model, pathToolStripMenuItem);
            amusform.Show(mainDockPanel);
            pathform.Show(mainDockPanel);
            mapform.Show(mainDockPanel);
            timer.Enabled = true;

            
        }

      
      
       
        private void MyInitialize() {        
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void amusementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (amusementsToolStripMenuItem.Checked)
            {
                amusform.Hide();
                amusementsToolStripMenuItem.Checked = false;
            }
            else
            {
                amusform.Show(mainDockPanel);
                amusementsToolStripMenuItem.Checked = true;
            } 
        }

        private void pathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pathToolStripMenuItem.Checked)
            {
                pathform.Hide();
                pathToolStripMenuItem.Checked = false;
            }
            else
            {
                pathform.Show(mainDockPanel);
                pathToolStripMenuItem.Checked = true;
            } 
        }
        private void showHideForm(ToolStripMenuItem itemA, ref Form formA) {
            if (itemA.Checked)
            {
                formA.Hide();
                itemA.Checked = false;
            }
            else
            {
                formA.Show(mainDockPanel);
                itemA.Checked = true;
            } 
        }

        private void timer_Tick(object sender, EventArgs e){
        
           #warning work with Person in view only when no another thread is running
            //todo: rozvrstvit do vlaken
            //---actions
            Task.Factory.StartNew(model.persList.Action);
           
           // model.persList.Action();
            if (timerTime >= 10)
            {
                Task.Factory.StartNew(model.amusList.Action).Wait();
                Task.Factory.StartNew(model.maps.Action).Wait();
                
                timerTime = 0;
             //   model.amusList.Action();
            //    model.effects.Action();
             //    model.maps.Action();
                
            }
           
            //---visual
            //model.dirtyClick
            Task.WaitAll();
            view.Action();
            MyUpdate();
            timerTime++;
        }
       
        public void MyUpdate() {
            this.moneyCount_toolStripMenuItem.Text = model.GetMoney().ToString();
            this.peopleCount_toolStripMenuItem.Text = model.CurrPeopleCount.ToString();
        }

        private void demolish_toolStripMenuItem_Click(object sender, EventArgs e) {
            if (model.demolishOn) {
                model.demolishOn = false;
                demolish_toolStripMenuItem.Text = Labels.demolishStart;
                demolish_toolStripMenuItem.ForeColor = Color.Black;
            }
            else {
                model.demolishOn = true;
                demolish_toolStripMenuItem.Text = Labels.demolishing;
                demolish_toolStripMenuItem.ForeColor = Color.Red;
            
            }
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
        Control map;

        public MapForm(Model m, View view, byte playingWidth, byte playingHeight) {
            InitializeComponent();
            model = m;
            map = view.CreateVisualMap(playingWidth + 2, playingHeight + 2, MainForm.sizeOfSquare);
            map.Parent = this;
            this.map.Click += new System.EventHandler(this.map_Click);
            view.CreateGate(model.gate);
        }

        private void MapForm_Load(object sender, EventArgs e) {

        }
        private void map_Click(object sender, EventArgs e) {

            if (!model.demolishOn && model.LastClick != null) {
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
                if (model.LastClick.prize > model.GetMoney()) { MessageBox.Show(Notices.cannotBuyNoMoney, Labels.warningMessBox, MessageBoxButtons.OK); return; }
                if (model.LastClick.CanBeBuild(x, y)) model.LastClick.Build(x, y);
                /* if (model.LastClick is Amusements) {
                    if (((Amusements)model.LastClick).CheckFreeLocation(x, y)) {
                        object[] arg = { model, new Coordinates(x, y) };
                        //todo:nize nejspise neni nutne ukladat, udelano v konstruktoru atrakce a nastavovat 
                        model.LastBuiltAmus = (Amusements)Activator.CreateInstance(model.LastClick.GetType(), arg);  //todo: melo by se vytvorit v novem vlakne                                          
                    }
                    return;
                }
                else {
                    object[] arg = { model, new Coordinates(x, y) };
                    Activator.CreateInstance(model.LastClick.GetType(), arg);
                }*/
                #endregion
#warning rusim tu schopnost prekladace spravne kontrolovat - nezajisti mi, ze dana trida bude mit spravny konstruktor
            }

        }
        
        
    }
}
