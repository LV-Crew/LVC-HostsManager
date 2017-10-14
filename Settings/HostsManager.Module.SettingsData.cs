using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HostsManager.Settings
{
    public static class SettingsData
    {
        public static String WHITEPAGE_IP = "1.2.3.4";
        public
        const int WM_NCLBUTTONDOWN = 0xA1;
        public
        const int HT_CAPTION = 0x2;
        public
        const int WM_LBUTTONDOWN = 0x0201;

        public  enum BlacklistTypes
        {
            INTERNAL = 1,
            STEVENBLACK = 2,
            HOSTSFILEDOTNET = 3
        }

        public  enum mIPReplaceMethod
        {
            KEEP_LOCALHOST = 0,
            SET_WHITEPAGE = 1,
            SET_CUSTOM = 2
        }

        public static System.Media.SoundPlayer player;
        public static mIPReplaceMethod replaceMethod = mIPReplaceMethod.SET_WHITEPAGE;
        public static String ipFrom = "0.0.0.0";
        public static String ipTo = "34.213.32.36";
        public static String hostsURL = "https://hosts-file.net/download/hosts.txt";
        public static ArrayList urls = new ArrayList();
        public static ArrayList addHosts = new ArrayList();
        public static String internalEditor = "INTERNAL";
        public static bool autoUpdate = false;
        public static bool isHidden = false;
        public static bool showFakeNews = false;
        public static bool showGambling = false;
        public static bool showPorn = false;
        public static bool showSocial = false;
        public static BlacklistTypes blacklistToUse = BlacklistTypes.INTERNAL;
        public static String externalEditorFile = "";
        public static bool DNServiceDisabled = false;
        public static bool DNSGoogleChanged = false;
        public static bool DNSOpenDNSChanged = false;
        public static String oldDNS = "192.168.2.1";
        public static String replaceIP = "0.0.0.0";
        public static bool UseHostsFileBlacklist = true;
        public static bool UseCustomBlacklist = false;
        public static bool UseStevensBlacklist = false;
    }
}
