using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using ClientApp.Models;
using Newtonsoft.Json;
using System.IO.Compression;

namespace ClientApp
{
    internal class TcpConnection : IDisposable
    {
        private string ip = "192.168.0.16";
        private int port = 8005;
        private NetworkStream stream;
        public TcpClient client { get; private set; }
        public bool Connected
        {
            get => client.Connected;
        }

        public TcpConnection()
        {
            client = new TcpClient();
        }

        public void Connect()
        {
            if (!client.Connected)
            {
                try
                {
                    client.Connect(ip, port);
                    stream = client.GetStream();
                    string compName = System.Net.Dns.GetHostName();
                    byte[] data = Encoding.UTF8.GetBytes(compName);
                    stream.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void GetCommandForResponse()
        {
            while (client.Connected)
            {
                string command = String.Empty;
                StringBuilder sb = new StringBuilder();
                byte[] data = new byte[64];
                try
                {
                    do
                    {
                        int bytes = stream.Read(data, 0, data.Length);
                        sb.Append(Encoding.UTF8.GetString(data));
                    } while (stream.DataAvailable);
                    command = sb.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                SerializableClass serializableClass = new SerializableClass();
                command=command.Replace("\0", String.Empty);
                string[] headers = command.Split(new char[] { ';' }, 3, StringSplitOptions.None);
                string serializedResponse = String.Empty;

                if (headers[0] == "Open" && headers[2] == "comp")
                {
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    List<DriveModel> listDrives = new List<DriveModel>();
                    foreach (DriveInfo drive in drives)
                    {
                        DriveModel model = new DriveModel(drive.Name,drive.Name, "drive");
                        listDrives.Add(model);
                    }
                    serializableClass.Drivers = listDrives;
                }

                if(headers[0]=="Open" && headers[2] =="drive")
                {
                    string[] dirs = Directory.GetDirectories(headers[1]);
                    string[] files=Directory.GetFiles(headers[1]);
                    List<DirectoryModel> listDirs=new List<DirectoryModel>();
                    List<FileModel> listFiles=new List<FileModel>();
                    foreach(string dir in dirs)
                    {
                        DirectoryInfo directory=new DirectoryInfo(dir);
                        DirectoryModel dirModel = new DirectoryModel(directory.Name, dir, "folder", directory.LastWriteTime.ToShortDateString());
                        listDirs.Add(dirModel);
                    }
                    foreach(string file in files)
                    {
                        FileInfo fileinfo = new FileInfo(file);
                        FileModel fileModel = new FileModel(fileinfo.Name, file, "file", fileinfo.LastWriteTime.ToShortDateString(), fileinfo.Extension);
                        listFiles.Add(fileModel);
                    }
                    serializableClass.Folders = listDirs;
                    serializableClass.Files = listFiles;
                }

                if(headers[0]=="Open" && headers[2]=="folder")
                {
                    string[] dirs = Directory.GetDirectories(headers[1]);
                    string[] files = Directory.GetFiles(headers[1]);
                    List<DirectoryModel> listDirs = new List<DirectoryModel>();
                    List<FileModel> listFiles = new List<FileModel>();
                    foreach (string dir in dirs)
                    {
                        DirectoryInfo directory = new DirectoryInfo(dir);
                        DirectoryModel dirModel = new DirectoryModel(directory.Name, dir, "folder", directory.LastWriteTime.ToShortDateString());
                        listDirs.Add(dirModel);
                    }
                    foreach (string file in files)
                    {
                        FileInfo fileinfo = new FileInfo(file);
                        FileModel fileModel = new FileModel(fileinfo.Name, file, "file", fileinfo.LastWriteTime.ToShortDateString(), fileinfo.Extension);
                        listFiles.Add(fileModel);
                    }
                    serializableClass.Folders = listDirs;
                    serializableClass.Files = listFiles;
                }

                if(headers[0]=="Download")
                {
                    SendFiles(headers[2], headers[1]);
                    continue;
                }


                serializedResponse = JsonConvert.SerializeObject(serializableClass);
                try
                {
                    byte[] dataResponse = Encoding.UTF8.GetBytes(serializedResponse);
                    stream.Write(dataResponse, 0, dataResponse.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void SendFiles(string type, string path)
        {
            string zipFile = @$"C:\Users\{Environment.UserName}\Downloads\zip.zip";
            string dirPath=String.Empty;

            if(type=="file")
            {
                FileInfo fileinfo=new FileInfo(path);
                dirPath=fileinfo.Directory.FullName+@"\dir_for_file";
                Directory.CreateDirectory(dirPath);
                File.Copy(fileinfo.FullName, dirPath+$@"\{fileinfo.Name}");

                path=dirPath;
            }

            if (File.Exists(zipFile)) File.Delete(zipFile);
            ZipFile.CreateFromDirectory(path, zipFile);
            
            if (Directory.Exists(dirPath)) Directory.Delete(dirPath, true);

            byte[] data;
            using (FileStream fs = new FileStream(zipFile, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
            }
            string fileLength = data.Length.ToString();
            byte[] length = Encoding.UTF8.GetBytes(fileLength);
            stream.Write(length, 0, length.Length);

            StringBuilder sb = new StringBuilder();
            do
            {
                int bytes = stream.Read(length, 0, length.Length);
                sb.Append(Encoding.UTF8.GetString(length, 0, bytes));
            } while (stream.DataAvailable);

            int responseLength = Int32.Parse(sb.ToString());
            if (responseLength == data.Length) Console.WriteLine("Correct");
            stream.Write(data, 0, data.Length);
        }

        public void Dispose()
        {
            if (stream != null) stream.Close();
            if (client != null) client.Dispose();
        }
        
    }
}
