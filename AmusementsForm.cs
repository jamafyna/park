using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LunaparkGame
{
    public partial class AmusementsForm : WeifenLuo.WinFormsUI.Docking.DockContent,IUpdatable
    {
        public AmusementsForm()
        {
            InitializeComponent();
           // CreateNewButton();
        }

        private void AmusementsForm_Load(object sender, EventArgs e)
        {

        }
        public void CreateNewButton(Control c) {
           
            
            Button b = new Button();
            b.BackColor = Color.Red;

            b.Parent = this;
            b.Click += new EventHandler(Click);
        }
        
        private new void Click(object sender, EventArgs e) {
           //todo: if povoleni stavit jine nez IO
            MessageBox.Show(sender.ToString());
            ((Button)sender).BackColor = Color.Purple;
        }
    }
}
