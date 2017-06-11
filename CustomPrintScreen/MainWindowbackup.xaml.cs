using System.Windows;
using Drawing = System.Drawing;
using DrawingImg = System.Drawing.Imaging;
using Forms = System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls;
using System;
using System.Diagnostics;

namespace HookSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowbackup : Window
    {
        byte SelectedScreen = 0;
        Drawing.Bitmap[] Bitmaps;
        string filename;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void OnPrintScreen()
        {
            Bitmaps = new Drawing.Bitmap[Forms.Screen.AllScreens.Length];
            filename = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute;
            //Filename.Text = filename;

            for (int i = 0; i < Forms.Screen.AllScreens.Length; i++)
            {
                Forms.Screen s = Forms.Screen.AllScreens[i];

                //Create a new bitmap.
                Bitmaps[i] = new Drawing.Bitmap(s.Bounds.Width,
                                   s.Bounds.Height,
                                   DrawingImg.PixelFormat.Format32bppArgb);

                // Create a graphics object from the bitmap.
                var gfxScreenshot = Drawing.Graphics.FromImage(Bitmaps[i]);

                // Take the screenshot from the upper left corner to the right bottom corner.
                gfxScreenshot.CopyFromScreen(s.Bounds.X,
                                            s.Bounds.Y,
                                            0,
                                            0,
                                            s.Bounds.Size,
                                            Drawing.CopyPixelOperation.SourceCopy);

                Image img = new Image();
                img.Source = BitmapToImageSource(Bitmaps[i]);

                Button btn = new Button();
                btn.Margin = new Thickness(8, 0, 0, 0);
                btn.Content = img;
                byte j = (byte)i;
                btn.Click += (sender, e) => { Debug.WriteLine(SelectedScreen); SelectedScreen = j; };

                imgs.Children.Add(btn);
            }
            Application.Current.MainWindow.Show();
        }

        BitmapImage BitmapToImageSource(Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Save the screenshot to the specified path that the user has chosen.
            string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Bitmaps[SelectedScreen].Save(DesktopPath + "/" + filename + ".png", DrawingImg.ImageFormat.Png);
        }
    }
}
