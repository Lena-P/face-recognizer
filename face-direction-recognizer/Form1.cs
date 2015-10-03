using System.Drawing;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using System;

namespace face_direction_recognizer
{
    public partial class Form1 : Form
    {
        private VideoCaptureDevice videoSource;
        private IFilter[] filters = new IFilter[3];
        public Form1()
        {
            InitializeComponent();
            InitializeFilters();
            InitializeCapture();
        }

        private void InitializeFilters()
        {
            filters[0] = new GaussianFilter(1, 3);
            filters[1] = new SobelDetektor();
            filters[2] = new Binarizator();
        }

        private void InitializeCapture()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(NewFrameHandler);
            videoSource.Start();
        }
        // найти лицо и зрачки
        // найти лицо и натянуть текстуру или найти особые точки и посчитать их отношение
        private void NewFrameHandler(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            FastBitmap fastBitmap = new FastBitmap(bitmap);
            for (int i = 0; i < filters.Length; i++)
            {
                filters[i].DoFilter(fastBitmap);
            }
            pictureBox1.Image = fastBitmap.GrayBitmap;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            videoSource.Stop();
            videoSource.WaitForStop();
        }
    }
}
