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
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Specialized;
using HttpLibrary;
using System.Reflection;
using System.Collections;
using System.Configuration;
using mydigitalstructure_attachment_loader.Properties;
using System.Web.Script.Serialization;

namespace mydigitalspace
{
    public partial class frmConfiguration : Form
    {
        string _sid = string.Empty;
        string _cookieHeader = string.Empty;
        bool _complete = false;
        bool _paused = false;
        string status = "running";
        Thread th;
        HttpConnection http;
        string baseRequestURI = "https://au.mydigitalstructure.com/";
        System.Windows.Forms.Timer timerQuery;

        public class ObjectType
        {
            public string ID { get; set; }
            public string Type { get; set; }
        }

        #region Object Instances

        public class Business
        {
            public string id { get; set; }
            public string tradename { get; set; }
        }
        public class Person
        {
            public string id { get; set; }
            public string firstname { get; set; }
            public string surname { get; set; }
            public string display
            {
                get
                {
                    return string.Format("{0} {1}", firstname, surname);
                }
            }
        }
        public class Action
        {
            public string id { get; set; }
            public string reference { get; set; }
            public string description { get; set; }
            public string display
            {
                get
                {
                    return string.Format("{0} {1}", reference, description);
                }
            }
        }
        public class Project
        {
            public string id { get; set; }
            public string reference { get; set; }
            public string description { get; set; }
            public string display
            {
                get
                {
                    return string.Format("{0} {1}", reference, description);
                }
            }
        }
        public class Document
        {
            public string id { get; set; }
            public string title { get; set; }
        }
        public class Site
        {
            public string id { get; set; }
            public string title { get; set; }
        }
        #endregion

        public frmConfiguration(string sid, string username, string cookieHeader)
        {
            InitializeComponent();

            _sid = sid;
            _cookieHeader = cookieHeader;
            txtUser.Text = username;

            LoadObjectTypes();
            LoadSettings();
            //InitializeFileWatcher();
            timerPoll.Enabled = true;
            timerPoll.Start();
        }

        private void LoadObjectTypes()
        {
            List<ObjectType> list = new List<ObjectType>();
            ObjectType ot = new ObjectType();
            ot.ID = "12";
            ot.Type = "Contact Business";
            list.Add(ot);
            ot = new ObjectType();
            ot.ID = "32";
            ot.Type = "Contact Person";
            list.Add(ot);
            ot = new ObjectType();
            ot.ID = "17";
            ot.Type = "Action";
            list.Add(ot);
            ot = new ObjectType();
            ot.ID = "1";
            ot.Type = "Project";
            list.Add(ot);
            ot = new ObjectType();
            ot.ID = "14";
            ot.Type = "Document";
            list.Add(ot);
            ot = new ObjectType();
            ot.ID = "85";
            ot.Type = "Site";
            list.Add(ot);
            cbObjectType.DisplayMember = "Type";
            cbObjectType.ValueMember = "ID";
            cbObjectType.DataSource = list;
        }

        private void LoadObjectInstances(string ObjectTypeID, string search)
        {
            string method = "";
            string requestURI = baseRequestURI;
            List<string> fields = new List<string>();
            switch (ObjectTypeID)
            {
                case "12":
                    {
                        method = "CONTACT_BUSINESS_SEARCH";
                        requestURI = requestURI + string.Format("rpc/contact/?method={0}&advanced=1&rf=XML&sid={1}", method, _sid);
                        string HTML = "";
                        fields.Add("tradename");
                        if (search.Length >= 3)
                        {
                            HTML = JsonPOST(requestURI, "tradename", search, fields);
                        }
                        else
                        {
                            HTML = JsonPOST(requestURI, "tradename", "", fields);
                        }
                        List<Business> list = ParseBusinessXML(HTML, "row");
                        cbObjectInstance.DataSource = list;
                        cbObjectInstance.DisplayMember = "tradename";
                        cbObjectInstance.ValueMember = "id";
                    }
                    break;
                case "32":
                    {
                        method = "CONTACT_PERSON_SEARCH";
                        requestURI = requestURI + string.Format("rpc/contact/?method={0}&advanced=1&rf=XML&sid={1}", method, _sid);
                        string HTML = "";
                        fields.Add("firstname");
                        fields.Add("surname");
                        if (search.Length >= 3)
                        {
                            HTML = JsonPOST(requestURI, "firstname", search, fields);
                        }
                        else
                        {
                            HTML = JsonPOST(requestURI, "firstname", "", fields);
                        }
                        List<Person> list = ParsePersonXML(HTML, "row");
                        cbObjectInstance.DataSource = list;
                        cbObjectInstance.DisplayMember = "display";
                        cbObjectInstance.ValueMember = "id";
                    }
                    break;
                case "17":
                    {
                        method = "ACTION_SEARCH";
                        requestURI = requestURI + string.Format("rpc/action/?method={0}&advanced=1&rf=XML&sid={1}", method, _sid);
                        string HTML = "";
                        fields.Add("actionreference");
                        fields.Add("description");
                        if (search.Length >= 3)
                        {
                            HTML = JsonPOST(requestURI, "actionreference", search, fields);
                        }
                        else
                        {
                            HTML = JsonPOST(requestURI, "actionreference", "", fields);
                        }
                        List<Action> list = ParseActionXML(HTML, "row");
                        cbObjectInstance.DataSource = list;
                        cbObjectInstance.DisplayMember = "display";
                        cbObjectInstance.ValueMember = "id";
                    }
                    break;
                case "1":
                    {
                        method = "PROJECT_SEARCH";
                        requestURI = requestURI + string.Format("rpc/project/?method={0}&advanced=1&rf=XML&sid={1}", method, _sid);
                        string HTML = "";
                        fields.Add("reference");
                        fields.Add("description");
                        if (search.Length >= 3)
                        {
                            HTML = JsonPOST(requestURI, "reference", search, fields);
                        }
                        else
                        {
                            HTML = JsonPOST(requestURI, "reference", "", fields);
                        }
                        List<Project> list = ParseProjectXML(HTML, "row");
                        cbObjectInstance.DataSource = list;
                        cbObjectInstance.DisplayMember = "display";
                        cbObjectInstance.ValueMember = "id";
                    }
                    break;
                case "14":
                    {
                        method = "DOCUMENT_SEARCH";
                        requestURI = requestURI + string.Format("rpc/document/?method={0}&advanced=1&rf=XML&sid={1}", method, _sid);
                        string HTML = "";
                        fields.Add("title");
                        if (search.Length >= 3)
                        {
                            HTML = JsonPOST(requestURI, "title", search, fields);
                        }
                        else
                        {
                            HTML = JsonPOST(requestURI, "title", "", fields);
                        }
                        List<Document> list = ParseDocumentXML(HTML, "row");
                        cbObjectInstance.DataSource = list;
                        cbObjectInstance.DisplayMember = "title";
                        cbObjectInstance.ValueMember = "id";
                    }
                    break;
                case "85":
                    {
                        method = "SETUP_SITE_SEARCH";
                        requestURI = requestURI + string.Format("onDemand/setup/?method={0}&rf=XML&sid={1}", method, _sid);
                        string HTML = "";
                        fields.Add("title");
                        if (search.Length >= 3)
                        {
                            HTML = JsonPOST(requestURI, "title", search, fields);
                        }
                        else
                        {
                            HTML = JsonPOST(requestURI, "title", "", fields);
                        }
                        List<Site> list = ParseSiteXML(HTML, "row");
                        cbObjectInstance.DataSource = list;
                        cbObjectInstance.DisplayMember = "title";
                        cbObjectInstance.ValueMember = "id";
                    }
                    break;
                default:
                    break;
            }
        }

        private void LoadSettings()
        {
            txtPassword.Text = Util.GetAppConfigSetting("password");
            txtConfirmPassword.Text = Util.GetAppConfigSetting("password");
            txtSyncFolder.Text = Util.GetAppConfigSetting("syncfolder");
            txtFolderMovedTo.Text = Util.GetAppConfigSetting("archivefolder");
            cbObjectType.SelectedValue = Util.GetAppConfigSetting("objecttype");
            cbObjectInstance.SelectedValue = Util.GetAppConfigSetting("objectinstance");
        }

        //private void InitializeFileWatcher()
        //{
        //    if (Util.GetAppConfigSetting("syncfolder").Length > 0)
        //    {
        //        fsw = new FileSystemWatcher();
        //        fsw.Path = Util.GetAppConfigSetting("syncfolder");
        //        //watch for changes in LastAccess and LastWrite times, and the renaming of files or directories
        //        fsw.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
        //        fsw.IncludeSubdirectories = false;

        //        //Add Event Handlers
        //        fsw.Changed += new FileSystemEventHandler(OnChanged);
        //        fsw.Created += new FileSystemEventHandler(OnChanged);
        //        fsw.Renamed += new RenamedEventHandler(OnRenamed);

        //        fsw.InternalBufferSize = 32768; //set buffer size to 32KB (each event can take up to 16B)

        //        //Begin Watching
        //        fsw.EnableRaisingEvents = true;
        //    }
        //    else
        //    {
        //        fsw = null;
        //    }
        //}

        #region File Watcher Events
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            string file = e.FullPath;
            FileAttributes attr = File.GetAttributes(file);

            //return if there is a directory change
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return;
            }
            ProcessFile(file);
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            string file = e.FullPath;
            FileAttributes attr = File.GetAttributes(file);

            //return if there is a directory change
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return;
            }
            ProcessFile(file);
        }
        #endregion

        private bool ProcessFile(string file)
        {
            notifyIcon1.Icon = mydigitalstructure_attachment_loader.Properties.Resources.syncing;
            notifyIcon1.Text = "Syncing...";

            bool success = false;
            try
            {
                FileInfo fi = new FileInfo(file);
                SetControlPropertyThreadSafe(lblStatus, "Text", string.Format("Uploading {0}", fi.Name));
                //upload file to server
                bool upload = UploadFile(file, Util.GetAppConfigSetting("objecttype"), Util.GetAppConfigSetting("objectinstance"));
                if (upload)
                {
                    //move to destination folder
                    string Dest = Path.Combine(Util.GetAppConfigSetting("archivefolder"), fi.Name);
                    File.Delete(Dest);
                    fi.MoveTo(Dest);
                }

                SetControlPropertyThreadSafe(lblStatus, "Text", "Files Up To Date");

                success = true;
            }
            catch
            {
                success = false; 
            }

            notifyIcon1.Icon = mydigitalstructure_attachment_loader.Properties.Resources.mydigitalspace;
            notifyIcon1.Text = "Files Up To Date";

            return success;
        }

        private void Run()
        {
            Dictionary<string, object> result = Util.ProcessLogin(Util.GetAppConfigSetting("username"), Util.GetAppConfigSetting("password"));
            if (result.ContainsKey("sid"))
            {
                _sid = result["sid"].ToString();
                _complete = false;
                _paused = false;

                string[] files = Directory.GetFiles(Util.GetAppConfigSetting("syncfolder"));
                foreach (string file in files)
                {
                    if (_paused)
                    {
                        while (_paused)
                        {
                            SetControlPropertyThreadSafe(lblStatus, "Text", "Sync Paused");
                            notifyIcon1.Icon = mydigitalstructure_attachment_loader.Properties.Resources.paused;
                            notifyIcon1.Text = "Paused...";
                        }
                    }
                    ProcessFile(file);
                }

                _complete = true;
                SetControlPropertyThreadSafe(lblStatus, "Text", "Files Up To Date");
            }
            else
            {
                SetControlPropertyThreadSafe(lblStatus, "Text", "Authentication Error!");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                http.Dispose();
                http = null;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                this.Close();
                Application.Exit();
            }
        }

        #region XML Parsing
        private static List<Business> ParseBusinessXML(string xmlContent, string XName, string Element="")
        {
            List<Business> list = new List<Business>();

            try
            {
                if (xmlContent != string.Empty)
                {
                    XDocument xml = XDocument.Parse(xmlContent);

                    var items = from x in xml.Descendants(XName)
                                  select new
                                  {
                                      id = x.Descendants("id").First().Value,
                                      tradename = x.Descendants("tradename").First().Value
                                  };
                    foreach(var item in items)
                    {
                        Business b = new Business();
                        b.id = item.id;
                        b.tradename = item.tradename;
                        list.Add(b);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return list;
        }
        private static List<Person> ParsePersonXML(string xmlContent, string XName, string Element = "")
        {
            List<Person> list = new List<Person>();

            try
            {
                if (xmlContent != string.Empty)
                {
                    XDocument xml = XDocument.Parse(xmlContent);

                    var items = from x in xml.Descendants(XName)
                                select new
                                {
                                    id = x.Descendants("id").First().Value,
                                    firstname = x.Descendants("firstname").First().Value,
                                    surname = x.Descendants("surname").First().Value
                                };
                    foreach (var item in items)
                    {
                        Person p = new Person();
                        p.id = item.id;
                        p.firstname = item.firstname;
                        p.surname = item.surname;
                        list.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return list;
        }
        private static List<Action> ParseActionXML(string xmlContent, string XName, string Element = "")
        {
            List<Action> list = new List<Action>();

            try
            {
                if (xmlContent != string.Empty)
                {
                    XDocument xml = XDocument.Parse(xmlContent);

                    var items = from x in xml.Descendants(XName)
                                select new
                                {
                                    id = x.Descendants("id").First().Value,
                                    reference = x.Descendants("actionreference").First().Value,
                                    description = x.Descendants("description").First().Value
                                };
                    foreach (var item in items)
                    {
                        Action a = new Action();
                        a.id = item.id;
                        a.reference = item.reference;
                        a.description = item.description;
                        list.Add(a);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return list;
        }
        private static List<Project> ParseProjectXML(string xmlContent, string XName, string Element = "")
        {
            List<Project> list = new List<Project>();

            try
            {
                if (xmlContent != string.Empty)
                {
                    XDocument xml = XDocument.Parse(xmlContent);

                    var items = from x in xml.Descendants(XName)
                                select new
                                {
                                    id = x.Descendants("id").First().Value,
                                    reference = x.Descendants("reference").First().Value,
                                    description = x.Descendants("description").First().Value
                                };
                    foreach (var item in items)
                    {
                        Project p = new Project();
                        p.id = item.id;
                        p.reference = item.reference;
                        p.description = item.description;
                        list.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return list;
        }
        private static List<Document> ParseDocumentXML(string xmlContent, string XName, string Element = "")
        {
            List<Document> list = new List<Document>();

            try
            {
                if (xmlContent != string.Empty)
                {
                    XDocument xml = XDocument.Parse(xmlContent);

                    var items = from x in xml.Descendants(XName)
                                select new
                                {
                                    id = x.Descendants("id").First().Value,
                                    title = x.Descendants("title").First().Value
                                };
                    foreach (var item in items)
                    {
                        Document d = new Document();
                        d.id = item.id;
                        d.title = item.title;
                        list.Add(d);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return list;
        }
        private static List<Site> ParseSiteXML(string xmlContent, string XName, string Element = "")
        {
            List<Site> list = new List<Site>();

            try
            {
                if (xmlContent != string.Empty)
                {
                    XDocument xml = XDocument.Parse(xmlContent);

                    var items = from x in xml.Descendants(XName)
                                select new
                                {
                                    id = x.Descendants("id").First().Value,
                                    title = x.Descendants("title").First().Value
                                };
                    foreach (var item in items)
                    {
                        Site s = new Site();
                        s.id = item.id;
                        s.title = item.title;
                        list.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return list;
        }
        #endregion

        private string JsonPOST(string requestURI, string searchby, string searchterm, List<string> fieldslist = null)
        {
            string XML = "";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(requestURI);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";
            string json = "";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string filters = new JavaScriptSerializer().Serialize(new
                {
                    name = searchby,
                    //comparison = "TEXT_STARTS_WITH",
                    comparison = "TEXT_IS_LIKE",
                    value1 = searchterm
                });

                string fields = "";
                foreach(string field in fieldslist)
                {
                    fields += "{" + string.Format("\"name\": \"{0}\"", field) + "},";
                }
                fields = fields.Substring(0, fields.Length - 1); //remove last comma

                json = "{" + string.Format("\"fields\":[{0}],", fields) + string.Format("\"filters\":[{0}]", filters) + "}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    XML = streamReader.ReadToEnd();
                }
            }

            return XML;
        }

        private bool UploadFile(string FileName, string ObjectType, string ObjectInstance)
        {
            bool success = false;

            NameValueCollection querystring = new NameValueCollection();
            querystring["sid"] = _sid;
            querystring.Add("maxfiles", "1");
            querystring.Add("object", ObjectType);
            querystring.Add("objectcontext", ObjectInstance);
            querystring.Add("filename0", Path.GetFileName(FileName));

            var httpRequest = new HttpWebRequestConnection();

            string requestURI = "https://au.mydigitalstructure.com/ondemand/attach/";

            var postBody = new HttpPostBodyBuilder.Multipart();
            foreach (string key in querystring.Keys)
            {
                postBody.AddParameter(key, querystring.Get(key));
            }

            var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);

            postBody.AddData(
            "oFile0",
            fileStream,
            Path.GetFileName(FileName),
            HTMLHelper.GetMimeType(FileName)
                );

            var bodyStream = postBody.PrepareData();
            bodyStream.Position = 0;
            var req = new HttpMessage.Request(bodyStream, "POST");
            req.ContentLength = bodyStream.Length;
            req.ContentType = postBody.GetContentType();
            req.Headers["Referer"] = requestURI;

            var response = httpRequest.Send(requestURI, req);
            if (response.Status == HttpStatusCode.OK)
            {
                success = true;
            }
            else
            {
                success = false;
            }

            bodyStream.Close();
            fileStream.Close();
            httpRequest.Dispose();

            return success;
        }

        private delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);

        public static void SetControlPropertyThreadSafe(Control control, string propertyName, object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate(SetControlPropertyThreadSafe), new object[] { control, propertyName, propertyValue });
            }
            else
            {
                Label lbl = control as Label;
                lbl.Text = propertyValue.ToString();
            }
        }

        private void frmConfiguration_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitApplication();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_complete)
            {
                timer1.Stop();
                timer1.Enabled = false;

                notifyIcon1.Icon = mydigitalstructure_attachment_loader.Properties.Resources.mydigitalspace;
                notifyIcon1.Text = "Files Up To Date";
            }
        }

        private void timerPoll_Tick(object sender, EventArgs e)
        {
            if (timerPoll.Interval == 10000) // 10 seconds for first iteration
            {
                timerPoll.Interval = 30000; //change to poll every 30 seconds
            }
            if (Util.GetAppConfigSetting("syncfolder").Length > 0 && !_paused && !timer1.Enabled)
            {
                notifyIcon1.Icon = mydigitalstructure_attachment_loader.Properties.Resources.syncing;
                notifyIcon1.Text = "Syncing...";
                status = "running";

                timer1.Enabled = true;
                timer1.Start();

                th = new Thread(new ThreadStart(Run));
                th.IsBackground = false;
                th.Start();
            }
        }

        private void timerQuery_Tick(object sender, EventArgs e)
        {
            RevokeQueryTimer();

            Cursor = Cursors.WaitCursor;
            try
            {
                LoadObjectInstances(cbObjectType.SelectedValue.ToString(), cbObjectInstance.Text);
            }
            catch { }
            Cursor = Cursors.Arrow;
        }

        void RevokeQueryTimer()
        {
            if (timerQuery != null)
            {
                timerQuery.Stop();
                timerQuery.Tick -= timerQuery_Tick;
                timerQuery = null;
            }
        }

        void RestartQueryTimer()
        {
            // Start or reset a pending query
            if (timerQuery == null)
            {
                timerQuery = new System.Windows.Forms.Timer { Enabled = true, Interval = 1000 };
                timerQuery.Tick += timerQuery_Tick;
            }
            else
            {
                timerQuery.Stop();
                timerQuery.Start();
            }
        }

        private void btnBrowseFolderLocation_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtSyncFolder.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnBrowseFolderMovedTo_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtFolderMovedTo.Text = folderBrowserDialog1.SelectedPath;
        }

        #region Context Menu

        private void miLoadConfig_Click(object sender, EventArgs e)
        {
            ToggleApplication();
        }

        private void miStartService_Click(object sender, EventArgs e)
        {
            switch (status)
            {
                //case "idle":
                //case "stopped":
                //    {
                //        notifyIcon1.Icon = mydigitalstructure_attachment_loader.Properties.Resources.mydigitalspace;
                //        notifyIcon1.Text = "Running...";
                //        status = "running";
                //        miStartService.Text = miStartService.Text.Replace("Start", "Pause");

                //        timer1.Enabled = true;
                //        timer1.Start();

                //        th = new Thread(new ThreadStart(Run));
                //        th.IsBackground = false;
                //        th.Start();
                //    }
                //    break;
                case "running":
                    {
                        notifyIcon1.Icon = mydigitalstructure_attachment_loader.Properties.Resources.paused;
                        notifyIcon1.Text = "Paused...";
                        status = "paused";
                        miStartService.Text = miStartService.Text.Replace("Pause", "Resume");

                        _paused = true;
                        //fsw.EnableRaisingEvents = false;
                        timer1.Stop();
                        timer1.Enabled = false;
                        timerPoll.Stop();
                        timerPoll.Enabled = false;
                    }
                    break;
                case "paused":
                    {
                        notifyIcon1.Icon = mydigitalstructure_attachment_loader.Properties.Resources.syncing;
                        notifyIcon1.Text = "Syncing...";
                        status = "running";
                        miStartService.Text = miStartService.Text.Replace("Resume", "Pause");
                        SetControlPropertyThreadSafe(lblStatus, "Text", "Sync Paused");

                        _paused = false;
                        //fsw.EnableRaisingEvents = true;
                        timer1.Enabled = true;
                        timer1.Start();
                        timerPoll.Start();
                        timerPoll.Enabled = true;
                    }
                    break;
            }
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to exit the Attachment Loader Application?", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                ExitApplication();
            }
        }

        #endregion

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ToggleApplication();
        }

        private void ToggleApplication()
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void ExitApplication()
        {
            try
            {
                //fsw.EnableRaisingEvents = false;
                http.Dispose();
                http = null;
                th.Abort();
            }
            catch
            {
            }
            finally
            {
                Application.Exit();
            }
        }

        private void frmConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {
            ToggleApplication();
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
        }

        private void btnAppSave_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            Dictionary<string, object> result = Util.ProcessLogin(txtUser.Text.Trim(), txtPassword.Text.Trim());
            if (result.ContainsKey("sid"))
            {
                _sid = result["sid"].ToString();

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Remove("username");
                config.AppSettings.Settings.Add("username", txtUser.Text.Trim());

                config.AppSettings.Settings.Remove("password");
                config.AppSettings.Settings.Add("password", txtPassword.Text.Trim());

                config.AppSettings.Settings.Remove("syncfolder");
                config.AppSettings.Settings.Add("syncfolder", txtSyncFolder.Text.Trim());

                config.AppSettings.Settings.Remove("archivefolder");
                config.AppSettings.Settings.Add("archivefolder", txtFolderMovedTo.Text.Trim());

                config.AppSettings.Settings.Remove("objecttype");
                config.AppSettings.Settings.Add("objecttype", cbObjectType.SelectedValue.ToString());

                config.AppSettings.Settings.Remove("objectinstance");
                config.AppSettings.Settings.Add("objectinstance", cbObjectInstance.SelectedValue.ToString());

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");                

                lblStatus.Text = "Application Settings Saved Successfully!";
            }
            else
            {
                lblStatus.Text = "Invalid Password Entered";
            }

            Cursor = Cursors.Arrow;
        }

        private void txtConfirmPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
            {
                lblPasswordValidation.Visible = true;
                btnAppSave.Enabled = false;
            }
            else
            {
                lblPasswordValidation.Visible = false;
                btnAppSave.Enabled = true;
            }
        }

        private void cbObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                
                LoadObjectInstances(cbObjectType.SelectedValue.ToString(), "");
            }
            catch { }

            Cursor = Cursors.Arrow;
        }

        private void cbObjectInstance_KeyUp(object sender, KeyEventArgs e)
        {
            //Cursor = Cursors.WaitCursor;
            //try
            //{
            //    if (cbObjectInstance.Text.Length >= 3)// || cbObjectInstance.Text.Length == 0)
            //    {
            //        LoadObjectInstances(cbObjectType.SelectedValue.ToString(), cbObjectInstance.Text);
            //    }
            //}
            //catch { }
            //Cursor = Cursors.Arrow;

            if (cbObjectInstance.Text.Length >= 3 && e.KeyCode != Keys.Tab)
            {
                RestartQueryTimer();
            }
            else
            {
                RevokeQueryTimer();
            }
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
        }
    }
}
