namespace LunaparkGame
{
    partial class AmusementBuyForm
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
            this.changeColor_button = new System.Windows.Forms.Button();
            this.build_button = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.info_label = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // changeColor_button
            // 
            this.changeColor_button.Location = new System.Drawing.Point(15, 247);
            this.changeColor_button.Name = "changeColor_button";
            this.changeColor_button.Size = new System.Drawing.Size(105, 25);
            this.changeColor_button.TabIndex = 1;
            this.changeColor_button.Text = global::LunaparkGame.Labels.changeColor;
            this.changeColor_button.UseVisualStyleBackColor = true;
            this.changeColor_button.Click += new System.EventHandler(this.changeColor_button_Click);
            // 
            // build_button
            // 
            this.build_button.Location = new System.Drawing.Point(12, 278);
            this.build_button.Name = "build_button";
            this.build_button.Size = new System.Drawing.Size(134, 32);
            this.build_button.TabIndex = 0;
            this.build_button.Text = global::LunaparkGame.Labels.buildButton;
            this.build_button.UseVisualStyleBackColor = true;
            this.build_button.Click += new System.EventHandler(this.build_button_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox.Location = new System.Drawing.Point(15, 21);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(131, 117);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // info_label
            // 
            this.info_label.AutoSize = true;
            this.info_label.Location = new System.Drawing.Point(12, 151);
            this.info_label.Name = "info_label";
            this.info_label.Size = new System.Drawing.Size(35, 13);
            this.info_label.TabIndex = 3;
            this.info_label.Text = "label1";
            // 
            // AmusementBuyForm
            // 
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(212, 322);
            this.Controls.Add(this.info_label);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.build_button);
            this.Controls.Add(this.changeColor_button);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "AmusementBuyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AmusementBuyForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button changeColor_button;
        private System.Windows.Forms.Button build_button;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label info_label;
        private System.Windows.Forms.ColorDialog colorDialog;
    }
}
