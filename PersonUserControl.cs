﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LunaparkGame {
    public partial class PersonUserControl : UserControl, IUpdatable {
        //todo: udelat onclosing a v ni p.isClicked=false;
        Person p;
        public PersonUserControl(Person p, Control parent)  {
            InitializeComponent();
            this.p = p;
           // this.Visible = true;
            this.Parent = parent;
            parent.Controls.SetChildIndex(this,0);
        }

        private void PersonUserControl_Load(object sender, EventArgs e) {
            this.idChange_label.Text = p.id.ToString();
            MyUpdate();
        }
        public void MyUpdate() {
            if (p.status == Person.Status.disposing) this.Dispose();
            this.contentmentChange_label.Text = p.GetContentment() + " %";
            this.hungerChange_label.Text = p.GetHunger() + " %";
            this.idAmusChange_label.Text = p.CurrAmusId.ToString();
            this.moneyChange_label.Text = p.GetMoney().ToString();
            this.Location = p.GetRealCoordinates();
            this.debug.Text = p.status.ToString();
            this.debug.Visible = false;
        }

        private void close_button_Click(object sender, EventArgs e) {
            this.Dispose();
        }
    }
}
