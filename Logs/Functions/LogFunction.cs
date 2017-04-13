using Logs.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
                MainViewModel.LogText += "\n [" + DateTime.Now + "] ERROR: " + ex.Message;
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
                        MainViewModel.LogText += "\n [" + DateTime.Now + "] INFO: " + MainViewModel.ClientLogsConfName + " zip folder successfully zipped";
                        MainViewModel.LogText += "\n [" + DateTime.Now + "] INFO: " + MainViewModel.ClientLogsConfName + " zip folder is under: " + MainViewModel.ClientLogsConfTempZip;
                        MainViewModel.IsBtnClientFTPEnabled = true;
                        MainViewModel.IsBtnUploadAllFTPEnabled = true;
                        Thread.Sleep(500);
                        MainViewModel.ProgressbarVisibility = Visibility.Hidden;
                        MainViewModel.TbProgressTextVisibility = Visibility.Hidden;
                    }
                    else if (File.Exists(MainViewModel.ServerLogsTempZip) && logName == MainViewModel.ServerLogsName)
                    {
                        MainViewModel.LogText += "\n [" + DateTime.Now + "] INFO: " + MainViewModel.ServerLogsName + " zip folder successfully zipped";
                        MainViewModel.LogText += "\n [" + DateTime.Now + "] INFO: " + MainViewModel.ServerLogsName + " zip folder is under: " + MainViewModel.ServerLogsTempZip;
                        MainViewModel.IsBtnServerFTPEnabled = true;
                        MainViewModel.IsBtnUploadAllFTPEnabled = true;
                        Thread.Sleep(500);
                        MainViewModel.ProgressbarVisibility = Visibility.Hidden;
                        MainViewModel.TbProgressTextVisibility = Visibility.Hidden;
                    }
                    else if (File.Exists(MainViewModel.LogsZipFolderPathZip) && logName == MainViewModel.LogZipFolderName)
                    {
                        MainViewModel.LogText += "\n [" + DateTime.Now + "] INFO: " + MainViewModel.LogZipFolderName + " zip folder successfully zipped";
                        MainViewModel.LogText += "\n [" + DateTime.Now + "] INFO: " + MainViewModel.LogZipFolderName + " zip folder is under: " + MainViewModel.LogsZipFolderPathZip;
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
                        MainViewModel.LogText += "\n [" + DateTime.Now + "] ERROR: " + ex.Message;
                    }
                    CreateLogs(logPath, logTempZip, logName);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MainViewModel.LogText += "\n [" + DateTime.Now + "] ERROR: " + ex.Message;
                MainViewModel.LogText += "\n [" + DateTime.Now + "] ERROR: Could not create " + MainViewModel.ClientLogsConfName.ToLower() + " zip folder";
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
                MainViewModel.LogText += "\n [" + DateTime.Now + "] ERROR: " + "File upload/transfer Failed.\r\nError Message: " + ex.Message;
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
                MainViewModel.LogText += "\n [" + DateTime.Now + "] ERROR: " + ex.Message;
            }
        }

        public static void StartCleaning(string file)
        {
            
            if (file == MainViewModel.ServerLogsTempZip)
            {
                MainViewModel.LogText += "\n [" + DateTime.Now + "] INFO: " + MainViewModel.ServerLogsName + " zip folder successfully uploaded to the FTP server";
                MainViewModel.IsBtnServerFTPEnabled = false;
                MainViewModel.IsBtnUploadAllFTPEnabled = false;
                MainViewModel.IsBtnClientFTPEnabled = true;
                DeleteFilesFoldersAfterUpload(MainViewModel.ServerLogsTempZip, MainViewModel.ServerLogsCopyTemp);

            }
            else if (file == MainViewModel.ClientLogsConfTempZip)
            {
                MainViewModel.LogText += "\n [" + DateTime.Now + "] INFO: " + MainViewModel.ClientLogsConfName + " zip folder successfully uploaded to the FTP server";
                MainViewModel.IsBtnClientFTPEnabled = false;
                MainViewModel.IsBtnUploadAllFTPEnabled = false;
                MainViewModel.IsBtnServerFTPEnabled = true;
                DeleteFilesFoldersAfterUpload(MainViewModel.ClientLogsConfTempZip, MainViewModel.ClientLogsConfTemp);

            }
            else if (file == MainViewModel.LogsZipFolderPathZip)
            {
                MainViewModel.LogText += "\n [" + DateTime.Now + "] INFO: " + MainViewModel.LogZipFolderName + " zip folder successfully uploaded to the FTP server";
                MainViewModel.IsBtnClientFTPEnabled = false;
                MainViewModel.IsBtnServerFTPEnabled = false;
                MainViewModel.IsBtnUploadAllFTPEnabled = false;
                DeleteFilesFoldersAfterUpload(MainViewModel.ServerLogsTempZip, MainViewModel.ServerLogsCopyTemp);
                DeleteFilesFoldersAfterUpload(MainViewModel.ClientLogsConfTempZip, MainViewModel.ClientLogsConfTemp);
                DeleteFilesFoldersAfterUpload(MainViewModel.LogsZipFolderPathZip, MainViewModel.LogsTemp);

            }

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
                MainViewModel.LogText += "\n [" + DateTime.Now + "] ERROR: " + ex.Message;
            }
            catch (DirectoryNotFoundException ex)
            {
                MainViewModel.LogText += "\n [" + DateTime.Now + "] ERROR: " + ex.Message;
            }
        }

    }

}
