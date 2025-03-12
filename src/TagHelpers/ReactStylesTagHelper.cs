using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using MvcReact.Internal;

namespace MvcReact.TagHelpers;

public class ReactStylesTagHelper : TagHelper
{
    private readonly IInternalFileProvider _fileProvider;
    private readonly IConfiguration _configuration;
    private readonly MvcReactOptions _options;
    private readonly IMemoryCache _memoryCache;

    public ReactStylesTagHelper(IInternalFileProvider fileProvider, IConfiguration configuration, IOptions<MvcReactOptions> options,
        IMemoryCache memoryCache)
    {
        _fileProvider = fileProvider;
        _configuration = configuration;
        _options = options.Value;
        _memoryCache = memoryCache;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // react styles are only needed in production or staging, since they will be handled by the dev server in development
        var environment = _configuration[Constants.ASPNETCORE_ENVIRONMENT];

        if (string.Equals(environment, Constants.ENVIRONMENT_DEVELOPMENT, StringComparison.OrdinalIgnoreCase))
        {
            output.SuppressOutput();
            return;
        }

        string content;

        // generate new style tags only if they are not already cached
        var cacheKey = $"${nameof(ReactStylesTagHelper)}_{context.UniqueId}";
        if (!_memoryCache.TryGetValue(cacheKey, out content))
        {
            switch (_options.DevServerType)
            {
                case DevServerType.CRA:
                    // Get the CRA generated index file, which includes optimized scripts
                    var indexPage = _fileProvider.GetFileInfo(_options.IndexHtmlPath);

                    // read the file
                    var fileContents = await File.ReadAllTextAsync(indexPage.PhysicalPath);

                    // find all link tags with the rel attribute set to stylesheet
                    var linkTags = Regex.Matches(fileContents, "<link.*?>", RegexOptions.IgnoreCase)
                        .Where(m => m.Value.Contains("rel=\"stylesheet\""))
                        .Select(m => m.Value);

                    content = string.Join(Environment.NewLine, linkTags);
                    break;
                case DevServerType.Vite:
                    // Get all css files in the Assets subfolder
                    var assetsFolder = Path.Combine(_options.BuildPath, "assets");
                    var cssFiles = Directory.GetFiles(assetsFolder, "*.css");

                    // Generate link tags for each css file
                    linkTags = cssFiles.Select(file => $"<link href=\"/assets/{Path.GetFileName(file)}\" rel=\"stylesheet\">");
                    content = string.Join(Environment.NewLine, linkTags);
                    break;
                default:
                    throw new Exception($"Unknown dev server type: {_options.DevServerType}");
            }

            _memoryCache.Set(cacheKey, content, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.TagHelperCacheMinutes)
            });
        }

        // if TagName is not set to null, it ignores explicitly-set content and expects attributes to be individually set  
        output.TagName = null;
        output.Content.SetHtmlContent(content);

    }
}