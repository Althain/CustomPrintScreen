using System.Windows;

namespace CustomPrintScreen
{
    /// <summary>
    /// Logika interakcji dla klasy CropWindow.xaml
    /// </summary>
    public partial class CropWindow : Window
    {
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                double ratio = Handler.Bitmaps[Id].Height * 1.0d / Handler.Bitmaps[Id].Width * 1.0d;
                Bitmap.Width = SystemParameters.PrimaryScreenWidth * 0.9d;
                Bitmap.Height = Bitmap.Width * ratio;
                Bitmap.Source = Handler.BitmapToImageSource(Handler.Bitmaps[id]);
            }
        }

        int id = -1;

        public CropWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            //If the Height has changed then calc half of the the offset to move the form
            if (sizeInfo.HeightChanged == true)
            {
                this.Top += (sizeInfo.PreviousSize.Height - sizeInfo.NewSize.Height) / 2;
            }

            //If the Width has changed then calc half of the the offset to move the form
            if (sizeInfo.WidthChanged == true)
            {
                this.Left += (sizeInfo.PreviousSize.Width - sizeInfo.NewSize.Width) / 2;
            }
        }
    }
}
