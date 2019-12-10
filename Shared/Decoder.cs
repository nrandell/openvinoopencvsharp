using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Shared
{
    public unsafe class Decoder : IDisposable
    {
        private AVFormatContext* _inputContext;
        private AVCodecContext* _decoderContext;
        private AVBufferRef* _hwDeviceContext;
        public readonly int StreamIndex;
        public readonly string CodecName;
        public readonly System.Drawing.Size FrameSize;
        public readonly AVPixelFormat PixelFormat;
        private readonly AVPixelFormat _hwPixelFormat;
        private readonly Converter _converter;
        private readonly DataTransfer<MatAndBuffer> _dataTransfer;

        public Decoder(string format, string camera, DataTransfer<MatAndBuffer> dataTransfer, bool enableHardware = false)
        {
            _dataTransfer = dataTransfer;
            var inputFormat = ffmpeg.av_find_input_format(format);
            if (inputFormat == null)
            {
                throw new ApplicationException($"Failed to find input format '{format}'");
            }

            var inputContext = ffmpeg.avformat_alloc_context();
            try
            {
                AVDictionary* options = null;
                ffmpeg.av_dict_set(&options, "video_size", "640x480", ffmpeg.AV_DICT_APPEND);
                ffmpeg.avformat_open_input(&inputContext, camera, inputFormat, &options).ThrowExceptionIfError();
                ffmpeg.av_dict_free(&options);
                options = null;

                try
                {
                    ffmpeg.avformat_find_stream_info(inputContext, null).ThrowExceptionIfError();
                    AVCodec* decoder;
                    var videoStream = ffmpeg.av_find_best_stream(inputContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &decoder, 0);
                    if (videoStream < 0)
                    {
                        throw new ApplicationException("No video stream found");
                    }

                    AVBufferRef* hwDeviceContext = null;
                    var (hwPixelFormat, pixelFormat) = enableHardware ? SortoutHardware(decoder, out hwDeviceContext) : (AVPixelFormat.AV_PIX_FMT_NONE, AVPixelFormat.AV_PIX_FMT_NONE);

                    var decoderContext = ffmpeg.avcodec_alloc_context3(decoder);
                    var video = inputContext->streams[videoStream];
                    video->discard = AVDiscard.AVDISCARD_NONKEY;
                    ffmpeg.avcodec_parameters_to_context(decoderContext, video->codecpar).ThrowExceptionIfError();

                    if (hwPixelFormat != AVPixelFormat.AV_PIX_FMT_NONE)
                    {
                        AVCodecContext_get_format getFormat = (_, formats) =>
                        {
                            //AVPixelFormat* pixelFormat;
                            for (var pixelFormat = formats; *pixelFormat != AVPixelFormat.AV_PIX_FMT_NONE; pixelFormat++)
                            {
                                if (*pixelFormat == hwPixelFormat)
                                {
                                    return *pixelFormat;
                                }
                            }
                            throw new ApplicationException("Failed to get hardware pixel format");
                        };

                        decoderContext->get_format = getFormat;
                    }

                    ffmpeg.av_opt_set_int(decoderContext, "refcounted_frames", 1, 0);

                    if (hwPixelFormat != AVPixelFormat.AV_PIX_FMT_NONE)
                    {
                        decoderContext->hw_device_ctx = ffmpeg.av_buffer_ref(hwDeviceContext);
                    }
                    else
                    {
                        pixelFormat = ConvertFormat(video->codec->pix_fmt);
                    }

                    ffmpeg.avcodec_open2(decoderContext, decoder, null).ThrowExceptionIfError();

                    // Now all opened
                    _inputContext = inputContext;
                    _decoderContext = decoderContext;
                    CodecName = ffmpeg.avcodec_get_name(decoder->id);
                    FrameSize = new System.Drawing.Size(video->codec->width, video->codec->height);
                    PixelFormat = pixelFormat;
                    StreamIndex = videoStream;
                    _hwPixelFormat = hwPixelFormat;

                    _converter = new Converter(FrameSize, pixelFormat);

                    Console.WriteLine($"Opened stream {StreamIndex} of {CodecName} as {FrameSize.Width} x {FrameSize.Height} @ {PixelFormat}");
                }
                catch (Exception)
                {
                    ffmpeg.avformat_close_input(&inputContext);
                    throw;
                }
            }
            catch (Exception)
            {
                ffmpeg.avformat_free_context(inputContext);
                throw;
            }
        }

        private static AVPixelFormat ConvertFormat(AVPixelFormat format) => format switch
        {
            AVPixelFormat.AV_PIX_FMT_YUVJ411P => AVPixelFormat.AV_PIX_FMT_YUV411P,
            AVPixelFormat.AV_PIX_FMT_YUVJ420P => AVPixelFormat.AV_PIX_FMT_YUV420P,
            AVPixelFormat.AV_PIX_FMT_YUVJ422P => AVPixelFormat.AV_PIX_FMT_YUV422P,
            AVPixelFormat.AV_PIX_FMT_YUVJ444P => AVPixelFormat.AV_PIX_FMT_YUV444P,
            AVPixelFormat.AV_PIX_FMT_YUVJ440P => AVPixelFormat.AV_PIX_FMT_YUV440P,
            _ => format,
        };

        private static (AVPixelFormat hwPixelFormat, AVPixelFormat pixelFormat) SortoutHardware(AVCodec* decoder, out AVBufferRef* hwDeviceContext)
        {
            var pixelFormat = AVPixelFormat.AV_PIX_FMT_NONE;
            var hwPixelFormat = AVPixelFormat.AV_PIX_FMT_NONE;
            var hwDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
            AVBufferRef* localHwDeviceContext = hwDeviceContext = null;

            for (var i = 0; ; i++)
            {
                var hwConfig = ffmpeg.avcodec_get_hw_config(decoder, i);
                if (hwConfig == null)
                {
                    Console.WriteLine("No hardware decoder");
                    break;
                }
                Console.WriteLine($"hwConfig: {hwConfig->methods} {hwConfig->pix_fmt} {hwConfig->device_type}");
            }

            for (var i = 0; ; i++)
            {
                var hwConfig = ffmpeg.avcodec_get_hw_config(decoder, i);
                if (hwConfig == null)
                {
                    Console.WriteLine("No hardware decoder");
                    break;
                }

                Console.WriteLine($"hwConfig: {hwConfig->methods} {hwConfig->pix_fmt} {hwConfig->device_type}");
                if ((hwConfig->methods & 0x02 /*FFmpeg.AutoGen. AV_CODEC_HW_CONFIG_METHOD_HW_FRAMES_CTX*/) != 0)
                {
                    ffmpeg.av_hwdevice_ctx_create(&localHwDeviceContext, hwConfig->device_type, null, null, 0).ThrowExceptionIfError();

                    var constraints = ffmpeg.av_hwdevice_get_hwframe_constraints(localHwDeviceContext, null);
                    AVPixelFormat found = AVPixelFormat.AV_PIX_FMT_NONE;
                    if (constraints != null)
                    {
                        AVPixelFormat first = AVPixelFormat.AV_PIX_FMT_NONE;
                        for (var p = constraints->valid_sw_formats; *p != AVPixelFormat.AV_PIX_FMT_NONE; p++)
                        {
                            Console.WriteLine($"Try pixel format {*p}");
                            if (ffmpeg.sws_isSupportedInput(*p) != 0)
                            {
                                Console.WriteLine($"Valid pixel format {*p}");
                                if (*p == AVPixelFormat.AV_PIX_FMT_NV12)
                                {
                                    found = *p;
                                    first = *p;
                                    break;
                                }
                                else if (first == AVPixelFormat.AV_PIX_FMT_NONE)
                                {
                                    first = *p;
                                }
                            }
                        }
                        ffmpeg.av_hwframe_constraints_free(&constraints);
                        if (found == AVPixelFormat.AV_PIX_FMT_NONE)
                        {
                            found = first;
                        }
                    }
                    if (found != AVPixelFormat.AV_PIX_FMT_NONE)
                    {
                        pixelFormat = found;
                        hwPixelFormat = hwConfig->pix_fmt;
                        hwDeviceType = hwConfig->device_type;
                        hwDeviceContext = localHwDeviceContext;
                        break;
                    }
                    else
                    {
                        ffmpeg.av_buffer_unref(&localHwDeviceContext);
                    }
                }
            }

            return (hwPixelFormat, pixelFormat);
        }

        public void Run()
        {
            var _packet = ffmpeg.av_packet_alloc();
            var _frameA = ffmpeg.av_frame_alloc();
            var _frameB = ffmpeg.av_frame_alloc();
            var _swFrame = ffmpeg.av_frame_alloc();
            while (true)
            {
                int error;
                do
                {
                    ffmpeg.av_packet_unref(_packet);
                    error = ffmpeg.av_read_frame(_inputContext, _packet);
                    if (error == ffmpeg.AVERROR_EOF)
                    {
                        throw new EndOfStreamException("Finished stream");
                    }
                    error.ThrowExceptionIfError();
                } while (_packet->stream_index != StreamIndex);

                ffmpeg.avcodec_send_packet(_decoderContext, _packet);

                var useA = true;
                while (true)
                {
                    var frame = useA ? _frameA : _frameB;
                    ffmpeg.av_frame_unref(frame);
                    error = ffmpeg.avcodec_receive_frame(_decoderContext, frame);
                    if (error == ffmpeg.AVERROR(ffmpeg.EAGAIN))
                    {
                        break;
                    }
                    else if (error == ffmpeg.AVERROR_EOF)
                    {
                        throw new EndOfStreamException("No more frames");
                    }
                    error.ThrowExceptionIfError();

                    if (_dataTransfer.CanWrite)
                    {
                        AVFrame* tmpFrame;
                        var isHw = frame->format == (int)_hwPixelFormat;
                        if (isHw)
                        {
                            ffmpeg.av_frame_unref(_swFrame);
                            _swFrame->format = (int)PixelFormat;
                            ffmpeg.av_hwframe_transfer_data(_swFrame, frame, 0).ThrowExceptionIfError();
                            ffmpeg.av_frame_copy_props(_swFrame, frame);
                            tmpFrame = _swFrame;
                        }
                        else
                        {
                            tmpFrame = frame;
                        }

                        var array = _converter.Convert(tmpFrame);
                        if (_dataTransfer.TryWrite(array))
                        {
                            useA = !useA;
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_inputContext != null)
            {
                //ffmpeg.av_frame_unref(_frameA);
                //ffmpeg.av_free(_frameA);
                //_frameA = null;

                //ffmpeg.av_frame_unref(_frameB);
                //ffmpeg.av_free(_frameB);
                //_frameB = null;

                //ffmpeg.av_frame_unref(_swFrame);
                //ffmpeg.av_free(_swFrame);
                //_swFrame = null;

                //ffmpeg.av_packet_unref(_packet);
                //ffmpeg.av_free(_packet);
                //_packet = null;

                var hwDeviceContext = _hwDeviceContext;
                ffmpeg.av_buffer_unref(&hwDeviceContext);
                _hwDeviceContext = null;

                ffmpeg.avcodec_close(_decoderContext);
                _decoderContext = null;

                var inputContext = _inputContext;
                ffmpeg.avformat_close_input(&inputContext);
                _inputContext = null;

                _converter.Dispose();
            }
        }

        public IReadOnlyDictionary<string, string> GetContextInfo()
        {
            AVDictionaryEntry* tag = null;
            var result = new Dictionary<string, string>();
            while ((tag = ffmpeg.av_dict_get(_inputContext->metadata, "", tag, ffmpeg.AV_DICT_IGNORE_SUFFIX)) != null)
            {
                var key = Marshal.PtrToStringAnsi((IntPtr)tag->key);
                var value = Marshal.PtrToStringAnsi((IntPtr)tag->value);

                if ((key != null) && (value != null))
                {
                    result.Add(key, value);
                }
            }

            return result;
        }
    }
}
