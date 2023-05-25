[![Build Status](https://dev.azure.com/ucdavis/MvcReact/_apis/build/status/ucdavis.MvcReact?branchName=main)](https://dev.azure.com/ucdavis/MvcReact/_build/latest?definitionId=35&branchName=main)
[![Build Status](https://github.com/ucdavis/MvcReact/actions/workflows/codeql.yml/badge.svg)](https://github.com/ucdavis/MvcReact/actions/workflows/codeql.yml/badge.svg)
[![NuGet version (MvcReact)](https://img.shields.io/nuget/v/MvcReact.svg)](https://www.nuget.org/packages/MvcReact/)


# MvcReact
Library to simplify setup of AspNetCore app that serves both Mvc and React pages

## Installation
```bash
dotnet add package MvcReact
```

## Usage

Add a using statement in your app initialization code (usually `Program.cs` or `Startup.cs`)

```csharp
using MvcReact;
```

Initialize services

```csharp
services.AddMvcReact();
```

Or for explicit control over settings...

```csharp
services.AddMvcReact(options =>
{
    options.SourcePath = "ClientApp";
    options.BuildPath = "ClientApp/build";
    options.IndexHtmlPath = "ClientApp/build/index.html";
    options.StaticAssetBasePath = "/static";
    options.StaticAssetHeaderCacheMaxAgeDays = 365;
    options.DevServerBundlePath = "/static/js/bundle.js";
    options.DevServerStartScript = "start";
    options.TagHelperCacheMinutes = 30;
    options.ExcludeHmrPathsRegex = "^(?!ws|.*?hot-update.js(on)?).*$";
});
```

`UseMvcReactStaticFiles` and `UseMvcReact` are the important extension methods here, along with `MvcReactOptions.ExcludeHmrPathsRegex` for letting hmr requests fall through

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<MvcReactOptions> mvcReactOptions)
{
    app.UseStaticFiles();
    app.UseMvcReactStaticFiles();
    app.UseRouting();

    app.UseEndpoints(routes =>
    {
        if (env.IsDevelopment())
        {
            routes.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}",
                constraints: new { controller = mvcReactOptions.Value.ExcludeHmrPathsRegex });
        }
        else
        {
            routes.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    });

    app.UseMvcReact();
}
```

Import tag helpers in `_ViewImports.cshtml`
```cshtml
@using MvcReact 
@addTagHelper *, MvcReact
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

Use `<react-scripts />` and `<react-styles />` tag helpers to embed relavent tags in a page/view

```html
@model SomeModel


@{
    ViewData["Title"] = "Page title";
}

<div id="react-app"></div>

@section Styles
{
    <react-styles />
}

@section Scripts
{
    <react-scripts />
}
```