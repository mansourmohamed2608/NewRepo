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
    public class QuestionConfiguration : IEntityTypeConfiguration<QuestionPost>
    {
        public void Configure(EntityTypeBuilder<QuestionPost> modelBuilder)
        {
            modelBuilder
                .HasKey(p => p.Id);

            modelBuilder
                .HasMany(post => post.Photos)
                .WithOne(photo => photo.QuestionPost)
                .HasForeignKey(photo => photo.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(post => post.Vedios)
                .WithOne(vedio => vedio.QuestionPost)
                .HasForeignKey(vedio => vedio.QuestionPostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                 .HasMany(post => post.Comments)
                 .WithOne(Comments => Comments.QuestionPost)
                 .HasForeignKey(Comments => Comments.QuestionPostId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                 .HasMany(post => post.Reacts)
                 .WithOne(Reacts => Reacts.QuestionPost)
                 .HasForeignKey(Comments => Comments.QuestionPostId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
