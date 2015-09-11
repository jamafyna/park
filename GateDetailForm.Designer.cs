namespace LunaparkGame {
    partial class GateDetailForm {
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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.prize_label = new System.Windows.Forms.Label();
            this.outOfService_button = new System.Windows.Forms.Button();
            this.prize_numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.info_label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.prize_numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(8, 11);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(171, 121);
            this.pictureBox.TabIndex = 11;
            this.pictureBox.TabStop = false;
            // 
            // prize_label
            // 
            this.prize_label.AutoSize = true;
            this.prize_label.Location = new System.Drawing.Point(5, 203);
            this.prize_label.Name = "prize_label";
            this.prize_label.Size = new System.Drawing.Size(74, 13);
            this.prize_label.TabIndex = 10;
            this.prize_label.Text = "Entrance fee: ";
            // 
            // outOfService_button
            // 
            this.outOfService_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outOfService_button.BackColor = System.Drawing.Color.Red;
            this.outOfService_button.Font = new System.Drawing.Font("Comic Sans MS", 8.25F);
            this.outOfService_button.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.outOfService_button.Location = new System.Drawing.Point(7, 149);
            this.outOfService_button.Name = "outOfService_button";
            this.outOfService_button.Size = new System.Drawing.Size(114, 40);
            this.outOfService_button.TabIndex = 9;
            this.outOfService_button.Text = global::LunaparkGame.Labels.outOfService;
            this.outOfService_button.UseVisualStyleBackColor = false;
            this.outOfService_button.Click += new System.EventHandler(this.outOfService_button_Click);
            // 
            // prize_numericUpDown1
            // 
            this.prize_numericUpDown1.Location = new System.Drawing.Point(90, 201);
            this.prize_numericUpDown1.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.prize_numericUpDown1.Name = "prize_numericUpDown1";
            this.prize_numericUpDown1.Size = new System.Drawing.Size(85, 20);
            this.prize_numericUpDown1.TabIndex = 12;
            this.prize_numericUpDown1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.prize_numericUpDown1.ValueChanged += new System.EventHandler(this.prize_numericUpDown1_ValueChanged);
            // 
            // info_label
            // 
            this.info_label.AutoSize = true;
            this.info_label.Location = new System.Drawing.Point(5, 225);
            this.info_label.Name = "info_label";
            this.info_label.Size = new System.Drawing.Size(74, 13);
            this.info_label.TabIndex = 13;
            this.info_label.Text = "Entrance fee: ";
            // 
            // GateDetailForm
            // 
            this.ClientSize = new System.Drawing.Size(188, 262);
            this.Controls.Add(this.info_label);
            this.Controls.Add(this.prize_numericUpDown1);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.prize_label);
            this.Controls.Add(this.outOfService_button);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "GateDetailForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GateDetailForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.prize_numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label prize_label;
        public System.Windows.Forms.Button outOfService_button;
        public System.Windows.Forms.NumericUpDown prize_numericUpDown1;
        private System.Windows.Forms.Label info_label;
    }
}
