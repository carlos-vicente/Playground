///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutions = GetFiles("../*.sln");

var solution = solutions.Single();

var unitTestsPath = "../*/bin/" + configuration + "/*.UnitTests.dll";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Restore NuGet")
    .Does(() =>
{
    // Restore all NuGet packages.
    Information("Restoring {0}...", solution);
    NuGetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore NuGet")
    .Does(() =>
{
    MSBuild(solution, settings => 
        settings
            .SetPlatformTarget(PlatformTarget.MSIL)
            .SetConfiguration(configuration)
            .WithTarget("Clean;Build")
    );
});

Task("Run NUnit tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var files = GetFiles(unitTestsPath);
    
    NUnit3(files);
});

Task("Code cover NUnit tests")
    .Does(() =>
{
    var assemblies = GetFiles(unitTestsPath);
    
    var coverSettings = new DotCoverCoverSettings()
        .WithFilter("-:*.UnitTests");
    
    DotCoverCover(
        tool => tool.NUnit3(assemblies),
        File("./results.dcvr").Path,
        coverSettings);
});


///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run NUnit tests")
    .IsDependentOn("Code cover NUnit tests");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
