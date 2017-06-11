using System.Windows.Input;

namespace CustomPrintScreen
{
    public class EntryPoint
    {
        [System.STAThreadAttribute]
        static void Main()
        {
            using (var hook = new KeyboardHook { SelectedKey = Key.PrintScreen })
            {
                var app = new App(hook);
                app.InitializeComponent();
                app.Run();
            }
        }

    }
}
