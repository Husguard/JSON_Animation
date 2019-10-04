using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using System;
using System.Threading;

namespace JSON
{
    public partial class Form1 : Form
    {
        Thread second = new Thread(new ThreadStart(Count));
        Graphics gr;
        private Animation parsedData;
        private int Frame=0;
        private int T1 = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        private int T2;
        private int T11;
        public Form1()
        {
            second.Start();
            //  using (StreamReader re = new StreamReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"/file5_1160_786.json"))
            using (StreamReader re = new StreamReader("C:/Users/Админ/Desktop/json/file1_500_500.json"))
            {
                JsonTextReader reader = new JsonTextReader(re);
                JsonSerializer se = new JsonSerializer();
                parsedData = se.Deserialize<Animation>(reader);
            }
            T11 = (((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) - T1);
            MessageBox.Show($"Downloaded for {T11} seconds");
            T1 = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("Домен приложения: {0}", Thread.GetDomain().FriendlyName);
        }
        // расчет рисования квадратов
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Painting();
        }
            private void Painting()
        {
            gr.DrawImage(pictureBox1.Image, 0, 0);
            //i-кадр, j- № квадраты, k-цвета
            for (int i = 0; i < parsedData.Width; i++) // parsedData.Frames
            {
                for (int j = 0; j < parsedData.Height; j++) // parsedData.Data[0].Count
                {
                    ((Bitmap)pictureBox1.Image).SetPixel(i, j, Color.FromArgb(parsedData.Data[Frame][i*j][0], parsedData.Data[Frame][i*j][1], parsedData.Data[Frame][i*j][2]));
                }
            }
        }
        private void Timer1_Tick(object sender, System.EventArgs e)
        {
            Painting();
            Frame++;

            if((Frame %= parsedData.Frames) == 0)
            {
                T2 = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - T1;
                double S = T11 + T2 - ((timer1.Interval/1000) * (parsedData.Frames - 1));
                MessageBox.Show($"{parsedData.Frames} Frames for {T2} seconds, Speed is {S}");
            }
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Interval = 10; // 1000 = 1 секунда = 100мсек
            this.Height = parsedData.Height;
            this.Width = parsedData.Width;
            pictureBox1.Width = parsedData.Width;
            pictureBox1.Height = parsedData.Height;
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gr = pictureBox1.CreateGraphics();
        }
    }
    public class Animation
    {

        public int Frames { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("data")]
        public List<List<List<int>>> Data { get; set; }
    }
}
