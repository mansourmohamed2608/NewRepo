using BDataBase.Core.Models.Accounts;
using DataBase.Core.Models.PhotoModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using DataBase.Core.Models.Accounts;

namespace DataBase.EF.DBConfiguration
{
    public class ProfileAccountConfiguration : IEntityTypeConfiguration<UserAccounts>
    {
        public void Configure(EntityTypeBuilder<UserAccounts> modelBuilder)
        {
            modelBuilder.
                HasKey(p => p.Id);

            modelBuilder.
                 HasMany(p => p.Posts)
                .WithOne(post => post.UserAccounts)
                .HasForeignKey(post => post.UserAccountsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(p => p.QuestionPosts)
                .WithOne(qp => qp.UserAccounts)
                .HasForeignKey(qp => qp.UserAccountsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.
                 HasMany(p => p.Notifications)
                .WithOne(n=> n.ActionedUser)
                .HasForeignKey(n => n.ActionedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(p => p.ProfilePhoto)
                .WithOne()
                .HasForeignKey<ProfilePhoto>(photo => photo.UserAccountsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(p => p.CoverPhoto)
                .WithOne()
                .HasForeignKey<CoverPhoto>(c => c.UserAccountsId)
                .OnDelete(DeleteBehavior.Cascade);
            
            
        }
    }
}
