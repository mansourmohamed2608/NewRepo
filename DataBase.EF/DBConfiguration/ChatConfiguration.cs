using DataBase.Core.Models.CommentModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase.Core.Models;

namespace DataBase.EF.DBConfiguration
{
    internal class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> modelBuilder)
        {
            modelBuilder
                .HasMany(post => post.Photos)
                .WithOne(photo => photo.Chat)
                .HasForeignKey(photo => photo.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(post => post.Vedios)
                .WithOne(vedio => vedio.Chat)
                .HasForeignKey(vedio => vedio.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
