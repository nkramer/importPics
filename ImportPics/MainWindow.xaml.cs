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

namespace ImportPics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> list = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
            tb.Focus();
            tb.SelectAll();
            lb.ItemsSource = list;
            //Import("test ");
        }

        private void Import(string description)
        {
            description = description.Trim();
            //Directory.CreateDirectory(@"")

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
                        list.Add("Create " + targetDir);
                        Debug.WriteLine("Create " + targetDir);
                        Directory.CreateDirectory(targetDir);
                    }

                    string newName = Path.GetFileName(file);
                    newName = newName.Replace("DSC", prefix + " ");
                    newName = newName.Replace("101_", prefix + " ");
                    string newPath = Path.Combine(targetDir, newName);

                    list.Add(file + " -> " + newPath);
                    Debug.WriteLine(file + " -> " + newPath);
                    File.Copy(file, newPath);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Import(tb.Text);
        }
    }
}
