using ClientApp;
using System;

class Program
{
    static void Main(string[] args)
    {
        using(TcpConnection tcpClient=new TcpConnection())
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler((object sender, EventArgs e) => tcpClient.Dispose());
            while(!tcpClient.Connected)
            {
                tcpClient.Connect();
            }
            tcpClient.GetCommandForResponse();
        }
    }
}
