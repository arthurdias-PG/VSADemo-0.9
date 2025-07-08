using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VSADemo.Common.Domain.Managers;

namespace VSADemo.Common.Persistence.Managers;

public class ManagerConfiguration : AuditableConfiguration<Manager>
{
    public override void PostConfigure(EntityTypeBuilder<Manager> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasMaxLength(Manager.NameMaxLength)
            .IsRequired();

        builder.Property(t => t.Email)
            .HasMaxLength(Email.EmailMaxLength)
            .IsRequired()
            .HasConversion(
                v => v.EmailAddress,
                v => new Email(v));

        builder.HasMany(t => t.Projects)
            .WithOne(t => t.Manager)
            .HasForeignKey(t => t.ManagerId)
            .IsRequired(false);

        builder.Navigation(m => m.Projects)
           .HasField("_projects")
           .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}