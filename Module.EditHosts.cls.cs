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
        public static void edit(String editInternal, ArrayList urls)
        {
            clsEditor c = new clsEditor();
            if (editInternal=="INTERNAL")
                c.doEditIntern(urls);
            else if(editInternal=="WORDPAD")
                c.doEditExtern();
        }
        public static void edit(String editInternal, ArrayList urls, String editorPath)
        {
            clsEditor c = new clsEditor();
            if (editInternal == "INTERNAL")
                c.doEditIntern(urls);
            else if (editInternal == "WORDPAD")
                c.doEditExtern();
            else if (editInternal == "CUSTOM")
                c.doEditCustom(editorPath);
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
                    System.IO.File.WriteAllText(Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts", f.mText);
                    f.mText="";
                }
            }
            catch (Exception ex) {  MessageBox.Show("Could not write hosts file!"); }
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

        public void doEditCustom(String editorPath)
        {
            try
            {
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(editorPath, Environment.GetEnvironmentVariable("windir") + "\\system32\\drivers\\etc\\hosts");
                p.WaitForExit();
            }
            catch (Exception ex) { MessageBox.Show("Could not open external editor."); }
        }
    }
}
