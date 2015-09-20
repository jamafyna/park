namespace LunaparkGame {
    partial class AmusementDetailForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.prize_numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.outOfService_button = new System.Windows.Forms.Button();
            this.prize_label = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.info_label = new System.Windows.Forms.Label();
            this.entrance_button = new System.Windows.Forms.Button();
            this.exit_button = new System.Windows.Forms.Button();
            this.damageText_label = new System.Windows.Forms.Label();
            this.damageValue_label = new System.Windows.Forms.Label();
            this.repair_button = new System.Windows.Forms.Button();
            this.queueValue_label = new System.Windows.Forms.Label();
            this.queueText_label = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.prize_numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // prize_numericUpDown1
            // 
            this.prize_numericUpDown1.Location = new System.Drawing.Point(92, 201);
            this.prize_numericUpDown1.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.prize_numericUpDown1.Name = "prize_numericUpDown1";
            this.prize_numericUpDown1.Size = new System.Drawing.Size(85, 20);
            this.prize_numericUpDown1.TabIndex = 5;
            this.prize_numericUpDown1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.prize_numericUpDown1.ValueChanged += new System.EventHandler(this.prize_numericUpDown1_ValueChanged);
            // 
            // outOfService_button
            // 
            this.outOfService_button.BackColor = System.Drawing.Color.Red;
            this.outOfService_button.Font = new System.Drawing.Font("Comic Sans MS", 8.25F);
            this.outOfService_button.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.outOfService_button.Location = new System.Drawing.Point(6, 150);
            this.outOfService_button.Name = "outOfService_button";
            this.outOfService_button.Size = new System.Drawing.Size(114, 40);
            this.outOfService_button.TabIndex = 6;
            this.outOfService_button.Text = global::LunaparkGame.Labels.outOfService;
            this.outOfService_button.UseVisualStyleBackColor = false;
            this.outOfService_button.Click += new System.EventHandler(this.outOfService_button_Click);
            // 
            // prize_label
            // 
            this.prize_label.AutoSize = true;
            this.prize_label.Location = new System.Drawing.Point(12, 204);
            this.prize_label.Name = "prize_label";
            this.prize_label.Size = new System.Drawing.Size(74, 13);
            this.prize_label.TabIndex = 7;
            this.prize_label.Text = Labels.entranceFee;
            // 
            // pictureBox
            // 
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox.Location = new System.Drawing.Point(6, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(171, 121);
            this.pictureBox.TabIndex = 8;
            this.pictureBox.TabStop = false;
            // 
            // info_label
            // 
            this.info_label.AutoSize = true;
            this.info_label.Location = new System.Drawing.Point(12, 273);
            this.info_label.Name = "info_label";
            this.info_label.Size = new System.Drawing.Size(0, 13);
            this.info_label.TabIndex = 9;
            // 
            // entrance_button
            // 
            this.entrance_button.Location = new System.Drawing.Point(5, 4);
            this.entrance_button.Name = "entrance_button";
            this.entrance_button.Size = new System.Drawing.Size(75, 23);
            this.entrance_button.TabIndex = 10;
            this.entrance_button.Text = global::LunaparkGame.Labels.entrainceBuild;
            this.entrance_button.UseVisualStyleBackColor = true;
            this.entrance_button.Visible = false;
            this.entrance_button.Click += new System.EventHandler(this.entrance_button_Click);
            // 
            // exit_button
            // 
            this.exit_button.Location = new System.Drawing.Point(101, 4);
            this.exit_button.Name = "exit_button";
            this.exit_button.Size = new System.Drawing.Size(75, 23);
            this.exit_button.TabIndex = 10;
            this.exit_button.Text = global::LunaparkGame.Labels.exitBuild;
            this.exit_button.UseVisualStyleBackColor = true;
            this.exit_button.Visible = false;
            this.exit_button.Click += new System.EventHandler(this.exit_button_Click);
            // 
            // damageText_label
            // 
            this.damageText_label.AutoSize = true;
            this.damageText_label.Location = new System.Drawing.Point(12, 226);
            this.damageText_label.Name = "damageText_label";
            this.damageText_label.Size = new System.Drawing.Size(53, 13);
            this.damageText_label.TabIndex = 12;
            this.damageText_label.Text = Labels.damage;
            // 
            // damageValue_label
            // 
            this.damageValue_label.AutoSize = true;
            this.damageValue_label.Location = new System.Drawing.Point(89, 226);
            this.damageValue_label.Name = "damageValue_label";
            this.damageValue_label.Size = new System.Drawing.Size(13, 13);
            this.damageValue_label.TabIndex = 13;
            this.damageValue_label.Text = "0";
            // 
            // repair_button
            // 
            this.repair_button.BackColor = System.Drawing.Color.Red;
            this.repair_button.Font = new System.Drawing.Font("Comic Sans MS", 8.25F);
            this.repair_button.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.repair_button.Location = new System.Drawing.Point(6, 268);
            this.repair_button.Name = "repair_button";
            this.repair_button.Size = new System.Drawing.Size(75, 25);
            this.repair_button.TabIndex = 10;
            this.repair_button.Text = global::LunaparkGame.Labels.repairButton;
            this.repair_button.UseVisualStyleBackColor = false;
            this.repair_button.Click += new System.EventHandler(this.repareButton_Click);
            // 
            // queueValue_label
            // 
            this.queueValue_label.AutoSize = true;
            this.queueValue_label.Location = new System.Drawing.Point(89, 246);
            this.queueValue_label.Name = "queueValue_label";
            this.queueValue_label.Size = new System.Drawing.Size(13, 13);
            this.queueValue_label.TabIndex = 16;
            this.queueValue_label.Text = "0";
            // 
            // queueText_label
            // 
            this.queueText_label.AutoSize = true;
            this.queueText_label.Location = new System.Drawing.Point(12, 246);
            this.queueText_label.Name = "queueText_label";
            this.queueText_label.Size = new System.Drawing.Size(81, 13);
            this.queueText_label.TabIndex = 15;
            this.queueText_label.Text = Labels.peopleInQueue;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.entrance_button);
            this.panel1.Controls.Add(this.exit_button);
            this.panel1.Location = new System.Drawing.Point(1, 299);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(176, 30);
            this.panel1.TabIndex = 17;
            // 
            // AmusementDetailForm
            // 
            this.AutoScroll = true;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(186, 341);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.queueValue_label);
            this.Controls.Add(this.queueText_label);
            this.Controls.Add(this.repair_button);
            this.Controls.Add(this.damageValue_label);
            this.Controls.Add(this.damageText_label);
            this.Controls.Add(this.info_label);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.prize_label);
            this.Controls.Add(this.outOfService_button);
            this.Controls.Add(this.prize_numericUpDown1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaximizeBox = false;
            this.Name = "AmusementDetailForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AmusementDetailForm_FormClosing);
            this.Load += new System.EventHandler(this.AmusementDetailForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.prize_numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.NumericUpDown prize_numericUpDown1;
        public System.Windows.Forms.Button outOfService_button;
        private System.Windows.Forms.Label prize_label;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label info_label;
        private System.Windows.Forms.Button entrance_button;
        private System.Windows.Forms.Button exit_button;
        private System.Windows.Forms.Label damageText_label;
        private System.Windows.Forms.Label damageValue_label;
        public System.Windows.Forms.Button repair_button;
        private System.Windows.Forms.Label queueValue_label;
        private System.Windows.Forms.Label queueText_label;
        private System.Windows.Forms.Panel panel1;
    }
}
