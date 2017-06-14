using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Shapes;
using Drawing = System.Drawing;
using DrawingImg = System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Diagnostics;

namespace CustomPrintScreen
{
    struct RecoverRatios
    {
        public double Width;
        public double Height;

        public RecoverRatios(double widthRatio, double heightRatio)
        {
            Width = widthRatio;
            Height = heightRatio;
        }
    }

    /// <summary>
    /// Logika interakcji dla klasy CropWindow.xaml
    /// </summary>
    public partial class CropWindow : Window
    {
        Drawing.Bitmap NewBitmap;
        Rectangle CropArea;
        Rectangle[] Faders = new Rectangle[4];
        Point startPoint;
        double BitmapHeightWidthRatio;
        RecoverRatios BitmapRecoverRatios;

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                BitmapHeightWidthRatio = Handler.Bitmaps[Id].Height * 1.0d / Handler.Bitmaps[Id].Width * 1.0d;

                canvas.Width = SystemParameters.PrimaryScreenWidth * 0.9d;
                canvas.Height = SystemParameters.PrimaryScreenHeight * 0.9d;

                BitmapRecoverRatios = new RecoverRatios();
                BitmapRecoverRatios.Width = Handler.Bitmaps[Id].Width / canvas.Width;
                BitmapRecoverRatios.Height = Handler.Bitmaps[Id].Height / canvas.Height;

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = Handler.BitmapToImageSource(Handler.Bitmaps[id]);
                canvas.Background = brush;
            }
        }

        int id = -1;

        public CropWindow()
        {
            InitializeComponent();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Handler.mainWindow.Show();
            Close();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            NewBitmap = new Drawing.Bitmap((int)CropArea.Width, (int)CropArea.Height);
            Drawing.Graphics g = Drawing.Graphics.FromImage(NewBitmap);

            Debug.WriteLine(startPoint.ToString());

            Drawing.Rectangle area = new Drawing.Rectangle(
                            (int)(startPoint.X* BitmapRecoverRatios.Width),
                            (int)(startPoint.Y* BitmapRecoverRatios.Height),
                            (int)(CropArea.Width* BitmapRecoverRatios.Width),
                            (int)(CropArea.Height* BitmapRecoverRatios.Height));


            g.DrawImage(Handler.Bitmaps[id], 0, 0, area, Drawing.GraphicsUnit.Pixel);
            NewBitmap.Save(Handler.GetFirstAvailableScreenName(), DrawingImg.ImageFormat.Png);

            g.Dispose();
            NewBitmap.Dispose();
            Handler.mainWindow.Show();
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

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CropArea == null)
            {
                startPoint = e.GetPosition(canvas);
                CropArea = new Rectangle
                {
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 1
                };

                // Crop area
                Canvas.SetLeft(CropArea, startPoint.X);
                Canvas.SetTop(CropArea, startPoint.X);
                canvas.Children.Add(CropArea);

                for (int i = 0; i < 4; i++)
                {
                    SolidColorBrush b = new SolidColorBrush(Color.FromArgb(155, 0,0, 0));

                    Faders[i] = new Rectangle()
                    {
                        Fill = b,
                        Width = 0,
                        Height = 0
                    };
                    canvas.Children.Add(Faders[i]);
                    Canvas.SetLeft(Faders[i], 0);
                    Canvas.SetTop(Faders[i], 0);
                }
                CalculateFaders();
            }

            if (startPoint.X > e.GetPosition(canvas).X || startPoint.Y > e.GetPosition(canvas).Y)
            {
                startPoint = e.GetPosition(canvas);

                CalculateFaders();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || CropArea == null)
                return;

            var pos = e.GetPosition(canvas);

            
            Faders[1].Height = pos.Y - Canvas.GetTop(Faders[1]) > 0 ? pos.Y - Canvas.GetTop(Faders[1]) : 0;

            Canvas.SetLeft(Faders[2], pos.X > startPoint.X ? pos.X : startPoint.X);
            Faders[2].Height = pos.Y - Canvas.GetTop(Faders[2]) > 0 ? pos.Y - Canvas.GetTop(Faders[2]) : 0;
            Faders[2].Width = canvas.Width - pos.X > 0 ? canvas.Width - pos.X : 0;

            Canvas.SetTop(Faders[3], pos.Y > startPoint.Y ? pos.Y : startPoint.Y);
            Faders[3].Height = pos.Y > startPoint.Y ? canvas.Height - pos.Y : canvas.Height - startPoint.Y;


            CropArea.Width = pos.X - startPoint.X  > 0 ? pos.X - startPoint.X : 1;
            CropArea.Height = pos.Y - startPoint.Y  > 0 ? pos.Y - startPoint.Y : 1;

            Canvas.SetLeft(CropArea, startPoint.X);
            Canvas.SetTop(CropArea, startPoint.Y);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine(startPoint.ToString());
        }

        void CalculateFaders()
        {
            // top rect
            Faders[0].Width = canvas.Width;
            Faders[0].Height = startPoint.Y - Canvas.GetTop(Faders[0]) > 0 ? startPoint.Y - Canvas.GetTop(Faders[0]) : 0;

            // left rect
            Canvas.SetTop(Faders[1], startPoint.Y);
            Faders[1].Width = startPoint.X - Canvas.GetLeft(Faders[1]) > 0 ? startPoint.X - Canvas.GetLeft(Faders[1]) : 0;

            // right rect
            Canvas.SetTop(Faders[2], startPoint.Y);

            // bottom rect
            Canvas.SetLeft(Faders[3], 0);
            Faders[3].Width = canvas.Width;
        }
    }
}
