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
using Un4seen.Bass;


namespace HostsManager
{
    //-----------------Form Methods-------------------
    public partial class frmCore : Form, IMessageFilter
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
        
        public frmCore()
        {
            InitializeComponent();
            clsBrandingINI.readINI();
            String wpHostName= clsBrandingData.DefaultBlockPage;
            clsSettingsData.ipTo = clsUtilitys.Resolver(wpHostName)[0];

            if (clsUtilitys.Services.isServiceActive())
                bnDisableDNS.Text = "Disable DNS-Client Service";
            else
                bnDisableDNS.Text = "Enable DNS-Client Service";


            if (File.Exists(clsBrandingData.BannerImage))
            {
                pbPicture.Image = Image.FromFile(clsBrandingData.BannerImage);
                pictureBox3.Image = Image.FromFile(clsBrandingData.BannerImage);
            }

            
            (new Settings.clsSettingsFunctions(this)).loadSettings();
            //Check whether to run silently
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                if (arguments[1] == "/auto")
                {
                    this.Opacity = 0;
                    this.ShowInTaskbar = false;
                    this.WindowState = FormWindowState.Minimized;
                    clsSettingsData.isHidden = true;
                }
            }            
        }


        
        //Load main form
        private void frmHostsManager_Load(object sender, EventArgs e)
        {
            txtCustomEditor.Text = Environment.GetEnvironmentVariable("windir") + "\\system32\\notepad.exe";
            try
            {
                if (!clsSettingsData.isHidden)
                {
                    if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                    {
                        int i = Bass.BASS_MusicLoad(clsBrandingData.BackgroundSound, 0, 0, BASSFlag.BASS_DEFAULT, 0);
                        Bass.BASS_ChannelPlay(i, true);
                    }
                }
            }
            catch (Exception ex) {
                int a = 0;
            }

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
            clsUtilitys.SSLCertificates.ReadFirefoxProfile();
            bnUpdate.Select();
            checkForSilentRun();
            loadListsToDownload();
            try
            {
                this.Icon = new Icon(clsBrandingData.ICONPATH);
            }
            catch (Exception) { }
            Settings.clsSettingsFunctions s=new Settings.clsSettingsFunctions(this);
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
            if (clsSettingsData.DNSGoogleChanged)
            {
                /*
                clsUtilitys.Dialogs.dlgOptions o = new clsUtilitys.Dialogs.dlgOptions();
                o.frm = (frmCore)this;
                o.txt = "Resetting DNS Server...";
                start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                start.Start(o);
                */
                clsCoreFunctions.setDNS(clsSettingsData.oldDNS);
                clsSettingsData.DNSGoogleChanged = false;
                bnSetDNSOpenDNS.Text = "Set DNS Server to OpenDNS";
                bnSetDNSServerGoogle.Text = "Set DNS Server to Google";

                clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                o1.frm = (frmCore)this;
                o1.txt = "DNS server has been reset.";
                o1.okbutton = true;
                start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                start.Start(o1);               
            }
            else
            {
                String _oldDNS = clsUtilitys.Info.getCurrentDNSServer();
                clsCoreFunctions.setDNS("8.8.8.8");
                bnSetDNSServerGoogle.Text = "Reset DNS Server";
                bnSetDNSOpenDNS.Text = "Set DNS Server to OpenDNS";
                clsSettingsData.DNSGoogleChanged = true;
                clsSettingsData.DNSOpenDNSChanged = false;
                clsSettingsData.oldDNS = _oldDNS;
                if (start != null)
                {
                    clsUtilitys.Dialogs.closeDialog();
                    start.Abort();
                }
                clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                o1.frm = (frmCore)this;
                o1.txt = "DNS server has been changed.";
                o1.okbutton = true;
                start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                start.Start(o1);                
            }
            (new Settings.clsSettingsFunctions(this)).saveSettings();
        }

        private void bnSetDNSOpenDNS_Click(object sender, EventArgs e)
        {
            Thread start = null;
            if (clsSettingsData.DNSOpenDNSChanged)
            {
                clsCoreFunctions.setDNS(clsSettingsData.oldDNS);
                clsSettingsData.DNSOpenDNSChanged = false;
                bnSetDNSOpenDNS.Text = "Set DNS Server to OpenDNS";
                bnSetDNSServerGoogle.Text = "Set DNS Server to Google";
                if (start != null)
                {
                    clsUtilitys.Dialogs.closeDialog();
                    start.Abort();
                }
                clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                o1.frm = (frmCore)this;
                o1.txt = "DNS server has been reset.";
                o1.okbutton = true;
                start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                start.Start(o1);
            }
            else
            {
                String _oldDNS = clsUtilitys.Info.getCurrentDNSServer();
                clsCoreFunctions.setDNS("208.67.222.222,208.67.220.220");
                bnSetDNSOpenDNS.Text = "Reset DNS Server";
                bnSetDNSServerGoogle.Text = "Set DNS Server to Google";
                clsSettingsData.DNSOpenDNSChanged = true;
                clsSettingsData.DNSGoogleChanged = false;
                clsSettingsData.oldDNS = _oldDNS;
                if (start != null)
                {
                    clsUtilitys.Dialogs.closeDialog();
                    start.Abort();
                }
                clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                o1.frm = (frmCore)this;
                o1.txt = "DNS server has been changed.";
                o1.okbutton = true;
                start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                start.Start(o1);
            }
            (new Settings.clsSettingsFunctions(this)).saveSettings();
            if (start != null)
            {
                clsUtilitys.Dialogs.closeDialog();
                start.Abort();
            }
        }

        private void bnHelpTools_Click(object sender, EventArgs e)
        {
            frmDialog f = new frmDialog();
            f.action =
             "Edit Hosts File: Edit the hosts file.\r\n" +
             "Remove Duplicates: Removes duplicate entrys from the hosts file.\r\n" +
             "Reset Hosts File: Set hostsfile to Windows standard.\r\n" +
             "Flush DNS Cache: removes temporarily saved DNS data.\r\n" +
             "Disable DNS Service: Do this if your System slows down while surfing.\r\n" +
             "Set DNS Server to Google: Set your DNS Server IP to that of Google (8.8.8.8).\r\n" +
             "Set DNS Server to OpenDNS: Set your DNS Server IP to that of OpenDNS.\r\n"+
             "Set DNS Server to Custom Value: Allows you to enter the IP of the DNS Server.";
            f.showButton = true;
            f.customHeight = 200;
            f.customWidth = 400;
            f.ShowDialog();
        }

        private void disableDNSThread()
        {
            Thread start = null;
            bool err = false;
            if (!clsUtilitys.Services.isServiceActive())
            {
                clsUtilitys.Dialogs.dlgOptions o = new clsUtilitys.Dialogs.dlgOptions();
                o.frm = (frmCore)this;
                o.txt = "Enabling DNS-Client Service...";
                start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                start.Start(o);
                Process p = clsUtilitys.executeNoWindow("sc.exe", "config Dnscache start=auto");
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
                if (start != null)
                {
                    clsUtilitys.Dialogs.closeDialog();
                    start.Abort();
                }
                if (!err)
                {
                    clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                    o1.frm = (frmCore)this;
                    o1.txt = "DNS-Client service enabled.";
                    o1.okbutton = true;
                    start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                    start.Start(o1);
                    bnDisableDNS.Text = "Disable DNS-Client Service";
                    clsSettingsData.DNServiceDisabled = false;
                }
                else
                {
                    clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                    o1.frm = (frmCore)this;
                    o1.txt = "Error enabling DNS-Client service. Please retry...";
                    o1.okbutton = true;
                    start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                    start.Start(o1);
                }
            }
            else
            {
                if (!clsUtilitys.isADMember())
                    clsCoreFunctions.disableDNSService(this);
                else
                {
                    clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                    o1.frm = (frmCore)this;
                    o1.txt = "You are a member of an Active Directory domain. This requires the DNS-Client service to be active.";
                    o1.okbutton = true;
                    start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                    start.Start(o1);
                }
            }
        }

        private void bnDisableDNS_Click(object sender, EventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(disableDNSThread);
            t.Start();
        }

        private void bnFlushDNSCache_Click(object sender, EventArgs e)
        {
            try
            {
                Process p = clsUtilitys.executeNoWindow("ipconfig.exe", "/flushdns");
                p.Start();
                p.WaitForExit();
                clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                o1.frm = (frmCore)this;
                o1.txt = "DNS Cache has been flushed.";
                o1.okbutton = true;
                Thread start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                start.Start(o1);

            }
            catch (Exception)
            {
            }
        }

        private void bnResetHostsFile_Click(object sender, EventArgs e)
        {
            try
            {
                FileSecurity fs = clsUtilitys.HostsFile.setHostsFilePermissions();
                String ret = System.IO.File.ReadAllText("default_hosts.tpl");
                System.IO.File.WriteAllText(
                 Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", ret);
                clsUtilitys.HostsFile.resetHostsFilePermissions(fs);
                clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                o1.frm = (frmCore)this;
                o1.txt = "Hosts file has been reset to system defaults.";
                o1.okbutton = true;
                Thread start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                start.Start(o1);
            }
            catch (Exception)
            {
                clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                o1.frm = (frmCore)this;
                o1.txt = "Could not write hosts file. Please check your antivirus for hosts file potection.";
                o1.okbutton = true;
                Thread start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                start.Start(o1);
            }
        }

        private void rbUseHostsFileBL_CheckedChanged(object sender, EventArgs e)
        {
            clsSettingsData.UseHostsFileBlacklist = ((CheckBox)sender).Checked;
            (new Settings.clsSettingsFunctions(this)).saveSettings();
        }

        private void rbUseStevenBlacksBL_CheckedChanged(object sender, EventArgs e)
        {
            clsSettingsData.UseStevensBlacklist = ((CheckBox)sender).Checked;
            (new Settings.clsSettingsFunctions(this)).saveSettings();
        }

        private void rbUseCustomBL_CheckedChanged(object sender, EventArgs e)
        {
            clsSettingsData.UseCustomBlacklist = ((CheckBox)sender).Checked;
            (new Settings.clsSettingsFunctions(this)).saveSettings();
        }

        private void cbAutoUpdate_Click(object sender, EventArgs e)
        {
            clsSettingsData.autoUpdate = cbAutoUpdate.Checked;
            clsCoreFunctions.doAutoUpdate();
            (new Settings.clsSettingsFunctions(this)).saveSettings();
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
            clsSettingsData.showFakeNews = cbFakeNews.Checked;
        }

        private void cbPorn_Click(object sender, EventArgs e)
        {
            saveListsToDownload();
            Microsoft.Win32.RegistryKey exampleRegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(clsBrandingData.COMPANY);
            exampleRegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(clsBrandingData.PRODUCT);
            if (clsSettingsData.showGambling)
                exampleRegistryKey.SetValue("ShowGambling", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowGambling", "FALSE");
            if (clsSettingsData.showFakeNews)
                exampleRegistryKey.SetValue("ShowFakeNews", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowFakeNews", "FALSE");
            if (clsSettingsData.showPorn)
                exampleRegistryKey.SetValue("ShowPorn", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowPorn", "FALSE");
            if (clsSettingsData.showSocial)
                exampleRegistryKey.SetValue("ShowSocial", "TRUE");
            else
                exampleRegistryKey.SetValue("ShowSocial", "FALSE");
            exampleRegistryKey.Close();
        }

        private void cbBackgroundMusic_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBackgroundMusic.Checked)
            {
                if (clsSettingsData.player != null)
                    clsSettingsData.player.Play();
            }
            else
            {
                if (clsSettingsData.player != null)
                    clsSettingsData.player.Stop();
            }
        }

        private void rbUseStevensBlacklist_CheckedChanged(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey exampleRegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(clsBrandingData.COMPANY);
            exampleRegistryKey=Microsoft.Win32.Registry.CurrentUser.CreateSubKey(clsBrandingData.PRODUCT);
            if (rbUseStevenBlacksBL.Checked)
            {
                clsSettingsData.UseStevensBlacklist = true;
                exampleRegistryKey.SetValue("UseStevenBlack", "TRUE");
            }
            else
            {
                clsSettingsData.UseStevensBlacklist = false;
                exampleRegistryKey.SetValue("UseStevenBlack", "FALSE");
            }
            if (rbUseCustomBL.Checked)
            {
                clsSettingsData.UseCustomBlacklist = true;
                exampleRegistryKey.SetValue("UseCustom", "TRUE");
            }
            else
            {
                clsSettingsData.UseCustomBlacklist = true;
                exampleRegistryKey.SetValue("UseCustom", "TRUE");
            }
            if (rbUseHostsFileBL.Checked)
            {
                clsSettingsData.UseHostsFileBlacklist = true;
                exampleRegistryKey.SetValue("UseHostsFie", "TRUE");
            }
        }

        private void bnAddHost_Click(object sender, EventArgs e)
        {
            if (txtAddHost.Text != "" && Uri.CheckHostName(txtAddHost.Text)==UriHostNameType.Dns)
                lbAddHosts.Items.Add(txtAddHost.Text);
            else
            {

            }
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
            {
                clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                o1.frm = (frmCore)this;
                o1.txt = "Wrong format. URL must begin with http://";
                o1.okbutton = true;
                Thread start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                start.Start(o1);
            }
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
                clsSettingsFunctions s = new clsSettingsFunctions(this);
                s.saveSettingsOptions();
                s.saveSettings();
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
                clsSettingsData.replaceMethod = clsSettingsData.mIPReplaceMethod.KEEP_LOCALHOST;
            else if ((rbRedirectWhitepage).Checked)
                clsSettingsData.replaceMethod = clsSettingsData.mIPReplaceMethod.SET_WHITEPAGE;
            else
            {
                Match match = Regex.Match(txtReplaceIP.Text, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
                if (match.Success)
                {
                    clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                    o1.frm = (frmCore)this;
                    o1.txt = "Invalid IP. Please check format.";
                    o1.okbutton = true;
                    Thread start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                    start.Start(o1);
                }
                else
                {
                    clsSettingsData.replaceMethod = clsSettingsData.mIPReplaceMethod.SET_CUSTOM;
                    clsSettingsData.replaceIP = txtReplaceIP.Text;
                }
            }
            if ((rbCustom.Checked && File.Exists(txtCustomEditor.Text)) || rbExternal.Checked || rbInternal.Checked)
            {
                clsSettingsFunctions s = new clsSettingsFunctions(this);
                s.saveSettingsOptions();
                (new Settings.clsSettingsFunctions(this)).saveSettings();
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
            lbOptionsBackup.Items.Clear();
            bnDisableDNS.Text =clsUtilitys.Services.isServiceActive() ? "Disable DNS-Client service" : "Enable DNS-Client service";
            Settings.clsSettingsFunctions s = new Settings.clsSettingsFunctions(this);
            s.loadOptions();
            tabCtrlPages.SelectedIndex = 2;
            resetButtons();
            ((Button)sender).BackColor = Color.Navy;           
            lblPage.Text = "Options";
        }

        private void bnMenuAbout_Click(object sender, EventArgs e)
        {
            lblVersion.Text = "Version: " + clsBrandingData.VERSION;
            lblName.Text = clsBrandingData.COMPANY + " " + clsBrandingData.PRODUCT;
            try
            {
                pictureBox2.ImageLocation = clsBrandingData.PRODUCTIMGPATH;
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

        public void initialBackups()
        {
            if (clsCoreFunctions.getBackups()!=null)
            {
                String filename = "hosts" + DateTime.Now.ToString() + "." + clsBrandingData.PRODUCT + ".bak";
                filename = filename.Replace(":", "");
                lbOptionsBackup.Items.Add(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\" + filename);
                
                try
                {
                    File.Copy(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\" + filename);
                }
                catch (Exception) { }
            }
        }

        private void bnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                initialBackups();
                if (clsUtilitys.isADMember())
                {
                    if (clsSettingsData.UseHostsFileBlacklist)
                    {
                        clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                        o1.frm = (frmCore)this;
                        o1.txt = "Please use only Steven Black's Blacklist when being a member of an Active Directory domain. Contact your system administrator for further information.";
                        o1.okbutton = true;
                        Thread start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                        start.Start(o1);                        
                    }
                }
                else
                {
                    DialogResult ret = DialogResult.Cancel;
                    if (clsUtilitys.Services.isServiceActive())
                        ret = clsUtilitys.Dialogs.showYesNoDialog("Do you want to disable the DNS-Client service? Not doing so will slow down your computer.");
                    if (ret == DialogResult.OK)
                        clsCoreFunctions.disableDNSService(this);
                    System.Threading.Thread t = new System.Threading.Thread(new ParameterizedThreadStart(clsCoreFunctions.updateHostsFile));
                    t.Start(this);
                }
                updateStats();
            }
            catch (Exception) { }
        }

        private void HostsFileThrd(object f)
        {
            FileSecurity fs = clsUtilitys.HostsFile.setHostsFilePermissions();
            clsEditHosts.edit(clsSettingsData.internalEditor, clsSettingsData.urls, clsSettingsData.externalEditorFile,(frmCore)f);
            clsUtilitys.HostsFile.resetHostsFilePermissions(fs);
       
        }

        private void bnEdit_Click(object sender, EventArgs e)
        {
            if (File.Exists(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts"))
            {
                System.Threading.Thread t = new System.Threading.Thread(new ParameterizedThreadStart(HostsFileThrd));
                t.Start(this);
            }
        }

        //-----------------Generic Methods-------------------
        
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

        private void checkForSilentRun()
        {
            //Check whether to run silently
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                if (arguments[1] == "/auto")
                {
                    //Set hosts file permissions to writable for group admins
                    FileSecurity fs = clsUtilitys.HostsFile.setHostsFilePermissions();
                    //Update hostsfile
                    clsCoreFunctions.updateHostsFile(this);
                    //Reset file permissions
                    clsUtilitys.HostsFile.resetHostsFilePermissions(fs);
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
                if (clsUtilitys.Info.isAntivir())
                    if (!clsSettingsData.isHidden)
                    {
                        frmNotifyAntivirus f = new frmNotifyAntivirus();
                        f.ShowDialog();
                    }

                //Branding
                this.Text = clsBrandingData.COMPANY + " " + clsBrandingData.PRODUCT + " v" + clsBrandingData.VERSION;
                try
                {
                    pbPicture.ImageLocation = clsBrandingData.PRODUCTIMGPATH;
                }
                catch (Exception) { }
            }
        }

        public void setServiceButtonText(String text)
        {
            bnDisableDNS.Text = text;
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
            clsSettingsData.showFakeNews = cbFakeNews.Checked;
            clsSettingsData.showGambling = cbGambling.Checked;
            clsSettingsData.showPorn = cbPorn.Checked;
            clsSettingsData.showSocial = cbSocial.Checked;
        }

        private void loadListsToDownload()
        {
            cbFakeNews.Checked = clsSettingsData.showFakeNews;
            cbGambling.Checked = clsSettingsData.showGambling;
            cbPorn.Checked = clsSettingsData.showPorn;
            cbSocial.Checked = clsSettingsData.showSocial;
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
                        lblCurrent.Text = hf.Length.ToString() + " Entrys; " + sz + " KB";
                    }
                    else
                        lblCurrent.Text = hf.Length.ToString() + " Entrys; " + fi.Length.ToString() + " Bytes";
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
            String filename = "hosts"+DateTime.Now.ToString()+"."+clsBrandingData.PRODUCT+".bak";
            filename = filename.Replace(":","");
            lbOptionsBackup.Items.Add(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\"+filename);


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
                FileSecurity fs = clsUtilitys.HostsFile.setHostsFilePermissions();                
                File.Copy(lbOptionsBackup.SelectedItem.ToString(), Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
                clsUtilitys.HostsFile.resetHostsFilePermissions(fs);
            }
        }

        private void bnSetOwnDNSServer(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("DNS Server", "Please enter the IP Adress of the DNS Server", " ", 0, 0);
            if (input.Trim().Length > 0)
            {
                Match match = Regex.Match(input, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
                if (match.Success)
                {
                    clsCoreFunctions.setDNS(input);
                    clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                    o1.frm = (frmCore)this;
                    o1.txt = "DNS server has been set.";
                    o1.okbutton = true;
                    Thread start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                    start.Start(o1);
                }
                else
                {
                    clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
                    o1.frm = (frmCore)this;
                    o1.txt = "Invalid IP. Please check format.";
                    o1.okbutton = true;
                    Thread start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
                    start.Start(o1);
                }
            }
        }

        private void tabOptionsBackup_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmDialog f = new frmDialog();
            f.action =
             "This window allows you to create- and restore backups of the hosts file.\r\n"+
             "Backups will be saved under windir\\system32\\drivers\\etc.";
            f.showButton = true;
            f.customHeight = 120;
            f.customWidth = 400;
            f.ShowDialog();
        }

        private void tabOptions_Click(object sender, EventArgs e)
        {

        }
        public delegate void tmp(frmCore f);

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                tabCtrlPages.SelectedIndex = 3;
                resetButtons();
                bnHelpMain.BackColor = Color.Navy;
                lblPage.Text = "Help";
                string curDir = Directory.GetCurrentDirectory();
                try
                {
                    string html = File.ReadAllText(curDir + "\\readme.html");
                    wbWebbrowserHelp.DocumentText = html;
                }
                catch (Exception) { }
                return true;    // indicate that you handled this keystroke
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void doRemoveDuplicates(object th)
        {
            clsUtilitys.Dialogs.dlgOptions o = new clsUtilitys.Dialogs.dlgOptions();
            o.frm = (frmCore)this;
            o.txt = "Removing duplicates.";
            o.thrd =(Thread)th;
            Thread start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialogThrd));
            start.Start(o);
            long dupCount = 0;

            String[] alltext = File.ReadAllLines(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
            String[] alltext1 = alltext;

            for (int i = 0; i < alltext1.Length; i++)
            {
                for (int z = i; z < alltext1.Length; z++)
                {
                    if (i < alltext.Length - 1)
                        if (alltext[i + 1] == alltext1[z])
                        {
                            if (alltext[z].Length > 1)
                                dupCount++;
                            alltext[z] = "";
                            for (int u = z; u < alltext1.Length - 1; u++)
                            {
                                alltext1[u] = alltext1[u + 1];
                            }
                            Array.Resize(ref alltext1, alltext1.Length - 1);
                        }
                }
                System.Threading.Thread.Sleep(10);
            }
            FileSecurity fs = clsUtilitys.HostsFile.setHostsFilePermissions();
            File.WriteAllLines(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", alltext1);
            clsUtilitys.HostsFile.resetHostsFilePermissions(fs);
            if (start != null)
            {
                clsUtilitys.Dialogs.closeDialog();
                start.Abort();
            }
            clsUtilitys.Dialogs.dlgOptions o1 = new clsUtilitys.Dialogs.dlgOptions();
            o1.frm = (frmCore)this;
            o1.txt = "Finished removing duplicates.\n" + dupCount.ToString() + " duplicates removed.";
            o1.okbutton = true;
            start = new Thread(new ParameterizedThreadStart(clsUtilitys.Dialogs.showDialog));
            start.Start(o1);
        }
        public delegate void tmp1();
        public void removeDuplicates()
        {
            FileInfo fi = new FileInfo(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
            bool cont=true;
            if(fi.Length>2097152)
            {
                frmDialog d = new frmDialog();
                d.action = "Hosts file size is > 2MB.\nDeduplication will take up to 30 minutes.\nAre you sure that you want to continue?";
                d.yesNoButtons = true;
                if (this.InvokeRequired)
                    this.Invoke(new tmp1(removeDuplicates));
                else
                {
                    if (d.ShowDialog(this) == DialogResult.OK)
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(doRemoveDuplicates));
                        t.Start(t);
                    }
                }
            }
        }

        private void bnDupliactes_Click(object sender, EventArgs e)
        {
           removeDuplicates();            
        }

        private void tabTools_Click(object sender, EventArgs e)
        {

        }

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}