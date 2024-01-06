# TK.Nuget

[![Build & Release](https://github.com/tonycknight/TK.Nuget/actions/workflows/build.yml/badge.svg)](https://github.com/tonycknight/TK.Nuget/actions/workflows/build.yml)

Abstractions over the Nuget.Protocol package.

## How to use

First, inject the `INugetClient` into IoC: 

```csharp
using Microsoft.Extensions.DependencyInjection;
using Tk.Nuget;


// where collection is of type IServiceCollection

collection.AddNugetClient();

```

To get the latest version of a package:

```csharp
var packageId = "My.Test.Library";
// client is of type Tk.Nuget.INugetClient

var vsn = client.GetLatestNugetVersionAsync(packageId);
```