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
using System.Threading;
using System.Threading.Tasks;

namespace GitReader.Internal;

internal sealed class AsyncLock
{
    private sealed class QueueEntry
    {
        private readonly TaskCompletionSource<Disposer> tcs = new();
        private readonly CancellationTokenRegistration ctr;

        public QueueEntry(CancellationToken ct) =>
            this.ctr = ct.Register(() => this.tcs.TrySetCanceled());

        public bool SetResult(Disposer disposer)
        {
            var result = this.tcs.TrySetResult(disposer);
            this.ctr.Dispose();
            return result;
        }

        public Disposer Wait() =>
            this.tcs.Task.GetAwaiter().GetResult();

        public Task<Disposer> WaitAsync() =>
            this.tcs.Task;
    }

    private static readonly ThreadLocal<int> counter = new();

    private readonly Disposer disposer;
    private readonly Queue<QueueEntry> queue = new();
    private int running;

    public AsyncLock() =>
        this.disposer = new(this);

    public Disposer Lock()
    {
        lock (this.queue)
        {
            var running = this.running++;
            if (running <= 0)
            {
                return this.disposer;
            }
            else
            {
                var queueEntry = new QueueEntry(default);
                this.queue.Enqueue(queueEntry);
                return queueEntry.Wait();
            }
        }
    }

    public Task<Disposer> LockAsync(CancellationToken ct)
    {
        lock (this.queue)
        {
            var running = this.running++;
            if (running <= 0)
            {
                return Utilities.FromResult(this.disposer);
            }
            else
            {
                var queueEntry = new QueueEntry(ct);
                this.queue.Enqueue(queueEntry);
                return queueEntry.WaitAsync();
            }
        }
    }

    private void InternalUnlock()
    {
        while (true)
        {
            if (this.running >= 1)
            {
                this.running--;
            }

            if (this.queue.Count == 0)
            {
                break;
            }

            var queueEntry = this.queue.Dequeue();
            if (queueEntry.SetResult(this.disposer))
            {
                break;
            }
        }
    }

    private void Unlock()
    {
#if true
        ThreadPool.QueueUserWorkItem(_ =>
        {
            lock (this.queue)
            {
                this.InternalUnlock();
            }
        });
#else
        try
        {
            // Will cause stack overflow when a lot of continuation queued and ran it series.
            // So it is splitted reentrancy continuation execution each 50 times.
            if (counter.Value++ >= 50)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    this.Unlock();
                });
            }
            else
            {
                lock (this.queue)
                {
                    this.InternalUnlock();
                }
            }
        }
        finally
        {
            counter.Value--;
        }
#endif
    }

    public sealed class Disposer : IDisposable
    {
        private readonly AsyncLock parent;

        internal Disposer(AsyncLock parent) =>
            this.parent = parent;

        public void Dispose() =>
            this.parent.Unlock();
    }
}
