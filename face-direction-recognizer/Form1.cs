using System.Drawing;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using System;
using System.Threading.Tasks;
using face_direction_recognizer.viola_jones;
using System.Collections.Generic;
using face_direction_recognizer.ModelHandlers;

namespace face_direction_recognizer
{
    public partial class Form1 : Form
    {
        private VideoCaptureDevice videoSource;
        private IFilter[] filters = new IFilter[3];
        private Detector detector;
        private Detector eyeDetector;
        private System.Drawing.Rectangle defaultRect;
        public Form1()
        {
            InitializeComponent();
            InitializeFilters();
            detector = new Detector("haarcascade_frontalface_alt2.xml");
            eyeDetector = new Detector("haarcascade_eye.xml");
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
            videoSource.VideoResolution = videoSource.VideoCapabilities[4];
            defaultRect = new System.Drawing.Rectangle(0, 0, videoSource.VideoResolution.FrameSize.Width, videoSource.VideoResolution.FrameSize.Height);
            videoSource.Start();
        }
        
        private void NewFrameHandler(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            FastBitmap notModifiedBitmap = new FastBitmap(bitmap);
            FastBitmap fastBitmap = new FastBitmap(bitmap);

            {
                Harries dummy = new Harries();
                byte[] gray = fastBitmap.GrayPixels;
                var ps = dummy.Corner(gray, fastBitmap.Width, fastBitmap.Height);
                int r = 3;
                var size = new Size(r, r);
                foreach(Point p in ps)
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.DrawRectangle(Pens.GreenYellow, new System.Drawing.Rectangle(p, size));
                    }
                }
                pictureBox1.Image = bitmap;
            }

            //for (int i = 0; i < filters.Length; i++)
            //{
            //    filters[0].DoFilter(fastBitmap);
            //}
            //Parallel.For(0, fastBitmap.Width, i =>
            // {
            //     for (int j = 0; j < fastBitmap.Height; j++)
            //     {
            //         int difference = notModifiedBitmap[i, j] - fastBitmap[i, j];
            //         fastBitmap[i, j] = difference > 0 ? (byte)difference : (byte)0;
            //     }
            // });
            List<System.Drawing.Rectangle> result = detector.getElements(fastBitmap, 1, 1.25f, 0.1f, 2, defaultRect);
            foreach(System.Drawing.Rectangle rect in result)
            {
                System.Drawing.Rectangle nRect = rect;
                nRect.Height /= 2;
                List<System.Drawing.Rectangle> eyeResult = eyeDetector.getElements(fastBitmap, 1, 1.25f, 0.1f, 1, nRect);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    if (eyeResult.Count > 0) g.DrawRectangles(Pens.Blue, eyeResult.ToArray());
                }
            };
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (result.Count > 0) g.DrawRectangles(Pens.GreenYellow, result.ToArray());
            }
            pictureBox1.Image = bitmap;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            videoSource.Stop();
            videoSource.WaitForStop();
        }
    }
}
