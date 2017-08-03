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
        public string mText = "";
        public frmEditHosts()
        {
            InitializeComponent();
        }

        private void frmEditHosts_Load(object sender, EventArgs e)
        {
            textBox1.Text = mText;
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            mText = textBox1.Text;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
         
        }
    }
}
