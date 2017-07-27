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
    public partial class frmOptions : Form
    {

        public String url = "";
        public String fileText="";
        public String convFrom = "";
        public String convTo = "";

        public frmOptions()
        {
            InitializeComponent();

   
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {

            button2.Select();

            if (url != "")
            {
                txtURL.ForeColor = Color.Black;
                txtURL.Text = url;
            }

            if (convFrom != "")
            {
                txtFrom.ForeColor = Color.Black;
                txtFrom.Text = convFrom;
            }

            if (txtTo.Text != "")
            {
                txtTo.ForeColor = Color.Black;
                txtTo.Text = convTo;
            }




            txtURL.GotFocus += (s, a) => { if (txtURL.ForeColor == Color.Gray) txtURL.Text = ""; };
            txtFrom.GotFocus += (s, a) => { if (txtFrom.ForeColor == Color.Gray) txtFrom.Text = ""; };
            txtTo.GotFocus += (s, a) => { if (txtTo.ForeColor == Color.Gray) txtTo.Text = ""; };

            txtURL.LostFocus += (s, a) =>
            {
                if (txtURL.Text == "")
                {
                    txtURL.Text = "https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts";
                    txtURL.ForeColor = Color.Gray;
                }
            };

            txtFrom.LostFocus += (s, a) =>
            {
                if (txtFrom.Text == "")
                {
                    txtFrom.Text = "0.0.0.0";
                    txtFrom.ForeColor = Color.Gray;
                }
            };

            txtTo.LostFocus += (s, a) =>
            {
                if (txtTo.Text == "")
                {
                    txtTo.Text = "0.0.0.0";
                    txtTo.ForeColor = Color.Gray;
                }
            };
        }

        private void bnEdit_Click(object sender, EventArgs e)
        {
            if (txtURL.Text != "")
                url = txtURL.Text;

            System.Net.WebClient wc = new System.Net.WebClient();
            if (fileText == "")
                fileText = wc.DownloadString(url);  // wc.DownloadFile(hostsURL, "hosts.tmp"); 

            frmEditHosts f = new frmEditHosts();
            f.Text = fileText;
            f.ShowDialog();
            fileText = f.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if ((txtFrom.ForeColor == Color.Gray && txtTo.ForeColor == Color.Black) || (txtFrom.ForeColor == Color.Black && txtTo.ForeColor == Color.Gray))
                MessageBox.Show("Please enter both \"From\" and \"To\" IP");
            else
            {
                if (txtURL.Text != "")
                    url = txtURL.Text;
                if (txtFrom.Text != "")
                    convFrom = txtFrom.Text;
                if (txtTo.Text != "")
                    convTo = txtTo.Text;
                DialogResult = DialogResult.OK;
                this.Close();                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtURL_TextChanged(object sender, EventArgs e)
        {
            if(txtURL.Text!= "https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts")
                txtURL.ForeColor = Color.Black;
        }

        private void txtTo_TextChanged(object sender, EventArgs e)
        {
            if (txtFrom.Text != "0.0.0.0")
                txtFrom.ForeColor = Color.Black;
        }

        private void txtFrom_TextChanged(object sender, EventArgs e)
        {
            if (txtTo.Text != "0.0.0.0")
                txtTo.ForeColor = Color.Black;
        }
    }
}
