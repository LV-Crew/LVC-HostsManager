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

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HostsManager
{

    //-----------------Form Methods-------------------
    public partial class frmHostsManager : Form
    {
        private String fileText = "";
        public String ipFrom = "0.0.0.0";
        public String ipTo = "34.213.32.36";
        private String hostsURL = "https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts";
        public ArrayList urls = new ArrayList();
        private bool internalEditor = false;
        private bool autoUpdate = false;
        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern Microsoft.Win32.SafeHandles.SafeFileHandle CreateFile(string filename, [MarshalAs(UnmanagedType.U4)]FileAccess fileaccess, [MarshalAs(UnmanagedType.U4)]FileShare fileshare, int securityattributes, [MarshalAs(UnmanagedType.U4)]FileMode creationdisposition, int flags, IntPtr template);
        private Microsoft.Win32.SafeHandles.SafeFileHandle safeFileHandle;
        public frmHostsManager()
        {            
            InitializeComponent();






            clsBrandingINI.readINI();
            loadSettings();

            //Check whether to run silently
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

        //Start hosts file update
        private void bnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                updateHostsFile();
                MessageBox.Show("Hosts file updated.");
            }
            catch (Exception ex) { }
        }

        //Load main form
        private void frmHostsManager_Load(object sender, EventArgs e)
        {
            //Import Certificate Authority
            importCert();
            bnUpdate.Select();
            checkForSilentRun();
            try
            {
                this.Icon = new Icon(Branding.ICONPATH);
            }
            catch (Exception ex) { }
        }

        //Update hosts file
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                updateHostsFile();
                MessageBox.Show("Hosts file updated.");
            }
            catch (Exception ex) { }
        }

        //Show options dialog
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
            if (o.DialogResult == DialogResult.OK)
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

                //Save settings to registry/XML
                saveSettings();
                //Create Task for auto update
                doAutoUpdate();

            }
        }

        //Exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //About dialog
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog();
        }

        //Edit hosts file dialog
        private void bnEdit_Click(object sender, EventArgs e)
        {
            FileSecurity fs = setHostsFilePermissions();
            doEdit.edit(internalEditor, urls);
            resetHostsFilePermissions(fs);
        }

        //Show help file
        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://hostsmanager.lv-crew.org/readme.html");
            }
            catch (Exception ex) { }
        }

        //Edit hosts file
        private void editHostsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Set file security of hosts file to be editable by admins
            FileSecurity fs = setHostsFilePermissions();
            //Show editor
            doEdit.edit(internalEditor, urls);
            //Reset file security of hosts file
            resetHostsFilePermissions(fs);
        }

        //Show homepage
        private void pbPicture_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://hostsmanager.lv-crew.org");
        }


        //-----------------Generic Methods-------------------
        
        //Check for antivir
        private bool isAntivir()
        {
            //Check porcesses for process names of antivirs
            if (Process.GetProcessesByName("avgnt").Length > 0 || Process.GetProcessesByName("inststub").Length > 0 || Process.GetProcessesByName("uiStub").Length > 0 || Process.GetProcessesByName("KLAgent").Length > 0 || Process.GetProcessesByName("vsserv").Length > 0 || Process.GetProcessesByName("VisthAux").Length > 0 || Process.GetProcessesByName("avastui").Length > 0)
                return true;
            else
                return false;
        }

        //Load setings from registry and XML
        private void loadSettings()
        {
            Microsoft.Win32.RegistryKey mexampleRegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("HostsManager");
            if (mexampleRegistryKey != null)
            {
                //depracted
                hostsURL = (String)mexampleRegistryKey.GetValue("URL");
                if (hostsURL == null)
                    hostsURL = "";
                //IP overwrite settings
                ipFrom = (String)mexampleRegistryKey.GetValue("ipFrom");
                if (ipFrom == null)
                    ipFrom = "";
                ipTo = (String)mexampleRegistryKey.GetValue("ipTo");
                if (ipTo == null)
                    ipTo = "";
                //Use internal editor?
                String b = (String)mexampleRegistryKey.GetValue("UseInternalEditor");
                if (b == "TRUE")
                    internalEditor = true;
                else
                    internalEditor = false;
                //Auto Update?
                b = (String)mexampleRegistryKey.GetValue("AutoUpdate");
                if (b == "TRUE")
                    autoUpdate = true;
                else
                    autoUpdate = false;                
            }

            //Read URLs from settings.txt
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ArrayList));
                StreamReader reader = new StreamReader("settings.xml");
                urls = (ArrayList)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception ex) { }
        }

        //Save Settings to Registry and XML
        private void saveSettings()
        {
            Microsoft.Win32.RegistryKey exampleRegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("HostsManager");
            //depracted
            exampleRegistryKey.SetValue("URL", hostsURL);
            //IP Overwrite settings
            exampleRegistryKey.SetValue("ipFrom", ipFrom);
            exampleRegistryKey.SetValue("ipTo", ipTo);
            //Use internal editor?
            if (internalEditor)
                exampleRegistryKey.SetValue("UseInternalEditor", "TRUE");
            else
                exampleRegistryKey.SetValue("UseInternalEditor", "FALSE");
            //AutoUpdate?
            if (autoUpdate)
                exampleRegistryKey.SetValue("AutoUpdate", "TRUE");
            else
                exampleRegistryKey.SetValue("AutoUpdate", "FALSE");


            exampleRegistryKey.Close();
            //Write ULRs to settings.xml
            var serializer = new XmlSerializer(typeof(ArrayList), new Type[] { typeof(String) });
            try
            {
                serializer.Serialize(XmlWriter.Create("settings.xml"), urls);
            }catch(Exception ex) { MessageBox.Show("Could not save settings."); }

        }

        //Set Permissions of hosts file to be writable by group admins
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

        //Restore hosts file permissions
        private void resetHostsFilePermissions(FileSecurity fs)
        {
            try
            {
                System.IO.File.SetAccessControl(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", fs);
            }
            catch (Exception ex) { }
        }

        //The update process
        private void updateHostsFile()
        {
            try
            {
                toolStripProgressBar1.Visible = true;
                //Get name of admin group
                SecurityIdentifier id = new SecurityIdentifier("S-1-5-32-544");
                string adminGroupName = id.Translate(typeof(NTAccount)).Value;

                System.Net.WebClient wc = new System.Net.WebClient();

                //Read hosts files from web
                if (fileText == "")
                    foreach(String u in urls)
                        fileText+=wc.DownloadString(u)+ "\r\n";
                if (urls.Count == 0)
                    fileText = wc.DownloadString("https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts");

                //IP Overwrite
                fileText = fileText.Replace(ipFrom, ipTo);
                //CR/LF detection
                if (!fileText.Contains((char)13))
                    fileText=fileText.Replace("\n", "\r\n");
                //Write temp hosts file
                System.IO.File.Delete("hosts.tmp");
                System.IO.File.WriteAllText("hosts.tmp", fileText);
                //Set permissions of hosts file to be writable by group admins
                FileSecurity fs=setHostsFilePermissions();


                using (FileStream fileStream = new FileStream(
         "c:\\windows\\system32\\drivers\\etc\\hosts", FileMode.OpenOrCreate,
         FileAccess.Read, FileShare.None))
                {
                    SafeFileHandle safeFileHandle = CreateFile("c:\\windows\\system32\\drivers\\etc\\hosts", FileAccess.Read, FileShare.Read, 0, FileMode.Open, 0, IntPtr.Zero);
                    safeFileHandle.DangerousRelease();


                    //                FileInfo fi = new FileInfo("c:\\windows\\system32\\drivers\\etc\\hosts");
                    //              fileStream.
                    //            fileStream.Unlock(0, fi.Length);
                    File.Delete("c:\\windows\\system32\\drivers\\etc\\hosts");
                }



                /*
                //Copy hosts file to real hosts file
                System.IO.File.Copy("hosts.tmp", Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", true);
                System.IO.File.Delete("hosts.tmp");
                //Reset permissions*/
                resetHostsFilePermissions(fs);

                toolStripProgressBar1.Visible = false;                
            }
            catch (Exception ex)//Hosts file update
            {
                //generate error string
                String add = "";
                if (isAntivir())
                    add = "Antivirus found!\n Please turn off hosts protection during hosts file update.\nRead the manual for further information.\n";
                MessageBox.Show("Error: " + add + ex.Message);
            }
        }

        //Start external process with hidden window      
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

      
        //Create Task for auto update
        public void doAutoUpdate()
        {
            //prepare ProcessStartInfo
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.FileName = "schtasks.exe";            


            if (autoUpdate)
            {
                //Create task using schtasks.exe
                //FileSecurity fs=setHostsFilePermissions(); !!! 080417DH - muss evtl wieder reingemacht werden.
                psi.Arguments = "/Create /tn LV-Crew.HostsManager /tr \"" + System.Reflection.Assembly.GetEntryAssembly().Location + " /auto\" /sc HOURLY /RL HIGHEST /F";
                Process p=System.Diagnostics.Process.Start(psi);
            }
            else
            {
                //Delete task using schtasks.exe
                System.Diagnostics.Process.Start("schtasks.exe", "/Delete /tn LV-Crew.HostsManager /F");
            }
        }

        //Search for FireFox Profile. 
        //080417DH - give credits
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

        private void importCert()
        {
            //Get Firefox profile path
            String profPath = ReadFirefoxProfile();

            //Import certificate to FF
            executeNoWindow("certutil\\certutil_moz.exe", "-A -n \"Testcert\" -t \"TCu,Cuw,Tuw\" -i cert.pem -d \"" + profPath + "\"");
            //Import certificate to Win
            executeNoWindow("certutil.exe", "-addstore \"Root\" cert.pem");
            executeNoWindow("certutil.exe", "-addstore \"CA\" cert.pem");
        }



        private void checkForSilentRun()
        {
            //Check whether to run silently
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                if (arguments[1] == "/auto")
                {
                    //Set hosts file permissions to writable for group admins
                    FileSecurity fs = setHostsFilePermissions();
                    //Update hostsfile
                    updateHostsFile();
                    //Reset file permissions
                    resetHostsFilePermissions(fs);
                    CancelEventArgs a = new CancelEventArgs(false);
                    Application.Exit(a);
                }
                else//bogus argument
                {
                    //Exit
                    CancelEventArgs a = new CancelEventArgs(true);
                    Application.Exit(a);
                }
            }
            else
            {
                //Check for Antivir
                if (isAntivir())
                    MessageBox.Show("Antivirus found!\nPlease turn off hosts protection during hosts file update.\nRead the manual for further information.\n");

                //Branding
                this.Text = Branding.COMPANY + " " + Branding.PRODUCT + " v" + Branding.VERSION;
                try
                {
                    pbPicture.ImageLocation = Branding.PRODUCTIMGPATH;
                }
                catch (Exception ex) { }
            }
        }
    }
}
