using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using WeifenLuo.WinFormsUI.Docking;

namespace LunaparkGame
{

    public class View
    {
        Model model;
        MainForm form;
        Control map;
        List<IUpdatable> forms = new List<IUpdatable>();
        Image[] images;
        DockPanel dockPanel;
        public AmusementsForm amusform;
        public PathForm pathform;
        public AccessoriesForm accform;
        public Queue<AmusementsFactory> notAddedAmus;
        public Queue<PathFactory> notAddedPaths;
        public Queue<MapObjectsFactory> notAddedOthers;

        public View(Model m, MainForm form, DockPanel mainDockPanel, AmusementsForm amform, PathForm pform,  AccessoriesForm oform, Data data)
        {
            this.model = m;
            this.form = form;
            this.dockPanel = mainDockPanel;
            this.amusform = amform;
            this.pathform = pform;
            this.accform = oform;
            this.notAddedAmus = data.otherAmus;
            this.notAddedPaths = data.otherPaths;
            this.notAddedOthers = data.otherOthers;
            this.images = data.GetImages();
            foreach (var item in data.initialAmus) amusform.CreateNewItem(images[item.internTypeId], item);
            foreach (var item in data.initialPaths) pathform.CreateNewItem(images[item.internTypeId], item);
            foreach (var item in data.initialOthers) accform.CreateNewItem(images[item.internTypeId], item);
            data = null; // due to GC

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
                p.control.Location = p.GetRealCoordinatesUnsynchronized();
                p.control.Visible = p.visible;
                
               // p.Control.Left++;
                    
            }        
        }
        private void DestructDirty()
        {
            MapObjects o;
            while(model.dirtyDestruct.TryDequeue(out o))
            {
               if(o.control!=null) o.control.Dispose(); 
            }
        }
        public static PictureBox PictureBoxCopy(PictureBox p) {
            throw new NotImplementedException();
        }
        private void NewDirty()
        {            
            MapObjects o;
            PictureBox pbox;
            while (model.dirtyNew.TryDequeue(out o))
            {
                if (o is Person) { //todo: idealne smazat a vykreslovat vzdy
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
                    pbox.Click+=new EventHandler(o.Click);
                    o.control = pbox;               
                }

               else {
                    pbox = new PictureBox();
                    int a, b;
                    o.GetRealSize(out a, out b);
                    pbox.Width = a - 1;
                    pbox.Height = b - 1;
                    pbox.Parent = map;
                    map.Controls.SetChildIndex(pbox, o.zIndex);
                    o.GetRealCoordinates(out a, out b);
                    pbox.Left = a + 1;
                    pbox.Top = b + 1;
                    pbox.Visible = true;
                    pbox.Click += new EventHandler(o.Click);
                    if (o.GetType() == typeof(AmusementEnterPath)) pbox.BackColor = Color.Red;
                    else if (o.GetType() == typeof(AmusementExitPath)) pbox.BackColor = Color.Blue;
                    else pbox.BackgroundImage=images[((IButtonCreatable)o).InternTypeId];
                    o.control = pbox;
                }
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
                   
                    forms.Add( new PersonUserControl((Person)o,map));                    
                }
                if (o is Amusements) {
                    AmusementDetailForm f = new AmusementDetailForm(model, (Amusements)o, images[((Amusements)o).InternTypeId]);
                    f.Show(dockPanel);
                    forms.Add(f);
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
            g.control = pbox;
            
           
        }
    }
}
