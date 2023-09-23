﻿using ScreenRecording.Core.Utilities;
using System;
using System.Threading.Tasks;

namespace ScreenRecording.Core.Output
{
    /// <summary>
    /// Adds asynchronous writes support for underlying stream.
    /// </summary>
    internal class AsyncVideoStreamWrapper : VideoStreamWrapperBase
    {
        private readonly SequentialInvoker writeInvoker = new SequentialInvoker();

        public AsyncVideoStreamWrapper(IAviVideoStreamInternal baseStream)
            : base(baseStream)
        {
        }

        public override void WriteFrame(bool isKeyFrame, byte[] frameData, int startIndex, int length)
        {
            Argument.IsNotNull(frameData, nameof(frameData));
            Argument.IsNotNegative(startIndex, nameof(startIndex));
            Argument.IsPositive(length, nameof(length));
            Argument.ConditionIsMet(startIndex + length <= frameData.Length, "End offset exceeds the length of frame data.");

            writeInvoker.Invoke(() => base.WriteFrame(isKeyFrame, frameData, startIndex, length));
        }

        public override Task WriteFrameAsync(bool isKeyFrame, byte[] frameData, int startIndex, int length)
        {
            Argument.IsNotNull(frameData, nameof(frameData));
            Argument.IsNotNegative(startIndex, nameof(startIndex));
            Argument.IsPositive(length, nameof(length));
            Argument.ConditionIsMet(startIndex + length <= frameData.Length, "End offset exceeds the length of frame data.");

            return writeInvoker.InvokeAsync(() => base.WriteFrame(isKeyFrame, frameData, startIndex, length));
        }

#if NET5_0_OR_GREATER
        public unsafe override void WriteFrame(bool isKeyFrame, ReadOnlySpan<byte> frameData)
        {
            Argument.Meets(frameData.Length > 0, nameof(frameData), "Cannot write an empty frame.");

            fixed (void* ptr = frameData)
            {
                var dataPtr = new IntPtr(ptr);
                var dataLength = frameData.Length;
                writeInvoker.Invoke(() =>
                {
                    var dataSpan = new Span<byte>(dataPtr.ToPointer(), dataLength);
                    base.WriteFrame(isKeyFrame, dataSpan);
                });
            }
        }

        public override Task WriteFrameAsync(bool isKeyFrame, ReadOnlyMemory<byte> frameData)
        {
            Argument.Meets(frameData.Length > 0, nameof(frameData), "Cannot write an empty frame.");

            return writeInvoker.InvokeAsync(() => base.WriteFrame(isKeyFrame, frameData.Span));
        }
#endif

        public override void FinishWriting()
        {
            // Perform all pending writes and then let the base stream to finish
            // (possibly writing some more data synchronously)
            writeInvoker.WaitForPendingInvocations();

            base.FinishWriting();
        }
    }
}
