using System;
using System.Configuration;
using System.Linq;

namespace CustomPrintScreen
{
    class Settings
    {
        public static bool PopupOnOneMonitor;
        public static bool AskForScreenName;

        /// <summary>
        /// Directory where image will besaved (desktop by default)
        /// </summary>
        public static string SaveDirectory;
        public static string DateFormat;

        public static void Load()
        {
            if (DoesSettingExist("PopupOnOneMonitor"))
                PopupOnOneMonitor = Properties.Settings.Default.PopupOnOneMonitor;
            else PopupOnOneMonitor = false;

            if (DoesSettingExist("AskForScreenName"))
                AskForScreenName = Properties.Settings.Default.AskForScreenName;
            else AskForScreenName = false;

            if (DoesSettingExist("SaveDirectory") && Properties.Settings.Default.SaveDirectory.Length == 0)
                SaveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
            else SaveDirectory = Properties.Settings.Default.SaveDirectory;

            if (DoesSettingExist("DateFormat"))
                DateFormat = Properties.Settings.Default.DateFormat;
            else DateFormat = "MMdd HHmm";

            Save();
        }

        public static void Save()
        {
            Properties.Settings.Default.PopupOnOneMonitor = PopupOnOneMonitor;
            Properties.Settings.Default.AskForScreenName = AskForScreenName;
            Properties.Settings.Default.SaveDirectory = SaveDirectory;
            Properties.Settings.Default.DateFormat = DateFormat;
            Properties.Settings.Default.Save();
        }

        static bool DoesSettingExist(string settingName)
        {
            return Properties.Settings.Default.Properties.Cast<SettingsProperty>().Any(prop => prop.Name == settingName);
        }
    }
}
