using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HostsManager
{
    public static class Branding
    {
        public static String VERSION = "2017.08.04c";
        public static String COMPANY = "LV-Crew";
        public static String PRODUCT = "HostsManager";
        public static String PRODUCTIMGPATH = "LV-Crew.HostsManager.Logo.png";
        public static String ICONPATH = "LV-Crew.HostsManager.Icon.ico";
        public static String DefaultBlankHost = "127.0.0.1";

        public static string BannerImage { get; internal set; }
        public static string BackgroundSound { get; internal set; }
    }
}
