using System;
using System.Collections.Generic;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.IO;
using System.Linq;
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

namespace Launcher.Desktop
{
    /// <summary>
    /// Interaktionslogik für ItemGrid.xaml
    /// </summary>
    public partial class ItemGrid : Page
    {
        public static double Spacing { get; set; }
        public static double SpacingX { get; set; }
        public static double SpacingY { get; set; }
        public static int maxX { get; set; }
        public static int maxY { get; set; }
        public static int Size { get; set; }

        public ItemGrid()
        {
            InitializeComponent();

            Page.Width = Screen.PrimaryScreen.Bounds.Width;
            Page.Height = Screen.PrimaryScreen.Bounds.Height;

            Size = 80;
            Spacing = 20;
            SpacingX = Spacing;
            SpacingY = Spacing;
            maxX = (int)((int)Page.Width / (Size + (int)SpacingX) / 1.37);
            maxY = (int)Page.Height / (Size + (int)SpacingY);
            SpacingX = (Page.Width - (Size * maxX)) / maxX;
            SpacingY = (Page.Height - 50 - (maxY * Size)) / maxY;


            iArr = new Item[maxX, maxY];

            for (int i = 0; i < maxX; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    Item it = new Item();
                    it.Height = Size;
                    it.Width = Size*1.5;
                    it.PosX = i;
                    it.PosY = j;
                    //it.Text.Text = "x" + it.PosX + " y" + it.PosY;
                    it.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    it.VerticalAlignment = VerticalAlignment.Top;
                    it.Margin = new Thickness(SpacingX * i + i * Size + Spacing, SpacingY * j + j * Size + 10, 0, 0);
                    iArr[i, j] = it;
                    Grid.Children.Add(it);
                }
            }

            // Initialize Items =====================================================================================>

            // Get the directory
            string desktopPath =
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            DirectoryInfo place = new DirectoryInfo(desktopPath);

            // Using GetFiles() method to get list of all
            // the files present in the Train directory
            FileInfo[] Files = place.GetFiles();

            int index = 0;
            int x = 0;
            int y = 0;

            bool finished = false;
            while (finished == false && x <= maxX && index < Files.Length)
            {
                if (y < maxY)
                {
                    if (iArr[x, y].initialized == false)
                    {
                        string p = desktopPath + @"\" + Files[index].ToString();
                        iArr[x, y].Path = p;
                        iArr[x, y].initialized = true;
                        string s = Files[index].ToString();
                        s = System.IO.Path.GetFileNameWithoutExtension(s);
                        iArr[x, y].SetIcon(p);
                        iArr[x, y].Text.Text = s;
                        y++;
                        index++;
                    }
                    else { y++; }
                }
                else
                {
                    x++;
                    y = 0;
                }
            }
            foreach (Item item in iArr)
            {
                if (!item.initialized)
                    item.Disable();
            }
        }

        private static Item[,] iArr;


        private void ItemRow()
        {
            Item[] items = new Item[maxX];
            for (int i = 0; i < maxX; i++)
            {
                Item it = new Item();
                it.Height = Size;
                it.Width = Size;
                it.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                it.VerticalAlignment = VerticalAlignment.Top;
                it.Margin = new Thickness(SpacingX * i + i * Size + 10, 10, 0, 0);
                items[i] = it;
                Grid.Children.Add(it);

            }
        }

        // Disable when click on Desktop or disabled Item ===========================================>


        public static void Deselect()
        {
            for (int i = 0; i < maxX; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    iArr[i, j].SetBackgroundempty();
                    iArr[i, j].selected = false;
                }

            }
        }
    }
}
