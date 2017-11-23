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
    public class clsSettingsFunctions
    {
        frmCore frmHosts;
        public clsSettingsFunctions(frmCore _hosts)
        {
            frmHosts = _hosts;
        }

        public void saveSettingsOptions()
        {
            if (frmHosts.cbAutoUpdate.Checked)
                clsSettingsData.autoUpdate = true;
            else
                clsSettingsData.autoUpdate = false;
            if (frmHosts.rbInternal.Checked)
            {
                clsSettingsData.internalEditor = "INTERNAL";
            }
            else if (frmHosts.rbExternal.Checked)
            {
                clsSettingsData.internalEditor = "WORDPAD";
            }
            else
            {
                clsSettingsData.internalEditor = "CUSTOM";
                clsSettingsData.externalEditorFile = frmHosts.txtCustomEditor.Text;
            }
            if ((frmHosts.txtFrom.ForeColor == Color.Gray && frmHosts.txtTo.ForeColor == Color.Black) ||
                (frmHosts.txtFrom.ForeColor == Color.Black && frmHosts.txtTo.ForeColor == Color.Gray))
                MessageBox.Show("Please enter both \"From\" and \"To\" IP");
            else
            {
                if (frmHosts.txtFrom.Text != "")
                    clsSettingsData.ipFrom = frmHosts.txtFrom.Text;
                if (frmHosts.txtTo.Text != "")
                    clsSettingsData.ipTo = frmHosts.txtTo.Text;
                clsSettingsData.urls.Clear();
                foreach (String item in frmHosts.lbURLs.Items)
                {
                    clsSettingsData.urls.Add(item);
                }
                clsSettingsData.addHosts.Clear();
                foreach (String host in frmHosts.lbAddHosts.Items)
                {
                    clsSettingsData.addHosts.Add(host);
                }
            }
        }

        //Load setings from registry and XML
        public void loadSettings()
        {
            try
            {
                Microsoft.Win32.RegistryKey mexampleRegistryKey =
                 Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software");
                mexampleRegistryKey = mexampleRegistryKey.OpenSubKey(clsBrandingData.COMPANY);
                mexampleRegistryKey = mexampleRegistryKey.OpenSubKey(clsBrandingData.PRODUCT);
                if (mexampleRegistryKey != null)
                {
                    //depracted
                    clsSettingsData.hostsURL = (String)mexampleRegistryKey.GetValue("URL");
                    if (clsSettingsData.hostsURL == null)
                        clsSettingsData.hostsURL = "";
                    //IP overwrite settings
                    clsSettingsData.ipFrom = (String)mexampleRegistryKey.GetValue("ipFrom");
                    if (clsSettingsData.ipFrom == null)
                        clsSettingsData.ipFrom = clsBrandingData.DefaultBlockPage;
                    clsSettingsData.ipTo = (String)mexampleRegistryKey.GetValue("ipTo");
                    if (clsSettingsData.ipTo == null)
                        clsSettingsData.ipTo = "";

                    //Use internal editor?
                    String b = (String)mexampleRegistryKey.GetValue("UseInternalEditor");
                    if (b == "INTERNAL")
                        clsSettingsData.internalEditor = "INTERNAL";
                    else if (b == "WORDPAD")
                        clsSettingsData.internalEditor = "WORDPAD";
                    else
                        clsSettingsData.internalEditor = "CUSTOM";
                    if (clsSettingsData.internalEditor == null)
                        clsSettingsData.internalEditor = "INTERNAL";

                    b = (String)mexampleRegistryKey.GetValue("ExternalEditorFile");
                    clsSettingsData.externalEditorFile = b;
                    if (clsSettingsData.externalEditorFile == null)
                        clsSettingsData.externalEditorFile = "";
                    b = (String)mexampleRegistryKey.GetValue("DNSServiceDisabled");
                    if (b == null) b = "FALSE";
                    clsSettingsData.DNServiceDisabled = b.Equals("TRUE") ? true : false;
                    if (clsSettingsData.DNServiceDisabled)
                        frmHosts.bnDisableDNS.Text = "Disable DNS Service";
                    b = (String)mexampleRegistryKey.GetValue("DNSGoogleChanged");
                    if (b == null) b = "FALSE";
                    clsSettingsData.DNSGoogleChanged = b.Equals("TRUE") ? true : false;
                    if (clsSettingsData.DNSGoogleChanged)
                        frmHosts.bnSetDNSServerGoogle.Text = "Reset DNS Server";
                    b = (String)mexampleRegistryKey.GetValue("DNSOpenDNSChanged");
                    if (b == null) b = "FALSE";
                    clsSettingsData.DNSOpenDNSChanged = b.Equals("TRUE") ? true : false;
                    if (clsSettingsData.DNSOpenDNSChanged)
                        frmHosts.bnSetDNSOpenDNS.Text = "Reset DNS Server";
                    b = (String)mexampleRegistryKey.GetValue("redirectType");
                    if (b == null)
                    {
                        b = "SET_WHITEPAGE";
                        clsSettingsData.replaceMethod = clsSettingsData.mIPReplaceMethod.SET_WHITEPAGE;
                    }
                    else if (b == "KEEP_LOCALHOST")
                    {
                        clsSettingsData.replaceMethod = clsSettingsData.mIPReplaceMethod.KEEP_LOCALHOST;
                    }
                    else if (b == "SET_CUSTOM")
                    {
                        clsSettingsData.replaceMethod = clsSettingsData.mIPReplaceMethod.SET_CUSTOM;
                        clsSettingsData.replaceIP = (String)mexampleRegistryKey.GetValue("replaceIP");
                    }
                    b = (String)mexampleRegistryKey.GetValue("UseHostsFileBlacklist");
                    if (b == null) b = "FALSE";
                    clsSettingsData.UseHostsFileBlacklist = b.Equals("TRUE") ? true : false;
                    b = (String)mexampleRegistryKey.GetValue("UseCustomBlacklist ");
                    if (b == null) b = "FALSE";
                    clsSettingsData.UseCustomBlacklist = b.Equals("TRUE") ? true : false;
                    b = (String)mexampleRegistryKey.GetValue("UseStevensBlacklist ");
                    if (b == null) b = "FALSE";
                    clsSettingsData.UseStevensBlacklist = b.Equals("TRUE") ? true : false;
                    if (!clsSettingsData.UseStevensBlacklist && !clsSettingsData.UseHostsFileBlacklist && !clsSettingsData.UseCustomBlacklist)
                        clsSettingsData.UseHostsFileBlacklist = true;
                    //Auto Update?
                    b = (String)mexampleRegistryKey.GetValue("AutoUpdate");
                    if (b == null)
                        b = "FALSE";
                    if (b == "TRUE")
                        clsSettingsData.autoUpdate = true;
                    else
                        clsSettingsData.autoUpdate = false;
                    b = (String)mexampleRegistryKey.GetValue("ShowFakeNews");
                    if (b == null)
                        b = "FALSE";
                    if (b == "TRUE")
                        clsSettingsData.showFakeNews = true;
                    else
                        clsSettingsData.showFakeNews = false;
                    b = (String)mexampleRegistryKey.GetValue("ShowSocial");
                    if (b == null)
                        b = "FALSE";
                    if (b == "TRUE")
                        clsSettingsData.showSocial = true;
                    else
                        clsSettingsData.showSocial = false;
                    b = (String)mexampleRegistryKey.GetValue("ShowGambling");
                    if (b == null)
                        b = "FALSE";
                    if (b == "TRUE")
                        clsSettingsData.showGambling = true;
                    else
                        clsSettingsData.showGambling = false;
                    b = (String)mexampleRegistryKey.GetValue("ShowPorn");
                    if (b == null)
                        b = "FALSE";
                    if (b == "TRUE")
                        clsSettingsData.showPorn = true;
                    else
                        clsSettingsData.showPorn = false;
                    b = (String)mexampleRegistryKey.GetValue("BlacklistToUse");
                    if (b == null)
                        b = "STEVENBLACK";
                    if (b == "INTERNAL")
                        clsSettingsData.blacklistToUse = clsSettingsData.BlacklistTypes.INTERNAL;
                    else if (b == "STEVENBLACK")
                        clsSettingsData.blacklistToUse = clsSettingsData.BlacklistTypes.STEVENBLACK;
                    else
                        clsSettingsData.blacklistToUse = clsSettingsData.BlacklistTypes.HOSTSFILEDOTNET;

                }
            }
            catch (Exception) { }

            //Read URLs from settings.xml
            try
            {
                if (File.Exists("settings.xml"))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ArrayList));
                    StreamReader reader = new StreamReader("settings.xml");
                    clsSettingsData.urls = (ArrayList)serializer.Deserialize(reader);
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
                    clsSettingsData.addHosts = (ArrayList)serializer.Deserialize(reader);
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
            exampleRegistryKey = exampleRegistryKey.CreateSubKey(clsBrandingData.COMPANY);
            exampleRegistryKey = exampleRegistryKey.CreateSubKey(clsBrandingData.PRODUCT);
            //depracted
            exampleRegistryKey.SetValue("URL", clsSettingsData.hostsURL);
            //IP Overwrite settings
            exampleRegistryKey.SetValue("ipFrom", clsSettingsData.ipFrom);
            exampleRegistryKey.SetValue("ipTo", clsSettingsData.ipTo);
            //Use internal editor?
            if (clsSettingsData.internalEditor == "INTERNAL")
                exampleRegistryKey.SetValue("UseInternalEditor", "INTERNAL");
            else if (clsSettingsData.internalEditor == "WORDPAD")
                exampleRegistryKey.SetValue("UseInternalEditor", "WORDPAD");
            else
                exampleRegistryKey.SetValue("UseInternalEditor", "CUSTOM");
            exampleRegistryKey.SetValue("ExternalEditorFile", clsSettingsData.externalEditorFile);
            //AutoUpdate?
            if (clsSettingsData.autoUpdate)
                exampleRegistryKey.SetValue("AutoUpdate", "TRUE");
            else
                exampleRegistryKey.SetValue("AutoUpdate", "FALSE");
            if (clsSettingsData.blacklistToUse == clsSettingsData.BlacklistTypes.STEVENBLACK)
                exampleRegistryKey.SetValue("BlacklistToUse", "STEVENBLACK");
            else if (clsSettingsData.blacklistToUse == clsSettingsData.BlacklistTypes.INTERNAL)
                exampleRegistryKey.SetValue("BlacklistToUse", "INTERNAL");
            else
                exampleRegistryKey.SetValue("BlacklistToUse", "HOSTSFILENET");
            exampleRegistryKey.SetValue("DNSServiceDisabled", clsSettingsData.DNServiceDisabled ? "TRUE" : "FALSE");
            exampleRegistryKey.SetValue("DNSGoogleChanged", clsSettingsData.DNSGoogleChanged ? "TRUE" : "FALSE");
            exampleRegistryKey.SetValue("DNSOpenDNSChanged", clsSettingsData.DNSOpenDNSChanged ? "TRUE" : "FALSE");
            exampleRegistryKey.SetValue("OldDNS", clsSettingsData.oldDNS);
            if (clsSettingsData.replaceMethod == clsSettingsData.mIPReplaceMethod.KEEP_LOCALHOST)
            {
                exampleRegistryKey.SetValue("redirectType", "KEEP_LOCALHOST");
            }
            else if (clsSettingsData.replaceMethod == clsSettingsData.mIPReplaceMethod.SET_CUSTOM)
            {
                exampleRegistryKey.SetValue("redirectType", "SET_CUSTOM");
                exampleRegistryKey.SetValue("replaceIP", clsSettingsData.replaceIP);
            }
            else
            {
                exampleRegistryKey.SetValue("redirectType", "SET_WHITEPAGE");
            }
            if (clsSettingsData.UseHostsFileBlacklist)
                exampleRegistryKey.SetValue("UseHostsFileBlacklist", "TRUE");
            else
                exampleRegistryKey.SetValue("UseHostsFileBlacklist", "FALSE");
            if (clsSettingsData.UseCustomBlacklist)
                exampleRegistryKey.SetValue("UseCustomBlacklist ", "TRUE");
            else
                exampleRegistryKey.SetValue("UseCustomBlacklist ", "FALSE");
            if (clsSettingsData.UseStevensBlacklist)
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
                serializer.Serialize(w, clsSettingsData.urls);
                w.Close();
            }
            catch (Exception)
            {
                if (!clsSettingsData.isHidden) MessageBox.Show("Could not save settings.");
            }

            //Write additional Hosts to blacklist.xml
            var serializer1 = new XmlSerializer(typeof(ArrayList), new Type[] {
    typeof(String)
   });
            try
            {
                XmlWriter w = XmlWriter.Create("Blacklist.xml");
                serializer1.Serialize(w, clsSettingsData.addHosts);
                w.Close();
            }
            catch (Exception)
            {
                if (!clsSettingsData.isHidden) MessageBox.Show("Could not save settings.");
            }
        }

        public void loadOptions()
        {
            if (clsSettingsData.replaceMethod == clsSettingsData.mIPReplaceMethod.KEEP_LOCALHOST)
            {
                frmHosts.rbRedirectLocalhost.Checked = true;
            }
            else if (clsSettingsData.replaceMethod == clsSettingsData.mIPReplaceMethod.SET_CUSTOM)
            {
                frmHosts.rbRedirectCustom.Checked = true;
                frmHosts.txtReplaceIP.Text = clsSettingsData.replaceIP;
            }
            else
            {
                frmHosts.rbRedirectWhitepage.Checked = true;
            }
            frmHosts.lbAddHosts.Items.Clear();
            frmHosts.lbURLs.Items.Clear();
            //FIll URL list
            foreach (String u in clsSettingsData.urls)
            {
                frmHosts.lbURLs.Items.Add(u);
            }
            foreach (String h in clsSettingsData.addHosts)
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
            if (clsSettingsData.internalEditor == "" || clsSettingsData.internalEditor == null)
                clsSettingsData.internalEditor = "INTERNAL";
            if (clsSettingsData.internalEditor == "INTERNAL")
            {
                frmHosts.rbInternal.Checked = true;
                frmHosts.rbExternal.Checked = false;
                frmHosts.rbCustom.Checked = false;
            }
            else if (clsSettingsData.internalEditor == "WORDPAD")
            {
                frmHosts.rbExternal.Checked = true;
                frmHosts.rbInternal.Checked = false;
                frmHosts.rbCustom.Checked = false;
            }
            else if (clsSettingsData.internalEditor == "CUSTOM")
            {
                frmHosts.rbCustom.Checked = true;
                frmHosts.rbInternal.Checked = false;
                frmHosts.rbExternal.Checked = false;
            }
            if (clsSettingsData.externalEditorFile != "")
                frmHosts.txtCustomEditor.Text = clsSettingsData.externalEditorFile;

            //Auto Update?
            if (clsSettingsData.autoUpdate)
            {
                frmHosts.cbAutoUpdate.Checked = true;
            }
            else
            {
                frmHosts.cbAutoUpdate.Checked = false;
            }
            frmHosts.rbUseHostsFileBL.Checked = clsSettingsData.UseHostsFileBlacklist;
            frmHosts.rbUseCustomBL.Checked = clsSettingsData.UseCustomBlacklist;
            frmHosts.rbUseStevenBlacksBL.Checked = clsSettingsData.UseStevensBlacklist;

            String[] files = clsCoreFunctions.getBackups();
            foreach (String f in files)
            {
                if (f != null)
                    if (f != "")
                        frmHosts.lbOptionsBackup.Items.Add(f);
            }
        }



        public  void saveOptions()
        {
            if (frmHosts.cbAutoUpdate.Checked)
                clsSettingsData.autoUpdate = true;
            else
                clsSettingsData.autoUpdate = false;
            if (frmHosts.rbInternal.Checked)
                clsSettingsData.internalEditor = "INTERNAL";
            else if (frmHosts.rbExternal.Checked)
                clsSettingsData.internalEditor = "WORDPAD";
            else
                clsSettingsData.internalEditor = "CUSTOM";
            frmHosts.txtCustomEditor.Text = clsSettingsData.externalEditorFile;
            if ((frmHosts.txtFrom.ForeColor == Color.Gray && frmHosts.txtTo.ForeColor == Color.Black) ||
             (frmHosts.txtFrom.ForeColor == Color.Black && frmHosts.txtTo.ForeColor == Color.Gray))
                MessageBox.Show("Please enter both \"From\" and \"To\" IP");
            else
            {
                if (frmHosts.txtFrom.Text != "")
                    clsSettingsData.ipFrom = frmHosts.txtFrom.Text;
                if (frmHosts.txtTo.Text != "")
                    clsSettingsData.ipTo = frmHosts.txtTo.Text;
                clsSettingsData.urls.Clear();
                foreach (String item in frmHosts.lbURLs.Items)
                {
                    clsSettingsData.urls.Add(item);
                }
                foreach (String host in frmHosts.lbAddHosts.Items)
                {
                    clsSettingsData.addHosts.Add(host);
                }
            }
        }
    }
}
