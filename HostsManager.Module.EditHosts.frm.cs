using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;namespace HostsManager
{
    public partial class frmEditHosts : Form
    {
        public string mText = "";
        public frmEditHosts()
        {
            InitializeComponent();
        }

        private void frmEditHosts_Load(object sender, EventArgs e)
        {
            txtHostsFile.Text = mText;
            bnSave.Select();
            try
            {
                this.Icon = new Icon(Branding.ICONPATH);
            }
            catch (Exception) { }
        }        
        
        //Save        
        private void bnSave_Click(object sender, EventArgs e)
        {            
            mText = txtHostsFile.Text;
            DialogResult = DialogResult.OK;
            this.Close();
        }        //Cancel        

        private void bnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
