using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubCordobaWallet.Infrastructure.Persistence.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("members");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Did).HasColumnName("did").IsRequired();
        builder.HasIndex(m => m.Did).IsUnique();

        builder.Property(m => m.MemberNumber).HasColumnName("member_number").IsRequired();
        builder.HasIndex(m => m.MemberNumber).IsUnique();

        builder.Property(m => m.FirstName).HasColumnName("first_name").IsRequired().HasMaxLength(100);
        builder.Property(m => m.LastName).HasColumnName("last_name").IsRequired().HasMaxLength(100);

        builder.Property(m => m.Dni).HasColumnName("dni").IsRequired().HasMaxLength(8);
        builder.HasIndex(m => m.Dni).IsUnique();

        builder.Property(m => m.CreatedAt).HasColumnName("created_at");
    }
}
