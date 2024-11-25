using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;


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
                    File.WriteAllText(path, "");
                }
                InitializeComponent();
                this.Show();
                PROCESSESListAdder();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Ошибка при запуске приложения", MessageBoxButton.OK);
            }

        }

        private void PROCESSESListAdder()
        {
            for (int i = 0; i < Process.GetProcesses().Count(); i++)
            {
                Process proc = Process.GetProcesses()[i];
                CheckBox box = new CheckBox();
                foreach (string name in File.ReadAllLines(path))
                {
                    if (name == proc.ProcessName)
                    {
                        box.IsChecked = true;
                    }
                }
                box.FontFamily = new FontFamily("Comic Sans MS");
                box.FontSize = 15;
                box.Foreground = Brushes.White;
                box.Content = proc.ProcessName;
                box.Checked += Check;
                box.Unchecked += UnCheck;


                PROCESSESList.Items.Add(box);

            }

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
                        
                        //CounterLabel.Content = new DateTime((DateTime.Now - starttime).Ticks).ToString("mm:ss");
                        Process[] proc = Process.GetProcessesByName(name.Trim());
                        foreach (var process in proc)
                        {
                            if (name.Length == 0)
                            {
                                continue;
                            }
                            else
                            {
                                try
                                {
                                    process.Kill();
                                }
                                catch (Exception ex)
                                {
                                    var converter = new BrushConverter();
                                    ongoing = false;
                                    Start.Content = "Старт";
                                    Start.BorderBrush = (Brush)converter.ConvertFrom("#7160e8");
                                }

                            }
                        }
                        await Task.Delay(1000);
                    }
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
                //CounterLabel.Visibility = Visibility.Visible;
                ongoing = true;
                Start.Content = " Вы в\nпотоке";
                Start.FontSize = 25;
                Start.BorderBrush = (Brush)converter.ConvertFrom("#49c8a1");
                KillProcess();
            }
            else
            {
                //CounterLabel.Visibility = Visibility.Hidden;
                ongoing = false;
                Start.Content = "Старт";
                Start.FontSize = 25;
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
            }
        }

        private void OpenBp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OpenInfo_Click(object sender, RoutedEventArgs e)
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

        private void FINDTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FindProc(FINDTextBox.Text);
            }
        }

        private void FindProc(string text)
        {
            PROCESSESList.Items.Clear();

            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName.ToLower().StartsWith(text.ToLower()))
                {
                    CheckBox box = new CheckBox();
                    foreach (string name in File.ReadAllLines(path))
                    {
                        if (name == proc.ProcessName)
                        {
                            box.IsChecked = true;
                        }
                    }
                    box.FontFamily = new FontFamily("Comic Sans MS");
                    box.FontSize = 15;
                    box.Foreground = Brushes.White;
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
            File.AppendAllText(path, $"{chBox.Content.ToString()}\n");

        }
        private void UnCheck(object sender, RoutedEventArgs e)
        {
            CheckBox chBox = (CheckBox)sender;

            string text = File.ReadAllText(path);
            text = text.Replace(chBox.Content.ToString(), null);
            File.WriteAllText(path, text);

        }
    }
}