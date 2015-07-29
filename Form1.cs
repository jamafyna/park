using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LunaparkGame
{
    public partial class MainForm : Form
    {
        //public Type lastClick{get; private set;}
        /// <summary>
        /// contains an unique instance for each type
        /// </summary>
        
       
        public const int sizeOfSquare = 50;
        AmusementsForm amusform;
        PathForm pathform;
       public Control map;
        Model model;
        View view;

        int timerTime = 0;
       

     /*   public MainForm() {
            InitializeComponent();
        }*/
        public MainForm(byte width, byte height)
        {
            InitializeComponent();
            IsMdiContainer = true;
            model = new Model(height,width);
            view = new View(model,this);
            map=view.CreateVisualMap(width, height, sizeOfSquare);
            map.Parent = mainDockPanel;
            this.map.Click += new System.EventHandler(this.map_Click);

            amusform = new AmusementsForm();
            pathform = new PathForm();
            amusform.Show(mainDockPanel);
            pathform.Show(mainDockPanel);
            var a = new AsphaltPath(model,new Coordinates(1,2));
            a.Control = new PictureBox();
            MessageBox.Show(a.Control.Anchor.ToString());
        }

      
        private void map_Click(object sender, EventArgs e) {
           
            if (!model.demolishOn && model.LastClick!=null)
            {
                #region
                MouseEventArgs mys = (MouseEventArgs)e;
               // byte x = (byte)(mys.X - mys.X % sizeOfSquare);
               // byte y = (byte)(mys.Y - mys.Y % sizeOfSquare);
                byte x = (byte)(mys.X / sizeOfSquare);
                byte y = (byte)(mys.Y / sizeOfSquare);
               
                //todo: kontrola na co vse mohl uzivatel kliknout, nejspise poslat udalost do modelu spolu se souradnicemi
                //todo: kontrola, zda neni neco rozestavene - mysleno pro free-shaped atrakce
                if(model.mustBeEnter){
                         if(!model.lastBuiltAmus.CheckEntranceAndBuild(x,y)) {
                                    //todo: nejspis nechci nic delat
                           //  MessageBox.Show(Notices.cannotDemolishAmusement, Labels.warningMessBox, MessageBoxButtons.OK);
                         }
                         return;
                }
                if(model.mustBeExit){
                    model.lastBuiltAmus.CheckExitAndBuild(x, y); //if not succeed, no annoing error-text                  
                    return;
                }
                if (model.LastClick.price > model.GetMoney()) { MessageBox.Show(Notices.cannotBuyNoMoney, Labels.warningMessBox, MessageBoxButtons.OK); return; }
                if (model.LastClick is Amusements)
                {
                    if (((Amusements)model.LastClick).CheckFreeLocation(x, y))
                    {
                        object[] arg = { x, y, model };
                        //todo:nize nejspise neni nutne ukladat, udelano v konstruktoru atrakce a nastavovat 
                        model.lastBuiltAmus = (Amusements)Activator.CreateInstance(model.LastClick.GetType(), arg);  //todo: melo by se vytvorit v novem vlakne                                          
                    }
                    return;
                }
                else
                {
                    object[] arg = {x,y,model };
                    Activator.CreateInstance(model.LastClick.GetType(), arg);
                }
                #endregion
#warning rusim tu schopnost prekladace spravne kontrolovat - nezajisti mi, ze dana trida bude mit spravny konstruktor
            }
            
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

        private void timer_Tick(object sender, EventArgs e)
        {
           //todo: rozvrstvit do vlaken
            //---actions
            model.persList.Action();
            if (timerTime >= 10)
            {
                timerTime = 0;
                model.amusList.Action();
                model.effects.Action();
                model.maps.Action();
                
            }
            //---visual
            //model.dirtyClick

        }
        public void Update() { 
        
        }
        
    }
}
