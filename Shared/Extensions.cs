using OpenCvSharp;
using OpenCvSharp.Util;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Shared
{
    public static class Extensions
    {
        public static Bitmap ToBitmap(this Mat src)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            var pf = (src.Channels()) switch
            {
                1 => PixelFormat.Format8bppIndexed,
                3 => PixelFormat.Format24bppRgb,
                4 => PixelFormat.Format32bppArgb,
                _ => throw new ArgumentException("Number of channels must be 1, 3 or 4.", nameof(src)),
            };
            return ToBitmap(src, pf);
        }

        public static Bitmap ToBitmap(this Mat src, PixelFormat pf)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            src.ThrowIfDisposed();

            Bitmap bitmap = new Bitmap(src.Width, src.Height, pf);
            ToBitmap(src, bitmap);
            return bitmap;
        }

        public static unsafe void ToBitmap(this Mat src, Bitmap dst)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (dst == null)
                throw new ArgumentNullException(nameof(dst));
            if (src.IsDisposed)
                throw new ArgumentException("The image is disposed.", nameof(src));
            if (src.Depth() != MatType.CV_8U)
                throw new ArgumentException("Depth of the image must be CV_8U");
            //if (src.IsSubmatrix())
            //    throw new ArgumentException("Submatrix is not supported");
            if (src.Width != dst.Width || src.Height != dst.Height)
                throw new ArgumentException("");

            PixelFormat pf = dst.PixelFormat;

            // 1プレーン用の場合、グレースケールのパレット情報を生成する
            if (pf == PixelFormat.Format8bppIndexed)
            {
                ColorPalette plt = dst.Palette;
                for (int x = 0; x < 256; x++)
                {
                    plt.Entries[x] = Color.FromArgb(x, x, x);
                }
                dst.Palette = plt;
            }

            int w = src.Width;
            int h = src.Height;
            Rectangle rect = new Rectangle(0, 0, w, h);
            BitmapData? bd = null;

            bool submat = src.IsSubmatrix();
            bool continuous = src.IsContinuous();

            try
            {
                bd = dst.LockBits(rect, ImageLockMode.WriteOnly, pf);

                IntPtr srcData = src.Data;
                byte* pSrc = (byte*)(srcData.ToPointer());
                byte* pDst = (byte*)(bd.Scan0.ToPointer());
                int ch = src.Channels();
                int sstep = (int)src.Step();
                int dstep = ((src.Width * ch) + 3) / 4 * 4; // 4の倍数に揃える
                int stride = bd.Stride;

                switch (pf)
                {
                    case PixelFormat.Format1bppIndexed:
                        {
                            if (submat)
                                throw new NotImplementedException("submatrix not supported");

                            // BitmapDataは4byte幅だが、IplImageは1byte幅
                            // 手作業で移し替える                 
                            //int offset = stride - (w / 8);
                            int x = 0;
                            int y;
                            int bytePos;
                            byte mask;
                            byte b = 0;
                            int i;
                            for (y = 0; y < h; y++)
                            {
                                for (bytePos = 0; bytePos < stride; bytePos++)
                                {
                                    if (x < w)
                                    {
                                        for (i = 0; i < 8; i++)
                                        {
                                            mask = (byte)(0x80 >> i);
                                            if (x < w && pSrc[sstep * y + x] == 0)
                                                b &= (byte)(mask ^ 0xff);
                                            else
                                                b |= mask;

                                            x++;
                                        }
                                        pDst[bytePos] = b;
                                    }
                                }
                                x = 0;
                                pDst += stride;
                            }
                            break;
                        }

                    case PixelFormat.Format8bppIndexed:
                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppArgb:
                        if (sstep == dstep && !submat && continuous)
                        {
                            uint imageSize = (uint)(src.DataEnd.ToInt64() - src.Data.ToInt64());
                            MemoryHelper.CopyMemory(pDst, pSrc, imageSize);
                        }
                        else
                        {
                            for (int y = 0; y < h; y++)
                            {
                                long offsetSrc = (y * sstep);
                                long offsetDst = (y * dstep);
                                // 一列ごとにコピー
                                MemoryHelper.CopyMemory(pDst + offsetDst, pSrc + offsetSrc, w * ch);
                            }
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            finally
            {
                dst.UnlockBits(bd);
            }
        }

    }
}