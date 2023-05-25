using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using MvcReact.Internal;

namespace MvcReact.TagHelpers;

public class ReactScriptsTagHelper : TagHelper
{
    private readonly IInternalFileProvider _fileProvider;
    private readonly IConfiguration _configuration;
    private readonly MvcReactOptions _options;
    private readonly IMemoryCache _memoryCache;

    public ReactScriptsTagHelper(IInternalFileProvider fileProvider, IConfiguration configuration, IOptions<MvcReactOptions> options,
        IMemoryCache memoryCache)
    {
        _fileProvider = fileProvider;
        _configuration = configuration;
        _options = options.Value;
        _memoryCache = memoryCache;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var environment = _configuration[Constants.ASPNETCORE_ENVIRONMENT];

        if (string.Equals(environment, Constants.ENVIRONMENT_DEVELOPMENT, StringComparison.OrdinalIgnoreCase))
        {
            // in development, we want to use the CRA dev server and not cache anything
            output.TagName = "script";
            output.Attributes.Add("src", _options.DevServerBundlePath);
            output.TagMode = TagMode.StartTagAndEndTag;
            return;
        }

        string content;

        // generate new script tags only if they are not already cached
        var cacheKey = $"${nameof(ReactScriptsTagHelper)}_{context.UniqueId}";
        if (!_memoryCache.TryGetValue(cacheKey, out content))
        {
            // Get the CRA generated index file, which includes optimized scripts
            var indexPage = _fileProvider.GetFileInfo(_options.IndexHtmlPath);

            // read the file
            var fileContents = await File.ReadAllTextAsync(indexPage.PhysicalPath);

            // find all script tags
            var scriptTags = Regex.Matches(fileContents, "<script.*?</script>", RegexOptions.Singleline);

            // get the script tags as strings
            var scriptTagsAsStrings = scriptTags.Select(m => m.Value).ToArray();

            content = string.Join(Environment.NewLine, scriptTagsAsStrings);

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