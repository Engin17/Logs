using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using Logs.Functions;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Logs.ViewModels
{
    /// <summary>
    /// This is our MainViewModel that is tied to the MainWindow
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Member variables
        private static string path = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
        private static string path2 = Directory.GetCurrentDirectory();
        private static readonly string serverLogsPath = @"C:\Program Files\SeeTec\log";
        private static readonly string serverLogsTempZip = @"C:\Program Files\SeeTec\TempLogs\ServerLog.zip";
        private static readonly string serverLogsCopyTemp = @"C:\Program Files\SeeTec\TempLogs\ServerLog";
        private static readonly string _serverLogsName = "server logs";

        private static readonly string clientLogsPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\log");
        private static readonly string clientConfPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\AppData\Local\SeeTec\conf");
        private static readonly string clientLogsConfTempZip = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf.zip";
        private static readonly string clientLogCopyTemp = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf\ClientLogs";
        private static readonly string clientConfCopyTemp = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf\ClientConf";
        private static readonly string clientLogsConfTemp = @"C:\Program Files\SeeTec\TempLogs\ClientLogsConf";
        private static readonly string _clientLogsConfName = "client logs and configuration";

        private bool _btnClientEnabled = false;
        private bool _btnServerEnabled = false;
        #endregion

        #region property members
        public bool BtnClientEnabled
        {
            get { return _btnClientEnabled; }
            set
            {
                if (_btnClientEnabled != value)
                {
                    _btnClientEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool BtnServerEnabled
        {
            get { return _btnServerEnabled; }
            set
            {
                if (_btnServerEnabled != value)
                {
                    _btnServerEnabled = value;
                    NotifyPropertyChanged();
                }
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
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

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
        #endregion

        public MainViewModel()
        {
            ExportClientLogsConfCommand = new RelayCommand(() => ExecuteExportClientLogsConfCommand());
            ExportServerLogsCommand = new RelayCommand(() => ExecuteExportServerLogsCommand());
            UploadClientFilesFTPCommand = new RelayCommand(() => ExecuteUploadClientFilesFTPCommand());
            UploadServerFilesFTPCommand = new RelayCommand(() => ExecuteUploadServerFilesFTPCommand());
        }

        /// <summary>
        /// Method for the ExportClientLogsConfCommand
        /// First copy the client logs to a specified directory
        /// Second copy the client conf to a specified directory
        /// Third zip the copied logs and conf
        /// </summary>
        private void ExecuteExportClientLogsConfCommand()
        {
            LogFunction.CopyLogs(clientLogsPath, clientLogCopyTemp, true);
            LogFunction.CopyLogs(clientConfPath, clientConfCopyTemp, true);
            LogFunction.CreateLogs(clientLogsConfTemp, clientLogsConfTempZip, _clientLogsConfName, clientLogsConfTemp);

            if (LogFunction.ClientLogsConfZipCreated == true) 
            {
                BtnClientEnabled = true;
            }
        }

        /// <summary>
        /// Method for the ExecuteExportServerLogsCommand
        /// First copy the server logs to a specified directory
        /// Second zip the copied logs
        /// </summary>
        private void ExecuteExportServerLogsCommand()
        {
            LogFunction.CopyLogs(serverLogsPath, serverLogsCopyTemp, true);
            LogFunction.CreateLogs(serverLogsCopyTemp, serverLogsTempZip, _serverLogsName, serverLogsCopyTemp);

            if (LogFunction.ServerLogsConfZipCreated == true)
            {
                BtnServerEnabled = true;
            }
        }

        /// <summary>
        /// Method for the ExecuteUploadClientFilesFTPCommand
        /// </summary>
        private void ExecuteUploadClientFilesFTPCommand()
        {
            LogFunction.UploadLogsFTP(clientLogsConfTempZip);
        }

        /// <summary>
        /// Method for the ExecuteUploadServerFilesFTPCommand
        /// </summary>
        private void ExecuteUploadServerFilesFTPCommand()
        {
            LogFunction.UploadLogsFTP(serverLogsTempZip);
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}