using Launcher.Properties;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Launcher.Desktop
{
    /// <summary>
    /// Interaktionslogik für Item.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public string Path { get; set; }
        public System.Drawing.Icon Icon { get; set; }
        public bool selected = false;
        public bool Enabled = true;
        public bool initialized = false;

        public Item()
        {
            InitializeComponent();

            ItemBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            Text.Text = "";
        }

        public void SetBackgroundempty()
        {
            ItemBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Enabled)
                ItemBorder.Background = new SolidColorBrush(Color.FromArgb(60, 80, 150, 220));
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (selected == false) this.ItemBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Enabled)
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                try
                {
                    System.Diagnostics.Process.Start(Path);
                }
                catch (Exception ex)
                {
                    Popup p = new Popup(ex.ToString(), "OK", "Die Datei oder der Ordner konnte nicht geöffnet werden. Pfad: " + desktopPath + Path.ToString());
                    p.Show();
                }
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Enabled)
                ItemBorder.Background = new SolidColorBrush(Color.FromArgb(80, 80, 150, 220));
            else
                ItemGrid.Deselect();
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Enabled)
            {
                if (selected == true)
                {
                    selected = false;
                    ItemBorder.Background = new SolidColorBrush(Color.FromArgb(95, 80, 150, 220));
                }
                else selected = true;
            }
        }

        public void Disable()
        {
            ItemBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            selected = false;
            Image.Source = null;
            Enabled = false;
        }
        public void Enable()
        {
            ItemBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            Image.Source = null;
            Enabled = true;
        }
        public void SetIcon(string Path)
        {
            //Image.Source = new ImageSource(Path);
        }

    }
}
