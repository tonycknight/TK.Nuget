# TK.Nuget

[![Build & Release](https://github.com/tonycknight/TK.Nuget/actions/workflows/build.yml/badge.svg)](https://github.com/tonycknight/TK.Nuget/actions/workflows/build.yml)

Abstractions over the Nuget.Protocol package.

## How to use

First, inject the `INugetClient` into IoC: 

```csharp
using Microsoft.Extensions.DependencyInjection;
using Tk.Nuget;

collection.AddNugetClient();
```

Given an `INugetClient` instance, get the latest version of a package:

```csharp
var vsn = client.GetLatestNugetVersionAsync("Newtonsoft.Json");

// Returns a non-null version if successful, null if not.
```

Given an `INugetClient` instance, see if the current version has an upgrade:

```csharp
var vsn = client.GetUpgradeVersionAsync("Newtonsoft.Json", "0.0.1");

// If vsn is non-null, an upgrade version is available. 
// If vsn is null, there is no available version.
```