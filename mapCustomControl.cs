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
        Pen pen = new Pen(Color.DarkSeaGreen, 1);
        SolidBrush sbr = new SolidBrush(Color.Lime);
        Point[] people = new Point[4000];

        public mapCustomControl(Model model) {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.model = model;
        }

        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
            this.BackColor = Color.Lime;
            DrawGrid(pe.Graphics, pen);
            //todo:draw paths
            
            DrawPeople(pe.Graphics);
            //todo:draw attraction
            
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
                sbr.Color = p.color;
                Point point = p.GetRealCoordinatesUnsynchronized();
                Size size = p.GetRealSize();
                gr.FillRectangle(sbr, new Rectangle(point, size));
                sbr.Color = Color.LightSalmon;
                gr.FillEllipse(sbr, point.X, point.Y - size.Width, size.Width, size.Width);
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
            sbr.Color = amus.color;
            gr.FillRectangle(sbr, rect);
            gr.DrawImage(amus.image, rect);
        }
        /// <summary>
        /// Draws all MapObjects, except of an item which has own color.
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="obj">MapObjects item but not item which has its own color.</param>
        private void DrawMapObject(Graphics gr, MapObjects obj) {
            Rectangle rect = new Rectangle(obj.GetRealCoordinates(), obj.GetRealSize());
            gr.DrawImage(obj.image, rect);
        }
        
        private void DrawGrid(Graphics graphics, Pen pen) {
            for (int i = 0; i <= this.Height; i++)
                graphics.DrawLine(pen, 0, i * MainForm.sizeOfSquare, this.Width * MainForm.sizeOfSquare, i * MainForm.sizeOfSquare);
            for (int i = 0; i <= this.Width; i++)
                graphics.DrawLine(pen, i * MainForm.sizeOfSquare, 0, i * MainForm.sizeOfSquare, this.Height * MainForm.sizeOfSquare);
        }

        private void mapCustomControl_Click(object sender, EventArgs e) {
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
