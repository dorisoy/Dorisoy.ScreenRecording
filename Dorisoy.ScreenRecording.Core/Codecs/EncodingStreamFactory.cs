﻿using ScreenRecording.Core.Output;
using ScreenRecording.Core.Utilities;
using System;
using System.Linq;

namespace ScreenRecording.Core.Codecs
{
    /// <summary>
    /// Provides extension methods for creating encoding streams with specific encoders.
    /// </summary>
    public static class EncodingStreamFactory
    {
        /// <summary>
        /// Adds new video stream with <see cref="UncompressedVideoEncoder"/>.
        /// </summary>
        /// <seealso cref="AviWriter.AddEncodingVideoStream"/>
        /// <seealso cref="UncompressedVideoEncoder"/>
        public static IAviVideoStream AddUncompressedVideoStream(this AviWriter writer, int width, int height)
        {
            Argument.IsNotNull(writer, nameof(writer));
            Argument.IsPositive(width, nameof(width));
            Argument.IsPositive(height, nameof(height));

            var encoder = new UncompressedVideoEncoder(width, height);
            return writer.AddEncodingVideoStream(encoder, true, width, height);
        }

#if NET48 || NET8_0_OR_GREATER && WINDOWS
        /// <summary>
        /// Adds new video stream with <see cref="MJpegWpfVideoEncoder"/>.
        /// </summary>
        /// <param name="writer">Writer object to which new stream is added.</param>
        /// <param name="width">Frame width.</param>
        /// <param name="height">Frame height.</param>
        /// <param name="quality">Requested quality of compression.</param>
        /// <seealso cref="AviWriter.AddEncodingVideoStream"/>
        /// <seealso cref="MJpegWpfVideoEncoder"/>
        public static IAviVideoStream AddMJpegWpfVideoStream(this AviWriter writer, int width, int height, int quality = 70)
        {
            Argument.IsNotNull(writer, nameof(writer));
            Argument.IsPositive(width, nameof(width));
            Argument.IsPositive(height, nameof(height));
            Argument.IsInRange(quality, 1, 100, nameof(quality));

            var encoder = new MJpegWpfVideoEncoder(width, height, quality);
            return writer.AddEncodingVideoStream(encoder, true, width, height);
        }
#endif

        /// <summary>
        /// Adds new video stream with <see cref="Mpeg4VcmVideoEncoder"/>.
        /// </summary>
        /// <param name="writer">Writer object to which new stream is added.</param>
        /// <param name="width">Frame width.</param>
        /// <param name="height">Frame height.</param>
        /// <param name="fps">Frames rate of the video.</param>
        /// <param name="frameCount">Number of frames if known in advance. Otherwise, specify <c>0</c>.</param>
        /// <param name="quality">Requested quality of compression.</param>
        /// <param name="codec">Specific MPEG-4 codec to use.</param>
        /// <param name="forceSingleThreadedAccess">
        /// When <c>true</c>, the created <see cref="Mpeg4VcmVideoEncoder"/> instance is wrapped into
        /// <see cref="SingleThreadedVideoEncoderWrapper"/>.
        /// </param>
        /// <seealso cref="AviWriter.AddEncodingVideoStream"/>
        /// <seealso cref="Mpeg4VcmVideoEncoder"/>
        /// <seealso cref="SingleThreadedVideoEncoderWrapper"/>
        public static IAviVideoStream AddMpeg4VcmVideoStream(this AviWriter writer, int width, int height,
            double fps, int frameCount = 0, int quality = 70, FourCC? codec = null,
            bool forceSingleThreadedAccess = false)
        {
            Argument.IsNotNull(writer, nameof(writer));
            Argument.IsPositive(width, nameof(width));
            Argument.IsPositive(height, nameof(height));
            Argument.IsPositive(fps, nameof(fps));
            Argument.IsNotNegative(frameCount, nameof(frameCount));
            Argument.IsInRange(quality, 1, 100, nameof(quality));

            var encoderFactory = codec.HasValue
                ? new Func<IVideoEncoder>(() => new Mpeg4VcmVideoEncoder(width, height, fps, frameCount, quality, codec.Value))
                : new Func<IVideoEncoder>(() => new Mpeg4VcmVideoEncoder(width, height, fps, frameCount, quality));
            var encoder = forceSingleThreadedAccess
                ? new SingleThreadedVideoEncoderWrapper(encoderFactory)
                : encoderFactory.Invoke();
            return writer.AddEncodingVideoStream(encoder, true, width, height);
        }

#if !NETSTANDARD
        /// <summary>
        /// Adds new audio stream with <see cref="Mp3LameAudioEncoder"/>.
        /// </summary>
        /// <seealso cref="AviWriter.AddEncodingAudioStream"/>
        /// <seealso cref="Mp3LameAudioEncoder"/>
        public static IAviAudioStream AddMp3LameAudioStream(this AviWriter writer, int channelCount, int sampleRate, int outputBitRateKbps = 160)
        {
            Argument.IsNotNull(writer, nameof(writer));
            Argument.IsInRange(channelCount, 1, 2, nameof(channelCount));
            Argument.IsPositive(sampleRate, nameof(sampleRate));
            Argument.Meets(Mp3LameAudioEncoder.SupportedBitRates.Contains(outputBitRateKbps), nameof(outputBitRateKbps));

            var encoder = new Mp3LameAudioEncoder(channelCount, sampleRate, outputBitRateKbps);
            return writer.AddEncodingAudioStream(encoder, true);
        }
#endif
    }
}
