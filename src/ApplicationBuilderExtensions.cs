using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MvcReact;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMvcReact(this IApplicationBuilder app)
    {
        // MS suggested hack to ensure correct ordering of routing and spa middleware
        app.UseEndpoints(e => { });

        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        var options = app.ApplicationServices.GetRequiredService<IOptions<MvcReactOptions>>().Value;

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = options.SourcePath;
            spa.Options.DevServerPort = options.DevServerPort;

            if (env.IsDevelopment())
            {
                spa.UseReactDevelopmentServer(npmScript: options.DevServerStartScript);
            }
        });

        return app;
    }

    public static IApplicationBuilder UseMvcReactStaticFiles(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<MvcReactOptions>>().Value;
        app.UseSpaStaticFiles(new StaticFileOptions()
        {
            OnPrepareResponse = (context) =>
            {
                // cache our static assest, i.e. CSS and JS, for a long time
                if ((context.Context.Request.Path.Value ?? "").StartsWith(options.StaticAssetBasePath))
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(options.StaticAssetHeaderCacheMaxAgeDays)
                    };
                }
            }
        });
        return app;
    }
}