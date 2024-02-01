using DataBase.Core.Models.Posts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.EF.DBConfiguration
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> modelBuilder)
        {
            modelBuilder
               .HasKey(p => p.Id);

            modelBuilder
                .HasMany(post => post.Photos)
                .WithOne(photo => photo.Post)
                .HasForeignKey(photo => photo.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(post => post.Vedios)
                .WithOne(vedio => vedio.Post)
                .HasForeignKey(vedio => vedio.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                 .HasMany(post => post.Comments)
                 .WithOne(Comments => Comments.Post)
                 .HasForeignKey(Comments => Comments.PostId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                 .HasMany(post => post.Reacts)
                 .WithOne(Reacts => Reacts.Post)
                 .HasForeignKey(Comments => Comments.PostId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
