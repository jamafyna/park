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
        Control map;
        Model model;      
       
        private AmusementEnterPath aEnterHelp;
        private AmusementExitPath aExitHelp;
       

     /*   public MainForm() {
            InitializeComponent();
        }*/
        public MainForm(byte width, byte height)
        {
            InitializeComponent();
            IsMdiContainer = true;
            amusform = new AmusementsForm();
            pathform = new PathForm();         
            amusform.Show(mainDockPanel);
            pathform.Show(mainDockPanel);
            CreateVisualMap(width,height,50);
            model = new Model(height,width);
            
           
        }

        private void CreateVisualMap(int width,int height, int sizeOfSquare) {

           // Model m = new Model();
            //todo: lepe vyrobit ve view
            map=new PictureBox();
            map.BackColor = Color.LightGreen;
            map.Top = 0;//mainDockPanel.Top;
            map.Left = 0;// mainDockPanel.Left;
            map.Width=width*(sizeOfSquare+2);
            map.Height=height*(sizeOfSquare);
            map.Parent = mainDockPanel;
            this.map.Click += new System.EventHandler(this.map_Click);


            //-----vytvoreni PictureBoxu
            Bitmap bmp = new Bitmap(width * sizeOfSquare, height * sizeOfSquare);
            ((PictureBox)map).Image = bmp;
            map.Size = bmp.Size;
            Graphics gr = Graphics.FromImage(bmp);
            
            //-----nakresleni mrizky-------------
            Pen pen = new Pen(Color.DarkSeaGreen, 1);
            for (int i = 0; i <= height; i++) 
                gr.DrawLine(pen, 0, i * sizeOfSquare, width * sizeOfSquare, i * sizeOfSquare);
            for (int i = 0; i <= width; i++)
                gr.DrawLine(pen, i * sizeOfSquare, 0, i * sizeOfSquare, height * sizeOfSquare);
             
            map.Refresh();//musime provest, pokud se zmenila bitmapa a zmenu chceme videt na obrazovce

            //stavba plotu a brany
         //   plot = new Plot(this);
         //   brana = new Brana(this);
            
        }

        private void map_Click(object sender, EventArgs e) {
            if (!model.demolishOn && model.lastClick!=null)
            {
                MouseEventArgs mys = (MouseEventArgs)e;
                byte x = (byte)(mys.X - mys.X % sizeOfSquare);
                byte y = (byte)(mys.Y - mys.Y % sizeOfSquare);
               // System.Drawing.Point loc = mys.Location;

                //todo: kontrola na co vse mohl uzivatel kliknout, nejspise poslat udalost do modelu spolu se souradnicemi
                //todo: kontrola, zda neni neco rozestavene
                if(model.mustBeEnter){
                         if(!model.lastBuiltAmus.CheckEntranceAndBuild(x,y)) {
                                    //todo: show chybu, idalne ulozit do modelu, aby si ji view vzal
                         }
                         return;
                }
                if(model.mustBeExit){
                     if(!model.lastBuiltAmus.CheckExitAndBuild(x,y)) {
                                    //todo: show chybu
                     }
                    return;
                }
               
                if (model.lastClick is Amusements)
                {
                    if (((Amusements)model.lastClick).CheckFreeLocation(x, y))
                    {
                        object[] arg = { x, y, model };
                        //todo:nize nejspise neni nutne ukladat, udelano v konstruktoru atrakce a nastavovat 
                        model.lastBuiltAmus = (Amusements)Activator.CreateInstance(model.lastClick.GetType(), arg);                                               
                    }
                    return;
                }
                else
                {
                    object[] arg = {x,y,model };
                    Activator.CreateInstance(model.lastClick.GetType(), arg);
                }
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
        
    }
}
