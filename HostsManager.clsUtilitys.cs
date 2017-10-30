using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace HostsManager
{
    public static class clsUtilitys
    {
        public static bool isADMember()
        {
            String pcname = Environment.GetEnvironmentVariable("computername");
            String domainname = Environment.GetEnvironmentVariable("logonserver");
            if (("\\\\" + pcname.ToLower()).Equals(domainname.ToLower()))
                return false;
            else
                return true;
        }

        public static class HostsFile
        {

            //Set Permissions of hosts file to be writable by group admins
            public static FileSecurity setHostsFilePermissions()
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
            public static void resetHostsFilePermissions(FileSecurity fs)
            {
                try
                {
                    System.IO.File.SetAccessControl(
                     Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", fs);
                }
                catch (Exception) { }
            }
        }

        public static class Dialogs
        {

            public static DialogResult showYesNoDialog(String text)
            {
                frmDialog d = new frmDialog();
                d.action = text;
                d.yesNoButtons = true;
                return d.ShowDialog();
            }


            public static void showOKDIalog(String text, bool larger = false)
            {
                frmDialog f = new frmDialog();
                f.action = text;
                if (larger)
                    f.customHeight = 150;
                f.showButton = true;
                f.ShowDialog();
            }


            public static frmDialog dlg;
            public static void showDialog(object action)
            {
                try
                {
                    dlg = new frmDialog();
                    dlg.action = (String)action;
                   // if (dlg.IsAccessible)
                        dlg.ShowDialog();
                }
                catch (ThreadAbortException) { }
            }

            public delegate void closedlgdeleg();
            public static void closeDialog()
            {
                if (dlg.InvokeRequired)
                    dlg.Invoke(new closedlgdeleg(closeDialog));
                else
                    dlg.Close();
            }

        }

        public static class SSLCertificates
        {

            //Search for FireFox Profile. 
            //080417DH - give credits
            public static string ReadFirefoxProfile()
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
                            if (lines.Length > 1)
                            {
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
                }
                return "";
            }

            private static void importCert(String profPath)
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
        }

        public static class Info
        { 
            //Check for antivir
            public static bool isAntivir()
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

           

            public static String getCurrentDNSServer()
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
        }

        public static class Downloader
        {
            public static String DownloadString(String url)
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
        

        public static class Services
        {

            public static bool isServiceActive()
            {
                string path = "Win32_Service.Name='dnscache'";
                ManagementPath p = new ManagementPath(path);
                ManagementObject ManagementObj = new ManagementObject(p);
                return (ManagementObj["StartMode"].ToString().Equals("Auto"));
            }





            public static void changeServiceStartMode(string hostname, string serviceName, string startMode)
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
        }

        //Start external process with hidden window      
        public static Process executeNoWindow(String cmd, String param)
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
    }
}
