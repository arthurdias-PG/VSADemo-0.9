namespace VSADemo.Common.Domain.Projects;

public static class ProjectErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Project.NotFound",
        "Project is not found");
}