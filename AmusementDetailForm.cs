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
        
        }

        private void AmusementDetailForm_Load(object sender, EventArgs e)
        {

        }

        private void AmusementDetailForm_FormClosing(object sender, FormClosingEventArgs e) {

        }

        private void build_button_Click(object sender, EventArgs e) {
            AmusementsFactory af = (AmusementsFactory)((Button)sender).Tag;
            model.SetLastClick(af);
            //this.Hide();
            
        }

    }
}
