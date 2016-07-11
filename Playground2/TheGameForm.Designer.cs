using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGame
{
    partial class TheGameForm
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "5",
            "Foobar",
            "The foobar item does stuff",
            "4"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Foo Item",
            "mburke"}, -1);
            this.leaderboard = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.inventory = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblNextMove = new System.Windows.Forms.Label();
            this.btnSchedule = new System.Windows.Forms.Button();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.log = new System.Windows.Forms.ListBox();
            this.moves = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblLastTick = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPollingEnabled = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // leaderboard
            // 
            this.leaderboard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.leaderboard.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.leaderboard.FullRowSelect = true;
            this.leaderboard.HideSelection = false;
            this.leaderboard.Location = new System.Drawing.Point(6, 6);
            this.leaderboard.Margin = new System.Windows.Forms.Padding(2);
            this.leaderboard.Name = "leaderboard";
            this.leaderboard.Size = new System.Drawing.Size(761, 241);
            this.leaderboard.TabIndex = 0;
            this.leaderboard.UseCompatibleStateImageBehavior = false;
            this.leaderboard.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Score";
            this.columnHeader1.Width = 95;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 109;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Effects";
            this.columnHeader3.Width = 176;
            // 
            // inventory
            // 
            this.inventory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inventory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.inventory.FullRowSelect = true;
            this.inventory.HideSelection = false;
            this.inventory.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.inventory.Location = new System.Drawing.Point(11, 271);
            this.inventory.Margin = new System.Windows.Forms.Padding(2);
            this.inventory.Name = "inventory";
            this.inventory.Size = new System.Drawing.Size(429, 317);
            this.inventory.TabIndex = 1;
            this.inventory.UseCompatibleStateImageBehavior = false;
            this.inventory.View = System.Windows.Forms.View.Details;
            this.inventory.DoubleClick += new System.EventHandler(this.btnSchedule_Click);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Count";
            this.columnHeader4.Width = 55;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Name";
            this.columnHeader5.Width = 117;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Description";
            this.columnHeader6.Width = 136;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Rarity";
            // 
            // lblNextMove
            // 
            this.lblNextMove.AutoSize = true;
            this.lblNextMove.Location = new System.Drawing.Point(8, 249);
            this.lblNextMove.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNextMove.Name = "lblNextMove";
            this.lblNextMove.Size = new System.Drawing.Size(35, 13);
            this.lblNextMove.TabIndex = 8;
            this.lblNextMove.Text = "label1";
            // 
            // btnSchedule
            // 
            this.btnSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSchedule.Location = new System.Drawing.Point(451, 339);
            this.btnSchedule.Margin = new System.Windows.Forms.Padding(2);
            this.btnSchedule.Name = "btnSchedule";
            this.btnSchedule.Size = new System.Drawing.Size(52, 23);
            this.btnSchedule.TabIndex = 9;
            this.btnSchedule.Text = "Add";
            this.btnSchedule.UseVisualStyleBackColor = true;
            this.btnSchedule.Click += new System.EventHandler(this.btnSchedule_Click);
            // 
            // txtTarget
            // 
            this.txtTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTarget.Location = new System.Drawing.Point(451, 291);
            this.txtTarget.Margin = new System.Windows.Forms.Padding(2);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(52, 20);
            this.txtTarget.TabIndex = 10;
            // 
            // log
            // 
            this.log.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.log.FormattingEnabled = true;
            this.log.Location = new System.Drawing.Point(11, 604);
            this.log.Margin = new System.Windows.Forms.Padding(2);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(749, 147);
            this.log.TabIndex = 11;
            // 
            // moves
            // 
            this.moves.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.moves.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader9});
            this.moves.FullRowSelect = true;
            this.moves.HideSelection = false;
            listViewItem2.Tag = "Foo Item";
            this.moves.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem2});
            this.moves.Location = new System.Drawing.Point(518, 271);
            this.moves.Name = "moves";
            this.moves.Size = new System.Drawing.Size(164, 317);
            this.moves.TabIndex = 12;
            this.moves.UseCompatibleStateImageBehavior = false;
            this.moves.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Item";
            this.columnHeader8.Width = 93;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Target";
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(451, 367);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(52, 23);
            this.btnRemove.TabIndex = 13;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.Location = new System.Drawing.Point(688, 335);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(75, 23);
            this.btnUp.TabIndex = 14;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Location = new System.Drawing.Point(688, 364);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 23);
            this.btnDown.TabIndex = 15;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblLastTick,
            this.lblPollingEnabled});
            this.statusStrip1.Location = new System.Drawing.Point(0, 764);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(771, 37);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblLastTick
            // 
            this.lblLastTick.Name = "lblLastTick";
            this.lblLastTick.Size = new System.Drawing.Size(238, 32);
            this.lblLastTick.Text = "toolStripStatusLabel1";
            // 
            // lblPollingEnabled
            // 
            this.lblPollingEnabled.DoubleClickEnabled = true;
            this.lblPollingEnabled.Name = "lblPollingEnabled";
            this.lblPollingEnabled.Size = new System.Drawing.Size(238, 32);
            this.lblPollingEnabled.Text = "toolStripStatusLabel2";
            this.lblPollingEnabled.DoubleClick += new System.EventHandler(this.lblPollingEnabled_DoubleClick);
            // 
            // TheGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 801);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.moves);
            this.Controls.Add(this.log);
            this.Controls.Add(this.txtTarget);
            this.Controls.Add(this.btnSchedule);
            this.Controls.Add(this.lblNextMove);
            this.Controls.Add(this.inventory);
            this.Controls.Add(this.leaderboard);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TheGameForm";
            this.Text = "Teh Game";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView leaderboard;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView inventory;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label lblNextMove;
        private System.Windows.Forms.Button btnSchedule;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ListBox log;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ListView moves;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblLastTick;
        private System.Windows.Forms.ToolStripStatusLabel lblPollingEnabled;
    }
}

