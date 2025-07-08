namespace VSADemo.Common.Domain.Managers;

public static class ManagerErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Manager.NotFound",
        "Manager is not found");
}