using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace LunaparkGame
{
    public partial class AmusementsForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        AmusementDetailForm detailForm;
        DockPanel dockP;
        ToolStripMenuItem amusItem;
        Panel lastItem;

        public AmusementsForm(Model model, DockPanel parent, ToolStripMenuItem amusement)
        {
            InitializeComponent();
            dockP = parent;
            detailForm = new AmusementDetailForm(model,parent);
            this.amusItem = amusement;
        }

        private void AmusementsForm_Load(object sender, EventArgs e)
        {

        }
        public void CreateNewItem(Image image, AmusementsFactory click) {
           
            Panel p = new Panel();
            p.Parent = this;
            p.Width = 90;
            p.Height = 120;
            if (lastItem != null) {
                if (lastItem.Right + p.Width <= this.Width)
                {
                    p.Top = lastItem.Top;
                    p.Left = lastItem.Right;
                }
                else {
                    p.Top = lastItem.Bottom;
                    p.Left = 0;
                }
            }
            else {
                p.Top = 10;
                p.Left = 0;
            }
            lastItem = p;
            Label l = new Label();
            l.Parent = p;
            l.Text = click.name;
            l.TextAlign = ContentAlignment.TopCenter;
            l.AutoSize = false ;
            l.Top = 5;
            l.Left = 0;
            l.Width = 90;
            l.Height = 15;
            Button b = new Button();
            b.Parent = p;
            b.Width = 70;
            b.Height = 70;
            b.BackgroundImage = image;
            b.BackgroundImageLayout = ImageLayout.Zoom;
            b.Tag = click;
            b.Top = 25;
            b.Left = 10;
            b.Click +=new System.EventHandler(this.Click);
            l = new Label();
            l.Parent = p;
            l.Text = Labels.prize+click.prize.ToString();
            l.TextAlign = ContentAlignment.TopCenter;
            l.Height = 15;
            l.Top = 100;
            l.Left = 0;
            l.Width = 90;
        }
        
        private new void Click(object sender, EventArgs e) {
            detailForm.Set((AmusementsFactory)((Button)sender).Tag, Properties.Images.amus_iceCream);
            if (detailForm.IsHidden) detailForm.Show(dockP);
            
        }

        private void AmusementsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            amusItem.Checked = false;
            e.Cancel = true;
        }

       
    }
}
