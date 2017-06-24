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
        public static DateTime ShotTime;

        public static MainWindow mainWindow;
        public static CropWindow cropWindow;
        public static InfoWindow infoWindow;
        public static SettingsWindow settingsWindow;
        public static ScreenNamePrompt namePrompt;

        /// <summary>
        /// Creates print screens, divides them into screens and saves in Handler.Bitmaps list
        /// </summary>
        public static void CreateScreens()
        {
            mainWindow.HideWindow();

            ShotTime = DateTime.Now;

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
            if (Settings.AskForScreenName)
            {
                if (namePrompt == null)
                {
                    namePrompt = new ScreenNamePrompt();
                    namePrompt.BitmapId = id;
                    namePrompt.Show();
                    namePrompt.Topmost = true;
                }
            }
            else
            {
                OutputScreen(id);

                if (hideAppAfter)
                    Handler.mainWindow.HideWindow(true);
            }
        }

        /// <summary>
        /// Outputs the screen to desktop with name of capture date
        /// </summary>
        /// <param name="id">Index of bitmap in array</param>
        /// <param name="filename">(w/o extension)If given, saves the screen with this name</param>
        public static void OutputScreen(int id, string filename = "")
        {
            // if parameter is default, than generate name basing on the date
            if (filename.Equals(""))
                filename = GenerateFilenameBasedOnDate();

            // If file with given name exists, than add a number at its end
            filename = GetAvailableName(filename);

            // Convert name to path on desktop and add extension
            filename = Settings.SaveDirectory + filename + ".png";

            // save
            Bitmaps[id]?.Save(filename, DrawingImg.ImageFormat.Png);
        }

        /// <summary>
        /// Returns a filename(w/o extension) basing on the date.
        /// </summary>
        /// <returns></returns>
        static string GenerateFilenameBasedOnDate()
        {
            string filename = ShotTime.ToString(Settings.DateFormat);

            filename = GetAvailableName(filename);

            return filename;
        }

        /// <summary>
        /// Returns name with additional number at the end if given name exists on desktop 
        /// </summary>
        /// <param name="basename">name of file w/o extension</param>
        /// <returns>Parameter basename with additional number at the end</returns>
        static string GetAvailableName(string basename)
        {
            for (int i = 0; ; i++)
            {
                string add = i > 0 ? i.ToString() : "";
                string fullname = Settings.SaveDirectory + basename + add + ".png";
                if (!File.Exists(fullname))
                {
                    return basename + add;
                }
            }
        }

        /// <summary>
        /// Returns string with path, available filename and extension
        /// </summary>
        /// <param name="filename">If not given, than it's date</param>
        /// <returns></returns>
        public static string GetFullPath(string filename = "")
        {
            if(filename.Equals(""))
                return Settings.SaveDirectory + GetAvailableName(ShotTime.ToString(Settings.DateFormat)) + ".png";
            else return Settings.SaveDirectory + GetAvailableName(filename) + ".png";
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
