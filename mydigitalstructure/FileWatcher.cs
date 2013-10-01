using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mydigitalstructure_attachment_loader
{
    /// <summary>
    /// Summary description for FileWatcher
    /// </summary>
    public class FileWatcher
    {
        private static FileSystemWatcher _watcher;
        private static string sLog = "";
        EventLog _eventlog = null;

        public FileWatcher(string sPath, EventLog eventLog)
        {
            _watcher = new FileSystemWatcher();
            _watcher.Path = sPath;
            _eventlog = eventLog;

            //watch for changes in LastAccess and LastWrite times, and the renaming of files or directories
            _watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
            _watcher.IncludeSubdirectories = true;

            //Add Event Handlers
            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.Created += new FileSystemEventHandler(OnChanged);
            _watcher.Deleted += new FileSystemEventHandler(OnChanged);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);

            _watcher.InternalBufferSize = 32768; //set buffer size to 32KB (each event can take up to 16B)

            //Begin Watching
            _watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.FullPath.ToLower().EndsWith(".pdf") || e.FullPath.ToLower().EndsWith(".iso"))
            {
                // Specify what is done when a file is changed, created, or deleted.
                string sLog = "File: " + e.FullPath + " " + e.ChangeType;
                short sCategory = 0;
                switch (e.ChangeType.ToString())
                {
                    case "Created":
                        sCategory = 1;
                        break;
                    case "Changed":
                        sCategory = 2;
                        break;
                    case "Deleted":
                        sCategory = 3;
                        break;
                    default:
                        break;
                }
                _eventlog.WriteEntry(sLog, EventLogEntryType.Information, 0, sCategory);
                Console.WriteLine(sLog);

                Indexer index = new Indexer(Properties.Settings.Default.Index_Location, _eventlog);

                if (e.ChangeType.ToString().Equals("Created"))
                {
                    bool b = index.AddToIndex(e.FullPath);

                    sLog = "File: " + e.FullPath + " added to index? - " + b;
                    _eventlog.WriteEntry(sLog, EventLogEntryType.Information, 0, sCategory);
                    Console.WriteLine(sLog);
                }
                else if ((e.ChangeType.ToString().Equals("Changed")))
                {
                    bool b = index.DeleteFromIndex(e.FullPath);
                    b = index.AddToIndex(e.FullPath);

                    sLog = "File: " + e.FullPath + " updated in index? - " + b;
                    _eventlog.WriteEntry(sLog, EventLogEntryType.Information, 0, sCategory);
                    Console.WriteLine(sLog);
                }
                else if (e.ChangeType.ToString().Equals("Deleted"))
                {
                    bool b = index.DeleteFromIndex(e.FullPath);

                    sLog = "File: " + e.FullPath + " deleted from index? - " + b;
                    _eventlog.WriteEntry(sLog, EventLogEntryType.Information, 0, sCategory);
                    Console.WriteLine(sLog);
                }
            }
            else if (Directory.Exists(e.FullPath)) //directory was modified/created
            {
                //string sLog = "Directory: " + e.FullPath + " " + e.ChangeType;
                //_eventlog.WriteEntry(sLog);
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            if (e.FullPath.ToLower().EndsWith(".pdf") || e.FullPath.ToLower().EndsWith(".iso"))
            {
                // Specify what is done when a file is renamed.
                string sLog = string.Format("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
                _eventlog.WriteEntry(sLog, EventLogEntryType.Information, 0, 4);
                Console.WriteLine(sLog);

                Indexer index = new Indexer(Properties.Settings.Default.Index_Location, _eventlog);
                bool b = index.DeleteFromIndex(e.OldFullPath);
                b = index.AddToIndex(e.FullPath);

                sLog = "File: " + e.FullPath + " renamed in index? - " + b;
                _eventlog.WriteEntry(sLog, EventLogEntryType.Information, 0, 4);
                Console.WriteLine(sLog);
            }
            else if (Directory.Exists(e.FullPath)) //directory was modified/created
            {
                //string sLog = string.Format("Directory: {0} renamed to {1}", e.OldFullPath, e.FullPath);
                //_eventlog.WriteEntry(sLog);
            }
        }

        public void RaiseEvents(bool bRaiseEvents)
        {
            _watcher.EnableRaisingEvents = bRaiseEvents;
        }

        public string Log
        {
            get
            {
                return sLog;
            }
        }

        public FileSystemWatcher watcher
        {
            get
            {
                return watcher;
            }
        }
    }
}
