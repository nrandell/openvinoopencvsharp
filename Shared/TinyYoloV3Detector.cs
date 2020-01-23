using OpenCvSharp;
using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Shared
{
    public class TinyYoloV3Detector : IDetector
    {
        public Net Net { get; }
        public string[] OutputLayerNames { get; }
        public int[] OutputLayerIds { get; }
        public string?[] LayerNames { get; }

        public TinyYoloV3Detector(string modelFile)
        {
            Net = CvDnn.ReadNetFromDarknet(Path.ChangeExtension(modelFile, ".cfg"), Path.ChangeExtension(modelFile, ".weights"));
            //Net.SetPreferableBackend(Net.Backend.INFERENCE_ENGINE);
            Net.SetPreferableBackend(Net.Backend.OPENCV);
            Net.SetPreferableTarget(Net.Target.CPU);
            OutputLayerIds = Net.GetUnconnectedOutLayers();
            LayerNames = Net.GetLayerNames();
            OutputLayerNames = Net.GetUnconnectedOutLayersNames()!;
        }

        public Rectangle[] Detect(Mat mat, byte[] buffer)
        {
            var matWidth = mat.Width;
            var matHeight = mat.Height;
            //using var blob = CvDnn.BlobFromImage(mat, 1.0, new OpenCvSharp.Size(300, 300), new OpenCvSharp.Scalar(104, 117, 123), false, false);
            //using var blob = CvDnn.BlobFromImage(mat, 1.0, new OpenCvSharp.Size(416, 416), mean: new OpenCvSharp.Scalar(0, 0, 0), swapRB: true, crop: false, ddepth: MatType.CV_8U); // , ddepth: MatType.CV_8U);
            //using var blob = CvDnn.BlobFromImage(mat, 1.0, new OpenCvSharp.Size(416, 416), mean: new OpenCvSharp.Scalar(0, 0, 0), swapRB: true, crop: false, ddepth: MatType.CV_8U); // , ddepth: MatType.CV_8U);
            //Net.SetInput(blob, scaleFactor: 1.0 / 255.0);
            using var blob = CvDnn.BlobFromImage(mat, 1.0 / 255.0, new OpenCvSharp.Size(416, 416), crop: false);
            Net.SetInput(blob);

            var outputLayers = new Mat[OutputLayerNames.Length];
            try
            {
                for (var i = 0; i < outputLayers.Length; i++)
                {
                    outputLayers[i] = new Mat();
                }
                Net.Forward(outputLayers, OutputLayerNames);

                //using var detections = Net.Forward()
                //using var detectionMat = new Mat(detections.Size(2), detections.Size(3), MatType.CV_32F, detections.Ptr(0));
                var rects = new List<Rect>();
                var confidences = new List<float>();
                var rectangles = new List<Rectangle>();
                for (var i = 0; i < outputLayers.Length; i++)
                {
                    var outs = outputLayers[i];
                    for (var j = 0; j < outs.Rows; j++)
                    {
                        var cols = outs.Row(j).ColRange(5, outs.Cols);
                        Cv2.MinMaxLoc(cols, out var _, out var confidence, out var _, out var maxVal);
                        //Cv2.MinMaxLoc(cols, out var minVal, out OpenCvSharp.Point maxVal);
                        var classes = maxVal.X;
                        //var confidence = outs.At<float>(j, classes + 5);
                        if (confidence > 0.5)
                        {
                            var centerX = outs.At<float>(j, 0) * matWidth;
                            var centerY = outs.At<float>(j, 1) * matHeight;
                            var width = outs.At<float>(j, 2) * matWidth;
                            var height = outs.At<float>(j, 3) * matHeight;
                            var left = centerX - (width / 2);
                            var top = centerY - (height / 2);
                            rects.Add(new Rect((int)left, (int)top, (int)width, (int)height));
                            confidences.Add((float)confidence);
                            Console.WriteLine($"Class {classes} at {centerX},{centerY} of {width}x{height} with confidence {confidence}");
                            rectangles.Add(new Rectangle((int)(centerX - (width / 2)), (int)(centerY - (height / 2)), (int)(centerX + (width / 2)), (int)(centerY + (height / 2))));
                        }

                    }

                }




                //for (var i = 0; i < detectionMat.Rows; i++)
                //{
                //    var confidence = detectionMat.At<float>(i, 2);
                //    if (confidence > 0.7)
                //    {
                //        var left = (int)(detectionMat.At<float>(i, 3) * frameWidth);
                //        var top = (int)(detectionMat.At<float>(i, 4) * frameHeight);
                //        var right = (int)(detectionMat.At<float>(i, 5) * frameWidth);
                //        var bottom = (int)(detectionMat.At<float>(i, 6) * frameHeight);

                //        rectangles.Add(new Rectangle(left, top, right - left, bottom - top));
                //    }
                //}

                CvDnn.NMSBoxes(rects, confidences, 0.5f, 0.3f, out var indices);
                var results = new List<Rectangle>(indices.Length);
                foreach (var index in indices)
                {
                    results.Add(rectangles[index]);
                }
                return results.ToArray();
            }
            finally
            {
                foreach (var outputLayer in outputLayers)
                {
                    outputLayer?.Dispose();
                }
            }
        }

    }
}
