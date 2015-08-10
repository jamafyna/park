using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LunaparkGame {
    public partial class AmusementDetailForm : WeifenLuo.WinFormsUI.Docking.DockContent, IUpdatable {
        Amusements a;
        Model model;
        public AmusementDetailForm(Model model, Amusements a, Image im) {
            InitializeComponent();
            exit_button.Tag = new AmusementExitPathFactory(a);
            entrance_button.Tag = new AmusementEnterPathFactory(a);
            this.model = model;
            this.pictureBox.Image = im;
            this.a = a;
            this.Text = a.name;
            MyUpdate();
        }

        private void AmusementDetailForm_FormClosing(object sender, FormClosingEventArgs e) {
            a.isClicked = false;
        }
        public void MyUpdate() {
            if (a.State == Amusements.Status.disposing) this.Close();
            this.prize_numericUpDown1.Value = a.CurrFee;
            if (a.isDemolishedEntrance()) entrance_button.Visible = true;
            else entrance_button.Visible = false;
            if (a.isDemolishedExit()) exit_button.Visible = true;
            else exit_button.Visible = false;
            if (a.GetType() == typeof(Gate)) this.info_label.Text = ((Gate)a).GetInfo();
            
            if(a.State == Amusements.Status.outOfService || a.State == Amusements.Status.disposing){
                outOfService_button.Text = Labels.outOfService;
                outOfService_button.BackColor = Color.Red;
            }
            else if (a.IsRunningOut) {
                outOfService_button.Text = Labels.noEntry; 
                outOfService_button.BackColor = Color.Orange;
            }
            else { 
                outOfService_button.Text = Labels.isRunning;
                outOfService_button.BackColor = Color.Lime;
            }             
        }

        private void outOfService_button_Click(object sender, EventArgs e) {
            
            if (a.State == Amusements.Status.outOfService || a.State == Amusements.Status.runningOut)
            {
                a.State=Amusements.Status.waitingForPeople;
            }
            else a.State = Amusements.Status.runningOut;
           
        }

        private void prize_numericUpDown1_ValueChanged(object sender, EventArgs e) {
                int fee=(int)prize_numericUpDown1.Value;
                a.CurrFee = fee;
                model.CheckCheapestFee(fee);            
        }

        private void exit_button_Click(object sender, EventArgs e) {
            model.SetLastClick((AmusementExitPathFactory)((Button)sender).Tag);
        }

        private void entrance_button_Click(object sender, EventArgs e) {
            model.SetLastClick((AmusementEnterPathFactory)((Button)sender).Tag);
        }
    }
}
