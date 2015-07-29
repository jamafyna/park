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
        public PathForm()
        {
            InitializeComponent();

            for (int i = 0; i < 30; i++)
            {
                CreateNewItem("pokud", 125, Properties.Images.path_asphalt, new AsphaltPath());
            }
            
            CreateNewItem("b", 125, Properties.Images.path_asphalt, new AsphaltPath());
            CreateNewItem("c", 125, Properties.Images.path_asphalt, new AsphaltPath());
        }

        public void CreateNewItem(string name, int value, Image image, MapObjects click) {

            
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
            l.Text = name;
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
            l = new Label();
            l.Parent = p;
            l.Text = value.ToString();
            l.TextAlign = ContentAlignment.TopCenter;
            l.Height = 15;
            l.Top = 80;
            l.Left = 0;
            l.Width = 70;


        
        }
    }

}
