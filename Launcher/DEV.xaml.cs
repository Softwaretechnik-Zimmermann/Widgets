using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Launcher
{
    /// <summary>
    /// Interaktionslogik für DEV.xaml
    /// </summary>
    public partial class DEV : Page
    {
        public DEV()
        {
            InitializeComponent();
        }

        private void mailto(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:softwaretechnik.zimmermann@gmail.com");
        }

        private void openupdates(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://sites.google.com/view/softwaretechnik-zimmermann/downloads/");
        }
    }
}
