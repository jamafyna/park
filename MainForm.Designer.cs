﻿using System.Drawing;
namespace LunaparkGame
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            WeifenLuo.WinFormsUI.Docking.DockPanelSkin dockPanelSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPanelSkin();
            WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin autoHideStripSkin1 = new WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient1 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin dockPaneStripSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient dockPaneStripGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient2 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient2 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient3 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient4 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient5 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient3 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient6 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient7 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            this.firstMenuStrip = new System.Windows.Forms.MenuStrip();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.continueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pause_EToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAs_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vIEWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.amusementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accessoriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hELPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.demolish_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propagate_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.research_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moneyCount_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.peopleCount_toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainDockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.firstMenuStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // firstMenuStrip
            // 
            this.firstMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameToolStripMenuItem,
            this.vIEWToolStripMenuItem,
            this.hELPToolStripMenuItem});
            this.firstMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.firstMenuStrip.Name = "firstMenuStrip";
            this.firstMenuStrip.Size = new System.Drawing.Size(773, 24);
            this.firstMenuStrip.TabIndex = 0;
            this.firstMenuStrip.Text = "Menu";
            this.firstMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.firstMenuStrip_ItemClicked);
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.continueToolStripMenuItem,
            this.pause_EToolStripMenuItem,
            this.newGameToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAs_ToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.gameToolStripMenuItem.Text = global::LunaparkGame.Labels.gameItem;
            this.gameToolStripMenuItem.Click += new System.EventHandler(this.gameToolStripMenuItem_Click);
            // 
            // continueToolStripMenuItem
            // 
            this.continueToolStripMenuItem.Name = "continueToolStripMenuItem";
            this.continueToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.continueToolStripMenuItem.Text = global::LunaparkGame.Labels.continueMenu;
            this.continueToolStripMenuItem.Click += new System.EventHandler(this.continueToolStripMenuItem_Click);
            // 
            // pause_EToolStripMenuItem
            // 
            this.pause_EToolStripMenuItem.Name = "pause_EToolStripMenuItem";
            this.pause_EToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.pause_EToolStripMenuItem.Text = global::LunaparkGame.Labels.pauseMenu;
            this.pause_EToolStripMenuItem.Click += new System.EventHandler(this.pause_EToolStripMenuItem_Click);
            // 
            // newGameToolStripMenuItem
            // 
            this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            this.newGameToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.newGameToolStripMenuItem.Text = global::LunaparkGame.Labels.newGameMenu;
            this.newGameToolStripMenuItem.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveToolStripMenuItem.Text = global::LunaparkGame.Labels.saveMenu;
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAs_ToolStripMenuItem
            // 
            this.saveAs_ToolStripMenuItem.Name = "saveAs_ToolStripMenuItem";
            this.saveAs_ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveAs_ToolStripMenuItem.Text = global::LunaparkGame.Labels.saveAsMenu;
            this.saveAs_ToolStripMenuItem.Click += new System.EventHandler(this.saveAs_ToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.loadToolStripMenuItem.Text = global::LunaparkGame.Labels.loadMenu;
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.quitToolStripMenuItem.Text = global::LunaparkGame.Labels.quitMenu;
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // vIEWToolStripMenuItem
            // 
            this.vIEWToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.amusementsToolStripMenuItem,
            this.pathToolStripMenuItem,
            this.accessoriesToolStripMenuItem});
            this.vIEWToolStripMenuItem.Name = "vIEWToolStripMenuItem";
            this.vIEWToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.vIEWToolStripMenuItem.Text = global::LunaparkGame.Labels.viewItem;
            // 
            // amusementsToolStripMenuItem
            // 
            this.amusementsToolStripMenuItem.Checked = true;
            this.amusementsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.amusementsToolStripMenuItem.Name = "amusementsToolStripMenuItem";
            this.amusementsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.amusementsToolStripMenuItem.Text = global::LunaparkGame.Labels.amusementButton;
            this.amusementsToolStripMenuItem.Click += new System.EventHandler(this.amusementsToolStripMenuItem_Click);
            // 
            // pathToolStripMenuItem
            // 
            this.pathToolStripMenuItem.Checked = true;
            this.pathToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pathToolStripMenuItem.Name = "pathToolStripMenuItem";
            this.pathToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pathToolStripMenuItem.Text = global::LunaparkGame.Labels.pathButton;
            this.pathToolStripMenuItem.Click += new System.EventHandler(this.pathToolStripMenuItem_Click);
            // 
            // accessoriesToolStripMenuItem
            // 
            this.accessoriesToolStripMenuItem.Name = "accessoriesToolStripMenuItem";
            this.accessoriesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.accessoriesToolStripMenuItem.Text = global::LunaparkGame.Labels.accessories;
            this.accessoriesToolStripMenuItem.Click += new System.EventHandler(this.accessoriesToolStripMenuItem_Click);
            // 
            // hELPToolStripMenuItem
            // 
            this.hELPToolStripMenuItem.Name = "hELPToolStripMenuItem";
            this.hELPToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.hELPToolStripMenuItem.Text = global::LunaparkGame.Labels.helpItem;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.demolish_toolStripMenuItem,
            this.propagate_toolStripMenuItem,
            this.research_toolStripMenuItem,
            this.toolStripMenuItem1,
            this.ToolStripMenuItem,
            this.moneyCount_toolStripMenuItem,
            this.ToolStripMenuItem2,
            this.peopleCount_toolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 24);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(773, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "MenuStrip2";
            // 
            // demolish_toolStripMenuItem
            // 
            this.demolish_toolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.demolish_toolStripMenuItem.Name = "demolish_toolStripMenuItem";
            this.demolish_toolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.demolish_toolStripMenuItem.Text = global::LunaparkGame.Labels.demolishStart;
            this.demolish_toolStripMenuItem.Click += new System.EventHandler(this.demolish_toolStripMenuItem_Click);
            // 
            // propagate_toolStripMenuItem
            // 
            this.propagate_toolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.propagate_toolStripMenuItem.Name = "propagate_toolStripMenuItem";
            this.propagate_toolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.propagate_toolStripMenuItem.Text = global::LunaparkGame.Labels.advertiseStart;
            this.propagate_toolStripMenuItem.Click += new System.EventHandler(this.propagate_toolStripMenuItem_Click);
            // 
            // research_toolStripMenuItem
            // 
            this.research_toolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.research_toolStripMenuItem.Name = "research_toolStripMenuItem";
            this.research_toolStripMenuItem.Size = new System.Drawing.Size(90, 20);
            this.research_toolStripMenuItem.Text = global::LunaparkGame.Labels.researchStart;
            this.research_toolStripMenuItem.Click += new System.EventHandler(this.research_toolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(106, 20);
            this.toolStripMenuItem1.Text = "                             ";
            // 
            // ToolStripMenuItem
            // 
            this.ToolStripMenuItem.Name = "ToolStripMenuItem";
            this.ToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.ToolStripMenuItem.Text = global::LunaparkGame.Labels.moneyCount;
            // 
            // moneyCount_toolStripMenuItem
            // 
            this.moneyCount_toolStripMenuItem.Name = "moneyCount_toolStripMenuItem";
            this.moneyCount_toolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.moneyCount_toolStripMenuItem.Text = "2000";
            // 
            // ToolStripMenuItem2
            // 
            this.ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            this.ToolStripMenuItem2.Size = new System.Drawing.Size(61, 20);
            this.ToolStripMenuItem2.Text = global::LunaparkGame.Labels.peopleCount;
            // 
            // peopleCount_toolStripMenuItem
            // 
            this.peopleCount_toolStripMenuItem.Name = "peopleCount_toolStripMenuItem";
            this.peopleCount_toolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.peopleCount_toolStripMenuItem.Text = "20000";
            // 
            // mainDockPanel
            // 
            this.mainDockPanel.ActiveAutoHideContent = null;
            this.mainDockPanel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.mainDockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainDockPanel.DockBackColor = System.Drawing.SystemColors.Control;
            this.mainDockPanel.Location = new System.Drawing.Point(0, 48);
            this.mainDockPanel.Name = "mainDockPanel";
            this.mainDockPanel.Size = new System.Drawing.Size(773, 338);
            dockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight;
            autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
            tabGradient1.EndColor = System.Drawing.SystemColors.Control;
            tabGradient1.StartColor = System.Drawing.SystemColors.Control;
            tabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            autoHideStripSkin1.TabGradient = tabGradient1;
            dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
            tabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
            dockPanelGradient2.EndColor = System.Drawing.SystemColors.Control;
            dockPanelGradient2.StartColor = System.Drawing.SystemColors.Control;
            dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
            tabGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
            dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
            tabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption;
            tabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption;
            tabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
            dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
            tabGradient5.EndColor = System.Drawing.SystemColors.Control;
            tabGradient5.StartColor = System.Drawing.SystemColors.Control;
            tabGradient5.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
            dockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
            tabGradient6.EndColor = System.Drawing.SystemColors.GradientInactiveCaption;
            tabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption;
            tabGradient6.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
            tabGradient7.EndColor = System.Drawing.Color.Transparent;
            tabGradient7.StartColor = System.Drawing.Color.Transparent;
            tabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
            dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
            dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
            this.mainDockPanel.Skin = dockPanelSkin1;
            this.mainDockPanel.TabIndex = 14;
            // 
            // timer
            // 
            this.timer.Interval = 16;
            this.timer.Tag = "";
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 386);
            this.Controls.Add(this.mainDockPanel);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.firstMenuStrip);
            this.MainMenuStrip = this.firstMenuStrip;
            this.Name = "MainForm";
            this.Text = "Lunapark";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing_1);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.firstMenuStrip.ResumeLayout(false);
            this.firstMenuStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip firstMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem continueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vIEWToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem amusementsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem accessoriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hELPToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem demolish_toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propagate_toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem research_toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moneyCount_toolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem peopleCount_toolStripMenuItem;
        public WeifenLuo.WinFormsUI.Docking.DockPanel mainDockPanel;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripMenuItem saveAs_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pause_EToolStripMenuItem;
    }
}

