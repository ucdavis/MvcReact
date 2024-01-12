namespace MvcReact;

public class MvcReactOptions
{
    /// <summary>
    /// The path to the index.html file generated by CRA
    /// </summary>
    /// <value></value>
    public string IndexHtmlPath { get; set; } = "";

    /// <summary>
    /// The path to the js bundle served by the CRA dev server
    /// </summary>
    /// <value></value>
    public string CraDevServerBundlePath { get; set; } = "";

    /// <summary>
    /// The path to the entry point for the Vite dev server
    /// </summary>
    public string ViteDevServerEntry { get; set; } = "main.tsx";

    /// <summary>
    /// The type of dev server used (CRA or Vite)
    /// </summary>
    public string DevServerType { get; set; } = MvcReact.DevServerType.CRA;

    /// <summary>
    /// The path to the js bundle served by the CRA dev server
    /// </summary>
    /// <value></value>
    public int DevServerPort { get; set; } = 3000;

    /// <summary>
    /// The npm script to start the CRA dev server
    /// </summary>
    /// <value></value>
    public string DevServerStartScript { get; set; } = "";

    /// <summary>
    /// A regex to exclude requests that should be passed through to the dev server
    /// </summary>
    /// <value></value>
    public string ExcludeHmrPathsRegex { get; set; } = "";

    /// <summary>
    /// The number of minutes to cache the generated script tags
    /// </summary>
    /// <value></value>
    public int TagHelperCacheMinutes { get; set; }

    /// <summary>
    /// The path to the build folder generated by CRA
    /// </summary>
    /// <value></value>
    public string BuildPath { get; set; } = "";

    /// <summary>
    /// The location where the CRA source code is located
    /// </summary>
    /// <value></value>
    public string SourcePath { get; set; } = "";

    /// <summary>
    /// The base path for static assets bundled by CRA
    /// </summary>
    /// <value></value>
    public string StaticAssetBasePath { get; set; } = "";

    /// <summary>
    /// The number of days to specify for the Cache-Control header for static assets
    /// </summary>
    /// <value></value>
    public int StaticAssetHeaderCacheMaxAgeDays { get; set; }
}