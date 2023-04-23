﻿////////////////////////////////////////////////////////////////////////////
//
// GitReader - Lightweight Git local repository traversal library.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using GitReader.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace GitReader.Structures;

public static class RepositoryExtension
{
    public static Branch? GetHead(
        this StructuredRepository repository) =>
        repository.head;

    public static Task<Commit?> GetCommitAsync(
        this StructuredRepository repository,
        Hash commit, CancellationToken ct = default) =>
        RepositoryFacade.GetCommitDirectlyAsync(repository, commit, ct);

    public static Task<Commit?> GetPrimaryParentCommitAsync(
        this Commit commit,
        CancellationToken ct = default) =>
        RepositoryFacade.GetPrimaryParentAsync(commit, ct);

    public static Task<Commit[]> GetParentCommitsAsync(
        this Commit commit,
        CancellationToken ct = default) =>
        RepositoryFacade.GetParentsAsync(commit, ct);

    public static void Deconstruct(
        this StructuredRepository repository,
        out string path,
        out Branch? head,
        out ReadOnlyDictionary<string, Branch> branches,
        out ReadOnlyDictionary<string, Branch> remoteBranches,
        out ReadOnlyDictionary<string, Tag> tags)
    {
        path = repository.Path;
        head = repository.head;
        branches = repository.branches;
        remoteBranches = repository.remoteBranches;
        tags = repository.tags;
    }

    public static void Deconstruct(
        this Branch branch,
        out string name,
        out Commit head)
    {
        name = branch.Name;
        head = branch.Head;
    }

    public static void Deconstruct(
        this Commit commit,
        out Hash hash,
        out Signature author,
        out Signature committer,
        out string message)
    {
        hash = commit.Hash;
        author = commit.Author;
        committer = commit.Committer;
        message = commit.Message;
    }

    public static void Deconstruct(
        this Tag tag,
        out Hash hash,
        out ObjectTypes type,
        out string name,
        out Signature? tagger,
        out string? message)
    {
        hash = tag.Hash;
        type = tag.Type;
        name = tag.Name;
        tagger = tag.Tagger;
        message = tag.Message;
    }
}
