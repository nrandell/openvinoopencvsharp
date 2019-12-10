using OpenCvSharp;
using OpenCvSharp.Dnn;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Shared;

namespace PeopleDetectionOpenCV
{
    public class CaffeDnnFaceDetector : IDetector
    {
        public Net Net { get; }

        public CaffeDnnFaceDetector(string modelFile)
        {
            var modelDirectory = Path.GetDirectoryName(modelFile)!;
            var configFile = Path.Combine(modelDirectory, "deploy.prototxt");

            Net = CvDnn.ReadNetFromCaffe(configFile, modelFile);
            Net.SetPreferableBackend(Net.Backend.INFERENCE_ENGINE);
            Net.SetPreferableTarget(Net.Target.OPENCL_FP16);
        }

        public Rectangle[] Detect(Mat mat, byte[] buffer)
        {
            var frameWidth = mat.Width;
            var frameHeight = mat.Height;
            using var blob = CvDnn.BlobFromImage(mat, 1.0, new OpenCvSharp.Size(300, 300), new OpenCvSharp.Scalar(104, 117, 123), false, false);
            Net.SetInput(blob);
            using var detections = Net.Forward();
            using var detectionMat = new Mat(detections.Size(2), detections.Size(3), MatType.CV_32F, detections.Ptr(0));
            var rectangles = new List<Rectangle>();
            for (var i = 0; i < detectionMat.Rows; i++)
            {
                var confidence = detectionMat.At<float>(i, 2);
                if (confidence > 0.7)
                {
                    var left = (int)(detectionMat.At<float>(i, 3) * frameWidth);
                    var top = (int)(detectionMat.At<float>(i, 4) * frameHeight);
                    var right = (int)(detectionMat.At<float>(i, 5) * frameWidth);
                    var bottom = (int)(detectionMat.At<float>(i, 6) * frameHeight);

                    rectangles.Add(new Rectangle(left, top, right - left, bottom - top));
                }
            }

            return rectangles.ToArray();
        }
    }
}
