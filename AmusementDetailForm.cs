using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LunaparkGame {
    public partial class AmusementDetailForm : WeifenLuo.WinFormsUI.Docking.DockContent, IUpdatable {
        public Amusements a { protected set; get; }
        protected Model model;
        public AmusementDetailForm() { }
        public AmusementDetailForm(Model model, Amusements a, Image im) {
            InitializeComponent();
            exit_button.Tag = new AmusementExitPathFactory(a, Properties.Images.exit);
            entrance_button.Tag = new AmusementEnterPathFactory(a, Properties.Images.enter);
            this.model = model;
            this.pictureBox.Image = im;
            this.a = a;
            this.Text = a.name;
            a.isClicked = true;
            MyUpdate();
        }

        protected void AmusementDetailForm_FormClosing(object sender, FormClosingEventArgs e) {
            a.isClicked = false;
        }
        public void MyUpdate() {
            if (a.State == Amusements.Status.disposing) this.Close();
            this.prize_numericUpDown1.Value = a.CurrFee;
            this.crashValue_label.Text = a.CrashnessPercent + "%";
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

            if (a.State == Amusements.Status.outOfService || a.State == Amusements.Status.runningOut) {
                if (a.Crashed) MessageBox.Show(Labels.warningMessBox, Notices.cannotChangeFirstRepair, MessageBoxButtons.OK);
                else { a.State = Amusements.Status.waitingForPeople; model.MarkBackInService(a); }
            }
            else { a.State = Amusements.Status.runningOut; model.MarkOutOfService(a); }
           
        }

        private void prize_numericUpDown1_ValueChanged(object sender, EventArgs e) {
                int fee=(int)prize_numericUpDown1.Value;
                a.CurrFee = fee;
                model.CheckCheapestFee(fee);            
        }

        private void exit_button_Click(object sender, EventArgs e) {
            model.SetLastClick((AmusementExitPathFactory)((Button)sender).Tag);
            model.demolishOn = false;
        }

        private void entrance_button_Click(object sender, EventArgs e) {
            model.SetLastClick((AmusementEnterPathFactory)((Button)sender).Tag);
            model.demolishOn = false;
        }

        private void button1_Click(object sender, EventArgs e) {
            a.RepairWhole();
        }

        private void AmusementDetailForm_Load(object sender, EventArgs e) {

        }
    }
}
