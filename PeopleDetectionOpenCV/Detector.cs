#if USE_DLIB
using DlibDotNet;
using OpenCvSharp;
using System;

namespace PeopleDetectionOpenCV
{
    public class Detector : IDisposable
    {
        public FrontalFaceDetector FaceDetector { get; }

        public Detector()
        {
            FaceDetector = Dlib.GetFrontalFaceDetector();
        }

        public Rectangle[] Detect(Mat mat, byte[] buffer)
        {
            var height = (uint)mat.Height;
            var width = (uint)mat.Width;
            using var cimg = Dlib.LoadImageData<BgrPixel>(buffer, height, width, width * 3);
            var faces = FaceDetector.Operator(cimg);
            return faces;
        }

        public void Dispose()
        {
            FaceDetector.Dispose();
        }
    }
}
#endif
