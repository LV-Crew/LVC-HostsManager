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
            lblVersion.Text = "Version: "+Branding.VERSION;
            lblName.Text = Branding.COMPANY + " "+Branding.PRODUCT;            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblName_Click(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = Branding.PRODUCTIMGPATH;
        }

        private void lblVersion_Click(object sender, EventArgs e)
        {

        }
    }
}
