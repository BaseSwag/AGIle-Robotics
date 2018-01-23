using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace ImageDownload
{
    class Program
    {
        static Timer timer = new Timer();
        static void Main(string[] args)
        {
            WebClient wc = new WebClient();
            timer.Interval = 1000 / 8;
            timer.Tick += (o, e) => {
                timer.Stop();
                wc.DownloadFileAsync(new Uri("http://10.0.0.1:8080/?action=snapshot"), "images\\image_" + DateTime.Now.Ticks + ".jpg");
            };

            wc.DownloadFileCompleted += (o, e) => {
                Console.Clear();
                int pics = Directory.GetFiles("images\\").Length;
                if ( pics < 15001)
                    timer.Start();
                Console.WriteLine("Files downloaded: " + pics);
            };

            timer.Start();

            while (true)
                Application.DoEvents();
           
        }
    }
}
