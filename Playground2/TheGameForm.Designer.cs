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
            this.leaderboard = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.inventory = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblNextMove = new System.Windows.Forms.Label();
            this.btnSchedule = new System.Windows.Forms.Button();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.log = new System.Windows.Forms.ListBox();
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
            this.leaderboard.Location = new System.Drawing.Point(12, 12);
            this.leaderboard.Name = "leaderboard";
            this.leaderboard.Size = new System.Drawing.Size(847, 307);
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
            this.inventory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.inventory.FullRowSelect = true;
            this.inventory.HideSelection = false;
            this.inventory.Location = new System.Drawing.Point(12, 367);
            this.inventory.Name = "inventory";
            this.inventory.Size = new System.Drawing.Size(685, 175);
            this.inventory.TabIndex = 1;
            this.inventory.UseCompatibleStateImageBehavior = false;
            this.inventory.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Count";
            this.columnHeader4.Width = 99;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Name";
            this.columnHeader5.Width = 257;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Rarity";
            // 
            // lblNextMove
            // 
            this.lblNextMove.AutoSize = true;
            this.lblNextMove.Location = new System.Drawing.Point(12, 322);
            this.lblNextMove.Name = "lblNextMove";
            this.lblNextMove.Size = new System.Drawing.Size(70, 25);
            this.lblNextMove.TabIndex = 8;
            this.lblNextMove.Text = "label1";
            // 
            // btnSchedule
            // 
            this.btnSchedule.Location = new System.Drawing.Point(746, 432);
            this.btnSchedule.Name = "btnSchedule";
            this.btnSchedule.Size = new System.Drawing.Size(75, 33);
            this.btnSchedule.TabIndex = 9;
            this.btnSchedule.Text = "button1";
            this.btnSchedule.UseVisualStyleBackColor = true;
            this.btnSchedule.Click += new System.EventHandler(this.btnSchedule_Click);
            // 
            // txtTarget
            // 
            this.txtTarget.Location = new System.Drawing.Point(731, 367);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(100, 31);
            this.txtTarget.TabIndex = 10;
            // 
            // log
            // 
            this.log.FormattingEnabled = true;
            this.log.ItemHeight = 25;
            this.log.Location = new System.Drawing.Point(12, 548);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(847, 129);
            this.log.TabIndex = 11;
            // 
            // TheGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 700);
            this.Controls.Add(this.log);
            this.Controls.Add(this.txtTarget);
            this.Controls.Add(this.btnSchedule);
            this.Controls.Add(this.lblNextMove);
            this.Controls.Add(this.inventory);
            this.Controls.Add(this.leaderboard);
            this.Name = "TheGameForm";
            this.Text = "Teh Game";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
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
    }
}

