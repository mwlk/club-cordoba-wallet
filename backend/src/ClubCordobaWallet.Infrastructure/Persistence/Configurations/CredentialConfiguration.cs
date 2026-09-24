using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubCordobaWallet.Infrastructure.Persistence.Configurations;

public class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> builder)
    {
        builder.ToTable("credentials");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.MemberId).HasColumnName("member_id");
        builder.HasOne(c => c.Member)
            .WithMany(m => m.Credentials)
            .HasForeignKey(c => c.MemberId);

        // La VC completa (id, type, issuer, credentialSubject, validFrom,
        // validUntil, credentialStatus, proof) tal cual la firmó el Issuer.
        // `text`, no `jsonb`: jsonb reordena claves y quita espacios al
        // guardar (confirmado leyendo filas crudas), lo que rompía la
        // promesa de persistir el documento firmado tal cual salió del
        // Issuer. Con `text` el string queda byte a byte idéntico al que
        // se firmó. Ver docs/decisiones.md.
        builder.Property(c => c.VcJson).HasColumnName("vc_json").HasColumnType("text").IsRequired();

        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
    }
}
