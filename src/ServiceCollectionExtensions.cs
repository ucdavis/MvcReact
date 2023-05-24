
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

using MvcReact.TagHelpers;

namespace MvcReact;

public static class ServiceCollectionExtensions
{
        public static IServiceCollection AddMvcReact(this IServiceCollection services)
        {
            return services.AddMvcReact(_ => { });
        }

        public static IServiceCollection AddMvcReact(this IServiceCollection services, Action<MvcReactOptions> configureOptions)
        {
            Action<MvcReactOptions> optionBuilder = options =>
            {
                // default config happens here
                options.SourcePath = "ClientApp";
                options.BuildPath = "ClientApp/build";
                options.IndexHtmlPath = "ClientApp/build/index.html";
                options.StaticAssetBasePath = "/static";
                options.StaticAssetHeaderCacheMaxAgeDays = 365;
                options.DevServerBundlePath = "/static/js/bundle.js";
                options.DevServerStartScript = "start";
                options.TagHelperCacheMinutes = 30;
                options.ExcludeHmrPathsRegex = "^(?!ws|.*?hot-update.js(on)?).*$";

                // allow for custom config...
                configureOptions(options);
            };
            services.Configure(optionBuilder);

            // not sure if there is a better way to get at the options here
            var options = new MvcReactOptions();
            optionBuilder(options);
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = options.BuildPath;
            });            

            services.AddScoped<IFileProvider>(_ => new PhysicalFileProvider(Directory.GetCurrentDirectory()));  // lgtm [cs/local-not-disposed] 
            services.AddTransient<ITagHelper, ReactScriptsTagHelper>();
            services.AddTransient<ITagHelper, ReactStylesTagHelper>();
            return services;
        }

}