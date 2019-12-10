using FFmpeg.AutoGen;
using OpenCvSharp;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Shared
{
    public unsafe class Converter : IDisposable
    {
        public System.Drawing.Size Size { get; }
        private readonly int _convertedBufferSize;
        private readonly IntPtr _convertedFrameBuffer;
        private readonly byte_ptrArray4 _dstData;
        private readonly int_array4 _dstLineSize;
        private readonly SwsContext* _convertContext;
        private readonly byte[] _buffer;
        private readonly GCHandle _bufferHandle;

        public Converter(System.Drawing.Size size, AVPixelFormat sourcePixelFormat)
        {
            Size = size;
            const AVPixelFormat targetPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
            var context = ffmpeg.sws_getContext(size.Width, size.Height, sourcePixelFormat, size.Width, size.Height, targetPixelFormat, ffmpeg.SWS_FAST_BILINEAR, null, null, null);
            _convertedBufferSize = ffmpeg.av_image_get_buffer_size(targetPixelFormat, size.Width, size.Height, 1);

            _buffer = new byte[_convertedBufferSize];
            _bufferHandle = GCHandle.Alloc(_buffer);

            _convertedFrameBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, 0);
            _dstData = new byte_ptrArray4();
            _dstLineSize = new int_array4();
            _convertContext = context;

            ffmpeg.av_image_fill_arrays(ref _dstData, ref _dstLineSize, (byte*)_convertedFrameBuffer, targetPixelFormat, size.Width, size.Height, 1);
        }

        public void Dispose()
        {
            _bufferHandle.Free();
            ffmpeg.sws_freeContext(_convertContext);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        internal static void DummyFreeData(IntPtr _, IntPtr __, IntPtr ___) { }
#pragma warning restore IDE0060 // Remove unused parameter

        public MatAndBuffer Convert(AVFrame* frame)
        {
            ffmpeg.sws_scale(_convertContext, frame->data, frame->linesize, 0, frame->height, _dstData, _dstLineSize);
            var mat = new Mat(Size.Height, Size.Width, MatType.CV_8UC3, _convertedFrameBuffer);
            return new MatAndBuffer(mat, _buffer);
        }
    }
}
