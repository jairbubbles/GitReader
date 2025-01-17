﻿////////////////////////////////////////////////////////////////////////////
//
// GitReader - Lightweight Git local repository traversal library.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using GitReader.Collections;

namespace GitReader.Structures;

public sealed class StructuredRepository : Repository
{
    internal Branch? head;
    internal ReadOnlyDictionary<string, Branch> branches = null!;
    internal ReadOnlyDictionary<string, Branch> remoteBranches = null!;
    internal ReadOnlyDictionary<string, Tag> tags = null!;

    internal StructuredRepository(
        string repositoryPath) :
        base(repositoryPath)
    {
    }

    public ReadOnlyDictionary<string, Branch> Branches =>
        this.branches;
    public ReadOnlyDictionary<string, Branch> RemoteBranches =>
        this.remoteBranches;
    public ReadOnlyDictionary<string, Tag> Tags =>
        this.tags;
}
