@echo off

.\.nuget\nuget.exe push target\nuget\PackageA.1.0.0.nupkg nugetfeed -Source http://localhost:8080/nugetDev

PAUSE