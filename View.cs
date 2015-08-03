using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace LunaparkGame
{

    public class View
    {
        Model model;
        Data data;
        MainForm form;
        Control map;
        List<IUpdatable> forms = new List<IUpdatable>();

        public View(Model m, MainForm form)
        {
            this.model = m;
            this.data = new Data();
            this.form = form;
        }
        public void Action()
        {
            DestructDirty();
            NewDirty();
            PeopleMove();
            ClickDirty();
            UpdateDirty();
            //throw new NotImplementedException();
        }
        private void PeopleMove() {
            foreach (Person p in model.persList) {
               // p.Control.Location = p.GetRealCoordinates();
                p.Control.Location = p.GetRealCoordinatesUnsynchronized();
                p.Control.Visible = p.visible;
                
               // p.Control.Left++;
                    
            }        
        }
        private void DestructDirty()
        {
            MapObjects o;
            while(model.dirtyDestruct.TryDequeue(out o))
            {
               if(o.Control!=null) o.Control.Dispose(); 
            }
        }
        public static PictureBox PictureBoxCopy(PictureBox p) {
            throw new NotImplementedException();
        }
        private void NewDirty()
        {            
            MapObjects o;
            PictureBox original;
            PictureBox pbox;
            while (model.dirtyNew.TryDequeue(out o))
            {
                if (o is Person) {
                    pbox = new PictureBox();
                    //obrazek hlavy
                    pbox.BackgroundImage = Properties.Images.person_head;
                    //urceni barvy obleceni pomoci RGB
                    Random random = new Random();
                    pbox.BackColor = ((Person)o).color;
                    //obrezek zustane takovy, jaky byl, tj. neroztahuje se, neopakuje apod.
                    pbox.BackgroundImageLayout = ImageLayout.None;
                    pbox.Width = MainForm.sizeOfSquare / 7;
                    pbox.Height = MainForm.sizeOfSquare / 2;
                    //uchyceni leveho dolniho rohu
                    pbox.Anchor = AnchorStyles.Bottom;
                    pbox.Anchor = AnchorStyles.Left;
                    pbox.Location = ((Person)o).GetRealCoordinates();
                    pbox.Parent = map;
                    map.Controls.SetChildIndex(pbox, 0);
                  //  vzhled.Click+=new EventHandler(((Person)o).Click);
                    o.Control = pbox;
                    
                
                }
                else if (o.GetType() == typeof(AsphaltPath)) {
                    pbox = new PictureBox();
                    pbox.BackgroundImage = Properties.Images.path_asphalt;
                    pbox.Width = MainForm.sizeOfSquare - 1;
                    pbox.Height = MainForm.sizeOfSquare - 1;
                    pbox.Parent = map;
                    map.Controls.SetChildIndex(pbox,10);
                    pbox.Left = o.coord.x * MainForm.sizeOfSquare + 1;
                    pbox.Top = o.coord.y * MainForm.sizeOfSquare + 1;
                    pbox.Visible = true;
                    o.Control = pbox;
                    
                }
                else if (o.GetType() == typeof(Restaurant)) {
                    pbox = new PictureBox();
                    pbox.BackgroundImage = Properties.Images.amus_iceCream ;
                    pbox.BackgroundImageLayout = ImageLayout.Zoom;
                    pbox.Width = ((Restaurant)o).width * MainForm.sizeOfSquare - 1;
                    pbox.Height = ((Restaurant)o).width * MainForm.sizeOfSquare - 1;
                    pbox.Parent = map;
                    map.Controls.SetChildIndex(pbox, 0);
                    pbox.Left = o.coord.x * MainForm.sizeOfSquare + 1;
                    pbox.Top = o.coord.y * MainForm.sizeOfSquare + 1;
                    pbox.Visible = true;
                    o.Control = pbox;

                }
                else if(o.GetType()==typeof(AmusementEnterPath)||o.GetType()==typeof(AmusementExitPath)){
                    pbox = new PictureBox();
                    pbox.BackColor = Color.Blue;
                   
                    pbox.Width =  MainForm.sizeOfSquare - 1;
                    pbox.Height =  MainForm.sizeOfSquare - 1;
                    pbox.Parent = map;
                    map.Controls.SetChildIndex(pbox, 0);
                    pbox.Left = o.coord.x * MainForm.sizeOfSquare + 1;
                    pbox.Top = o.coord.y * MainForm.sizeOfSquare + 1;
                    pbox.Visible = true;
                    o.Control = pbox;
                
                }

             /*   else if (data.dict.TryGetValue(o.GetType(), out original))
                {
                    pbox = PictureBoxCopy(original);
                    pbox.Left = o.coord.x*MainForm.sizeOfSquare + 1;
                    pbox.Top = o.coord.y*MainForm.sizeOfSquare + 1;
                    pbox.Parent = form.map;
                    map.Controls.SetChildIndex(pbox, (int)pbox.Tag);
                    o.Control = pbox;
                }
                else throw new MyDebugException("View.NewDirty - dict nebyl klic");*/
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
                    //if clicked...
                    forms.Add( new PersonForm((Person)o,map));
                }
            }
        
        }

        private void UpdateDirty() {
            foreach (var item in forms) {
                item.MyUpdate();
            }
        
        }
        public Control CreateVisualMap(int realWidth, int realHeight, int sizeOfSquare)
        {          
            map = new PictureBox();
            map.BackColor = Color.LightGreen;
            map.Top = 0;//mainDockPanel.Top;
            map.Left = 0;// mainDockPanel.Left;
            
            //-----vytvoreni PictureBoxu
            Bitmap bmp = new Bitmap(realWidth * sizeOfSquare + 1, realHeight * sizeOfSquare + 1);
            ((PictureBox)map).Image = bmp;
            map.Size = bmp.Size;
            Graphics gr = Graphics.FromImage(bmp);
            map.Size = bmp.Size;
            //-----nakresleni mrizky-------------
            Pen pen = new Pen(Color.DarkSeaGreen, 1);
            for (int i = 0; i <= realHeight; i++)
                gr.DrawLine(pen, 0, i * sizeOfSquare, realWidth * sizeOfSquare, i * sizeOfSquare);
            for (int i = 0; i <= realWidth; i++)
                gr.DrawLine(pen, i * sizeOfSquare, 0, i * sizeOfSquare, realHeight * sizeOfSquare);

            map.Refresh();//musime provest, pokud se zmenila bitmapa a zmenu chceme videt na obrazovce
            CreateBorder(sizeOfSquare, realHeight * sizeOfSquare, top: 0, left: 0); // West
            CreateBorder((realWidth - 2) * sizeOfSquare + 1, sizeOfSquare, top: 0, left: sizeOfSquare); // North
            CreateBorder(sizeOfSquare, realHeight * sizeOfSquare, top: 0, left: (realWidth - 1) * sizeOfSquare + 1); // East
            CreateBorder((realWidth - 2) * sizeOfSquare + 1, sizeOfSquare, top: (realHeight - 1) * sizeOfSquare + 1, left: sizeOfSquare); // South
         
            //stavba plotu a brany
            //   plot = new Plot(this);
            //   brana = new Brana(this);
            return map;
        }
        private void CreateBorder(int width, int height, int top, int left) {
            PictureBox pbox = new PictureBox();
            pbox.Parent = map;
            map.Controls.SetChildIndex(pbox,0);
            pbox.Height = height;
            pbox.Width = width;           
            pbox.Top = top;
            pbox.Left = left;
            pbox.BackgroundImage = Properties.Images.plot;           
        }
        public void CreateGate(Gate g) {
            PictureBox pbox = new PictureBox();          
            pbox.Parent = map;
            map.Controls.SetChildIndex(pbox, 0);
            pbox.Top = g.coord.y * MainForm.sizeOfSquare;
            pbox.Left = g.coord.x * MainForm.sizeOfSquare;
            pbox.Height = Gate.height * MainForm.sizeOfSquare;
            pbox.Width = Gate.width * MainForm.sizeOfSquare;
            pbox.BackgroundImage = Properties.Images.gate;
            pbox.BackgroundImageLayout = ImageLayout.Zoom;
            pbox.Click += new EventHandler(g.Click);
            g.Control = pbox;
           
        }
    }
}
