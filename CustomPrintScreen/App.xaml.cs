using System;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace CustomPrintScreen
{
    public partial class App : Application
    {
        private MainWindow window;

        public App(KeyboardHook keyboardHook)
        {
            if (keyboardHook == null)
                throw new ArgumentNullException("keyboardHook");

            keyboardHook.KeyCombinationPressed += KeyCombinationPressed;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            window = new MainWindow();
        }

        void KeyCombinationPressed(object sender, EventArgs e)
        {
            Dispatcher?.Invoke(DispatcherPriority.Normal, new ThreadStart(PrintScreenExe));
        }

        void PrintScreenExe()
        {
            Handler.CreateScreens();
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            window.DrawApplication();
            window.Show();
            window.Activate();
        }
    }
}
