namespace Forms2012
{
    partial class Splash1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash1));
            this.lblSplash = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblSplash
            // 
            this.lblSplash.AutoSize = true;
            this.lblSplash.BackColor = System.Drawing.Color.Transparent;
            this.lblSplash.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblSplash.Location = new System.Drawing.Point(58, 143);
            this.lblSplash.Name = "lblSplash";
            this.lblSplash.Size = new System.Drawing.Size(16, 13);
            this.lblSplash.TabIndex = 0;
            this.lblSplash.Text = "...";
            // 
            // Splash1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Forms2012.Properties.Resources.splash;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(597, 165);
            this.Controls.Add(this.lblSplash);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Splash1";
            this.ShowInTaskbar = false;
            this.Text = "Splash1";
            this.Load += new System.EventHandler(this.Splash1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSplash;
    }
}