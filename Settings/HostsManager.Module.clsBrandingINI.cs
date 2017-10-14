using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace HostsManager.Settings
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
                GetPrivateProfileString("HostsManager", "Version", clsBrandingData.VERSION, ret, 4096, currdir);
                clsBrandingData.VERSION = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "Company", clsBrandingData.COMPANY, ret, 4096, currdir);
                clsBrandingData.COMPANY = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "Product", clsBrandingData.PRODUCT, ret, 4096, currdir);
                clsBrandingData.PRODUCT = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "ProductImage", clsBrandingData.PRODUCTIMGPATH, ret,
                    4096, currdir);
                clsBrandingData.PRODUCTIMGPATH = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "ProductIcon", clsBrandingData.ICONPATH, ret, 4096,
                    currdir);
                clsBrandingData.ICONPATH = ret.ToString();
                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "DefaultBlockPage", clsBrandingData.DefaultBlankHost, ret, 4096,
                    currdir);
                clsBrandingData.DefaultBlankHost = ret.ToString();

                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "BannerImage", clsBrandingData.BannerImage, ret, 4096,
                    currdir);
                clsBrandingData.BannerImage = ret.ToString();

                ret = new StringBuilder(4096, 4096);
                GetPrivateProfileString("HostsManager", "BackgroundSound", clsBrandingData.BackgroundSound, ret, 4096,
                    currdir);
                clsBrandingData.BackgroundSound = ret.ToString();

                


            }
        }
    }
}
