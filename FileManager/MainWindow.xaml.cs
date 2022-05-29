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
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;

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
        public MainWindow()
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken token= tokenSource.Token;
            server = new ServerFileManager(System.Net.IPAddress.Parse(ip), port);
            serverThread = new Thread(new ParameterizedThreadStart(server.Listen));
            serverThread.Start(token);

            Items = new List<Model>();
            LastElements = new Stack<string>();

            InitializeComponent();

            DataContext = this;
        }

        private void showComputers()
        {
            computers = server.GetComputers();
            string[] ID=computers.Keys.ToArray();
            string[] name=computers.Values.ToArray();
            Items.Clear();
            for(int i=0;i<ID.Length;i++)
            {
                CompModel comp = new CompModel(name[i], "comp", ID[i]);
                Items.Add(comp);
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
                        string response=String.Empty;

                        if(Items[index] is CompModel comp)
                        {
                            client=server.GetComputer(comp.ID);
                            await client.SendCommandAsync(Commands.Open, "", "drives");
 
                            response = await client.GetResponseAsync();
                        }

                        if(Items[index] is DriveModel drive)
                        {
                            await client.SendCommandAsync(Commands.Open, drive.path, drive.Type);
                            response = await client.GetResponseAsync();
                        }

                        if(Items[index] is DirectoryModel directory)
                        {
                            await client.SendCommandAsync(Commands.Open, directory.path, directory.Type);
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
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            showComputers();//временно
            /*if (LastElements.Count>0)
            {
                ShowDirs.openFileOrDir(LastElements.Pop(), Items);
                phonesList.Items.Refresh();
                if (LastElements.Count > 0) textBox.Text = Directory.GetParent(Items[0].Element.ToString()).ToString();
            }*/
        }

        private void phonesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if(LastElements.Count>0 && Items.Count>0) textBox.Text = Directory.GetParent(Items[0].Element.ToString()).ToString();
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
            server.Dispose();

            string[] files=Directory.GetFiles(@"D:\FILES");
            foreach (string file in files)
            {
                File.Delete(file);
            }
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

        private ImageSource createIcon()
        {
            ImageSource imageSource = null;
            using (FileStream fs = new FileStream(@$"icon\compIcon.png", FileMode.Open))
            {
                var png = new MemoryStream();

                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                image.Save(png, System.Drawing.Imaging.ImageFormat.Png);
                imageSource = BitmapFrame.Create(png);
            }
            
            return imageSource;
        }
    }
}
