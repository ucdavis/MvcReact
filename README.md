[![Build Status](https://dev.azure.com/ucdavis/MvcReact/_apis/build/status/ucdavis.MvcReact?branchName=main)](https://dev.azure.com/ucdavis/MvcReact/_build/latest?definitionId=35&branchName=main)
[![Build Status](https://github.com/ucdavis/MvcReact/actions/workflows/codeql.yml/badge.svg)](https://github.com/ucdavis/MvcReact/actions/workflows/codeql.yml/badge.svg)
[![NuGet version (MvcReact)](https://img.shields.io/nuget/v/MvcReact.svg)](https://www.nuget.org/packages/MvcReact/)


# MvcReact
Library to simplify setup of AspNetCore app that serves both Mvc and React pages.
Supported dev servers are CRA (Webpack) and Vite.

## Installation
```bash
dotnet add package MvcReact
# to install vite dependencies
cd ClientApp
npm install -D vite @vitejs/plugin-react
```


## Usage

Add a using statement in your app initialization code (usually `Program.cs` or `Startup.cs`)

```csharp
using MvcReact;
```

Initialize CRA services

```csharp
services.AddCraServices();
```

Or for explicit control over settings...

```csharp
services.AddCraServices(options =>
{
    options.SourcePath = "ClientApp";
    options.BuildPath = "ClientApp/build";
    options.IndexHtmlPath = "ClientApp/build/index.html";
    options.StaticAssetBasePath = "/static";
    options.StaticAssetHeaderCacheMaxAgeDays = 365;
    options.CraDevServerBundlePath = "/static/js/bundle.js";
    options.DevServerStartScript = "start";
    options.DevServerPort = 3000;
    options.TagHelperCacheMinutes = 30;
    options.ExcludeHmrPathsRegex = "^(?!ws|.*?hot-update.js(on)?).*$";
});
```

Initialize Vite services

```csharp
services.AddViteServices();
```

Or for explicit control over settings...

```csharp
services.AddViteServices(options =>
{
    options.SourcePath = "ClientApp";
    options.BuildPath = "ClientApp/build";
    options.IndexHtmlPath = "ClientApp/build/index.html";
    options.StaticAssetBasePath = "/static";
    options.StaticAssetHeaderCacheMaxAgeDays = 365;
    options.DevServerStartScript = "start";
    options.DevServerPort = 5173;
    options.ViteDevServerEntry = "/index.tsx";
    options.TagHelperCacheMinutes = 30;
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

Configuring Vite

```typescript
import { UserConfig, defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig(async ({ mode }) => {
  // Load app-level env vars to node-level env vars.
  const env = { ...process.env, ...loadEnv(mode, process.cwd()) };
  process.env = env;

  const config: UserConfig = {
    root: "src",
    publicDir: "public",
    build: {
      outDir: "build",
      // rollupOptions beyond scope of this snippet
    },
    plugins: [react()],
    optimizeDeps: {
      include: [],
    },
    server: {
      port: 5173,
      hmr: {
        clientPort: 5173,
      },
      strictPort: true,
    },
  };

  return config;
});
```

Vite tends to leave orphaned node processes after a debugging session. This can be addressed by
having the npm start script kill the node process listening on a given port.

```json
{
  "scripts": {
    "start": "npx kill-port 5173 && vite"
  }
}
```
