// Get arguments passed to the script

var target = Argument("target", "All");
var configuration = Argument("configuration", "Release");
var buildLabel = Argument("buildLabel", string.Empty);
var buildInfo = Argument("buildInfo", string.Empty);

// Parse release notes.
var releaseNotes = ParseReleaseNotes("./ReleaseNotes.md");

// Set version.
var version = releaseNotes.Version.ToString();
var semVersion = version + (buildLabel != "" ? ("-" + buildLabel) : string.Empty);
Information("Building version {0} of Querify.", version);

// Define directories.
var buildResultDir = "./build";
var testResultsDir = buildResultDir + "/test-results";

//////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
	CleanDirectories(new DirectoryPath[] {
		buildResultDir, testResultsDir });    
});

Task("Patch-Assembly-Info")
	.Description("Patches the AssemblyInfo files.")
	.IsDependentOn("Clean")
	.Does(() =>
{
	var file = "./src/SolutionInfo.cs";
	CreateAssemblyInfo(file, new AssemblyInfoSettings {
		Product = "Querify",
		Version = version,
		FileVersion = version,
		InformationalVersion = (version + buildInfo).Trim(),
		Copyright = "Copyright (c) Lotpath 2014",
		Description = "Query and paging extensions for NHibernate"
	});
});

Task("Build-Solution")
	.IsDependentOn("Patch-Assembly-Info")
	.Does(() =>
{
	MSBuild("./src/Querify.sln", s => 
		{ 
			s.Configuration = configuration;
		});
});

Task("Run-Unit-Tests")
	.IsDependentOn("Build-Solution")
	.Does(() =>
{
	XUnit("./src/**/bin/" + configuration + "/*.Tests.dll", new XUnitSettings {
		OutputDirectory = testResultsDir,
		XmlReport = true,
		HtmlReport = true
	});
});

Task("All")
	.Description("Final target.")
	.IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////////

RunTarget(target);