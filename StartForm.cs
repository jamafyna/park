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
        private const byte width1 = 10;
        private const byte height1 = 10;
        private const byte width2 = 10;
        private const byte height2 = 20;
        private const byte width3 = 15;
        private const byte height3 = 15;

        public byte width {get;private set;}
        public byte height{get;private set;}
        public StartForm()
        {
            InitializeComponent();
            width = width1;
            height = height1;
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
            //this.Close(); 
            this.Visible = false;
            MainForm mainForm = new MainForm(this.width, this.height);
            mainForm.Visible = true;
            //Application.Run(mainForm);
        }

        private void radioButton20_CheckedChanged(object sender, EventArgs e)
        {
            width = width1;
            height = height1;
        }

        private void radioButton40_CheckedChanged(object sender, EventArgs e)
        {
            width = width2;
            height = height2;
        }

        private void radioButton60_CheckedChanged(object sender, EventArgs e)
        {
            width = width3;
            height = height3;
        }

        private void StartForm_Load(object sender, EventArgs e)
        {

        }

        private void StartForm_FormClosed(object sender, FormClosedEventArgs e)
        {
          //  this.Close();//Application.Exit();
        }


    }
}
