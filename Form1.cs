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
        public Type lastClick{get; private set;}
        /// <summary>
        /// contains an unique instance for each type
        /// </summary>
       // public MapObjects lastClick{set; private get;}
        MapObjects lastBuildItem;
        static int sizeOfSquare = 50;
        AmusementsForm amusform;
        PathForm pathform;
        Control map;
        Model model;
       
        bool DemolishOn;

        public MainForm()
        {
            InitializeComponent();
            IsMdiContainer = true;
            amusform = new AmusementsForm();
            pathform = new PathForm();         
            amusform.Show(mainDockPanel);
            pathform.Show(mainDockPanel);
            CreateVisualMap(10,15,50);
           
           
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
            if (!DemolishOn)
            {
                MouseEventArgs mys = (MouseEventArgs)e;
                int x = mys.X - mys.X % sizeOfSquare;
                int y = mys.Y - mys.Y % sizeOfSquare;
                //  if(lastClick ==)

                //todo: kontrola na co vse mohl uzivatel kliknout, nejspise poslat udalost do modelu spolu se souradnicemi
                //todo: kontrola, zda neni neco rozestavene
                if (lastClick is Amusements)
                {
                    // lastClick.Create(x,y);

                    
                }
                else
                {
                    // if (lastClick.Create(x, y)) { 
                    //do sth or nothing
                }

                object[] arg = { model, x, y };
                Activator.CreateInstance(lastClick, arg);
                throw new NotImplementedException();
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
