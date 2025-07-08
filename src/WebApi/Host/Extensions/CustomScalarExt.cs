using Scalar.AspNetCore;

namespace VSADemo.Host.Extensions;

public static class CustomScalarExt
{
    public static void MapCustomScalarApiReference(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapScalarApiReference(options => options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient));
    }
}