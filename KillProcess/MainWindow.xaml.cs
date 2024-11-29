using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace KillProcess
{
    public partial class MainWindow : Window
    {
        string path = "proc.txt";
        private bool ongoing = false;


        public MainWindow()
        {
            try
            {
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, "!НЕ БАЛОВАТЬСЯ!\n");
                }
                InitializeComponent();
                this.Show();


            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Ошибка при запуске приложения", MessageBoxButton.OK);
                this.Close();
            }
            VisualStyle();

        }





        private void KillProcess()
        {
            DateTime starttime = DateTime.Now;
            async Task LoopAsync()
            {

                while (ongoing)
                {
                    foreach (string name in File.ReadAllLines(path))
                    {
                        try
                        {
                            if (name.Split(" ")[0] == "kill")
                            {
                                Process[] proc = Process.GetProcessesByName(name.Split(" ")[1].Trim());
                                foreach (var process in proc)
                                {
                                    try
                                    {
                                        process.Kill();
                                    }
                                    catch
                                    {
                                        var converter = new BrushConverter();
                                        ongoing = false;
                                        Start.Content = "Старт";
                                        Start.BorderBrush = (Brush)converter.ConvertFrom("#7160e8");
                                    }
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }
                        //CounterLabel.Content = new DateTime((DateTime.Now - starttime).Ticks).ToString("mm:ss");

                    }
                    await Task.Delay(1000);
                }
            }



            _ = LoopAsync().ContinueWith(t =>
            {
            });
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {

            var converter = new BrushConverter();
            if (!ongoing)
            {
                int c = 0;
                //CounterLabel.Visibility = Visibility.Visible;
                foreach (CheckBox t in PROCESSESList.Items)
                {
                    if (t.IsChecked == true)
                    {
                        c++;
                        break;
                    }
                }
                if (c > 0)
                {
                    ongoing = true;
                    Start.Content = " Вы в\nпотоке";
                    Start.FontSize = 30;
                    Start.BorderBrush = (Brush)converter.ConvertFrom("#49c8a1");
                    KillProcess();
                }
                else
                {
                    MessageBox.Show("Не выбрано ни одного процесса", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                //CounterLabel.Visibility = Visibility.Hidden;
                ongoing = false;
                Start.Content = "Старт";
                Start.FontSize = 35;
                Start.BorderBrush = (Brush)converter.ConvertFrom("#7160e8");
            }

        }

        private void OpenOptions_Click(object sender, RoutedEventArgs e)
        {
            if (OPTIONSBar.Visibility == Visibility.Visible)
            {
                OPTIONSBar.Visibility = Visibility.Hidden;
                AdditionalTOOLBar.Visibility = Visibility.Hidden;
                FINDWindow.Visibility = Visibility.Hidden;

            }
            else
            {
                OPTIONSBar.Visibility = Visibility.Visible;
                AdditionalTOOLBar.Visibility = Visibility.Visible;
                FINDWindow.Visibility = Visibility.Hidden;
                INFOBar.Visibility = Visibility.Hidden;
                PersonalizationBar.Visibility = Visibility.Hidden;
            }
        }



        private void OpenInfo_Click(object sender, RoutedEventArgs e)
        {
            if (INFOBar.Visibility == Visibility.Visible)
            {
                INFOBar.Visibility = Visibility.Hidden;
                PersonalizationBar.Visibility = Visibility.Hidden;
            }
            else
            {
                INFOBar.Visibility = Visibility.Visible;
                AdditionalTOOLBar.Visibility = Visibility.Hidden;
                FINDWindow.Visibility = Visibility.Hidden;
                OPTIONSBar.Visibility = Visibility.Hidden;
            }

        }
        private void OpenBp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FINDBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FINDWindow.Visibility == Visibility.Visible)
            {
                FINDWindow.Visibility = Visibility.Hidden;
                FINDTextBox.Text = string.Empty;


            }
            else
            {
                FINDWindow.Visibility = Visibility.Visible;
                FINDTextBox.Text = string.Empty;

            }
        }

        private void FindProc(string text)
        {
            PROCESSESList.Items.Clear();

            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName.ToLower().Contains(text.ToLower()))
                {
                    CheckBox box = new CheckBox();
                    foreach (string name in File.ReadAllLines(path))
                    {
                        try
                        {
                            if (name.Split(" ")[1] == proc.ProcessName)
                            {
                                box.IsChecked = true;
                            }
                        }
                        catch { }

                    }
                    box.FontFamily = new FontFamily("Comic Sans MS");
                    box.FontSize = 15;
                    box.Foreground = ThemesLabel.Foreground;
                    box.Content = proc.ProcessName;
                    box.Checked += Check;
                    box.Unchecked += UnCheck;

                    PROCESSESList.Items.Add(box);
                }
                
            }
        }

        private void Check(object sender, RoutedEventArgs e)
        {
            CheckBox chBox = (CheckBox)sender;
            if (ongoing)
            {
                MessageBox.Show("Сначала выйдите из потока", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                chBox.IsChecked = false;
            }
            else
            {
                File.AppendAllText(path, $"kill {chBox.Content.ToString()}\n");
            }


        }
        private void UnCheck(object sender, RoutedEventArgs e)
        {

            CheckBox chBox = (CheckBox)sender;
            if (ongoing)
            {
                MessageBox.Show("Сначала выйдите из потока", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                chBox.IsChecked = true;
            }
            else
            {

                string text = File.ReadAllText(path);
                string tmp = $"kill {chBox.Content.ToString()}";
                text = text.Replace(tmp, null);

                File.WriteAllText(path, text);
            }


        }


        private void FINDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FindProc(FINDTextBox.Text);
        }

        private void GitHubBtn_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/sudzekai/ProcessKiller";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        void layoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void VisualStyle()
        {
            var converter = new BrushConverter();
            Style st = new Style();
            st.Setters.Add(new Setter { Property = BorderBrushProperty, Value = (Brush)converter.ConvertFrom("#7160e8") });
            st.Setters.Add(new Setter { Property = FontFamilyProperty, Value = new FontFamily("Comic Sans MS") });

            // background //
            MainGrid.Background = (Brush)converter.ConvertFrom("#1e1e1e");
            PROCESSESList.Background = (Brush)converter.ConvertFrom("#1e1e1e");
            FINDTextBox.Background = (Brush)converter.ConvertFrom("#1e1e1e");


            // borders //
            Border[] borders = { TopBorder, TOOLBar, AdditionalTOOLBar, INFOBar, OPTIONSBar, PersonalizationBar, FINDWindow };
            foreach (Border b in borders)
            {
                b.Style = st;
                b.Background = (Brush)converter.ConvertFrom("#1e1e1e");
            }

            // buttons //
            Button[] buttons = { Start, FINDBtn, GLobalOptionsBtn, PersonalizationBtn, GitHubBtn };
            foreach (Button button in buttons)
            {
                button.Style = st;
            }

            // labels //
            Label[] labels = { TitleLabel, ThemesLabel, ProcessesListLabel };
            foreach (Label label in labels)
            {
                label.Foreground = Brushes.White;
            }

            // other //
            Start.Foreground = Brushes.White;
            SvgImagesLoad();
            FindProc(string.Empty);

        }

        private void VisualStyle(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Style st = new Style();
            st.Setters.Add(new Setter { Property = BorderBrushProperty, Value = btn.BorderBrush });
            st.Setters.Add(new Setter { Property = FontFamilyProperty, Value = btn.FontFamily });
            st.Setters.Add(new Setter { Property = ForegroundProperty, Value = btn.Foreground });

            // background //
            MainGrid.Background = btn.Background;
            PROCESSESList.Background = btn.Background;
            FINDTextBox.Background = btn.Background;
            

            // borders //
            Border[] borders = { TopBorder, TOOLBar, AdditionalTOOLBar, INFOBar, OPTIONSBar, PersonalizationBar, FINDWindow };
            foreach (Border b in borders)
            {
                b.Style = st;
                b.Background = btn.Background;
            }

            // buttons //
            Button[] buttons = { Start, FINDBtn, GLobalOptionsBtn, PersonalizationBtn, GitHubBtn };
            foreach (Button button in buttons)
            {
                button.Style = st;
            }

            // labels //
            Label[] labels = { TitleLabel,ThemesLabel, ProcessesListLabel };
            foreach (Label label in labels)
            {
                label.Foreground = btn.Foreground;
            }

            // other //
            Start.Foreground = btn.Foreground;
            SvgImagesLoad();
            FindProc(string.Empty);
        }

        private void PersonalizationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PersonalizationBar.Visibility == Visibility.Visible)
            {
                PersonalizationBar.Visibility = Visibility.Hidden;
            }
            else
            {
                PersonalizationBar.Visibility = Visibility.Visible;
            }
        }

        string[] shapes = { 
                //OptBtn
                "M11 15H17V17H11V15M9 7H7V9H9V7M11 13H17V11H11V13M11 9H17V7H11V9M9 11H7V13H9V11M21 5V19C21 20.1 20.1 21 19 21H5C3.9 21 3 20.1 3 19V5C3 3.9 3.9 3 5 3H19C20.1 3 21 3.9 21 5M19 5H5V19H19V5M9 15H7V17H9V15Z",
                //FINDBtn
                "M9.5,3A6.5,6.5 0 0,1 16,9.5C16,11.11 15.41,12.59 14.44,13.73L14.71,14H15.5L20.5,19L19,20.5L14,15.5V14.71L13.73,14.44C12.59,15.41 11.11,16 9.5,16A6.5,6.5 0 0,1 3,9.5A6.5,6.5 0 0,1 9.5,3M9.5,5C7,5 5,7 5,9.5C5,12 7,14 9.5,14C12,14 14,12 14,9.5C14,7 12,5 9.5,5Z",
                //PersonalizationBtn
                "M20.71,4.63L19.37,3.29C19,2.9 18.35,2.9 17.96,3.29L9,12.25L11.75,15L20.71,6.04C21.1,5.65 21.1,5 20.71,4.63M7,14A3,3 0 0,0 4,17C4,18.31 2.84,19 2,19C2.92,20.22 4.5,21 6,21A4,4 0 0,0 10,17A3,3 0 0,0 7,14Z",
                //GitHubBtn
                "M12,2A10,10 0 0,0 2,12C2,16.42 4.87,20.17 8.84,21.5C9.34,21.58 9.5,21.27 9.5,21C9.5,20.77 9.5,20.14 9.5,19.31C6.73,19.91 6.14,17.97 6.14,17.97C5.68,16.81 5.03,16.5 5.03,16.5C4.12,15.88 5.1,15.9 5.1,15.9C6.1,15.97 6.63,16.93 6.63,16.93C7.5,18.45 8.97,18 9.54,17.76C9.63,17.11 9.89,16.67 10.17,16.42C7.95,16.17 5.62,15.31 5.62,11.5C5.62,10.39 6,9.5 6.65,8.79C6.55,8.54 6.2,7.5 6.75,6.15C6.75,6.15 7.59,5.88 9.5,7.17C10.29,6.95 11.15,6.84 12,6.84C12.85,6.84 13.71,6.95 14.5,7.17C16.41,5.88 17.25,6.15 17.25,6.15C17.8,7.5 17.45,8.54 17.35,8.79C18,9.5 18.38,10.39 18.38,11.5C18.38,15.32 16.04,16.16 13.81,16.41C14.17,16.72 14.5,17.33 14.5,18.26C14.5,19.6 14.5,20.68 14.5,21C14.5,21.27 14.66,21.59 15.17,21.5C19.14,20.16 22,16.42 22,12A10,10 0 0,0 12,2Z",
                //GlobalOptionsBtn
                "M 11.130859 1 C 10.185672 1 9.3584008 1.6770236 9.1699219 2.6015625 L 8.96875 3.5957031 C 7.9576597 3.9605887 7.0388923 4.4895343 6.2324219 5.1699219 L 5.2753906 4.8496094 C 4.3783829 4.5486135 3.3832049 4.9272373 2.9101562 5.7441406 A 1.0001 1.0001 0 0 0 2.9082031 5.7460938 L 2.0390625 7.2539062 C 1.5673844 8.0723528 1.7375841 9.1257327 2.4453125 9.7519531 L 3.203125 10.421875 C 3.1075862 10.937431 3 11.449451 3 12 C 3 12.550549 3.1075862 13.062569 3.203125 13.578125 L 2.4453125 14.248047 C 1.7378643 14.874019 1.5665404 15.926105 2.0371094 16.744141 A 1.0001 1.0001 0 0 0 2.0371094 16.746094 L 2.9082031 18.253906 C 3.3810509 19.07245 4.3797986 19.448292 5.2753906 19.148438 L 6.234375 18.828125 C 7.040366 19.508238 7.9567682 20.039638 8.9667969 20.404297 L 9.1699219 21.396484 C 9.358338 22.322698 10.185672 23 11.130859 23 L 12.869141 23 C 13.814328 23 14.641599 22.322976 14.830078 21.398438 L 15.03125 20.404297 C 16.041078 20.039682 16.959756 19.510031 17.765625 18.830078 L 18.722656 19.150391 C 19.618248 19.450245 20.618949 19.072451 21.091797 18.253906 L 21.960938 16.748047 L 21.960938 16.746094 C 22.431797 15.929058 22.261346 14.875304 21.554688 14.25 L 21.552734 14.248047 L 20.796875 13.578125 C 20.89246 13.062663 21 12.550549 21 12 C 21 11.449451 20.892414 10.937431 20.796875 10.421875 L 21.554688 9.7519531 C 22.262136 9.1259807 22.43346 8.0738949 21.962891 7.2558594 A 1.0001 1.0001 0 0 0 21.962891 7.2539062 L 21.091797 5.7460938 C 20.618949 4.9275505 19.620201 4.551708 18.724609 4.8515625 L 17.765625 5.171875 C 16.959634 4.4917616 16.043232 3.9603621 15.033203 3.5957031 L 14.830078 2.6035156 L 14.830078 2.6015625 C 14.64232 1.6750184 13.813741 1 12.869141 1 L 11.130859 1 z M 11.130859 3 L 12.869141 3 A 1.0001 1.0001 0 0 0 12.871094 3 L 13.175781 4.5019531 A 1.0001 1.0001 0 0 0 13.886719 5.265625 C 15.0359 5.5870803 16.064818 6.1897218 16.892578 7 A 1.0001 1.0001 0 0 0 17.910156 7.2324219 L 19.359375 6.7480469 L 20.228516 8.2519531 L 20.228516 8.2539062 L 19.080078 9.2695312 A 1.0001 1.0001 0 0 0 18.773438 10.265625 C 18.917495 10.830299 19 11.407457 19 12 C 19 12.592543 18.91749 13.169704 18.773438 13.734375 A 1.0001 1.0001 0 0 0 19.080078 14.730469 L 20.228516 15.746094 L 19.359375 17.253906 L 17.908203 16.767578 A 1.0001 1.0001 0 0 0 16.890625 17.001953 C 16.062865 17.812231 15.033946 18.41292 13.884766 18.734375 A 1.0001 1.0001 0 0 0 13.175781 19.498047 L 12.869141 21 L 11.130859 21 L 11.128906 20.998047 L 10.824219 19.498047 A 1.0001 1.0001 0 0 0 10.113281 18.734375 C 8.9641006 18.41292 7.9351818 17.810278 7.1074219 17 A 1.0001 1.0001 0 0 0 6.0898438 16.767578 L 4.640625 17.251953 L 3.7714844 15.748047 L 3.7714844 15.746094 L 4.9199219 14.730469 A 1.0001 1.0001 0 0 0 5.2265625 13.734375 C 5.0825046 13.169704 5 12.592543 5 12 C 5 11.407457 5.0825046 10.830296 5.2265625 10.265625 A 1.0001 1.0001 0 0 0 4.9199219 9.2695312 L 3.7714844 8.2539062 L 4.640625 6.7480469 L 4.640625 6.7460938 L 6.0898438 7.2324219 A 1.0001 1.0001 0 0 0 7.1074219 6.9980469 C 7.9366867 6.1871518 8.9632441 5.5870749 10.113281 5.265625 A 1.0001 1.0001 0 0 0 10.824219 4.5019531 L 11.130859 3 z M 12 8 C 10.75 8 9.6852256 8.5047556 9.0019531 9.2734375 C 8.3186806 10.042119 8 11.027778 8 12 C 8 12.972222 8.3186806 13.957881 9.0019531 14.726562 C 9.6852256 15.495245 10.75 16 12 16 C 13.25 16 14.314774 15.495244 14.998047 14.726562 C 15.681319 13.957882 16 12.972222 16 12 C 16 11.027778 15.681319 10.042119 14.998047 9.2734375 C 14.314774 8.5047556 13.25 8 12 8 z M 12 10 C 12.749999 10 13.185226 10.245244 13.501953 10.601562 C 13.81868 10.957882 14 11.472222 14 12 C 14 12.527778 13.81868 13.042119 13.501953 13.398438 C 13.185226 13.754755 12.749999 14 12 14 C 11.250001 14 10.814774 13.754756 10.498047 13.398438 C 10.18132 13.042118 10 12.527778 10 12 C 10 11.472222 10.18132 10.957881 10.498047 10.601562 C 10.814774 10.245245 11.250001 10 12 10 z",
                //OpenBP
                "M7.226249999999999 2.04125a0.3125 0.3125 0 0 1 0.5475 0L9.61875 5.543749999999999a0.625 0.625 0 0 0 0.9475 0.18375L13.239374999999999 3.4375a0.3125 0.3125 0 0 1 0.49875 0.324375l-1.77125 6.4037500000000005a0.625 0.625 0 0 1 -0.5974999999999999 0.45875H3.6312499999999996a0.625 0.625 0 0 1 -0.598125 -0.45875L1.2625 3.7624999999999997a0.3125 0.3125 0 0 1 0.49875 -0.324375l2.6725 2.29a0.625 0.625 0 0 0 0.9475 -0.18375z",
                //OpenInfo
                "M16,12A2,2 0 0,1 18,10A2,2 0 0,1 20,12A2,2 0 0,1 18,14A2,2 0 0,1 16,12M10,12A2,2 0 0,1 12,10A2,2 0 0,1 14,12A2,2 0 0,1 12,14A2,2 0 0,1 10,12M4,12A2,2 0 0,1 6,10A2,2 0 0,1 8,12A2,2 0 0,1 6,14A2,2 0 0,1 4,12Z",
                //CloseBtn
                "M6.99486 7.00636C6.60433 7.39689 6.60433 8.03005 6.99486 8.42058L10.58 12.0057L6.99486 15.5909C6.60433 15.9814 6.60433 16.6146 6.99486 17.0051C7.38538 17.3956 8.01855 17.3956 8.40907 17.0051L11.9942 13.4199L15.5794 17.0051C15.9699 17.3956 16.6031 17.3956 16.9936 17.0051C17.3841 16.6146 17.3841 15.9814 16.9936 15.5909L13.4084 12.0057L16.9936 8.42059C17.3841 8.03007 17.3841 7.3969 16.9936 7.00638C16.603 6.61585 15.9699 6.61585 15.5794 7.00638L11.9942 10.5915L8.40907 7.00636C8.01855 6.61584 7.38538 6.61584 6.99486 7.00636Z",
                //MinimizeBtn
                "M442,1049 L418,1049 C415.791,1049 414,1050.79 414,1053 C414,1055.21 415.791,1057 418,1057 L442,1057 C444.209,1057 446,1055.21 446,1053 C446,1050.79 444.209,1049 442,1049"
            };

        private void SvgImagesLoad()
        {

            Button[] buttons =
            {
                OptBtn,
                FINDBtn,
                PersonalizationBtn,
                GitHubBtn,
                GLobalOptionsBtn,
                OpenBp,
                OpenInfo,
                CloseBtn,
                MinimizeBtn
            };

            for (int i = 0; i < shapes.Count(); i++)
            {
                System.Windows.Shapes.Path image = new System.Windows.Shapes.Path();
                image.Data = Geometry.Parse(shapes[i]);
                if (buttons[i] != MinimizeBtn && buttons[i] != CloseBtn && buttons[i] != FINDBtn)
                {
                    image.Width = 45;
                    image.Height = 45;
                    image.Fill = ThemesLabel.Foreground;
                }
                else
                {
                    image.Width = 15;
                    image.Height = 15;
                    if (buttons[i] == CloseBtn)
                    {
                        image.Fill = Brushes.Red;
                    }
                    else if (buttons[i] == FINDBtn)
                    {
                        image.Height += 15;
                        image.Width += 15;
                        image.Fill = ThemesLabel.Foreground;
                    }
                    else
                    {
                        image.Fill = ThemesLabel.Foreground;
                    }
                }

                image.Stretch = Stretch.Uniform;
                buttons[i].Content = image;
            }
        }

    }

}