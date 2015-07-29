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
        
        public View(Model m, MainForm form)
        {
            this.model = m;
            this.data = new Data();
            this.form = form;
        }
        public void Action()
        {

            throw new NotImplementedException();
        }
        private void DestructDirty()
        {
            foreach (var item in model.dirtyDestruct)
            {
                //item.Dispose();
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
                if (data.dict.TryGetValue(o.GetType(), out original))
                {
                    pbox = PictureBoxCopy(original);
                    pbox.Left = o.coord.x*MainForm.sizeOfSquare + 1;
                    pbox.Top = o.coord.y*MainForm.sizeOfSquare + 1;
                    pbox.Parent = form.map;
                    form.map.Controls.SetChildIndex(pbox, (int)pbox.Tag);
                    o.Control = pbox;
                }
                else throw new MyDebugException("View.NewDirty - dict nebyl klic");
            }

        }
        private void ClickDirty() { 
            //nezapomenout zmenit spravne click v danem objektu
        
        }

        public Control CreateVisualMap(int width, int height, int sizeOfSquare)
        {

            // Model m = new Model();
            //todo: lepe vyrobit ve view
            map = new PictureBox();
            map.BackColor = Color.LightGreen;
            map.Top = 0;//mainDockPanel.Top;
            map.Left = 0;// mainDockPanel.Left;
            map.Width = width * (sizeOfSquare + 2);
            map.Height = height * (sizeOfSquare);
            


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
            return map;
        }

       
    }
    

}
