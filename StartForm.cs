using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LunaparkGame
{
    public partial class StartForm : Form
    {
        public int width {get;private set;}
        public int height{get;private set;}
        public StartForm()
        {
            InitializeComponent();
        }

        private void newGame_button_Click(object sender, EventArgs e)
        {
            sizeOfMap_panel.Visible=true;
            Start_panel.Visible = false;
        }

        private void exit_button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Start_button1_Click(object sender, EventArgs e)
        {
            this.Close();  
        }

        private void radioButton20_CheckedChanged(object sender, EventArgs e)
        {
            width = 10;
            height = 10;
        }

        private void radioButton40_CheckedChanged(object sender, EventArgs e)
        {
            width = 10;
            height = 20;
        }

        private void radioButton60_CheckedChanged(object sender, EventArgs e)
        {
            width = 15;
            height = 15;
        }


    }
}
