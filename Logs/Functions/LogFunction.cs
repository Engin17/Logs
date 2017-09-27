using Logs.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using Ionic.Zip;
using Microsoft.Win32;

namespace Logs.Functions
{
    public class LogFunction
    {
        // Global static field for the log create process problem.
        private static bool logProblem = false;

        /// <summary>
        /// Method for copy the client logs and conf to the temp logs folder
        /// </summary>
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

                    // call this method again after deleting old logs folder
                    CopyLogs(logPath, logCopyTemp, copySubDirs);
                }

            }
            catch (DirectoryNotFoundException ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextError + ex.Message;
            }
            catch (IOException ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextError + ex.Message;
            }
        }

        /// <summary>
        /// Method to create server logs or client logs
        /// </summary>
        public static void CreateLogs(string logPath, string logTempZip, string logName)
        {
            // check if logs already exported
            FileInfo sFile = new FileInfo(logTempZip);
            bool fileExist = sFile.Exists;

            try
            {
                if (!fileExist)
                {
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.AddDirectory(logPath);

                        // Dont show zipping save progress during uploading all logs
                        if (!MainViewModel.IsUploadingAllLogs)
                        {
                            zip.SaveProgress += ZipSaveProgress;
                        }

                        // Set this buffer sizes because there is an bug in the zipping process from Ionic zip
                        // By using the default sizes sometimes it can happen that the zipping process freezes
                        zip.BufferSize = 1000000;
                        zip.CodecBufferSize = 1000000;
                        zip.Save(logTempZip);
                    }

                    // Check if currently client logs are zipped and if the zip process succeeded
                    if (logName == MainViewModel.ClientLogsConfName && File.Exists(MainViewModel.ClientLogsConfTempZip))
                    {
                        MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ClientLogsConfName + MainViewModel.LogTextZipSuccess;
                        MainViewModel.LogText += MainViewModel.ClientLogsConfTempZip;

                        try
                        {
                            if (Directory.Exists(MainViewModel.ClientLogsConfTemp))
                            {
                                Directory.Delete(MainViewModel.ClientLogsConfTemp, true);
                            }
                        }
                        catch (DirectoryNotFoundException)
                        {

                        }
                        catch (IOException)
                        {

                        }

                        MainViewModel.UpdatePropertiesCreateLogsAtEnd(MainViewModel.ClientLogsConfName);
                    }

                    // Check if currently server logs are zipped and if the zip process succeeded
                    else if (File.Exists(MainViewModel.ServerLogsTempZip) && logName == MainViewModel.ServerLogsName)
                    {
                        MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ServerLogsName + MainViewModel.LogTextZipSuccess;
                        MainViewModel.LogText += MainViewModel.ServerLogsTempZip;

                        MainViewModel.UpdatePropertiesCreateLogsAtEnd(MainViewModel.ServerLogsName);
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
                    catch (UnauthorizedAccessException)
                    {

                    }

                    // call this method again after deleting old logs zip file
                    CreateLogs(logPath, logTempZip, logName);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextError + ex.Message;
                MainViewModel.LogText += MainViewModel.LogTextError + MainViewModel.LogTextCouldNotCreate + MainViewModel.ClientLogsConfName.ToLower() + MainViewModel.LogTextZipFolder;
            }
            catch (IOException ex)
            {
                // TODO: If we have not enough space then we have to reset the update the progress bar and the buttons
            }
        }

        private static void ZipSaveProgress(object sender, SaveProgressEventArgs e)
        {
            try
            {
                if (e.EventType == ZipProgressEventType.Saving_BeforeWriteEntry && logProblem == false)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainViewModel.ProgressBarMaximum = e.EntriesTotal;
                        MainViewModel.ProgressBarValue = e.EntriesSaved + 1;
                    });
                }
            }
            catch (NullReferenceException)
            {
                // If user quits programm during the create logs process
                logProblem = true;
            }
        }

        /// <summary>
        /// Method to upload files to the FTP server
        /// </summary>
        public static void UploadLogsFTP(string logPath, string logName)
        {
            FileStream fs = null;
            Stream rs = null;

            try
            {
                FileInfo fileInf = new FileInfo(logPath);
                string file = logPath;
                string uploadFileName = new FileInfo(file).Name;

                // If the file is 
                if (logName != "")
                {
                    uploadFileName = logName + "_" + uploadFileName;
                }
  
                string uploadUrl = "ftp://217.22.207.192/";
                fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                string ftpUrl = string.Format("{0}/{1}", uploadUrl, uploadFileName);
                FtpWebRequest requestObj = FtpWebRequest.Create(ftpUrl) as FtpWebRequest;
                requestObj.Method = WebRequestMethods.Ftp.UploadFile;
                requestObj.Credentials = new NetworkCredential("anonymous", "");
                rs = requestObj.GetRequestStream();

                byte[] buffer = new byte[8192];
                int read = fs.Read(buffer, 0, buffer.Length);

                long transferred = 0;
                MainViewModel.ProgressBarMaximum = fileInf.Length;

                while (read > 0)
                {
                    rs.Write(buffer, 0, read);

                    transferred += read;
                    MainViewModel.ProgressBarValue = transferred;

                    read = fs.Read(buffer, 0, buffer.Length);
                }
                rs.Flush();

            }
            catch (Exception ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextError + MainViewModel.LogTextUploadFailed + ex.Message;
                MainViewModel.IsUploadSucceeded = false;
                MainViewModel.ProgressbarVisibility = Visibility.Hidden;
                MainViewModel.TbProgressText = ex.Message;
                MainViewModel.UpdateFTPButtons();
                MainViewModel.IsUploadingCustomFile = false;
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
                    CreateTempLogsFolder(zipPath);
                }
                // Open log zip folder with windows explorer
                Process.Start(zipPath);
            }
            catch (Win32Exception ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextError + ex.Message;
            }
        }

        /// <summary>
        /// Method to select the path of the custom zip file
        /// </summary>
        public static void SelectCustomFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Zip files|*.zip;*.rar";
            if (openFileDialog.ShowDialog() == true)
            {
                MainViewModel.SelectedCustomFilePath = openFileDialog.FileName;
                MainViewModel.TbSelectedCustomFileName = " " + Path.GetFileName(MainViewModel.SelectedCustomFilePath);
                MainViewModel.CustomZipFile = MainViewModel.SelectedCustomFilePath;
                MainViewModel.IsBtnUploadCustomFileEnabled = true;
                MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogTextCustomZipFileSelected + "\n " + MainViewModel.SelectedCustomFilePath;
                MainViewModel.TbProgressText = MainViewModel.LogTextCustomZipFileSelected;
            }
            
        }

        /// <summary>
        /// Method to create the temp logs folder for the zip files
        /// </summary>
        public static void CreateTempLogsFolder(string tempPath)
        {
            try
            {
                Directory.CreateDirectory(tempPath);
            }
            catch (Exception ex)
            {
                MainViewModel.LogText += MainViewModel.LogTextError + ex.Message;
            }
        }

        /// <summary>
        /// Method to copy created client and server zip to all logs folder
        /// </summary>
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
        /// Method which is called after uploading zip files to the seetec ftp server
        /// First check if client, server or both logs are uploaded
        /// Second check if upload is succeeded. If yes do something, if not set upload succeeded to true again
        /// </summary>
        private static void StartCleaning(string file)
        {
            if (file == MainViewModel.ServerLogsTempZip && !MainViewModel.IsUploadingCustomFile)
            {
                if (MainViewModel.IsUploadSucceeded)
                {
                    MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ServerLogsName + MainViewModel.LogTextZipFolder + MainViewModel.LogTextUploadSucceeded;
                    DeleteFilesFoldersAfterUpload(MainViewModel.ServerLogsTempZip, MainViewModel.ServerLogsCopyTemp);
                    MainViewModel.UpdatePropertiesFTPUpload(MainViewModel.ServerLogsName);
                }             
            }

            else if (file == MainViewModel.ClientLogsConfTempZip && !MainViewModel.IsUploadingCustomFile)
            {
                if (MainViewModel.IsUploadSucceeded)
                {
                    MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ClientLogsConfName + MainViewModel.LogTextZipFolder + MainViewModel.LogTextUploadSucceeded;
                    DeleteFilesFoldersAfterUpload(MainViewModel.ClientLogsConfTempZip, MainViewModel.ClientLogsConfTemp);
                    MainViewModel.UpdatePropertiesFTPUpload(MainViewModel.ClientLogsConfName);
                }
            }

            else if (file == MainViewModel.LogsZipFolderPathZip && !MainViewModel.IsUploadingCustomFile)
            {
                if (MainViewModel.IsUploadSucceeded)
                {
                    MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogZipFolderName + MainViewModel.LogTextZipFolder + MainViewModel.LogTextUploadSucceeded;
                    DeleteFilesFoldersAfterUpload(MainViewModel.ServerLogsTempZip, MainViewModel.ServerLogsCopyTemp);
                    DeleteFilesFoldersAfterUpload(MainViewModel.ClientLogsConfTempZip, MainViewModel.ClientLogsConfTemp);
                    DeleteFilesFoldersAfterUpload(MainViewModel.LogsZipFolderPathZip, MainViewModel.LogsTemp);
                    MainViewModel.UpdatePropertiesFTPUpload(MainViewModel.LogZipFolderName);
                }
                
            }
            else if (file == MainViewModel.CustomZipFile && MainViewModel.IsUploadingCustomFile)
            {
                if (MainViewModel.IsUploadSucceeded)
                {
                    MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogTextCustomFile + MainViewModel.LogTextUploadSucceeded;
                    DeleteCustomZipFileAfterUpload(MainViewModel.CustomZipFile);
                    MainViewModel.UpdatePropertiesFTPUpload(MainViewModel.LogTextCustomFile);
                }           
            }
            MainViewModel.IsUploadSucceeded = true;
            MainViewModel.IsUploadingCustomFile = false;
        }

        /// <summary>
        /// Method to delete files and folders after uploading
        /// </summary>
        private static void DeleteFilesFoldersAfterUpload(string file, string folder)
        {
            try
            {
                File.Delete(file);
                Directory.Delete(folder, true);
            }
            catch (FileNotFoundException)
            {
                //Do nothing. Not important
            }
            catch (DirectoryNotFoundException)
            {
                //Do nothing. Not important
            }
        }

        /// <summary>
        /// Method to delete custom zip files after uploading
        /// </summary>
        private static void DeleteCustomZipFileAfterUpload(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (FileNotFoundException)
            {
                //Do nothing. Not important
            }
        }

        /// <summary>
        /// Method to check internet connection 
        /// </summary>
        public static void CheckInternetConnection()
        {
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send("www.google.com", 200);

                if (reply.Status == IPStatus.Success)
                {
                    MainViewModel.IsInternetConnectionAvailable = true;
                }
                else
                {
                    if (reply.Status == IPStatus.Success)
                    {
                        MainViewModel.IsInternetConnectionAvailable = true;
                    }
                }

            }
            catch
            {
                if (MainViewModel.IsInternetConnectionAvailable)
                {
                    MainViewModel.IsInternetConnectionAvailable = false;
                    MainViewModel.LogText += MainViewModel.LogTextWarning + MainViewModel.LogTextNoInternet;
                    MainViewModel.TbProgressText = MainViewModel.LogTextNoInternet;

                    // No internet connection. Disable FTP buttons
                    MainViewModel.UpdateFTPButtons();
                }
            }
        }

        /// <summary>
        /// Method to check how big the logs folder is
        /// </summary>
        public static void CheckFolderSize(string logPath)
        {
            try
            {
                long length = Directory.GetFiles(logPath, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
                double size = Math.Round((length / 1024d) / 1024d, 2);
                size = Math.Round(size, 2);

                if (size > 1000d)
                {
                    size = Math.Round(size / 1024d, 2);

                    MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogTextFolderSize + size + MainViewModel.LogTextFolderBePatientGB;
                }
                else if (size > 300d)
                {
                    MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogTextFolderSize + size + MainViewModel.LogTextFolderBePatientMB;
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MainViewModel.LogText += "\n " + ex + " Not found";
            }
        }

        /// <summary>
        /// Method to check how big the logs zip file is
        /// </summary>
        public static void CheckZipSize(string zipPath)
        {
            long length = new FileInfo(zipPath).Length;
            double size = Math.Round((length / 1024d) / 1024d, 2);
            MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.LogTextZipSize + size + MainViewModel.LogTextZipBePatientMB;
        }

        /// <summary>
        /// Methos to check if logs location is not available
        /// </summary>
        public static void CheckLogsAvailabilty()
        {
            if (!MainViewModel.IsBtnClientLogsConfEnabled)
            {
                MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ClientLogsConfName + MainViewModel.LogTextLogsNotAvailabe;
            }

            if (!MainViewModel.IsBtnServerLogsEnabled)
            {
                MainViewModel.LogText += MainViewModel.LogTextInfo + MainViewModel.ServerLogsName + MainViewModel.LogTextLogsNotAvailabe;
            }
        }
    }

}
