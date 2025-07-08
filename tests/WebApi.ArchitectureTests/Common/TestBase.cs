using System.Reflection;

namespace VSADemo.ArchitectureTests.Common;

public abstract class TestBase
{
    protected const string DomainAssemblyName = "Domain";
    protected const string CommandsAssemblyName = "Commands";
    protected const string QueriesAssemblyName = "Queries";

    protected static readonly Assembly RootAssembly = typeof(VSADemo.Program).Assembly;
}