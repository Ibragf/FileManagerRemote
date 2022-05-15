using FileManager.NetworkTCP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Newtonsoft.Json;
using FileManager.Models;

namespace FileManager
{
    public partial class MainWindow : Window
    {
        int port=8005;//изменить
        string ip= "192.168.0.15";//изменить
        private ServerFileManager server;
        private ClientFileManager client;
        private Dictionary<string, string> computers;
        Thread serverThread;
        CancellationTokenSource tokenSource;
        public List<ItemModel> Items { get; set; }
        private Stack<string> LastElements;
        private DeserializedModels models;
        public MainWindow()
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken token= tokenSource.Token;
            server = new ServerFileManager(System.Net.IPAddress.Parse(ip), port);
            serverThread = new Thread(new ParameterizedThreadStart(server.Listen));
            serverThread.Start(token);

            Items = new List<ItemModel>();
            LastElements = new Stack<string>();

            InitializeComponent();

            DataContext = this;
        }

        private void showComputers()
        {
            string[] ID=computers.Keys.ToArray();
            string[] name=computers.Values.ToArray();
            Items.Clear();
            for(int i=0;i<ID.Length;i++)
            {
                ItemModel item=new ItemModel(name[i],ID[i],ID);
                Items.Add(item);
                phonesList.Items.Refresh();
            }
        }

        private async void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            #region oldCode
            /*if (sender.GetType() == typeof(ListView))
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
            }*/
            #endregion
            if(sender is ListView view)
            {
                try
                {
                    int index= view.SelectedIndex;
                    if(index != -1)
                    {
                        if(Items[index].Element is string)
                        {
                            client=server.GetComputer(Items[index].lastWriteDate);
                            await client.SendCommandAsync(Commands.Open, "drives");
                        }

                        string response = await client.GetResponseAsync();
                        if (response != null)
                        {
                            models = JsonConvert.DeserializeObject<DeserializedModels>(response);
                            Items.Clear();
                            foreach(var item in models.items)
                            {
                                Items.Add(item);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    phonesList.Items.Refresh();
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            computers = server.GetComputers();//временно
            showComputers();//временно
            if (LastElements.Count>0)
            {
                ShowDirs.openFileOrDir(LastElements.Pop(), Items);
                phonesList.Items.Refresh();
                if (LastElements.Count > 0) textBox.Text = Directory.GetParent(Items[0].Element.ToString()).ToString();
            }
        }

        private void phonesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LastElements.Count>0 && Items.Count>0) textBox.Text = Directory.GetParent(Items[0].Element.ToString()).ToString();
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            server.Dispose();
            tokenSource.Cancel();
            tokenSource.Dispose();
        }
        public void delete(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            if (Directory.Exists(path) && Directory.GetFileSystemEntries(path).Length == 0)
            {
                Directory.Delete(path);
                return;
            }
            DirectoryInfo directory = new DirectoryInfo(path);
            string Pat = directory.ToString();
            if (directory.Exists)
            {
                FileInfo[] files = directory.GetFiles();
                if (files.Length > 0)
                {
                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }
                }

                DirectoryInfo[] dirs = directory.GetDirectories();
                if (dirs.Length > 0)
                {
                    foreach (var dir in dirs)
                    {
                        delete(dir.ToString());
                    }
                }
            }
            delete(path);
        }
    }
}
