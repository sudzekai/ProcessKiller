using System;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
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
                PROCESSESListAdder();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Ошибка при запуске приложения", MessageBoxButton.OK);
                this.Close();
            }

        }

        private void PROCESSESListAdder()
        {
            for (int i = 0; i < Process.GetProcesses().Count(); i++)
            {
                Process proc = Process.GetProcesses()[i];
                CheckBox box = new CheckBox();
                try
                {
                    foreach (string name in File.ReadAllLines(path))
                    {
                        if (name.Split(" ")[1] == proc.ProcessName)
                        {
                            box.IsChecked = true;
                        }
                    }
                }
                catch   { };
                
                box.FontFamily = new FontFamily("Comic Sans MS");
                box.FontSize = 15;
                box.Foreground = Brushes.White;
                box.Content = proc.ProcessName;
                box.Checked += Check;
                box.Unchecked += UnCheck;


                PROCESSESList.Items.Add(box);
                

            }
            FindProc(FINDTextBox.Text);

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
                                        catch (Exception ex)
                                        {
                                            var converter = new BrushConverter();
                                            ongoing = false;
                                            Start.Content = "Старт";
                                            Start.BorderBrush = (Brush)converter.ConvertFrom("#7160e8");
                                        }
                                }
                            }
                        }
                        catch (Exception ex)
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
            }
        }



        private void OpenInfo_Click(object sender, RoutedEventArgs e)
        {
            if (INFOBar.Visibility == Visibility.Visible)
            {
                INFOBar.Visibility = Visibility.Hidden;
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

            string text = File.ReadAllText(path);
            string tmp = $"kill {chBox.Content.ToString()}";
            text = text.Replace(tmp, null);
            
            File.WriteAllText(path, text);

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
    }
}