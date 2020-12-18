# tinybeans-archive

## Purpose

The purpose of this project is to automate the backup creation of journals created on Tinybeans.  This is to provide a simple and agnostic backup to a file system with all useful metadata.

## Technologies Used

The following list summarizes the tech stack:

* .NET Core (v3.1)
* .NET Standard (v2.0)
* JSON (via Newtonsoft JSON.Net)
* Inversion of Control (Ninject)
* Unit Testing (NUnit)

## Running and Debugging

[TODO]

## Downloading Journal Entries

The general directory structure is indicated below:

```layout
[Root Directory] / [Journal ID] / [YYYY-MM] / [YYYY-MM-dd]
```

Notes on the above:

* The application will need write/modify permissions to the _Root Directory_ path
* A directory of _YYYY-MM-dd_ will be made as long as at least one journal entry exists for that day
* Each _YYYY-MM-dd_ directory will have a JSON file called _YYYY-MM-dd.json_, which contains metadata for the day (e.g. the display order for the journal entries on that day).

Within each _YYYY-MM-dd_ directory, there will be any number of files.  As long as there is a journal item (i.e. image, video, text), you will find the day's JSON metadata file mentioned above.  Each journal artifact will also have its own metadata file.  For example, if image file _AB12CD.jpg_ was found, you would find _AB12CD.jpg.json_ file containing the metadata for it.

## Future Plans

[TODO]

## Additonal Backstory

Inspiration for this idea was from my wife, who noted that our images/videos were backed up regularly to the cloud, but the "curated journals" we made in Tinybeans were not.

I initially sent a DM to Tinybean's Twitter handle to ask if they had documentation on their API, Swagger docs, etc.  However, they responded that they currently do not have plans to implement this.  After some Google-ing, I found someone else had a similar idea [built in Clojure](https://github.com/oliyh/tinybeans-archive/tree/master/src/tinybeans_archive).  Seeing the source code did provide some guidance on a few of the API calls I would need to make, so thanks to [NAME] for having that repo public!
