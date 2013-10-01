using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml.Linq;
using HttpLibrary;
using System.Collections.Specialized;
using mydigitalstructure_attachment_loader.Properties;
using System.Configuration;

namespace mydigitalspace
{
    public partial class frmLogin : Form
    {
        string _cookieHeader = string.Empty;
        HttpConnection http;

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            Dictionary<string, object> result = Util.ProcessLogin(tbUserName.Text.Trim(), tbPassword.Text.Trim());
            if (result.ContainsKey("sid"))
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Remove("username");
                config.AppSettings.Settings.Add("username", tbUserName.Text.Trim());
                config.AppSettings.Settings.Remove("password");
                config.AppSettings.Settings.Add("password", tbPassword.Text.Trim());
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                Form frm = new frmConfiguration(result["sid"].ToString(), tbUserName.Text.Trim(), _cookieHeader);
                frm.Show();
                this.Hide();
            }
            else
            {
                lblErrorMessage.Visible = true;
            }

            //Form frm = new frmConfiguration("123", tbUserName.Text.Trim(), _cookieHeader);
            //frm.Show();
            //this.Hide();
            Cursor = Cursors.Arrow;
        }
    }
}
