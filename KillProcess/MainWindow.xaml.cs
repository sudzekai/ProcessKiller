using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace KillProcess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private bool ongoing = false;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                this.Show();
                ProcListAppender();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Ошибка при запуске приложения", MessageBoxButton.OK);
            }

        }

        private void Timer()
        {

            async Task LoopAsync()
            {
                while (ongoing)
                {
                    foreach (string processName in ProcessList.Items)
                    {
                        Process[] proc = Process.GetProcessesByName(processName);
                        foreach (var process in proc)
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
                                if (processName.Length == 0)
                                {
                                    MessageBox.Show("Удалите пустую строку из списка процессов, либо перезапустите приложение", "Ошибка", MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show($"Процесс - {processName}\n{ex}", "Ошибка", MessageBoxButton.OK);
                                }

                                break;
                            }
                        }
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
                ongoing = true;
                Start.Content = "Отключить";
                Start.BorderBrush = (Brush)converter.ConvertFrom("#49c8a1");
                Timer();
            }
            else
            {
                ongoing = false;
                Start.Content = "Старт";
                Start.BorderBrush = (Brush)converter.ConvertFrom("#7160e8");
            }

        }

        private void ProcessList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ProcessList.Items.Remove(ProcessList.SelectedItem);
                ProcListToFile();
            }
        }

        private void InputProcessName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessList.Items.Add(InputProcessName.Text);
                InputProcessName.Text = null;
                ProcListToFile();
            }

        }

        private void OptionsBtn_Click(object sender, RoutedEventArgs e)
        {
            Options.Visibility = Visibility.Hidden;
        }

        private void OpenOptions_Click(object sender, RoutedEventArgs e)
        {
            Options.Visibility = Visibility.Visible;
        }

        private void ProcListAppender()
        {
            string path = "ProcessList.txt";
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "");
            }
            else
            {
                for (int i = 0; i < File.ReadAllText(path).Split(' ').Count(); i++)
                {
                    ProcessList.Items.Add(File.ReadAllText(path).Split()[i].Trim());
                }
            }


        }

        private void ProcListToFile()
        {
            string path = "ProcessList.txt";
            File.WriteAllText(path, "");
            string text = "";
            foreach (string item in ProcessList.Items)
            {
                text += item + " ";

            }
            File.AppendAllText(path, $"{text.TrimEnd()}");

        }
    }
}