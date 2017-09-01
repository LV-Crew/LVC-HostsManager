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
        public int customHeight = 0;
        public int customWidth = 0;
        public bool yesNoButtons = false;

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
            if (customHeight != 0)
            {
                this.Height = customHeight;
                button1.Top = customHeight - button1.Height;
                label1.Top = 0;
                label1.Height = customHeight;
            }
            if (customWidth != 0)
            {
                this.Width = customWidth;
                button1.Left = customWidth - button1.Width;
                label1.Width = customWidth;
            }
            if (yesNoButtons)
            {
                button1.Visible = true;
                button2.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult=DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult=DialogResult.Cancel;
            this.Close();
        }
    }
}
