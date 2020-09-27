This project is in **EARLY DEVELOPMENT** and the goal of the project is far from being realized!

# Db-Language
![](https://github.com/bassclefstudio/Db-Language/workflows/.NET%20Core%20Build,%20Test,%20and%20Pack/badge.svg)
![](https://github.com/bassclefstudio/Db-Language/workflows/.NET%20Core%20Build%20and%20Test/badge.svg)

BassClefStudio.DbLanguage is an interpreted programming language that runs on top of the .NET platform. It provides cross-platform implementations of device-specific features (such as the file system, cameras, Bluetooth, etc.) through the use of an advanced Capabilities framework, as well as unified deployment and cross-app communication across platforms.

## BassClefStudio.DbLanguage.Core
The DbLanguage.Core project contains the code for the base system that interprets and executes Db code. This includes documentation, types, memory, methods, exceptions, etc.

## BassClefStudio.DbLanguage.Compiler
The DbLanguage.Compiler project contains a parser and a builder to take string code from files or input, parse it into tokens, and then build a Db library from the result. This is how Db code will be compiled/interpreted (there will be options for partial AOT as well as interpreted models).

## BassClefStudio.DbLanguage.Tests
The DbLanguage.Tests project contains unit tests for the other projects, including .Core and .Compiler (.Base is in progress).

## BassClefStudio.DbLanguage.Base
The DbLanguage.Base project provides the basic data types, such as `object`, `string`, and `int`, etc. as well as methods and classes to implement additional base libraries in C# (for example, providing platform-specific features).
