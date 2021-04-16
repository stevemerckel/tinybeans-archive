# tinybeans-archive

## Purpose

The purpose of this project is to automate the backup creation of journals created on Tinybeans.  This is to provide a simple and agnostic backup to a file system with all useful metadata.

## Technologies Used

The following list summarizes the tech stack:

* .NET Core (v3.1)
* JSON (via _Newtonsoft JSON_)
* Inversion of Control (via _Ninject_)
* Unit Testing (via _NUnit_)

## Solution Overview and Debugging

The primary console application runs from `TBA.ConsoleApp`.  There is an optional testbed project called `TBA.Sandbox` for doing ad-hoc testing of functionality.  Either of these can be run from Visual Studio.

The bulk of business logic is in `TBA.Common`.  A good starting point would be the looking at the methods and functions in `JournalManager.cs`.

Tests are contained in `TBA.Tests` project.

## Downloaded Journal Entries

The general directory structure is indicated below:

```layout
[Root Directory] / [Journal ID] / [yyyy] / [MM] / [dd]
```

Tinybeans supports multiple journals under a single account, hence the `[Journal ID]` prefix to downloaded content.

Given the above, a journal entry on January 17, 2019, would cause a directory to be written like this:

`[Root Directory]` \ `[12345]` \ `[2019]` \ `[01]` \ `[17]` \ `[*.jpg|mp4|txt]`

Notes on the above:

* The _Root Directory_ above will need to be known at runtime.
  * When debugging from Visual Studio, it will default to `Environment.CurrentDirectory`.
  * [TODO: Behavior when running without attachment to VS.]
* The application will need write/modify permissions to the above path
* A `[dd]` directory
* Each _YYYY-MM-dd_ directory will have a JSON file called `manifest.[yyyy-MM-dd].json`, which contains metadata about the content in that day.

## Future Plans

[TODO]

## Additonal Backstory

Inspiration for this idea was from my wife, who noted that our images/videos were backed up regularly to the cloud, but the "curated journals" we made in Tinybeans were not.

I initially sent an email to Tinybeans support, asking whether they had documentation on their API.  They responded that they currently do not have the API documented for public use, and have no plans to implement this.  After some Google-ing, I found someone else had made a similar idea [built in Clojure](https://github.com/oliyh/tinybeans-archive/tree/master/src/tinybeans_archive).  Seeing the source code did provide some guidance on a few of the API calls I would need to make, so kudos to [Oliver](https://github.com/oliyh) for publicly providing some useful information!
