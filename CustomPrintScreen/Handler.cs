using System.Windows;
using Drawing = System.Drawing;
using DrawingImg = System.Drawing.Imaging;
using Forms = System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CustomPrintScreen
{
    /// <summary>
    /// This class is responisble for:
    /// Creating and holding screens, determining if the app is run in advanced mode and holds last capture time
    /// </summary>
    class Handler
    {
        /// <summary>
        /// Holds all bitmaps made when print screen is clicked
        /// </summary>
        public static List<Drawing.Bitmap> Bitmaps = new List<Drawing.Bitmap>();

        /// <summary>
        /// Determines the application mode
        /// </summary>
        public static bool AdvancedMode;

        /// <summary>
        /// Time when last print screen was made. Used for file save.
        /// </summary>
        static string ShotTime;

        /// <summary>
        /// Creates print screens, divides them into screens and saves in Handler.Bitmaps list
        /// </summary>
        public static void CreateScreens()
        {
            ShotTime = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute;

            for (int i = 0; i < Forms.Screen.AllScreens.Length; i++)
            {
                Forms.Screen s = Forms.Screen.AllScreens[i];

                // function CopyFromScreen doesn't work for fullscreen applications
                // Games are run in primary screens, when they are full screen
                // so if the screen is primary, than use low level print screen
                if (s.Primary)
                {
                    Bitmaps.Add(ScreenCapture.CaptureScreen());
                    continue;
                }

                //Create a new bitmap.
                Bitmaps.Add(new Drawing.Bitmap(s.Bounds.Width,
                                   s.Bounds.Height,
                                   DrawingImg.PixelFormat.Format32bppArgb));

                // Create a graphics object from the bitmap.
                var tmp = Drawing.Graphics.FromImage(Bitmaps[i]);
                tmp.CopyFromScreen(s.Bounds.X, s.Bounds.Y, 0, 0, s.Bounds.Size, Drawing.CopyPixelOperation.SourceCopy);
            }
        }

        /// <summary>
        /// Saves the bitmap stored in Handler.Bitmaps list and pointed by id
        /// </summary>
        /// <param name="id">Pointer to index of an array</param>
        public static void SaveScreen(int id, bool hideAppAfter = true)
        {
            string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            
            for (int i = 0; ; i++)
            {
                string add = i > 0 ? i.ToString() : "";
                string fullname = DesktopPath + "/" + ShotTime + add + ".png";
                if (!File.Exists(fullname))
                {
                    Bitmaps[id]?.Save(fullname, DrawingImg.ImageFormat.Png);
                    break;
                }
            }

            if (hideAppAfter)
            {
                MainWindow mw = (MainWindow)Application.Current.MainWindow;
                mw.Hide();
                mw.imgs.Children.Clear();
                ClearData();
            }
        }

        /// <summary>
        /// Removes all the bitmaps and images from the app
        /// </summary>
        public static void ClearData()
        {
            for(int i = Bitmaps.Count-1; i >= 0; i--)
            {
                Bitmaps[i].Dispose();
            }

            Bitmaps.Clear();
        }

        /// <summary>
        /// Loads an image from the Resources folder in solution
        /// </summary>
        /// <param name="name">Name of file with extension</param>
        /// <returns>BitmapImage of image</returns>
        public static BitmapImage LoadImage(string name)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri("pack://application:,,,/CustomPrintScreen;component/Resources/"+name);
            logo.EndInit();

            return logo;
        }

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource BitmapToImageSource(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }
    }
}
