using VSADemo.Common.Domain.Managers;
using VSADemo.Common.Domain.Projects;


namespace VSADemo.Common.Persistence;

// TODO: New strongly typed IDs should be registered here

[EfCoreConverter<ProjectId>]
[EfCoreConverter<ManagerId>]
internal sealed partial class VogenEfCoreConverters;