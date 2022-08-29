namespace nm_AD_UID
{
    partial class frmAD_UID
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
            this.tabCtrl = new System.Windows.Forms.TabControl();
            this.tbNoUID = new System.Windows.Forms.TabPage();
            this.pnlDataNoUI = new System.Windows.Forms.Panel();
            this.tbUID = new System.Windows.Forms.TabPage();
            this.pnlDataUI = new System.Windows.Forms.Panel();
            this.tabCtrl.SuspendLayout();
            this.tbNoUID.SuspendLayout();
            this.tbUID.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabCtrl
            // 
            this.tabCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCtrl.Controls.Add(this.tbUID);
            this.tabCtrl.Controls.Add(this.tbNoUID);
            this.tabCtrl.Location = new System.Drawing.Point(0, 0);
            this.tabCtrl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.SelectedIndex = 0;
            this.tabCtrl.Size = new System.Drawing.Size(798, 450);
            this.tabCtrl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabCtrl.TabIndex = 0;
            // 
            // tbNoUID
            // 
            this.tbNoUID.AutoScroll = true;
            this.tbNoUID.Controls.Add(this.pnlDataNoUI);
            this.tbNoUID.Location = new System.Drawing.Point(4, 22);
            this.tbNoUID.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbNoUID.Name = "tbNoUID";
            this.tbNoUID.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbNoUID.Size = new System.Drawing.Size(790, 424);
            this.tbNoUID.TabIndex = 0;
            this.tbNoUID.Text = "Users without UID";
            this.tbNoUID.UseVisualStyleBackColor = true;
            // 
            // pnlDataNoUI
            // 
            this.pnlDataNoUI.AutoSize = true;
            this.pnlDataNoUI.Location = new System.Drawing.Point(5, 27);
            this.pnlDataNoUI.Name = "pnlDataNoUI";
            this.pnlDataNoUI.Size = new System.Drawing.Size(515, 361);
            this.pnlDataNoUI.TabIndex = 0;
            // 
            // tbUID
            // 
            this.tbUID.AutoScroll = true;
            this.tbUID.Controls.Add(this.pnlDataUI);
            this.tbUID.Location = new System.Drawing.Point(4, 22);
            this.tbUID.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbUID.Name = "tbUID";
            this.tbUID.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbUID.Size = new System.Drawing.Size(790, 424);
            this.tbUID.TabIndex = 1;
            this.tbUID.Text = "Users with UID";
            this.tbUID.UseVisualStyleBackColor = true;
            // 
            // pnlDataUI
            // 
            this.pnlDataUI.AutoSize = true;
            this.pnlDataUI.Location = new System.Drawing.Point(3, 37);
            this.pnlDataUI.Name = "pnlDataUI";
            this.pnlDataUI.Size = new System.Drawing.Size(502, 346);
            this.pnlDataUI.TabIndex = 0;
            // 
            // frmAD_UID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabCtrl);
            this.Name = "frmAD_UID";
            this.Text = "UID Setter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabCtrl.ResumeLayout(false);
            this.tbNoUID.ResumeLayout(false);
            this.tbNoUID.PerformLayout();
            this.tbUID.ResumeLayout(false);
            this.tbUID.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabCtrl;
        private System.Windows.Forms.TabPage tbNoUID;
        private System.Windows.Forms.TabPage tbUID;
        private System.Windows.Forms.Panel pnlDataNoUI;
        private System.Windows.Forms.Panel pnlDataUI;
    }
}

