using OpenCvSharp;
using Shared;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FaceDetectionOpenVino
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var path = Environment.GetEnvironmentVariable("PATH");
            Environment.SetEnvironmentVariable("PATH", @"C:\Program Files (x86)\IntelSWTools\openvino\deployment_tools\inference_engine\bin\intel64\Release;C:\Program Files (x86)\IntelSWTools\openvino\opencv\bin;" + path);
            WindowsLibraryLoader.Instance.AdditionalPaths.Add(@"C:\Users\nick\Source\Repos\OpenVinoOpenCvSharp\install\bin");
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: PersonDetection <input format> <input camera> <model> <output>");
                return -1;
            }

            try
            {
                FFmpegHelper.Register();

                var buildInfo = OpenCvSharp.Cv2.GetBuildInformation();
                Console.WriteLine($"Build info: {buildInfo}");

                var inputFormat = args[0];
                var inputCamera = args[1];
                var model = args[2];
                var output = args[3];

                var outputFolder = new DirectoryInfo(output);
                if (!outputFolder.Exists)
                {
                    outputFolder.Create();
                }

                var dataTransfer = new DataTransfer<MatAndBuffer>();
                var detector = new InferenceEngineDetector(model);
                ProcessStream(inputFormat, inputCamera, dataTransfer);

                var counter = 0;
                while (true)
                {
                    using var data = await dataTransfer.GetNext(default);
                    HandleFrame(data.Value.Mat, data.Value.Buffer, ref counter, detector, outputFolder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex.Message}");
                Console.WriteLine(ex);
                return -2;
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

        private static void HandleFrame(Mat mat, byte[] buffer, ref int counter, IDetector detector, DirectoryInfo outputFolder)
        {
            var sw = Stopwatch.StartNew();

            var faces = detector.Detect(mat, buffer);
            var predictTime = sw.Elapsed;
            if (faces.Length > 0)
            {
                using var bitmap = mat.ToBitmap();

                AugmentOutputImage(faces, bitmap);
                var outputFileName = Path.Combine(outputFolder.FullName, $"image-{counter:D03}.png");
                bitmap.Save(outputFileName, ImageFormat.Png);
                counter = (counter + 1) % 20;
                Console.WriteLine($"Processed {counter} in {predictTime} total {sw.Elapsed}");
            }
            else
            {
                Console.WriteLine($"Skipped in {predictTime} total {sw.Elapsed}");
            }
        }

        private static void AugmentOutputImage(Rectangle[] output, Bitmap bitmap)
        {
            Console.WriteLine($"Got {output.Length} detections");

            using var graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.Red, 4);

            for (var i = 0; i < output.Length; i++)
            {
                var rectangle = output[i];
                var rect = new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
                graphics.DrawRectangle(pen, rect);
            }
        }
    }
}
