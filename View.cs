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
        Graphics graphics;
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
            //DestructDirty();
            //NewDirty();
            PeopleMove();
            ClickDirty();
            UpdateDirty();
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
        private MapObjects FindObject(int realX, int realY, byte mapX, byte mapY){
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
     /*   private void DestructDirty()
        {
            MapObjects o;
            while(model.dirtyDestruct.TryDequeue(out o))
            {
               if(o.control!=null) o.control.Dispose(); 
            }
        }*/
        public static PictureBox PictureBoxCopy(PictureBox p) {
            throw new NotImplementedException();
        }
      /*  private void NewDirty()
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
                    
                    Size size = o.GetRealSize();
                    int a = size.Width;
                    int b = size.Height;
                    pbox.Width = a - 1;
                    pbox.Height = b - 1;
                    pbox.Parent = map;
                    map.Controls.SetChildIndex(pbox, o.zIndex);
                    Point point = o.GetRealCoordinates();
                    pbox.Left = point.X + 1;
                    pbox.Top = point.Y + 1;
                    pbox.Visible = true;
                    pbox.Click += new EventHandler(o.Click);
                    if (o.GetType() == typeof(AmusementEnterPath)) pbox.BackColor = Color.Red;
                    else if (o.GetType() == typeof(AmusementExitPath)) pbox.BackColor = Color.Blue;
                    else {
                        pbox.BackgroundImage = images[o.internTypeID];
                        pbox.BackgroundImageLayout = ImageLayout.Zoom;
                        if(o is Amusements) pbox.BackColor = ((Amusements)o).color;
                    }
                    o.control = pbox;
                }
            }

        }
      */
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
                    if (o.isClicked) {
                        foreach (Form f in forms) {
                            if (f is AmusementDetailForm && ((AmusementDetailForm)f).a == o) { f.Activate(); break; }
                        }
                    }
                    else {
                        AmusementDetailForm f = new AmusementDetailForm(model, (Amusements)o, images[((Amusements)o).internTypeID]);
                        f.Show(dockPanel);
                        forms.Add(f);
                    }
                }
            }
        
        }

        private void UpdateDirty() {
            foreach (var item in forms) {
                item.MyUpdate();
            }
        
        }


        public void UpdatePeople() {
            Color c = Color.Blue;
            SolidBrush brush = new SolidBrush(c);
            
            graphics.FillRectangle(brush,100,200,7,25 );
        }
        public Control CreateVisualMap(int playingWidth, int playingHeight, int sizeOfSquare)
        {          
            map = new PictureBox();
            map.BackColor = Color.LightGreen;
            map.Top = 0; // mainDockPanel.Top;
            map.Left = 0; // mainDockPanel.Left;
            
            //-----vytvoreni PictureBoxu
            Bitmap bmp = new Bitmap(playingWidth * sizeOfSquare + 1, playingHeight * sizeOfSquare + 1);
            ((PictureBox)map).Image = bmp;
            map.Size = bmp.Size;
            graphics = Graphics.FromImage(bmp);
            map.Size = bmp.Size;
            //-----nakresleni mrizky-------------
            Pen pen = new Pen(Color.DarkSeaGreen, 1);
            for (int i = 0; i <= playingHeight; i++)
                graphics.DrawLine(pen, 0, i * sizeOfSquare, playingWidth * sizeOfSquare, i * sizeOfSquare);
            for (int i = 0; i <= playingWidth; i++)
                graphics.DrawLine(pen, i * sizeOfSquare, 0, i * sizeOfSquare, playingHeight * sizeOfSquare);

            map.Refresh();//musime provest, pokud se zmenila bitmapa a zmenu chceme videt na obrazovce
            CreateBorder(sizeOfSquare, playingHeight * sizeOfSquare, top: 0, left: 0); // West
            CreateBorder((playingWidth - 2) * sizeOfSquare + 1, sizeOfSquare, top: 0, left: sizeOfSquare); // North
            CreateBorder(sizeOfSquare, playingHeight * sizeOfSquare, top: 0, left: (playingWidth - 1) * sizeOfSquare + 1); // East
            CreateBorder((playingWidth - 2) * sizeOfSquare + 1, sizeOfSquare, top: (playingHeight - 1) * sizeOfSquare + 1, left: sizeOfSquare); // South
         
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
