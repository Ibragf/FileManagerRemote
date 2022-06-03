using FileManager.Commands;
using FileManager.Models;
using FileManager.NetworkTCP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileManager.ViewModel
{
    internal class FileViewModel : IDisposable
    {
        public ObservableCollection<Model> Items { get; private set; }
        public DownloadCommand downloadCommand { get; private set; }
        public Model? SelectedItem { get; set; }
        private bool _isTaskCompleted=true;

        private Dictionary<string, string> computers;
        private SerializableClass serializableClass;
        private ClientFileManager client;
        private ServerFileManager server;


        public FileViewModel()
        {
            Items = new ObservableCollection<Model>();
            downloadCommand = new DownloadCommand(DownloadFileAsync,CanExecuteDownload);

            server = new ServerFileManager(System.Net.IPAddress.Parse("192.168.0.16"), 8005);
            Thread serverThread=new Thread(server.Listen);
            serverThread.Start();
            
            ShowComputers();
        }

        public async void DownloadFileAsync(object model)
        {
            try
            {
                if(SelectedItem is FileModel|| SelectedItem is DirectoryModel)
                {
                    _isTaskCompleted = false;
                    //DeserializeResponse(SendCommand(Command.Download, model).Result);
                    string result = await SendCommandAsync(Command.Download, SelectedItem);
                    byte[] data = client.DownloadFile();
                    Task task = Task.Factory.StartNew(() =>
                    {
                        int var = SelectedItem.Name.LastIndexOf('.');
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < var; i++)
                        {
                            sb.Append(SelectedItem.Name[i]);
                        }

                        using (FileStream fs = new FileStream(@$"C:\Users\{Environment.UserName}\Downloads\{sb.ToString()}.zip", FileMode.Create))
                        {
                            fs.Write(data, 0, data.Length);
                        }
                    });
                    task.Wait();

                    _isTaskCompleted = task.IsCompleted;

                }
            }
            catch (SocketException ex)
            {
                client.Dispose();
                MessageBox.Show((int)ex.SocketErrorCode + ":" + ex.SocketErrorCode.ToString());
            }
            catch (Exception ex)
            {
                client.Dispose();
                MessageBox.Show(ex.Message);
            }
        }

        public async void OpenItemAsync(object model)
        {
            try
            {
                if(model is not FileModel)
                {
                    DeserializeResponse(await SendCommandAsync(Command.Open, model));
                }
            }
            catch (SocketException ex)
            {
                client.Dispose();
                MessageBox.Show((int)ex.SocketErrorCode + ":" + ex.SocketErrorCode.ToString());
            }
            catch (Exception ex)
            {
                client.Dispose();
                MessageBox.Show(ex.Message);
            }
        }

        private bool CanExecuteDownload(object model)
        {
            if(SelectedItem is null || SelectedItem is CompModel || SelectedItem is DriveModel)
            {
                return false;
            }
            return true;
        }

        private async Task<string> SendCommandAsync(Command command, object model)
        {
            string response;
            if(model is CompModel comp)
            {
                client = server.GetComputer(comp.ID);
                await client.SendCommandAsync(Command.Open, "", "comp");

                response =await client.GetResponseAsync();
                return response;
            }

            Model _model = (Model) model;
            await client.SendCommandAsync(command, _model.Path, _model.Type);
            response = await client.GetResponseAsync();

            return response;
        }

        private void DeserializeResponse(string response)
        {
            if (!string.IsNullOrEmpty(response))
            {
                Thread.Sleep(10);
                serializableClass = JsonConvert.DeserializeObject<SerializableClass>(response);
                Items.Clear();
                if (serializableClass.Drivers != null)
                {
                    foreach (var item in serializableClass.Drivers)
                    {
                        Items.Add(item);
                    }
                }
                if (serializableClass.Folders != null)
                {
                    foreach (var item in serializableClass.Folders)
                    {
                        Items.Add(item);
                    }
                }
                if (serializableClass.Files != null)
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

        public void ShowComputers()
        {
            computers = server.GetComputers();
            string[] ID = computers.Keys.ToArray();
            string[] name = computers.Values.ToArray();
            Items.Clear();
            for (int i = 0; i < ID.Length; i++)
            {
                CompModel comp = new CompModel(name[i], "comp", ID[i]);
                Items.Add(comp);
            }
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}
