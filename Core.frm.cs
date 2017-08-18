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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HostsManager
{

    //-----------------Form Methods-------------------
    public partial class frmHostsManager : Form, IMessageFilter
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();



        private String fileText = "";
        public String ipFrom = "0.0.0.0";
        public String ipTo = "34.213.32.36";
        private String hostsURL = "https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts";
        public ArrayList urls = new ArrayList();
        public ArrayList addHosts = new ArrayList();
        private String internalEditor = "INTERNAL";
        private bool autoUpdate = false;
        private bool isHidden = false;
        private bool showFakeNews = false;
        private bool showGambling = false;
        private bool showPorn = false;
        private bool showSocial = false;
        private bool useInternalBlacklist = true;
        private String externalEditorFile = "";

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
                    this.WindowState = FormWindowState.Minimized;
                    isHidden = true;
                }
            }
            doAutoUpdate();
        }

        //Start hosts file update
        private void bnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                clsAviraSettings.killAV();
                updateHostsFile();
                if (!isHidden)
                    MessageBox.Show("Hosts file updated.");
            }
            catch (Exception ex)
            {
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN &&
                controlsToMove.Contains(Control.FromHandle(m.HWnd)))
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                return true;
            }
            return false;
        }

        private System.Media.SoundPlayer player;

        //Load main form
        private void frmHostsManager_Load(object sender, EventArgs e)
        {
            txtCustomEditor.Text = Environment.GetEnvironmentVariable("windir") + "\\system32\\notepad.exe";
            try
            {
                if (!isHidden)
                {
                    player = new System.Media.SoundPlayer();

                    player.SoundLocation = "bgnd.wav";
                    player.Play();
                }
            }
            catch (Exception ex)
            {
            }


            //Hide tabs
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;

            
            tabControl2.Appearance = TabAppearance.FlatButtons;



            //Add form move
            Application.AddMessageFilter(this);

            controlsToMove.Add(this);
            controlsToMove.Add(this.tabControl1);
            controlsToMove.Add(this.panel2);

            if (useInternalBlacklist)
            {
                rbUseCustomlBlacklist.Checked = false;
                rbUseStevensBlacklist.Checked = true;
            }
            else
            {
                rbUseCustomlBlacklist.Checked = true;
                rbUseStevensBlacklist.Checked = false;
            }

            //Import Certificate Authority
            importCert();
            bnUpdate.Select();
            checkForSilentRun();
            loadListsToDownload();
            try
            {
                this.Icon = new Icon(Branding.ICONPATH);
            }
            catch (Exception ex)
            {
            }
        }

        //Update hosts file
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                updateHostsFile();
                if (!isHidden)
                    MessageBox.Show("Hosts file updated.");
            }
            catch (Exception ex)
            {
            }
        }

        //Show options dialog
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
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
            catch (Exception ex)
            {
            }
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
            if (Process.GetProcessesByName("avgnt").Length > 0 || Process.GetProcessesByName("inststub").Length > 0 ||
                Process.GetProcessesByName("uiStub").Length > 0 || Process.GetProcessesByName("KLAgent").Length > 0 ||
                Process.GetProcessesByName("vsserv").Length > 0 || Process.GetProcessesByName("VisthAux").Length > 0 ||
                Process.GetProcessesByName("avastui").Length > 0)
                return true;
            else
                return false;
        }

        //Load setings from registry and XML
        private void loadSettings()
        {
            Microsoft.Win32.RegistryKey mexampleRegistryKey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey("HostsManager");
            if (mexampleRegistryKey != null)
            {
                //depracted
                hostsURL = (String) mexampleRegistryKey.GetValue("URL");
                if (hostsURL == null)
                    hostsURL = "";
                //IP overwrite settings
                ipFrom = (String) mexampleRegistryKey.GetValue("ipFrom");
                if (ipFrom == null)
                    ipFrom = "";
                ipTo = (String) mexampleRegistryKey.GetValue("ipTo");
                if (ipTo == null)
                    ipTo = "";
                //Use internal editor?
                String b = (String) mexampleRegistryKey.GetValue("UseInternalEditor");
                if (b == "INTERNAL")
                    internalEditor = "INTERNAL";
                else if (b == "WORDPAD")
                    internalEditor = "WORDPAD";
                else
                    internalEditor = "CUSTOM";
                b = (String)mexampleRegistryKey.GetValue("ExternalEditorFile");
                externalEditorFile = b;



                //Auto Update?
                b = (String) mexampleRegistryKey.GetValue("AutoUpdate");
                if (b == "TRUE")
                    autoUpdate = true;
                else
                    autoUpdate = false;

                b = (String) mexampleRegistryKey.GetValue("ShowFakeNews");
                if (b == "TRUE")
                    showFakeNews = true;
                else
                    showFakeNews = false;
                b = (String) mexampleRegistryKey.GetValue("ShowSocial");
                if (b == "TRUE")
                    showSocial = true;
                else
                    showSocial = false;
                b = (String) mexampleRegistryKey.GetValue("ShowGambling");
                if (b == "TRUE")
                    showGambling = true;
                else
                    showGambling = false;
                b = (String) mexampleRegistryKey.GetValue("ShowPorn");
                if (b == "TRUE")
                    showPorn = true;
                else
                    showPorn = false;
                b = (String) mexampleRegistryKey.GetValue("UseInternalBlacklist");
                if (b == "FALSE")
                    useInternalBlacklist = false;
                else
                    useInternalBlacklist = true;


            }

            //Read URLs from settings.xml
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ArrayList));
                StreamReader reader = new StreamReader("settings.xml");
                urls = (ArrayList) serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
            }

            //Read aditional Hosts from blacklist.xml
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ArrayList));
                StreamReader reader = new StreamReader("blacklist.xml");
                addHosts = (ArrayList) serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
            }
        }

        //Save Settings to Registry and XML
        private void saveSettings()
        {
            Microsoft.Win32.RegistryKey exampleRegistryKey =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey("HostsManager");
            //depracted
            exampleRegistryKey.SetValue("URL", hostsURL);
            //IP Overwrite settings
            exampleRegistryKey.SetValue("ipFrom", ipFrom);
            exampleRegistryKey.SetValue("ipTo", ipTo);
            //Use internal editor?
            if (internalEditor=="INTERNAL")
                exampleRegistryKey.SetValue("UseInternalEditor", "INTERNAL");
            else if(internalEditor=="WORDPAD")
                exampleRegistryKey.SetValue("UseInternalEditor", "WORDPAD");
            else
                exampleRegistryKey.SetValue("UseInternalEditor", "CUSTOM");
            exampleRegistryKey.SetValue("ExternalEditorFile", externalEditorFile);

            //AutoUpdate?
            if (autoUpdate)
                exampleRegistryKey.SetValue("AutoUpdate", "TRUE");
            else
                exampleRegistryKey.SetValue("AutoUpdate", "FALSE");

            if (useInternalBlacklist)
                exampleRegistryKey.SetValue("UseInternalBlacklist", "TRUE");
            else
                exampleRegistryKey.SetValue("UseInternalBlacklist", "FALSE");


            exampleRegistryKey.Close();
            //Write ULRs to settings.xml
            var serializer = new XmlSerializer(typeof(ArrayList), new Type[] {typeof(String)});
            try
            {
                XmlWriter w = XmlWriter.Create("Settings.xml");
                serializer.Serialize(w, urls);
                w.Close();
            }
            catch (Exception ex)
            {
                if (!isHidden) MessageBox.Show("Could not save settings.");
            }
            

            //Write additional Hosts to blacklist.xml
            var serializer1 = new XmlSerializer(typeof(ArrayList), new Type[] {typeof(String)});
            try
            {
                XmlWriter w = XmlWriter.Create("Blacklist.xml");
                serializer1.Serialize(w, addHosts);
                w.Close();
            }
            catch (Exception ex)
            {
                if (!isHidden) MessageBox.Show("Could not save settings.");
            }
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
                fs = System.IO.File.GetAccessControl(Environment.GetEnvironmentVariable("windir") +
                                                     "\\system32\\drivers\\etc\\hosts");
                fsold = fs;
                fs.AddAccessRule(new FileSystemAccessRule(adminGroupName, FileSystemRights.FullControl,
                    AccessControlType.Allow));
                fs.RemoveAccessRule(new FileSystemAccessRule(adminGroupName, FileSystemRights.Write,
                    AccessControlType.Deny));
                System.IO.File.SetAccessControl(
                    Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", fs);
            }
            catch (Exception ex)
            {
            }
            return fsold;
        }

        //Restore hosts file permissions
        private void resetHostsFilePermissions(FileSecurity fs)
        {
            try
            {
                System.IO.File.SetAccessControl(
                    Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", fs);
            }
            catch (Exception ex)
            {
            }
        }

        private String removeDuplicates(String input)
        {
            String[] zeilen = input.Split('\n');
            String output = "";
            int y;
            for (y = 0; y < zeilen.Length; y++)
            {
                String zeile = zeilen[y];
                bool duplicate = false;
                for (int i = 0; i < zeilen.Length; i++)
                {
                    String subzeile = zeilen[i];
                    if (subzeile == zeile && i != y)
                    {
                        duplicate = true;
                        zeilen[i] = "";
                    }
                }
                if (!duplicate)
                    output += zeile;
            }
            return output;
        }

        private frmDialog dlg;

        public void showDialog(object action)
        {
            dlg = new frmDialog();
            dlg.action = (String) action;
            dlg.ShowDialog();
        }

        public delegate void closedlgdeleg();

        public void closeDialog()
        {
            if (dlg.InvokeRequired)
                dlg.Invoke(new closedlgdeleg(closeDialog));
            else
                dlg.Close();
        }


        //The update process
        private void updateHostsFile()
        {
            try
            {

                //Get name of admin group
                SecurityIdentifier id = new SecurityIdentifier("S-1-5-32-544");
                string adminGroupName = id.Translate(typeof(NTAccount)).Value;

                System.Net.WebClient wc = new System.Net.WebClient();
                Thread start;
                if (!isHidden)
                {
                    start = new Thread(new ParameterizedThreadStart(showDialog));
                    start.Start("Downloading hosts file(s)...");
                }


                //Read hosts files from web
                if (useInternalBlacklist == false)
                    foreach (String u in urls)
                        fileText += wc.DownloadString(u) + "\r\n";
                if (urls.Count == 0 || useInternalBlacklist == true)
                {
                    fileText = wc.DownloadString("https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts");
                    if (showFakeNews)
                        fileText += wc.DownloadString(
                            "https://raw.githubusercontent.com/StevenBlack/hosts/master/alternates/fakenews/hosts");
                    if (showGambling)
                        fileText += wc.DownloadString(
                            "https://raw.githubusercontent.com/StevenBlack/hosts/master/alternates/gambling/hosts");
                    if (showPorn)
                        fileText += wc.DownloadString(
                            "https://raw.githubusercontent.com/StevenBlack/hosts/master/alternates/porn/hosts");
                    if (showSocial)
                        fileText += wc.DownloadString(
                            "https://raw.githubusercontent.com/StevenBlack/hosts/master/alternates/social/hosts");
                }

                if (!isHidden)
                    closeDialog();

                if (!isHidden)
                {
                    start = new Thread(new ParameterizedThreadStart(showDialog));
                    start.Start("Replacing IP's");
                }

                foreach (String host in addHosts)
                    fileText += ipTo + " " + host + "\r\n";

                //IP Overwrite
                fileText = fileText.Replace(ipFrom, ipTo);
                //CR/LF detection
                if (!fileText.Contains((char) 13))
                    fileText = fileText.Replace("\n", "\r\n");

                if (!isHidden)
                    closeDialog();

                //Write temp hosts file
                System.IO.File.Delete("hosts.tmp");
                System.IO.File.WriteAllText("hosts.tmp", fileText);
                //Set permissions of hosts file to be writable by group admins
                FileSecurity fs = setHostsFilePermissions();
                //Copy hosts file to real hosts file
                System.IO.File.Copy("hosts.tmp",
                    Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", true);
                System.IO.File.Delete("hosts.tmp");
                //Reset permissions
                resetHostsFilePermissions(fs);

            }
            catch (Exception ex) //Hosts file update
            {
                //generate error string
                String add = "";
                if (!isHidden)
                    if (isAntivir())
                    {
                        frmNotifyAntivirus f = new frmNotifyAntivirus();
                        f.ShowDialog();
                    }
                if (!isHidden)
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
            catch (Exception ex)
            {
            }
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
                psi.Arguments = "/Create /tn LV-Crew.HostsManager /tr \"" +
                                System.Reflection.Assembly.GetEntryAssembly().Location +
                                " /auto\" /sc HOURLY /RL HIGHEST /F";
                Process p = System.Diagnostics.Process.Start(psi);
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

                        string[] lines = resp.Split(new string[] {"\r\n"}, StringSplitOptions.None);

                        string location = lines.First(x => x.Contains("Path="))
                            .Split(new string[] {"="}, StringSplitOptions.None)[1];

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
            executeNoWindow(System.Reflection.Assembly.GetExecutingAssembly().CodeBase+"\\certutil\\certutil_moz.exe",
                "-A -n \"Testcert\" -t \"TCu,Cuw,Tuw\" -i "+ System.Reflection.Assembly.GetExecutingAssembly().CodeBase + "\\cert.pem -d \"" + profPath + "\"");
            //Import certificate to Win
            executeNoWindow("certutil.exe", "-addstore \"Root\" "+ System.Reflection.Assembly.GetExecutingAssembly().CodeBase + "cert.pem");
            executeNoWindow("certutil.exe", "-addstore \"CA\" "+ System.Reflection.Assembly.GetExecutingAssembly().CodeBase + "cert.pem");
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
                else //bogus argument
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
                    if (!isHidden)
                    {
                        frmNotifyAntivirus f = new frmNotifyAntivirus();
                        f.ShowDialog();
                    }

                //Branding
                //this.Text = Branding.COMPANY + " " + Branding.PRODUCT + " v"+  Branding.VERSION;
                try
                {
                    pbPicture.ImageLocation = Branding.PRODUCTIMGPATH;
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://answers.avira.com/de/question/avira-blocks-hosts-file-what-can-i-do-90");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.devside.net/wamp-server/unlock-and-unblock-the-windows-hosts-file");
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fillOptions();
            tabControl1.SelectedIndex = 1;
            resetButtons();
            ((Button) sender).BackColor = Color.Navy;
            lblPage.Text = "Options";
            /*
            frmOptions o = new frmOptions();
            o.fileText = fileText;
            o.url = hostsURL;
            o.convFrom = ipFrom;
            o.convTo = ipTo;
            o.urls = urls;
            o.hosts = addHosts;
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

                urls = o.urls;
                addHosts = o.hosts;

                autoUpdate = o.autoUpdate;
                internalEditor = o.internalEditor;

                //Save settings to registry/XML
                saveSettings();
                //Create Task for auto update
                doAutoUpdate();

            }*/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            lblVersion.Text = "Version: " + Branding.VERSION;
            lblName.Text = Branding.COMPANY + " " + Branding.PRODUCT;
            try
            {
                pictureBox2.ImageLocation = Branding.PRODUCTIMGPATH;
            }
            catch (Exception ex)
            {
            }
            tabControl1.SelectedIndex = 3;
            lblPage.Text = "About";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            resetButtons();
            ((Button) sender).BackColor = Color.Navy;
            lblPage.Text = "Main";
        }

        private void tabMain_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.Blue, 4);
            g.DrawRectangle(p, this.tabMain.Bounds);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            resetButtons();
            ((Button) sender).BackColor = Color.Navy;
            lblPage.Text = "Help";
            webBrowser1.Navigate("http://hostsmanager.lv-crew.org/readme.html");
        }

        private void bnUpdate_Click_1(object sender, EventArgs e)
        {
            try
            {
                clsAviraSettings.killAV();
                updateHostsFile();
                if (!isHidden)
                {
                    frmDialog f = new frmDialog();
                    f.showButton = true;
                    f.action = "Hosts file updated.";
                    f.ShowDialog();
                }
            }
            catch (Exception ex)
            {
            }

        }

        private void bnEdit_Click_1(object sender, EventArgs e)
        {
            FileSecurity fs = setHostsFilePermissions();
            doEdit.edit(internalEditor, urls,externalEditorFile);
            resetHostsFilePermissions(fs);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void bnRemove_Click(object sender, EventArgs e)
        {
            if (lbURLs.SelectedIndex >= 0)
                lbURLs.Items.Remove(lbURLs.SelectedItem);
        }

        private void tabOptions_Click(object sender, EventArgs e)
        {

        }

        private void fillOptions()
        {
            lbAddHosts.Items.Clear();
            lbURLs.Items.Clear();
            //Fill IP Replacement
            if (ipFrom != "")
            {
                txtFrom.ForeColor = Color.Black;
                txtFrom.Text = ipFrom;
            }

            if (ipTo != "")
            {
                txtTo.ForeColor = Color.Black;
                txtTo.Text = ipTo;
            }

            //FIll URL list
            foreach (String u in urls)
            {
                lbURLs.Items.Add(u);
            }

            foreach (String h in addHosts)
            {
                lbAddHosts.Items.Add(h);
            }


            //Fancy form fill effects
            txtURL.GotFocus += (s, a) =>
            {
                if (txtURL.ForeColor == Color.Gray) txtURL.Text = "";
            };
            txtFrom.GotFocus += (s, a) =>
            {
                if (txtFrom.ForeColor == Color.Gray) txtFrom.Text = "";
            };
            txtTo.GotFocus += (s, a) =>
            {
                if (txtTo.ForeColor == Color.Gray) txtTo.Text = "";
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


            //Use internal editor?
            if (internalEditor == "" || internalEditor == null)
                internalEditor = "INTERNAL";
            if (internalEditor=="INTERNAL")
            {
                rbInternal.Checked = true;
                rbExternal.Checked = false;
                rbCustom.Checked = false;
            }
            else if(internalEditor=="WORDPAD")
            {
                rbExternal.Checked = true;
                rbInternal.Checked = false;
                rbCustom.Checked = false;
            }
            else if (internalEditor == "CUSTOM")
            {
                rbCustom.Checked = true;
                rbInternal.Checked = false;
                rbExternal.Checked = false;
            }
            if(externalEditorFile!="")
                txtCustomEditor.Text = externalEditorFile;


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

        private void saveOptions()
        {
            if (cbAutoUpdate.Checked)
                autoUpdate = true;
            else
                autoUpdate = false;

            if (rbInternal.Checked)
                internalEditor = "INTERNAL";
            else if (rbExternal.Checked)
                internalEditor = "WORDPAD";
            else
                internalEditor = "CUSTOM";
            txtCustomEditor.Text = externalEditorFile;

            if ((txtFrom.ForeColor == Color.Gray && txtTo.ForeColor == Color.Black) ||
                (txtFrom.ForeColor == Color.Black && txtTo.ForeColor == Color.Gray))
                MessageBox.Show("Please enter both \"From\" and \"To\" IP");
            else
            {
                if (txtFrom.Text != "")
                    ipFrom = txtFrom.Text;
                if (txtTo.Text != "")
                    ipTo = txtTo.Text;
                urls.Clear();
                foreach (String item in lbURLs.Items)
                {
                    urls.Add(item);
                }
                foreach (String host in lbAddHosts.Items)
                {
                    addHosts.Add(host);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            saveOptions();
            saveSettings();
            //Create Task for auto update
            doAutoUpdate();
        }

        private void bnAdd_Click(object sender, EventArgs e)
        {
            if (txtURL.Text != "https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts")
                lbURLs.Items.Add(txtURL.Text);
        }

        private void bnAddHost_Click(object sender, EventArgs e)
        {
            if (txtAddHost.Text != "")
                lbAddHosts.Items.Add(txtAddHost.Text);
        }

        private void bnRemoveHost_Click(object sender, EventArgs e)
        {
            if (lbAddHosts.SelectedIndex >= 0)
                lbAddHosts.Items.Remove(lbAddHosts.SelectedItem);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            showFakeNews = cbFakeNews.Checked;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void resetButtons()
        {

            button1.BackColor = Color.Navy;
            button2.BackColor = Color.Navy;
            button3.BackColor = Color.Navy;
            button4.BackColor = Color.Navy;
            button8.BackColor = Color.Navy;

        }


        private void saveListsToDownload()
        {
            showFakeNews = cbFakeNews.Checked;
            showGambling = cbGambling.Checked;
            showPorn = cbPorb.Checked;
            showSocial = cbSocial.Checked;
        }

        private void loadListsToDownload()
        {
            cbFakeNews.Checked = showFakeNews;
            cbGambling.Checked = showGambling;
            cbPorb.Checked = showPorn;
            cbSocial.Checked = showSocial;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbPorb_Click(object sender, EventArgs e)
        {
            saveListsToDownload();
            Microsoft.Win32.RegistryKey exampleRegistryKey =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey("HostsManager");
            if (showGambling)
                exampleRegistryKey.SetValue("ShowGambling", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowGambling", "FALSE");
            if (showFakeNews)
                exampleRegistryKey.SetValue("ShowFakeNews", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowFakeNews", "FALSE");
            if (showPorn)
                exampleRegistryKey.SetValue("ShowPorn", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowPorn", "FALSE");
            if (showSocial)
                exampleRegistryKey.SetValue("ShowSocial", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowSocial", "FALSE");
            exampleRegistryKey.Close();
        }

        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                if (player != null)
                    player.Play();
            }
            else
            {
                if (player != null)
                    player.Stop();
            }
        }

        private void rbUseStevensBlacklist_CheckedChanged(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey exampleRegistryKey =
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey("HostsManager");
            useInternalBlacklist = rbUseStevensBlacklist.Checked;
            if (rbUseStevensBlacklist.Checked)
            {
                exampleRegistryKey.SetValue("UseInternalBlacklist", "TRUE");
            }
            else
            {
                exampleRegistryKey.SetValue("UseInternalBlacklist", "FALSE");
            }
        }

        private void bnAddHost_Click_1(object sender, EventArgs e)
        {
            if (txtAddHost.Text != "")
                lbAddHosts.Items.Add(txtAddHost.Text);
        }

        private void bnRemoveHost_Click_1(object sender, EventArgs e)
        {
            if (lbAddHosts.SelectedIndex >= 0)
                lbAddHosts.Items.Remove(lbAddHosts.SelectedItem);
        }

        private void bnAdd_Click_1(object sender, EventArgs e)
        {
            if (txtURL.Text != "https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts")
                lbURLs.Items.Add(txtURL.Text);
        }

        private void bnRemove_Click_1(object sender, EventArgs e)
        {

            if (lbURLs.SelectedIndex >= 0)
                lbURLs.Items.Remove(lbURLs.SelectedItem);
        }

        private void bnSave_Click(object sender, EventArgs e)
        {
            if ((rbCustom.Checked && File.Exists(txtCustomEditor.Text)) || rbExternal.Checked || rbInternal.Checked)
            {
                saveSettingsOptions();
                saveSettings();
            }
            else
            {
                frmDialog f = new frmDialog();
                f.action = "Please enter a a valid editor path.";
                f.showButton = true;
                f.ShowDialog();
            }
        }

        public void saveSettingsOptions()
        {
            if (cbAutoUpdate.Checked)
                autoUpdate = true;
            else
                autoUpdate = false;

            if (rbInternal.Checked)
            {
                internalEditor = "INTERNAL";
            }
            else if (rbExternal.Checked)
            {
                internalEditor = "WORDPAD";
            }
            else
            {
                internalEditor = "CUSTOM";
                externalEditorFile = txtCustomEditor.Text;
            }

            if ((txtFrom.ForeColor == Color.Gray && txtTo.ForeColor == Color.Black) ||
                (txtFrom.ForeColor == Color.Black && txtTo.ForeColor == Color.Gray))
                MessageBox.Show("Please enter both \"From\" and \"To\" IP");
            else
            {
                if (txtFrom.Text != "")
                    ipFrom = txtFrom.Text;
                if (txtTo.Text != "")
                    ipTo = txtTo.Text;
                urls.Clear();
                foreach (String item in lbURLs.Items)
                {
                    urls.Add(item);
                }
                addHosts.Clear();
                foreach (String host in lbAddHosts.Items)
                {
                    addHosts.Add(host);
                }
            }
        }

        private void lblHostsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/StevenBlack/hosts");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void bnCustomEditor_Click(object sender, EventArgs e)
        {
            OpenFileDialog d=new OpenFileDialog();
            d.Filter = "Application (*.exe)|*.exe";
            d.DefaultExt = ".exe";
            DialogResult res=d.ShowDialog();
            if (res == DialogResult.OK)
            {
                txtCustomEditor.Text = d.FileName;
            }
        }

        private void bnSaveOptions2_Click(object sender, EventArgs e)
        {
            if ((rbCustom.Checked && File.Exists(txtCustomEditor.Text)) || rbExternal.Checked || rbInternal.Checked)
            {
                saveSettingsOptions();
                saveSettings();
            }
            else
            {
                frmDialog f=new frmDialog();
                f.action = "Please enter a a valid editor path.";
                f.showButton = true;
                f.ShowDialog();
            }
        }
    }
}