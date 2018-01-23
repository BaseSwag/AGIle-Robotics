using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;

namespace RobotControl
{
    public partial class Form1 : Form
    {
        Stream s;
        TcpClient tcpClient;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect("192.168.1.187", 8342);
            s = tcpClient.GetStream();
            while(true)
            {
                double x = -.13;
                double y = 1;
                double c = 1;

                WriteStream(s, x, y, c);
            }
            
        }

        double Map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        void WriteStream(Stream s, double x, double y, double c = 0)
        {
            x = Map(x, -1, 1, 0, 255);
            y = Map(y, -1, 1, 0, 255);
            c = Map(c, 0, 1, 0, 255);

            s.Write(new byte[] { (byte)x }, 0, 1);
            s.Write(new byte[] { (byte)y }, 0, 1);
            s.Write(new byte[] { (byte)c }, 0, 1);
            s.Write(new byte[] { 0 }, 0, 1);
            s.Flush();
        }

        void Forward()
        {
            s.Write(new byte[] { 0 }, 0, 1);
            s.Flush();
        }

        void Right()
        {
            s.Write(new byte[] { 1 }, 0, 1);
            s.Flush();
        }

        void Left()
        {
            s.Write(new byte[] { 2 }, 0, 1);
            s.Flush();
        }

        void Backward()
        {
            s.Write(new byte[] { 3 }, 0, 1);
            s.Flush();
        }

        void Stop()
        {
            s.Write(new byte[] { 4 }, 0, 1);
            s.Flush();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
           /* switch(e.KeyCode)
            {
                case Keys.Up:
                    Forward();
                    break;
                case Keys.Down:
                    Backward();
                    break;
                case Keys.Left:
                    Left();
                    break;
                case Keys.Right:
                    Right();
                    break;
            }*/
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //pictureBox1.Image = LoadPicture("http://192.168.1.187:8080/?action=snapshot");
        }

        private Bitmap LoadPicture(string url)
        {
            HttpWebRequest wreq;
            HttpWebResponse wresp;
            Stream mystream;
            Bitmap bmp;

            bmp = null;
            mystream = null;
            wresp = null;
            try
            {
                wreq = (HttpWebRequest)WebRequest.Create(url);
                wreq.AllowWriteStreamBuffering = true;

                wresp = (HttpWebResponse)wreq.GetResponse();

                if ((mystream = wresp.GetResponseStream()) != null)
                    bmp = new Bitmap(mystream);
            }
            finally
            {
                if (mystream != null)
                    mystream.Close();

                if (wresp != null)
                    wresp.Close();
            }
            return (bmp);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            s.Write(new byte[] { 0 }, 0, 1);
            s.Write(new byte[] { 0 }, 0, 1);
            s.Write(new byte[] { 0 }, 0, 1);
            s.Write(new byte[] { 5 }, 0, 1);
            s.Flush();
        }
    }
}
