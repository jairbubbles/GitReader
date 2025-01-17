# GitReader

Lightweight Git local repository traversal library.

![GitReader](Images/GitReader.100.png)

# Status

[![Project Status: WIP – Initial development is in progress, but there has not yet been a stable, usable release suitable for the public.](https://www.repostatus.org/badges/latest/wip.svg)](https://www.repostatus.org/#wip)

|Target|Pakcage|
|:----|:----|
|Any|[![NuGet GitReader](https://img.shields.io/nuget/v/GitReader.svg?style=flat)](https://www.nuget.org/packages/GitReader)|
|F# binding|[![NuGet FSharp.GitReader](https://img.shields.io/nuget/v/FSharp.GitReader.svg?style=flat)](https://www.nuget.org/packages/FSharp.GitReader)|

----

[![Japanese language](Images/Japanese.256.png)](https://github.com/kekyo/GitReader/blob/main/README_ja.md)

## What is this?

Have you ever wanted to access information about your local Git repository in .NET?
Explore and tag branches, get commit dates and contributor information, and read commit directory structures and files.

GitReader is written only managed code Git local repository traversal library for a wide range of .NET environments.
It is lightweight, has a concise, easy-to-use interface, does not depend any other libraries, and does not contain native libraries,
making it suitable for any environment.

Example:

```csharp
using GitReader;
using GitReader.Structures;

using var repository =
    await Repository.Factory.OpenStructureAsync(
        "/home/kekyo/Projects/YourOwnLocalGitRepo");

if (repository.GetCurrentHead() is { } head)
{
    Console.WriteLine($"Name: {head.Name}");

    Console.WriteLine($"Hash: {head.Head.Hash}");
    Console.WriteLine($"Author: {head.Head.Author}");
    Console.WriteLine($"Committer: {head.Head.Committer}");
    Console.WriteLine($"Subject: {head.Head.Subject}");
    Console.WriteLine($"Body: {head.Head.Body}");
}
```

It has the following features:

* It provides information on Git branches, tags, and commits.
* Branch tree traversal.
* Read only interface makes immutability.
* Both high-level and primitive interfaces ready.
* Fully asynchronous operation.
* Only contains 100% managed code. Independent of any external libraries other than the BCL and its compliant libraries.
* Reliable zlib decompression using the .NET standard deflate implementation.

This library was designed from the ground up to replace `libgit2sharp`, on which [RelaxVersioner](https://github.com/kekyo/CenterCLR.RelaxVersioner) depended.
It primarily fits the purpose of easily extracting commit information from a Git repository.

### Target .NET platforms

* .NET 7.0 to 5.0
* .NET Core 3.1 to 2.0
* .NET Standard 2.1 to 1.6
* .NET Framework 4.8.1 to 3.5

### F# specialized binding

F# 5.0 or upper, it contains F# friendly signature definition.

* .NET 7.0 to 5.0
* .NET Core 3.1 to 2.0
* .NET Standard 2.1, 2.0
* .NET Framework 4.8.1 to 4.6.1

Note: All target framework variations are tested only newest it.

----

## How to use

Install [GitReader](https://www.nuget.org/packages/GitReader) from NuGet.

* Install [FSharp.GitReader](https://www.nuget.org/packages/FSharp.GitReader) when you need to use with F#.
  It has F# friendly signature definition.
  You can freely use the same version of a package to switch back and forth between C# and F#,
  as long as the runtime instances are compatible.

GitReader has high-level interfaces and primitive interfaces.

* The high-level interface is an interface that abstracts the Git repository.
  Easy to handle without knowing the internal structure of Git.
  It is possible to retrieve branch, tag, and commit information, and to read files (Blobs) at a glance.
* The primitive interface is an interface that exposes the internal structure of the Git repository as it is,
  It is simple to handle if you know the internal structure of Git,
  and it offers high performance in asynchronous processing.

### Sample Code

Comprehensive sample code can be found in the [samples directory](/samples).
The following things are minimal code fragments.

----

## Samples (High-level interfaces)

The high-level interface is easily referenced by automatically reading much of the information tied to a commit.

### Get current head commit

```csharp
using GitReader;
using GitReader.Structures;

using StructuredRepository repository =
    await Repository.Factory.OpenStructureAsync(
        "/home/kekyo/Projects/YourOwnLocalGitRepo");

// Found current head
if (repository.GetCurrentHead() is Branch head)
{
    Console.WriteLine($"Name: {head.Name}");

    Console.WriteLine($"Hash: {head.Head.Hash}");
    Console.WriteLine($"Author: {head.Head.Author}");
    Console.WriteLine($"Committer: {head.Head.Committer}");
    Console.WriteLine($"Subject: {head.Head.Subject}");
    Console.WriteLine($"Body: {head.Head.Body}");
}
```

### Get a commit directly

```csharp
if (await repository.GetCommitAsync(
    "1205dc34ce48bda28fc543daaf9525a9bb6e6d10") is Commit commit)
{
    Console.WriteLine($"Hash: {commit.Hash}");
    Console.WriteLine($"Author: {commit.Author}");
    Console.WriteLine($"Committer: {commit.Committer}");
    Console.WriteLine($"Subject: {commit.Subject}");
    Console.WriteLine($"Body: {commit.Body}");
}
```

### Get a branch head commit

```csharp
Branch branch = repository.Branches["develop"];

Console.WriteLine($"Name: {branch.Name}");

Console.WriteLine($"Hash: {branch.Head.Hash}");
Console.WriteLine($"Author: {branch.Head.Author}");
Console.WriteLine($"Committer: {branch.Head.Committer}");
Console.WriteLine($"Subject: {branch.Head.Subject}");
Console.WriteLine($"Body: {branch.Head.Body}");
```

### Get a remote branch head commit

```csharp
Branch branch = repository.RemoteBranches["origin/develop"];

Console.WriteLine($"Name: {branch.Name}");

Console.WriteLine($"Hash: {branch.Head.Hash}");
Console.WriteLine($"Author: {branch.Head.Author}");
Console.WriteLine($"Committer: {branch.Head.Committer}");
Console.WriteLine($"Subject: {branch.Head.Subject}");
Console.WriteLine($"Body: {branch.Head.Body}");
```

### Get a tag

```csharp
Tag tag = repository.Tags["1.2.3"];

Console.WriteLine($"Name: {tag.Name}");

Console.WriteLine($"Hash: {tag.Hash}");
Console.WriteLine($"Author: {tag.Author}");
Console.WriteLine($"Committer: {tag.Committer}");
Console.WriteLine($"Message: {tag.Message}");
```

### Get related branches and tags from a commit

```csharp
if (await repository.GetCommitAsync(
    "1205dc34ce48bda28fc543daaf9525a9bb6e6d10") is Commit commit)
{
    Branch[] branches = commit.Branches;
    Branch[] remoteBranches = commit.RemoteBranches;
    Tags[] tags = commit.Tags;

    // ...
}
```

### Enumerate branches

```csharp
foreach (Branch branch in repository.Branches.Values)
{
    Console.WriteLine($"Name: {branch.Name}");

    Console.WriteLine($"Hash: {branch.Head.Hash}");
    Console.WriteLine($"Author: {branch.Head.Author}");
    Console.WriteLine($"Committer: {branch.Head.Committer}");
    Console.WriteLine($"Subject: {branch.Head.Subject}");
    Console.WriteLine($"Body: {branch.Head.Body}");
}
```

### Enumerate tags

```csharp
foreach (Tag tag in repository.Tags.Values)
{
    Console.WriteLine($"Name: {tag.Name}");

    Console.WriteLine($"Hash: {tag.Hash}");
    Console.WriteLine($"Author: {tag.Author}");
    Console.WriteLine($"Committer: {tag.Committer}");
    Console.WriteLine($"Message: {tag.Message}");
}
```

### Get parent commits

```csharp
if (await repository.GetCommitAsync(
    "6961a50ef3ad4e43ed9774daffd8457d32cf5e75") is Command commit)
{
    Commit[] parents = await commit.GetParentCommitsAsync();

    foreach (Commit parent in parents)
    {
        Console.WriteLine($"Hash: {parent.Hash}");
        Console.WriteLine($"Author: {parent.Author}");
        Console.WriteLine($"Committer: {parent.Committer}");
        Console.WriteLine($"Subject: {parent.Subject}");
        Console.WriteLine($"Body: {parent.Body}");
    }
}
```

### Get commit tree information

Tree information is the tree structure of directories and files that are placed when a commit is checked out.
The code shown here does not actually 'check out', but reads these structures as information.

```csharp
if (await repository.GetCommitAsync(
    "6961a50ef3ad4e43ed9774daffd8457d32cf5e75") is Command commit)
{
    TreeRoot treeRoot = await commit.GetTreeRootAsync();

    foreach (TreeEntry entry in treeRoot.Children)
    {
        Console.WriteLine($"Hash: {entry.Hash}");
        Console.WriteLine($"Name: {entry.Name}");
        Console.WriteLine($"Modes: {entry.Modes}");
    }
}
```

### Read blob by stream

```csharp
if (await repository.GetCommitAsync(
    "6961a50ef3ad4e43ed9774daffd8457d32cf5e75") is Command commit)
{
    TreeRoot treeRoot = await commit.GetTreeRootAsync();

    foreach (TreeEntry entry in treeRoot.Children)
    {
        // For Blob, the instance type is `TreeBlobEntry`.
        if (entry is TreeBlobEntry blob)
        {
            using Stream stream = await blob.OpenBlobAsync();

            // (You can access the blob...)
        }
    }
}
```

### Traverse a branch through primary commits

A commit in Git can have multiple parent commits.
This occurs with merge commits, where there are links to all parent commits.
The first parent commit is called the "primary commit"
and is always present except for the first commit in the repository.

Use `GetParentCommitsAsync()` to get links to all parent commits.

As a general thing about Git,
it is important to note that the parent-child relationship of commits (caused by branching and merging),
always expressed as one direction, from "child" to "parent".

This is also true for the high-level interface; there is no interface for referencing a child from its parent.
Therefore, if you wish to perform such a search, you must construct the link in the reverse direction on your own.

The following example recursively searches for a parent commit from a child commit.

```csharp
Branch branch = repository.Branches["develop"];

Console.WriteLine($"Name: {branch.Name}");

Commit? current = branch.Head;

// Continue as long as the parent commit exists.
while (current != null)
{
    Console.WriteLine($"Hash: {current.Hash}");
    Console.WriteLine($"Author: {current.Author}");
    Console.WriteLine($"Committer: {current.Committer}");
    Console.WriteLine($"Subject: {current.Subject}");
    Console.WriteLine($"Body: {current.Body}");

    // Get primary parent commit.
    current = await current.GetPrimaryParentCommitAsync();
}
```

----

## Samples (Primitive interfaces)

The high-level interface is implemented internally using these primitive interfaces.
We do not have a complete list of all examples, so we recommend referring to the GitReader code if you need information.

* You may want to start with [RepositoryFacade class](/GitReader.Core/Structures/RepositoryFacade.cs).

### Read current head commit

```csharp
using GitReader;
using GitReader.Primitive;

using PrimitiveRepository repository =
    await Repository.Factory.OpenPrimitiveAsync(
        "/home/kekyo/Projects/YourOwnLocalGitRepo");

if (await repository.GetCurrentHeadReferenceAsync() is PrimitiveReference head)
{
    if (await repository.GetCommitAsync(head) is PrimitiveCommit commit)
    {
        Console.WriteLine($"Hash: {commit.Hash}");
        Console.WriteLine($"Author: {commit.Author}");
        Console.WriteLine($"Committer: {commit.Committer}");
        Console.WriteLine($"Message: {commit.Message}");
    }
}
```

### Read a commit directly

```csharp
if (await repository.GetCommitAsync(
    "1205dc34ce48bda28fc543daaf9525a9bb6e6d10") is PrimitiveCommit commit)
{
    Console.WriteLine($"Hash: {commit.Hash}");
    Console.WriteLine($"Author: {commit.Author}");
    Console.WriteLine($"Committer: {commit.Committer}");
    Console.WriteLine($"Message: {commit.Message}");
}
```

### Read a branch head commit

```csharp
PrimitiveReference head = await repository.GetBranchHeadReferenceAsync("develop");

if (await repository.GetCommitAsync(head) is PrimitiveCommit commit)
{
    Console.WriteLine($"Hash: {commit.Hash}");
    Console.WriteLine($"Author: {commit.Author}");
    Console.WriteLine($"Committer: {commit.Committer}");
    Console.WriteLine($"Message: {commit.Message}");
}
```

### Enumerate branches

```csharp
PrimitiveReference[] branches = await repository.GetBranchHeadReferencesAsync();

foreach (PrimitiveReference branch in branches)
{
    Console.WriteLine($"Name: {branch.Name}");
    Console.WriteLine($"Commit: {branch.Commit}");
}
```

### Enumerate tags

```csharp
PrimitiveReference[] tagReferences = await repository.GetTagReferencesAsync();

foreach (PrimitiveReference tagReference in tagReferences)
{
    PrimitiveTag tag = await repository.GetTagAsync(tagReference);

    Console.WriteLine($"Hash: {tag.Hash}");
    Console.WriteLine($"Type: {tag.Type}");
    Console.WriteLine($"Name: {tag.Name}");
    Console.WriteLine($"Tagger: {tag.Tagger}");
    Console.WriteLine($"Message: {tag.Message}");
}
```

### Read commit tree information

```csharp
if (await repository.GetCommitAsync(
    "1205dc34ce48bda28fc543daaf9525a9bb6e6d10") is PrimitiveCommit commit)
{
    PrimitiveTree tree = await repository.GetTreeAsync(commit.TreeRoot);

    foreach (Hash childHash in tree.Children)
    {
        PrimitiveTreeEntry child = await repository.GetTreeAsync(childHash);

        Console.WriteLine($"Hash: {child.Hash}");
        Console.WriteLine($"Name: {child.Name}");
        Console.WriteLine($"Modes: {child.Modes}");
    }
}
```

### Read blob by stream

```csharp
if (await repository.GetCommitAsync(
    "1205dc34ce48bda28fc543daaf9525a9bb6e6d10") is PrimitiveCommit commit)
{
    PrimitiveTree tree = await repository.GetTreeAsync(commit.TreeRoot);

    foreach (Hash childHash in tree.Children)
    {
        PrimitiveTreeEntry child = await repository.GetTreeAsync(childHash);
        if (child.Modes.HasFlag(PrimitiveModeFlags.File))
        {
            using Stream stream = await repository.OpenBlobAsync(child.Hash);

            // (You can access the blob...)
        }
    }
}
```

### Traverse a commit through primary commits

```csharp
if (await repository.GetCommitAsync(
    "1205dc34ce48bda28fc543daaf9525a9bb6e6d10") is PrimitiveCommit commit)
{
    while (true)
    {
        Console.WriteLine($"Hash: {commit.Hash}");
        Console.WriteLine($"Author: {commit.Author}");
        Console.WriteLine($"Committer: {commit.Committer}");
        Console.WriteLine($"Message: {commit.Message}");

        // Bottom of branch.
        if (commit.Parents.Length == 0)
        {
            break;
        }

        // Get primary parent.
        Hash primary = commit.Parents[0];
        if (await repository.GetCommitAsync(primary) is not PrimitiveCommit parent)
        {
            throw new Exception();
        }

        current = parent;
    }
}
```

----

## Samples (Others)

### SHA1 hash operations

```csharp
Hash hashFromString = "1205dc34ce48bda28fc543daaf9525a9bb6e6d10";
Hash hashFromArray = new byte[] { 0x12, 0x05, 0xdc, ... };

var hashFromStringConstructor =
    new Hash("1205dc34ce48bda28fc543daaf9525a9bb6e6d10");
var hashFromArrayConstructor =
    new Hash(new byte[] { 0x12, 0x05, 0xdc, ... });

if (Hash.TryParse("1205dc34ce48bda28fc543daaf9525a9bb6e6d10", out Hash hash))
{
    // ...
}

Commit commit = ...;
Hash targetHash = commit;
```

### Enumerate remote urls

```csharp
foreach (KeyValuePair<string, string> entry in repository.RemoteUrls)
{
    Console.WriteLine($"Remote: Name={entry.Key}, Url={entry.Value}");
}
```

----

## TODO

* Read stashing information.
* Read submodule information.
* Makes configurable minor execution parameters.

----

## License

Apache-v2

## History

* 0.9.0:
  * Exposed remote urls.
  * Changed some type names avoid confliction.
* 0.8.0:
  * Added tree/blob accessors.
  * Improved performance.
* 0.7.0:
  * Switched primitive interface types with prefix `Primitive`.
  * Improved performance.
  * Tested large repositories.
* 0.6.0:
  * Improved message handling on high-level interfaces.
  * Re-implemented delta compression decoder.
  * Supported both FETCH_HEAD and packed_refs parser.
  * Improved performance.
  * Removed index locker.
  * Fixed contains invalid hash on annotated commit tag.
  * Improved minor interface features.
* 0.5.0:
  * Supported deconstructor by F# active patterns.
  * Downgraded at least F# version 5.
* 0.4.0:
  * Added F# binding.
  * Fixed lack for head branch name.
* 0.3.0:
  * Supported ability for not found detection.
* 0.2.0:
  * The shape of the public interfaces are almost fixed.
  * Improved high-level interfaces.
  * Splitted core library (Preparation for F# binding)
* 0.1.0:
  * Initial release.
