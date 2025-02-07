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
using System.Windows.Shapes;

namespace Launcher
{
    /// <summary>
    /// Interaktionslogik für Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {
        public Popup(string Message, string ButtonText, string WindowTitle)
        {
            InitializeComponent();

            if (Properties.Settings.Default.darkmode != "false")
            {
                background.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                tb.Foreground = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            }

            tb.Text = Message;
            b1.Content = ButtonText;
            window.Title = WindowTitle;
        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
