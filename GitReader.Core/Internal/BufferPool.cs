﻿////////////////////////////////////////////////////////////////////////////
//
// GitReader - Lightweight Git local repository traversal library.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace GitReader.Internal;

internal struct Buffer : IDisposable
{
    private byte[] buffer;

    internal Buffer(byte[] buffer) =>
        this.buffer = buffer;

    public void Dispose() =>
        BufferPool.Release(ref this.buffer);

    public int Length =>
        this.buffer.Length;

    public byte this[int index]
    {
        get => this.buffer[index];
        set => this.buffer[index] = value;
    }

    public static implicit operator Buffer(byte[] buffer) =>
        new(buffer);
    public static implicit operator byte[](Buffer buffer) =>
        buffer.buffer;
}

internal static class BufferPool
{
    private static readonly Dictionary<uint, List<WeakReference>> buffers = new();

    public static Buffer Take(uint size)
    {
        List<WeakReference> bufferStack;

        lock (buffers)
        {
            if (!buffers.TryGetValue(size, out bufferStack!))
            {
                bufferStack = new();
                buffers.Add(size, bufferStack);
            }
        }

        lock (bufferStack)
        {
            for (var index = 0; index < bufferStack.Count; index++)
            {
                var wr = bufferStack[index];
                if (wr.Target is byte[] buffer)
                {
                    wr.Target = null;
                    return new(buffer);
                }
            }

            return new(new byte[size]);
        }
    }

    public static Buffer Take(int size) =>
        Take((uint)size);

    internal static void Release(ref byte[] buffer)
    {
        if (buffer == null)
        {
            return;
        }

        var size = buffer.Length;

        List<WeakReference> bufferStack;

        lock (buffers)
        {
            if (!buffers.TryGetValue((uint)size, out bufferStack!))
            {
                bufferStack = new();
                buffers.Add((uint)size, bufferStack);
            }
        }

        lock (bufferStack)
        {
            WeakReference wr;

            for (var index = 0; index < bufferStack.Count; index++)
            {
                wr = bufferStack[index];
                if (wr.Target == null)
                {
                    wr.Target = buffer;
                    buffer = null!;
                    return;
                }
            }

            wr = new WeakReference(buffer);
            bufferStack.Add(wr);
        }

        buffer = null!;
    }
}
