namespace HostsManager
{
    partial class frmEditHosts
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>        private System.ComponentModel.IContainer components = null;
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
        /// </summary>        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditHosts));
            this.txtHostsFile = new System.Windows.Forms.TextBox();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtHostsFile
            // 
            this.txtHostsFile.BackColor = System.Drawing.Color.Black;
            this.txtHostsFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHostsFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.txtHostsFile.Location = new System.Drawing.Point(12, 12);
            this.txtHostsFile.MaxLength = 999999999;
            this.txtHostsFile.Multiline = true;
            this.txtHostsFile.Name = "txtHostsFile";
            this.txtHostsFile.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtHostsFile.Size = new System.Drawing.Size(560, 629);
            this.txtHostsFile.TabIndex = 0;
            // 
            // bnCancel
            // 
            this.bnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.bnCancel.Location = new System.Drawing.Point(497, 647);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 1;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // bnSave
            // 
            this.bnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.bnSave.Location = new System.Drawing.Point(416, 647);
            this.bnSave.Name = "bnSave";
            this.bnSave.Size = new System.Drawing.Size(75, 23);
            this.bnSave.TabIndex = 2;
            this.bnSave.Text = "Save";
            this.bnSave.UseVisualStyleBackColor = true;
            this.bnSave.Click += new System.EventHandler(this.bnSave_Click);
            // 
            // frmEditHosts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(584, 683);
            this.ControlBox = false;
            this.Controls.Add(this.bnSave);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.txtHostsFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEditHosts";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmEditHosts_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion        private System.Windows.Forms.TextBox txtHostsFile;        private System.Windows.Forms.Button bnCancel;        private System.Windows.Forms.Button bnSave;
    }
}