iiUniverseAtPeace
=====

iiUniverseAtPeace is a C# library supporting the modification of files relating to Universe at War: Earth Assault, the 2007 RPG game developed by Petroglyph Games.
The library supports:

| Name                         | Read | Write | Comment |
|------------------------------|:----:|-------|:--------|
| DAT                          | ✗   |   ✗   |
| MEG                          | ✔   |   ✗   | 


## Usage

Instantiate the relevant class and call the `Process` method passing the filename.

```csharp
var megProcessor = new MegProcessor();
var files = megProcessor.Process("D:\\SteamLibrary\\steamapps\\common\\Universe at War Earth Assault\\Data\\Patch.meg");

foreach (var (filename, bytes) in files)
{
    var justFileName = Path.GetFileName(filename);
    var outputPath = Path.Combine(@"D:\data\Universe At War Assault", justFileName);
    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
    await File.WriteAllBytesAsync(outputPath, bytes);
}
```


## Compiling

To clone and run this application, you'll need [Git](https://git-scm.com) and [.NET](https://dotnet.microsoft.com/) installed on your computer. From your command line:

```
# Clone this repository
$ git clone https://github.com/btigi/iiUniverseAtPeace

# Go into the repository
$ cd src

# Build  the app
$ dotnet build
```


## Licencing

iiUniverseAtPeace is licenced under the MIT License. Full licence details are available in licence.md