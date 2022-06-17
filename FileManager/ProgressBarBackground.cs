using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using FileManager.ViewModel;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.IO;
using FileManager.Models;
using System.Runtime.CompilerServices;
using FileManager.NetworkTCP;
using System.Diagnostics;

namespace FileManager
{
    internal class ProgressBarBackground : INotifyPropertyChanged
    {
        private double progressValue = 0;
        private Visibility progressVisibility=Visibility.Hidden;
        private BackgroundWorker backgroundWorker;
        public ClientFileManager client;
        public Model selectedItem { get; set; }
        public Action<object, DoWorkEventArgs> worker_DoWork;

        public Visibility ProgressVisibility
        {
            get { return progressVisibility; }
            set
            {
                progressVisibility = value;
                OnPropertyChanged("ProgressVisibility");
            }
        }
        public double ProgressValue
        {
            get { return progressValue; }
            set
            {
                progressValue = value;
                OnPropertyChanged("ProgressValue");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void Start()
        {
            ProgressValue = 0;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork+=new DoWorkEventHandler(worker_DoWork);
            backgroundWorker.ProgressChanged += ProgressChanged;
            backgroundWorker.RunWorkerCompleted += WorkCompleted;
            backgroundWorker.RunWorkerAsync();
            
        }

        private void doWork(object sender, DoWorkEventArgs e)
        {
            for (int i=0;i<100;i++)
            {
                (sender as BackgroundWorker).ReportProgress(i + 1);
                Thread.Sleep(10);
            }
            e.Result = 500;
        }
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressVisibility=Visibility.Visible;
            ProgressValue+=(double)e.UserState;
        }

        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //MessageBox.Show("completed");
            byte[] data = (byte[]) e.Result;

            string filename;
            if (selectedItem is FileModel)
            {
                int index = selectedItem.Name.LastIndexOf('.');
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < index; i++)
                {
                    sb.Append(selectedItem.Name[i]);
                }
                filename = sb.ToString();
            }
            else filename = selectedItem.Name;

            Task task = Task.Factory.StartNew(() =>
            {
                using (FileStream fs = new FileStream(@$"C:\Users\{Environment.UserName}\Downloads\{filename}.zip", FileMode.OpenOrCreate))
                {
                    fs.Write(data, 0, data.Length);
                }
            });
            task.Wait();

            Thread.Sleep(1000);
            ProgressVisibility = Visibility.Hidden;
        }
    }
}
