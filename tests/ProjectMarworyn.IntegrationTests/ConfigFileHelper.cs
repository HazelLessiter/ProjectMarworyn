namespace ProjectMarworyn.IntegrationTests;

internal static class ConfigFileHelper
{
    private static readonly string SolutionRoot = FindSolutionRoot();

    public static string GetPath(string relativeToCoreConfig) =>
        Path.Combine(SolutionRoot, "src", "ProjectMarworyn.Core", relativeToCoreConfig);

    private static string FindSolutionRoot()
    {
        var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (dir != null && !dir.GetFiles("*.sln").Any())
            dir = dir.Parent;

        return dir?.FullName ?? throw new DirectoryNotFoundException("Could not locate solution root from test assembly.");
    }
}