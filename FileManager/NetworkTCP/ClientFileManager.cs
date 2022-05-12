﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public void SendCommand(string command)
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            stream.Write(data, 0, data.Length);
        }

        public string GetResponse()
        {
            string response = getStringFromStream(stream);
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
