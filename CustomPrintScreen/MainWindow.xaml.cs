using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Controls;
using System;
using System.Windows.Media;

namespace CustomPrintScreen
{
    public partial class MainWindow : Window
    {
        private LowLevelKeyboardListener _listener;

        public MainWindow()
        {
            InitializeComponent();
            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;
            _listener.HookKeyboard();

            (InfoBtn.Content as Image).Width = Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.016f;
            (SettingsBtn.Content as Image).Width = Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.016f;
            (CloseBtn.Content as Image).Width = Forms.Screen.PrimaryScreen.WorkingArea.Width * 0.016f;

            Handler.mainWindow = this;
        }

        void _listener_OnKeyPressed(object sender, EventArgs e)
        {


            if (Handler.Bitmaps.Count == 0)
            {
                Handler.CreateScreens();
                DrawApplication();
            }
            _listener.UnHookKeyboard();
            _listener.HookKeyboard();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _listener.UnHookKeyboard();
        }

        /// <summary>
        /// Function draws the application basing on the screens
        /// </summary>
        public void DrawApplication()
        {
            int ScreensAmount = Forms.Screen.AllScreens.Length;

            // if user has one screen and doesn't want the window to pop up
            if (ScreensAmount == 1 && !Settings.PopupOnOneMonitor)
            {
                // in advanced mode user will be able to crop and save
                if (Handler.AdvancedMode)
                {
                    OpenCropWindow(0);
                }
                // other way the screen will be simply saved
                else
                {
                    Handler.SaveScreen(0);
                    return;
                }
            }
            else
            {
                for (int i = 0; i < ScreensAmount; i++)
                {
                    // dump will be used for onclick events in buttons
                    int dump = i;

                    if (Handler.AdvancedMode)
                    {
                        // Structure
                        // Every screenshot will be saved as grid's background and to the grid
                        // will be added a stackpanel which will hold two buttons for cropping
                        // and saving

                        #region Create grid

                        double ratio = Handler.Bitmaps[i].Height * 1.0d / Handler.Bitmaps[i].Width * 1.0d;

                        Grid grid = new Grid()
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            Width = SystemParameters.PrimaryScreenWidth * 0.9d / ScreensAmount * 1.0d,
                            Margin = new Thickness(8, 0, 0, 0)
                        };

                        grid.Height = grid.Width * ratio;
                        grid.Background = new ImageBrush(Handler.BitmapToImageSource(Handler.Bitmaps[i]));
                        imgs.Children.Add(grid);

                        #endregion

                        #region Create a StackPanel

                        StackPanel sp = new StackPanel()
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Orientation = Orientation.Horizontal

                        };

                        grid.Children.Add(sp);
                        #endregion

                        #region Create buttons for cropping and saving and add them to StackPanel

                        Button[] imgbuttons = new Button[2];
                        string[] imgspaths = new string[] { "crop.png", "save.png" };

                        for (int j = 0; j < 2; j++)
                        {
                            imgbuttons[j] = new Button()
                            {
                                Width = 100,
                                Height = 100,
                                Background = new SolidColorBrush(Color.FromArgb(130, 255, 255, 255)),
                                Margin = new Thickness(4, 0, 0, 0),
                                BorderThickness = new Thickness(4, 4, 4, 4),
                                Content = new Image()
                                {
                                    Source = Handler.LoadImage(imgspaths[j])
                                }
                            };

                            sp.Children.Add(imgbuttons[j]);
                        }

                        imgbuttons[0].Click += (sender, e) => { OpenCropWindow(dump); };

                        imgbuttons[1].Click += (sender, e) =>
                        {
                            Handler.SaveScreen(dump, false);
                        };

                        #endregion
                    }
                    else
                    {
                        // Stucture
                        // Every screenshot will be a buttons's content.
                        // By pressing the button you save the image
                        Button btn = new Button()
                        {
                            Margin = new Thickness(8, 0, 0, 0),
                            BorderThickness = new Thickness(4, 4, 4, 4),
                            Content = new Image()
                            {
                                Width = SystemParameters.PrimaryScreenWidth * 0.9f / ScreensAmount,
                                Source = Handler.BitmapToImageSource(Handler.Bitmaps[i])
                            }
                        };
                        btn.Click += (sender, e) =>
                        {
                            Handler.SaveScreen(dump);
                            Hide();
                        };

                        imgs.Children.Add(btn);
                    }
                }

                Topmost = true;
                Show();
                Activate();
            }
        }

        public void HideWindow(bool clearData = false)
        {
            Hide();
            if(clearData)
            {
                imgs.Children.Clear();
                Handler.ClearData();
            }
        }

        public void CloseWindow()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to quit the application?", "Exit Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
                Close();
        }

        void OpenCropWindow(int id)
        {
            Hide();
            Handler.cropWindow = new CropWindow();
            Handler.cropWindow.Id = id;
            Handler.cropWindow.Show();
        }

        void OpenInfoWindow()
        {
            Hide();
            Handler.infoWindow = new InfoWindow();
            Handler.infoWindow.Show();
        }

        void OpenSettingsWindow()
        {
            MessageBox.Show("Not done yet");
            return;

            Hide();
            Handler.settingsWindow = new SettingsWindow();
            Handler.settingsWindow.Show();
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

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void HideBtn_Click(object sender, RoutedEventArgs e)
        {
            HideWindow(true);
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenSettingsWindow();
        }

        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenInfoWindow();
        }
    }
}
