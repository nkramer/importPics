using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace ImportPics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> list = new ObservableCollection<string>();

        private static MainWindow Instance;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            tb.Focus();
            tb.SelectAll();
            lb.ItemsSource = list;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string text = tb.Text;
            var dispatcher = this.Dispatcher;
            new Thread(() => BackgroundImport(text, dispatcher)).Start();
        }

        private void AddTextToUi(string line)
        {
            list.Add(line);
            Debug.WriteLine(line);
        }

        private static void BackgroundAddTextToUi(string line, Dispatcher d)
        {
            Action a = new Action(() => Instance.AddTextToUi(line));
            d.Invoke(a);
        }

        private static void BackgroundImport(string description, Dispatcher d)
        {
            description = description.Trim();

            var dirs = Directory.EnumerateDirectories(@"h:\dcim");
            foreach (var dir in dirs)
            {
                var files = Directory.EnumerateFiles(dir);
                foreach (var file in files)
                {
                    DateTime date = File.GetCreationTime(file);
                    string dateStr = date.ToString("yyyy-MM-dd");
                    string prefix = dateStr + " " + description;
                    string targetDir = Path.Combine(@"E:\Pictures", prefix);
                    if (!Directory.Exists(targetDir)) {
                        BackgroundAddTextToUi("Create " + targetDir, d);
                        Directory.CreateDirectory(targetDir);
                    }

                    string newName = Path.GetFileName(file);
                    newName = newName.Replace("DSC", prefix + " ");
                    newName = newName.Replace("101_", prefix + " ");
                    string newPath = Path.Combine(targetDir, newName);

                    BackgroundAddTextToUi(file + " -> " + newPath, d);
                    Debug.WriteLine(file + " -> " + newPath);
                    File.Copy(file, newPath);
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
