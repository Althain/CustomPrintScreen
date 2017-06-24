using System;
using System.Windows;
using System.Windows.Controls;


namespace CustomPrintScreen
{
    /// <summary>
    /// Logika interakcji dla klasy SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        string DefaultDateFormat = "MMdd HHmm";
        public SettingsWindow()
        {
            InitializeComponent();
            StPopupOnOneMonitor.IsChecked = Settings.PopupOnOneMonitor;
            StAskForScreenName.IsChecked = Settings.AskForScreenName;
            StDirectory.Text = Settings.SaveDirectory;
            StDateFormat.Text = Settings.DateFormat;

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var forbiddenChars = new char[] { '/', '\\', ':', '*', '?', '<', '>', '|' };

            foreach (char c in forbiddenChars)
            {
                if (StDateFormat.Text.Contains(c.ToString()))
                {
                    MessageBox.Show("You can't use '" + c + "' in date format");
                    return;
                }
            }

            Settings.PopupOnOneMonitor = (bool)StPopupOnOneMonitor.IsChecked;
            Settings.AskForScreenName = (bool)StAskForScreenName.IsChecked;
            Settings.SaveDirectory = StDirectory.Text;

            if (StDateFormat.Text.Length == 0)
                Settings.DateFormat = DefaultDateFormat;
            else Settings.DateFormat = StDateFormat.Text;

            Settings.Save();

            Handler.mainWindow.Show();
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Handler.mainWindow.Show();
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                StDirectory.Text = dialog.SelectedPath + "\\";
            }
        }

        private void StDateFormat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StDateFormat.Text.Length == 0)
                StDateFormatPreview.Text = DateTime.Now.ToString(DefaultDateFormat);
            else StDateFormatPreview.Text = DateTime.Now.ToString(StDateFormat.Text);

        }
    }
}
