using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Db.Configurations
{
    public class InlineResultStatisticsConfiguration : IEntityTypeConfiguration<InlineResultStatistics>
    {
        public void Configure(EntityTypeBuilder<InlineResultStatistics> entity)
        {
            entity.ToTable("InlineResultStatistics");

            entity.Property(x => x.Id).UseIdentityAlwaysColumn();

            entity.HasOne(x => x.TgUser).WithMany(x => x.PictureClickStatistics)
                .HasForeignKey(x => x.TgUserId)
                .HasConstraintName("inline_result_statistics_tg_user_fk_id");

            entity.HasOne(x => x.TgMediaFile).WithMany(x => x.ClickStatistics)
                .HasForeignKey(x => x.TgMediaFileId)
                .HasConstraintName("inline_result_statistics_tg_media_file_fk_id");
        }
    }
}