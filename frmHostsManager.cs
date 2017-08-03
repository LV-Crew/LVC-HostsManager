﻿/*  This program is free software: you can redistribute it and/or modify
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HostsManager
{
    public partial class frmHostsManager : Form
    {
        private String fileText = "";
        public String ipFrom = "0.0.0.0";
        public String ipTo = "34.213.32.36";
        private String hostsURL = "https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts";
        public ArrayList urls = new ArrayList();
        private bool internalEditor = false;
        private bool autoUpdate = false;

        public frmHostsManager()
        {            
            InitializeComponent();
            loadSettings();

            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                if (arguments[1] == "/auto")
                {
                    this.Opacity = 0;
                    this.ShowInTaskbar = false;
                }
            }

            doAutoUpdate();
        }

        private bool isAntivir()
        {
            if (Process.GetProcessesByName("avgnt").Length > 0 || Process.GetProcessesByName("inststub").Length > 0 || Process.GetProcessesByName("uiStub").Length > 0 || Process.GetProcessesByName("KLAgent").Length > 0 || Process.GetProcessesByName("vsserv").Length > 0 || Process.GetProcessesByName("VisthAux").Length > 0 || Process.GetProcessesByName("avastui").Length > 0)
                return true;
            else
                return false;
        }

        private void loadSettings()
        {
            Microsoft.Win32.RegistryKey mexampleRegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("HostsManager");
            if (mexampleRegistryKey != null)
            {
                hostsURL = (String)mexampleRegistryKey.GetValue("URL");
                if (hostsURL == null)
                    hostsURL = "";
                ipFrom = (String)mexampleRegistryKey.GetValue("ipFrom");
                if (ipFrom == null)
                    ipFrom = "";
                ipTo = (String)mexampleRegistryKey.GetValue("ipTo");
                if (ipTo == null)
                    ipTo = "";

                String b = (String)mexampleRegistryKey.GetValue("UseInternalEditor");
                if (b == "TRUE")
                    internalEditor = true;
                else
                    internalEditor = false;

                b = (String)mexampleRegistryKey.GetValue("AutoUpdate");
                if (b == "TRUE")
                    autoUpdate = true;
                else
                    autoUpdate = false;                
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ArrayList));
                StreamReader reader = new StreamReader("settings.xml");
                urls = (ArrayList)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception ex) { }
        }

        private void saveSettings()
        {
            Microsoft.Win32.RegistryKey exampleRegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("HostsManager");
            exampleRegistryKey.SetValue("URL", hostsURL);
            exampleRegistryKey.SetValue("ipFrom", ipFrom);
            exampleRegistryKey.SetValue("ipTo", ipTo);
            if (internalEditor)
                exampleRegistryKey.SetValue("UseInternalEditor", "TRUE");
            else
                exampleRegistryKey.SetValue("UseInternalEditor", "FALSE");

            if (autoUpdate)
                exampleRegistryKey.SetValue("AutoUpdate", "TRUE");
            else
                exampleRegistryKey.SetValue("AutoUpdate", "FALSE");


            exampleRegistryKey.Close();

            var serializer = new XmlSerializer(typeof(ArrayList), new Type[] { typeof(String) });
            try
            {
                serializer.Serialize(XmlWriter.Create("settings.xml"), urls);
            }catch(Exception ex) { MessageBox.Show("Could not save settings."); }

        }

        private FileSecurity setHostsFilePermissions()
        {
            FileSecurity fs = null;
            FileSecurity fsold = null;
            try
            {
                SecurityIdentifier id = new SecurityIdentifier("S-1-5-32-544");
                string adminGroupName = id.Translate(typeof(NTAccount)).Value;
                fs = System.IO.File.GetAccessControl(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
                fsold = fs;
                fs.AddAccessRule(new FileSystemAccessRule(adminGroupName, FileSystemRights.FullControl, AccessControlType.Allow));
                fs.RemoveAccessRule(new FileSystemAccessRule(adminGroupName, FileSystemRights.Write, AccessControlType.Deny));
                System.IO.File.SetAccessControl(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", fs);
            }
            catch (Exception ex) { }
            return fsold;
        }

        private void resetHostsFilePermissions(FileSecurity fs)
        {
            try
            {
                System.IO.File.SetAccessControl(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", fs);
            }
            catch (Exception ex) { }
        }


        private void updateHostsFile()
        {
            try
            {
                toolStripProgressBar1.Visible = true;
                SecurityIdentifier id = new SecurityIdentifier("S-1-5-32-544");
                string adminGroupName = id.Translate(typeof(NTAccount)).Value;

                System.Net.WebClient wc = new System.Net.WebClient();

                if (fileText == "")
                    foreach(String u in urls)
                        fileText+=wc.DownloadString(u)+ "\r\n";
                if (urls.Count == 0)
                    fileText = wc.DownloadString("https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts");

                fileText = fileText.Replace(ipFrom, ipTo);
                if (!fileText.Contains((char)13))
                    fileText=fileText.Replace("\n", "\r\n");
                
                System.IO.File.Delete("hosts.tmp");
                System.IO.File.WriteAllText("hosts.tmp", fileText);

                FileSecurity fs=setHostsFilePermissions();
                System.IO.File.Copy("hosts.tmp", Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", true);
                System.IO.File.Delete("hosts.tmp");
                resetHostsFilePermissions(fs);

                toolStripProgressBar1.Visible = false;                
            }
            catch (Exception ex)
            {
                String add = "";
                if (isAntivir())
                    add = "Antivirus found!\n Please turn off hosts protection during hosts file update.\nRead the manual for further information.\n";
                MessageBox.Show("Error: " + add + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                updateHostsFile();
                MessageBox.Show("Hosts file updated.");
            }
            catch (Exception ex) { }
        }

        private void executeNoWindow(String cmd, String param)
        {
            try
            {
                ProcessStartInfo pi = new ProcessStartInfo(cmd, param);
                pi.CreateNoWindow = true;
                pi.UseShellExecute = false;
                pi.RedirectStandardOutput = true;
                pi.RedirectStandardError = true;
                Process.Start(pi);
            }
            catch (Exception ex) { }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String profPath=ReadFirefoxProfile();

            executeNoWindow("certutil\\certutil_moz.exe", "-A -n \"Testcert\" -t \"TCu,Cuw,Tuw\" -i cert.pem -d \"" + profPath + "\"");
            executeNoWindow("certutil.exe", "-addstore \"Root\" cert.pem");
            executeNoWindow("certutil.exe", "-addstore \"CA\" cert.pem");
            
            bnUpdate.Select();

            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                if (arguments[1] == "/auto")
                {
                    FileSecurity fs=setHostsFilePermissions();
                    updateHostsFile();
                    resetHostsFilePermissions(fs);
                    CancelEventArgs a = new CancelEventArgs(false);
                    Application.Exit(a);
                }
                else
                {
                    CancelEventArgs a = new CancelEventArgs(true);
                    Application.Exit(a);
                }
            }
            else
            {
                if (isAntivir())
                    MessageBox.Show("Antivirus found!\nPlease turn off hosts protection during hosts file update.\nRead the manual for further information.\n");

                this.Text = Branding.COMPANY + " " + Branding.PRODUCT+" v"+Branding.VERSION;
                pictureBox1.ImageLocation = Branding.PRODUCTIMGPATH;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
         
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                updateHostsFile();
                MessageBox.Show("Hosts file updated.");
            }
            catch (Exception ex) { }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOptions o = new frmOptions();
            o.fileText = fileText;
            o.url = hostsURL;            
            o.convFrom = ipFrom;
            o.convTo = ipTo;
            o.urls = urls;
            o.internalEditor = internalEditor;
            o.autoUpdate = autoUpdate;
            o.ShowDialog();
            if(o.DialogResult==DialogResult.OK)
            {
                if (o.fileText != "")
                    fileText = o.fileText;
                if (o.convTo != "")
                    ipTo = o.convTo;
                if (o.convFrom != "")
                    ipFrom = o.convFrom;
                if (o.url != "")
                    hostsURL = o.url;
                if (o.urls.Count > 0)
                    urls = o.urls;
                autoUpdate = o.autoUpdate;
                internalEditor = o.internalEditor;

                saveSettings();
                doAutoUpdate();
      
            }
        }

        public void doAutoUpdate()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.FileName = "schtasks.exe";            


            if (autoUpdate)
            {
                setHostsFilePermissions();
                //psi.Arguments = "/Delete /tn LV-Crew.HostsManager /F";
                //System.Diagnostics.Process.Start(psi);
                psi.Arguments = "/Create /tn LV-Crew.HostsManager /tr \"" + System.Reflection.Assembly.GetEntryAssembly().Location + " /auto\" /sc HOURLY /RL HIGHEST";
                System.Diagnostics.Process.Start(psi);
            }
            else
            {
                System.Diagnostics.Process.Start("schtasks.exe", "/Delete /tn LV-Crew.HostsManager /F");
            }
        }
        
        public string ReadFirefoxProfile()
        {
            string apppath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string mozilla = System.IO.Path.Combine(apppath, "Mozilla");

            bool exist = System.IO.Directory.Exists(mozilla);

            if (exist)
            {

                string firefox = System.IO.Path.Combine(mozilla, "firefox");

                if (System.IO.Directory.Exists(firefox))
                {
                    string prof_file = System.IO.Path.Combine(firefox, "profiles.ini");

                    bool file_exist = System.IO.File.Exists(prof_file);

                    if (file_exist)
                    {
                        StreamReader rdr = new StreamReader(prof_file);

                        string resp = rdr.ReadToEnd();

                        string[] lines = resp.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                        string location = lines.First(x => x.Contains("Path=")).Split(new string[] { "=" }, StringSplitOptions.None)[1];

                        string prof_dir = System.IO.Path.Combine(firefox, location);

                        return prof_dir;
                    }
                }
            }
            return "";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog();
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FileSecurity fs=setHostsFilePermissions();
            doEdit.edit(internalEditor, urls);
            resetHostsFilePermissions(fs);
        }

        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("readme.txt");
            }
            catch (Exception ex) { }
        }

        private void editHostsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSecurity fs=setHostsFilePermissions();
            doEdit.edit(internalEditor, urls);
            resetHostsFilePermissions(fs);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://hostsmanager.lv-crew.org");
        }
    }
}
