using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostsManager
{
    public partial class frmAbout : Form
    {

        public frmAbout()
        {
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            lblVersion.Text = "Version: v"+Branding.VERSION;
            lblName.Text = Branding.COMPANY + " "+Branding.PRODUCT;
            pbPicture.ImageLocation = Branding.PRODUCTIMGPATH;
            try
            {
                this.Icon = new Icon(Branding.ICONPATH);
                pbPicture.ImageLocation = Branding.PRODUCTIMGPATH;
            }
            catch (Exception ex) { }            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
