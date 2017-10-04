using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;
using System.IO;
using Logs.Functions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Threading;
using System.Linq;


namespace Logs.ViewModels
{
    /// <summary>
    /// This is our MainViewModel that is tied to the MainWindow.
    /// </summary>
    public class MainViewModel
    {
        #region Member variables
        private static string _seeTecInstallPath = "";

        private static string _serverLogsPath = "";

        private static string _serverLogsTempZip = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\Templogs\ServerLogs.zip");

        private static string _serverLogsCopyTemp = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\Templogs\ServerLogs");


        private static readonly string _serverLogsName = "Server logs";

        private static readonly string _clientLogsPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\log");
        private static readonly string clientConfPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\conf");

        private static string _clientLogsConfTempZip = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\Templogs\ClientLogsConf.zip");

        private static string _clientLogCopyTemp = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\Templogs\ClientLogsConf\ClientLogs");

        private static string _clientConfCopyTemp = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\Templogs\ClientLogsConf\ClientConf");

        private static string _clientLogsConfTemp = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\Templogs\ClientLogsConf");

        private static readonly string _clientLogsConfName = "Client logs and configuration";

        private static string _logsZipPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\Templogs");

        private static string _logsClientConfCopyTempZip = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\Templogs\AllLog\ClientLogsConf.zip");

        private static string _logsServerCopyTempZip = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\Templogs\AllLog\ServerLogs.zip");

        private static string _logsZipFolderPathZip = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\Templogs\AllLog\AllLogs.zip");

        private static string _logsTemp = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\Templogs\AllLog");

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
        private static bool _isBtnSelectCustomFileEnabled = true;
        private static bool _isBtnUploadCustomFileEnabled = false;

        private static bool _isInternetConnectionAvailable = true;

        private static string _logText = "Collect and upload logs!!!";
        private static readonly string _logTextError = "\n\n [" + DateTime.Now + "] ERROR: ";
        private static readonly string _logTextInfo = "\n\n [" + DateTime.Now + "] INFO: ";
        private static readonly string _logTextWarning = "\n\n [" + DateTime.Now + "] WARNING: ";
        private static readonly string _logTextZipSuccess = " log folder successfully zipped \n ";
        private static readonly string _logTextZipLocation = " log folder is under: \n ";
        private static readonly string _logTextCouldNotCreate = " Could not create ";
        private static readonly string _logTextZipFolder = " zip file";
        private static readonly string _logTextUploadFailed = "File upload Failed.\r\n Error Message: ";
        private static readonly string _logTextUploadSucceeded = " successfully uploaded to the SeeTec FTP server. \n Please inform support staff that the logs are uploaded to the FTP server";
        private static readonly string _logTextUploadSuccess = " successfully uploaded to the SeeTec FTP server.";
        private static readonly string _logTextNoInternet = "No internet connection available. FTP upload disabled.";
        private static readonly string _logTextFolderSize = "The log folder size is about ";
        private static readonly string _logTextFolderBePatientMB = " MB. \n Creating log zip file may take some time. Please be patient.";
        private static readonly string _logTextFolderBePatientGB = " GB. \n Creating log zip file may take some time. Please be patient.";
        private static readonly string _logTextZipSize = "The zip folder size is about ";
        private static readonly string _logTextZipBePatientMB = " MB. \n Depending on the internet speed and the size of the zip file the upload may take some time. \n Please be patient and wait until the file is uploaded to the FTP server";
        private static readonly string _logTextLogsNotAvailabe = " are not available. Logs zip file cannot be created.";

        private static readonly string _logTextCustomZipFileSelected = "Custom zip file selected.";
        private static readonly string _logTextCustomFile = "Custom zip";

        private static string _tbProgressText = "";
        private static readonly string progressTextCreateClientfiles = "Please wait... \ncreating client zip file";
        private static readonly string progressTextCreateServerfiles = "Please wait... \ncreating server zip file";
        private static readonly string progressTextUploadClientfiles = "Please wait... \nuploading client file to the FTP server";
        private static readonly string progressTextUploadServerfiles = "Please wait... \nuploading server file to the FTP server";
        private static readonly string progressTextUploadAllfiles = "Please wait... \nuploading all files to the FTP server";
        private static readonly string progressTextFileNotExist = "Log zip file doesnt exist. Please create new logs";

        private static string _tbServerLogsName = "HD-";
        private static string _tbClientLogsConfName = "HD-";
        private static string _tbAllLogsName = "HD-";

        private static Visibility _tbProgressTextVisibility = Visibility.Visible;
        private static Visibility _progressbarVisibility = Visibility.Hidden;
        private static bool _isProgressBarIndeterminate = false;

        // Just a initial value for the ProgressBarMaximum. We could also set it to 1. 
        private static long _progressBarMaximum = 100;
        private static long _progressBarValue = 0;

        private static string _selectedCustomFilePath = "";
        private static string _tbSelectedCustomFileName = "";
        private static string _customZipFile = "";
        private static readonly string progressTextUploadCustomFile = "Please wait... \nuploading custom file to the FTP server";
        private static bool _isUploadingCustomFile = false;
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

        #region Button Properties
        public static bool IsBtnClientFTPEnabled
        {
            get { return _isBtnClientFTPEnabled; }
            set
            {
                // The Client FTP upload button will only enabled if the the client logs zip file exists and if we have an internet connection here
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
            {   // The Server FTP upload button will only enabled if the the server logs zip file exists and if we have an internet connection here
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
                // The All logs FTP upload button will only enabled if the Client FTP upload button and Server FTP upload button are enabled
                if (IsBtnClientFTPEnabled && IsBtnServerFTPEnabled && IsInternetConnectionAvailable && value == true)
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
                // Client logs button will only enabled if the client logs and conf directory exists in localappdata seetec
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
                // Server logs button will only enabled if the server exits in the SeeTec install path. It might be that only a Client is installed
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

        public static bool IsBtnSelectCustomFileEnabled
        {
            get { return _isBtnSelectCustomFileEnabled; }
            set
            {
                _isBtnSelectCustomFileEnabled = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static bool IsBtnUploadCustomFileEnabled
        {
            get { return _isBtnUploadCustomFileEnabled; }
            set
            {
                // Upload custom button will only enabled if a zip file is selected and available
                if (File.Exists(CustomZipFile) && IsInternetConnectionAvailable)
                {
                    _isBtnUploadCustomFileEnabled = value;
                }
                else
                {
                    _isBtnUploadCustomFileEnabled = false;
                }
                RaiseStaticPropertyChanged();
            }
        }

        #endregion

        public static bool IsInternetConnectionAvailable
        {
            get { return _isInternetConnectionAvailable; }
            set
            {
                _isInternetConnectionAvailable = value;
            }
        }

        public static string SeeTecInstallPath
        {
            get { return _seeTecInstallPath; }
            set
            {
                _seeTecInstallPath = value;
            }
        }

        public static string ClientLogsPath
        {
            get { return _clientLogsPath; }
        }

        public static string ServerLogsPath
        {
            get { return _serverLogsPath; }
            set
            {
                _serverLogsPath = value;
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
            set
            {
                _serverLogsTempZip = value;
            }
        }

        public static string ServerLogsCopyTemp
        {
            get { return _serverLogsCopyTemp; }
            set
            {
                _serverLogsCopyTemp = value;
            }
        }

        public static string ClientLogsConfTempZip
        {
            get { return _clientLogsConfTempZip; }
            set
            {
                _clientLogsConfTempZip = value;
            }
        }

        public static string ClientLogCopyTemp
        {
            get { return _clientLogCopyTemp; }
            set
            {
                _clientLogCopyTemp = value;
            }
        }
        public static string ClientConfCopyTemp
        {
            get { return _clientConfCopyTemp; }
            set
            {
                _clientConfCopyTemp = value;
            }
        }

        public static string LogsZipFolderPathZip
        {
            get { return _logsZipFolderPathZip; }
            set
            {
                _logsZipFolderPathZip = value;
            }
        }

        public static string LogsZipPath
        {
            get { return _logsZipPath; }
            set
            {
                _logsZipPath = value;
            }
        }

        public static string LogsTemp
        {
            get { return _logsTemp; }
            set
            {
                _logsTemp = value;
            }
        }

        public static string ClientLogsConfTemp
        {
            get { return _clientLogsConfTemp; }
            set
            {
                _clientLogsConfTemp = value;
            }
        }

        public static string LogsClientConfCopyTempZip
        {
            get { return _logsClientConfCopyTempZip; }
            set
            {
                _logsClientConfCopyTempZip = value;
            }
        }

        public static string LogsServerCopyTempZip
        {
            get { return _logsServerCopyTempZip; }
            set
            {
                _logsServerCopyTempZip = value;
            }
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

        public static string LogTextWarning
        {
            get { return _logTextWarning; }
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

        public static string LogTextCustomZipFileSelected
        {
            get { return _logTextCustomZipFileSelected; }
        }

        public static string LogTextCustomFile
        {
            get { return _logTextCustomFile; }
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

        #region Custom Zip file properties
        public static string SelectedCustomFilePath
        {
            get { return _selectedCustomFilePath; }
            set { _selectedCustomFilePath = value; }
        }

        public static string TbSelectedCustomFileName
        {
            get { return _tbSelectedCustomFileName; }
            set
            {
                _tbSelectedCustomFileName = value;
                RaiseStaticPropertyChanged();
            }
        }

        public static string CustomZipFile
        {
            get { return _customZipFile; }
            set { _customZipFile = value; }
        }
        #endregion

        public static bool IsProgressBarIndeterminate
        {
            get { return _isProgressBarIndeterminate; }
            set
            {
                _isProgressBarIndeterminate = value;
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

        public static bool IsUploadingCustomFile
        {
            get { return _isUploadingCustomFile; }
            set { _isUploadingCustomFile = value; }
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
        /// it will upload all files to the FTP server
        /// </summary>
        public ICommand UploadAllFilesFTPCommand { get; private set; }

        /// <summary>
        /// Simple property to hold the 'SelectCustomFileCommand' - when executed
        /// it will select the custom zip file
        /// </summary>
        public ICommand SelectCustomFileCommand { get; private set; }

        /// <summary>
        /// Simple property to hold the 'UploadCustomFileCommand' - when executed
        /// it will upload selcted custom file to the FTP server
        /// </summary>
        public ICommand UploadCustomFileCommand { get; private set; }
        #endregion

        public MainViewModel()
        {
            // Relay commands for the buttons
            ExportClientLogsConfCommand = new RelayCommand(() => ExecuteExportClientLogsConfCommand());
            ExportServerLogsCommand = new RelayCommand(() => ExecuteExportServerLogsCommand());
            UploadClientFilesFTPCommand = new RelayCommand(() => ExecuteUploadClientFilesFTPCommand());
            UploadServerFilesFTPCommand = new RelayCommand(() => ExecuteUploadServerFilesFTPCommand());
            OpenLogsPathCommand = new RelayCommand(() => ExecuteOpenLogsPathCommand());
            UploadAllFilesFTPCommand = new RelayCommand(() => ExecuteUploadAllFilesFTPCommand());
            SelectCustomFileCommand = new RelayCommand(() => ExecuteSelectCustomFileCommand());
            UploadCustomFileCommand = new RelayCommand(() => ExecuteUploadCustomFileCommand());

            // Find out where SeeTec is installed
            FindSeeTecInstallPath();

            // Check if internet connection is available at start. If not it will disable the ftp upload function
            LogFunction.CheckInternetConnection();

            // Enable upload buttons at start if zip file already exists
            UpdateFTPButtons();

            // Enable create logs buttons at start if the log directorys exists
            UpdateCreateLogsAndSelectButtons();

            // Check if create client logs button and create server logs button are enabled. If not inform user that the not available logs cannot be created 
            LogFunction.CheckLogsAvailabilty();

            // Subscribe to an event which is fired when the event indicates that the associated process exited
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
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

            LogFunction.CheckInternetConnection();
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

            LogFunction.CheckInternetConnection();
        }

        /// <summary>
        /// Execute UploadClientFilesFTPCommand
        /// </summary>
        private void ExecuteUploadClientFilesFTPCommand()
        {

            LogFunction.CheckInternetConnection();

            // Check if log zip folder and internet connection are available. 
            // Yes: start upload
            // No: Update upload buttons
            if (File.Exists(ClientLogsConfTempZip) && IsInternetConnectionAvailable)
            {
                UpdatePropertiesUploadLogs(progressTextUploadClientfiles);

                Thread t = new Thread(new ThreadStart(StartUploadingClientFilesFTP));
                t.Start();
            }
            else
            {
                // Check if internet connection is available
                // Yes: Inform user that there are no logs to upload
                // No: Inform Customer that there is no internet connection
                if (IsInternetConnectionAvailable)
                {
                    TbProgressText = progressTextFileNotExist;
                    LogText += LogTextError + progressTextFileNotExist;
                }
                UpdateFTPButtons();
            }
        }

        /// <summary>
        /// Execute UploadServerFilesFTPCommand
        /// </summary>
        private void ExecuteUploadServerFilesFTPCommand()
        {
            LogFunction.CheckInternetConnection();

            // Check if log zip folder and internet connection are available. 
            // Yes: start upload
            // No: Update upload buttons
            if (File.Exists(ServerLogsTempZip) && IsInternetConnectionAvailable)
            {
                UpdatePropertiesUploadLogs(progressTextUploadServerfiles);

                Thread t = new Thread(new ThreadStart(StartUploadingServerFilesFTP));
                t.Start();
            }
            else
            {
                // Check if internet connection is available
                // Yes: Inform user that there are no logs to upload
                // No: Inform Customer that there is no internet connection
                if (IsInternetConnectionAvailable)
                {
                    TbProgressText = progressTextFileNotExist;
                    LogText += LogTextError + progressTextFileNotExist;
                }
                UpdateFTPButtons();
            }
        }

        /// <summary>
        /// Execute UploadAllFilesFTPCommand
        /// </summary>
        private void ExecuteUploadAllFilesFTPCommand()
        {
            LogFunction.CheckInternetConnection();

            // Check if log zip folder and internet connection are available. 
            // Yes: start upload
            // No: Update upload buttons
            if ((File.Exists(ClientLogsConfTempZip)) && (File.Exists(ServerLogsTempZip)) && IsInternetConnectionAvailable)
            {
                UpdatePropertiesUploadLogs(progressTextUploadAllfiles);

                Thread t = new Thread(new ThreadStart(StartUploadingAllFilesFTP));
                t.Start();
            }
            else
            {
                // Check if internet connection is available
                // Yes: Inform user that there are no logs to upload
                // No: Inform Customer that there is no internet connection
                if (IsInternetConnectionAvailable)
                {
                    TbProgressText = progressTextFileNotExist;
                    LogText += LogTextError + progressTextFileNotExist;
                }
                UpdateFTPButtons();
            }
        }

        /// <summary>
        /// Execute OpenLogsPathCommand
        /// Method to open the temp logs folder where the log zip folders are located
        /// </summary>
        private void ExecuteOpenLogsPathCommand()
        {
            LogFunction.OpenLogsPath(LogsZipPath);
        }

        /// <summary>
        /// Execute SelectCustomFileCommand
        /// Method to select the custom file for the upload
        /// </summary>
        private void ExecuteSelectCustomFileCommand()
        {
            LogFunction.SelectCustomFile();

            LogFunction.CheckInternetConnection();

            UpdateFTPButtons();
        }

        /// <summary>
        /// Execute UploadCustomFileCommand
        /// Method to upload selected custom file to the FTP server
        /// </summary>
        private void ExecuteUploadCustomFileCommand()
        {
            LogFunction.CheckInternetConnection();

            // Check if log zip folder and internet connection are available. 
            // Yes: start upload
            // No: Update upload buttons
            if (File.Exists(CustomZipFile) && IsInternetConnectionAvailable)
            {
                UpdatePropertiesUploadLogs(progressTextUploadCustomFile);

                IsUploadingCustomFile = true;

                Thread t = new Thread(new ThreadStart(StartUploadingCustomFilesFTP));
                t.Start();
            }
            else
            {
                // Check if internet connection is available
                // Yes: Inform user that there are no logs to upload
                // No: Inform Customer that there is no internet connection
                if (IsInternetConnectionAvailable)
                {
                    TbProgressText = progressTextFileNotExist;
                    LogText += LogTextError + progressTextFileNotExist;
                }
                UpdateFTPButtons();
            }
        }

        /// <summary>
        /// Method to zip server logs
        /// First check the folder size of the server logs
        /// Second set the IsUploadingAllLogs to false to activate zipping save progress
        /// At last zip server logs to the temp logs folder
        /// </summary>
        private void StartCreatingServerLogs()
        {
            LogFunction.CheckFolderSize(ServerLogsPath);
            IsUploadingAllLogs = false;
            LogFunction.CreateLogs(ServerLogsPath, ServerLogsTempZip, ServerLogsName);
        }

        /// <summary>
        /// Method to zip Client logs and conf
        /// First check the folder size of the client logs
        /// Second copy the client logs to temp logs folder
        /// Third copy the client conf to temp logs folder
        /// Fourth set the IsUploadingAllLogs to false to activate zipping save progress
        /// At last zip client logs and conf to the temp logs folder
        /// </summary>
        private void StartCreatingClientLogsConfLogs()
        {
            LogFunction.CheckFolderSize(ClientLogsPath);
            IsProgressBarIndeterminate = true;
            LogFunction.CopyLogs(ClientLogsPath, ClientLogCopyTemp, true);
            LogFunction.CopyLogs(clientConfPath, ClientConfCopyTemp, true);
            IsProgressBarIndeterminate = false;
            IsUploadingAllLogs = false;
            LogFunction.CreateLogs(ClientLogsConfTemp, ClientLogsConfTempZip, ClientLogsConfName);
        }

        /// <summary>
        /// Method to upload client files to the FTP server
        /// First check the zip folder size of client logs
        /// Second upload the client logs and conf zip to the seetec ftp server
        /// At last update the create logs buttons
        /// </summary>
        private void StartUploadingClientFilesFTP()
        {
            LogFunction.CheckZipSize(ClientLogsConfTempZip);
            LogFunction.UploadLogsFTP(ClientLogsConfTempZip, TbClientLogsConfName);

            UpdateCreateLogsAndSelectButtons();
        }

        /// <summary>
        /// Method to upload server files to the FTP server
        /// First check the zip folder size of server logs
        /// Second upload the server logs zip to the seetec ftp server
        /// At last update the create logs buttons
        /// </summary>
        private void StartUploadingServerFilesFTP()
        {
            LogFunction.CheckZipSize(ServerLogsTempZip);
            LogFunction.UploadLogsFTP(ServerLogsTempZip, TbServerLogsName);

            UpdateCreateLogsAndSelectButtons();
        }

        /// <summary>
        /// Method to upload all files to the FTP server
        /// First activate the progress bar indeterminate mode. Because we dont use save progress for zipping all logs
        /// Second set the IsUploadingAllLogs to true to deactivate zipping save progress
        /// Third we copy the existing serverlogs and client logs zip to a all logs folder
        /// Fourth we zip the all logs folder
        /// Fifth stop indeterminate mode for the progress bar 
        /// Sixth check the zip folder size of client logs
        /// Seventh upload all logs zip to the seetec ftp server
        /// At last update the create logs buttons
        /// </summary>
        private void StartUploadingAllFilesFTP()
        {
            IsProgressBarIndeterminate = true;
            IsUploadingAllLogs = true;
            LogFunction.CopyClientServerZipFolder();
            LogFunction.CreateLogs(LogsTemp, LogsZipFolderPathZip, LogZipFolderName);
            IsProgressBarIndeterminate = false;
            LogFunction.CheckZipSize(LogsZipFolderPathZip);
            LogFunction.UploadLogsFTP(LogsZipFolderPathZip, TbAllLogsName);

            UpdateCreateLogsAndSelectButtons();
        }

        /// <summary>
        /// Method to upload custom files to the FTP server
        /// First check the zip folder size of custom ZIP file
        /// Second upload the ZIP file to the seetec ftp server
        /// At last update the create logs buttons
        /// </summary>
        private void StartUploadingCustomFilesFTP()
        {
            LogFunction.CheckZipSize(CustomZipFile);
            LogFunction.UploadLogsFTP(CustomZipFile, "");

            UpdateCreateLogsAndSelectButtons();
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
            IsBtnSelectCustomFileEnabled = false;
            ProgressbarVisibility = Visibility.Visible;
            TbProgressText = progressText;
        }

        public static void UpdatePropertiesCreateLogsAtEnd(string logZipName)
        {
            ProgressbarVisibility = Visibility.Hidden;
            ProgressBarValue = 0;
            TbProgressText = logZipName + LogTextZipSuccess;
            IsBtnServerFTPEnabled = true;
            IsBtnClientFTPEnabled = true;
            IsBtnUploadAllFTPEnabled = true;
            IsBtnServerLogsEnabled = true;
            IsBtnClientLogsConfEnabled = true;
            IsBtnSelectCustomFileEnabled = true;
            IsBtnUploadCustomFileEnabled = true;
        }

        private void UpdatePropertiesUploadLogs(string progressText)
        {
            ProgressbarVisibility = Visibility.Visible;
            TbProgressText = progressText;
            IsBtnServerLogsEnabled = false;
            IsBtnClientLogsConfEnabled = false;
            IsBtnClientFTPEnabled = false;
            IsBtnServerFTPEnabled = false;
            IsBtnUploadAllFTPEnabled = false;
            IsBtnSelectCustomFileEnabled = false;
            IsBtnUploadCustomFileEnabled = false;
        }

        private void UpdateCreateLogsAndSelectButtons()
        {
            IsBtnServerLogsEnabled = true;
            IsBtnClientLogsConfEnabled = true;
            IsBtnSelectCustomFileEnabled = true;
        }

        public static void UpdatePropertiesFTPUpload(string logZipName)
        {
            ProgressbarVisibility = Visibility.Hidden;
            TbProgressText = logZipName + LogTextUploadSuccess;
            IsBtnClientFTPEnabled = true;
            IsBtnServerFTPEnabled = true;
            IsBtnUploadAllFTPEnabled = true;
            TbSelectedCustomFileName = "";
            IsBtnUploadCustomFileEnabled = true;
        }

        public static void UpdateFTPButtons()
        {
            IsBtnClientFTPEnabled = true;
            IsBtnServerFTPEnabled = true;
            IsBtnUploadAllFTPEnabled = true;
            IsBtnUploadCustomFileEnabled = true;
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
            {
                handler(null, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Method to find the SeeTec installation path
        /// First check if Cayuga is installed in the default path
        /// If not installes in the default path then search for SeeTec folder
        /// </summary>
        private static void FindSeeTecInstallPath()
        {
            string defaultSeeTecInstallPath = @"C:\Program Files\SeeTec";

            try
            {
                // Check if Cayuga is installed in the default path
                // Yes: Set installation path as default
                // No: Search drives for SeeTec folder
                if (Directory.Exists(defaultSeeTecInstallPath))
                {
                    SeeTecInstallPath = defaultSeeTecInstallPath;
                }
                else
                {
                    string[] dirsLevelOne;
                    string[] dirsLevelOne2;
                    string[] dirsLevelTwo;

                    // Check for available hard drives and iterate one by one over all hard drives 
                    foreach (DriveInfo d in DriveInfo.GetDrives().Where(x => x.IsReady))
                    {
                        try
                        {
                            // First search in the top directory for SeeTec folder
                            dirsLevelOne = Directory.GetDirectories(d.RootDirectory.FullName);

                            foreach (var item in dirsLevelOne)
                            {
                                // Check if SeeTec folder is found in the top directory
                                dirsLevelOne2 = Directory.GetDirectories(d.RootDirectory.FullName, "SeeTec");

                                // If SeeTec folder is found in the top directory then set the installation path
                                if (dirsLevelOne2.Length == 1)
                                {
                                    SeeTecInstallPath = dirsLevelOne2[0];
                                    break;
                                }
                                else
                                {
                                    // If SeeTec folder is not in the top directory then search for teh folder in the subdirectory for it
                                    dirsLevelTwo = Directory.GetDirectories(item, "SeeTec");

                                    // If SeeTec folder is found in the subdirectory then set the installation path
                                    if (dirsLevelTwo.Length == 1)
                                    {
                                        SeeTecInstallPath = dirsLevelTwo[0];
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch
            {

            }
            // Set all needed paths after finding the installation path
            SetAllPaths();
        }

        /// <summary>
        /// Method to set all needed paths
        /// </summary>
        private static void SetAllPaths()
        {
            ServerLogsPath = SeeTecInstallPath + @"\log";
        }

        /// <summary>
        /// Method to delete copied temporary log folders and files before exiting application
        /// It will also delete temporary files from the zipping process
        /// </summary>
        private void OnProcessExit(object sender, EventArgs e)
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

                foreach (string sFile in Directory.GetFiles(LogsZipPath, "*.tmp"))
                {
                    File.Delete(sFile);
                }
            }
            catch (DirectoryNotFoundException)
            {

            }
            catch (FileNotFoundException)
            {

            }

        }
    }
}