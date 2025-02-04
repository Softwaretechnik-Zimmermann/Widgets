using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace Launcher
{
    /// <summary>
    /// Interaktionslogik für shutdown.xaml
    /// </summary>
    public partial class shutdown : Page
    {
        [DllImport("user32")]
        public static extern void LockWorkStation();

        [DllImport("user32")]
        public static extern bool ExitWindowsEx(GraphicsUnit uFlags, uint dwReason);

        public shutdown()
        {
            InitializeComponent();
            if (Properties.Settings.Default.darkmode != "false")
            {
                background.Background = new SolidColorBrush(Colors.Black);
            }

        }

        private void Shutdown(object sender, RoutedEventArgs e)
        {
            _ = Process.Start("shutdown", "-s -t 00");
        }

        private void restart(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("shutdown.exe", "-r -t 00");
        }

        private void lockusr(object sender, RoutedEventArgs e)
        {
            LockWorkStation();
        }

        private void logoff(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.SetSuspendState(PowerState.Suspend, true, true);
        }
    }
}
