using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace LunaparkGame
{
    public partial class StartForm : Form
    {
        private const byte width1 = 15;
        private const byte height1 = 15;
        private const byte width2 = 20;
        private const byte height2 = 30;
        private const byte width3 = 50;
        private const byte height3 = 50;

        public byte width {get;private set;}
        public byte height{get;private set;}
        MainForm mainForm;
        string currFilePath="";
        string mainDirectory;

        public StartForm()
        {
            InitializeComponent();
            width = width1;
            height = height1;            
            mainDirectory = System.IO.Directory.GetCurrentDirectory() + @"\" + "saved-games";
            System.IO.Directory.CreateDirectory(mainDirectory);
        }

        public void NewGame() {
            if (mainForm != null) {
                mainForm.CloseAll();
                mainForm.Visible = false; }
            this.Visible = true;
            sizeOfMap_panel.Visible = true;
            Start_panel.Visible = false;      
        }
        public void Save(object game) {
            if (currFilePath == "") { SaveAs(game); return; }
            SaveToFile(game, currFilePath, true);
        }
        public void SaveAs(object game) {
            saveFileDialog.InitialDirectory = mainDirectory;
            saveFileDialog.Filter = "Lunapark Files (.lun)|*.lun";
            DateTime d = DateTime.Now;
            string name = String.Concat(d.Year, "-", d.Month, "-", d.Day, "_", d.Hour, "-", d.Minute, "-", d.Second);
            saveFileDialog.FileName = name;
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                currFilePath = saveFileDialog.FileName;
                SaveToFile(game, currFilePath, false);
            };
        
        }

        private void SaveToFile(object game, string path, bool fileExists) {
            System.IO.FileStream fs = null;
            try {
                try {
                    if (!fileExists) fs = new System.IO.FileStream(path, FileMode.CreateNew);
                    }
                catch (IOException) {
                    DialogResult dr = MessageBox.Show("This file already exists. Do you want to replace it?", Labels.warningMessBox, MessageBoxButtons.YesNo);
                    if (dr == DialogResult.No) SaveAs(game);
                }
                if (fs == null) fs = new System.IO.FileStream(path, FileMode.Create);
                BinaryFormatter binF = new BinaryFormatter();
                binF.Serialize(fs, game);
            }
           /* catch (IOException e) {
                MessageBox.Show();
            }*/
            finally {
                if (fs != null) fs.Close();
            }
        }
        public object LoadGame() {
            openFileDialog.InitialDirectory = mainDirectory;
            openFileDialog.Filter = "Lunapark Files (.lun)|*.lun";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                return LoadFromFile(openFileDialog.FileName);
            }
            else return null;          
        }
        
        private object LoadFromFile(string filePath) {
            FileStream fs = null;
            try {
                fs = new System.IO.FileStream(filePath, FileMode.Open);
                this.currFilePath = filePath;
                BinaryFormatter binF = new BinaryFormatter();
                return binF.Deserialize(fs);
            }
            finally {
                if (fs != null) fs.Close();
            }
        }

        private void newGame_button_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void exit_button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Start_button1_Click(object sender, EventArgs e)
        {
            //this.Close(); 
            this.Visible = false;
            mainForm = new MainForm(this.width, this.height, this);
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

        private void StartForm_FormClosing(object sender, FormClosingEventArgs e) {
            Application.Exit();
           // this.Close();
           // if (mainForm != null) mainForm.Close();
        }

        private void loadGame_button_Click(object sender, EventArgs e) {

            this.Visible = false;
            mainForm = new MainForm(this.width, this.height, this);           
            mainForm.ChangeAfterDeserialization((View2)LoadGame()) ;
            mainForm.Visible = true; 
        }

       


    }
}
