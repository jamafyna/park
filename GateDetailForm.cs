using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LunaparkGame {
    public partial class GateDetailForm: WeifenLuo.WinFormsUI.Docking.DockContent, IUpdatable {
        Gate gate;
        Model model;

        public GateDetailForm(Model model, Gate gate, Image im) {
            InitializeComponent();
            this.gate = gate;         
            this.model = model;
            this.pictureBox.Image = im;
            this.Text = gate.name;
            gate.isClicked = true;
            MyUpdate();
        }

        private void prize_numericUpDown1_ValueChanged(object sender, EventArgs e) {
            int fee = (int)prize_numericUpDown1.Value;
            gate.entranceFee = fee;  
        }
        public void MyUpdate() {

            this.prize_numericUpDown1.Value = gate.entranceFee;
           
            this.info_label.Text = gate.GetInfo();

            if (gate.State == Amusements.Status.outOfService || gate.State == Amusements.Status.disposing) {
                outOfService_button.Text = Labels.outOfServiceGate;
                outOfService_button.BackColor = Color.Red;
            }
            else if (gate.IsRunningOut) {
                outOfService_button.Text = Labels.noEntryGate;             
                outOfService_button.BackColor = Color.Orange;
            }
            else {
                outOfService_button.Text = Labels.isRunningGate;
                outOfService_button.BackColor = Color.Lime;
            }
        }


        private void outOfService_button_Click(object sender, EventArgs e) {
            if (gate.State == Amusements.Status.outOfService || gate.State == Amusements.Status.runningOut) {
                gate.State = Amusements.Status.waitingForPeople;
                model.parkClosed = false;
            }
            else { gate.State = Amusements.Status.runningOut;  }
           
        }

        private void GateDetailForm_FormClosing(object sender, FormClosingEventArgs e) {
            gate.isClicked = false;
        }
    }
}
