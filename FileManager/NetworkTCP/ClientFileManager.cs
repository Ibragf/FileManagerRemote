using FileManager.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileManager.NetworkTCP
{
    internal class ClientFileManager : IDisposable
    {
        public readonly string ID;
        private TcpClient tcpClient;
        private ServerFileManager server;
        private NetworkStream stream;

        public string compName { get; private set; } = string.Empty;
        public bool IsConnected {
            get
            {
                return tcpClient.Connected;
            }
        }

        public ClientFileManager(TcpClient tcpClient, ServerFileManager server)
        {
            ID=Guid.NewGuid().ToString();
            this.tcpClient = tcpClient;
            this.server = server;
            server.addConnection(this);
            stream=Initialize();
        }
        private NetworkStream Initialize()
        {
            NetworkStream networkStream = null;
            try
            {
                networkStream = tcpClient.GetStream();
                compName=getStringFromStream(networkStream);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return networkStream;
        }

        public void DownloadFile(object sender, DoWorkEventArgs e)
        {
            byte[] data;
            lock(stream)
            {
                byte[] buffer = new byte[512];

                StringBuilder sb = new StringBuilder();
                do
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
                } while (stream.DataAvailable);

                string fileLength = sb.ToString();
                double lenght = Double.Parse(fileLength);
                Encoding.UTF8.GetBytes(fileLength).CopyTo(buffer, 0);
                stream.Write(buffer, 0, buffer.Length);

                data = new byte[(int)lenght];
                int index = 0;
                do
                {
                    double bytes = stream.Read(buffer, 0, buffer.Length);
                    double percentProgress = (bytes / lenght)*100;
                    (sender as BackgroundWorker).ReportProgress((int)percentProgress, percentProgress);
                    if (bytes<buffer.Length)
                    {
                        byte[] newBuffer=new byte[(int)bytes];
                        for(int i=0; i<newBuffer.Length; i++)
                        {
                            newBuffer[i]=buffer[i];
                        }
                        newBuffer.CopyTo(data, index);
                        break;
                    }
                    buffer.CopyTo(data, index);
                    index +=(int)bytes;
                } while (stream.DataAvailable);
            }
            e.Result= data;
        }

        private string getStringFromStream(NetworkStream networkStream)
        {
            StringBuilder sb = new StringBuilder();
            byte[] data=new byte[256];
            int bytes = 0;
            do
            {
                bytes=networkStream.Read(data, 0, data.Length);
                sb.Append(Encoding.UTF8.GetString(data, 0, bytes));
            } while (networkStream.DataAvailable);

            return sb.ToString();
        }

        public async Task<int> SendCommandAsync(Command command, string path, string type)
        {
            var task =await Task.Run(() => SendCommand(command, path, type));
            if (task!=-1111) throw new SocketException(task);
            return task;
        }
        public async Task<string> GetResponseAsync()
        {
            var response = await Task.Run(() => GetResponse());
            return response;
        }
        
        private int SendCommand(Command command, string path, string type)
        {
            int errorCode = -1111;
            try
            {
                string commandStr = command.ToString() + ";" + path + ";" + type;
                byte[] data = Encoding.UTF8.GetBytes(commandStr);
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                if(ex.InnerException is SocketException exSocket)
                {
                    errorCode = (int)exSocket.SocketErrorCode;
                }
            }
            return errorCode;
        }

        public string GetResponse()
        {
            string response = String.Empty;
            try
            {
                response = getStringFromStream(stream);
            }
            catch(Exception ex)
            {

            }
            return response;
        }

        public void Close()
        {
            if(tcpClient != null) tcpClient.Close();
            if(stream != null) stream.Close();
        }

        public void Dispose()
        {
            server.RemoveConnection(this);
            Close();
        }

    }
}
