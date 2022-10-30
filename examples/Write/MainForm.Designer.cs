namespace Write
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
            this.appLabel = new System.Windows.Forms.Label();
            this.dictateBtn = new System.Windows.Forms.Button();
            this.speechBox = new System.Windows.Forms.TextBox();
            this.clearBtn = new System.Windows.Forms.Button();
            this.stopBtn = new System.Windows.Forms.Button();
            this.voiceBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // appLabel
            // 
            this.appLabel.AutoSize = true;
            this.appLabel.Location = new System.Drawing.Point(153, 279);
            this.appLabel.Name = "appLabel";
            this.appLabel.Size = new System.Drawing.Size(22, 15);
            this.appLabel.TabIndex = 0;
            this.appLabel.Text = "___";
            // 
            // dictateBtn
            // 
            this.dictateBtn.Location = new System.Drawing.Point(474, 266);
            this.dictateBtn.Name = "dictateBtn";
            this.dictateBtn.Size = new System.Drawing.Size(123, 41);
            this.dictateBtn.TabIndex = 1;
            this.dictateBtn.Text = "Dictate!";
            this.dictateBtn.UseVisualStyleBackColor = true;
            this.dictateBtn.Click += new System.EventHandler(this.dictateBtn_Click);
            // 
            // speechBox
            // 
            this.speechBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.speechBox.Location = new System.Drawing.Point(12, 49);
            this.speechBox.Multiline = true;
            this.speechBox.Name = "speechBox";
            this.speechBox.Size = new System.Drawing.Size(585, 199);
            this.speechBox.TabIndex = 2;
            // 
            // clearBtn
            // 
            this.clearBtn.Location = new System.Drawing.Point(12, 266);
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(123, 41);
            this.clearBtn.TabIndex = 3;
            this.clearBtn.Text = "Clear";
            this.clearBtn.UseVisualStyleBackColor = true;
            this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
            // 
            // stopBtn
            // 
            this.stopBtn.Enabled = false;
            this.stopBtn.Location = new System.Drawing.Point(345, 266);
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.Size = new System.Drawing.Size(123, 41);
            this.stopBtn.TabIndex = 4;
            this.stopBtn.Text = "Stop";
            this.stopBtn.UseVisualStyleBackColor = true;
            this.stopBtn.Click += new System.EventHandler(this.stopBtn_Click);
            // 
            // voiceBar
            // 
            this.voiceBar.Location = new System.Drawing.Point(82, 14);
            this.voiceBar.Name = "voiceBar";
            this.voiceBar.Size = new System.Drawing.Size(515, 23);
            this.voiceBar.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Volume";
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Location = new System.Drawing.Point(238, 279);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(22, 15);
            this.timeLabel.TabIndex = 7;
            this.timeLabel.Text = "___";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 324);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.voiceBar);
            this.Controls.Add(this.stopBtn);
            this.Controls.Add(this.clearBtn);
            this.Controls.Add(this.speechBox);
            this.Controls.Add(this.dictateBtn);
            this.Controls.Add(this.appLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Wit.AI Speech to Text";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label appLabel;
        private System.Windows.Forms.Button dictateBtn;
        private System.Windows.Forms.TextBox speechBox;
        private System.Windows.Forms.Button clearBtn;
        private System.Windows.Forms.Button stopBtn;
        private System.Windows.Forms.ProgressBar voiceBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label timeLabel;
    }
}