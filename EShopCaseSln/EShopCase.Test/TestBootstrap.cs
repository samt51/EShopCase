
using Serilog;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace EShopCase.Test;

public static class TestBootstrap
{
    static TestBootstrap()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Fatal()
            .CreateLogger();
    }
}