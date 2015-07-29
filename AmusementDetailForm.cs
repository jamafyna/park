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
    public partial class AmusementDetailForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        DockPanel dockPanel;
        public Color color { get; private set; }
        Model model;
        public AmusementDetailForm(Model m, DockPanel parent)
        {
            InitializeComponent();
            this.dockPanel = parent;
            model = m;          
            this.Hide();
        }

        private void changeColor_button_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK) //NEVIM, JAK ZRUSIT ZAVRENI CELEHO FORMULARE PO UKONCENI COLORDIALOGU
            {
                color = colorDialog.Color;
                this.pictureBox.BackColor = color;
            }
        }
        public void Set(string infoText, string name, Image image) {
            this.Text = name;
            this.info_label.Text = infoText;
            this.pictureBox.Image = image;
            this.pictureBox.Width = image.Width;
            this.pictureBox.Height = image.Height;
            
            this.pictureBox.BackColor = color;
        
        }

        private void AmusementDetailForm_Load(object sender, EventArgs e)
        {

        }

        private void buy_button_Click(object sender, EventArgs e)
        {
            model.LastClick = (MapObjects)((Button)sender).Tag;
            MessageBox.Show("podarilos e");
            this.Hide();
        }

    }
}
