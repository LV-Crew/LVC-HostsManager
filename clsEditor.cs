using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostsManager
{

    public static class doEdit
    {
        public static void edit(bool editInternal, ArrayList urls)
        {
            clsEditor c = new clsEditor();
            if (editInternal)
                c.doEditIntern(urls);
            else
                c.doEditExtern();
        }                  
    }

    class clsEditor
    {
        public String doEditIntern(ArrayList urls)
        {
            String fileText = "";
            System.Net.WebClient wc = new System.Net.WebClient();
            if(urls.Count>0)
                foreach (String u in urls)
                    fileText += wc.DownloadString(u) + "\r\n";
            else
                fileText = wc.DownloadString("https://raw.githubusercontent.com/StevenBlack/hosts/master/hosts");

            frmEditHosts f = new frmEditHosts();
            f.mText = fileText;
            f.ShowDialog();
            fileText = f.mText;

            return fileText;
        }

        public void doEditExtern()
        {
            System.Diagnostics.Process.Start("wordpad.exe", Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
        }
    }
}
