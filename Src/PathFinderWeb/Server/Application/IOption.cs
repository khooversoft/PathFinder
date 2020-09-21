using PathFinder.sdk.Application;

namespace PathFinderWeb.Server.Application
{
    public interface IOption
    {
        string? ConfigFile { get; }
        string PathFinderApiUrl { get; }
        string? SecretId { get; }
        string? Environment { get; }
        RunEnvironment RunEnvironment { get; }
    }
}