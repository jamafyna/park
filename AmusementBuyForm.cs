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
    public partial class AmusementBuyForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        DockPanel dockPanel;
        public Color color { get; private set; }
        GameRecords model;
        bool isHorizontal = true;
        public AmusementBuyForm(GameRecords m, DockPanel parent)
        {
            InitializeComponent();
            this.dockPanel = parent;
            model = m;
            color = Color.Yellow;
            this.Hide();
        }

        private void changeColor_button_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK) 
            {
                color = colorDialog.Color;
                this.pictureBox.BackColor = color;
                ((AmusementsFactory)(this.build_button.Tag)).color = color;
            }
        }
        public void Set(AmusementsFactory click, Image image) {
            this.Text = click.name;
            this.info_label.Text = click.GetInfo();
            this.pictureBox.Image = image;
            this.pictureBox.Width = image.Width;
            this.pictureBox.Height = image.Height;            
            this.pictureBox.BackColor = color;
            this.build_button.Tag = click;
            if (click is RectangleAmusementsFactory) {
                rectangleA_button.Visible = true;
                isHorizontal = true;
            }
            else rectangleA_button.Visible = false;
        
        }

        

        private void build_button_Click(object sender, EventArgs e) {
            AmusementsFactory af = (AmusementsFactory)((Button)sender).Tag;
            if(af is RectangleAmusementsFactory) ((RectangleAmusementsFactory)af).isHorizontal=isHorizontal; 
            model.SetLastClick(af);
            model.demolishOn = false;
            //this.Hide();
            
        }

        private void AmusementBuyForm_FormClosing(object sender, FormClosingEventArgs e) {
            this.Hide();
            e.Cancel = true;
        }

        private void rectangleA_button_Click(object sender, EventArgs e) {
            if (isHorizontal) rectangleA_button.Text = Labels.verticalButtonText;
            else rectangleA_button.Text = Labels.horizontalButtonText;
            isHorizontal = !isHorizontal;
        }

    }
}
