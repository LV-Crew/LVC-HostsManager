using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace HostsManager
{



        public static class clsBrandingINI
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern long SetPrivateProfileString(
    string lpAppName, string lpKeyName, string lpString, string lpFileName);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern long GetPrivateProfileString(
            string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString,
            uint nSize, string lpFileName);

        public static void readINI()
        {
            String currdir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            currdir = currdir+"\\branding.ini";
            if (System.IO.File.Exists(currdir))
            {
                StringBuilder ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "Version", Branding.VERSION, ret, 4096, currdir);
                Branding.VERSION = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "Company", Branding.COMPANY, ret, 4096, currdir);
                Branding.COMPANY = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "Product", Branding.PRODUCT, ret, 4096, currdir);
                Branding.PRODUCT = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "ProductImage", Branding.PRODUCTIMGPATH, ret,
                    4096, currdir);
                Branding.PRODUCTIMGPATH = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "ProductIcon", Branding.ICONPATH, ret, 4096,
                    currdir);
                Branding.ICONPATH = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "DefaultBlankHost", Branding.DefaultBlankHost, ret, 4096,
                    currdir);
                Branding.DefaultBlankHost = ret.ToString();

                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "BannerImage", Branding.BannerImage, ret, 4096,
                    currdir);
                Branding.BannerImage = ret.ToString();

                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "BackgroundSound", Branding.BackgroundSound, ret, 4096,
                    currdir);
                Branding.BackgroundSound = ret.ToString();

                


            }
        }
    }
}
