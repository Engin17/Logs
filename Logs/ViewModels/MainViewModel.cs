using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;
using System.IO;
using Logs.Functions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Threading;


namespace Logs.ViewModels
{
    /// <summary>
    /// This is our MainViewModel that is tied to the MainWindow
    /// </summary>
    public class MainViewModel
    {
        #region Member variables
        private static string seeTecInstallPath = Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString();

        //private static string _serverLogsPath = seeTecInstallPath + @"\log";
        private static readonly string _serverLogsPath = @"C:\Program Files\SeeTec\log";

        //private static readonly string _serverLogsTempZip = seeTecInstallPath + @"\TempLogs\ServerLog.zip";
        private static readonly string _serverLogsTempZip = @"C:\Program Files\SeeTec\TempLogs\ServerLog.zip";

        //private static readonly string _serverLogsCopyTemp = seeTecInstallPath + @"\TempLogs\ServerLog";
        private static readonly string _serverLogsCopyTemp = @"C:\Program Files\SeeTec\TempLogs\ServerLog";

        private static readonly string _serverLogsName = "Server logs";


        private static readonly string _clientLogsPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\log");
        private static readonly string clientConfPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\conf");

        private static readonly string _clientLogsConfTempZip = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf.zip";
        //private static readonly string _clientLogsConfTempZip = seeTecInstallPath + @"\TempLogs\ClientLogsConf.zip";

        private static readonly string clientLogCopyTemp = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf\ClientLogs";
        //private static readonly string clientLogCopyTemp = seeTecInstallPath + @"\TempLogs\ClientLogsConf\ClientLogs";

        private static readonly string clientConfCopyTemp = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf\ClientConf";
        //private static readonly string clientConfCopyTemp = seeTecInstallPath + @"\TempLogs\ClientLogsConf\ClientConf";

        private static readonly string _clientLogsConfTemp = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf";
        //private static readonly string _clientLogsConfTemp = seeTecInstallPath + @"\TempLogs\ClientLogsConf";

        private static readonly string _clientLogsConfName = "Client logs and configuration";

        private static readonly string logsZipPath = @"C:\Program Files\SeeTec\TempLogs";
        //private static readonly string logsZipPath = seeTecInstallPath + @"\TempLogs";

        private static readonly string _logsClientConfCopyTempZip = @"C:\Program Files\SeeTec\TempLogs\AllLog\ClientLogsConf.zip";
        //private static readonly string _logsClientConfCopyTempZip = seeTecInstallPath + @"\TempLogs\AllLog\ClientLogsConf.zip";

        private static readonly string _logsServerCopyTempZip = @"C:\Program Files\SeeTec\TempLogs\AllLog\ServerLog.zip";
        //private static readonly string _logsServerCopyTempZip = seeTecInstallPath + @"\TempLogs\AllLog\ServerLog.zip";

        private static readonly string _logsZipFolderPathZip = @"C:\Program Files\SeeTec\TempLogs\AllLog.zip";
        //private static readonly string _logsZipFolderPathZip = seeTecInstallPath + @"\TempLogs\AllLog.zip";

        private static readonly string _logsTemp = @"C:\Program Files\SeeTec\TempLogs\AllLog";
        //private static readonly string _logsTemp = seeTecInstallPath + @"\TempLogs\AllLog";

        private static readonly string _logZipFolderName = "Server, client logs and configuration";

        private static bool _isUploadSucceeded = true;

        private static bool _isUploadingAllLogs = false;

        private static bool _isClientLogsConfCreated = false;
        private static bool _isServerlogsCreated = false;

        private static bool _isBtnClientFTPEnabled = false;
        private static bool _isBtnServerFTPEnabled = false;
        private static bool _isBtnUploadAllFTPEnabled = false;
        private static bool _isBtnClientLogsConfEnabled = false;
        private static bool _isBtnServerLogsEnabled = false;

        private static bool _isInternetConnectionAvailable = true;

        private static string _logText = "Welcome!!! \n";
        private static readonly string _logTextError = "\n [" + DateTime.Now + "] ERROR: ";
        private static readonly string _logTextInfo = "\n [" + DateTime.Now + "] INFO: ";
        private static readonly string _logTextZipSuccess = " zip folder successfully zipped \n ";
        private static readonly string _logTextZipLocation = " zip folder is under: \n ";
        private static readonly string _logTextCouldNotCreate = " Could not create ";
        private static readonly string _logTextZipFolder = " zip folder";
        private static readonly string _logTextUploadFailed = "File upload Failed.\r\n Error Message: ";
        private static readonly string _logTextUploadSucceeded = " zip folder successfully uploaded to the SeeTec FTP server. \n Please inform support staff that the logs are uploaded to the FTP server";
        private static readonly string _logTextUploadSuccess = " zip folder successfully uploaded";
        private static readonly string _logTextNoInternet = "No internet connection available. FTP upload disabled";
        private static readonly string _logTextFolderSize = "The log folder size is about ";
        private static readonly string _logTextFolderBePatientMB = " MB. \n Creating log zip folder may take some time. Please be patient.";
        private static readonly string _logTextFolderBePatientGB = " GB. \n Creating log zip folder may take some time. Please be patient.";
        private static readonly string _logTextZipSize = "The zip folder size is about ";
        private static readonly string _logTextZipBePatientMB = " MB. \n Depending on the internet speed and the size of the zip folder the upload may take some time. \n Please be patient and wait until the logs are uploaded to the FTP server";
        private static readonly string _logTextLogsNotAvailabe = " are not available. Logs zip folder cannot be created.";

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
        private static long _progressBarMaximum = 100;
        private static long _progressBarValue = 0;

        // FileSystemWatcher variable. Watches whether a created zip file was deleted oder renamed
        FileSystemWatcher watcher;
        #endregion

        #region Property members

        public static bool IsUploadSucceeded
        {
            get { return _isUploadSucceeded; }
            set
            {
                _isUploadSucceeded = value;
            }
        }

        public static bool IsUploadingAllLogs
        {
            get { return _isUploadingAllLogs; }
            set
            {
                _isUploadingAllLogs = value;
            }
        }
        public static bool IsClientLogsConfCreated
        {
            get { return _isClientLogsConfCreated; }
            set
            {
                _isClientLogsConfCreated = value;
            }
        }

        public static bool IsServerlogsCreated
        {
            get { return _isServerlogsCreated; }
            set
            {
                _isServerlogsCreated = value;
            }
        }

        public static bool IsBtnClientFTPEnabled
        {
            get { return _isBtnClientFTPEnabled; }
            set
            {
                if (File.Exists(ClientLogsConfTempZip) && IsInternetConnectionAvailable)
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
                if (File.Exists(ServerLogsTempZip) && IsInternetConnectionAvailable)
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
                if (value == true && Directory.Exists(ClientLogsPath) && Directory.Exists(clientConfPath))
                {
                    _isBtnClientLogsConfEnabled = value;
                }
                else
                {
                    _isBtnClientLogsConfEnabled = false;
                }
                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsBtnServerLogsEnabled
        {
            get { return _isBtnServerLogsEnabled; }
            set
            {
                if (value == true && Directory.Exists(ServerLogsPath))
                {
                    _isBtnServerLogsEnabled = value;
                }
                else
                {
                    _isBtnServerLogsEnabled = false;
                }
                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsInternetConnectionAvailable
        {
            get { return _isInternetConnectionAvailable; }
            set
            {
                _isInternetConnectionAvailable = value;
            }
        }

        public static string ClientLogsPath
        {
            get { return _clientLogsPath; }
        }

        public static string ServerLogsPath
        {
            get { return _serverLogsPath; }
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

        public static string LogsClientConfCopyTempZip
        {
            get { return _logsClientConfCopyTempZip; }
        }

        public static string LogsServerCopyTempZip
        {
            get { return _logsServerCopyTempZip; }
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

        public static string LogTextUploadSuccess
        {
            get { return _logTextUploadSuccess; }
        }

        public static string LogTextNoInternet
        {
            get { return _logTextNoInternet; }
        }

        public static string LogTextFolderSize
        {
            get { return _logTextFolderSize; }
        }

        public static string LogTextFolderBePatientMB
        {
            get { return _logTextFolderBePatientMB; }
        }

        public static string LogTextFolderBePatientGB
        {
            get { return _logTextFolderBePatientGB; }
        }

        public static string LogTextZipSize
        {
            get { return _logTextZipSize; }
        }

        public static string LogTextZipBePatientMB
        {
            get { return _logTextZipBePatientMB; }
        }

        public static string LogTextLogsNotAvailabe
        {
            get { return _logTextLogsNotAvailabe; }
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

        public static long ProgressBarMaximum
        {
            get { return _progressBarMaximum; }
            set
            {
                _progressBarMaximum = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static long ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
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

            if (!LogFunction.CheckInternetConnection())
            {
                IsInternetConnectionAvailable = false;
                LogText += LogTextInfo + LogTextNoInternet;
            }

            // Enable upload buttons if zip file already exists
            IsBtnClientFTPEnabled = true;
            IsBtnServerFTPEnabled = true;
            IsBtnUploadAllFTPEnabled = true;

            // Enable create logs buttons if the log directorys exists
            IsBtnClientLogsConfEnabled = true;
            IsBtnServerLogsEnabled = true;

            if (!IsBtnClientLogsConfEnabled)
            {
                LogFunction.LogsNotAvailable(ClientLogsConfName);
            }
            if (!IsBtnServerLogsEnabled)
            {
                LogFunction.LogsNotAvailable(ServerLogsName);
            }

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            FSWInitialize();
        }

        /// <summary>
        /// Execute ExportClientLogsConfCommand
        /// Starts the method to zip clint logs and conf with other thread
        /// </summary>
        private void ExecuteExportClientLogsConfCommand()
        {
            UpdatePropertiesCreateLogsAtStart(progressTextCreateClientfiles);

            Thread t = new Thread(new ThreadStart(StartCreatingClientLogsConfLogs));
            t.Start();
        }

        /// <summary>
        /// Execute ExportServerLogsCommand
        /// Starts the method to zip server logs with other thread
        /// </summary>
        private void ExecuteExportServerLogsCommand()
        {
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
            LogFunction.CheckFolderSize(ServerLogsPath);
            LogFunction.CopyLogs(ServerLogsPath, ServerLogsCopyTemp, true);
            IsUploadingAllLogs = false;
            LogFunction.CreateLogs(ServerLogsCopyTemp, ServerLogsTempZip, ServerLogsName);
        }

        /// <summary>
        /// Method to zip Client logs and conf
        /// First copy the client logs to a specified directory
        /// Second copy the client conf to a specified directory
        /// Third zip the copied logs and conf
        /// At last enable create client logs and conf button
        /// </summary>
        private void StartCreatingClientLogsConfLogs()
        {
            LogFunction.CheckFolderSize(ClientLogsPath);
            LogFunction.CopyLogs(ClientLogsPath, clientLogCopyTemp, true);
            LogFunction.CopyLogs(clientConfPath, clientConfCopyTemp, true);
            IsUploadingAllLogs = false;
            LogFunction.CreateLogs(ClientLogsConfTemp, ClientLogsConfTempZip, ClientLogsConfName);
        }

        /// <summary>
        /// Method to upload client files to the FTP server
        /// </summary>
        private void StartUploadingClientFilesFTP()
        {
            LogFunction.CheckZipSize(ClientLogsConfTempZip);
            LogFunction.UploadLogsFTP(ClientLogsConfTempZip, TbClientLogsConfName);

            UpdateCreateLogsButtonProperties();
        }

        /// <summary>
        /// Method to upload server files to the FTP server
        /// </summary>
        private void StartUploadingServerFilesFTP()
        {
            LogFunction.CheckZipSize(ServerLogsTempZip);
            LogFunction.UploadLogsFTP(ServerLogsTempZip, TbServerLogsName);

            UpdateCreateLogsButtonProperties();
        }

        /// <summary>
        /// Method to upload all files to the FTP server
        /// </summary>
        private void StartUploadingAllFilesFTP()
        {
            IsUploadingAllLogs = true;
            LogFunction.CopyClientServerZipFolder();
            LogFunction.CreateLogs(LogsTemp, LogsZipFolderPathZip, LogZipFolderName);

            LogFunction.CheckZipSize(LogsZipFolderPathZip);
            LogFunction.UploadLogsFTP(LogsZipFolderPathZip, TbAllLogsName);

            UpdateCreateLogsButtonProperties();
        }

        #region Update properties members
        private void UpdatePropertiesCreateLogsAtStart(string progressText)
        {
            ProgressBarValue = 0;
            IsBtnClientFTPEnabled = false;
            IsBtnServerFTPEnabled = false;
            IsBtnUploadAllFTPEnabled = false;
            IsBtnServerLogsEnabled = false;
            IsBtnClientLogsConfEnabled = false;
            ProgressbarVisibility = Visibility.Visible;
            TbProgressText = progressText;
            TbProgressTextVisibility = Visibility.Visible;
        }

        public static void UpdatePropertiesCreateLogsAtEnd(string logZipName)
        {
            TbProgressText = logZipName + "\n" + LogTextZipSuccess;
            IsBtnServerFTPEnabled = true;
            IsBtnClientFTPEnabled = true;
            IsBtnUploadAllFTPEnabled = true;
            IsBtnServerLogsEnabled = true;
            IsBtnClientLogsConfEnabled = true;
            ProgressbarVisibility = Visibility.Hidden;
            ProgressBarValue = 0;
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

        public static void UpdateFTPUploadButtons(string logZipName)
        {
            ProgressbarVisibility = Visibility.Hidden;
            TbProgressText = logZipName + "\n" + LogTextUploadSuccess;
            IsBtnClientFTPEnabled = true;
            IsBtnServerFTPEnabled = true;
            IsBtnUploadAllFTPEnabled = true;
        }

        private static void UpdateFTPButtonsAfterDeleteZipFile()
        {
            IsBtnClientFTPEnabled = true;
            IsBtnServerFTPEnabled = true;
            IsBtnUploadAllFTPEnabled = true;
        }
        #endregion

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
        /// Method to delete copied temporary log folders and files before exiting application
        /// </summary>
        private static void OnProcessExit(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(ClientLogsConfTemp))
                {
                    Directory.Delete(ClientLogsConfTemp, true);
                }

                if (Directory.Exists(ServerLogsCopyTemp))
                {
                    Directory.Delete(ServerLogsCopyTemp, true);
                }

                if (Directory.Exists(LogsTemp))
                {
                    Directory.Delete(LogsTemp, true);
                }

                foreach (string sFile in Directory.GetFiles(logsZipPath, "*.tmp"))
                {
                    File.Delete(sFile);
                }
            }
            catch (DirectoryNotFoundException)
            {

            }
        }

        #region FileSystemWatcher Members
        /// <summary>
        /// Listens to the file system change notifications and raises events when a zip file in a directory, changes.
        /// </summary>
        private void FSWInitialize()
        {
            // If temp folder doesnt exist then we have to create the temp folder for the zip files 
            if (!Directory.Exists(logsZipPath))
            {
                LogFunction.CreateTempLogsFolder(logsZipPath);
            }

            if (Directory.Exists(logsZipPath))
            {
                // Create a new FileSystemWatcher and set its properties.
                watcher = new FileSystemWatcher();

                // Set path and filter. Only watch zip files
                watcher.Path = logsZipPath;
                watcher.Filter = "*.zip";

                // Add event handlers.
                watcher.Deleted += new FileSystemEventHandler(FSW_Deleted);
                watcher.Renamed += new RenamedEventHandler(FSW_Renamed);

                // Begin watching.
                watcher.EnableRaisingEvents = true;
            }
        }

        // Define the event handlers.
        void FSW_Renamed(object sender, RenamedEventArgs e)
        {
            // Specify what is done when a zip file is deleted.
            UpdateFTPButtonsAfterDeleteZipFile();
        }

        void FSW_Deleted(object sender, FileSystemEventArgs e)
        {
            // Specify what is done when a zip file is renamed.
            UpdateFTPButtonsAfterDeleteZipFile();
        }
        #endregion
    }
}