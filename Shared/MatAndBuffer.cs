using OpenCvSharp;

namespace Shared
{
    public class MatAndBuffer
    {
        public Mat Mat { get; }
        public byte[] Buffer { get; }

        public MatAndBuffer(Mat mat, byte[] buffer)
        {
            Mat = mat;
            Buffer = buffer;
        }
    }
}
