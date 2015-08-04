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
        public AmusementsForm(Model model, DockPanel parent, ToolStripMenuItem amusement)
        {
            InitializeComponent();
           CreateNewButton();
           dockP = parent;
            detailForm = new AmusementDetailForm(model,parent);
            this.amusItem = amusement;
           // this.IsMdiContainer = true;
        }

        private void AmusementsForm_Load(object sender, EventArgs e)
        {

        }
        public void CreateNewButton() {
           
            
            Button b = new Button();
            b.BackColor = Color.Red;

            b.Parent = this;
            b.Click += new EventHandler(Click);
            //todo: b.Tag=potrebne
        }
        
        private new void Click(object sender, EventArgs e) {
            detailForm.Set("neco","blabla",Properties.Images.amus_iceCream);//todo: udelat spravne, ziskat neco z .Tag a dohledat spravne info
           // if (detailForm.IsHidden) 
            detailForm.Show(dockP);
            
        }

        private void AmusementsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            amusItem.Checked = false;
            e.Cancel = true;
        }

       
    }
}
