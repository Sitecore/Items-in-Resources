# Items-in-Resources

## Alpha-1.1

This project aims to extend Sitecore implementation of data provider to make it support storing different 
parts of content tree in separate locations such as SQL Server database and read-only binary files.

### Release Notes

#### 0.0.1.1

* Supports Sitecore 8.2
* Default items are editable (they are saved to sql at first change)
* Default items can be copied, duplicated and moved to different location
* Default items can be spreaded between several binary files
* Querying sql items is supported (shorter than 600 nested items e.g. /sitecore/item1/item2/item3/.../item600, more lead to StackOverflowException)

#### 0.0.1.0

* Supports Sitecore 8.1
* Default items are available read-only (but deletable). 
* Querying sql items by full path is not supported (http://localhost/sitecore/content/home/new%20item.aspx leads to 404)
* Copying and duplicating original items are not supported.
* Only some of data provider caches are used

### Tools

* Visual Studio 2015 due to C# 6 features
* Sitecore Instance Manager 1.4.0.481 (available in [QA branch](http://dl.sitecore.net/updater/qa/sim) only on 21-Oct-16) due to dependency on SC.* nugets

### Deployment

1. Install Sitecore 8.2 Update-0 (e.g. using SIM)
2. Select the installed instance and click `Ribbon -> Home -> Bundled Tools -> Generate NuGets for selected instance`
3. Open solution in Visual Studio 
4. Use Visual Studio deploy to deploy Website to the installed instance 
5. Delete the contents of the `Items`, `SharedFields`, `UnversionedFields`, `VersionedFields` tables of **core**, **master** and **web** databases

### Usage

After deployment Sitecore is ready to use, all the default Sitecore items are in the place. Everything is supported excepting:  
* **partially supported** fast query and sitecore query
