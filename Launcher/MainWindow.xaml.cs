using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Runtime.InteropServices;
using static System.Windows.Forms.LinkLabel;
using System.Xml.Linq;

namespace Launcher
{

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		public Window Popupwindow;

        // window order

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOOLWINDOW = 0x00000080; // Optional: Makes the window not appear in the Alt-Tab menu
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;

        // net speedtest

        private string[] QueryCodes = { "q", "zip", "id", };
		public Dictionary<string,string> GetInternetSpeedInfo()
        {
			Dictionary<string, string> result = null;
			Process process = null;
			try
            {
				process = new Process();
				result = new Dictionary<string, string>();

				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

				process.StartInfo.FileName = "speedtest.exe";

				process.Start();
				string output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();

				List<string> list_output = output.Split('\n').ToList();

				result["Server"] = list_output[3].Split(':')[1].Trim();
				result["ISP"] = list_output[4].Split(':')[1].Trim();
				result["Latency"] = list_output[5].Split(':')[1].Trim();
				result["Download"] = list_output[6].Replace("Download:", "").Trim();
				result["Upload"] = list_output[7].Replace("Upload:", "").Trim();
				result["Packetloss"] = list_output[8].Split(':')[1].Trim();


				netspeed.Content = result["Download"] + " Mb/s";
				netping.Content = result["Latency"] + " ms";
			}
			catch (Exception)
            {

				throw;
            }
			finally
            {
				if (process != null) process.Dispose();
            }
			return result;
        }

		public MainWindow()
		{
			InitializeComponent();
            Loaded += window_Loaded;

            maingrid.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

			window.Height = Properties.Settings.Default.height;
			window.Width = Properties.Settings.Default.width;

			this.Top = Properties.Settings.Default.top;
			this.Left = Properties.Settings.Default.left;

			web = 1;


			runrefresh();
			Loadsettings();


			// get max ram
			System.Management.ObjectQuery wql = new System.Management.ObjectQuery("SELECT * FROM Win32_OperatingSystem");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
			ManagementObjectCollection results = searcher.Get();

			double res;
			foreach (ManagementObject result in results)
			{
				res = Convert.ToDouble(result["TotalVisibleMemorySize"]);
				double fres = Math.Round((res / (1024 * 1024)), 2);
				rambar.Maximum = res / 1000;
				double e = fres;
				int j = (int)Math.Round(e);
				maxram.Content = j + "GB";
			}

			bool b = Properties.Settings.Default.fullscreen;
			if (b == true)
				Properties.Settings.Default.fullscreen = false;
			else Properties.Settings.Default.fullscreen = true;
			maxw();
			Properties.Settings.Default.fullscreen = b;
			popupgrid.Opacity = 0;

            if (Properties.Settings.Default.showlinks == true)
            {
                linkactive.IsChecked = true;
                Properties.Settings.Default.showlinks = true;
                linkborder.Height = 100;
                linkborder.Width = 200;
                linkborder.Margin = new Thickness(0, 0, 0, 10);
                Properties.Settings.Default.date = true;
            }
            else
            {
                linkactive.IsChecked = false;
                Properties.Settings.Default.showlinks = false;
                if (Popupwindow != null && Popupwindow is LINKS)
                {
                    Popupwindow.Close();
                    Popupwindow = null;
                }
                linkborder.Height = 0;
                linkborder.Width = 0;
                linkborder.Margin = new Thickness(0);
                Properties.Settings.Default.date = false;
            }


            if (Properties.Settings.Default.firststart == true)
			{
                var wind = new DEV();
                wind.Show();

            }

        }

        private async void Loadsettings()
        {
			await Task.Delay(10);

			if (Properties.Settings.Default.darkmode == "false")
			{
				UI obj = new UI();
				obj.color1 = "#99CCCCCC";
				obj.color2 = "#FF000000";
				obj.color3 = "#FF2F7FD6";
				DataContext = obj;
			}
			if (Properties.Settings.Default.darkmode == "true")
			{
				UI obj = new UI();
				obj.color1 = "#99000000";
				obj.color2 = "#FFFFFFFF";
				obj.color3 = "#FF0F184E";
				DataContext = obj;
			}

			if (Properties.Settings.Default.darkmode == "default")
			{
				UI obj = new UI();
				obj.color1 = Properties.Settings.Default.cl1;
				obj.color2 = Properties.Settings.Default.cl2;
				obj.color3 = Properties.Settings.Default.cl3;
				DataContext = obj;
			}

        
        

			if (Properties.Settings.Default.cornerradius == true)
			{
				settings.CornerRadius = new CornerRadius(14);
				roundedb.IsChecked = true;
			}
			else
			{
				settings.CornerRadius = new CornerRadius(0);
				roundedb.IsChecked = false;
			}

			if (Properties.Settings.Default.topmost == true)
			{
				window.Topmost = true;
				foreshow.IsChecked = true;
			}
			else
			{
				window.Topmost = false;
				foreshow.IsChecked = false;
			}

			if (Properties.Settings.Default.vertical == false)
			{
				system.Orientation = widgets.Orientation = System.Windows.Controls.Orientation.Horizontal;
			}
			else
			{
				system.Orientation = widgets.Orientation = System.Windows.Controls.Orientation.Vertical;
			}

			if (Properties.Settings.Default.apps == true)
			{
				system.Opacity = 100;
				systemcheck.IsChecked = true;
				systemscroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
				system.IsEnabled = true;
			}
			else
			{
				system.Opacity = 0;
				systemcheck.IsChecked = false;
				systemscroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
				system.IsEnabled = false;
			}


			if (Properties.Settings.Default.time == true)
			{
				timecheck.IsChecked = true;
				timeg.Height = 100;
				timeg.Width = 200;
                timeg.Margin = new Thickness(0, 0, 0, 10);
            }
            else
			{
				timecheck.IsChecked = false;
				timeg.Height = 0;
				timeg.Width = 0;
                timeg.Margin = new Thickness(0);
            }
            if (Properties.Settings.Default.showseconds == true)
			{
				timeseconds.IsChecked = true;
			}
			else
			{
				timeseconds.IsChecked = false;
			}
			if (Properties.Settings.Default.date == true)
			{
				datecheck.IsChecked = true;
				dateb.Height = 100;
				dateb.Width = 200;
                dateb.Margin = new Thickness(0, 0, 0, 10);

            }
            else
			{
				datecheck.IsChecked = false;
				dateb.Height = 0;
				dateb.Width = 0;
                dateb.Margin = new Thickness(0);
            }
            if (Properties.Settings.Default.weather == true)
			{
				weatherb.Height = 100;
				weatherb.Width = 200;
				weathercheck.IsChecked = true;
                weatherb.Margin = new Thickness(0, 0, 0, 10);
            }
            else
			{
				weatherb.Height = 0;
				weatherb.Width = 0;
				weathercheck.IsChecked = false;
                weatherb.Margin = new Thickness(0);
            }
            if (Properties.Settings.Default.ressources == false)
			{
				cpuborder.Height = 0;
				cpuborder.Width = 0;
				ramborder.Height = 0;
				ramborder.Width = 0;
				cpuborder.Margin = new Thickness(0);
				ramborder.Margin = new Thickness(0);
				rescheck.IsChecked = false;
			}
			else
			{
				cpuborder.Height = 100;
				cpuborder.Width = 200;
				ramborder.Height = 100;
				ramborder.Width = 200;
				cpuborder.Margin = new Thickness(0, 0, 0, 10);
				ramborder.Margin = new Thickness(0, 0, 0, 10);
				rescheck.IsChecked = true;
			}
			if (Properties.Settings.Default.netspeed == false)
			{
				netborder.Height = 0;
				netborder.Width = 0;
				netcheck.IsChecked = false;
                netborder.Margin = new Thickness(0);
            }
            else
			{
				netborder.Height = 100;
				netborder.Width = 200;
				netcheck.IsChecked = true;
                netborder.Margin = new Thickness(0, 0, 0, 10);
            }

            del = (int)Properties.Settings.Default.refresh;
			refreshtime.Value = Properties.Settings.Default.refresh;

			double c = Properties.Settings.Default.taskscale;
			maingrid.Margin = new Thickness(0, 0, 0, c);
			taskbarslider.Value = c;

			settings.Width = 0;

			ansichtslider.Value = Properties.Settings.Default.zoom;

			titlebartrigger.Height = 10;
			titlegrid.Height = 0;
			titlebartrigger.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));

			if (Properties.Settings.Default.showseconds == true)
				timeseconds.IsChecked = true;
			else timeseconds.IsChecked = false;


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
		



		private async void runrefresh()
        {
			await Task.Delay(1000);
			cpuRefresh();
		}

		private void windowkeydown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.S))
			{
				if (settings.Width == 0)
					settings.Width = 300;
				else settings.Width = 0;
			}
			if (e.Key == Key.System && e.SystemKey == Key.F4)
			{
				popup.Content = new shutdown();
				popupgrid.Opacity = 100;
				popupf.Opacity = 100;
				e.Handled = true;
			}
		}



        private void Minw(object sender, RoutedEventArgs e)
		{
			window.ShowInTaskbar = true;
			System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized;
		}

		private void maxw(object sender, RoutedEventArgs e)
		{
			maxw();
		}
		private void maxw()
		{
			if (Properties.Settings.Default.fullscreen == true)
			{
				System.Windows.Application.Current.MainWindow.WindowState = WindowState.Normal;
				titlebartrigger.Height = 10;
				titlebartrigger.Background = null;
				titlegrid.Height = 30;
				window.ResizeMode = ResizeMode.CanResizeWithGrip;
				maingrid.Background = new SolidColorBrush(Color.FromArgb(40, 222, 222, 222));
				stackpanel.Margin = new Thickness(0, 27, 0, 0);
				settings.Margin = new Thickness(0, 27, 5, 10);
				maingrid.Margin = new Thickness(0, 0, 0, 0);
				taskbarslider.IsEnabled = button.IsEnabled = min.IsEnabled = false;
				Properties.Settings.Default.fullscreen = false;
			}
			else
			{
				window.ResizeMode = ResizeMode.NoResize;
				System.Windows.Application.Current.MainWindow.WindowState = WindowState.Maximized;
				titlebartrigger.Height = 10;
				titlegrid.Height = 0;
				titlebartrigger.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
				maingrid.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
				stackpanel.Margin = new Thickness(0, 10, 0, 0);
				settings.Margin = new Thickness(0, 10, 5, 10);
				maingrid.Margin = new Thickness(0, 0, 0, taskbarslider.Value);
				taskbarslider.IsEnabled = button.IsEnabled = min.IsEnabled = true;
				Properties.Settings.Default.fullscreen = true;
			}
		}

		private void closew(object sender, RoutedEventArgs e)
		{
			safe(sender, e);
			System.Windows.Application.Current.Shutdown();
		}

		private void safe(object sender, RoutedEventArgs e)
		{
			if (System.Windows.Application.Current.MainWindow.WindowState != WindowState.Maximized)
			{
				Properties.Settings.Default.zoom = ansichtslider.Value;
				Properties.Settings.Default.height = window.Height;
				Properties.Settings.Default.width = window.Width;
			}

			Properties.Settings.Default.taskscale = taskbarslider.Value;
			Properties.Settings.Default.refresh = refreshtime.Value;

			Properties.Settings.Default.top = this.Top;
			Properties.Settings.Default.left = this.Left;


			Properties.Settings.Default.Save();
		}


		private void explorero(object sender, RoutedEventArgs e)
		{
			_ = System.Diagnostics.Process.Start("C:/Windows/system32/explorer.exe");
		}

		private void cmdo(object sender, RoutedEventArgs e)
		{
			_ = System.Diagnostics.Process.Start("C:/Windows/system32/cmd.exe");
		}

		private void editoro(object sender, RoutedEventArgs e)
		{
			_ = System.Diagnostics.Process.Start("C:/Windows/system32/notepad.exe");
		}
		private void calco(object sender, RoutedEventArgs e)
		{
			_ = System.Diagnostics.Process.Start("C:/Windows/system32/calc.exe");
		}


		private void ansichtslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
		}



		private void dragmove(object sender, MouseButtonEventArgs e)
		{
			if (window.WindowState == WindowState.Normal)
			{
				if (e.LeftButton == MouseButtonState.Pressed)
				{
					titlegrid.Background = new SolidColorBrush(Color.FromArgb(80, 222, 222, 222));
					maingrid.Background = new SolidColorBrush(Color.FromArgb(90, 222, 222, 222));
					DragMove();
				}
				else
					titlegrid.Background = new SolidColorBrush(Color.FromArgb(35, 142, 142, 142));
				maingrid.Background = new SolidColorBrush(Color.FromArgb(40, 222, 222, 222));
			}
		}


		private void settingsshow(object sender, RoutedEventArgs e)
		{
			if (settings.Width == 0)
				settings.Width = 300;
			else settings.Width = 0;
		}

		private void foreshow_Checked(object sender, RoutedEventArgs e)
		{
			if (foreshow.IsChecked == true)
			{
				window.Topmost = true;
				Properties.Settings.Default.topmost = true;
			}
			else
			{
				window.Topmost = false;
				Properties.Settings.Default.topmost = false;
			}
		}
		private void verticalswitch(object sender, RoutedEventArgs e)
		{
			if (Properties.Settings.Default.vertical == true)
			{
				Properties.Settings.Default.vertical = false;
				system.Orientation = widgets.Orientation = System.Windows.Controls.Orientation.Horizontal;
			}
			else
			{
				Properties.Settings.Default.vertical = true;
				system.Orientation = widgets.Orientation = System.Windows.Controls.Orientation.Vertical;
			}
		}


		private void ress(object sender, RoutedEventArgs e)
		{
			ressb.Content = "Neustart der App erforderlich";
			Properties.Settings.Default.Reset();
			// Zoom
			ansichtslider.Value = 100;
			// Aktualisierungsgeschwindigkeit
			refreshtime.Value = 900;
			del = 900;
			// Farben
			UI obj = new UI();
			obj.color1 = "#99CCCCCC";
			obj.color2 = "#FF000000";
			obj.color3 = "#FF2F7FD6";
			DataContext = obj;
			// Symbol Zoom
			ansichtslider.Value = Properties.Settings.Default.zoom;
			// Taskleiste Abstand
			double c = Properties.Settings.Default.taskscale;
			maingrid.Margin = new Thickness(0, 0, 0, c);
			taskbarslider.Value = c;

			InitializeComponent();
		}


		private void linkactive_Click(object sender, RoutedEventArgs e)
		{
			if (linkactive.IsChecked == true)
			{
				Properties.Settings.Default.showlinks = true;
				if (Popupwindow == null)
				{
					Linkedit(sender, e);
				}
				linkborder.Height = 100;
                linkborder.Width = 200;
                linkborder.Margin = new Thickness(0, 0, 0, 10);
				Properties.Settings.Default.date = true;
			}
			else
			{
				Properties.Settings.Default.showlinks = false;
				if (Popupwindow != null && Popupwindow is LINKS)
				{
					Popupwindow.Close();
					Popupwindow = null;
				}
                linkborder.Height = 0;
                linkborder.Width = 0;
                linkborder.Margin = new Thickness(0);
				Properties.Settings.Default.date = false;
			}
        }

		public static bool linkeditopen = false;

        private void Linkedit(object sender, RoutedEventArgs e)
		{
			if (linkeditopen == false) {
				var wind = new LINKS();
				Popupwindow = wind;
				wind.Show();
				linkeditopen = true;
			}
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
                System.Windows.Forms.MessageBox.Show("Ungültiger Link: \n" + ex, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


        public void Linkrefresh()
        {
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


        private void Apps_Checked(object sender, RoutedEventArgs e)
		{
			if (systemcheck.IsChecked == true)
			{
				system.Opacity = 100;
				Properties.Settings.Default.apps = true;
				systemscroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
				system.IsEnabled = true;
			}
			else
			{
				system.Opacity = 0;
				Properties.Settings.Default.apps = false;
				systemscroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
				system.IsEnabled = false;
			}
		}


		private void timerefresh(object sender, RoutedEventArgs e)
		{
			// sekunden werden übersprungen
			DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
			{
				if (Properties.Settings.Default.showseconds == true)
					this.time.Content = DateTime.Now.ToString("HH:mm:ss");
				if (Properties.Settings.Default.showseconds == false)
					this.time.Content = DateTime.Now.ToString("HH:mm");
				this.date.Content = DateTime.Now.ToString("dd") + ". " + DateTime.Now.ToString("Y");
				day.Content = DateTime.Now.ToString("dddd");
			}, this.Dispatcher);
		}

		private void timecheck_Checked(object sender, RoutedEventArgs e)
		{
			if (timecheck.IsChecked == true)
			{
				timeg.Height = 100;
				timeg.Width = 200;
                timeg.Margin = new Thickness(0, 0, 0, 10);
                Properties.Settings.Default.time = true;
			}
			else
			{
				timeg.Height = 0;
				timeg.Width = 0;
                timeg.Margin = new Thickness(0);
                Properties.Settings.Default.time = false;
			}
		}

		private void datecheck_Checked(object sender, RoutedEventArgs e)
		{
			if (datecheck.IsChecked == true)
			{
				dateb.Height = 100;
				dateb.Width = 200;
                dateb.Margin = new Thickness(0, 0, 0, 10);
                Properties.Settings.Default.date = true;
			}
			else
			{
				dateb.Height = 0;
				dateb.Width = 0;
                dateb.Margin = new Thickness(0);
                Properties.Settings.Default.date = false;
			}
		}

		private void weathercheck_Checked(object sender, RoutedEventArgs e)
		{
			if (weathercheck.IsChecked == true)
			{
				weatherb.Height = 100;
				weatherb.Width = 200;
                weatherb.Margin = new Thickness(0, 0, 0, 10);
                Properties.Settings.Default.weather = true;
			}
			else
			{
				weatherb.Height = 0;
				weatherb.Width = 0;
                weatherb.Margin = new Thickness(0);
                Properties.Settings.Default.weather = false;
			}
		}

		private void showseconds(object sender, RoutedEventArgs e)
		{
			if (timeseconds.IsChecked == true)
				Properties.Settings.Default.showseconds = true;
			else Properties.Settings.Default.showseconds = false;
		}

		private void refreshtime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			del = (int)refreshtime.Value;
		}

		private void timebc(object sender, RoutedEventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Color cl1 = Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
				UI obj = new UI();
				obj.color1 = cl1.ToString();
				Properties.Settings.Default.cl1 = cl1.ToString();
				obj.color2 = Properties.Settings.Default.cl2;
				obj.color3 = Properties.Settings.Default.cl3;
				DataContext = obj;
				Properties.Settings.Default.darkmode = "default";
			}
		}

		private void timefc(object sender, RoutedEventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Color cl2 = Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
				UI obj = new UI();
				obj.color1 = Properties.Settings.Default.cl1;
				obj.color2 = cl2.ToString();
				Properties.Settings.Default.cl2 = cl2.ToString();
				obj.color3 = Properties.Settings.Default.cl3;
				DataContext = obj;
				Properties.Settings.Default.darkmode = "default";
			}
		}

		private void timerc(object sender, RoutedEventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Color cl3 = Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
				UI obj = new UI();
				obj.color1 = Properties.Settings.Default.cl1;
				obj.color2 = Properties.Settings.Default.cl2;
				obj.color3 = cl3.ToString();
				Properties.Settings.Default.cl3 = cl3.ToString();
				DataContext = obj;
				Properties.Settings.Default.darkmode = "default";
			}
		}

		private void roundedb_Checked(object sender, RoutedEventArgs e)
		{
			if (roundedb.IsChecked == true)
			{
				settings.CornerRadius = new CornerRadius(14);
				Properties.Settings.Default.cornerradius = true;
			}
			else
			{
				settings.CornerRadius = new CornerRadius(0);
				Properties.Settings.Default.cornerradius = false;
			}
		}

		private void weather_Loaded(object sender, RoutedEventArgs e)
		{
			weather.Content = "Wetter nicht verfügbar.";
		}
		private void rescheck_Checked(object sender, RoutedEventArgs e)
		{
			if (rescheck.IsChecked == false)
			{
				cpuborder.Height = 0;
				cpuborder.Width = 0;
				ramborder.Height = 0;
				ramborder.Width = 0;
				cpuborder.Margin = new Thickness(0);
				ramborder.Margin = new Thickness(0);
				Properties.Settings.Default.ressources = false;
			}
			else
			{
				cpuborder.Height = 100;
				cpuborder.Width = 200;
				ramborder.Height = 100;
				ramborder.Width = 200;
				cpuborder.Margin = new Thickness(0, 0, 0, 10);
				ramborder.Margin = new Thickness(0, 0, 0, 10);
				Properties.Settings.Default.ressources = true;
				cpuRefresh();
			}
		}

		private void netcheck_Click(object sender, RoutedEventArgs e)
		{
			if (netcheck.IsChecked == false)
			{
				Properties.Settings.Default.netspeed = false;
				netborder.Height = 0;
				netborder.Width = 0;
                netborder.Margin = new Thickness(0, 0, 0, 0);
            }
            else
			{
				Properties.Settings.Default.netspeed = true;
				cpuRefresh();
				netborder.Height = 100;
				netborder.Width = 200;
                netborder.Margin = new Thickness(0, 0, 0, 10);
            }

        }


		private void light(object sender, RoutedEventArgs e)
		{
			UI obj = new UI();
			obj.color1 = "#99CCCCCC";
			obj.color2 = "#FF000000";
			obj.color3 = "#FF2F7FD6";
			DataContext = obj;
			Properties.Settings.Default.darkmode = "false";
		}

		private void dark(object sender, RoutedEventArgs e)
		{
			UI obj = new UI();
			obj.color1 = "#99000000";
			obj.color2 = "#FFFFFFFF";
			obj.color3 = "#FF0F184E";
			DataContext = obj;
			Properties.Settings.Default.darkmode = "true";
		}

		// Ressurcen CPU RAM =========================================================>
		public void cpuRefresh()
		{
			_ = getResUsageAsync();
		}

		int web;
		int del = 1800;

		public async Task getResUsageAsync()
		{

			PerformanceCounter ramCounter;

			ramCounter = new PerformanceCounter("Memory", "Available MBytes");

			ramusage.Content = rambar.Maximum - ramCounter.NextValue() + "MB";
			rambar.Value = rambar.Maximum - ramCounter.NextValue();

			await Task.Delay(del / 3);
			PerformanceCounter cpuCounter;

			cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

			cpuusage.Content = cpuCounter.NextValue() + "%";
			Thread.Sleep(100);

			double d = cpuCounter.NextValue();
			int i = (int)Math.Round(d);

			cpuusage.Content = i + "%";
			cpubar.Value = i;
			await Task.Delay(del / 3);

			// trigger Netzwerkgeschwindigkeit
			if (web == 1)
			{
				CheckSpeedWebClient();
				web += 1;
			}
			else
			{
				if (web != 20)
					web += 1;
				else web = 1;
			}

			if (netspeed.Content == null)
				netspeed.Content = "wird geladen...";

			await Task.Delay(del/3);

			_ = getResUsageAsync();

		}

		private void tbmin(object sender, RoutedEventArgs e)
		{
			taskbarslider.Value -= 1;
		}

		private void tbplus(object sender, RoutedEventArgs e)
		{
			taskbarslider.Value += 1;
		}

		private void taskscale(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			maingrid.Margin = new Thickness(0, 0, 0, taskbarslider.Value);
		}

		private void tasksliderenter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			taskscaler.Fill = new SolidColorBrush(Color.FromArgb(50, 234, 255, 84));
		}

		private void tasksliderleave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			taskscaler.Fill = new SolidColorBrush(Color.FromArgb(0, 234, 255, 84));
		}

		private void closepopup(object sender, RoutedEventArgs e)
		{
			popup.Content = null;
			popupgrid.Opacity = 0;
			popupf.Opacity = 0;
			Properties.Settings.Default.firststart = false;
		}

		// Internet Geschwindigkeit ================================>
		private void netspeedcheck(object sender, EventArgs e)
		{
			//netspeed.Content = "Down: " + Properties.Settings.Default.Download + " Mb/s Up:" + Properties.Settings.Default.Download + " Mb/s";
			//netping.Content = Properties.Settings.Default.Ping + " ms";

		}

		public async void CheckSpeedWebClient()
		{
			Thread thread = null;
			Process process = null;
			String output = string.Empty;
			thread = new Thread(new ThreadStart(() =>
			{
				try
				{
					process = new Process();

					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.CreateNoWindow = true;
					process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					process.StartInfo.FileName = "speedtest.exe";
					process.Start();
					output = process.StandardOutput.ReadToEnd();
					process.WaitForExit();

					output = output.Split(new string[] { "Download:" }, StringSplitOptions.None)[1];
					output = output.Split(new string[] { "Upload:" }, StringSplitOptions.None)[0].Trim();
					this.DownloadSpeed = output;
				}
				
				catch (Exception)
				{

					
				}
				finally
				{
					if (process != null) process.Dispose();
				}
			
			}
			));
			thread.Start();

			netspeed.Content = DownloadSpeed;


			netping.Content = Properties.Settings.Default.Ping;
			try
			{
				await Task.Delay(del / 4);

				WebClient testclient = new WebClient();
				testclient.DownloadFile("http://dl.google.com/googletalk/googletalk-setup.exe", @"net.temp");
			}
			catch
			{
				await Task.Delay(del / 4);
				netbar.Value = 0;
				netspeed.Content = "Keine Internetverbindung";
				netmax.Content = "";
				return;
			}
			finally
			{ 
				try
				{
					double[] speeds = new double[5];
					for (int i = 0; i < 5; i++)
					{
						await Task.Delay(del / 4);
						int jQueryFileSize = 1569;
						WebClient client = new WebClient();
						DateTime startTime = DateTime.Now;
						client.DownloadFile("http://dl.google.com/googletalk/googletalk-setup.exe", @"net.temp");
						DateTime endTime = DateTime.Now;
						speeds[i] = Math.Round(jQueryFileSize / (endTime - startTime).TotalSeconds, 2);
					}
					File.Delete(@"net.temp");

					var download = Math.Round(speeds.Average() * 0.008f, 1);
					//netspeed.Content = download + "Mb/s";
					//netbar.Value = download;
					//netbar.Maximum = download;
					//netbar.Maximum = Math.Round(netbar.Maximum * 1.5 / 50, 0) * 50;
					if (netbar.Maximum < download)
						netbar.Maximum += 50;
					//netmax.Content = netbar.Maximum + "Mb/s";
				}
				catch
				{
					await Task.Delay(del / 4);
					netbar.Value = 0;
					netspeed.Content = "Keine Internetverbindung";
					netmax.Content = "";
				}
			} 
		}

		public string DownloadSpeed { get; set; }
		
		public void GetInternetSpeedInfo2()
        {
			Thread thread = null;
			Process process = null;
			string output = string.Empty;
			thread = new Thread(new ThreadStart(() =>
			{
				try
				{
					process = new Process();

					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.CreateNoWindow = true;
					process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					process.StartInfo.FileName = "speedtest.exe";
					process.Start();
					output = process.StandardOutput.ReadToEnd();
					process.WaitForExit();
					output = output.Split(new string[] { "Download:" }, StringSplitOptions.None)[1];
					output = output.Split(new string[] { "Upload:" }, StringSplitOptions.None)[0].Trim();
					this.DownloadSpeed = output;
					netspeed.Content = DownloadSpeed;
				}
				catch (Exception)
				{
					if (thread != null)
						thread.Abort();
					System.Windows.MessageBox.Show("catch");
				}
				finally
				{
					if (process != null)
					{
						process.Close();
						process.Dispose();
					}
					System.Windows.MessageBox.Show("finally");
				}
			}
			));
			thread.Start();
        }

		protected void CheckSpeed(object sender, EventArgs e)
		{
			double[] speeds = new double[5];
			for (int i = 0; i < 5; i++)
			{
				int jQueryFileSize = 261; //Size of File in KB.
				WebClient client = new WebClient();
				DateTime startTime = DateTime.Now;
				client.DownloadData("http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.js");
				DateTime endTime = DateTime.Now;
				speeds[i] = Math.Round((jQueryFileSize / (endTime - startTime).TotalSeconds));
			}
			netspeed.Content = string.Format("{0}KB/s", speeds.Average());
			netbar.Value = speeds.Average();
			netbar.Maximum = Math.Round(netbar.Value * 1.5 / 50, 0) * 50;
			netmax.Content = Math.Round(netbar.Value * 1.5 / 50, 0) * 50 + "Mb/s";
			if (netbar.Maximum < netbar.Value)
				netbar.Maximum += 50;
		}

		private void netrefresh()
        {
			WebClient wc = new WebClient();
			try
			{
				downloaddata();
				DateTime dt1 = DateTime.Now;
				byte[] data = wc.DownloadData("http://google.com");
				DateTime dt2 = DateTime.Now;
				var net = Math.Round((data.Length / 1024) / (dt2 - dt1).TotalSeconds, 2);
				netspeed.Content = net + " Mb/s";
				netbar.Value = net;
				netbar.Maximum = Math.Round(net * 1.5 / 50, 0) * 50;
				netmax.Content = Math.Round(net * 1.5 / 50, 0) * 50 + "Mb/s";
				if (netbar.Maximum < netbar.Value)
					netbar.Maximum += 50;
			}
			catch { }
		}

        private void downloaddata()
        {
			WebClient wc = new WebClient();
			try
			{
				wc.DownloadData("http://google.com");
			}
			catch { }
		}

		bool c = true;

		private async void mouseenter(object sender, System.Windows.Input.MouseEventArgs e)
        {
			c = true;
			var d = 5;
			await Task.Delay(d);
			titlegrid.Height = 7;
			await Task.Delay(d);
			titlegrid.Height = 15;
			await Task.Delay(d);
			titlegrid.Height = 24;
			await Task.Delay(d);
			titlegrid.Height = 30;
			titlebartrigger.Height = 0;
			await Task.Delay(1000);
			if (c == true)
			{
				waitforhide();
			}
		}

        private async void waitforhide()
        {
			c = false;
			await Task.Delay(2000);
			if (titlegrid.Height == 30 && c == false && System.Windows.Application.Current.MainWindow.WindowState != WindowState.Normal && titlegrid.IsMouseOver != true)
			{
				var d = 10;
				await Task.Delay(d);
				titlegrid.Height = 30;
				await Task.Delay(d);
				titlegrid.Height = 28;
				await Task.Delay(d);
				titlegrid.Height = 21;
				await Task.Delay(d);
				titlegrid.Height = 14;
				await Task.Delay(d);
				titlegrid.Height = 7;
				await Task.Delay(d);
				titlegrid.Height = 0;
				titlebartrigger.Height = 10;
			}
		}

		private void mouseleave_teatlegrid(object sender, System.Windows.Input.MouseEventArgs e)
        {
			waitforhide();
        }

        private async void window_mouseenter(object sender, System.Windows.Input.MouseEventArgs e)
        {
			await Task.Delay(1000);
			if (window.WindowState != WindowState.Minimized)
				window.ShowInTaskbar = false;
        }

        private void windowclose(object sender, System.ComponentModel.CancelEventArgs e)
        {
			if (netborder.Height == 0)
				Properties.Settings.Default.netspeed = false;
			else Properties.Settings.Default.netspeed = true;
			

			if (System.Windows.Application.Current.MainWindow.WindowState != WindowState.Maximized)
			{
				Properties.Settings.Default.zoom = ansichtslider.Value;
				Properties.Settings.Default.height = window.Height;
				Properties.Settings.Default.width = window.Width;
			}

			Properties.Settings.Default.taskscale = taskbarslider.Value;
			Properties.Settings.Default.refresh = refreshtime.Value;

			Properties.Settings.Default.top = this.Top;
			Properties.Settings.Default.left = this.Left;


			Properties.Settings.Default.Save();
		}


        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            var hWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;

            // Set the window style to include WS_EX_NOACTIVATE
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);

            // Position the window behind other applications
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }
    }
}

