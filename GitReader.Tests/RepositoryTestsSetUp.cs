﻿////////////////////////////////////////////////////////////////////////////
//
// GitReader - Lightweight Git local repository traversal library.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.IO.Compression;
using VerifyTests;

namespace GitReader;

public sealed class RepositoryTestsSetUp
{
    private sealed class ByteDataConverter :
        WriteOnlyJsonConverter<byte[]>
    {
        public override void Write(VerifyJsonWriter writer, byte[] data) =>
            writer.WriteValue(
                BitConverter.ToString(data).Replace("-", string.Empty).ToLowerInvariant());
    }

    public static readonly string BasePath =
        Path.Combine("tests", $"{DateTime.Now:yyyyMMdd_HHmmss}");

    static RepositoryTestsSetUp()
    {
        VerifierSettings.DontScrubDateTimes();
        VerifierSettings.AddExtraSettings(setting =>
            setting.Converters.Add(new ByteDataConverter()));

        if (!Directory.Exists(BasePath))
        {
            try
            {
                Directory.CreateDirectory(BasePath);
            }
            catch
            {
            }

            ZipFile.ExtractToDirectory("artifacts/test1.zip", BasePath);
        }
    }
}
