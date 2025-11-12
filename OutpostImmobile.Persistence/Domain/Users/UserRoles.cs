using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutpostImmobile.Persistence.Enums;

namespace OutpostImmobile.Persistence.Domain.Users;
public class UserRoles
{
    public Guid Id { get; }
    public UserRoleNames RoleName { get; set; }
}

internal class UserRolesEntityConfiguration : IEntityTypeConfiguration<UserRoles>
{
    public void Configure(EntityTypeBuilder<UserRoles> builder)
    {
        builder.HasKey(x => x.Id);
    }
}