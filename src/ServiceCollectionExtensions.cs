
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MvcReact.Internal;
using MvcReact.TagHelpers;

namespace MvcReact;

public static class ServiceCollectionExtensions
{
        public static IServiceCollection AddCraServices(this IServiceCollection services)
        {
            return services.AddCraServices(_ => { });
        }

        public static IServiceCollection AddCraServices(this IServiceCollection services, Action<MvcReactOptions> configureOptions)
        {
            Action<MvcReactOptions> optionBuilder = options =>
            {
                // default config happens here
                options.SourcePath = "ClientApp";
                options.BuildPath = "ClientApp/build";
                options.IndexHtmlPath = "ClientApp/build/index.html";
                options.StaticAssetBasePath = "/static";
                options.StaticAssetHeaderCacheMaxAgeDays = 365;
                options.CraDevServerBundlePath = "/static/js/bundle.js";
                options.DevServerStartScript = "start";
                options.DevServerType = DevServerType.CRA;
                options.DevServerPort = 3000;
                options.TagHelperCacheMinutes = 30;
                options.ExcludeHmrPathsRegex = "^(?!ws|.*?hot-update.js(on)?).*$";

                // allow for custom config...
                configureOptions(options);
            };

            AddReactMvcServices(services, optionBuilder);
            
            return services;
        }

        public static IServiceCollection AddViteServices(this IServiceCollection services)
        {
            return services.AddViteServices(_ => { });
        }

        public static IServiceCollection AddViteServices(this IServiceCollection services, Action<MvcReactOptions> configureOptions)
        {
            Action<MvcReactOptions> optionBuilder = options =>
            {
                // default config happens here
                options.SourcePath = "ClientApp";
                options.BuildPath = "ClientApp/build";
                options.IndexHtmlPath = "ClientApp/build/index.html";
                options.StaticAssetBasePath = "/static";
                options.StaticAssetHeaderCacheMaxAgeDays = 365;
                options.DevServerStartScript = "start";
                options.DevServerType = DevServerType.Vite;
                options.DevServerPort = 5173;
                options.ViteDevServerEntry = "/index.tsx";
                options.TagHelperCacheMinutes = 30;


                // allow for custom config...
                configureOptions(options);
            };

            AddReactMvcServices(services, optionBuilder);
            
            return services;
        }

        private static void AddReactMvcServices(IServiceCollection services, Action<MvcReactOptions> optionBuilder)
        {
            services.Configure(optionBuilder);

            // not sure if there is a better way to get at the options here
            var options = new MvcReactOptions();
            optionBuilder(options);
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = options.BuildPath;
            });            

            services.AddMemoryCache();
            services.AddScoped<IInternalFileProvider>(_ => new InternalFileProvider(Directory.GetCurrentDirectory()));  // lgtm [cs/local-not-disposed] 
            services.AddTransient<ITagHelper, ReactScriptsTagHelper>();
            services.AddTransient<ITagHelper, ReactStylesTagHelper>();
        }

}