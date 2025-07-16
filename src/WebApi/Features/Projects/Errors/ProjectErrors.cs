using System;

namespace VSADemo.Features.Projects.Errors;

public class ProjectErrors
{
  public static readonly Error ProjectNotFound = Error.NotFound("Project.NotFound", "Project not found.");

}
