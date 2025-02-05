using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Db.Configurations
{
    public class TgMediaFileConfiguration : IEntityTypeConfiguration<TgMediaFile>
    {
        public void Configure(EntityTypeBuilder<TgMediaFile> entity)
        {
            entity.ToTable("TgMediaFile");

            entity.HasIndex(x => x.Id, "tg_media_file_id_index");

            entity.Property(x => x.Id).UseIdentityAlwaysColumn();

            entity.Property(x => x.OwnerTgUserId).IsRequired();

            entity.HasOne(x => x.TgUserOwner).WithMany(x => x.Pictures)
                .HasForeignKey(x => x.OwnerTgUserId)
                .HasConstraintName("tg_media_file_tg_user_fk_id");
        }
    }
}