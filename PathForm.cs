using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LunaparkGame
{
    public partial class PathForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        Panel lastPanel;
        Model model;
        ToolStripMenuItem pathItem;
        public PathForm(Model m, ToolStripMenuItem pathItem)
        {
            InitializeComponent();
            model = m;
            this.pathItem = pathItem;            
        }
        /// <summary>
        /// Creates new menu item. User can after build it.
        /// </summary>
        /// <param name="image">An image of the new item. </param>
        /// <param name="click">Specific PathFactory which the item represents. It is return if user clicks on this item.</param>
        public void CreateNewItem(Image image, PathFactory click) {
           
            Panel p = new Panel();
            p.Parent = this;
            p.Width = 70;
            p.Height = 100;
            if (lastPanel != null) {
                if (lastPanel.Right + p.Width <= this.Width)
                {
                    p.Top = lastPanel.Top;
                    p.Left = lastPanel.Right;
                }
                else {
                    p.Top = lastPanel.Bottom;
                    p.Left = 0;
                }
            }
            else {
                p.Top = 10;
                p.Left = 0;
            }
            lastPanel = p;
            Label l = new Label();
            l.Parent = p;
            l.Text = click.name;
            l.TextAlign = ContentAlignment.TopCenter;
            l.AutoSize = false ;
            l.Top = 5;
            l.Left = 0;
            l.Width = 70;
            l.Height = 15;
            Button b = new Button();
            b.Parent = p;
            b.Width = 50;
            b.Height = 50;
            b.BackgroundImage = image;
            b.Tag = click;
            b.Top = 25;
            b.Left = 10;
            b.Click +=new System.EventHandler(this.Click);
            l = new Label();
            l.Parent = p;
            l.Text = Labels.prize + click.prize.ToString();
            l.TextAlign = ContentAlignment.TopCenter;
            l.Height = 15;
            l.Top = 80;
            l.Left = 0;
            l.Width = 70;
        
        }
        //todo:overit, co z toho se pouziva
        private new void Click(Button sender, EventArgs e )
        {
            model.SetLastClick((PathFactory)sender.Tag);
        }
        private new void Click(object sender, EventArgs e) {
            model.SetLastClick((PathFactory)((Button)sender).Tag);
        }

        private void PathForm_FormClosing(object sender, FormClosingEventArgs e) {
            this.Hide();
            pathItem.Checked = false;
            e.Cancel = true;
        }
    }

}
