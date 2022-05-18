using FileManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileManager
{
    internal static class ShowDirs
    {
        /*public static void openDrives(List<ItemModel> components)
        {
            components.Clear();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                //ImageSource image = CreateIcon(drive.Name);
                ItemModel info = new ItemModel(drive.Name, drive.DriveType.ToString(), drive);
                components.Add(info);
            }
        }

        public static void openFileOrDir(int index, List<ItemModel> components, Stack<string> LastElements)
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
                    ImageSource image = createIconForFolders(dir);
                    ItemModel info = new ItemModel(directoryInfo.Name, directoryInfo.LastWriteTime.ToShortDateString(), directoryInfo, image);
                    components.Add(info);
                }
                foreach (string file in Directory.GetFiles(path))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    ImageSource image = createIconForFiles(fileInfo.FullName);
                    if (!fileInfo.Exists) continue;
                    ItemModel info = new ItemModel(fileInfo.Name, fileInfo.LastWriteTime.ToShortDateString(),fileInfo.Extension, fileInfo, image);
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
                    ImageSource image = createIconForFolders(dir);
                    ItemModel info = new ItemModel(directoryInfo.Name, directoryInfo.LastWriteTime.ToShortDateString(), directoryInfo, image);
                    components.Add(info);
                }
                foreach (string file in Directory.GetFiles(path))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    ImageSource image = createIconForFiles(fileInfo.FullName);
                    if (!fileInfo.Exists) continue;
                    ItemModel info = new ItemModel(fileInfo.Name, fileInfo.LastWriteTime.ToShortDateString(), fileInfo.Extension , fileInfo, image);
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

        public static void openFileOrDir(string lastElement, List<ItemModel> components)
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
                ImageSource image = createIconForFolders(dir);
                ItemModel info = new ItemModel(directoryInfo.Name, directoryInfo.LastWriteTime.ToShortDateString(), directoryInfo,image);
                components.Add(info);
            }
            foreach (string file in Directory.GetFiles(path))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (!fileInfo.Exists) continue;
                ImageSource image=createIconForFiles(fileInfo.FullName);
                ItemModel info = new ItemModel(fileInfo.Name, fileInfo.LastWriteTime.ToShortDateString(), fileInfo.Extension,fileInfo, image);
                components.Add(info);
            }
        }

        private static ImageSource createIconForFiles(string path)
        {
            ImageSource image = null;
            Icon icon = Icon.ExtractAssociatedIcon(path);
            MemoryStream stream=null;
            if (icon != null)
            {
                using (var bmp = icon.ToBitmap())
                {
                    stream = new MemoryStream();
                    
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    image = BitmapFrame.Create(stream);
                      
                }
            }
            return image;
        }

        private static ImageSource createIconForFolders(string path)
        {
            ImageSource imageSource = null;
            using (FileStream fs = new FileStream(@"icon\folderIcon.png", FileMode.Open))
            {
                var png = new MemoryStream();
                
                Image image = Image.FromStream(fs);
                image.Save(png, System.Drawing.Imaging.ImageFormat.Png);
                imageSource = BitmapFrame.Create(png);
            }
            return imageSource;
        }*/
    }
}
