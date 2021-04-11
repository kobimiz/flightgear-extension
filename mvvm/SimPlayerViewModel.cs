using System;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Threading;

namespace flightgearExtension.mvvm
{
    class SimPlayerViewModel : INotifyPropertyChanged
    {
        private volatile bool shouldStop;
        private Thread t;
        private ManualResetEvent threadSuspender = new ManualResetEvent(true);

        public event PropertyChangedEventHandler PropertyChanged;

        public int VM_frameIndex {
            get => model.frameIndex;
            set => model.frameIndex = value;
        }
        public double VM_FPS {
            get => model.FPS;
            set => model.FPS = value;
        }
        public string[] VM_Data
        {
            get => model.Data;
            set => model.Data = value;
        }

        private ISimPlayerModel model;

        public SimPlayerViewModel(ISimPlayerModel model)
        {
            shouldStop = true;
            this.model = model;
            threadSuspender = new ManualResetEvent(false);

            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };


            t = new Thread(delegate () {
                while (true)
                {
                    if (shouldStop)
                        threadSuspender.WaitOne();
                    skip(1);
                    Thread.Sleep((int)(1000 / model.FPS));
                }
            });
            t.Start();
        }
        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public bool loadCSV(string path)
        {
            try
            {
                model.Data = System.IO.File.ReadAllLines(path);
                return true;
            }
            catch (Exception)
            {
                model.Data = null;
                return false;
            }
        }

        public void pause()
        {
            threadSuspender.Reset();
            shouldStop = true;
        }

        public void play()
        {
            if (shouldStop)
            {
                shouldStop = false;
                threadSuspender.Set();
            }
        }

        public void skip(int frames)
        {
            model.frameIndex += frames;
        }
    }
}
