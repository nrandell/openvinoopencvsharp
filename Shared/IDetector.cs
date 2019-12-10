using OpenCvSharp;
using System.Drawing;

namespace Shared
{
    public interface IDetector
    {
        Rectangle[] Detect(Mat mat, byte[] buffer);
    }
}
