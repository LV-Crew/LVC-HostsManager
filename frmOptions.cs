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
        //----------------------------Form events---------------------------------------
        private void frmOptions_Load(object sender, EventArgs e)
        {
            bnOK.Select();
            fillForm();
            //Branding
            this.Text = Branding.COMPANY + " "+Branding.PRODUCT+" Options";
            try
            {
                this.Icon = new Icon(Branding.ICONPATH);
            }
            catch (Exception ex) { }
        }

        //OK
        private void bnOK_Click(object sender, EventArgs e)
        {
            saveSettings();
        }

        //Cancel
        private void bnAbbrechen_Click(object sender, EventArgs e)
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

        private void bnAdd_Click(object sender, EventArgs e)
        {
            if(txtURL.Text!="https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts")
                lbURLs.Items.Add(txtURL.Text);
        }

        private void bnRemove_Click(object sender, EventArgs e)
        {
            if (lbURLs.SelectedIndex >= 0)
                lbURLs.Items.Remove(lbURLs.SelectedItem);
        }


//----------------------------Generic functions---------------------------------------      

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

        private void fillForm()
        {

            //Fill IP Replacement
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

            //FIll URL list
            foreach (String u in urls)
            {
                lbURLs.Items.Add(u);
            }


            //Fancy form fill effects
            txtURL.GotFocus += (s, a) => { if (txtURL.ForeColor == Color.Gray) txtURL.Text = ""; };
            txtFrom.GotFocus += (s, a) => { if (txtFrom.ForeColor == Color.Gray) txtFrom.Text = ""; };
            txtTo.GotFocus += (s, a) => { if (txtTo.ForeColor == Color.Gray) txtTo.Text = ""; };

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


            //Use internal editor?
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

            //Auto Update?
            if (autoUpdate)
            {
                cbAutoUpdate.Checked = true;
            }
            else
            {
                cbAutoUpdate.Checked = false;
            }
        }

        public void saveSettings()
        {
            if (cbAutoUpdate.Checked)
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
                foreach (String item in lbURLs.Items)
                {
                    urls.Add(item);
                }
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
