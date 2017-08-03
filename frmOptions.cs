using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskScheduler;

namespace HostsManager
{
    public partial class frmOptions : Form
    {
        public ArrayList urls = new ArrayList();
        public String url = "";
        public String fileText="";
        public String convFrom = "";
        public String convTo = "";
        public bool internalEditor = false;
        public bool autoUpdate = false;

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

            foreach(String u in urls)
            {
                listBox1.Items.Add(u);
            }



            txtURL.GotFocus += (s, a) => { if (txtURL.ForeColor == Color.Gray) txtURL.Text = ""; };
            txtFrom.GotFocus += (s, a) => { if (txtFrom.ForeColor == Color.Gray) txtFrom.Text = ""; };
            txtTo.GotFocus += (s, a) => { if (txtTo.ForeColor == Color.Gray) txtTo.Text = ""; };

            if (internalEditor)
            {
                rbInternal.Checked = true;
                rbExternal.Checked = false;
            }
            else
            {
                rbExternal.Checked = true;
                rbInternal.Checked = false;
            }

            if(autoUpdate)
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }

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
                    txtTo.Text = "34.213.32.36";
                    txtTo.ForeColor = Color.Gray;
                }
            };

            this.Text = Branding.COMPANY + " "+Branding.PRODUCT+" Options";
        }

        private void bnEdit_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                autoUpdate = true;
            else
                autoUpdate = false;

            if (rbInternal.Checked)
                internalEditor = true;
            else
                internalEditor = false;

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
                urls.Clear();
                foreach(String item in listBox1.Items)
                {
                    urls.Add(item);
                }
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
            if (txtTo.Text != "34.213.32.36")
                txtTo.ForeColor = Color.Black;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(txtURL.Text!="https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts")
                listBox1.Items.Add(txtURL.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
                listBox1.Items.Remove(listBox1.SelectedItem);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void bnEditHosts_Click(object sender, EventArgs e)
        {
            if (txtURL.Text != "")
                url = txtURL.Text;
            try
            {
             
                System.Net.WebClient wc = new System.Net.WebClient();
                if (fileText == "")
                    foreach (String u in urls)
                        fileText += wc.DownloadString(u) + "\r\n";
                if (urls.Count == 0)
                    fileText = wc.DownloadString("https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts");
                fileText.Replace(txtTo.Text, txtFrom.Text);
                frmEditHosts f = new frmEditHosts();
                f.mText = fileText;
                f.ShowDialog();
                fileText = f.mText;
             
                System.Diagnostics.Process.Start("wordpad.exe", Environment.GetEnvironmentVariable("windir")+"\\system32\\drivers\\etc\\hosts");
            }
            catch (Exception ex) { }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void setHostsFilePermissions()
        {
            try
            {
                SecurityIdentifier id = new SecurityIdentifier("S-1-5-32-544");
                string adminGroupName = id.Translate(typeof(NTAccount)).Value;
                FileSecurity fs = System.IO.File.GetAccessControl(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
                fs.AddAccessRule(new FileSystemAccessRule(adminGroupName, FileSystemRights.FullControl, AccessControlType.Allow));
                fs.RemoveAccessRule(new FileSystemAccessRule(adminGroupName, FileSystemRights.Write, AccessControlType.Deny));
                System.IO.File.SetAccessControl(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", fs);
            }
            catch (Exception ex) { }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            setHostsFilePermissions();
            System.Diagnostics.Process.Start("schtasks.exe", "/Delete /tn LV-Crew.HostsManager /F");                     
            System.Diagnostics.Process.Start("schtasks.exe", "/Create /tn LV-Crew.HostsManager /tr \"" + System.Reflection.Assembly.GetEntryAssembly().Location  + "\" /sc DAILY");
            MessageBox.Show("The auto-update has been added to the windows task scheduler.");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           
        }
    }
}
