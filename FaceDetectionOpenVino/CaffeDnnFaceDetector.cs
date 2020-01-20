using OpenCvSharp;
using OpenCvSharp.Dnn;
using Shared;
using System;
using System.Drawing;
using System.IO;

namespace FaceDetectionOpenVino
{
    public class CaffeDnnFaceDetector : IDetector
    {
        private const double Mean = 127.5;

        public Net Net { get; }

        public CaffeDnnFaceDetector(string modelFile)
        {
            var configFile = Path.ChangeExtension(modelFile, ".prototxt")!;

            Net = CvDnn.ReadNetFromCaffe(configFile, modelFile);
            //Cv2.SetNumThreads(2);
            //Net.SetPreferableBackend(Net.Backend.OPENCV);
            Net.SetPreferableBackend(Net.Backend.INFERENCE_ENGINE);
            Net.SetPreferableTarget(Net.Target.MYRIAD);
            //Net.SetPreferableTarget(Net.Target.OPENCL);
        }

        public Rectangle[] Detect(Mat mat, byte[] buffer)
        {
            var frameWidth = mat.Width;
            var frameHeight = mat.Height;
            //using var blob = CvDnn.BlobFromImage(mat, 1.0, new OpenCvSharp.Size(300, 300), new OpenCvSharp.Scalar(104, 117, 123), false, false);
            using var blob = CvDnn.BlobFromImage(mat, 1.0 / Mean, new OpenCvSharp.Size(300, 300), new OpenCvSharp.Scalar(Mean), true, false);
            Net.SetInput(blob);
            using var detections = Net.Forward();
            using var detectionMat = new Mat(detections.Size(2), detections.Size(3), MatType.CV_32F, detections.Ptr(0));
            var bestConfidence = 0.0;
            Rectangle? candidate = null;

            for (var i = 0; i < detectionMat.Rows; i++)
            {
                var confidence = detectionMat.At<float>(i, 2);
                if ((confidence > 0.5) && (confidence > bestConfidence))
                {
                    var left = (int)(detectionMat.At<float>(i, 3) * frameWidth);
                    var top = (int)(detectionMat.At<float>(i, 4) * frameHeight);
                    var right = (int)(detectionMat.At<float>(i, 5) * frameWidth);
                    var bottom = (int)(detectionMat.At<float>(i, 6) * frameHeight);
                    candidate = new Rectangle(left, top, right - left, bottom - top);
                }
            }

            if (candidate != null)
            {
                return new Rectangle[] { candidate.Value };
            }
            else
            {
                return Array.Empty<Rectangle>();
            }
        }
    }
}
