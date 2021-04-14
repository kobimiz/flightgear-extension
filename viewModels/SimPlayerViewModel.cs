using System;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using OxyPlot;

namespace flightgearExtension.viewModels
{
    public class SimPlayerViewModel : ViewModel
    {
        private volatile bool shouldStop;
        private Thread t;
        private ManualResetEvent threadSuspender = new ManualResetEvent(true);
        private TcpClient client;
        private StreamWriter writer;

        public SimPlayerViewModel(Model model) : base(model)
        {
            shouldStop = true;
            client = null;
            writer = null;
            threadSuspender = new ManualResetEvent(false);

            t = new Thread(delegate () {
                while (true)
                {
                    if (shouldStop)
                        threadSuspender.WaitOne();
                    skip(1);
                    Thread.Sleep((int)(1000 / this.model.FPS));
                }
            });
            t.Start();
        }

        public bool loadCSV(string path)
        {
            try
            {
                model.Data = System.IO.File.ReadAllLines(path);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                model.Data = null;
                return false;
            }
        }

        public bool loadXML(string path)
        {
            try
            {
                model.headings = classes.Utility.getVariableNamesFromXML(path);
                return true;
            }
            catch (Exception)
            {
                model.headings = null;
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

        public void startSim()
        {
            if (client == null)
            {
                client = new TcpClient("localhost", 5400);
                writer = new StreamWriter(client.GetStream());
            }
            VM_frameIndex = 0;
            play();
        }
        public void closeConnections()
        {
            if (writer != null)
            {
                writer.Close();
                client.Close();
            }
        }
        public bool isConnected()
        {
            return writer != null;
        }

        // sends the ith frame to the simulator
        public void sendCurrFrame()
        {
            writer.WriteLine(VM_Data[VM_frameIndex]);
            writer.Flush();
        }
    }
}
