using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace FileManager
{
    public partial class MainWindow : Window
    {
        private int SelectedIndex = -1;
        public List<ItemModel> Items { get; set; }
        private Stack<string> LastElements;
        public MainWindow()
        {
            Items = new List<ItemModel>();
            LastElements = new Stack<string>();
            InitializeComponent();
            ShowDirs.openDrives(Items);
            DataContext = this;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(ListView))
            {
                try
                {
                    ListView view = (ListView)sender;
                    int index = view.SelectedIndex;
                    ShowDirs.openFileOrDir(index, Items, LastElements);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    
                }
                finally
                {
                    phonesList.Items.Refresh();
                }
            }
        }

        public void delete(string path)
        {
            if(!Directory.Exists(path))
            {
                return;
            }
            if (Directory.Exists(path) && Directory.GetFileSystemEntries(path).Length==0)
            {
                Directory.Delete(path);
                return;
            }
            DirectoryInfo directory = new DirectoryInfo(path);
            string Pat = directory.ToString();
            if(directory.Exists)
            {
                FileInfo[] files = directory.GetFiles();
                if (files.Length>0)
                {
                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }
                }

                DirectoryInfo[] dirs = directory.GetDirectories();
                if(dirs.Length>0)
                {
                    foreach (var dir in dirs)
                    {
                        delete(dir.ToString());
                    }
                }
            }
            delete(path);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(LastElements.Count>0)
            {
                ShowDirs.openFileOrDir(LastElements.Pop(), Items);
                phonesList.Items.Refresh();
                if (LastElements.Count > 0) textBox.Text = Directory.GetParent((Items[0].Element.ToString())).ToString();
            }
        }

        private void phonesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LastElements.Count>0 && Items.Count>0) textBox.Text = Directory.GetParent((Items[0].Element.ToString())).ToString();
        }
    }

    public class ItemModel
    {
        public ItemModel(string name, string date, object Element)
        {
            this.name = name;
            lastWriteDate = date;
            this.Element = Element;
        }

        public ItemModel(string name, string date, object Element, ImageSource imageSource)
        {
            this.Image=imageSource;
            this.name = name;
            lastWriteDate = date;
            this.Element = Element;
        }

        public ItemModel(string name, string date,string extension, object Element, ImageSource imageSource)
        {
            this.name = name;
            this.extension = extension;
            this.Element = Element;
            this.Image = imageSource;
            lastWriteDate = date;
        }
        public string name { get; set; } = String.Empty;
        public string lastWriteDate { get; set; }=String.Empty;
        public object Element { get; set; } = null;
        public string extension { get; set; } = String.Empty;
        public ImageSource Image { get; set; } = null;
    }
}
