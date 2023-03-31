using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace ALX2Homework1WF
{
    public partial class Form1 : Form
    {
        public string serverAddress = "http://51.91.120.89/TABLICE/";
        long bytes = 0;
        long totalBytes = 0;
        string[] result;
        public Form1()
        {
            InitializeComponent();
            string dir = @"C:\HM";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        private async Task GetImagesList()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(serverAddress);

                if (response.IsSuccessStatusCode)
                {
                    var imagesList = await response.Content.ReadAsStringAsync();
                    var clearList = Regex.Replace(imagesList, @"\r\n", ",");
                    clearList = clearList.Remove(clearList.Length - 1);

                    var charSeparator = ',';
                    result = clearList.Split(charSeparator);

                    foreach (var image in result)
                    {
                        textBox1.Text += $"{image} {Environment.NewLine}";
                    }
                }
            }
        }
        private void DownloadImage(string serverAddress, string imageName)
        {
            using (var clientImage = new HttpClient())
            {
                using (var s = clientImage.GetStreamAsync($"{serverAddress}{imageName}"))
                {
                    using (var fs = new FileStream($"C:\\HM\\{imageName}", FileMode.Create))
                    {
                        s.Result.CopyTo(fs);
                        bytes = fs.Length;
                    }
                }
            }
        }
        private void DownloadImagesSync()
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            textBox1.Text = string.Empty;
            label2.Text = $"Total bytes downloaded:";
            label1.Text = $"Time elapsed:";
            totalBytes = 0;

            foreach (var image in result)
            {
                textBox1.Text += $"Downloading - {image}{Environment.NewLine}";
                DownloadImage(serverAddress, image);

                textBox1.Text += $"File size: {bytes} bytes {Environment.NewLine}";
                totalBytes += bytes;
            }
            label2.Text = $"Total bytes downloaded: {totalBytes} bytes {Environment.NewLine}";
            sw.Stop();
            label1.Text = $"Time elapsed: {sw.ElapsedMilliseconds} milliseconds";

        }

        private async Task<string> DownloadImagesAsync()
        {

            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            label2.Text = $"Total bytes downloaded:";
            label1.Text = $"Time elapsed:";
            textBox1.Text = string.Empty;
            totalBytes = 0;

            foreach (var image in result)
            {
                textBox1.Text += $"Downloading - {image}{Environment.NewLine}";
                await Task.Run(() => DownloadImage(serverAddress, image));

                textBox1.Text += $"File size: {bytes} bytes {Environment.NewLine}";
                totalBytes += bytes;
            }
            label2.Text = $"Total bytes downloaded: {totalBytes} bytes {Environment.NewLine}";
            sw.Stop();
            label1.Text = $"Time elapsed: {sw.ElapsedMilliseconds} milliseconds";

            return "Yep";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            label2.Text = $"Total bytes downloaded:";
            label1.Text = $"Time elapsed:";
            GetImagesList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DownloadImagesSync();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DownloadImagesAsync();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "C:\\HM"); 
        }
    }
}