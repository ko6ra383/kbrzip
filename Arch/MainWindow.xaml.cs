using HuffLib;
using Microsoft.Win32;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace Arch
{
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }
        private string soursePath;
        private string resPath;
        private bool compress = true;
        private void Switch(object sender, RoutedEventArgs e) {
            if (compress) {
                SwitchButton.Content = "Разархивация";
                compress = false;
            }
            else {
                SwitchButton.Content = "Архивация";
                compress = true;
            }
        }

        private void SelectSourcePath(object sender, RoutedEventArgs e) {
            var ofd = new OpenFileDialog();
            bool? res = ofd.ShowDialog();
            if (res != false) {
                soursePath = ofd.FileName;
            }
        }
        private void SelectResPath(object sender, RoutedEventArgs e) {
            var ofd = new SaveFileDialog();
            bool? res = ofd.ShowDialog();
            if (res != false) {
                resPath = ofd.FileName;
            }
        }
        private void Commit(object sender, RoutedEventArgs e) {
            if (soursePath != null && resPath != null) {
                if (compress) Huffman.Compress(soursePath, resPath);
                else Huffman.Decompress(soursePath, resPath);
            }
            else {
                CommitButton.Content = "ОШИБКА";
                CommitButton.Foreground = Brushes.Red;
                Thread.Sleep(2000);
                //CommitButton.Content = "Выполнить";
                CommitButton.Foreground = Brushes.Black;
            }
        }
    }
}
