using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Logs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Check if program running as administrator
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (principal.IsInRole(WindowsBuiltInRole.Administrator) == false)
            {
                //Get the full qualified path to the own executable
                string x = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;

                //Start itself with adminrights
                if (RunElevated(x) == true)
                {
                    //Close old programwindow
                    this.Close();
                }
            }

            InitializeComponent();
        }

        /// <summary>
        /// Run a program with elevated rights
        /// </summary>
        /// <param name="fileName">Path to the executable to run</param>
        /// <returns>True if no error has occurred</returns>
        private static bool RunElevated(string fileName)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.Verb = "runas";
            processInfo.FileName = fileName;
            try
            {
                Process.Start(processInfo);
                return true;
            }
            catch (Win32Exception)
            {
                //Do nothing. Probably the user canceled the UAC window
            }
            return false;
        }       
    }
}
