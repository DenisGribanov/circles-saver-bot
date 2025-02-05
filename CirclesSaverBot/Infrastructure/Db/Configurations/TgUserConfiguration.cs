using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Db.Configurations
{
    public class TgUserConfiguration : IEntityTypeConfiguration<TgUser>
    {
        public void Configure(EntityTypeBuilder<TgUser> entity)
        {
            entity.HasKey(e => e.ChatId)
                .HasName("tg_user_pk");

            entity.ToTable("TgUser");

            entity.HasIndex(e => e.ChatId, "tg_user_chat_id_uindex")
                .IsUnique();

            entity.Property(e => e.ChatId)
            .ValueGeneratedNever();
        }
    }
}