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
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace HostsManager
{

    //-----------------Form Methods-------------------
    public partial class frmHostsManager : Form, IMessageFilter
    {
        public const String WHITEPAGE_IP = "1.2.3.4";
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        public enum BlacklistTypes
        {
            INTERNAL=1,
            STEVENBLACK=2,
            HOSTSFILEDOTNET=3
        }

        public enum mIPReplaceMethod
        {
            KEEP_LOCALHOST=0,
            SET_WHITEPAGE=1,
            SET_CUSTOM=2
        }

        private mIPReplaceMethod replaceMethod=mIPReplaceMethod.SET_WHITEPAGE;

        private String fileText = "";
        public String ipFrom = "0.0.0.0";
        public String ipTo = "34.213.32.36";
        private String hostsURL = "https://hosts-file.net/download/hosts.txt";
        public ArrayList urls = new ArrayList();
        public ArrayList addHosts = new ArrayList();
        private String internalEditor = "INTERNAL";
        private bool autoUpdate = false;
        private bool isHidden = false;
        private bool showFakeNews = false;
        private bool showGambling = false;
        private bool showPorn = false;
        private bool showSocial = false;
        private BlacklistTypes blacklistToUse = BlacklistTypes.INTERNAL;
        private String externalEditorFile = "";
        private bool DNServiceDisabled = false;
        private bool DNSGoogleChanged = false;
        private bool DNSOpenDNSChanged = false;
        private String oldDNS = "192.168.2.1";
        private String replaceIP = "0.0.0.0";

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

            if (blacklistToUse == BlacklistTypes.STEVENBLACK)
            {
                rbUseCustomlBlacklist.Checked = false;
                rbUseHostsFileBL.Checked = false;
                rbUseStevensBlacklist.Checked = true;
            }
            else if (blacklistToUse == BlacklistTypes.INTERNAL)
            {
                rbUseCustomlBlacklist.Checked = true;
                rbUseHostsFileBL.Checked = false;
                rbUseStevensBlacklist.Checked = false;
            }
            else
            {
                rbUseCustomlBlacklist.Checked = false;
                rbUseHostsFileBL.Checked = true;
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
     
                b = (String) mexampleRegistryKey.GetValue("DNSServiceDisabled");
                if (b == null) b = "FALSE";
                DNServiceDisabled = b.Equals("TRUE") ? true : false;
                if (DNServiceDisabled)
                    bnDisableDNS.Text = "Disable DNS Service";

                b = (String)mexampleRegistryKey.GetValue("DNSGoogleChanged");
                if (b == null) b = "FALSE";
                DNSGoogleChanged = b.Equals("TRUE") ? true : false;
                if (DNSGoogleChanged)
                    bnSetDNSServerGoogle.Text = "Reset DNS Server";

                b = (String)mexampleRegistryKey.GetValue("DNSOpenDNSChanged");
                if (b == null) b = "FALSE";
                DNSOpenDNSChanged = b.Equals("TRUE") ? true : false;
                if (DNSOpenDNSChanged)
                    bnSetDNSOpenDNS.Text = "Reset DNS Server";

                b = (String)mexampleRegistryKey.GetValue("redirectType");
                if (b == null)
                {
                    b = "SET_WHITEPAGE";
                    replaceMethod=mIPReplaceMethod.SET_WHITEPAGE;
                }
                else if (b == "KEEP_LOCALHOST")
                {
                    replaceMethod = mIPReplaceMethod.KEEP_LOCALHOST;
                }
                else if (b == "SET_CUSTOM")
                {
                    replaceMethod=mIPReplaceMethod.SET_CUSTOM;
                    replaceIP= (String)mexampleRegistryKey.GetValue("replaceIP");
                }

                DNSOpenDNSChanged = b.Equals("TRUE") ? true : false;
                b = (String)mexampleRegistryKey.GetValue("OldDNS");
                oldDNS = b;

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
                b = (String) mexampleRegistryKey.GetValue("BlacklistToUse");
                if (b == "INTERNAL")
                    blacklistToUse=BlacklistTypes.INTERNAL;
                else if(b == "STEVENBLACK")                
                    blacklistToUse=BlacklistTypes.STEVENBLACK;
                else
                    blacklistToUse=BlacklistTypes.HOSTSFILEDOTNET;


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

            if (blacklistToUse==BlacklistTypes.STEVENBLACK)
                exampleRegistryKey.SetValue("BlacklistToUse", "STEVENBLACK");
            else if(blacklistToUse==BlacklistTypes.INTERNAL)
                exampleRegistryKey.SetValue("BlacklistToUse", "INTERNAL");
            else
                exampleRegistryKey.SetValue("BlacklistToUse","HOSTSFILENET");
     
            exampleRegistryKey.SetValue("DNSServiceDisabled", DNServiceDisabled ? "TRUE" : "FALSE");
            exampleRegistryKey.SetValue("DNSGoogleChanged",DNSGoogleChanged?"TRUE":"FALSE");
            exampleRegistryKey.SetValue("DNSOpenDNSChanged",DNSOpenDNSChanged?"TRUE":"FALSE");
            exampleRegistryKey.SetValue("OldDNS",oldDNS);
            if (replaceMethod == mIPReplaceMethod.KEEP_LOCALHOST)
            {
                exampleRegistryKey.SetValue("redirectType","KEEP_LOCALHOST");
            }
            else if (replaceMethod == mIPReplaceMethod.SET_CUSTOM)
            {
                exampleRegistryKey.SetValue("redirectType", "SET_CUSTOM");
                exampleRegistryKey.SetValue("replaceIP", replaceIP);
            }
            else
            {
                exampleRegistryKey.SetValue("redirectType","SET_WHITEPAGE");
            }
            
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
            try
            {
                dlg = new frmDialog();
                dlg.action = (String) action;
                if(dlg.IsAccessible)
                dlg.ShowDialog();
            }
            catch (ThreadAbortException ex)
            {
            }
        }

        public delegate void closedlgdeleg();

        public void closeDialog()
        {
            if (dlg.InvokeRequired)
                dlg.Invoke(new closedlgdeleg(closeDialog));
            else
                dlg.Close();
        }

        private String checkDuplicates(String txt, ref int cnt)
        {
            String[] txtArr = txt.Split('\n');
            String addTxt = "";


            foreach(String line in txtArr)
            {
                int count = new Regex(Regex.Escape(line)).Matches(txt).Count;
                if (count > 1)
                {
                    txt = txt.Replace(line + "\n", "");
                    addTxt += line + "\n";
                    cnt++;
                }                
            }
            return txt+addTxt;
        }

        //The update process
        private void updateHostsFile()
        {
            bool err = false;
            Thread start=null;
            try
            {

                if (urls.Count == 0 && blacklistToUse == BlacklistTypes.INTERNAL)
                {
                    frmDialog d = new frmDialog();
                    d.action = "Your personal hostsfile is empty.";
                    d.showButton = true;
                    d.ShowDialog();
                    err = true;

                }
                else
                {

                    //Get name of admin group
                    SecurityIdentifier id = new SecurityIdentifier("S-1-5-32-544");
                string adminGroupName = id.Translate(typeof(NTAccount)).Value;

                System.Net.WebClient wc = new System.Net.WebClient();
              
                if (!isHidden && !err)
                {
                    start = new Thread(new ParameterizedThreadStart(showDialog));
                    start.Start("Downloading hosts file(s)...");
                }


               

                    //Read hosts files from web
                    if (blacklistToUse == BlacklistTypes.INTERNAL)
                    {
                        foreach (String u in urls)
                            fileText += wc.DownloadString(u) + "\r\n";
                    }
                    else if (blacklistToUse == BlacklistTypes.STEVENBLACK)
                    {
                        fileText = wc.DownloadString(
                            "https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts");
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
                    else
                    {
                        fileText = wc.DownloadString("https://hosts-file.net/download/hosts.txt");
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


                    if (replaceMethod == mIPReplaceMethod.KEEP_LOCALHOST)
                        ipTo = ipFrom;
                    else if (replaceMethod == mIPReplaceMethod.SET_CUSTOM)
                    {
                        ipTo = replaceIP;
                    }
                    else
                        ipTo = WHITEPAGE_IP;
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
                if (!isHidden && !err)
                {
                    frmDialog f = new frmDialog();
                    f.showButton = true;
                    f.action = "Hosts file updated.";
                    f.ShowDialog();
                }

            }
            catch (Exception ex) //Hosts file update
            {
                if(start!=null)
                    start.Abort();
                //generate error string
                String add = "";
                if (!isHidden && !err)
                    if (isAntivir())
                    {
                        frmNotifyAntivirus f = new frmNotifyAntivirus();
                        f.ShowDialog();
                    }
                if (!isHidden && !err)
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
            executeNoWindow("certutil.exe", "-addstore \"Root\" "+ System.Reflection.Assembly.GetExecutingAssembly().CodeBase + "\\cert.pem");
            executeNoWindow("certutil.exe", "-addstore \"CA\" "+ System.Reflection.Assembly.GetExecutingAssembly().CodeBase + "\\cert.pem");
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

        private void bnMenuExit_Click(object sender, EventArgs e)
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

        private void bnMenuOptions_Click(object sender, EventArgs e)
        {
            bnDisableDNS.Text = isServiceActive() ? "Disable DNS-Client service" : "Enable DNS-Client service";
            fillOptions();
            tabControl1.SelectedIndex = 2;
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

        private void bnMenuAbout_Click(object sender, EventArgs e)
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
            tabControl1.SelectedIndex = 4;
            lblPage.Text = "About";

        }

        private void bnMenuMain_Click(object sender, EventArgs e)
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

        private void bnMenuHelp_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
            resetButtons();
            ((Button) sender).BackColor = Color.Navy;
            lblPage.Text = "Help";
            webBrowser1.Navigate("http://hostsmanager.lv-crew.org/readme.html");
        }

        private bool isADMember()
        {
            String pcname = Environment.GetEnvironmentVariable("computername");
            String domainname = Environment.GetEnvironmentVariable("logonserver");
            if (("\\\\" + pcname.ToLower()).Equals(domainname.ToLower()))
                return false;
            else
                return true;
        }

        private DialogResult showYesNoDialog(String text)
        {
            frmDialog d=new frmDialog();
            d.action = text;
            d.yesNoButtons = true;
            return d.ShowDialog();
        }

        private void bnUpdate_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (isADMember())
                {
                    if (blacklistToUse == BlacklistTypes.HOSTSFILEDOTNET)
                    {
                        showOKDIalog(
                            "Please use Steven Black's Blacklist when being a member of an Active Directory domain. Contact your system administrator for further information.\r\n\r\n",true);
                    }
                    else
                    {
                        updateHostsFile();
                    }
                }
                else
                {
                    DialogResult ret = DialogResult.Cancel;
                    if (isServiceActive())
                        ret=showYesNoDialog("Do you want to disable the DNS-Client service? Not doing so will slow down your computer.");                    
                    if (ret == DialogResult.OK)
                        disableDNSService();

                    updateHostsFile();
                }                                
            }
            catch (Exception ex)
            {
            }

        }

        private void bnEdit_Click_1(object sender, EventArgs e)
        {
            if (File.Exists(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts"))
            {
                FileSecurity fs = setHostsFilePermissions();
                doEdit.edit(internalEditor, urls, externalEditorFile);
                resetHostsFilePermissions(fs);
            }
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
            if (replaceMethod == mIPReplaceMethod.KEEP_LOCALHOST)
            {
                rbRedirectLocalhost.Checked = true;
            }
            else if (replaceMethod == mIPReplaceMethod.SET_CUSTOM)
            {
                rbRedirectCustom.Checked = true;
                txtReplaceIP.Text = replaceIP;
            }
            else
            {
                rbRedirectWhitepage.Checked = true;
            }

                lbAddHosts.Items.Clear();
            lbURLs.Items.Clear();
            //Fill IP Replacement
            /*
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
            */
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

            bnMenuMain.BackColor = Color.Navy;
            bnMenuOptions.BackColor = Color.Navy;
            bnMenuAbout.BackColor = Color.Navy;
            bnMenuExit.BackColor = Color.Navy;
            bnMenuHelp.BackColor = Color.Navy;

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
            
            if (rbUseCustomlBlacklist.Checked)
            {
                blacklistToUse=BlacklistTypes.INTERNAL;
                exampleRegistryKey.SetValue("BlacklistToUse", "INTERNAL");
            }
            else if (rbUseStevensBlacklist.Checked)
            {
                blacklistToUse = BlacklistTypes.STEVENBLACK;
                exampleRegistryKey.SetValue("BlacklistToUse", "STEVENBLACK");
            }
            else
            {
                blacklistToUse=BlacklistTypes.HOSTSFILEDOTNET;
                exampleRegistryKey.SetValue("BlacklistToUse","HOSTSFILENET");
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
            if ((rbRedirectLocalhost).Checked)
                replaceMethod = mIPReplaceMethod.KEEP_LOCALHOST;
            else if ((rbRedirectWhitepage).Checked)
                replaceMethod = mIPReplaceMethod.SET_WHITEPAGE;
            else
            {
                replaceMethod = mIPReplaceMethod.SET_CUSTOM;
                replaceIP = txtReplaceIP.Text;
            }
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

        private void bnMenuTools_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            lblPage.Text = "Tools";
        }

        private void label7_Click_1(object sender, EventArgs e)
        {

        }


        private static String getCurrentDNSServer()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                    IPAddressCollection dnsAddresses = ipProperties.DnsAddresses;

                    foreach (IPAddress dnsAdress in dnsAddresses)
                    {
                        return dnsAdress.ToString();
                    }
                }
            }
            return "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread start = null;
            if (DNSGoogleChanged)
            {
                start = new Thread(new ParameterizedThreadStart(showDialog));
                start.Start("Resetting DNS Server...");
                setDNS(oldDNS);
                DNSGoogleChanged = false;
                bnSetDNSOpenDNS.Text = "Set DNS Server to OpenDNS";
                bnSetDNSServerGoogle.Text = "Set DNS Server to Google";
                if (start != null)
                    start.Abort();
                showOKDIalog("DNS server has been reset.");
            }
            else
            {
                start = new Thread(new ParameterizedThreadStart(showDialog));
                start.Start("Setting DNS Server...");
                String _oldDNS = getCurrentDNSServer();
                setDNS("8.8.8.8");
                bnSetDNSServerGoogle.Text = "Reset DNS Server";
                bnSetDNSOpenDNS.Text = "Set DNS Server to OpenDNS";
                DNSGoogleChanged = true;
                DNSOpenDNSChanged = false;
                oldDNS = _oldDNS;
                if (start != null)
                    start.Abort();
                showOKDIalog("DNS server has been changed.");
            }
            saveSettings();            

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Thread start = null;
            if (DNSOpenDNSChanged)
            {
                start = new Thread(new ParameterizedThreadStart(showDialog));
                start.Start("Resetting DNS Server...");
                setDNS(oldDNS);
                DNSOpenDNSChanged = false;
                bnSetDNSOpenDNS.Text = "Set DNS Server to OpenDNS";
                bnSetDNSServerGoogle.Text = "Set DNS Server to Google";
                if (start != null)
                    start.Abort();
                showOKDIalog("DNS server has been reset.");
            }
            else
            {
                start = new Thread(new ParameterizedThreadStart(showDialog));
                start.Start("Setting DNS Server...");
                String _oldDNS = getCurrentDNSServer();
                setDNS("208.67.222.222,208.67.220.220");
                bnSetDNSOpenDNS.Text = "Reset DNS Server";
                bnSetDNSServerGoogle.Text = "Set DNS Server to Google";
                DNSOpenDNSChanged = true;
                DNSGoogleChanged = false;
                oldDNS = _oldDNS;
                if (start != null)
                    start.Abort();
                showOKDIalog("DNS server has been changed.");
            }
            saveSettings();
            if(start!=null && start.IsAlive)
                start.Abort();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                FileSecurity fs = setHostsFilePermissions();
                String hostsFile = System.IO.File.ReadAllText("default_hosts.tpl");
                System.IO.File.WriteAllText(
                    Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", hostsFile);
                resetHostsFilePermissions(fs);
            }
            catch (Exception ex)
            {
                frmDialog f=new frmDialog();
                f.action = "Something, most likely anitivirus software, is blocking the hosts file.";
                f.showButton = true;
                f.ShowDialog();
            }
        }

        private bool isServiceActive()
        {
            string path = "Win32_Service.Name='dnscache'";
            ManagementPath p = new ManagementPath(path);
            ManagementObject ManagementObj = new ManagementObject(p);
            return (ManagementObj["StartMode"].ToString().Equals("Auto"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread start = null;
            bool err = false;
            if (!isServiceActive())
            {
               
                
                    start = new Thread(new ParameterizedThreadStart(showDialog));
                    start.Start("Enabling DNS-Client Service...");
                    Process p = Process.Start("sc.exe", "config Dnscache start=auto");
                    p.WaitForExit();
                    ServiceController _ServiceController = new ServiceController("dnscache");
                    if (!_ServiceController.ServiceHandle.IsInvalid)
                    {
                        try
                        {
                            _ServiceController.Start();
                            _ServiceController.WaitForStatus(ServiceControllerStatus.StartPending,
                                TimeSpan.FromMilliseconds(10000));
                        }
                        catch (Exception ex)
                        {
                            err = true;
                        }
                    }
                    if (start != null && start.IsAlive)
                    {
                        start.Suspend();
                    }

                    if (!err)
                    {
                        showOKDIalog("DNS-Client service enabled.");
                        bnDisableDNS.Text = "Disable DNS-Client Service";
                        DNServiceDisabled = false;
                    }
                    else
                        showOKDIalog("Error enabling DNS-Client service. Please retry...");
           
            }
            else
            {
                if(!isADMember())
                    disableDNSService();
                else
                    showOKDIalog("You are a member of an Active Directory domain. This requires the DNS-Client service to be active.");
            }
   
        }

        private void disableDNSService()
        {
            System.Threading.Thread start=null;
            bool err = false;
            start = new Thread(new ParameterizedThreadStart(showDialog));
            start.Start("Disabling DNS-Client Service...");
            ServiceController _ServiceController = new ServiceController("dnscache");
            if (!_ServiceController.ServiceHandle.IsInvalid)
            {
                try
                {
                    _ServiceController.Stop();
                    _ServiceController.WaitForStatus(ServiceControllerStatus.Stopped,
                        TimeSpan.FromMilliseconds(10000));
                }
                catch (Exception ex)
                {
                    err = true;
                }
                try
                {
                    if (!err)
                    {
                        Process p = Process.Start("sc.exe", "config dnscache start=disabled");
                        p.WaitForExit();
                        DNServiceDisabled = true;
                        bnDisableDNS.Text = "Enable DNS-Client Service";
                    }
                }
                catch (Exception ex)
                {
                    err = true;
                }
            }
            if (start != null && start.ThreadState == System.Threading.ThreadState.Running)
                start.Suspend();
            if (!err)
                showOKDIalog("DNS-Client service disabled.");
            else
                showOKDIalog("Erro disabling DNS-Client service. Please retry..");
        }

         public void changeServiceStartMode(string hostname, string serviceName, string startMode)
        {
            try
            {
                ManagementObject classInstance =
                    new ManagementObject(@"\\" + hostname + @"\root\cimv2",
                        "Win32_Service.Name='" + serviceName + "'",
                        null);

                // Obtain in-parameters for the method
                ManagementBaseObject inParams =
                    classInstance.GetMethodParameters("ChangeStartMode");

                // Add the input parameters.
                inParams["StartMode"] = startMode;

                // Execute the method and obtain the return values.
                ManagementBaseObject outParams =
                    classInstance.InvokeMethod("ChangeStartMode", inParams, null);

                // List outParams
                //Console.WriteLine("Out parameters:");
                //richTextBox1.AppendText(DateTime.Now.ToString() + ": ReturnValue: " + outParams["ReturnValue"]);
            }
            catch (ManagementException err)
            {
                //richTextBox1.AppendText(DateTime.Now.ToString() + ": An error occurred while trying to execute the WMI method: " + err.Message);
            }
        }
        public void setDNS(String DNS)
        {

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet) || (nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)) //&& (nic.OperationalStatus == OperationalStatus.Up))
                {


            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (ManagementObject objMO in objMOC)
            {
                if ((bool)objMO["IPEnabled"])
                {
                    // if you are using the System.Net.NetworkInformation.NetworkInterface you'll need to change this line to if (objMO["Caption"].ToString().Contains(NIC)) and pass in the Description property instead of the name 
                   // if (objMO["Caption"].Equals(NIC))
                    //{
                        try
                        {
                            ManagementBaseObject newDNS =
                                objMO.GetMethodParameters("SetDNSServerSearchOrder");
                            newDNS["DNSServerSearchOrder"] = DNS.Split(',');
                            ManagementBaseObject setDNS =
                                objMO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    //}
                }
            }
        }
    }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            frmDialog f=new frmDialog();
            f.action = "Edit hosts file: Edit the hosts file\r\nReset hosts file: Set hostsfile to Windows standard\r\nFlush DNS cache: removes temporarily saved DNS data.\r\nDisable DNS Service: Do this if your System slows down while surfing.\r\nSet DNS Server to Google: Set your DNS Server IP to that of Google (8.8.8.8)\r\nSet DNS Server to OpenDNS. Set your DNS Server IP to that of OpenDNS";
            f.showButton = true;
            f.customHeight = 150;
            f.customWidth = 400;    
            f.ShowDialog();
        }

        private void tabTools_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                FileSecurity fs = setHostsFilePermissions();
                String hostsFile = System.IO.File.ReadAllText("default_hosts.tpl");
                System.IO.File.WriteAllText(
                    Environment.GetEnvironmentVariable("windir") + "\\System32\\drivers\\etc\\hosts", hostsFile);
                resetHostsFilePermissions(fs);
            }
            catch (Exception ex)
            {
                showOKDIalog("Hosts file blocked. Please disable hosts file protection in your antivirus software.");
            }
        }

        private void showOKDIalog(String text, bool larger=false)
        {
            frmDialog f = new frmDialog();
            f.action = text;
            if (larger)
                f.customHeight = 150;
            f.showButton = true;      
            f.ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (isServiceActive())
            {
                try
                {
                    Process P = Process.Start("ipconfig.exe", "/flushdns");
                    P.WaitForExit();
                    showOKDIalog("The DNS cache has been flushed.");
                }
                catch (Exception ex)
                {
                    showOKDIalog("Error flushing dns cache.");
                }
            }
            else
            {
                showOKDIalog("DNS-Client service disabled. Flushing the DNS cache doesnt have any effect.");
            }
        }

        private void bnFlushDNSCache_Click(object sender, EventArgs e)
        {
            try
            {
                Process p = System.Diagnostics.Process.Start("ipconfig.exe", "/flushdns");
                p.Start();
                p.WaitForExit();
                showOKDIalog("DNS Cache has been flushed.");
            }
            catch (Exception ex)
            {
                
            }
        }

        private void tabTools_Click_1(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
         
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
      
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            try
            {
                FileSecurity fs = setHostsFilePermissions();
                String ret = System.IO.File.ReadAllText("default_hosts.tpl");
                System.IO.File.WriteAllText(
                    Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", ret);
                resetHostsFilePermissions(fs);
                showOKDIalog("Hosts fiele has been reset to system defaults.");
            }
            catch (Exception ex)
            {
                showOKDIalog("Could not write hosts file. Please check your antivirus for hosts file potection.");
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            if (File.Exists(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts"))
            {
                try
                {
                    showOKDIalog("Removing duplicates. This may take up to an hour.");
                    int cnt = 0;
                    FileSecurity fs = setHostsFilePermissions();
                    String hosts = File.ReadAllText(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
                    hosts = checkDuplicates(hosts, ref cnt);
                    File.WriteAllText(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", hosts);
                    resetHostsFilePermissions(fs);
                    showOKDIalog(cnt.ToString() + " duplicates removed.");
                }catch(Exception ex)
                {
                    showOKDIalog("Error accessing hosts file. Please check your antivirus.");
                }
            }
            else
                showOKDIalog("Hosts file not found.");
        }
    }
}