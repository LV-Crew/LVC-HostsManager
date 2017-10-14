using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace HostsManager
{
    public partial class frmNotifyAntivirus : Form
    {
        public frmNotifyAntivirus()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://answers.avira.com/de/question/avira-blocks-hosts-file-what-can-i-do-90");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.devside.net/wamp-server/unlock-and-unblock-the-windows-hosts-file");
        }

        private void frmNotifyAntivirus_Load(object sender, EventArgs e)
        {            
            button1.Select();
        }

    }
}
