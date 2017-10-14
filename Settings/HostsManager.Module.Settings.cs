using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HostsManager.Settings
{
    public class Settings
    {
        frmHostsManager frmHosts;
        public Settings(frmHostsManager _hosts)
        {
            frmHosts = _hosts;
        }

        //Load setings from registry and XML
        public void loadSettings()
        {
            Microsoft.Win32.RegistryKey mexampleRegistryKey =
             Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software");
            mexampleRegistryKey = mexampleRegistryKey.OpenSubKey("LV-Crew");
            mexampleRegistryKey = mexampleRegistryKey.OpenSubKey("HostsManager");
            if (mexampleRegistryKey != null)
            {
                //depracted
                SettingsData.hostsURL = (String)mexampleRegistryKey.GetValue("URL");
                if (SettingsData.hostsURL == null)
                    SettingsData.hostsURL = "";
                //IP overwrite settings
                SettingsData.ipFrom = (String)mexampleRegistryKey.GetValue("ipFrom");
                if (SettingsData.ipFrom == null)
                    SettingsData.ipFrom = Branding.DefaultBlankHost;
                SettingsData.ipTo = (String)mexampleRegistryKey.GetValue("ipTo");
                if (SettingsData.ipTo == null)
                    SettingsData.ipTo = "";

                //Use internal editor?
                String b = (String)mexampleRegistryKey.GetValue("UseInternalEditor");
                if (b == "INTERNAL")
                    SettingsData.internalEditor = "INTERNAL";
                else if (b == "WORDPAD")
                    SettingsData.internalEditor = "WORDPAD";
                else
                    SettingsData.internalEditor = "CUSTOM";
                if (SettingsData.internalEditor == null)
                    SettingsData.internalEditor = "INTERNAL";

                b = (String)mexampleRegistryKey.GetValue("ExternalEditorFile");
                SettingsData.externalEditorFile = b;
                if (SettingsData.externalEditorFile == null)
                    SettingsData.externalEditorFile = "";
                b = (String)mexampleRegistryKey.GetValue("DNSServiceDisabled");
                if (b == null) b = "FALSE";
                SettingsData.DNServiceDisabled = b.Equals("TRUE") ? true : false;
                if (SettingsData.DNServiceDisabled)
                    frmHosts.bnDisableDNS.Text = "Disable DNS Service";
                b = (String)mexampleRegistryKey.GetValue("DNSGoogleChanged");
                if (b == null) b = "FALSE";
                SettingsData.DNSGoogleChanged = b.Equals("TRUE") ? true : false;
                if (SettingsData.DNSGoogleChanged)
                    frmHosts.bnSetDNSServerGoogle.Text = "Reset DNS Server";
                b = (String)mexampleRegistryKey.GetValue("DNSOpenDNSChanged");
                if (b == null) b = "FALSE";
                SettingsData.DNSOpenDNSChanged = b.Equals("TRUE") ? true : false;
                if (SettingsData.DNSOpenDNSChanged)
                    frmHosts.bnSetDNSOpenDNS.Text = "Reset DNS Server";
                b = (String)mexampleRegistryKey.GetValue("redirectType");
                if (b == null)
                {
                    b = "SET_WHITEPAGE";
                    SettingsData.replaceMethod = SettingsData.mIPReplaceMethod.SET_WHITEPAGE;
                }
                else if (b == "KEEP_LOCALHOST")
                {
                    SettingsData.replaceMethod = SettingsData.mIPReplaceMethod.KEEP_LOCALHOST;
                }
                else if (b == "SET_CUSTOM")
                {
                    SettingsData.replaceMethod = SettingsData.mIPReplaceMethod.SET_CUSTOM;
                    SettingsData.replaceIP = (String)mexampleRegistryKey.GetValue("replaceIP");
                }
                b = (String)mexampleRegistryKey.GetValue("UseHostsFileBlacklist ");
                if (b == null) b = "FALSE";
                SettingsData.UseHostsFileBlacklist = b.Equals("TRUE") ? true : false;
                b = (String)mexampleRegistryKey.GetValue("UseCustomBlacklist ");
                if (b == null) b = "FALSE";
                SettingsData.UseCustomBlacklist = b.Equals("TRUE") ? true : false;
                b = (String)mexampleRegistryKey.GetValue("UseStevensBlacklist ");
                if (b == null) b = "FALSE";
                SettingsData.UseStevensBlacklist = b.Equals("TRUE") ? true : false;
                if (!SettingsData.UseStevensBlacklist && !SettingsData.UseHostsFileBlacklist && !SettingsData.UseCustomBlacklist)
                    SettingsData.UseHostsFileBlacklist = true;
                //Auto Update?
                b = (String)mexampleRegistryKey.GetValue("AutoUpdate");
                if (b == null)
                    b = "FALSE";
                if (b == "TRUE")
                    SettingsData.autoUpdate = true;
                else
                    SettingsData.autoUpdate = false;
                b = (String)mexampleRegistryKey.GetValue("ShowFakeNews");
                if (b == null)
                    b = "FALSE";
                if (b == "TRUE")
                    SettingsData.showFakeNews = true;
                else
                    SettingsData.showFakeNews = false;
                b = (String)mexampleRegistryKey.GetValue("ShowSocial");
                if (b == null)
                    b = "FALSE";
                if (b == "TRUE")
                    SettingsData.showSocial = true;
                else
                    SettingsData.showSocial = false;
                b = (String)mexampleRegistryKey.GetValue("ShowGambling");
                if (b == null)
                    b = "FALSE";
                if (b == "TRUE")
                    SettingsData.showGambling = true;
                else
                    SettingsData.showGambling = false;
                b = (String)mexampleRegistryKey.GetValue("ShowPorn");
                if (b == null)
                    b = "FALSE";
                if (b == "TRUE")
                    SettingsData.showPorn = true;
                else
                    SettingsData.showPorn = false;
                b = (String)mexampleRegistryKey.GetValue("BlacklistToUse");
                if (b == null)
                    b = "STEVENBLACK";
                if (b == "INTERNAL")
                    SettingsData.blacklistToUse = SettingsData.BlacklistTypes.INTERNAL;
                else if (b == "STEVENBLACK")
                    SettingsData.blacklistToUse = SettingsData.BlacklistTypes.STEVENBLACK;
                else
                    SettingsData.blacklistToUse = SettingsData.BlacklistTypes.HOSTSFILEDOTNET;

            }

            //Read URLs from settings.xml
            try
            {
                if (File.Exists("settings.xml"))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ArrayList));
                    StreamReader reader = new StreamReader("settings.xml");
                    SettingsData.urls = (ArrayList)serializer.Deserialize(reader);
                    reader.Close();
                }
            }
            catch (Exception) { }

            //Read aditional Hosts from blacklist.xml
            try
            {
                if (File.Exists("blacklist.xml"))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ArrayList));
                    StreamReader reader = new StreamReader("blacklist.xml");
                    SettingsData.addHosts = (ArrayList)serializer.Deserialize(reader);
                    reader.Close();
                }
            }
            catch (Exception) { }
        }

        //Save Settings to Registry and XML
        public void saveSettings()
        {
            Microsoft.Win32.RegistryKey exampleRegistryKey =
             Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE");
            exampleRegistryKey = exampleRegistryKey.CreateSubKey("LV-Crew");
            exampleRegistryKey = exampleRegistryKey.CreateSubKey("HostsManager");
            //depracted
            exampleRegistryKey.SetValue("URL", SettingsData.hostsURL);
            //IP Overwrite settings
            exampleRegistryKey.SetValue("ipFrom", SettingsData.ipFrom);
            exampleRegistryKey.SetValue("ipTo", SettingsData.ipTo);
            //Use internal editor?
            if (SettingsData.internalEditor == "INTERNAL")
                exampleRegistryKey.SetValue("UseInternalEditor", "INTERNAL");
            else if (SettingsData.internalEditor == "WORDPAD")
                exampleRegistryKey.SetValue("UseInternalEditor", "WORDPAD");
            else
                exampleRegistryKey.SetValue("UseInternalEditor", "CUSTOM");
            exampleRegistryKey.SetValue("ExternalEditorFile", SettingsData.externalEditorFile);
            //AutoUpdate?
            if (SettingsData.autoUpdate)
                exampleRegistryKey.SetValue("AutoUpdate", "TRUE");
            else
                exampleRegistryKey.SetValue("AutoUpdate", "FALSE");
            if (SettingsData.blacklistToUse == SettingsData.BlacklistTypes.STEVENBLACK)
                exampleRegistryKey.SetValue("BlacklistToUse", "STEVENBLACK");
            else if (SettingsData.blacklistToUse == SettingsData.BlacklistTypes.INTERNAL)
                exampleRegistryKey.SetValue("BlacklistToUse", "INTERNAL");
            else
                exampleRegistryKey.SetValue("BlacklistToUse", "HOSTSFILENET");
            exampleRegistryKey.SetValue("DNSServiceDisabled", SettingsData.DNServiceDisabled ? "TRUE" : "FALSE");
            exampleRegistryKey.SetValue("DNSGoogleChanged", SettingsData.DNSGoogleChanged ? "TRUE" : "FALSE");
            exampleRegistryKey.SetValue("DNSOpenDNSChanged", SettingsData.DNSOpenDNSChanged ? "TRUE" : "FALSE");
            exampleRegistryKey.SetValue("OldDNS", SettingsData.oldDNS);
            if (SettingsData.replaceMethod == SettingsData.mIPReplaceMethod.KEEP_LOCALHOST)
            {
                exampleRegistryKey.SetValue("redirectType", "KEEP_LOCALHOST");
            }
            else if (SettingsData.replaceMethod == SettingsData.mIPReplaceMethod.SET_CUSTOM)
            {
                exampleRegistryKey.SetValue("redirectType", "SET_CUSTOM");
                exampleRegistryKey.SetValue("replaceIP", SettingsData.replaceIP);
            }
            else
            {
                exampleRegistryKey.SetValue("redirectType", "SET_WHITEPAGE");
            }
            if (SettingsData.UseHostsFileBlacklist)
                exampleRegistryKey.SetValue("UseHostsFileBlacklist", "TRUE");
            else
                exampleRegistryKey.SetValue("UseHostsFileBlacklist", "FALSE");
            if (SettingsData.UseCustomBlacklist)
                exampleRegistryKey.SetValue("UseCustomBlacklist ", "TRUE");
            else
                exampleRegistryKey.SetValue("UseCustomBlacklist ", "FALSE");
            if (SettingsData.UseStevensBlacklist)
                exampleRegistryKey.SetValue("UseStevensBlacklist ", "TRUE");
            else
                exampleRegistryKey.SetValue("UseStevensBlacklist ", "FALSE");
            exampleRegistryKey.Close();
            //Write ULRs to settings.xml
            var serializer = new XmlSerializer(typeof(ArrayList), new Type[] {
    typeof(String)
   });
            try
            {
                XmlWriter w = XmlWriter.Create("Settings.xml");
                serializer.Serialize(w, SettingsData.urls);
                w.Close();
            }
            catch (Exception)
            {
                if (!SettingsData.isHidden) MessageBox.Show("Could not save settings.");
            }

            //Write additional Hosts to blacklist.xml
            var serializer1 = new XmlSerializer(typeof(ArrayList), new Type[] {
    typeof(String)
   });
            try
            {
                XmlWriter w = XmlWriter.Create("Blacklist.xml");
                serializer1.Serialize(w, SettingsData.addHosts);
                w.Close();
            }
            catch (Exception)
            {
                if (!SettingsData.isHidden) MessageBox.Show("Could not save settings.");
            }
        }

        public void loadOptions()
        {
            if (SettingsData.replaceMethod == SettingsData.mIPReplaceMethod.KEEP_LOCALHOST)
            {
                frmHosts.rbRedirectLocalhost.Checked = true;
            }
            else if (SettingsData.replaceMethod == SettingsData.mIPReplaceMethod.SET_CUSTOM)
            {
                frmHosts.rbRedirectCustom.Checked = true;
                frmHosts.txtReplaceIP.Text = SettingsData.replaceIP;
            }
            else
            {
                frmHosts.rbRedirectWhitepage.Checked = true;
            }
            frmHosts.lbAddHosts.Items.Clear();
            frmHosts.lbURLs.Items.Clear();
            //FIll URL list
            foreach (String u in SettingsData.urls)
            {
                frmHosts.lbURLs.Items.Add(u);
            }
            foreach (String h in SettingsData.addHosts)
            {
                frmHosts.lbAddHosts.Items.Add(h);
            }

            //Fancy form fill effects
            frmHosts.txtURL.GotFocus += (s, a) => {
                if (frmHosts.txtURL.ForeColor == Color.Gray) frmHosts.txtURL.Text = "";
            };

            frmHosts.txtTo.LostFocus += (s, a) => {
                if (frmHosts.txtTo.Text == "")
                {
                    frmHosts.txtTo.Text = "34.213.32.36";
                    frmHosts.txtTo.ForeColor = Color.Gray;
                }
            };

            //Use internal editor?
            if (SettingsData.internalEditor == "" || SettingsData.internalEditor == null)
                SettingsData.internalEditor = "INTERNAL";
            if (SettingsData.internalEditor == "INTERNAL")
            {
                frmHosts.rbInternal.Checked = true;
                frmHosts.rbExternal.Checked = false;
                frmHosts.rbCustom.Checked = false;
            }
            else if (SettingsData.internalEditor == "WORDPAD")
            {
                frmHosts.rbExternal.Checked = true;
                frmHosts.rbInternal.Checked = false;
                frmHosts.rbCustom.Checked = false;
            }
            else if (SettingsData.internalEditor == "CUSTOM")
            {
                frmHosts.rbCustom.Checked = true;
                frmHosts.rbInternal.Checked = false;
                frmHosts.rbExternal.Checked = false;
            }
            if (SettingsData.externalEditorFile != "")
                frmHosts.txtCustomEditor.Text = SettingsData.externalEditorFile;

            //Auto Update?
            if (SettingsData.autoUpdate)
            {
                frmHosts.cbAutoUpdate.Checked = true;
            }
            else
            {
                frmHosts.cbAutoUpdate.Checked = false;
            }
            frmHosts.rbUseHostsFileBL.Checked = SettingsData.UseHostsFileBlacklist;
            frmHosts.rbUseCustomBL.Checked = SettingsData.UseCustomBlacklist;
            frmHosts.rbUseStevenBlacksBL.Checked = SettingsData.UseStevensBlacklist;

            String[] files = getBackups();
            foreach (String f in files)
            {
                if (f != null)
                    if (f != "")
                        frmHosts.lbOptionsBackup.Items.Add(f);
            }
        }

        private String[] getBackups()
        {
            String[] f = Directory.GetFiles((Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\"));
            String[] r = new String[f.Length];
            int count = 0;
            for (int i = 0; i < f.Length; i++)
            {

                if (f[i] != null)
                {
                    if (f[i].Contains(".bak"))
                    {
                        r[count] = f[i];
                        count++;
                    }
                }
            }
            return r;
        }

        public  void saveOptions()
        {
            if (frmHosts.cbAutoUpdate.Checked)
                SettingsData.autoUpdate = true;
            else
                SettingsData.autoUpdate = false;
            if (frmHosts.rbInternal.Checked)
                SettingsData.internalEditor = "INTERNAL";
            else if (frmHosts.rbExternal.Checked)
                SettingsData.internalEditor = "WORDPAD";
            else
                SettingsData.internalEditor = "CUSTOM";
            frmHosts.txtCustomEditor.Text = SettingsData.externalEditorFile;
            if ((frmHosts.txtFrom.ForeColor == Color.Gray && frmHosts.txtTo.ForeColor == Color.Black) ||
             (frmHosts.txtFrom.ForeColor == Color.Black && frmHosts.txtTo.ForeColor == Color.Gray))
                MessageBox.Show("Please enter both \"From\" and \"To\" IP");
            else
            {
                if (frmHosts.txtFrom.Text != "")
                    SettingsData.ipFrom = frmHosts.txtFrom.Text;
                if (frmHosts.txtTo.Text != "")
                    SettingsData.ipTo = frmHosts.txtTo.Text;
                SettingsData.urls.Clear();
                foreach (String item in frmHosts.lbURLs.Items)
                {
                    SettingsData.urls.Add(item);
                }
                foreach (String host in frmHosts.lbAddHosts.Items)
                {
                    SettingsData.addHosts.Add(host);
                }
            }
        }
    }
}
