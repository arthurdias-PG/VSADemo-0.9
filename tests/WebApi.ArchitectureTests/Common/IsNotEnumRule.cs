using Mono.Cecil;

namespace VSADemo.ArchitectureTests.Common;

public class IsNotEnumRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type) => !type.IsEnum;
}