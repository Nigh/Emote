﻿namespace NekoHacks
{
    partial class FormConsole
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
            this.flpArg = new System.Windows.Forms.FlowLayoutPanel();
            this.panelCtrl = new System.Windows.Forms.Panel();
            this.btnTime = new System.Windows.Forms.Button();
            this.btnGet = new System.Windows.Forms.Button();
            this.chkWind = new System.Windows.Forms.CheckBox();
            this.panelCtrl.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpArg
            // 
            this.flpArg.AutoScroll = true;
            this.flpArg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpArg.Location = new System.Drawing.Point(0, 0);
            this.flpArg.Name = "flpArg";
            this.flpArg.Size = new System.Drawing.Size(337, 351);
            this.flpArg.TabIndex = 0;
            // 
            // panelCtrl
            // 
            this.panelCtrl.Controls.Add(this.btnTime);
            this.panelCtrl.Controls.Add(this.btnGet);
            this.panelCtrl.Controls.Add(this.chkWind);
            this.panelCtrl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelCtrl.Location = new System.Drawing.Point(0, 351);
            this.panelCtrl.Name = "panelCtrl";
            this.panelCtrl.Size = new System.Drawing.Size(337, 64);
            this.panelCtrl.TabIndex = 1;
            // 
            // btnTime
            // 
            this.btnTime.Location = new System.Drawing.Point(250, 36);
            this.btnTime.Name = "btnTime";
            this.btnTime.Size = new System.Drawing.Size(75, 23);
            this.btnTime.TabIndex = 2;
            this.btnTime.Text = "时间线";
            this.btnTime.UseVisualStyleBackColor = true;
            this.btnTime.Click += new System.EventHandler(this.btnTime_Click);
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(250, 7);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(75, 23);
            this.btnGet.TabIndex = 1;
            this.btnGet.Text = "参数";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // chkWind
            // 
            this.chkWind.AutoSize = true;
            this.chkWind.Checked = true;
            this.chkWind.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWind.Location = new System.Drawing.Point(12, 6);
            this.chkWind.Name = "chkWind";
            this.chkWind.Size = new System.Drawing.Size(68, 18);
            this.chkWind.TabIndex = 0;
            this.chkWind.Text = "起风了";
            this.chkWind.UseVisualStyleBackColor = true;
            this.chkWind.CheckedChanged += new System.EventHandler(this.chkWind_CheckedChanged);
            // 
            // FormConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 415);
            this.Controls.Add(this.flpArg);
            this.Controls.Add(this.panelCtrl);
            this.Name = "FormConsole";
            this.ShowIcon = false;
            this.Text = "猫娘控制台";
            this.Load += new System.EventHandler(this.FormConsole_Load);
            this.panelCtrl.ResumeLayout(false);
            this.panelCtrl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpArg;
        private System.Windows.Forms.Panel panelCtrl;
        private System.Windows.Forms.CheckBox chkWind;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.Button btnTime;
    }
}