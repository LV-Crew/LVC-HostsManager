/*  This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see<http://www.gnu.org/licenses/>.

    Idea:   Tobias B. Besemer
    Coding: Dennis M. Heine
    
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostsManager
{
    public partial class frmMain : Form
    {
        private String fileText = "";
        private String hostsURL = "http://winhelp2002.mvps.org/hosts.txt";

        public frmMain()
        {            
            InitializeComponent();

            txtURL.GotFocus += (s, a) => { if (txtURL.ForeColor == Color.Gray) txtURL.Text = ""; };
            txtFrom.GotFocus += (s, a) => { if (txtFrom.ForeColor == Color.Gray) txtFrom.Text = ""; };
            txtTo.GotFocus += (s, a) => { if (txtTo.ForeColor == Color.Gray) txtTo.Text = ""; };

            txtURL.LostFocus += (s, a) =>
            {
                if (txtURL.Text == "")
                {
                    txtURL.Text = "http://winhelp2002.mvps.org/hosts.txt";
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

        private bool isAntivir()
        {
            if (Process.GetProcessesByName("avgnt").Length > 0 || Process.GetProcessesByName("inststub").Length > 0 || Process.GetProcessesByName("uiStub").Length > 0 || Process.GetProcessesByName("KLAgent").Length > 0 || Process.GetProcessesByName("vsserv").Length > 0 || Process.GetProcessesByName("VisthAux").Length > 0 || Process.GetProcessesByName("avastui").Length > 0)
                return true;
            else
                return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            String ipFrom = "0.0.0.0";
            String ipTo = "0.0.0.0";

            if (txtURL.Text != "")
                hostsURL = txtURL.Text;
            if (txtFrom.Text != "")
                ipFrom = txtFrom.Text;
            if (txtTo.Text != "")
                ipTo = txtTo.Text;            

            if ((txtFrom.ForeColor==Color.Gray  && txtTo.ForeColor== Color.Black) || (txtFrom.ForeColor == Color.Black && txtTo.ForeColor == Color.Gray))
                MessageBox.Show("Please enter both \"From\" and \"To\" IP");
            else
            {
                try
                {
                    SecurityIdentifier id = new SecurityIdentifier("S-1-5-32-544");
                    string adminGroupName = id.Translate(typeof(NTAccount)).Value;

                    System.Net.WebClient wc = new System.Net.WebClient();
                    if(fileText=="")
                        fileText = wc.DownloadString(hostsURL);  // wc.DownloadFile(hostsURL, "hosts.tmp");                    
                    fileText = fileText.Replace(ipTo, ipFrom);
                    System.IO.File.Delete("hosts.tmp");
                    System.IO.File.WriteAllText("hosts.tmp", fileText);

                    FileSecurity fs = System.IO.File.GetAccessControl(Environment.GetEnvironmentVariable("windir")+"\\system32\\drivers\\etc\\hosts");
                    fs.AddAccessRule(new FileSystemAccessRule(adminGroupName, FileSystemRights.FullControl, AccessControlType.Allow));
                    fs.RemoveAccessRule(new FileSystemAccessRule(adminGroupName, FileSystemRights.Write, AccessControlType.Deny));
                    System.IO.File.SetAccessControl(Environment.GetEnvironmentVariable("windir") +"\\system32\\drivers\\etc\\hosts", fs);

                    System.IO.File.Copy("hosts.tmp", Environment.GetEnvironmentVariable("windir") +"\\system32\\drivers\\etc\\hosts", true);
                    System.IO.File.Delete("hosts.tmp");

                    fs.RemoveAccessRule(new FileSystemAccessRule(adminGroupName, FileSystemRights.FullControl, AccessControlType.Allow));
                    fs.AddAccessRule(new FileSystemAccessRule(adminGroupName, FileSystemRights.Write, AccessControlType.Deny));

                    MessageBox.Show("Hosts file updated.");

                }catch(Exception ex) {
                    String add = "";
                    if (isAntivir())
                        add = "Antivirus found. Prease disable it during hosts file update.\nRead the manual for further informations.\n";
                    MessageBox.Show("Error: " + add+ ex.Message);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            txtURL.ForeColor = Color.Black;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            txtTo.ForeColor = Color.Black;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            txtFrom.ForeColor = Color.Black;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bnUpdate.Select();            
            if (isAntivir())  
                MessageBox.Show("Antivirus found. Prease disable it during hosts file update.\nRead the manual for further informations.\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            if (fileText == "")
                fileText = wc.DownloadString(hostsURL);  // wc.DownloadFile(hostsURL, "hosts.tmp"); 

            frmEditHosts f = new frmEditHosts();
            f.Text = fileText;
            f.ShowDialog();
            fileText = f.Text;
        }
    }
}
