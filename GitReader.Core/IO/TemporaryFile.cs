////////////////////////////////////////////////////////////////////////////
//
// GitReader - Lightweight Git local repository traversal library.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;

/* Unmerged change from project 'GitReader.Core (netstandard1.6)'
Before:
using System.Runtime.InteropServices;

#if !NETSTANDARD1_6
After:
using System.Runtime.InteropServices;
using GitReader;
using GitReader.Internal;
using GitReader.IO;

#if !NETSTANDARD1_6
*/

/* Unmerged change from project 'GitReader.Core (net35)'
Before:
using System.Runtime.InteropServices;

#if !NETSTANDARD1_6
After:
using System.Runtime.InteropServices;
using GitReader;
using GitReader.Internal;
using GitReader.IO;

#if !NETSTANDARD1_6
*/
using System.Runtime.InteropServices;
using GitReader.Internal;

#if !NETSTANDARD1_6
using System.Runtime.ConstrainedExecution;
#endif

namespace GitReader.IO;

internal sealed class TemporaryFile :
#if !NETSTANDARD1_6
    CriticalFinalizerObject,
#endif
    IDisposable
{
    private GCHandle pathHandle;
    private GCHandle streamHandle;

    private TemporaryFile(
        string path, Stream stream)
    {
        pathHandle = GCHandle.Alloc(path, GCHandleType.Normal);
        streamHandle = GCHandle.Alloc(stream, GCHandleType.Normal);

        Debug.WriteLine($"GitReader: TemporaryFile: Created: {path}");
    }

    ~TemporaryFile() =>
        Dispose();

    public void Dispose()
    {
        if (streamHandle.IsAllocated &&
            streamHandle.Target is Stream stream)
        {
            streamHandle.Free();
            stream.Dispose();
        }

        if (pathHandle.IsAllocated &&
            pathHandle.Target is string path)
        {
            pathHandle.Free();
            try
            {
                File.Delete(path);
            }
            catch
            {
            }
            Debug.WriteLine($"GitReader: TemporaryFile: Deleted: {path}");
        }

        GC.SuppressFinalize(this);
    }

    public Stream Stream =>
        (Stream)streamHandle.Target!;

    public string Path =>
        (string)pathHandle.Target!;

    public static TemporaryFile CreateFile()
    {
        var path = Utilities.Combine(
            System.IO.Path.GetTempPath(),
            System.IO.Path.GetTempFileName());

        var stream = new FileStream(
            path,
            FileMode.Create,
            FileAccess.ReadWrite,
            FileShare.None);

        return new TemporaryFile(path, stream);
    }
}
