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
    class Handler
    {
        public static List<Drawing.Bitmap> Bitmaps = new List<Drawing.Bitmap>();
        static List<Drawing.Graphics> bitmapsgraphics = new List<Drawing.Graphics>();
        public static bool AdvancedMode;
        public static string ShotTime;

        /// <summary>
        /// Creates print screens, divides them and saves in Handler.Bitmaps
        /// </summary>
        public static void CreateScreens()
        {
            for (int i = 0; i < Forms.Screen.AllScreens.Length; i++)
            {
                Forms.Screen s = Forms.Screen.AllScreens[i];

                //Create a new bitmap.
                Bitmaps.Add(new Drawing.Bitmap(s.Bounds.Width,
                                   s.Bounds.Height,
                                   DrawingImg.PixelFormat.Format32bppArgb));

                // Create a graphics object from the bitmap.
                bitmapsgraphics.Add(Drawing.Graphics.FromImage(Bitmaps[i]));

                // Take the screenshot from the upper left corner to the right bottom corner.
                bitmapsgraphics[bitmapsgraphics.Count-1].CopyFromScreen(s.Bounds.X,
                                            s.Bounds.Y,
                                            0,
                                            0,
                                            s.Bounds.Size,
                                            Drawing.CopyPixelOperation.SourceCopy);

                
            }
        }

        /// <summary>
        /// Saves the bitmap stored in Handler.Bitmaps list and pointed by id
        /// </summary>
        /// <param name="id">Pointer to index of an array</param>
        public static void SaveScreen(int id)
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
            ClearData();
        }

        /// <summary>
        /// Removes all the bitmaps and images from the app
        /// </summary>
        public static void ClearData()
        {
            for(int i = Bitmaps.Count-1; i >= 0; i--)
            {
                Bitmaps[i] = null;
                bitmapsgraphics[i] = null;
            }

            Bitmaps.Clear();
            bitmapsgraphics.Clear();
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            mw.imgs.Children.Clear();
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
            logo.UriSource = new Uri("pack://application:,,,/KeyboardHook;component/Resources/"+name);
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
