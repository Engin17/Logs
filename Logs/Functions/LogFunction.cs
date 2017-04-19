using Logs.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;

namespace Logs.Functions
{
    public class LogFunction
    {
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
                    Directory.Delete(logCopyTemp, true);
                    CopyLogs(logPath, logCopyTemp, copySubDirs);
                }


            }
            catch (DirectoryNotFoundException ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextError + ex.Message;
            }
        }

        /// <summary>
        /// Method to create server logs or client logs
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="logTempZip"></param>
        public static void CreateLogs(string logPath, string logTempZip, string logName)
        {
            // check if logs already exported
            FileInfo sFile = new FileInfo(logTempZip);
            bool fileExist = sFile.Exists;

            try
            {
                if (!fileExist)
                {
                    
                    ZipFile.CreateFromDirectory(logPath, logTempZip, CompressionLevel.Fastest, true);

                    if (logName == MainViewModel.ClientLogsConfName && File.Exists(MainViewModel.ClientLogsConfTempZip))
                    {
                        MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ClientLogsConfName + MainViewModel.LogTextZipSuccess;
                        MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ClientLogsConfName + MainViewModel.LogTextZipLocation + MainViewModel.ClientLogsConfTempZip;

                        MainViewModel.UpdatePropertiesCreateLogsAtEnd();
                    }
                    else if (File.Exists(MainViewModel.ServerLogsTempZip) && logName == MainViewModel.ServerLogsName)
                    {
                        MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ServerLogsName + MainViewModel.LogTextZipSuccess;
                        MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ServerLogsName + MainViewModel.LogTextZipLocation + MainViewModel.ServerLogsTempZip;

                        MainViewModel.UpdatePropertiesCreateLogsAtEnd();
                    }
                    else if (File.Exists(MainViewModel.LogsZipFolderPathZip) && logName == MainViewModel.LogZipFolderName)
                    {
                        MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogZipFolderName + MainViewModel.LogTextZipSuccess;
                        MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogZipFolderName + MainViewModel.LogTextZipLocation + MainViewModel.LogsZipFolderPathZip;
                    }
                }
                else
                {
                    try
                    {
                        File.Delete(logTempZip);
                    }
                    catch (FileNotFoundException ex)
                    {
                        MainViewModel.LogText += MainViewModel.LogTextInfo + ex.Message;
                    }
                    CreateLogs(logPath, logTempZip, logName);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextError + ex.Message;
                MainViewModel.LogText += MainViewModel.LogTextError + MainViewModel.LogTextCouldNotCreate + MainViewModel.ClientLogsConfName.ToLower() + MainViewModel.LogTextZipFolder;
            }
        }

        /// <summary>
        /// Method to upload files to the FTP server
        /// </summary>
        /// <param name="logPath"></param>
        public static void UploadLogsFTP(string logPath, string logName)
        {
            FileStream fs = null;
            Stream rs = null;

            try
            {
                string file = logPath;
                string uploadFileName = new FileInfo(file).Name;
                uploadFileName = logName + "_" + uploadFileName;
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

                Thread.Sleep(500);
                MainViewModel.ProgressbarVisibility = Visibility.Hidden;
                MainViewModel.TbProgressTextVisibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextError + MainViewModel.LogTextUploadFailed + ex.Message;
                MainViewModel.IsUploadSucceeded = false;
                MainViewModel.ProgressbarVisibility = Visibility.Hidden;
                MainViewModel.TbProgressTextVisibility = Visibility.Hidden;
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
            StartCleaning(logPath);
        }

        /// <summary>
        /// Method to open created log zip folder path
        /// </summary>
        public static void OpenLogsPath(string zipPath)
        {
            try
            {
                if (!Directory.Exists(zipPath))
                {
                    Directory.CreateDirectory(zipPath);
                }
                // Open log zip folder with task manager
                Process.Start(zipPath);
            }
            catch (Win32Exception ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextError + ex.Message;
            }
        }

        public static void CopyClientServerZipFolder()
        {
            if (!Directory.Exists(MainViewModel.LogsTemp))
            {
                Directory.CreateDirectory(MainViewModel.LogsTemp);
            }

            try
            {
                // Will not overwrite if the destination file already exists.
                File.Copy(MainViewModel.ClientLogsConfTempZip, MainViewModel.LogsClientConfCopyTempZip, true);
                File.Copy(MainViewModel.ServerLogsTempZip, MainViewModel.LogsServerCopyTempZip, true);
            }

            // Catch exception if the file was already copied.
            catch (IOException ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextError + ex.Message;
            }
        }

        /// <summary>
        /// Metho
        /// </summary>
        private static void StartCleaning(string file)
        {

            if (file == MainViewModel.ServerLogsTempZip)
            {
                if (MainViewModel.IsUploadSucceeded)
                {
                    MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ServerLogsName + MainViewModel.LogTextUploadSucceeded;
                    DeleteFilesFoldersAfterUpload(MainViewModel.ServerLogsTempZip, MainViewModel.ServerLogsCopyTemp);
                }
                MainViewModel.UpdateFTPUploadButons();
            }

            else if (file == MainViewModel.ClientLogsConfTempZip)
            {
                if (MainViewModel.IsUploadSucceeded)
                {
                    MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ClientLogsConfName + MainViewModel.LogTextUploadSucceeded;
                    DeleteFilesFoldersAfterUpload(MainViewModel.ClientLogsConfTempZip, MainViewModel.ClientLogsConfTemp);
                }
                MainViewModel.UpdateFTPUploadButons();                
            }

            else if (file == MainViewModel.LogsZipFolderPathZip)
            {
                if (MainViewModel.IsUploadSucceeded)
                {
                    MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogZipFolderName + MainViewModel.LogTextUploadSucceeded;
                    DeleteFilesFoldersAfterUpload(MainViewModel.ServerLogsTempZip, MainViewModel.ServerLogsCopyTemp);
                    DeleteFilesFoldersAfterUpload(MainViewModel.ClientLogsConfTempZip, MainViewModel.ClientLogsConfTemp);
                    DeleteFilesFoldersAfterUpload(MainViewModel.LogsZipFolderPathZip, MainViewModel.LogsTemp);
                }
                MainViewModel.UpdateFTPUploadButons();;
            }
            MainViewModel.UpdateFTPUploadButons();
            MainViewModel.IsUploadSucceeded = true;
        }

        private static void DeleteFilesFoldersAfterUpload(string file, string folder)
        {
            try
            {
                File.Delete(file);
                Directory.Delete(folder, true);
            }
            catch (FileNotFoundException ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextInfo + ex.Message;
            }
            catch (DirectoryNotFoundException ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextInfo + ex.Message;
            }
        }

        /// <summary>
        /// Method to check internet connection 
        /// </summary>
        public static bool CheckInternetConnection()
        {
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send("www.google.com", 100);

                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        public static void CheckFolderSize(string logPath)
        {
            try
            {
                long length = Directory.GetFiles(logPath, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                double size = Math.Round((length / 1024d) / 1024d, 2);
                size = Math.Round(size, 2);


                if (size > 1000d)
                {
                    size = Math.Round(size / 1000d, 2);

                    MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogTextFolderSize + size + MainViewModel.LogTextFolderBePatientGB;
                }
                else if (size > 300d)
                {
                    MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogTextFolderSize + size + MainViewModel.LogTextFolderBePatientMB;
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MainViewModel.LogText += "\n Not found";
            }
        }

        public static void CheckZipSize(string zipPath)
        {
            // Depending on the internet speed and the size of the zip folder the upload may take some time. please be patient and wait until the logs are uploaded to the FTP server
            long length = new FileInfo(MainViewModel.ClientLogsConfTempZip).Length;
            double size = Math.Round((length / 1204d) / 1024d, 2);
            MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogTextZipSize + size + MainViewModel.LogTextZipBePatientMB;
        }
    }

}
