namespace HostsManager
{
    partial class frmOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptions));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbAutoUpdate = new System.Windows.Forms.CheckBox();
            this.rbExternal = new System.Windows.Forms.RadioButton();
            this.rbInternal = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.bnRemove = new System.Windows.Forms.Button();
            this.bnAdd = new System.Windows.Forms.Button();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.lbURLs = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFrom = new System.Windows.Forms.TextBox();
            this.bnAbbrechen = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbAutoUpdate);
            this.groupBox1.Controls.Add(this.rbExternal);
            this.groupBox1.Controls.Add(this.rbInternal);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.bnRemove);
            this.groupBox1.Controls.Add(this.bnAdd);
            this.groupBox1.Controls.Add(this.txtURL);
            this.groupBox1.Controls.Add(this.lbURLs);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtTo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtFrom);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(309, 339);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Optional Settings";
            // 
            // cbAutoUpdate
            // 
            this.cbAutoUpdate.AutoSize = true;
            this.cbAutoUpdate.Location = new System.Drawing.Point(15, 306);
            this.cbAutoUpdate.Name = "cbAutoUpdate";
            this.cbAutoUpdate.Size = new System.Drawing.Size(199, 17);
            this.cbAutoUpdate.TabIndex = 33;
            this.cbAutoUpdate.Text = "Automatically update hosts file hourly";
            this.cbAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // rbExternal
            // 
            this.rbExternal.AutoSize = true;
            this.rbExternal.Checked = true;
            this.rbExternal.Location = new System.Drawing.Point(97, 267);
            this.rbExternal.Name = "rbExternal";
            this.rbExternal.Size = new System.Drawing.Size(69, 17);
            this.rbExternal.TabIndex = 32;
            this.rbExternal.TabStop = true;
            this.rbExternal.Text = "Wordpad";
            this.rbExternal.UseVisualStyleBackColor = true;
            // 
            // rbInternal
            // 
            this.rbInternal.AutoSize = true;
            this.rbInternal.Location = new System.Drawing.Point(97, 250);
            this.rbInternal.Name = "rbInternal";
            this.rbInternal.Size = new System.Drawing.Size(60, 17);
            this.rbInternal.TabIndex = 31;
            this.rbInternal.Text = "Internal";
            this.rbInternal.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 252);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Hosts file editor:";
            // 
            // bnRemove
            // 
            this.bnRemove.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bnRemove.BackgroundImage")));
            this.bnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bnRemove.Location = new System.Drawing.Point(147, 129);
            this.bnRemove.Name = "bnRemove";
            this.bnRemove.Size = new System.Drawing.Size(26, 26);
            this.bnRemove.TabIndex = 28;
            this.bnRemove.UseVisualStyleBackColor = true;
            this.bnRemove.Click += new System.EventHandler(this.bnRemove_Click);
            // 
            // bnAdd
            // 
            this.bnAdd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bnAdd.BackgroundImage")));
            this.bnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bnAdd.Location = new System.Drawing.Point(122, 129);
            this.bnAdd.Name = "bnAdd";
            this.bnAdd.Size = new System.Drawing.Size(26, 26);
            this.bnAdd.TabIndex = 27;
            this.bnAdd.UseVisualStyleBackColor = true;
            this.bnAdd.Click += new System.EventHandler(this.bnAdd_Click);
            // 
            // txtURL
            // 
            this.txtURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtURL.ForeColor = System.Drawing.Color.Gray;
            this.txtURL.Location = new System.Drawing.Point(6, 154);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(290, 20);
            this.txtURL.TabIndex = 26;
            // 
            // lbURLs
            // 
            this.lbURLs.FormattingEnabled = true;
            this.lbURLs.Location = new System.Drawing.Point(5, 35);
            this.lbURLs.Name = "lbURLs";
            this.lbURLs.Size = new System.Drawing.Size(291, 95);
            this.lbURLs.TabIndex = 25;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(77, 211);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "With";
            // 
            // txtTo
            // 
            this.txtTo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTo.ForeColor = System.Drawing.Color.Gray;
            this.txtTo.Location = new System.Drawing.Point(138, 209);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(100, 20);
            this.txtTo.TabIndex = 18;
            this.txtTo.Text = "34.213.32.36";
            this.txtTo.TextChanged += new System.EventHandler(this.txtTo_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 192);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Overwrite IP: Replace";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Overwrite hosts file URLs";
            // 
            // txtFrom
            // 
            this.txtFrom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFrom.ForeColor = System.Drawing.Color.Gray;
            this.txtFrom.Location = new System.Drawing.Point(138, 190);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(100, 20);
            this.txtFrom.TabIndex = 21;
            this.txtFrom.Text = "0.0.0.0";
            this.txtFrom.TextChanged += new System.EventHandler(this.txtFrom_TextChanged);
            // 
            // bnAbbrechen
            // 
            this.bnAbbrechen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bnAbbrechen.Location = new System.Drawing.Point(245, 362);
            this.bnAbbrechen.Name = "bnAbbrechen";
            this.bnAbbrechen.Size = new System.Drawing.Size(75, 26);
            this.bnAbbrechen.TabIndex = 23;
            this.bnAbbrechen.Text = "Cancel";
            this.bnAbbrechen.UseVisualStyleBackColor = true;
            this.bnAbbrechen.Click += new System.EventHandler(this.bnAbbrechen_Click);
            // 
            // bnOK
            // 
            this.bnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bnOK.Location = new System.Drawing.Point(164, 362);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 26);
            this.bnOK.TabIndex = 22;
            this.bnOK.Text = "Save";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(332, 399);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnAbbrechen);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LV-Crew HostsManager Options";
            this.Load += new System.EventHandler(this.frmOptions_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.Button bnAbbrechen;
        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Button bnRemove;
        private System.Windows.Forms.Button bnAdd;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.ListBox lbURLs;
        private System.Windows.Forms.RadioButton rbExternal;
        private System.Windows.Forms.RadioButton rbInternal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbAutoUpdate;
    }
}