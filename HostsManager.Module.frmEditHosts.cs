using System;
using System.Drawing;
using System.Windows.Forms;
using HostsManager.Settings;

namespace HostsManager
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
                this.Icon = new Icon(clsBrandingData.ICONPATH);
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
