#addin nuget:?package=Cake.Docker
#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/HelloWorld/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./src/HelloWorld.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./src/HelloWorld.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./src/HelloWorld.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {
        NoResults = true
        });
});

/////////////////////////////////////////////////////////////////////////
// Build docker image for the solution
/////////////////////////////////////////////////////////////////////////
Task("Docker-Build")
.Does(() => {
    var settings = new DockerBuildSettings { Tag = new[] {"helloworld-ditu.net:latest" }};
    DockerBuild(settings, "./src");
});

/////////////////////////////////////////////////////////////////////////
// Build Nuget Package
/////////////////////////////////////////////////////////////////////////
/*Task("Package")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
    {

        var nuGetPackSettings=new NuGetPackSettings{
            Id = "HelloWorld",
            Title = "HelloWorld",
            Authors = new[] {"ditu"},
            Owners = new[] {"ditu"},
            Description = "HelloWorld project",
            Symbols = false,
            NoPackageAnalysis = true,
            Files = new[]{
                new NuSpecContent {Source = "HelloWorld.dll",Target = "bin"},
            },
            BasePath = "./src/HelloWorld/bin/Release/netcoreapp1.1/" + configuration,
            OutPutDirectory = "./build/nuget"

        };
        NuGetPack(nuGetPackSettings);
    });
*/


Task("Package")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    var nuGetPackSettings   = new NuGetPackSettings {
        Id                      = "HelloWorld",
        Version                 = "0.1.0",
        Title                   = "HelloWorld",
        Authors                 = new[] {"ditu"},
        Owners                  = new[] {"ditu"},
        Description             = "HelloWorld",
        ProjectUrl              = new Uri("http://helloworld"),
        IconUrl                 = new Uri("http://helloworld"),
        LicenseUrl              = new Uri("http://helloworld"),
        Copyright               = "ditu",
        ReleaseNotes            = new [] {"Bug fixes", "Issue fixes", "Typos"},
        Tags                    = new [] {"ditu", "Cake", "Sample"},
        RequireLicenseAcceptance= false,
        Symbols                 = false,
        NoPackageAnalysis       = true,
        Files                   = new [] {
            new NuSpecContent {Source = "HelloWorld.dll", Target = "bin"},
        },
        BasePath                = "./src/HelloWorld/bin/release/netcoreapp1.1/",
        OutputDirectory         = "./BuildArtifacts/nuget"
    };

    NuGetPack(nuGetPackSettings);
});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests")
	.IsDependentOn("Docker-Build")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
