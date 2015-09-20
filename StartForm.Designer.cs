namespace LunaparkGame
{
    partial class StartForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sizeOfMap_panel = new System.Windows.Forms.Panel();
            this.textBox1Rozmer = new System.Windows.Forms.TextBox();
            this.radioButton60 = new System.Windows.Forms.RadioButton();
            this.radioButton40 = new System.Windows.Forms.RadioButton();
            this.radioButton20 = new System.Windows.Forms.RadioButton();
            this.Start_button1 = new System.Windows.Forms.Button();
            this.Start_panel = new System.Windows.Forms.Panel();
            this.exit_button = new System.Windows.Forms.Button();
            this.loadGame_button = new System.Windows.Forms.Button();
            this.newGame_button = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.sizeOfMap_panel.SuspendLayout();
            this.Start_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sizeOfMap_panel
            // 
            this.sizeOfMap_panel.BackColor = System.Drawing.SystemColors.Control;
            this.sizeOfMap_panel.Controls.Add(this.textBox1Rozmer);
            this.sizeOfMap_panel.Controls.Add(this.radioButton60);
            this.sizeOfMap_panel.Controls.Add(this.radioButton40);
            this.sizeOfMap_panel.Controls.Add(this.radioButton20);
            this.sizeOfMap_panel.Controls.Add(this.Start_button1);
            this.sizeOfMap_panel.Location = new System.Drawing.Point(12, 2);
            this.sizeOfMap_panel.Name = "sizeOfMap_panel";
            this.sizeOfMap_panel.Size = new System.Drawing.Size(233, 248);
            this.sizeOfMap_panel.TabIndex = 7;
            this.sizeOfMap_panel.Visible = false;
            // 
            // textBox1Rozmer
            // 
            this.textBox1Rozmer.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1Rozmer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1Rozmer.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBox1Rozmer.Location = new System.Drawing.Point(3, 18);
            this.textBox1Rozmer.Name = "textBox1Rozmer";
            this.textBox1Rozmer.Size = new System.Drawing.Size(220, 19);
            this.textBox1Rozmer.TabIndex = 5;
            this.textBox1Rozmer.Text = Labels.sizeOfMap;
            this.textBox1Rozmer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // radioButton60
            // 
            this.radioButton60.AutoSize = true;
            this.radioButton60.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.radioButton60.Location = new System.Drawing.Point(70, 141);
            this.radioButton60.Name = "radioButton60";
            this.radioButton60.Size = new System.Drawing.Size(70, 22);
            this.radioButton60.TabIndex = 4;
            this.radioButton60.Text = "15 x 15";
            this.radioButton60.UseVisualStyleBackColor = true;
            this.radioButton60.CheckedChanged += new System.EventHandler(this.radioButton60_CheckedChanged);
            // 
            // radioButton40
            // 
            this.radioButton40.AutoSize = true;
            this.radioButton40.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.radioButton40.Location = new System.Drawing.Point(70, 96);
            this.radioButton40.Name = "radioButton40";
            this.radioButton40.Size = new System.Drawing.Size(72, 22);
            this.radioButton40.TabIndex = 3;
            this.radioButton40.Text = "10 x 20";
            this.radioButton40.UseVisualStyleBackColor = true;
            this.radioButton40.CheckedChanged += new System.EventHandler(this.radioButton40_CheckedChanged);
            // 
            // radioButton20
            // 
            this.radioButton20.AutoSize = true;
            this.radioButton20.Checked = true;
            this.radioButton20.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.radioButton20.Location = new System.Drawing.Point(70, 53);
            this.radioButton20.Name = "radioButton20";
            this.radioButton20.Size = new System.Drawing.Size(70, 22);
            this.radioButton20.TabIndex = 2;
            this.radioButton20.TabStop = true;
            this.radioButton20.Text = "10 x 10";
            this.radioButton20.UseVisualStyleBackColor = true;
            this.radioButton20.CheckedChanged += new System.EventHandler(this.radioButton20_CheckedChanged);
            // 
            // Start_button1
            // 
            this.Start_button1.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Start_button1.Location = new System.Drawing.Point(51, 183);
            this.Start_button1.Name = "Start_button1";
            this.Start_button1.Size = new System.Drawing.Size(117, 46);
            this.Start_button1.TabIndex = 0;
            this.Start_button1.Text = global::LunaparkGame.Labels.startButton;
            this.Start_button1.UseVisualStyleBackColor = true;
            this.Start_button1.Click += new System.EventHandler(this.Start_button1_Click);
            // 
            // Start_panel
            // 
            this.Start_panel.Controls.Add(this.exit_button);
            this.Start_panel.Controls.Add(this.loadGame_button);
            this.Start_panel.Controls.Add(this.newGame_button);
            this.Start_panel.Location = new System.Drawing.Point(1, 2);
            this.Start_panel.Name = "Start_panel";
            this.Start_panel.Size = new System.Drawing.Size(300, 242);
            this.Start_panel.TabIndex = 8;
            // 
            // exit_button
            // 
            this.exit_button.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.exit_button.Location = new System.Drawing.Point(19, 113);
            this.exit_button.Name = "exit_button";
            this.exit_button.Size = new System.Drawing.Size(265, 31);
            this.exit_button.TabIndex = 4;
            this.exit_button.Text = global::LunaparkGame.Labels.exitButton;
            this.exit_button.UseVisualStyleBackColor = true;
            this.exit_button.Click += new System.EventHandler(this.exit_button_Click);
            // 
            // loadGame_button
            // 
            this.loadGame_button.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.loadGame_button.Location = new System.Drawing.Point(19, 76);
            this.loadGame_button.Name = "loadGame_button";
            this.loadGame_button.Size = new System.Drawing.Size(265, 31);
            this.loadGame_button.TabIndex = 3;
            this.loadGame_button.Text = global::LunaparkGame.Labels.loadButton;
            this.loadGame_button.UseVisualStyleBackColor = true;
            this.loadGame_button.Click += new System.EventHandler(this.loadGame_button_Click);
            // 
            // newGame_button
            // 
            this.newGame_button.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.newGame_button.Location = new System.Drawing.Point(19, 39);
            this.newGame_button.Name = "newGame_button";
            this.newGame_button.Size = new System.Drawing.Size(265, 31);
            this.newGame_button.TabIndex = 2;
            this.newGame_button.Text = global::LunaparkGame.Labels.newGameMenu;
            this.newGame_button.UseVisualStyleBackColor = true;
            this.newGame_button.Click += new System.EventHandler(this.newGame_button_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "file";
            // 
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(305, 247);
            this.Controls.Add(this.Start_panel);
            this.Controls.Add(this.sizeOfMap_panel);
            this.Name = "StartForm";
            this.Text = Labels.lunaparkUpperBar;
            this.sizeOfMap_panel.ResumeLayout(false);
            this.sizeOfMap_panel.PerformLayout();
            this.Start_panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1Rozmer;
        private System.Windows.Forms.RadioButton radioButton60;
        private System.Windows.Forms.RadioButton radioButton40;
        private System.Windows.Forms.RadioButton radioButton20;
        private System.Windows.Forms.Button Start_button1;
        private System.Windows.Forms.Panel Start_panel;
        private System.Windows.Forms.Button exit_button;
        private System.Windows.Forms.Button loadGame_button;
        private System.Windows.Forms.Button newGame_button;
        private System.Windows.Forms.Panel sizeOfMap_panel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}