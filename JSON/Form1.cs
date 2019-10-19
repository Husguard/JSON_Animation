using System.Collections.Generic;
using System.Windows.Forms;
using ServiceStack.Text;
using System.IO;
using System.Drawing;
using System;
using System.Threading.Tasks;
using System.Linq;
using FastBitmapLib;

namespace JSON
{
    public partial class Form1 : Form
    {
        private Animation parsedData;
        private int Frame = 0;
        private int T1 = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        private int T2;
        private int T11;
        public Form1()
        {
            using (StreamReader re = new StreamReader(new BufferedStream(File.OpenRead("C:/Users/Админ/Desktop/json/file3_220_320.json"), 1024*1024)))
            {
                parsedData = JsonSerializer.DeserializeFromReader<Animation>(re); // свой deserializer или unsafe code
            }
            T11 = (((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) - T1);
            MessageBox.Show($"Downloaded for {T11} seconds");
            InitializeComponent();
            T1 = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
        }
        private void Painting()
        {
            using (var fastBitmap = ((Bitmap)pictureBox4.Image).FastLock())
            {
                for (int j = 0; j < parsedData.Height / 2; j++)
                {
                    for (int i = 0; i < parsedData.Width / 2; i++)
                    {
                        fastBitmap.SetPixel(i, j, Color.FromArgb(parsedData.Data[Frame][j * parsedData.Width + i][0],
                        parsedData.Data[Frame][j * parsedData.Width + i][1], parsedData.Data[Frame][j * parsedData.Width + i][2]));
                    }
                }
            }
        }
        private void PaintingT()
        {
            using (var fastBitmap = ((Bitmap)pictureBox3.Image).FastLock())
            {
                for (int j = parsedData.Height / 2; j < parsedData.Height; j++)
                {
                    for (int i = 0; i < parsedData.Width / 2; i++)
                    {
                        fastBitmap.SetPixel(i, j - parsedData.Height / 2, Color.FromArgb(parsedData.Data[Frame][j * parsedData.Width + i][0],
                        parsedData.Data[Frame][j * parsedData.Width + i][1], parsedData.Data[Frame][j * parsedData.Width + i][2]));

                    }
                }
            }
        }
        private void Painting2()
        {
            using (var fastBitmap = ((Bitmap)pictureBox2.Image).FastLock())
            {
                for (int j = 0; j < parsedData.Height / 2; j++)
                {
                    for (int i = parsedData.Width / 2; i < parsedData.Width; i++)
                    {
                        fastBitmap.SetPixel(i - parsedData.Width / 2, j, Color.FromArgb(parsedData.Data[Frame][j * parsedData.Width + i][0],
                        parsedData.Data[Frame][j * parsedData.Width + i][1], parsedData.Data[Frame][j * parsedData.Width + i][2]));
                    }
                }
            }
        }
        private void Painting3()
        {
            using (var fastBitmap = ((Bitmap)pictureBox1.Image).FastLock())
            {
                for (int j = parsedData.Height / 2; j < parsedData.Height; j++)
            {
                    for (int i = parsedData.Width / 2; i < parsedData.Width; i++)
                    {
                        fastBitmap.SetPixel(i - parsedData.Width / 2, j - parsedData.Height / 2, Color.FromArgb(parsedData.Data[Frame][j * parsedData.Width + i][0],
                        parsedData.Data[Frame][j * parsedData.Width + i][1], parsedData.Data[Frame][j * parsedData.Width + i][2]));
                    }
                }
            }
        }
        private void Timer1_Tick(object sender, System.EventArgs e)
        {
            Drawing();
            Frame++;
            if ((Frame == parsedData.Frames))
            {
                Frame = 0;
                T2 = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - T1;
                double S = T11 + T2 - (0.04 * (parsedData.Frames - 1));
          //      timer1.Stop();
           //     MessageBox.Show($"{parsedData.Frames} Frames for {T2} seconds, Speed is {S}");
            }
        }
        private void Form1_Load(object sender, System.EventArgs e)
        {
            this.DoubleBuffered = true;
            timer1.Enabled = true;
            timer1.Interval = 15; 
            PictureBoxInit();
            Drawing();
        }
        private void PictureBoxInit()
        {
            this.Height = parsedData.Height;
            this.Width = parsedData.Width;
            var PictureBoxes = Controls.OfType<PictureBox>().ToList();
            int PictureBoxCount = Controls.OfType<PictureBox>().Count() / 2;

            var x_new = parsedData.Width / (PictureBoxCount);
            var y_new = parsedData.Height / (PictureBoxCount);
            for (int i = 0; i < PictureBoxCount * 2; i++)
            {
                int x = 0;
                int y = 0;
                if (i == 1)
                {
                    y = y_new;
                }
                if (i == 2)
                {
                    x = x_new;
                }
                if (i == 3)
                {
                    x = x_new;
                    y = y_new;
                }
                PictureBoxes[i].Location = new Point(x, y);
                PictureBoxes[i].Width = x_new;
                PictureBoxes[i].Height = y_new;
                PictureBoxes[i].Image = new Bitmap(PictureBoxes[i].Width, PictureBoxes[i].Height);
            }
        }
        private void Drawing()
        {
            Task[] tasks1 = new Task[3]
            {
                new Task(PaintingT),
                new Task(Painting2),
                new Task(Painting3)
            };
            foreach (var t in tasks1) t.Start();
            Painting();
            Task.WaitAll(tasks1);
            tasks1[0].Dispose();
            tasks1[1].Dispose();
            tasks1[2].Dispose();
            Invalidate(true);
        }
    }
    public class Animation
    {
        public int Frames { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<List<List<byte>>> Data { get; set; }
    }
}
