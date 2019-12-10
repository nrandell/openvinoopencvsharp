using OpenCvSharp;
using OpenCvSharp.Dnn;
using Shared;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PeopleDetectionOpenCV
{
    public class TensorFlowDnnFaceDetector : IDetector
    {
        public Net Net { get; }

        public TensorFlowDnnFaceDetector(string modelFile)
        {
            var modelDirectory = Path.GetDirectoryName(modelFile)!;
            var configFile = Path.Combine(modelDirectory, "opencv_face_detector.pbtxt");

            Net = CvDnn.ReadNetFromTensorflow(modelFile, configFile);
            // Net.SetPreferableTarget(Net.Target.OPENCL_FP16);
            Net.SetPreferableBackend(Net.Backend.OPENCV);
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
