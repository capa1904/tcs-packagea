@echo off

.\.nuget\nuget.exe push target\nuget\PackageA.1.0.0.nupkg nugetfeed-release -Source http://localhost:8080/nugetRelease

PAUSE