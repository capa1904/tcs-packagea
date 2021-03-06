﻿#r "FakeLib.dll"
open Fake
open System

// -------------------------------------------------------------------------------------------------------------------------------------------------

let targetDir = __SOURCE_DIRECTORY__@@"target/"
let buildDir = targetDir @@ "build/"
let nugetOutputFilesDir = targetDir @@ "nuget/"
let nugetInputFilesDir = targetDir @@ "nugetFiles/"

let testBuildDir = targetDir @@ "test/"
let testProjectFile = "PackageA.Test.Unit/PackageA.Test.Unit.csproj"
// -------------------------------------------------------------------------------------------------------------------------------------------------

let projectFile = "PackageA/PackageA.csproj"
let packageName = "PackageA"
let nuspecFile = "PackageA.nuspec"
let buildSuffix = match System.Environment.GetEnvironmentVariable("BUILD_BRANCH") with
                  | "develop" -> ""
                  | _ -> "-pre"

let version = getBuildParamOrDefault "version" "1.0.0" + buildSuffix

// -------------------------------------------------------------------------------------------------------------------------------------------------

let PrintYellow infoMessage =
    let curColor = Console.ForegroundColor
    Console.ForegroundColor <- ConsoleColor.Yellow
    printf "%s" infoMessage
    Console.ForegroundColor <- curColor

// -------------------------------------------------------------------------------------------------------------------------------------------------

Description "Clean the target directory"
Target "Clean" (fun _ ->
    CleanDir targetDir
)

// -------------------------------------------------------------------------------------------------------------------------------------------------

Description "Compiles the project file"
Target "Build" (fun _ ->
    run "Clean"

    MSBuildHelper.build(fun x ->
        { x  with
            Verbosity = Some Minimal
            Properties =["OutputPath", "../" @@ buildDir; "Configuration", getBuildParamOrDefault "buildMode" "Debug"]
            NodeReuse = false}) projectFile
    |> ignore

    MSBuildHelper.build(fun x ->
        { x  with
            Verbosity = Some Minimal
            Properties =["OutputPath", "../" @@ testBuildDir; "Configuration", getBuildParamOrDefault "buildMode" "Debug"]
            NodeReuse = false}) testProjectFile
    |> ignore

)

// -------------------------------------------------------------------------------------------------------------------------------------------------
Description "Builds ths NuGetPackage"
Target "BuildNuGetPackage" (fun _ ->

    let infoMessage = sprintf "Creating nuGet-Package: %s.%s.nupkg\n\n" packageName version
    PrintYellow infoMessage

    CreateDir nugetInputFilesDir
    CopyFiles (nugetInputFilesDir @@ "lib" @@ "net45") !!(buildDir + "PackageA.*")
    CreateDir nugetOutputFilesDir

    NuGet (fun p ->
        { p with
            WorkingDir = nugetInputFilesDir
            Version = version
            OutputPath = nugetOutputFilesDir
            Title = packageName
            Project = packageName
        }) nuspecFile
)

// -------------------------------------------------------------------------------------------------------------------------------------------------
Description "Run unit tests without category 'LongRunning'"
Target "Test" (fun _ ->
    !!(testBuildDir @@ "**/*.Test.Unit.dll")
      |> NUnit (fun p ->
          {p with
             DisableShadowCopy = true;
             ExcludeCategory = "SkipInFake|LongRunning"
             ShowLabels = false
             OutputFile = testBuildDir @@ "UnitTestResults_PackageA.xml" })
)

// -------------------------------------------------------------------------------------------------------------------------------------------------
/// Prints all available targets.
let listTargets() =
    let curColor = Console.ForegroundColor
    Console.ForegroundColor <- ConsoleColor.Green
    printf "Available targets:\n"
    TargetDict.Values
      |> Seq.iter (fun target ->
            Console.ForegroundColor <- ConsoleColor.Yellow
            printf " %s\n" target.Name
            Console.ForegroundColor <- ConsoleColor.Gray
            printf " %s\n" (if target.Description <> null then target.Description + "\n" else "")
            )
    Console.ForegroundColor <- curColor

// -------------------------------------------------------------------------------------------------------------------------------------------------
// Default target
Description "Display all available targets"
Target "help" (fun _ ->
    printfn "Run \"fake <TargetName>\" to run a target.\n\n"

    listTargets()
)

// -------------------------------------------------------------------------------------------------------------------------------------------------
RunTargetOrDefault "help"
