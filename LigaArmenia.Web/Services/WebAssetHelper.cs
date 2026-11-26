namespace LigaArmenia.Web.Services;

public interface IWebAssetHelper
{
    bool Exists(string relativeWwwrootPath);
    string ResolveUrl(string relativeWwwrootPath);
}

public sealed class WebAssetHelper : IWebAssetHelper
{
    private readonly IWebHostEnvironment _env;

    public WebAssetHelper(IWebHostEnvironment env)
    {
        _env = env;
    }

    public bool Exists(string relativeWwwrootPath)
    {
        if (string.IsNullOrWhiteSpace(_env.WebRootPath) || string.IsNullOrWhiteSpace(relativeWwwrootPath))
            return false;

        var normalized = relativeWwwrootPath.TrimStart('~', '/', '\\')
            .Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_env.WebRootPath, normalized);
        return File.Exists(fullPath);
    }

    public string ResolveUrl(string relativeWwwrootPath)
    {
        return "/" + relativeWwwrootPath.TrimStart('~', '/', '\\').Replace('\\', '/');
    }
}
