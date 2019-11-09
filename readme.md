# Workshell PE Class Library

[![License](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/Workshell/pe/blob/master/license.txt)
[![NuGet](https://img.shields.io/nuget/v/Workshell.PE.svg)](https://www.nuget.org/packages/Workshell.PE/)
[![Build Status](https://dev.azure.com/Workshell-DevOps/pe/_apis/build/status/Build%20Master?branchName=master)](https://dev.azure.com/Workshell-DevOps/pe/_build/latest?definitionId=2&branchName=master)

This is a class library for reading the Portable Executable file format convering all the major data sections including:

* Core Headers (DOS, File, Optional)
* Section Table
* Debug Directory
* Relocations
* Imports
* Delayed Imports
* Exports
* Resources
* TLS
* Load Configuration
* Certificates
* .NET

For help getting started please see the wiki. Any suggestions for improvements and ideas welcome.


## Installation

Stable builds are available as NuGet packages. You can install it via the Package Manager or via the Package Manager Console:

```
Install-Package Workshell.PE
Install-Package Workshell.PE.Resources
```


## Basic Usage

You begin by calling `PortableExecutableImage.FromFileAsync(string)` to load the executable image, such as:

```
var image = await PortableExecutableImage.FromFileAsync("user32.dll");
```

You can then retireve information from the image. Some information is retrieved in a direct manor, like a lot of the core
header information, for example:

```
var imageBase = image.NTHeaders.OptionalHeader.ImageBase;
```

And some information is indirect via classes that work on the image. If you wanted all the exports for example:

```
var exports = await Exports.GetAsync(image);
```

You can then iterate each export:

```
foreach(var export in exports)
{
    Console.WriteLine(export.Name);
}
```

At present there is no real documentation so the best option is to explore the source code and experiment!

If you have questions please do get in touch we're always happy to help.


## MIT License

Copyright (c) Workshell Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.