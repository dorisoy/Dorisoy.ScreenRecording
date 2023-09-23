﻿// Mp3LameAudioEncoder is not supported on .NET Standard yet
#if !NETSTANDARD
using System;

namespace ScreenRecording.Core.Codecs
{
    partial class Mp3LameAudioEncoder
    {
        /// <summary>
        /// Interface is used to access the API of the LAME DLL.
        /// </summary>
        /// <remarks>
        /// Clients of <see cref="Mp3LameAudioEncoder"/> class need not to work with
        /// this interface directly.
        /// </remarks>
        public interface ILameFacade
        {
            /// <summary>
            /// Number of audio channels.
            /// </summary>
            int ChannelCount { get; set; }

            /// <summary>
            /// Sample rate of source audio data.
            /// </summary>
            int InputSampleRate { get; set; }

            /// <summary>
            /// Bit rate of encoded data.
            /// </summary>
            int OutputBitRate { get; set; }

            /// <summary>
            /// Sample rate of encoded data.
            /// </summary>
            int OutputSampleRate { get; }

            /// <summary>
            /// Frame size of encoded data.
            /// </summary>
            int FrameSize { get; }

            /// <summary>
            /// Encoder delay.
            /// </summary>
            int EncoderDelay { get; }

            /// <summary>
            /// Initializes the encoding process.
            /// </summary>
            void PrepareEncoding();

#if NET5_0_OR_GREATER
            /// <summary>
            /// Encodes a chunk of audio data.
            /// </summary>
            int Encode(ReadOnlySpan<byte> source, int sampleCount, Span<byte> dest);

            /// <summary>
            /// Finalizes the encoding process.
            /// </summary>
            int FinishEncoding(Span<byte> dest);
#else
            /// <summary>
            /// Encodes a chunk of audio data.
            /// </summary>
            int Encode(byte[] source, int sourceIndex, int sampleCount, byte[] dest, int destIndex);

            /// <summary>
            /// Finalizes the encoding process.
            /// </summary>
            int FinishEncoding(byte[] dest, int destIndex);
#endif
        }
    }
}
#endif