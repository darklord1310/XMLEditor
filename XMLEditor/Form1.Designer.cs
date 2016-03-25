﻿namespace XMLEditor
{
    partial class Form1
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTcDesc = new System.Windows.Forms.Label();
            this.txtBoxTcDesc = new System.Windows.Forms.TextBox();
            this.lblSqDesc = new System.Windows.Forms.Label();
            this.txtBoxSqDesc = new System.Windows.Forms.TextBox();
            this.lblDiagCmd = new System.Windows.Forms.Label();
            this.txtBoxCat = new System.Windows.Forms.TextBox();
            this.txtBoxMod = new System.Windows.Forms.TextBox();
            this.cBoxFunc = new System.Windows.Forms.ComboBox();
            this.lblPara = new System.Windows.Forms.Label();
            this.txtBoxPara = new System.Windows.Forms.TextBox();
            this.lblExpOut = new System.Windows.Forms.Label();
            this.txtBoxExpOut = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.treeView1 = new XMLEditor.DropDownTreeView();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.databaseToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(507, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editCategoryToolStripMenuItem});
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.databaseToolStripMenuItem.Text = "Open";
            // 
            // editCategoryToolStripMenuItem
            // 
            this.editCategoryToolStripMenuItem.Name = "editCategoryToolStripMenuItem";
            this.editCategoryToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.editCategoryToolStripMenuItem.Text = "Open CSV file";
            this.editCategoryToolStripMenuItem.Click += new System.EventHandler(this.editCategoryToolStripMenuItem_Click);
            // 
            // lblTcDesc
            // 
            this.lblTcDesc.AutoSize = true;
            this.lblTcDesc.Location = new System.Drawing.Point(270, 42);
            this.lblTcDesc.Name = "lblTcDesc";
            this.lblTcDesc.Size = new System.Drawing.Size(111, 13);
            this.lblTcDesc.TabIndex = 4;
            this.lblTcDesc.Text = "Test Case Description";
            // 
            // txtBoxTcDesc
            // 
            this.txtBoxTcDesc.Enabled = false;
            this.txtBoxTcDesc.Location = new System.Drawing.Point(269, 58);
            this.txtBoxTcDesc.Multiline = true;
            this.txtBoxTcDesc.Name = "txtBoxTcDesc";
            this.txtBoxTcDesc.Size = new System.Drawing.Size(226, 42);
            this.txtBoxTcDesc.TabIndex = 5;
            this.txtBoxTcDesc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBoxTcDesc_KeyDown);
            // 
            // lblSqDesc
            // 
            this.lblSqDesc.AutoSize = true;
            this.lblSqDesc.Location = new System.Drawing.Point(270, 120);
            this.lblSqDesc.Name = "lblSqDesc";
            this.lblSqDesc.Size = new System.Drawing.Size(99, 13);
            this.lblSqDesc.TabIndex = 6;
            this.lblSqDesc.Text = "Seq No Description";
            // 
            // txtBoxSqDesc
            // 
            this.txtBoxSqDesc.Enabled = false;
            this.txtBoxSqDesc.Location = new System.Drawing.Point(269, 136);
            this.txtBoxSqDesc.Multiline = true;
            this.txtBoxSqDesc.Name = "txtBoxSqDesc";
            this.txtBoxSqDesc.Size = new System.Drawing.Size(226, 42);
            this.txtBoxSqDesc.TabIndex = 7;
            this.txtBoxSqDesc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBoxSqDesc_KeyDown);
            // 
            // lblDiagCmd
            // 
            this.lblDiagCmd.AutoSize = true;
            this.lblDiagCmd.Location = new System.Drawing.Point(270, 191);
            this.lblDiagCmd.Name = "lblDiagCmd";
            this.lblDiagCmd.Size = new System.Drawing.Size(107, 13);
            this.lblDiagCmd.TabIndex = 8;
            this.lblDiagCmd.Text = "Diagnostic Command";
            // 
            // txtBoxCat
            // 
            this.txtBoxCat.Enabled = false;
            this.txtBoxCat.Location = new System.Drawing.Point(269, 208);
            this.txtBoxCat.Name = "txtBoxCat";
            this.txtBoxCat.Size = new System.Drawing.Size(47, 20);
            this.txtBoxCat.TabIndex = 9;
            // 
            // txtBoxMod
            // 
            this.txtBoxMod.Enabled = false;
            this.txtBoxMod.Location = new System.Drawing.Point(322, 208);
            this.txtBoxMod.Name = "txtBoxMod";
            this.txtBoxMod.Size = new System.Drawing.Size(59, 20);
            this.txtBoxMod.TabIndex = 10;
            // 
            // cBoxFunc
            // 
            this.cBoxFunc.Enabled = false;
            this.cBoxFunc.FormattingEnabled = true;
            this.cBoxFunc.Location = new System.Drawing.Point(387, 207);
            this.cBoxFunc.Name = "cBoxFunc";
            this.cBoxFunc.Size = new System.Drawing.Size(108, 21);
            this.cBoxFunc.TabIndex = 11;
            this.cBoxFunc.DropDown += new System.EventHandler(this.cBoxFunc_DropDown);
            this.cBoxFunc.SelectedIndexChanged += new System.EventHandler(this.cBoxFunc_SelectedIndexChanged);
            // 
            // lblPara
            // 
            this.lblPara.AutoSize = true;
            this.lblPara.Location = new System.Drawing.Point(269, 235);
            this.lblPara.Name = "lblPara";
            this.lblPara.Size = new System.Drawing.Size(55, 13);
            this.lblPara.TabIndex = 12;
            this.lblPara.Text = "Parameter";
            // 
            // txtBoxPara
            // 
            this.txtBoxPara.Enabled = false;
            this.txtBoxPara.Location = new System.Drawing.Point(269, 251);
            this.txtBoxPara.Multiline = true;
            this.txtBoxPara.Name = "txtBoxPara";
            this.txtBoxPara.Size = new System.Drawing.Size(226, 42);
            this.txtBoxPara.TabIndex = 13;
            this.txtBoxPara.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBoxPara_KeyDown);
            // 
            // lblExpOut
            // 
            this.lblExpOut.AutoSize = true;
            this.lblExpOut.Location = new System.Drawing.Point(269, 300);
            this.lblExpOut.Name = "lblExpOut";
            this.lblExpOut.Size = new System.Drawing.Size(98, 13);
            this.lblExpOut.TabIndex = 14;
            this.lblExpOut.Text = "Expected Outcome";
            // 
            // txtBoxExpOut
            // 
            this.txtBoxExpOut.Enabled = false;
            this.txtBoxExpOut.Location = new System.Drawing.Point(269, 316);
            this.txtBoxExpOut.Multiline = true;
            this.txtBoxExpOut.Name = "txtBoxExpOut";
            this.txtBoxExpOut.Size = new System.Drawing.Size(226, 42);
            this.txtBoxExpOut.TabIndex = 15;
            this.txtBoxExpOut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBoxExpOut_KeyDown);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(269, 367);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.SystemColors.Control;
            this.treeView1.Location = new System.Drawing.Point(13, 42);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(231, 316);
            this.treeView1.TabIndex = 3;
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
            this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 402);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtBoxExpOut);
            this.Controls.Add(this.lblExpOut);
            this.Controls.Add(this.txtBoxPara);
            this.Controls.Add(this.lblPara);
            this.Controls.Add(this.cBoxFunc);
            this.Controls.Add(this.txtBoxMod);
            this.Controls.Add(this.txtBoxCat);
            this.Controls.Add(this.lblDiagCmd);
            this.Controls.Add(this.txtBoxSqDesc);
            this.Controls.Add(this.lblSqDesc);
            this.Controls.Add(this.txtBoxTcDesc);
            this.Controls.Add(this.lblTcDesc);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XML Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem databaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editCategoryToolStripMenuItem;
        private DropDownTreeView treeView1;
        private System.Windows.Forms.Label lblTcDesc;
        private System.Windows.Forms.TextBox txtBoxTcDesc;
        private System.Windows.Forms.Label lblSqDesc;
        private System.Windows.Forms.TextBox txtBoxSqDesc;
        private System.Windows.Forms.Label lblDiagCmd;
        private System.Windows.Forms.TextBox txtBoxCat;
        private System.Windows.Forms.TextBox txtBoxMod;
        private System.Windows.Forms.ComboBox cBoxFunc;
        private System.Windows.Forms.Label lblPara;
        private System.Windows.Forms.TextBox txtBoxPara;
        private System.Windows.Forms.Label lblExpOut;
        private System.Windows.Forms.TextBox txtBoxExpOut;
        private System.Windows.Forms.Button button1;
    }
}

