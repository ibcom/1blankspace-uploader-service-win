using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using mydigitalstructure_attachment_loader.Properties;

namespace mydigitalspace
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string _cookieHeader = string.Empty;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Util.GetAppConfigSetting("password").Length > 0)
            {
                Dictionary<string, object> result = Util.ProcessLogin(Util.GetAppConfigSetting("username"), Util.GetAppConfigSetting("password"));
                if (result.ContainsKey("sid"))
                {
                    Form frm = new frmConfiguration(result["sid"].ToString(), Util.GetAppConfigSetting("username"), _cookieHeader);
                    Application.Run(frm);
                }
                else
                {
                    Application.Run(new frmLogin());
                }
            }
            else
            {
                Application.Run(new frmLogin());
            }
        }
    }
}
