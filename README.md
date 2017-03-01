# Items-in-Resources

This project aims to extend Sitecore implementation of data provider to make it support storing different 
parts of content tree in separate locations such as SQL Server database and read-only binary files.

### Release Notes

Find release notes in [releases section](https://github.com/Sitecore/Items-in-Resources/releases).

### Tools

* Visual Studio 2015 due to C# 6 features
* Sitecore Instance Manager 1.4.0.481 (available in [QA branch](http://dl.sitecore.net/updater/qa/sim) only on 21-Oct-16) due to dependency on SC.* nugets

### Deployment

1. Install Sitecore 8.2 Update-2 (e.g. using SIM)
2. Select the installed instance and click `Ribbon -> Home -> Bundled Tools -> Generate NuGets for selected instance`
3. Open solution in Visual Studio 
4. Use Visual Studio deploy to deploy Website to the installed instance 
5. Delete the contents of the `Items`, `SharedFields`, `UnversionedFields`, `VersionedFields` tables of **core** and **master** databases

### Usage

After deployment Sitecore is ready to use, all the default Sitecore items are in the place. Everything is supported excepting:  
* **partially supported** fast query and sitecore query

Check known issues in releases section.
