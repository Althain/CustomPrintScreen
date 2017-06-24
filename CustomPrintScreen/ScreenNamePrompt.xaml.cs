using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;

namespace CustomPrintScreen
{
    /// <summary>
    /// Logika interakcji dla klasy ScreenNamePrompt.xaml
    /// </summary>
    public partial class ScreenNamePrompt : Window
    {
        public int BitmapId;
        bool IsNameCorrect = true;

        public ScreenNamePrompt()
        {
            InitializeComponent();

            FileNameInput.Focus();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if(IsNameCorrect)
            {
                if (Handler.cropWindow != null)
                    Handler.cropWindow.OutputCroppedImage(FileNameInput.Text);
                else Handler.OutputScreen(BitmapId, FileNameInput.Text);

                if(!Handler.AdvancedMode)
                    Handler.mainWindow.HideWindow(true);

                Close();
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Handler.cropWindow == null)
                Handler.mainWindow.Show();

            Close();
        }

        private void FileNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = FileNameInput.Text;
            IsNameCorrect = true;

            char[] forbiddenChars = new char[] { ' ', '.' };

            foreach(char c in forbiddenChars)
            {
                if(text.Contains(c))
                {
                    TriggerError("You can't use '" + c + "' in name");
                    break;
                }
            }

            if (IsNameCorrect)
                ErrorOutput.Visibility = Visibility.Collapsed;
        }

        void TriggerError(string msg)
        {
            IsNameCorrect = false;
            ErrorOutput.Text = msg;
            ErrorOutput.Visibility = Visibility.Visible;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Handler.namePrompt = null;
            e.Cancel = false;
        }
    }
}
