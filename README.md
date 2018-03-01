# log-message-framework45

A dotnet framework solution to be built with [dotnet-pipelines](https://github.com/ddieruf/dotnet-pipelines).

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 
See deployment for notes on how to deploy the project on a live system.

### Prerequisites

What things you need to install the software and how to install them

```
Give examples
```

### Solution Projects

A step by step series of examples that tell you have to get a development env running

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [Visual Studio 2017 Community](http://www.dropwizard.io/1.0.2/docs/) - To Write code
* [xUnit](http://www.dropwizard.io/1.0.2/docs/) - For testing code
* [netframework4.5](https://docs.microsoft.com/en-us/dotnet/standard/frameworks) - The runtime
* [dotnet-pipelines](https://github.com/ddieruf/dotnet-pipelines) - To test and deploy

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Workarounds and Issues

### Associating the SDK
Applies to: All project(*.csproj) files - close VS and use a text editor to modify the file

Add this with in the Project node

```
<Project Sdk="Microsoft.NET.Sdk"
```

### Adding target framework and getting around general assembly issue
Applies to: All project(*.csproj) files - close VS and use a text editor to modify the file
This must be added within the primary PropertyGroup and the TargetFramework needs to reflect the project's target.

```
<PropertyGroup>
...
<TargetFramework>net452</TargetFramework>
<GenerateAssemblyInfo>false</GenerateAssemblyInfo> <!--https://github.com/dotnet/cli/issues/4710-->
</PropertyGroup>
```

### Remove compile references
Applies to: All project(*.csproj) files - close VS and use a text editor to modify the file
https://stackoverflow.com/questions/43325916/duplicate-content-items-were-included-the-net-sdk-includes-content-items-f
https://blogs.msdn.microsoft.com/shaneosborne/2017/01/31/breaking-changes-in-visual-studio-2017-rc-build-26127-00/

```
REMOVE: <ItemGroup><Compile ... /></ItemGroup>
```

### Add FrameworkPathOverride to target mono's complete dotnet framework
Applies to: All project(*.csproj) files - close VS and use a text editor to modify the file
https://github.com/dotnet/sdk/issues/335

This must be added just after the primary PropertyGroup

```
...
</PropertyGroup>
<PropertyGroup>
  <!-- When compiling .NET SDK 2.0 projects targeting .NET 4.x on Mono using 'dotnet build' you -->
  <!-- have to teach MSBuild where the Mono copy of the reference asssemblies is -->
  <TargetIsMono Condition="$(TargetFramework.StartsWith('net4')) and '$(OS)' == 'Unix'">true</TargetIsMono>
    
  <!-- Look in the standard install locations -->
  <BaseFrameworkPathOverrideForMono Condition="'$(BaseFrameworkPathOverrideForMono)' == '' AND '$(TargetIsMono)' == 'true' AND EXISTS('/Library/Frameworks/Mono.framework/Versions/Current/lib/mono')">/Library/Frameworks/Mono.framework/Versions/Current/lib/mono</BaseFrameworkPathOverrideForMono>
  <BaseFrameworkPathOverrideForMono Condition="'$(BaseFrameworkPathOverrideForMono)' == '' AND '$(TargetIsMono)' == 'true' AND EXISTS('/usr/lib/mono')">/usr/lib/mono</BaseFrameworkPathOverrideForMono>
  <BaseFrameworkPathOverrideForMono Condition="'$(BaseFrameworkPathOverrideForMono)' == '' AND '$(TargetIsMono)' == 'true' AND EXISTS('/usr/local/lib/mono')">/usr/local/lib/mono</BaseFrameworkPathOverrideForMono>

  <!-- If we found Mono reference assemblies, then use them -->
  <FrameworkPathOverride Condition="'$(BaseFrameworkPathOverrideForMono)' != '' AND '$(TargetFramework)' == 'net45'">$(BaseFrameworkPathOverrideForMono)/4.5-api</FrameworkPathOverride>
  <FrameworkPathOverride Condition="'$(BaseFrameworkPathOverrideForMono)' != '' AND '$(TargetFramework)' == 'net451'">$(BaseFrameworkPathOverrideForMono)/4.5.1-api</FrameworkPathOverride>
  <FrameworkPathOverride Condition="'$(BaseFrameworkPathOverrideForMono)' != '' AND '$(TargetFramework)' == 'net452'">$(BaseFrameworkPathOverrideForMono)/4.5.2-api</FrameworkPathOverride>
  <FrameworkPathOverride Condition="'$(BaseFrameworkPathOverrideForMono)' != '' AND '$(TargetFramework)' == 'net46'">$(BaseFrameworkPathOverrideForMono)/4.6-api</FrameworkPathOverride>
  <FrameworkPathOverride Condition="'$(BaseFrameworkPathOverrideForMono)' != '' AND '$(TargetFramework)' == 'net461'">$(BaseFrameworkPathOverrideForMono)/4.6.1-api</FrameworkPathOverride>
  <FrameworkPathOverride Condition="'$(BaseFrameworkPathOverrideForMono)' != '' AND '$(TargetFramework)' == 'net462'">$(BaseFrameworkPathOverrideForMono)/4.6.2-api</FrameworkPathOverride>
  <FrameworkPathOverride Condition="'$(BaseFrameworkPathOverrideForMono)' != '' AND '$(TargetFramework)' == 'net47'">$(BaseFrameworkPathOverrideForMono)/4.7-api</FrameworkPathOverride>
  <FrameworkPathOverride Condition="'$(BaseFrameworkPathOverrideForMono)' != '' AND '$(TargetFramework)' == 'net471'">$(BaseFrameworkPathOverrideForMono)/4.7.1-api</FrameworkPathOverride>
  <EnableFrameworkPathOverride Condition="'$(BaseFrameworkPathOverrideForMono)' != ''">true</EnableFrameworkPathOverride>

  <!-- Add the Facades directory.  Not sure how else to do this. Necessary at least for .NET 4.5 -->
  <AssemblySearchPaths Condition="'$(BaseFrameworkPathOverrideForMono)' != ''">$(FrameworkPathOverride)/Facades;$(AssemblySearchPaths)</AssemblySearchPaths>
</PropertyGroup>
...
```

## Authors

* **David Dieruf** - *Initial work* - [GitHub](https://github.com/ddieruf)

## License

This project is licensed under the Apache License - see the [LICENSE.md](LICENSE.md) file for details