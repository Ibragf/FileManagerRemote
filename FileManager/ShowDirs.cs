using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    internal static class ShowDirs
    {
        public static void openDrives(List<Info> components)
        {
            components.Clear();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                Info info = new Info(drive.Name, drive.DriveType.ToString(), drive);
                components.Add(info);
            }
        }

        public static void openFileOrDir(int index, List<Info> components, Stack<string> LastElements)
        {
            if (index == -1) return;
            if (components[index].Element.GetType() == typeof(DriveInfo))
            {
                LastElements.Push("drive");
                string path = components[index].Element.ToString();
                components.Clear();
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                    Info info = new Info(directoryInfo.Name, directoryInfo.LastWriteTime.ToShortDateString(), directoryInfo);
                    components.Add(info);
                }
                foreach (string file in Directory.GetFiles(path))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    Info info = new Info(fileInfo.Name, fileInfo.LastWriteTime.ToShortDateString(),fileInfo.Extension, fileInfo);
                    components.Add(info);
                }
                return;
            }
            if (components[index].Element.GetType() == typeof(DirectoryInfo))
            {
                LastElements.Push(Directory.GetParent(components[index].Element.ToString()).ToString());
                string path = components[index].Element.ToString();
                components.Clear();
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                    Info info = new Info(directoryInfo.Name, directoryInfo.LastWriteTime.ToShortDateString(), directoryInfo);
                    components.Add(info);
                }
                foreach (string file in Directory.GetFiles(path))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    Info info = new Info(fileInfo.Name, fileInfo.LastWriteTime.ToShortDateString(), fileInfo.Extension , fileInfo);
                    components.Add(info);
                }
                return;
            }
            if (components[index].Element.GetType() == typeof(FileInfo))
            {
                ProcessStartInfo process = new ProcessStartInfo();
                process.UseShellExecute = true;
                process.FileName = components[index].Element.ToString();
                Process.Start(process);
            }
        }

        public static void openFileOrDir(string lastElement, List<Info> components)
        {
            if (lastElement == "drive")
            {
                components.Clear();
                openDrives(components);
                return;
            }
            string path = lastElement;
            components.Clear();
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                Info info = new Info(directoryInfo.Name, directoryInfo.LastWriteTime.ToShortDateString(), directoryInfo);
                components.Add(info);
            }
            foreach (string file in Directory.GetFiles(path))
            {
                FileInfo fileInfo = new FileInfo(file);
                Info info = new Info(fileInfo.Name, fileInfo.LastWriteTime.ToShortDateString(), fileInfo);
                components.Add(info);
            }
        }
    }
}
