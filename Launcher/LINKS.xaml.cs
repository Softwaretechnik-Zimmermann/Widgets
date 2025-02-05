using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Launcher
{
    /// <summary>
    /// Interaktionslogik für LINKS.xaml
    /// </summary>
    public partial class LINKS : System.Windows.Window
    {
        public LINKS()
        {
            InitializeComponent();

            if (Properties.Settings.Default.darkmode != "false")
            {
                background.Background = new SolidColorBrush(Color.FromRgb(20,20,20));
            }

            edit.Height = 0;

            // Load Buttons from Save


            string Load = Properties.Settings.Default.ButtonLinks;
            var Values = Load.Split('|');
            
            b1.Content = Values[0];
            b1.Tag = Values[1];
            b2.Content = Values[2];
            b2.Tag = Values[3];
            b3.Content = Values[4];
            b3.Tag = Values[5];
            b4.Content = Values[6];
            b4.Tag = Values[7];

            }

        private void b_Click(object sender, RoutedEventArgs e)
        {
            var btn = (System.Windows.Controls.Button)sender;
            try
            {

                System.Diagnostics.Process.Start((string)btn.Tag);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Ungültiger Link: \n" + ex , "Fehler" , MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


        private void editb_Click(object sender, RoutedEventArgs e)
        {
            if (edit.Height == 0)
            {
                edit.Height = 50;
            }
            else edit.Height = 0;
        }

        private void apply(object sender, RoutedEventArgs e)
        {
            if (name != null && link != null)
            {

                edit.Height = 0;
                MainWindow mainWin = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

                switch (selectlink.SelectedIndex)
                {
                    case 0:
                        {
                            b1.Content = name.Text;
                            b1.Tag = link.Text;
                            mainWin.UpdateButton1(name.Text, link.Text);
                        }
                        break;

                    case 1:
                        {
                            b2.Content = name.Text;
                            b2.Tag = link.Text;
                            mainWin.UpdateButton2(name.Text, link.Text);
                        }
                        break;

                    case 2:
                        {
                            b3.Content = name.Text;
                            b3.Tag = link.Text;
                            mainWin.UpdateButton3(name.Text, link.Text);
                        }
                        break;

                    case 3:
                        {
                            b4.Content = name.Text;
                            b4.Tag = link.Text;
                            mainWin.UpdateButton4(name.Text, link.Text);
                        }
                        break;

                }

                string Save = "";
                Save = String.Join("|", new[]{
                    b1.Content, b1.Tag.ToString(), 
                    b2.Content, b2.Tag.ToString(), 
                    b3.Content, b3.Tag.ToString(),
                    b4.Content, b4.Tag.ToString(),
                });
                Properties.Settings.Default.ButtonLinks = Save;
                Properties.Settings.Default.Save();

                link.Text = "https://";
                if (selectlink.SelectedIndex == 3)
                selectlink.SelectedIndex += 0;
                else selectlink.SelectedIndex = 0;   
            }

        }

        private void name_TextChanged(object sender, TextChangedEventArgs e)
        {
            name.Text = name.Text.Replace("|", "");
        }

        private void link_TextChanged(object sender, TextChangedEventArgs e)
        {
            link.Text = link.Text.Replace("|", "");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.linkeditopen = false;
        }
    }
}
