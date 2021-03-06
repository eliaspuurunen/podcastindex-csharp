# Agfinn.PodcastIndex.Lib

This is a simple wrapper around the [Podcast Index](https://podcastindex-org.github.io/docs-api/) API. Targets .NET Standard 2.0.

Supports both read-only operations & read-write. Read-Write requires a RW key from Podcast Index.

## Nuget Package

[Nuget Package](https://www.nuget.org/packages/podcastindex-csharp)

## Usage

```csharp

var apiKey = "YOUR API KEY";
var apiSecret = "YOUR SECRET KEY HERE";
var userAgent = "Elias @agfinn Test Harness 1.0";

var lib = new PodcastIndex.Lib.PodcastIndexInstance(apiKey, apiSecret, userAgent);

var result = await lib.SearchByTermAsync("batman", "university");

Console.WriteLine("---SEARCH BY TERM---");
foreach (var item in result)
{
	Console.WriteLine($"{item.title} - {item.ownerName} - {item.url}");
}

```
