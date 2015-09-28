piaudgfhf;iasjdfao;izuf`di cjqpw﻿using System.Drawing;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;

namespace face_direction_recognizer
{
    public partial class Form1 : Form
    {
        private VideoCaptureDevice videoSource;

        public Form1()
        {
            InitializeComponent();
            InitializeCapture();
        }

        private void InitializeCapture()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(NewFrameHandler);
            videoSource.Start();
        }

        private void NewFrameHandler(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            FastBitmap fastBitmap = new FastBitmap(bitmap);
            pictureBox1.Image = fastBitmap.GrayBitmap;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            videoSource.Stop();
            videoSource.WaitForStop();
        }
    }
}
