using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            timer.Interval = 500;
            timer.Tick += (o, e) => {
                timer.Stop();
                wc.DownloadFileAsync(new Uri("http://10.0.0.1:8080/?action=snapshot"), "images\image_" + DateTime.Now.Ticks + ".jpg");
            };

            wc.DownloadFileCompleted += (o, e) => {
                Console.WriteLine("File downloaded");
                timer.Start();
            };

            timer.Start();

            while (true)
                Application.DoEvents();
           
        }
    }
}
