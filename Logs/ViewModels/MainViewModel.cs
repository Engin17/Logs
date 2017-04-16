using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;
using System.IO;
using Logs.Functions;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Threading;
using System.Net.NetworkInformation;

namespace Logs.ViewModels
{
    /// <summary>
    /// This is our MainViewModel that is tied to the MainWindow
    /// </summary>
    public class MainViewModel
    {
        #region Member variables
        private static string path = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
        private static string path2 = Directory.GetCurrentDirectory();
        private static readonly string serverLogsPath = @"C:\Program Files\SeeTec\log2";
        private static readonly string _serverLogsTempZip = @"C:\Program Files\SeeTec\TempLogs\ServerLog.zip";
        private static readonly string _serverLogsCopyTemp = @"C:\Program Files\SeeTec\TempLogs\ServerLog";
        private static readonly string _serverLogsName = "Server logs";

        private static readonly string clientLogsPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\log");
        private static readonly string clientConfPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\conf");
        private static readonly string _clientLogsConfTempZip = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf.zip";
        private static readonly string clientLogCopyTemp = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf\ClientLogs";
        private static readonly string clientConfCopyTemp = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf\ClientConf";
        private static readonly string _clientLogsConfTemp = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf";
        private static readonly string _clientLogsConfName = "Client logs and configuration";

        private static readonly string logsZipPath = @"C:\Program Files\SeeTec\TempLogs";
        private static readonly string logsClientConfCopyTemp = @"C:\Program Files\SeeTec\TempLogs\AllLog\ClientLogsConf";
        private static readonly string logsServerCopyTemp = @"C:\Program Files\SeeTec\TempLogs\AllLog\ServerLogs";
        private static readonly string _logsZipFolderPathZip = @"C:\Program Files\SeeTec\TempLogs\AllLog.zip";
        private static readonly string _logsTemp = @"C:\Program Files\SeeTec\TempLogs\AllLog";
        private static readonly string _logZipFolderName = "Server, client logs and configuration";

        private static bool _isUploadSucceeded = true;

        private static bool _isClientLogsConfCreated = false;
        private static bool _isServerlogsCreated = false;

        private static bool _isBtnClientFTPEnabled = false;
        private static bool _isBtnServerFTPEnabled = false;
        private static bool _isBtnUploadAllFTPEnabled = false;
        private static bool _isBtnClientLogsConfEnabled = true;
        private static bool _isBtnServerLogsEnabled = true;

        private static string _logText = "Welcome!!! \n";
        private static readonly string _logTextError = "\n [" + DateTime.Now + "] ERROR: ";
        private static readonly string _logTextInfo = "\n [" + DateTime.Now + "] INFO: ";
        private static readonly string _logTextZipSuccess = " zip folder successfully zipped";
        private static readonly string _logTextZipLocation = " zip folder is under: \n ";
        private static readonly string _logTextCouldNotCreate = " Could not create ";
        private static readonly string _logTextZipFolder = " zip folder";
        private static readonly string _logTextUploadFailed = "File upload Failed.\r\n Error Message: ";
        private static readonly string _logTextUploadSucceeded = " zip folder successfully uploaded to the SeeTec FTP server";

        private static string _tbProgressText = "";
        private static readonly string progressTextCreateClientfiles = "Please wait... \ncreating client files zip folder";
        private static readonly string progressTextCreateServerfiles = "Please wait... \ncreating server files zip folder";
        private static readonly string progressTextUploadClientfiles = "Please wait... \nuploading client files to the FTP server";
        private static readonly string progressTextUploadServerfiles = "Please wait... \nuploading server files to the FTP server";
        private static readonly string progressTextUploadAllfiles = "Please wait... \nuploading all files to the FTP server";

        private static string _tbServerLogsName = "HD-";
        private static string _tbClientLogsConfName = "HD-";
        private static string _tbAllLogsName = "HD-";

        private static Visibility _tbProgressTextVisibility = Visibility.Hidden;
        private static Visibility _progressbarVisibility = Visibility.Hidden;
        #endregion

        #region Property members

        public static bool IsUploadSucceeded
        {
            get { return _isUploadSucceeded; }
            set
            {
                _isUploadSucceeded = value;
                RaiseStaticPropertyChanged();
            }
        }
        public static bool IsClientLogsConfCreated
        {
            get { return _isClientLogsConfCreated; }
            set
            {
                _isClientLogsConfCreated = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsServerlogsCreated
        {
            get { return _isServerlogsCreated; }
            set
            {
                _isServerlogsCreated = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsBtnClientFTPEnabled
        {
            get { return _isBtnClientFTPEnabled; }
            set
            {
                if (File.Exists(ClientLogsConfTempZip) && CheckInternetConnection())
                {
                    _isBtnClientFTPEnabled = value;
                }
                else
                {
                    _isBtnClientFTPEnabled = false;
                }
                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsBtnServerFTPEnabled
        {
            get { return _isBtnServerFTPEnabled; }
            set
            {
                if (File.Exists(ServerLogsTempZip))
                {
                    _isBtnServerFTPEnabled = value;
                }
                else
                {
                    _isBtnServerFTPEnabled = false;
                }
                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsBtnUploadAllFTPEnabled
        {
            get { return _isBtnUploadAllFTPEnabled; }
            set
            {
                if (IsBtnClientFTPEnabled && IsBtnServerFTPEnabled && value == true)
                {
                    _isBtnUploadAllFTPEnabled = value;
                }
                else
                {
                    _isBtnUploadAllFTPEnabled = false;
                }
                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsBtnClientLogsConfEnabled
        {
            get { return _isBtnClientLogsConfEnabled; }
            set
            {
                _isBtnClientLogsConfEnabled = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsBtnServerLogsEnabled
        {
            get { return _isBtnServerLogsEnabled; }
            set
            {
                _isBtnServerLogsEnabled = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static string ServerLogsName
        {
            get { return _serverLogsName; }
        }

        public static string ClientLogsConfName
        {
            get { return _clientLogsConfName; }
        }

        public static string LogZipFolderName
        {
            get { return _logZipFolderName; }
        }

        public static string ServerLogsTempZip
        {
            get { return _serverLogsTempZip; }
        }

        public static string ServerLogsCopyTemp
        {
            get { return _serverLogsCopyTemp; }
        }

        public static string ClientLogsConfTempZip
        {
            get { return _clientLogsConfTempZip; }
        }

        public static string LogsZipFolderPathZip
        {
            get { return _logsZipFolderPathZip; }
        }

        public static string LogsTemp
        {
            get { return _logsTemp; }
        }

        public static string ClientLogsConfTemp
        {
            get { return _clientLogsConfTemp; }
        }

        #region Logs Properties
        public static string LogText
        {
            get { return _logText; }
            set
            {
                _logText = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static string LogTextError
        {
            get { return _logTextError; }
        }

        public static string LogTextInfo
        {
            get { return _logTextInfo; }
        }

        public static string LogTextZipSuccess
        {
            get { return _logTextZipSuccess; }
        }

        public static string LogTextZipLocation
        {
            get { return _logTextZipLocation; }
        }

        public static string LogTextCouldNotCreate
        {
            get { return _logTextCouldNotCreate; }
        }

        public static string LogTextZipFolder
        {
            get { return _logTextZipFolder; }
        }

        public static string LogTextUploadFailed
        {
            get { return _logTextUploadFailed; }
        }

        public static string LogTextUploadSucceeded
        {
            get { return _logTextUploadSucceeded; }
        }
        #endregion

        public static string TbServerLogsName
        {
            get { return _tbServerLogsName; }
            set
            {
                _tbServerLogsName = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static string TbClientLogsConfName
        {
            get { return _tbClientLogsConfName; }
            set
            {
                _tbClientLogsConfName = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static string TbAllLogsName
        {
            get { return _tbAllLogsName; }
            set
            {
                _tbAllLogsName = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static string TbProgressText
        {
            get { return _tbProgressText; }
            set
            {
                _tbProgressText = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static Visibility TbProgressTextVisibility
        {
            get { return _tbProgressTextVisibility; }
            set
            {
                _tbProgressTextVisibility = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static Visibility ProgressbarVisibility
        {
            get { return _progressbarVisibility; }
            set
            {
                _progressbarVisibility = value;
                RaiseStaticPropertyChanged();
            }
        }
        #endregion

        #region ICommand properties
        /// <summary>
        /// Simple property to hold the 'ExportClientLogsConfCommand' - when executed
        /// it will export client logs and client conf to the temp folder and after 
        /// copying it will zip both folders
        /// </summary>
        public ICommand ExportClientLogsConfCommand { get; private set; }

        /// <summary>
        /// Simple property to hold the 'ExportServerLogsCommand' - when executed
        /// it will export server logs to the temp folder and after 
        /// copying it will zip this folders
        /// </summary>
        public ICommand ExportServerLogsCommand { get; private set; }

        /// <summary>
        /// Simple property to hold the 'UploadClientFilesFTPCommand' - when executed
        /// it will upload client files to the FTP server
        /// </summary>
        public ICommand UploadClientFilesFTPCommand { get; private set; }

        /// <summary>
        /// Simple property to hold the 'UploadServerFilesFTPCommand' - when executed
        /// it will upload server files to the FTP server
        /// </summary>
        public ICommand UploadServerFilesFTPCommand { get; private set; }

        /// <summary>
        /// Simple property to hold the 'OpenLogsPathCommand' - when executed
        /// it will open the created logs zip folder path
        /// </summary>
        public ICommand OpenLogsPathCommand { get; private set; }

        /// <summary>
        /// Simple property to hold the 'OpenLogsPathCommand' - when executed
        /// it will open the created logs zip folder path
        /// </summary>
        public ICommand UploadAllFilesFTPCommand { get; private set; }
        #endregion

        public MainViewModel()
        {
            ExportClientLogsConfCommand = new RelayCommand(() => ExecuteExportClientLogsConfCommand());
            ExportServerLogsCommand = new RelayCommand(() => ExecuteExportServerLogsCommand());
            UploadClientFilesFTPCommand = new RelayCommand(() => ExecuteUploadClientFilesFTPCommand());
            UploadServerFilesFTPCommand = new RelayCommand(() => ExecuteUploadServerFilesFTPCommand());
            OpenLogsPathCommand = new RelayCommand(() => ExecuteOpenLogsPathCommand());
            UploadAllFilesFTPCommand = new RelayCommand(() => ExecuteUploadAllFilesFTPCommand());

            // Enable upload buttons if zip file already exists
            IsBtnClientFTPEnabled = true;
            IsBtnServerFTPEnabled = true;
            IsBtnUploadAllFTPEnabled = true;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }

        /// <summary>
        /// Execute ExportClientLogsConfCommand
        /// Starts the method to zip clint logs and conf with other thread
        /// </summary>
        private void ExecuteExportClientLogsConfCommand()
        {
            IsBtnClientLogsConfEnabled = false;
            UpdatePropertiesCreateLogsAtStart(progressTextCreateClientfiles);

            Thread t = new Thread(new ThreadStart(StartCreatingClientLogsConf));
            t.Start();
        }

        /// <summary>
        /// Execute ExportServerLogsCommand
        /// Starts the method to zip server logs with other thread
        /// </summary>
        private void ExecuteExportServerLogsCommand()
        {
            IsBtnServerLogsEnabled = false;
            UpdatePropertiesCreateLogsAtStart(progressTextCreateServerfiles);

            Thread t = new Thread(new ThreadStart(StartCreatingServerLogs));
            t.Start();
        }

        /// <summary>
        /// Execute UploadClientFilesFTPCommand
        /// </summary>
        private void ExecuteUploadClientFilesFTPCommand()
        {
            UpdatePropertiesUploadLogs(progressTextUploadClientfiles);

            Thread t = new Thread(new ThreadStart(StartUploadingClientFilesFTP));
            t.Start();
        }

        /// <summary>
        /// Execute UploadServerFilesFTPCommand
        /// </summary>
        private void ExecuteUploadServerFilesFTPCommand()
        {
            UpdatePropertiesUploadLogs(progressTextUploadServerfiles);

            Thread t = new Thread(new ThreadStart(StartUploadingServerFilesFTP));
            t.Start();
        }

        /// <summary>
        /// Execute UploadAllFilesFTPCommand
        /// </summary>
        private void ExecuteUploadAllFilesFTPCommand()
        {
            UpdatePropertiesUploadLogs(progressTextUploadAllfiles);

            Thread t = new Thread(new ThreadStart(StartUploadingAllFilesFTP));
            t.Start();
        }

        /// <summary>
        /// Execute OpenLogsPathCommand
        /// </summary>
        private void ExecuteOpenLogsPathCommand()
        {
            LogFunction.OpenLogsPath(logsZipPath);
        }

        /// <summary>
        /// Method to zip server logs
        /// First copy the server logs to a specified directory
        /// Second zip the copied logs
        /// At last enable create server logs button
        /// </summary>
        private void StartCreatingServerLogs()
        {
            LogFunction.CopyLogs(serverLogsPath, ServerLogsCopyTemp, true);

            LogFunction.CreateLogs(ServerLogsCopyTemp, ServerLogsTempZip, ServerLogsName);

            IsBtnServerLogsEnabled = true;
        }

        /// <summary>
        /// Method to zip Client logs and conf
        /// First copy the client logs to a specified directory
        /// Second copy the client conf to a specified directory
        /// Third zip the copied logs and conf
        /// At last enable create client logs and conf button
        /// </summary>
        private void StartCreatingClientLogsConf()
        {
            LogFunction.CopyLogs(clientLogsPath, clientLogCopyTemp, true);
            LogFunction.CopyLogs(clientConfPath, clientConfCopyTemp, true);

            LogFunction.CreateLogs(ClientLogsConfTemp, ClientLogsConfTempZip, ClientLogsConfName);

            IsBtnClientLogsConfEnabled = true;
        }

        /// <summary>
        /// Method to upload client files to the FTP server
        /// </summary>
        private void StartUploadingClientFilesFTP()
        {
            LogFunction.UploadLogsFTP(ClientLogsConfTempZip, TbClientLogsConfName);

            UpdateCreateLogsButtonProperties();
        }

        /// <summary>
        /// Method to upload server files to the FTP server
        /// </summary>
        private void StartUploadingServerFilesFTP()
        {
            LogFunction.UploadLogsFTP(ServerLogsTempZip, TbServerLogsName);

            UpdateCreateLogsButtonProperties();
        }

        /// <summary>
        /// Method to upload all files to the FTP server
        /// </summary>
        private void StartUploadingAllFilesFTP()
        {
            LogFunction.CopyLogs(_clientLogsConfTemp, logsClientConfCopyTemp, true);
            LogFunction.CopyLogs(_serverLogsCopyTemp, logsServerCopyTemp, true);

            LogFunction.CreateLogs(LogsTemp, LogsZipFolderPathZip, LogZipFolderName);

            LogFunction.UploadLogsFTP(LogsZipFolderPathZip, TbAllLogsName);

            UpdateCreateLogsButtonProperties();
        }

        #region Update properties methods
        private void UpdatePropertiesCreateLogsAtStart(string progressText)
        {
            ProgressbarVisibility = Visibility.Visible;
            TbProgressText = progressText;
            TbProgressTextVisibility = Visibility.Visible;

            IsBtnClientFTPEnabled = true;
            IsBtnServerFTPEnabled = true;
            IsBtnUploadAllFTPEnabled = true;
        }

        public static void UpdatePropertiesCreateLogsAtEnd()
        {
            IsBtnServerFTPEnabled = true;
            IsBtnClientFTPEnabled = true;
            IsBtnUploadAllFTPEnabled = true;
            Thread.Sleep(500);
            ProgressbarVisibility = Visibility.Hidden;
            TbProgressTextVisibility = Visibility.Hidden;
        }

        private void UpdatePropertiesUploadLogs(string progressText)
        {
            ProgressbarVisibility = Visibility.Visible;
            TbProgressText = progressText;
            TbProgressTextVisibility = Visibility.Visible;
            IsBtnServerLogsEnabled = false;
            IsBtnClientLogsConfEnabled = false;
            IsBtnClientFTPEnabled = false;
            IsBtnServerFTPEnabled = false;
            IsBtnUploadAllFTPEnabled = false;
        }

        private void UpdateCreateLogsButtonProperties()
        {
            IsBtnServerLogsEnabled = true;
            IsBtnClientLogsConfEnabled = true;
        }

        public static void UpdateFTPUploadButons()
        {
            IsBtnClientFTPEnabled = true;
            IsBtnServerFTPEnabled = true;
            IsBtnUploadAllFTPEnabled = true;
        }
        #endregion

        /// <summary>
        /// Method to check internet connection 
        /// </summary>
        public static bool CheckInternetConnection()
        {
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send("www.google.de", 100);

                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        // This method is called by the set accessor of static properties.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged([CallerMemberName] string propertyName = null)
        {
            EventHandler<PropertyChangedEventArgs> handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Method to delete copied temporary log folders before exiting application exit
        /// </summary>
        private static void OnProcessExit(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(_clientLogsConfTemp))
                {
                    Directory.Delete(_clientLogsConfTemp, true);
                }

                if (Directory.Exists(_serverLogsCopyTemp))
                {
                    Directory.Delete(_serverLogsCopyTemp, true);
                }

                if (Directory.Exists(_logsTemp))
                {
                    Directory.Delete(_logsTemp, true);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                LogText += "\n [" + DateTime.Now + "] ERROR: " + ex.Message;
            }
        }
    }
}