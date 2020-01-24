using OpenCvSharp;
using OpenCvSharp.Dnn;
using Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StandaloneFaceDetection
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("usage: StandaloneFaceDetection <input format> <input camera> <model> <output>");
                return -1;
            }

            try
            {
                FFmpegHelper.Register();

                var buildInfo = OpenCvSharp.Cv2.GetBuildInformation();
                Console.WriteLine($"Build information: {buildInfo}");

                var inputFormat = args[0];
                var inputCamera = args[1];
                var model = args[2];
                var output = args[3];
                var target = args.Length < 5 ? Net.Target.CPU : Enum.Parse<Net.Target>(args[4]);
                var backend = args.Length < 6 ? Net.Backend.OPENCV : Enum.Parse<Net.Backend>(args[5]); 

                var outputFolder = new DirectoryInfo(output);
                if (!outputFolder.Exists)
                {
                    outputFolder.Create();
                }

                var dataTransfer = new DataTransfer<MatAndBuffer>();
                IDetector detector;
                if (model.EndsWith("bin", StringComparison.OrdinalIgnoreCase))
                {
                    detector = new InferenceEngineDetector(model, backend, target);
                }
                else if (model.EndsWith("cfg", StringComparison.OrdinalIgnoreCase))
                {
                    detector = new TinyYoloV3Detector(model);
                }
                else if (model.EndsWith("caffemodel", StringComparison.OrdinalIgnoreCase))
                {
                    detector = new CaffeDnnFaceDetector(model);
                }
                else
                {
                    throw new ArgumentException($"Unknown model type: {model}");
                }
                ProcessStream(inputFormat, inputCamera, dataTransfer);

                var counter = 0;
                while (true)
                {
                    using var data = await dataTransfer.GetNext(default);
                    HandleFrame(data.Value.Mat, data.Value.Buffer, ref counter, detector);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return -1;
            }
        }

        private static void ProcessStream(string inputFormat, string inputCamera, DataTransfer<MatAndBuffer> dataTransfer)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    using var decoder = new Decoder(inputFormat, inputCamera, dataTransfer);
                    Console.WriteLine($"Codec name: {decoder.CodecName}");

                    var info = decoder.GetContextInfo();
                    foreach (var (key, value) in info)
                    {
                        Console.WriteLine($"{key} = '{value}'");
                    }
                    decoder.Run();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in thread: {ex.Message}");
                }
            });

            thread.Start();
        }

        private static void HandleFrame(Mat mat, byte[] buffer, ref int counter, IDetector detector)
        {
            var sw = Stopwatch.StartNew();

            var faces = detector.Detect(mat, buffer);
            var predictTime = sw.Elapsed;
            if (faces.Length > 0)
            {
                var rectangle = faces[0];
                var centerX = rectangle.Left + (rectangle.Width / 2);
                var centerY = rectangle.Top + (rectangle.Height / 2);
                Console.WriteLine($"Processed {counter} in {predictTime} @ ({centerX},{centerY})");
            }
        }
    }
}
