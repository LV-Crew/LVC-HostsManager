using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HostsManager
{
    class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool WritePrivateProfileString(
            string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint GetPrivateProfileString(
            string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString,
            uint nSize, string lpFileName);
    }

    public static class clsBrandingINI
    {
        public static void readINI()
        {
            String currdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            currdir = currdir+"\\branding.ini";

            if (System.IO.File.Exists(currdir))
            {

                StringBuilder ret = new StringBuilder(4096, 4096);
                NativeMethods.GetPrivateProfileString("HostsManager", "Version", Branding.VERSION, ret, 4096, currdir);
                Branding.VERSION = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                NativeMethods.GetPrivateProfileString("HostsManager", "Company", Branding.COMPANY, ret, 4096, currdir);
                Branding.COMPANY = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                NativeMethods.GetPrivateProfileString("HostsManager", "Product", Branding.PRODUCT, ret, 4096, currdir);
                Branding.PRODUCT = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                NativeMethods.GetPrivateProfileString("HostsManager", "ProductImage", Branding.PRODUCTIMGPATH, ret,
                    4096, currdir);
                Branding.PRODUCTIMGPATH = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                NativeMethods.GetPrivateProfileString("HostsManager", "ProductIcon", Branding.ICONPATH, ret, 4096,
                    currdir);
                Branding.ICONPATH = ret.ToString();
            }
        }
    }
}
