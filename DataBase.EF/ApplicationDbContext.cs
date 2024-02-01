using BDataBase.Core.Models.Accounts;
using DataBase.Core.Interfaces;
using DataBase.Core.Models;
using DataBase.Core.Models.Accounts;
using DataBase.Core.Models.Authentication;
using DataBase.Core.Models.CommentModels;
using DataBase.Core.Models.Notifications;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.Posts;
using DataBase.Core.Models.Reacts;
using DataBase.Core.Models.VedioModels;
using DataBase.EF.DBConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.EF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<TokenCode> TokenCodes { get; set; }
        public DbSet<UserAccounts> UserAccounts { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<QuestionPost> QuestionPosts { get; set; }
        public DbSet<CoverPhoto> CoverPhotos { get; set; }
        public DbSet<ProfilePhoto> ProfilePhotos { get; set; }
        public DbSet<PostPhoto> PostPhotos { get; set; }
        public DbSet<QuestionPhoto> QuestionPhotos { get; set; }
        public DbSet<QuestionCommentPhoto> QuestionCommentPhotos { get; set; }
        public DbSet<PostCommentPhoto> PostCommentPhotos { get; set; }
        public DbSet<PostVedio> PostVedios { get; set; }
        public DbSet<QuestionVedio> QuestionVedios { get; set; }
        public DbSet<QuestionCommentVedio> QuestionCommentVedios { get; set; }
        public DbSet<PostCommentVedio> PostCommentVedios { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<QuestionComment> QuestionComments { get; set; }
        public DbSet<PostReact> PostReacts { get; set; }
        public DbSet<QuestionReact> QuestionReacts { get; set; }
        public DbSet<QuestionCommentReact> QuestionCommentReacts { get; set; }
        public DbSet<PostCommentReact> PostCommentReacts { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Notifications> Notification { get; set; }
        public DbSet<Chat> Chat { get; set; }
        public DbSet<ChatPhoto> ChatPhoto { get; set; }
        public DbSet<ChatVedio> ChatVedio { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ProfileAccountConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());
            modelBuilder.ApplyConfiguration(new CommentPostConfiguration());

            // Friend entity configuration
            modelBuilder.Entity<Friend>()
                .HasOne(f => f.FirstUser)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.FirstUserId)
                .OnDelete(DeleteBehavior.Restrict); // Or use DeleteBehavior.Cascade if you want to delete friends when a profile is deleted.

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.SecondUser)
                .WithMany()
                .HasForeignKey(f => f.SecondUserId)
                .OnDelete(DeleteBehavior.Restrict); // Or use DeleteBehavior.Cascade if you want to delete friends when a profile is deleted.

            // FriendRequest entity configuration
            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Requestor)
                .WithMany(u => u.FriendRequests)
                .HasForeignKey(fr => fr.RequestorId)
                .OnDelete(DeleteBehavior.Restrict); // Or use DeleteBehavior.Cascade if you want to delete friend requests when a profile is deleted.

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany()
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict); // Or use DeleteBehavior.Cascade if you want to delete friend requests when a profile is deleted.
            modelBuilder.Entity<PostCommentReact>()
                .HasOne(qc => qc.UserAccounts)
                .WithMany()
                .HasForeignKey(qc => qc.UserAccountsId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuestionCommentReact>()
                .HasOne(pc => pc.UserAccounts)
                .WithMany()
                .HasForeignKey(pc => pc.UserAccountsId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PostComment>()
                .HasOne(pc => pc.UserAccounts)
                .WithMany()
                .HasForeignKey(pc => pc.UserAccountsId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PostReact>()
                .HasOne(pr => pr.UserAccounts)
                .WithMany()
                .HasForeignKey(pr => pr.UserAccountsId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuestionComment>()
                .HasOne(qc => qc.UserAccounts)
                .WithMany()
                .HasForeignKey(qc => qc.UserAccountsId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuestionReact>()
                .HasOne(qr => qr.UserAccounts)
                .WithMany()
                .HasForeignKey(qr => qr.UserAccountsId)
                .OnDelete(DeleteBehavior.Restrict);






            // Configure the primary key for IdentityUserLogin<string>
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(login => new { login.LoginProvider, login.ProviderKey });
        }


    }
}
