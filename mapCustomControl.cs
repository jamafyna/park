using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LunaparkGame {
    public partial class mapCustomControl : Control {
        Model model;
        View2 view;
        Pen pen = new Pen(Color.DarkSeaGreen, 1);
        SolidBrush sbr = new SolidBrush(Color.Lime);
        Rectangle[] borderRects;

        public mapCustomControl(Model model, View2 view) {                     
            InitializeComponent();
            borderRects = InitializeBorderRects(model.playingWidth + 2, model.playingHeight + 2);
            this.Width = (model.playingWidth + 2) * MainForm.sizeOfSquare + 1;
            this.Height = (model.playingHeight + 2) * MainForm.sizeOfSquare + 1;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.model = model;
            this.view = view;
        }

        public void InitializeAfterDeserialization(Model m, View2 v ) {
            this.model = m;
            this.view = v;
            this.Width = (model.playingWidth + 2) * MainForm.sizeOfSquare + 1;
            this.Height = (model.playingHeight + 2) * MainForm.sizeOfSquare + 1;
            borderRects = InitializeBorderRects(model.playingWidth + 2, model.playingHeight + 2);
        }
        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
            this.BackColor = Color.Lime;
            DrawGrid(pe.Graphics, pen);
            DrawBorder(pe.Graphics, Properties.Images.plot);
            
            List<Path> paths = model.maps.GetPathsUnsynchronized();
            foreach (Path item in paths) {
                DrawMapObject(pe.Graphics,item);
            }
            DrawPeople(pe.Graphics);
            foreach (Amusements a in model.amusList.GetAmusementsUnsynchronized()) {
                DrawAmusement(pe.Graphics, a);
            }
            
        }
        private Rectangle[] InitializeBorderRects(int mapWidth, int mapHeight) {
            int width = mapWidth * MainForm.sizeOfSquare - 1;
            int height = mapHeight * MainForm.sizeOfSquare - 1;
            int squareSize = MainForm.sizeOfSquare - 1;          
            Rectangle[] rects = {
                                    new Rectangle(1, 1, squareSize, height),
                                    new Rectangle(1, 1, width, squareSize),
                                    new Rectangle(1, height - squareSize + 1, width, squareSize ),
                                    new Rectangle(width - squareSize + 1, 1, squareSize, height)
                          };
            return rects;
        }

        private void DrawPerson(Graphics gr, Person p) {
            sbr.Color = p.color;
            Point point=p.GetRealCoordinatesUnsynchronized();
            Size size = p.GetRealSize();
            gr.FillRectangle(sbr, new Rectangle(point, size));
            sbr.Color = Color.LightSalmon;
            gr.FillEllipse(sbr, point.X, point.Y - size.Width, size.Width, size.Width);            
        }

        private void DrawPeople(Graphics gr) {
            foreach (Person p in model.persList) {
                if (p.visible) {
                    sbr.Color = p.color;
                    Point point = p.GetRealCoordinatesUnsynchronized();
                    Size size = p.GetRealSize();
                    point.Y -= size.Height;
                    gr.FillRectangle(sbr, new Rectangle(point, size));
                    sbr.Color = Color.LightSalmon;
                    gr.FillEllipse(sbr, point.X - 1, point.Y - size.Width + 1, size.Width + 2, size.Width + 2);
                   // gr.FillEllipse(sbr, point.X - 1, point.Y - 5, 7, 7);
                }
            }      
        }

       /* private void DrawImages(Image im, Color color, Graphics gr, Rectangle rect) {
            sbr.Color = color;
            gr.FillRectangle(sbr, rect);
            gr.DrawImage(im, rect);
        }*/
        private void DrawAmusement(Graphics gr, Amusements amus) {
            Size size = amus.GetRealSize();
            Point point = amus.GetRealCoordinates();
            Rectangle rect = new Rectangle(point, size);
            if (!amus.Crashed) sbr.Color = amus.color;
            else sbr.Color = Color.Black;
            gr.FillRectangle(sbr, rect);
            if (amus is RectangleAmusements && !((RectangleAmusements)amus).isHorizontalOriented) {
                Image im = (Image)(view.images[amus.internTypeID]).Clone();
                im.RotateFlip(RotateFlipType.Rotate90FlipNone);
                gr.DrawImage(im, rect);
            }
            else gr.DrawImage(view.images[amus.internTypeID], rect);
        }
        /// <summary>
        /// Draws all MapObjects, except of an item which has own color.
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="obj">MapObjects item but not item which has its own color.</param>
        private void DrawMapObject(Graphics gr, MapObjects obj) {
            Rectangle rect = new Rectangle(obj.GetRealCoordinates(), obj.GetRealSize());
            gr.DrawImage(view.images[obj.internTypeID], rect);
        }
        
        private void DrawGrid(Graphics graphics, Pen pen) {
            for (int i = 0; i <= this.Height; i++)
                graphics.DrawLine(pen, 0, i * MainForm.sizeOfSquare, this.Width * MainForm.sizeOfSquare, i * MainForm.sizeOfSquare);
            for (int i = 0; i <= this.Width; i++)
                graphics.DrawLine(pen, i * MainForm.sizeOfSquare, 0, i * MainForm.sizeOfSquare, this.Height * MainForm.sizeOfSquare);
        }

        private void DrawBorder(Graphics gr, Image im) {           
            TextureBrush br = new TextureBrush(im, System.Drawing.Drawing2D.WrapMode.Tile);                       
            gr.FillRectangles(br, borderRects);
        }
        private void mapCustomControl_Click(object sender, EventArgs e) {
            MouseEventArgs mouse = (MouseEventArgs)e;
            int realX = mouse.X;
            int realY = mouse.Y;
            byte mapX = (byte)(mouse.X / MainForm.sizeOfSquare);
            byte mapY = (byte)(mouse.Y / MainForm.sizeOfSquare);
            MapObjects obj = FindObject(realX, realY, mapX, mapY);
            if (obj != null) obj.Click(sender, e);
            else if (mapX != 0 && mapX != model.internalWidth - 1 && mapY != 0 && mapY != model.internalHeight - 1) { // border
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
        private MapObjects FindObject(int realX, int realY, byte mapX, byte mapY) {
            MapObjects obj = null;
            if ((obj = model.maps.GetAmusement(mapX, mapY)) != null) return obj;
            foreach (Person p in model.persList) {
                if (p.IsInside(realX, realY)) return p;
            }
            if ((obj = model.maps.GetPath(mapX, mapY)) != null) return obj;
            return null;
        }
    
    }
}
