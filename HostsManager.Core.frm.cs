/*  This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.    You should have received a copy of the GNU General Public License
    along with this program.If not, see<http://www.gnu.org/licenses/>.    
    
    Idea:   Tobias B. Besemer
    Coding: Dennis M. Heine
    
*/using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using HostsManager.Settings;

namespace HostsManager
{
    //-----------------Form Methods-------------------
    public partial class frmHostsManager : Form, IMessageFilter
    {
        public String WHITEPAGE_IP = "1.2.3.4";
        public
        const int WM_NCLBUTTONDOWN = 0xA1;
        public
        const int HT_CAPTION = 0x2;
        public
        const int WM_LBUTTONDOWN = 0x0201;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private HashSet<Control> controlsToMove = new HashSet<Control>();
        public enum BlacklistTypes
        {
            INTERNAL = 1,
            STEVENBLACK = 2,
            HOSTSFILEDOTNET = 3
        }

        public enum mIPReplaceMethod
        {
            KEEP_LOCALHOST = 0,
            SET_WHITEPAGE = 1,
            SET_CUSTOM = 2
        }
        
        public frmHostsManager()
        {
            InitializeComponent();
            clsBrandingINI.readINI();
            SettingsData.ipTo = Branding.DefaultBlankHost;
            if (File.Exists(Branding.BannerImage))
            {
                pbPicture.Image = Image.FromFile(Branding.BannerImage);
                pictureBox3.Image = Image.FromFile(Branding.BannerImage);
            }

            
            (new Settings.Settings(this)).loadSettings();
            //Check whether to run silently
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                if (arguments[1] == "/auto")
                {
                    this.Opacity = 0;
                    this.ShowInTaskbar = false;
                    this.WindowState = FormWindowState.Minimized;
                    SettingsData.isHidden = true;
                }
            }            
        }

        //Load main form
        private void frmHostsManager_Load(object sender, EventArgs e)
        {
            txtCustomEditor.Text = Environment.GetEnvironmentVariable("windir") + "\\system32\\notepad.exe";
            try
            {
                if (!SettingsData.isHidden)
                {
                    SettingsData.player = new System.Media.SoundPlayer();
                    if (File.Exists(Branding.BackgroundSound))
                    {
                        SettingsData.player.SoundLocation = Branding.BackgroundSound;//"bgnd.wav";
                        SettingsData.player.Play();
                    }
                }
            }
            catch (Exception) { }

            //Hide tabs
            tabCtrlPages.Appearance = TabAppearance.FlatButtons;
            tabCtrlPages.ItemSize = new Size(0, 1);
            tabCtrlPages.SizeMode = TabSizeMode.Fixed;

            tabCtrlOptions.Appearance = TabAppearance.FlatButtons;

            //Add form move
            Application.AddMessageFilter(this);
            controlsToMove.Add(this);
            controlsToMove.Add(this.tabCtrlPages);
            controlsToMove.Add(this.pnlTitleBar);
            //Import Certificate Authority
            //importCert();
            ReadFirefoxProfile();
            bnUpdate.Select();
            checkForSilentRun();
            loadListsToDownload();
            try
            {
                this.Icon = new Icon(Branding.ICONPATH);
            }
            catch (Exception) { }
            Settings.Settings s=new Settings.Settings(this);
            s.loadOptions();
            updateStats();
        }

        //=============Control Elements events========================

        private void bnCheckForUpdates_Click(object sender, EventArgs e)
        {
            wbUpdates.Visible = true;
            wbUpdates.Navigate("https://github.com/LV-Crew/HostsManager/releases/");
        }

        private void bnSetDNSServerGoogle_Click(object sender, EventArgs e)
        {
            Thread start = null;
            if (SettingsData.DNSGoogleChanged)
            {
                start = new Thread(new ParameterizedThreadStart(showDialog));
                start.Start("Resetting DNS Server...");
                setDNS(SettingsData.oldDNS);
                SettingsData.DNSGoogleChanged = false;
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
                SettingsData.DNSGoogleChanged = true;
                SettingsData.DNSOpenDNSChanged = false;
                SettingsData.oldDNS = _oldDNS;
                if (start != null)
                    start.Abort();
                showOKDIalog("DNS server has been changed.");
            }
            (new Settings.Settings(this)).saveSettings();
        }

        private void bnSetDNSOpenDNS_Click(object sender, EventArgs e)
        {
            Thread start = null;
            if (SettingsData.DNSOpenDNSChanged)
            {
                start = new Thread(new ParameterizedThreadStart(showDialog));
                start.Start("Resetting DNS Server...");
                setDNS(SettingsData.oldDNS);
                SettingsData.DNSOpenDNSChanged = false;
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
                SettingsData.DNSOpenDNSChanged = true;
                SettingsData.DNSGoogleChanged = false;
                SettingsData.oldDNS = _oldDNS;
                if (start != null)
                    start.Abort();
                showOKDIalog("DNS server has been changed.");
            }
            (new Settings.Settings(this)).saveSettings();
            if (start != null && start.IsAlive)
                start.Abort();
        }

        private void bnHelpTools_Click(object sender, EventArgs e)
        {
            frmDialog f = new frmDialog();
            f.action =
             "Edit hosts file: Edit the hosts file.\r\n" +
             "Remove duplicates: Removes duplicate entrys from the hosts file.\r\n" +
             "Reset hosts file: Set hostsfile to Windows standard.\r\n" +
             "Flush DNS cache: removes temporarily saved DNS data.\r\n" +
             "Disable DNS Service: Do this if your System slows down while surfing.\r\n" +
             "Set DNS Server to Google: Set your DNS Server IP to that of Google (8.8.8.8).\r\n" +
             "Set DNS Server to OpenDNS: Set your DNS Server IP to that of OpenDNS.";
            f.showButton = true;
            f.customHeight = 200;
            f.customWidth = 400;
            f.ShowDialog();
        }

        private void bnDisableDNS_Click(object sender, EventArgs e)
        {
            Thread start = null;
            bool err = false;
            if (!isServiceActive())
            {

                start = new Thread(new ParameterizedThreadStart(showDialog));
                start.Start("Enabling DNS-Client Service...");
                Process p = executeNoWindow("sc.exe", "config Dnscache start=auto");
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
                    catch (Exception)
                    {
                        err = true;
                    }
                }
                if (start != null && start.IsAlive)
                {
                    start.Abort();
                }
                if (!err)
                {
                    showOKDIalog("DNS-Client service enabled.");
                    bnDisableDNS.Text = "Disable DNS-Client Service";
                    SettingsData.DNServiceDisabled = false;
                }
                else
                    showOKDIalog("Error enabling DNS-Client service. Please retry...");
            }
            else
            {
                if (!isADMember())
                    disableDNSService();
                else
                    showOKDIalog("You are a member of an Active Directory domain. This requires the DNS-Client service to be active.");
            }
        }

        private void bnFlushDNSCache_Click(object sender, EventArgs e)
        {
            try
            {
                Process p = executeNoWindow("ipconfig.exe", "/flushdns");
                p.Start();
                p.WaitForExit();
                showOKDIalog("DNS Cache has been flushed.");
            }
            catch (Exception)
            {
            }
        }

        private void bnResetHostsFile_Click(object sender, EventArgs e)
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
            catch (Exception)
            {
                showOKDIalog("Could not write hosts file. Please check your antivirus for hosts file potection.");
            }
        }

        private void bnDuplicates_Click(object sender, EventArgs e)
        {
            frmDialog d = new frmDialog();
            d.action = "Depending on your hosts file size,\nthis can take up to an hour.";
            d.yesNoButtons = true;
            DialogResult r = d.ShowDialog();
            if (r == DialogResult.OK)
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
                    }
                    catch (Exception)
                    {
                        showOKDIalog("Error accessing hosts file. Please check your antivirus.");
                    }
                }
                else
                    showOKDIalog("Hosts file not found.");
            }
        }

        private void rbUseHostsFileBL_CheckedChanged(object sender, EventArgs e)
        {
            SettingsData.UseHostsFileBlacklist = ((CheckBox)sender).Checked;
            (new Settings.Settings(this)).saveSettings();
        }

        private void rbUseStevenBlacksBL_CheckedChanged(object sender, EventArgs e)
        {
            SettingsData.UseStevensBlacklist = ((CheckBox)sender).Checked;
            (new Settings.Settings(this)).saveSettings();
        }

        private void rbUseCustomBL_CheckedChanged(object sender, EventArgs e)
        {
            SettingsData.UseCustomBlacklist = ((CheckBox)sender).Checked;
            (new Settings.Settings(this)).saveSettings();
        }

        private void cbAutoUpdate_Click(object sender, EventArgs e)
        {
            SettingsData.autoUpdate = cbAutoUpdate.Checked;
            doAutoUpdate();
            (new Settings.Settings(this)).saveSettings();
        }

        private void bnHelpMain_Click(object sender, EventArgs e)
        {
            frmDialog f = new frmDialog();
            f.action =
             "Select one or more blacklists to update your system.\r\n" +
             "The selected blacklists will get added into your hosts file.";
            f.showButton = true;
            f.customHeight = 100;
            f.customWidth = 400;
            f.ShowDialog();
        }

        private void bnHelpOptionsMain_Click(object sender, EventArgs e)
        {
            frmDialog f = new frmDialog();
            f.action =
             "Redirection: Sets the IP to which blacklisted hosts are being redirected to.\r\n" +
             "Hosts file editor: Sets the editor that is being used by Hosts Manager to edit the hosts file.\r\n" +
             "Automatically update host file: Creates a Windows task which updates the hosts file daily.";
            f.showButton = true;
            f.customHeight = 120;
            f.customWidth = 500;
            f.ShowDialog();
        }

        private void bnHelpOptionsStevenBlack_Click(object sender, EventArgs e)
        {
            frmDialog f = new frmDialog();
            f.action =
             "This dialog allows you to selct the categorys of\r\n" +
             "Steven Black's hosts file you whish to use.";
            f.showButton = true;
            f.customHeight = 120;
            f.customWidth = 300;
            f.ShowDialog();
        }

        private void bnHelpOptionsCustom_Click(object sender, EventArgs e)
        {
            frmDialog f = new frmDialog();
            f.action =
             "Custom hosts file URLs: Aditional URLs to download the hosts file from.\r\n" +
             "Add hosts to blacklist: Aditional hostnames that should be blacklisted.";
            f.showButton = true;
            f.customHeight = 120;
            f.customWidth = 400;
            f.ShowDialog();
        }

        private void cbFakeNews_CheckedChanged(object sender, EventArgs e)
        {
            SettingsData.showFakeNews = cbFakeNews.Checked;
        }

        private void cbPorn_Click(object sender, EventArgs e)
        {
            saveListsToDownload();
            Microsoft.Win32.RegistryKey exampleRegistryKey =
             Microsoft.Win32.Registry.CurrentUser.CreateSubKey("HostsManager");
            if (SettingsData.showGambling)
                exampleRegistryKey.SetValue("ShowGambling", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowGambling", "FALSE");
            if (SettingsData.showFakeNews)
                exampleRegistryKey.SetValue("ShowFakeNews", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowFakeNews", "FALSE");
            if (SettingsData.showPorn)
                exampleRegistryKey.SetValue("ShowPorn", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowPorn", "FALSE");
            if (SettingsData.showSocial)
                exampleRegistryKey.SetValue("ShowSocial", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowSocial", "FALSE");
            exampleRegistryKey.Close();
        }

        private void cbBackgroundMusic_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBackgroundMusic.Checked)
            {
                if (SettingsData.player != null)
                    SettingsData.player.Play();
            }
            else
            {
                if (SettingsData.player != null)
                    SettingsData.player.Stop();
            }
        }

        private void rbUseStevensBlacklist_CheckedChanged(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey exampleRegistryKey =
             Microsoft.Win32.Registry.CurrentUser.CreateSubKey("HostsManager");
            if (rbUseStevenBlacksBL.Checked)
            {
                SettingsData.UseStevensBlacklist = true;
                exampleRegistryKey.SetValue("UseStevenBlack", "TRUE");
            }
            else
            {
                SettingsData.UseStevensBlacklist = false;
                exampleRegistryKey.SetValue("UseStevenBlack", "FALSE");
            }
            if (rbUseCustomBL.Checked)
            {
                SettingsData.UseCustomBlacklist = true;
                exampleRegistryKey.SetValue("UseCustom", "TRUE");
            }
            else
            {
                SettingsData.UseCustomBlacklist = true;
                exampleRegistryKey.SetValue("UseCustom", "TRUE");
            }
            if (rbUseHostsFileBL.Checked)
            {
                SettingsData.UseHostsFileBlacklist = true;
                exampleRegistryKey.SetValue("UseHostsFie", "TRUE");
            }
        }

        private void bnAddHost_Click(object sender, EventArgs e)
        {
            if (txtAddHost.Text != "")
                lbAddHosts.Items.Add(txtAddHost.Text);
            txtAddHost.Text = "";
        }

        private void bnRemoveHost_Click(object sender, EventArgs e)
        {
            if (lbAddHosts.SelectedIndex >= 0)
                lbAddHosts.Items.Remove(lbAddHosts.SelectedItem);
        }

        private void bnAdd_Click(object sender, EventArgs e)
        {
            String url = txtURL.Text;
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result)
            {
                lbURLs.Items.Add(txtURL.Text);
                txtURL.Text = "";
            }
            else
                showOKDIalog("Wrong format. URL must begin with http://");
        }

        private void bnRemove_Click(object sender, EventArgs e)
        {
            if (lbURLs.SelectedIndex >= 0)
                lbURLs.Items.Remove(lbURLs.SelectedItem);
        }

        private void bnSave_Click(object sender, EventArgs e)
        {
            if ((rbCustom.Checked && File.Exists(txtCustomEditor.Text)) || rbExternal.Checked || rbInternal.Checked)
            {
                saveSettingsOptions();
                (new Settings.Settings(this)).saveSettings();
            }
            else
            {
                frmDialog f = new frmDialog();
                f.action = "Please enter a a valid editor path.";
                f.showButton = true;
                f.ShowDialog();
            }
        }

        private void lblHostsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/StevenBlack/hosts");
        }

        private void bnCustomEditor_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Application (*.exe)|*.exe";
            d.DefaultExt = ".exe";
            DialogResult res = d.ShowDialog();
            if (res == DialogResult.OK)
            {
                txtCustomEditor.Text = d.FileName;
            }
        }

        private void bnSaveOptions2_Click(object sender, EventArgs e)
        {
            if ((rbRedirectLocalhost).Checked)
                SettingsData.replaceMethod = SettingsData.mIPReplaceMethod.KEEP_LOCALHOST;
            else if ((rbRedirectWhitepage).Checked)
                SettingsData.replaceMethod = SettingsData.mIPReplaceMethod.SET_WHITEPAGE;
            else
            {
                SettingsData.replaceMethod = SettingsData.mIPReplaceMethod.SET_CUSTOM;
                SettingsData.replaceIP = txtReplaceIP.Text;
            }
            if ((rbCustom.Checked && File.Exists(txtCustomEditor.Text)) || rbExternal.Checked || rbInternal.Checked)
            {
                saveSettingsOptions();
                (new Settings.Settings(this)).saveSettings();
            }
            else
            {
                frmDialog f = new frmDialog();
                f.action = "Please enter a a valid editor path.";
                f.showButton = true;
                f.ShowDialog();
            }
        }

        private void bnMenuTools_Click(object sender, EventArgs e)
        {
            tabCtrlPages.SelectedIndex = 1;
            lblPage.Text = "Tools";
        }

        //Show homepage
        private void pbPicture_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://hostsmanager.lv-crew.org");
        }

        private void bnMenuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bnCloseForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bnMinimizeForm_Clock(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void bnMenuOptions_Click(object sender, EventArgs e)
        {
            bnDisableDNS.Text = isServiceActive() ? "Disable DNS-Client service" : "Enable DNS-Client service";
            Settings.Settings s = new Settings.Settings(this);
            s.loadOptions();
            tabCtrlPages.SelectedIndex = 2;
            resetButtons();
            ((Button)sender).BackColor = Color.Navy;
            lblPage.Text = "Options";
        }

        private void bnMenuAbout_Click(object sender, EventArgs e)
        {
            lblVersion.Text = "Version: " + Branding.VERSION;
            lblName.Text = Branding.COMPANY + " " + Branding.PRODUCT;
            try
            {
                pictureBox2.ImageLocation = Branding.PRODUCTIMGPATH;
            }
            catch (Exception) { }
            tabCtrlPages.SelectedIndex = 4;
            lblPage.Text = "About";
        }

        private void bnMenuMain_Click(object sender, EventArgs e)
        {
            tabCtrlPages.SelectedIndex = 0;
            resetButtons();
            ((Button)sender).BackColor = Color.Navy;
            lblPage.Text = "Main";
            updateStats();
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.Blue, 4);
            g.DrawRectangle(p, this.tabMain.Bounds);
        }

        private void bnMenuHelp_Click(object sender, EventArgs e)
        {
            tabCtrlPages.SelectedIndex = 3;
            resetButtons();
            ((Button)sender).BackColor = Color.Navy;
            lblPage.Text = "Help";
            string curDir = Directory.GetCurrentDirectory();            
            try
            {
                string html = File.ReadAllText(curDir+"\\readme.html");
                wbWebbrowserHelp.DocumentText = html;
            }
            catch (Exception) { }          
        }

        private void bnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (isADMember())
                {
                    if (SettingsData.UseHostsFileBlacklist)
                    {
                        showOKDIalog(
                         "Please use only Steven Black's Blacklist when being a member of an Active Directory domain. Contact your system administrator for further information.\r\n\r\n", true);
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
                        ret = showYesNoDialog("Do you want to disable the DNS-Client service? Not doing so will slow down your computer.");
                    if (ret == DialogResult.OK)
                        disableDNSService();
                    updateHostsFile();
                }
                updateStats();
            }
            catch (Exception) { }
        }

        private void bnEdit_Click(object sender, EventArgs e)
        {
            if (File.Exists(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts"))
            {
                FileSecurity fs = setHostsFilePermissions();
                doEdit.edit(SettingsData.internalEditor, SettingsData.urls, SettingsData.externalEditorFile);
                resetHostsFilePermissions(fs);
            }
        }

        //-----------------Generic Methods-------------------
        public void saveSettingsOptions()
        {
            if (cbAutoUpdate.Checked)
                SettingsData.autoUpdate = true;
            else
                SettingsData.autoUpdate = false;
            if (rbInternal.Checked)
            {
                SettingsData.internalEditor = "INTERNAL";
            }
            else if (rbExternal.Checked)
            {
                SettingsData.internalEditor = "WORDPAD";
            }
            else
            {
                SettingsData.internalEditor = "CUSTOM";
                SettingsData.externalEditorFile = txtCustomEditor.Text;
            }
            if ((txtFrom.ForeColor == Color.Gray && txtTo.ForeColor == Color.Black) ||
             (txtFrom.ForeColor == Color.Black && txtTo.ForeColor == Color.Gray))
                MessageBox.Show("Please enter both \"From\" and \"To\" IP");
            else
            {
                if (txtFrom.Text != "")
                    SettingsData.ipFrom = txtFrom.Text;
                if (txtTo.Text != "")
                    SettingsData.ipTo = txtTo.Text;
                SettingsData.urls.Clear();
                foreach (String item in lbURLs.Items)
                {
                    SettingsData.urls.Add(item);
                }
                SettingsData.addHosts.Clear();
                foreach (String host in lbAddHosts.Items)
                {
                    SettingsData.addHosts.Add(host);
                }
            }
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
            frmDialog d = new frmDialog();
            d.action = text;
            d.yesNoButtons = true;
            return d.ShowDialog();
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
            catch (Exception) { }
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
            catch (Exception) { }
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
                dlg.action = (String)action;
                if (dlg.IsAccessible)
                    dlg.ShowDialog();
            }
            catch (ThreadAbortException) { }
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

            foreach (String line in txtArr)
            {
                int count = new Regex(Regex.Escape(line)).Matches(txt).Count;
                if (count > 1)
                {
                    txt = txt.Replace(line + "\n", "");
                    addTxt += line + "\n";
                    cnt++;
                }
            }
            return txt + addTxt;
        }

        class Downloader
        {
            public String DownloadString(String url)
            {
                String ret = "";
                WebClient wc = new WebClient();
                try
                {
                    ret = wc.DownloadString(url);
                }
                catch (Exception) { }
                return ret;
            }
        }

        //The update process
        private void updateHostsFile()
        {
            bool err = false;
            Thread start = null;
            try
            {
                if (SettingsData.urls.Count == 0 && SettingsData.UseCustomBlacklist && !SettingsData.isHidden)
                {
                    frmDialog d = new frmDialog();
                    d.action = "Your personal hosts file is empty.";
                    d.showButton = true;
                    d.ShowDialog();
                    err = true;
                }
                else
                {
                    //Get name of admin group
                    SecurityIdentifier id = new SecurityIdentifier("S-1-5-32-544");
                    string adminGroupName = id.Translate(typeof(NTAccount)).Value;

                    Downloader wc = new Downloader();
                    if (!SettingsData.isHidden && !err)
                    {
                        start = new Thread(new ParameterizedThreadStart(showDialog));
                        start.Start("Downloading hosts file(s)...");
                    }

                    String fileText = "";
                    //Read hosts files from web
                    if (SettingsData.UseCustomBlacklist)
                    {
                        foreach (String u in SettingsData.urls)
                            fileText += "\r\n" + wc.DownloadString(u);
                    }
                    if (SettingsData.UseStevensBlacklist)
                    {
                        fileText += wc.DownloadString(
                         "https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts");
                        if (SettingsData.showFakeNews)
                            fileText += wc.DownloadString(
                             "https://raw.githubusercontent.com/StevenBlack/hosts/master/extensions/fakenews/hosts");
                        if (SettingsData.showGambling)
                            fileText += wc.DownloadString(
                             "https://raw.githubusercontent.com/StevenBlack/hosts/master/extensions/gambling/hosts");
                        if (SettingsData.showPorn)
                        {
                            fileText += wc.DownloadString(
                             "https://raw.githubusercontent.com/StevenBlack/hosts/master/extensions/porn/clefspeare13/hosts");
                            fileText += wc.DownloadString(
                             "https://raw.githubusercontent.com/StevenBlack/hosts/master/extensions/porn/sinfonietta/hosts");
                        }
                        if (SettingsData.showSocial)
                            fileText += wc.DownloadString(
                             "https://raw.githubusercontent.com/StevenBlack/hosts/master/extensions/social/hosts");
                    }
                    if (SettingsData.UseHostsFileBlacklist)
                    {
                        fileText += wc.DownloadString("https://hosts-file.net/download/hosts.txt");
                        fileText += wc.DownloadString("https://hosts-file.net/hphosts-partial.txt");
                    }
                    if (!SettingsData.isHidden)
                        closeDialog();
                    if (!SettingsData.isHidden)
                    {
                        start = new Thread(new ParameterizedThreadStart(showDialog));
                        start.Start("Replacing IP's");
                    }

                    //IP Overwrite
                    if (SettingsData.replaceMethod == SettingsData.mIPReplaceMethod.KEEP_LOCALHOST)
                        SettingsData.ipTo = SettingsData.ipFrom;
                    else if (SettingsData.replaceMethod == SettingsData.mIPReplaceMethod.SET_CUSTOM)
                    {
                        SettingsData.ipTo = SettingsData.replaceIP;
                    }
                    else
                    {
                        SettingsData.ipTo = Branding.DefaultBlankHost;
                        SettingsData.ipTo =Dns.GetHostAddresses(SettingsData.ipTo)[0].ToString();
                    }
                    if (SettingsData.replaceMethod != SettingsData.mIPReplaceMethod.KEEP_LOCALHOST)
                    {
                        fileText = fileText.Replace("0.0.0.0", SettingsData.ipTo);
                        fileText = fileText.Replace("127.0.0.1", SettingsData.ipTo);
                        fileText = fileText.Replace("::1", SettingsData.ipTo);
                        fileText = fileText.Replace("0\t", SettingsData.ipTo +"\t");
                    }
                    foreach (String host in SettingsData.addHosts)
                        fileText += "\r\n" + SettingsData.ipTo + " " + host;

                    //CR/LF detection
                    fileText = fileText.Replace("\r", "");
                    fileText = fileText.Replace("\n", "\r\n");
                    if (!SettingsData.isHidden)
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
                if (!SettingsData.isHidden && !err)
                {
                    frmDialog f = new frmDialog();
                    f.showButton = true;
                    f.action = "Hosts file updated.";
                    f.ShowDialog();
                }
            }
            catch (Exception ex) //Hosts file update
            {
                if (start != null)
                    start.Abort();
                //generate error string
                String add = "";
                if (!SettingsData.isHidden && !err)
                    if (isAntivir())
                    {
                        frmNotifyAntivirus f = new frmNotifyAntivirus();
                        f.ShowDialog();
                    }
                if (!SettingsData.isHidden && !err)
                    MessageBox.Show("Error: " + add + ex.Message);
            }
        }

        //Start external process with hidden window      
        private Process executeNoWindow(String cmd, String param)
        {
            try
            {
                ProcessStartInfo pi = new ProcessStartInfo(cmd, param);
                pi.CreateNoWindow = true;
                pi.UseShellExecute = false;
                pi.RedirectStandardOutput = true;
                pi.RedirectStandardError = true;
                pi.RedirectStandardInput = true;
                pi.WindowStyle = ProcessWindowStyle.Hidden;
                return Process.Start(pi);
            }
            catch (Exception) { }
            return null;
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
            if (SettingsData.autoUpdate)
            {
                //Create task using schtasks.exe
                //FileSecurity fs=setHostsFilePermissions(); !!! 080417DH - muss evtl wieder reingemacht werden.
                String Arguments = "/Create /tn \"LV-Crew HostsManager\" /tr \"" +
                 System.Reflection.Assembly.GetEntryAssembly().Location +
                 " /auto\" /sc DAILY /RL HIGHEST /F";
                Process p = executeNoWindow("schtasks.exe", Arguments);
            }
            else
            {
                //Delete task using schtasks.exe                
                executeNoWindow("schtasks.exe", "/Delete /tn LV-Crew.HostsManager /F");
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
                        string[] lines = resp.Split(new string[] {
       "\r\n"
      }, StringSplitOptions.None);
                        string location = lines.First(x => x.Contains("Path="))
                         .Split(new string[] {
        "="
                         }, StringSplitOptions.None)[1];
                        foreach (String line in lines)
                        {
                            if (line.Contains("Path="))
                            {
                                String profpath = line.Split(new string[] {
         "="
        }, StringSplitOptions.None)[1];
                                profpath = System.IO.Path.Combine(firefox, profpath).Replace("/", "\\");
                                importCert(profpath);
                            }
                        }
                        string prof_dir = System.IO.Path.Combine(firefox, location);
                        return prof_dir.Replace("/", "\\");
                    }
                }
            }
            return "";
        }

        private void importCert(String profPath)
        {
            //Get Firefox profile path
            //String profPath = ReadFirefoxProfile();
            String currpath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            currpath = currpath.Substring(0, currpath.LastIndexOf("/"));
            currpath = currpath.Replace("file:///", "").Replace("/", "\\");
            //Import certificate to FF
            executeNoWindow(currpath + "\\certutil\\certutil_moz.exe",
             "-A -n \"Testcert\" -t \"TCu,Cuw,Tuw\" -i " + currpath + "\\cert.pem -d \"" + profPath + "\"");
            //Import certificate to Win
            executeNoWindow("certutil.exe", "-addstore \"Root\" " + currpath + "\\cert.pem");
            executeNoWindow("certutil.exe", "-addstore \"CA\" " + currpath + "\\cert.pem");
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
                    if (!SettingsData.isHidden)
                    {
                        frmNotifyAntivirus f = new frmNotifyAntivirus();
                        f.ShowDialog();
                    }

                //Branding
                this.Text = Branding.COMPANY + " " + Branding.PRODUCT + " v" + Branding.VERSION;
                try
                {
                    pbPicture.ImageLocation = Branding.PRODUCTIMGPATH;
                }
                catch (Exception) { }
            }
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
            SettingsData.showFakeNews = cbFakeNews.Checked;
            SettingsData.showGambling = cbGambling.Checked;
            SettingsData.showPorn = cbPorn.Checked;
            SettingsData.showSocial = cbSocial.Checked;
        }

        private void loadListsToDownload()
        {
            cbFakeNews.Checked = SettingsData.showFakeNews;
            cbGambling.Checked = SettingsData.showGambling;
            cbPorn.Checked = SettingsData.showPorn;
            cbSocial.Checked = SettingsData.showSocial;
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

        private bool isServiceActive()
        {
            string path = "Win32_Service.Name='dnscache'";
            ManagementPath p = new ManagementPath(path);
            ManagementObject ManagementObj = new ManagementObject(p);
            return (ManagementObj["StartMode"].ToString().Equals("Auto"));
        }

        private void disableDNSService()
        {
            System.Threading.Thread start = null;
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
                catch (Exception)
                {
                    err = true;
                }
                try
                {
                    if (!err)
                    {
                        Process p = executeNoWindow("sc.exe", "config dnscache start=disabled");
                        p.WaitForExit();
                        SettingsData.DNServiceDisabled = true;
                        bnDisableDNS.Text = "Enable DNS-Client Service";
                    }
                }
                catch (Exception)
                {
                    err = true;
                }
            }
            if (start != null && start.ThreadState == System.Threading.ThreadState.Running)
                start.Abort();
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
            }
            catch (ManagementException) { }
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
                        }
                    }
                }
            }
        }

        private void showOKDIalog(String text, bool larger = false)
        {
            frmDialog f = new frmDialog();
            f.action = text;
            if (larger)
                f.customHeight = 150;
            f.showButton = true;
            f.ShowDialog();
        }

        private void updateStats()
        {
            if (File.Exists(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts"))
            {
                try
                {
                    String[] hf = File.ReadAllLines(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
                    FileInfo fi = new FileInfo(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
                    if (fi.Length >= 1024)
                    {
                        String sz = ((double)fi.Length / 1024).ToString();
                        int li = sz.LastIndexOf(".");
                        if (li <= 0)
                            li = sz.LastIndexOf(",");
                        sz = sz.Substring(0, li);
                        lblCurrent.Text = hf.Length.ToString() + " Lines; " + sz + " KB";
                    }
                    else
                        lblCurrent.Text = hf.Length.ToString() + " Lines; " + fi.Length.ToString() + " Bytes";
                }
                catch (Exception)
                {
                    lblCurrent.Text = "-no info-";
                }
            }
            else
            {
                lblCurrent.Text = "-no info-";
            }
        }

        private void bnOptionsCreateBackup_Click(object sender, EventArgs e)
        {
            String filename = "hosts"+DateTime.Now.ToString()+".bak";
            filename = filename.Replace(":","");
            lbOptionsBackup.Items.Add(filename);
            
            try
            {
                File.Copy(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\"+filename);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        private void bnOptionsRestoreBackup_Click(object sender, EventArgs e)
        {
            if (lbOptionsBackup.SelectedIndex >= 0)
            {
                FileSecurity fs = setHostsFilePermissions();                
                File.Copy(lbOptionsBackup.SelectedItem.ToString(), Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
                resetHostsFilePermissions(fs);
            }
        }
    }
}