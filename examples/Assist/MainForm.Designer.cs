namespace Assist
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.listenBtn = new System.Windows.Forms.Button();
            this.appLabel = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.InputLabel = new System.Windows.Forms.Label();
            this.OutputLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listenBtn
            // 
            this.listenBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("listenBtn.BackgroundImage")));
            this.listenBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.listenBtn.Location = new System.Drawing.Point(156, 60);
            this.listenBtn.Name = "listenBtn";
            this.listenBtn.Size = new System.Drawing.Size(280, 278);
            this.listenBtn.TabIndex = 0;
            this.listenBtn.UseVisualStyleBackColor = true;
            this.listenBtn.Click += new System.EventHandler(this.listenBtn_Click);
            // 
            // appLabel
            // 
            this.appLabel.AutoSize = true;
            this.appLabel.Location = new System.Drawing.Point(260, 363);
            this.appLabel.Name = "appLabel";
            this.appLabel.Size = new System.Drawing.Size(22, 15);
            this.appLabel.TabIndex = 1;
            this.appLabel.Text = "___";
            this.appLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Location = new System.Drawing.Point(261, 25);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(22, 15);
            this.timeLabel.TabIndex = 2;
            this.timeLabel.Text = "___";
            this.timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InputLabel
            // 
            this.InputLabel.AutoSize = true;
            this.InputLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.InputLabel.ForeColor = System.Drawing.Color.Red;
            this.InputLabel.Location = new System.Drawing.Point(25, 116);
            this.InputLabel.MaximumSize = new System.Drawing.Size(120, 0);
            this.InputLabel.Name = "InputLabel";
            this.InputLabel.Size = new System.Drawing.Size(17, 21);
            this.InputLabel.TabIndex = 3;
            this.InputLabel.Text = "_";
            this.InputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OutputLabel
            // 
            this.OutputLabel.AutoSize = true;
            this.OutputLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.OutputLabel.ForeColor = System.Drawing.Color.Green;
            this.OutputLabel.Location = new System.Drawing.Point(455, 116);
            this.OutputLabel.MaximumSize = new System.Drawing.Size(130, 0);
            this.OutputLabel.Name = "OutputLabel";
            this.OutputLabel.Size = new System.Drawing.Size(17, 21);
            this.OutputLabel.TabIndex = 4;
            this.OutputLabel.Text = "_";
            this.OutputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(601, 408);
            this.Controls.Add(this.OutputLabel);
            this.Controls.Add(this.InputLabel);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.appLabel);
            this.Controls.Add(this.listenBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Wit.AI Voice Assistant";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button listenBtn;
        private System.Windows.Forms.Label appLabel;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Label InputLabel;
        private System.Windows.Forms.Label OutputLabel;
    }
}