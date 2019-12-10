using FFmpeg.AutoGen;
using System;
using System.Runtime.InteropServices;

namespace Shared
{
    public static class FFmpegHelper
    {
        public static unsafe string? AvStrerror(int error)
        {
            const int bufferSize = 1024;
            Span<byte> buffer = stackalloc byte[bufferSize];
            fixed (byte* bp = buffer)
            {
                ffmpeg.av_strerror(error, bp, (ulong)bufferSize);
                return Marshal.PtrToStringAnsi((IntPtr)bp);
            }
        }

        public static int ThrowExceptionIfError(this int error)
        {
            if (error < 0) throw new ApplicationException(AvStrerror(error));
            return error;
        }

        public static void Register()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    ffmpeg.RootPath = @"c:\utils\ffmpeg\bin";
                    break;

                default:
                    break;
            }

            ffmpeg.avdevice_register_all();
        }
    }
}
