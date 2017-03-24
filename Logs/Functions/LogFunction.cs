using Logs.ViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Logs.Functions
{
    public class LogFunction
    {
        private static bool _clientLogsConfZipCreated;
        private static bool _serverLogsZipCreated;
        private static string _logText = "Welcome \n";

        /// <summary>
        /// Property to check if client logs and conf zip folder is created
        /// </summary>
        public static bool ClientLogsConfZipCreated
        {
            get { return _clientLogsConfZipCreated; }
            set { _clientLogsConfZipCreated = value; }
        }

        /// <summary>
        /// Property to check if server logs zip folder is created
        /// </summary>
        public static bool ServerLogsConfZipCreated
        {
            get { return _serverLogsZipCreated; }
            set { _serverLogsZipCreated = value; }
        }

        public static string LogText
        {
            get { return _logText; }
            set
            {
                if (_logText == value) return;
                _logText = value;
                RaiseStaticPropertyChanged("LogText");
            }
        }


        /// <summary>
        /// Method for copy the server logs, because we can not zip the server logs when the services are running
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="logCopyTemp"></param>
        /// <param name="copySubDirs"></param>
        public static void CopyLogs(string logPath, string logCopyTemp, bool copySubDirs)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(logPath);

                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(logCopyTemp))
                {
                    Directory.CreateDirectory(logCopyTemp);


                    // Get the files in the directory and copy them to the new location.
                    FileInfo[] files = dir.GetFiles();

                    foreach (FileInfo file in files)
                    {
                        string temppath = Path.Combine(logCopyTemp, file.Name);
                        file.CopyTo(temppath, false);
                    }

                    // If copying subdirectories, copy them and their contents to new location.
                    if (copySubDirs)
                    {
                        foreach (DirectoryInfo subdir in dirs)
                        {
                            string temppath = Path.Combine(logCopyTemp, subdir.Name);
                            CopyLogs(subdir.FullName, temppath, copySubDirs);
                        }
                    }
                }

                else
                {
                    // delete old copied logs and copy new logs inside the temp folder
                    Directory.Delete(logCopyTemp, true);
                    CopyLogs(logPath, logCopyTemp, true);
                }


            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Directory not found: " + ex.Message, "Fail");
            }

        }

        /// <summary>
        /// Method to create server logs or client logs
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="logTempZip"></param>
        public static void CreateLogs(string logPath, string logTempZip, string logName, string logTemp)
        {
            LogFunction test = new LogFunction();
            // check if logs already exported
            FileInfo sFile = new FileInfo(logTempZip);
            bool fileExist = sFile.Exists;
           
            try
            {
                if (!fileExist)
                {

                    ZipFile.CreateFromDirectory(logPath, logTempZip, CompressionLevel.Fastest, true);
                    LogText += "\n " + DateTime.Now;
                    MessageBox.Show(logName + " successfully zipped", "Succeeded");

                    if (logName == MainViewModel.ServerLogsName)
                    {
                        ServerLogsConfZipCreated = true;
                    }
                    else if (logName == MainViewModel.ClientLogsConfName)
                    {
                        ClientLogsConfZipCreated = true;
                    }
                }
                else
                {
                    File.Delete(logTempZip);
                    CreateLogs(logPath, logTempZip, logName, logTemp);
                }
                if (Directory.Exists(logTemp))
                {
                    Directory.Delete(logTemp, true);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Directory not found: " + ex.Message, "Fail");
            }
        }

        /// <summary>
        /// Method to upload files to the FTP server
        /// </summary>
        /// <param name="logPath"></param>
        public static void UploadLogsFTP(string logPath)
        {
            FileStream fs = null;
            Stream rs = null;

            try
            {
                string file = logPath;
                string uploadFileName = new FileInfo(file).Name;
                string uploadUrl = "ftp://217.22.207.192/";
                fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                string ftpUrl = string.Format("{0}/{1}", uploadUrl, uploadFileName);
                FtpWebRequest requestObj = FtpWebRequest.Create(ftpUrl) as FtpWebRequest;
                requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                requestObj.Credentials = new NetworkCredential("anonymous", "");
                rs = requestObj.GetRequestStream();

                byte[] buffer = new byte[8092];
                int read = 0;
                while ((read = fs.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, read);
                }
                rs.Flush();
                MessageBox.Show("File uploaded on FTP", "Succeeded");
            }
            catch (Exception ex)
            {
                MessageBox.Show("File upload/transfer Failed.\r\nError Message:\r\n" + ex.Message, "Succeeded");
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }

                if (rs != null)
                {
                    rs.Close();
                    rs.Dispose();
                }
            }
            try
            {
                // Delete exported ClientLogsConf.zip after upload
                File.Delete(logPath);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Evet to update static properties
        /// </summary>
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged(string propName)
        {
            EventHandler<PropertyChangedEventArgs> handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propName));
        }
    }

}
