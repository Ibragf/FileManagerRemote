using FileManager.NetworkTCP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using FileManager.Models;
using System.Net.Sockets;
using FileManager.ViewModel;
using System.Diagnostics;

namespace FileManager
{
    public partial class MainWindow : Window
    {
        int port=8005;//изменить
        string ip= "192.168.0.16";//изменить
        private ServerFileManager server;
        private ClientFileManager client;
        private Dictionary<string, string> computers;
        Thread serverThread;
        CancellationTokenSource tokenSource;
        public List<Model> Items { get; set; }
        private SerializableClass serializableClass;
        private Stack<string> LastElements;
        private FileViewModel viewModel;
        public MainWindow()
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken token= tokenSource.Token;
            /*server = new ServerFileManager(System.Net.IPAddress.Parse(ip), port);
            serverThread = new Thread(new ThreadStart(server.Listen));
            serverThread.Start(token);*/

            Items = new List<Model>();
            LastElements = new Stack<string>();

            InitializeComponent();
            viewModel = new FileViewModel(this);

            DataContext = viewModel;
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
                #region oldcode
                /*
                try
                {
                    int index= view.SelectedIndex;
                    var objec=view.SelectedItem;
                    if(index != -1)
                    {
                        string response=String.Empty;

                        if(Items[index] is CompModel comp)
                        {
                            client=server.GetComputer(comp.ID);
                            await client.SendCommandAsync(Command.Open, "", "drives");
 
                            response = await client.GetResponseAsync();
                        }

                        if(Items[index] is DriveModel drive)
                        {
                            await client.SendCommandAsync(Command.Open, drive.Path, drive.Type);
                            response = await client.GetResponseAsync();
                        }

                        if(Items[index] is DirectoryModel directory)
                        {
                            await client.SendCommandAsync(Command.Open, directory.Path, directory.Type);
                            response=await client.GetResponseAsync();
                        }
                       

                        if(!string.IsNullOrEmpty(response))
                        {
                            Thread.Sleep(10);
                            serializableClass = JsonConvert.DeserializeObject<SerializableClass>(response);
                            Items.Clear();
                            if(serializableClass.Drivers!=null)
                            {
                                foreach (var item in serializableClass.Drivers)
                                {
                                    Items.Add(item);
                                }
                            }
                            if(serializableClass.Folders != null)
                            {
                                foreach (var item in serializableClass.Folders)
                                {
                                    Items.Add(item);
                                }
                            }
                            if(serializableClass.Files!= null)
                            {
                                int i = 0;
                                foreach (var item in serializableClass.Files)
                                {
                                    Items.Add(item);
                                    item.createImageSource(i);
                                    i++;
                                }
                            }
                        }
                    }
                }
                catch(SocketException ex)
                {
                    client.Dispose();
                    showComputers();
                    MessageBox.Show((int)ex.SocketErrorCode+":"+ex.SocketErrorCode.ToString());
                    //MessageBox.Show(ex.ErrorCode.ToString());
                    //MessageBox.Show(ex.Message+"\n"+ex.StackTrace);
                }
                catch (Exception ex)
                {
                    client.Dispose();
                    showComputers();
                    MessageBox.Show("ошибка");
                }
                finally
                {
                    phonesList.Items.Refresh();
                }
                */
                #endregion end
                object model=view.SelectedItem;
                if(model != null)
                {
                    viewModel.OpenItemAsync(model);
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowComputers();//временно
        }

        private void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ListView view)
            {
                if (view.SelectedItem == null)
                {
                    viewModel.SelectedItem = null;
                    return;
                }
                viewModel.SelectedItem = (Model) view.SelectedItem;
            }
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel.Dispose();

            string[] files=Directory.GetFiles(@"D:\FILES");
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
        private void listview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListView view)
            {
                if(viewModel.SelectedItem?.Name==((Model)view.SelectedItem)?.Name)
                {
                    view.SelectedItem = null;
                    viewModel.SelectedItem = null;
                }
            }
        }
    }
}
