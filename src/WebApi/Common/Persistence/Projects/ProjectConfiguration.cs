using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSADemo.Common.Domain.Projects;

namespace VSADemo.Common.Persistence.Projects;

public class ProjectConfiguration : AuditableConfiguration<Project>
{
    public override void PostConfigure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasMaxLength(Project.NameMaxLength)
            .IsRequired();

        builder.Property(t => t.Deadline)
            .IsRequired()
            .HasConversion(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    }
}