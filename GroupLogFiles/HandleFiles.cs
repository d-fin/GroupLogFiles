using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GroupLogFiles
{
    internal class HandleFiles
    {
        private string logFile;
        private string logFileDirectory;
        public HandleFiles() 
        {
            this.logFile = String.Empty;
            this.logFileDirectory = String.Empty; 
        }

        public string LogFile
        {
            get { return logFile; }
            set { logFile = value; }
        }

        public string LogFileDirectory
        {
            get { return logFileDirectory; }
            set { logFileDirectory = value; }
        }

        public string PromptForLogFilePath()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
                openFileDialog.Title = "Select a Log File Location";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
            }
            return null;
        }

        public string PromptForLogFileDirectory()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a folder to store the log files:";
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string selectedFolderPath = folderBrowserDialog.SelectedPath;
                    return selectedFolderPath;
                }
                else
                {
                    return "N/A";
                }
            }
        }

        public bool DoesDirectoryExist()
        {
            return Directory.Exists(GetDirectoryFromConfig());
        }

        public string GetDirectoryFromConfig()
        {
            return ConfigurationManager.AppSettings["logFileDirectory"];
        }

        public void UpdateLogDirectoryInConfig(string logFileDirectory)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["logFileDirectory"].Value = logFileDirectory;
            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }

        public void UpdateLogPathInConfig(string logPath)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["logPath"].Value = logPath;
            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }

        public bool DoesLogPathExist()
        {
            return File.Exists(GetLogFilePathFromConfig());
        }
        public string GetLogFilePathFromConfig()
        {
            return ConfigurationManager.AppSettings["logPath"];
        }
    }
}
