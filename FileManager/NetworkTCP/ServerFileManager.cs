using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace FileManager.NetworkTCP
{
    internal class ServerFileManager : IDisposable
    {
        private IPAddress ipAddress;
        private int port;
        private static TcpListener tcpListener;
        private LinkedList<ClientFileManager> clients;

        public ServerFileManager(IPAddress ip, int port)
        {
            this.port = port;
            ipAddress=ip;
            clients = new LinkedList<ClientFileManager>();
        }

        public void Listen()
        {
            try
            {
                tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();
                while(true)
                {
                    TcpClient TcpClient = tcpListener.AcceptTcpClient();
                    ClientFileManager client = new ClientFileManager(TcpClient, this);
                }
            }
            catch (SocketException ex)
            {
                if(ex.SocketErrorCode!=SocketError.Interrupted)
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                Dispose();
            }
        }

        public void addConnection(ClientFileManager client)
        {
            clients.AddLast(client);
        }

        public Dictionary<string, string> GetComputers()
        {
            Dictionary<string, string> computers = new Dictionary<string, string>();
            foreach (ClientFileManager client in clients)
            {
                computers.Add(client.ID, client.compName);
            }
            return computers;
        }

        public ClientFileManager GetComputer(string id)
        {
            return clients.FirstOrDefault(x => x.ID == id);
        }

        public void RemoveConnection(ClientFileManager client)
        {
            clients.Remove(clients.Where(x=>x.ID==client.ID).FirstOrDefault());
        }

        public void Dispose()
        {
            tcpListener.Stop();
            foreach (ClientFileManager client in clients)
            {
                client.Close();
            }
        }
    }
}
