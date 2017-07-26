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
    public partial class frmEditHosts : Form
    {
        public string Text = "";
        public frmEditHosts()
        {
            InitializeComponent();
        }

        private void frmEditHosts_Load(object sender, EventArgs e)
        {
            textBox1.Text = Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            Text = textBox1.Text;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
