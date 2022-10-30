namespace Speak
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
            this.speakBtn = new System.Windows.Forms.Button();
            this.speechBox = new System.Windows.Forms.TextBox();
            this.clearBtn = new System.Windows.Forms.Button();
            this.appLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // speakBtn
            // 
            this.speakBtn.Location = new System.Drawing.Point(376, 146);
            this.speakBtn.Name = "speakBtn";
            this.speakBtn.Size = new System.Drawing.Size(110, 45);
            this.speakBtn.TabIndex = 0;
            this.speakBtn.Text = "Speak!";
            this.speakBtn.UseVisualStyleBackColor = true;
            this.speakBtn.Click += new System.EventHandler(this.speakBtn_Click);
            // 
            // speechBox
            // 
            this.speechBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.speechBox.Location = new System.Drawing.Point(12, 12);
            this.speechBox.Multiline = true;
            this.speechBox.Name = "speechBox";
            this.speechBox.Size = new System.Drawing.Size(473, 128);
            this.speechBox.TabIndex = 1;
            // 
            // clearBtn
            // 
            this.clearBtn.Location = new System.Drawing.Point(12, 146);
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(110, 45);
            this.clearBtn.TabIndex = 2;
            this.clearBtn.Text = "Clear";
            this.clearBtn.UseVisualStyleBackColor = true;
            this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
            // 
            // appLabel
            // 
            this.appLabel.AutoSize = true;
            this.appLabel.Location = new System.Drawing.Point(138, 161);
            this.appLabel.Name = "appLabel";
            this.appLabel.Size = new System.Drawing.Size(22, 15);
            this.appLabel.TabIndex = 3;
            this.appLabel.Text = "___";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 203);
            this.Controls.Add(this.appLabel);
            this.Controls.Add(this.clearBtn);
            this.Controls.Add(this.speechBox);
            this.Controls.Add(this.speakBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Wit.AI Text to Speech";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button speakBtn;
        private System.Windows.Forms.TextBox speechBox;
        private System.Windows.Forms.Button clearBtn;
        private System.Windows.Forms.Label appLabel;
    }
}