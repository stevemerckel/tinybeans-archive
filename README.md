# tinybeans-archive

## Purpose

The purpose of this project is to automate the backup creation of journals created on Tinybeans.  This is to provide a simple and agnostic backup to a file system with all useful metadata.

## Backstory

Inspiration for this idea was from my wife, who noted that our images/videos were backed up regularly to the cloud, but the "curated journals" we made in Tinybeans were not.

I initially sent an email to Tinybeans support, asking whether they had documentation on their API.  They responded that they currently do not have the API documented for public use, and have no plans to implement this.

After some Google-ing, I found someone else had made a similar idea [built in Clojure](https://github.com/oliyh/tinybeans-archive/tree/master/src/tinybeans_archive).  Seeing the source code did provide some guidance on a few of the API calls I would need to make, so kudos to [Oliver](https://github.com/oliyh) for publicly providing some useful information!

## Technologies Used

The following list summarizes the tech stack:

* .NET (v5)
* JSON (via [NewtonSoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/))
* Inversion of Control (via [Ninject](https://www.nuget.org/packages/ninject))
* Unit Testing (via [NUnit](https://www.nuget.org/packages/nunit))

## Solution Overview and Debugging

The primary console application runs from `TBA.ConsoleApp`.

There is an optional testbed project called `TBA.Sandbox` for doing ad-hoc testing of functionality.

The bulk of business logic is in `TBA.Common`.  A good starting point would be the looking at the methods and functions in `JournalManager.cs`.

Tests are contained in `TBA.Tests` project.

A web API for fetching moment-data is in `TBA.Api`.  Note that this project uses the _InternalsVisibleTo_ attribute to allow internal objects to be testable.

## Running for the First Time

In the `$/assets` directory is a file called `runtime.settings.TEMPLATE`.  Make a copy of this file, and name it `runtime.settings`.  This will also resolve any build issues.

The next step is to modify the values.  At minimum, you will need to add your own value the JSON file by replacing the string `INSERT-YOUR-KEY-HERE`.  This VALUE can be found by using web browser inspection tools, or a HTTP/S packet sniffer like _Fiddler_.  Connect to tinybeans.com, login, and browse around the main page.  Then look at the traffic in the "request headers" section, and grab the value for the `authorization` header.

## Downloaded Journal Entries

The general directory structure is indicated below:

```layout
[Root Directory] / [Journal ID] / [yyyy] / [MM] / [dd]
```

Tinybeans supports multiple journals under a single account, hence the `[Journal ID]` prefix to downloaded content.

Given the above, a journal entry on January 17, 2019, would cause a directory to be written like this:

`[Root Directory]` / `[12345]` / `[2019]` / `[01]` / `[17]` / `[*.jpg|mp4|txt]`

Notes on the above:

* The _Root Directory_ above will need to be known at runtime.
  * When debugging from Visual Studio, it will default to [Environment.CurrentDirectory](https://docs.microsoft.com/en-us/dotnet/api/system.environment.currentdirectory?view=netcore-3.1).
  * [TODO: Behavior when running without attachment to VS.]
* The application will need full permissions to the _Root Directory_.
* A `[dd]` directory will be created only when at least one entry exists for the target date.  Same concept applies to the `[MM]` directory.
* Each _yyyy-MM-dd_ directory will have a JSON file called `manifest.[yyyy-MM-dd].json`, which contains metadata about the content in that day.

## Future Plans

Some general ideas:

* Add a _reconsider_ logic for detecting changes at the remote Tinybeans API that had not yet been propagated to the local.
* Build a web frontend and backing API for creating a "readonly" view of the data downloaded locally.
