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
    public partial class frmDialog : Form
    {
        public String action = "";
        public bool showButton = false;

        public frmDialog()
        {
            InitializeComponent();            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void frmDialog_Load(object sender, EventArgs e)
        {
            label1.Text = action;
            button1.Visible = showButton;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
