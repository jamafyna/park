﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Serialization;

namespace LunaparkGame
{
    
    public partial class MainForm : Form, IUpdatable, ISerializable {
        
        public const int sizeOfSquare = 40;
        public const int propagatePrize = 10;
        public const int researchPrize = 10;

        private byte playingWidth, playingHeight;

        volatile GameRecords model;
        volatile View2 view;
        private StartForm startForm;
        public MapForm mapform;
        private AmusementsForm amusform;
        private PathForm pathform;
        private AccessoriesForm accform;
        int timerTicks = 0;
        long debugtime = 0;
        Task[] tasks;

        public MainForm(byte playingWidth, byte playingHeight, StartForm startForm) {
            InitializeComponent();
            InitializeTasks();
            IsMdiContainer = true;
            this.playingHeight = playingHeight;
            this.playingWidth = playingWidth;
            this.startForm = startForm;

            model = new GameRecords(playingHeight, playingWidth, startForm.initialAmusementsFilename, startForm.initialPathsFilename, startForm.initialAccessoriesFilename, startForm.revealingRulesFilename);

            amusform = new AmusementsForm(model, mainDockPanel, amusementsToolStripMenuItem);
            pathform = new PathForm(model, pathToolStripMenuItem);
            accform = new AccessoriesForm(model, accessoriesToolStripMenuItem);
            view = new View2(model, this);
            mapform = new MapForm(model, view, playingWidth, playingHeight);

            amusform.Show(mainDockPanel);
            pathform.Show(mainDockPanel);
            accessoriesToolStripMenuItem.Checked = false;
            mapform.Show(mainDockPanel);
            timer.Enabled = true;
           
            
        }
        public MainForm(SerializationInfo si, StreamingContext sc) {                        
//todo: odstranit button1
            model = (GameRecords)si.GetValue("model", typeof(GameRecords));
            view = (View2)si.GetValue("view", typeof(View2));
            view.form = this;
            playingHeight = si.GetByte("height");
            playingWidth = si.GetByte("width");
            InitializeComponent();
            InitializeTasks();
            timerTicks = 0;
            IsMdiContainer = true;
            accessoriesToolStripMenuItem.Checked = false;
            amusform = new AmusementsForm(model, mainDockPanel, amusementsToolStripMenuItem);
            pathform = new PathForm(model, pathToolStripMenuItem);
            accform = new AccessoriesForm(model, accessoriesToolStripMenuItem);
            mapform = new MapForm(model, view, playingWidth, playingHeight);           
            PrepareFormsStartAppearance(model.currOfferedAmus, model.currOfferedPaths, model.currOfferedOthers, model.images);           
            amusform.Show(mainDockPanel);
            pathform.Show(mainDockPanel);
           
            view.form = this;
            mapform.InitializeAfterDeserialization(model, view);
            
            timer.Enabled = true;
            MyUpdate();
            this.Refresh();
        }

        public void ChangeAfterDeserialization(View2 view) {
            view.form = this;
            this.model = view.model;
            this.view = view;
            this.playingHeight = model.playingHeight;
            this.playingWidth = model.playingWidth;
           // InitializeComponent();
            InitializeTasks();
            timerTicks = 0;
            IsMdiContainer = true;
            accessoriesToolStripMenuItem.Checked = false;
            amusform.Dispose();
            pathform.Dispose();
            accform.Dispose();
            amusform = new AmusementsForm(model, mainDockPanel, amusementsToolStripMenuItem);
            pathform = new PathForm(model, pathToolStripMenuItem);
            accform = new AccessoriesForm(model, accessoriesToolStripMenuItem);

            PrepareFormsStartAppearance(model.currOfferedAmus, model.currOfferedPaths, model.currOfferedOthers, model.images);
            amusform.Show(mainDockPanel);
            pathform.Show(mainDockPanel);
            mapform.InitializeAfterDeserialization(model, view);
            
            timer.Enabled = true;
            MyUpdate();
            this.Refresh();
        
        }

        public void GetObjectData(SerializationInfo si, StreamingContext sc) {
            si.AddValue("model", model);
            si.AddValue("view", view);
            si.AddValue("height", playingHeight);
            si.AddValue("width", playingWidth);
        }

        
        private void DoNothing() { } // due to set IsCompleted to initial tasks
        private void InitializeTasks() {
            tasks = new Task[3];
            for (int i = 0; i < 3; i++) { //todo: chci vyrobit task a nastavit mu hodnotu na completed
                tasks[i] = Task.Factory.StartNew(DoNothing);
            }
            Task.WaitAll(tasks);
        
        }
       
        public void PrepareFormsStartAppearance(IEnumerable<AmusementsFactory> amus, IEnumerable<PathFactory> paths, IEnumerable<MapObjectsFactory> others, Image[] images) {
            foreach (AmusementsFactory item in amus) {
                amusform.CreateNewItem(images[item.internTypeId], item);
            }
            foreach (PathFactory item in paths) {
                pathform.CreateNewItem(images[item.internTypeId], item);
            }
            foreach (MapObjectsFactory item in others) {
                accform.CreateNewItem(images[item.internTypeId], item);
            }
        
        }
        /// <summary>
        /// Creates a new buyButton for the given item in an corresponding form.
        /// </summary>
        /// <param name="item">A MapObjectFactory item which represents new item to buy. </param>
        /// <param name="im">An Image which is corresponding to given MapObjectFactory item. </param>
        public void AddNewItemToForm(MapObjectsFactory item, Image im) {
            if (item is AmusementsFactory)
                amusform.CreateNewItem(im, (AmusementsFactory)item);
            else if (item is PathFactory)
                pathform.CreateNewItem(im, (PathFactory)item);
            else accform.CreateNewItem(im, item);
        }

        public void ShowDockingForm(WeifenLuo.WinFormsUI.Docking.DockContent form) {
            form.Show(mainDockPanel);
        }


        private void MainForm_Load(object sender, EventArgs e) {

        }

        private void amusementsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (amusementsToolStripMenuItem.Checked) {
                amusform.Hide();
                amusementsToolStripMenuItem.Checked = false;
            }
            else {
                amusform.Show(mainDockPanel);
                amusementsToolStripMenuItem.Checked = true;
            }
        }

        private void pathToolStripMenuItem_Click(object sender, EventArgs e) {
            if (pathToolStripMenuItem.Checked) {
                pathform.Hide();
                pathToolStripMenuItem.Checked = false;
            }
            else {
                pathform.Show(mainDockPanel);
                pathToolStripMenuItem.Checked = true;
            }
        }
        
        private void accessoriesToolStripMenuItem_Click(object sender, EventArgs e) {
            if (accessoriesToolStripMenuItem.Checked) {
                accform.Hide();
                accessoriesToolStripMenuItem.Checked = false;
            }
            else {
                accform.Show(mainDockPanel);
                accessoriesToolStripMenuItem.Checked = true;
            }
        }

        private void timer_Tick(object sender, EventArgs e) {

#warning work with Person in view only when no another thread is running
            
            //todo: rozvrstvit do vlaken
           
            //---actions
            if(!model.parkClosed) tasks[0] = Task.Factory.StartNew(model.persList.Action);
          
            debugtime++;
            // model.persList.Action();
            if (timerTicks >= 10) {
                if (!model.parkClosed) {
                    tasks[1] = Task.Factory.StartNew(model.amusList.Action);
                    tasks[2] = Task.Factory.StartNew(model.maps.Action);
                }

                timerTicks = 0;
                //   model.amusList.Action();
                //    model.effects.Action();
                //    model.maps.Action();
                model.effects.Action(this);
                
                
            }

            //---visual
            //model.dirtyClick
            
            Task.WaitAll(tasks);
           
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
            model.demolishOn = !model.demolishOn;
        }

        private void propagate_toolStripMenuItem_Click(object sender, EventArgs e) {
            if (model.effects.propagateOn) {
                model.effects.propagateOn = false;
                propagate_toolStripMenuItem.Text = Labels.advertiseStart;
                propagate_toolStripMenuItem.ForeColor = Color.Black;
            }
            else {
                model.effects.propagateOn = true;
                propagate_toolStripMenuItem.Text = Labels.advertising;
                propagate_toolStripMenuItem.ForeColor = Color.DarkGreen;
            }
        }

        private void research_toolStripMenuItem_Click(object sender, EventArgs e) {
            if (model.effects.researchOn) {
                model.effects.researchOn = false;
                research_toolStripMenuItem.Text = Labels.researchStart;
                research_toolStripMenuItem.ForeColor = Color.Black;
            }
            else {
                model.effects.researchOn = true;
                research_toolStripMenuItem.Text = Labels.researching;
                research_toolStripMenuItem.ForeColor = Color.DarkGreen;

            }
        }

        private void gameToolStripMenuItem_Click(object sender, EventArgs e) {
           // timer.Enabled = false;
        }
        private void pause_EToolStripMenuItem_Click(object sender, EventArgs e) {
            timer.Enabled = false;
        }
        private void continueToolStripMenuItem_Click(object sender, EventArgs e) {
            timer.Enabled = true;
        }
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e) {
           
#warning Zde se nepouzivaji resources, protoze pridavani do nich ted hlasi chybu
            DialogResult dr = MessageBox.Show(Notices.closeGame, Labels.warningMessBox, MessageBoxButtons.YesNo);
             if (dr == DialogResult.Yes) {
                 startForm.NewGame();
             }
            
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            //Program.SaveToFile(view, new System.IO.FileStream("zkouskaUkladani.txt",System.IO.FileMode.Create));
            startForm.Save(view);
            timer.Enabled = true;
        }
        private void saveAs_ToolStripMenuItem_Click(object sender, EventArgs e) {
            startForm.SaveAs(view);
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            if(MessageBox.Show("Do you really want to leave this game?","Warning", MessageBoxButtons.YesNo) == DialogResult.No) return;
            view.CallBeforeDeserialization();
            Task.WaitAll(tasks);
            view = (View2) startForm.LoadGame();
            ChangeAfterDeserialization(view);
            /* view = Program.LoadFromFile(new System.IO.FileStream("zkouskaUkladani.txt", System.IO.FileMode.Open));
            view.form = this;
            model = view.model;
            ChangeAfterDeserialization(model, view);*/
        }
        private void quitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e) {
           StringBuilder sb= new StringBuilder();
            sb.Append(System.Diagnostics.Process.GetCurrentProcess().Threads.Count.ToString());
            System.Diagnostics.ProcessThreadCollection myThreads = System.Diagnostics.Process.GetCurrentProcess().Threads;
            foreach (System.Diagnostics.ProcessThread  t in myThreads) {
               sb.Append( t.ThreadState.ToString());
            }
            MessageBox.Show(sb.ToString()); 
                            
        }

        

        
       private void firstMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }

       

        private void MainForm_FormClosing_1(object sender, FormClosingEventArgs e) {
            DialogResult dr = MessageBox.Show(Notices.closeGame, Labels.warningMessBox, MessageBoxButtons.YesNo);
            if (dr == DialogResult.No) e.Cancel = true;
            else {
                startForm.Visible = true;
                startForm.MainFormWasClosed();
            }
        }

        

      

        

       

        

        
    }   
   
}
