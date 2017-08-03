using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            try
            {
                String txt = System.IO.File.ReadAllText(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
                
                frmEditHosts f = new frmEditHosts();
                f.mText = txt;
                f.ShowDialog();
                if (f.DialogResult == DialogResult.OK)
                {
                    txt = f.mText;
                    System.IO.File.WriteAllText(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", txt);
                }
            }
            catch (Exception ex) { MessageBox.Show("Could not write hosts file!"); }
            return fileText;
        }

        public void doEditExtern()
        {
            try
            {
                System.Diagnostics.Process p=System.Diagnostics.Process.Start("wordpad.exe", Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
                p.WaitForExit();
            }
            catch (Exception ex) { MessageBox.Show("Could not open external editor."); }
        }
    }
}
