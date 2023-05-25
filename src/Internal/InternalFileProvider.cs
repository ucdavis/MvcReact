using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace MvcReact.Internal;

// just inheriting from IFileProvider to avoid potential conflicts with other IFileProviders
public interface IInternalFileProvider : IFileProvider
{
}

internal class InternalFileProvider : PhysicalFileProvider, IInternalFileProvider
{
    public InternalFileProvider(string root) : base(root)
    {
    }
}
