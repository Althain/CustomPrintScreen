using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPrintScreen
{
    class Settings
    {
        public static bool PopupOnOneMonitor = true;
        public static bool AskForScreenName = true;

        /// <summary>
        /// Directory where image will besaved (desktop by default)
        /// </summary>
        public static string SaveDirectory;
        public static string DateFormat = "MMdd HHmm";

        public static void Load()
        {
            PopupOnOneMonitor = Properties.Settings.Default.PopupOnOneMonitor;
            AskForScreenName = Properties.Settings.Default.AskForScreenName;

            if (Properties.Settings.Default.SaveDirectory.Length == 0)
                SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
            else SaveDirectory = Properties.Settings.Default.SaveDirectory;

            DateFormat = Properties.Settings.Default.DateFormat;

        }

        public static void Save()
        {
            Properties.Settings.Default.PopupOnOneMonitor = PopupOnOneMonitor;
            Properties.Settings.Default.AskForScreenName = AskForScreenName;
            Properties.Settings.Default.SaveDirectory = SaveDirectory;
            Properties.Settings.Default.DateFormat = DateFormat;
            Properties.Settings.Default.Save();
        }
    }
}
