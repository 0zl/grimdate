
namespace grimdate
{
    partial class UI
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
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.CheckLabel = new System.Windows.Forms.Label();
            this.DownloadingGrim = new System.Windows.Forms.Label();
            this.HardUpdateDone = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(12, 38);
            this.ProgressBar.MarqueeAnimationSpeed = 1;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(213, 10);
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ProgressBar.TabIndex = 0;
            this.ProgressBar.UseWaitCursor = true;
            // 
            // CheckLabel
            // 
            this.CheckLabel.AutoSize = true;
            this.CheckLabel.Location = new System.Drawing.Point(62, 13);
            this.CheckLabel.Name = "CheckLabel";
            this.CheckLabel.Size = new System.Drawing.Size(110, 13);
            this.CheckLabel.TabIndex = 1;
            this.CheckLabel.Text = "Checking for Updates";
            this.CheckLabel.UseWaitCursor = true;
            // 
            // DownloadingGrim
            // 
            this.DownloadingGrim.AutoSize = true;
            this.DownloadingGrim.Location = new System.Drawing.Point(63, 13);
            this.DownloadingGrim.Name = "DownloadingGrim";
            this.DownloadingGrim.Size = new System.Drawing.Size(110, 13);
            this.DownloadingGrim.TabIndex = 2;
            this.DownloadingGrim.Text = "Downloading GrimLite";
            this.DownloadingGrim.UseWaitCursor = true;
            this.DownloadingGrim.Visible = false;
            // 
            // HardUpdateDone
            // 
            this.HardUpdateDone.AutoSize = true;
            this.HardUpdateDone.Location = new System.Drawing.Point(58, 13);
            this.HardUpdateDone.Name = "HardUpdateDone";
            this.HardUpdateDone.Size = new System.Drawing.Size(123, 13);
            this.HardUpdateDone.TabIndex = 3;
            this.HardUpdateDone.Text = "Downloaded. Have Fun.";
            this.HardUpdateDone.UseWaitCursor = true;
            this.HardUpdateDone.Visible = false;
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.ClientSize = new System.Drawing.Size(237, 60);
            this.Controls.Add(this.HardUpdateDone);
            this.Controls.Add(this.DownloadingGrim);
            this.Controls.Add(this.CheckLabel);
            this.Controls.Add(this.ProgressBar);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "UI";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GrimLite";
            this.TopMost = true;
            this.UseWaitCursor = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UI_FormClosing);
            this.Load += new System.EventHandler(this.Updater_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Label CheckLabel;
        private System.Windows.Forms.Label DownloadingGrim;
        private System.Windows.Forms.Label HardUpdateDone;
    }
}

