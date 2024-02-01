using BDataBase.Core.Models.Accounts;
using DataBase.Core;
using DataBase.Core.Interfaces;
using DataBase.Core.Models;
using DataBase.Core.Models.Accounts;
using DataBase.Core.Models.CommentModels;
using DataBase.Core.Models.Notifications;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.Posts;
using DataBase.Core.Models.Reacts;
using DataBase.Core.Models.VedioModels;
using DataBase.EF.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBaseRepository<UserAccounts> UserAccounts { get; private set; }
        public IBaseRepository<Friend> Friends { get; private set; }
        public IBaseRepository<FriendRequest> FriendRequests { get; private set; }
        public IBaseRepository<Notifications> Notification { get; private set; }
        public IBaseRepository<QuestionPost> QuestionPost { get; private set; }
        public IBaseRepository<Post> Post { get; private set; }
        public IBaseRepository<ProfilePhoto> ProfilePhoto { get; private set; }
        public IBaseRepository<CoverPhoto> CoverPhoto { get; private set; }
        public IBaseRepository<PostPhoto> PostPhoto { get; private set; }
        public IBaseRepository<PostVedio> PostVedio { get; private set; }
        public IBaseRepository<QuestionPhoto> QuestionPhoto { get; private set; }
        public IBaseRepository<QuestionVedio> QuestionVedio { get; private set; }
        public IBaseRepository<PostComment> PostComment { get; private set; }
        public IBaseRepository<QuestionComment> QuestionComment { get; private set; }
        public IBaseRepository<PostReact> PostReact { get; private set; }
        public IBaseRepository<QuestionReact> QuestionReact { get; private set; }
        public IBaseRepository<PostCommentReact> PostCommentReact { get; private set; }
        public IBaseRepository<QuestionCommentReact> QuestionCommentReact { get; private set; }
        public IBaseRepository<QuestionCommentVedio> QuestionCommentVedio { get; private set; }
        public IBaseRepository<QuestionCommentPhoto> QuestionCommentPhoto { get; private set; }
        public IBaseRepository<PostCommentVedio> PostCommentVedio { get; private set; }
        public IBaseRepository<PostCommentPhoto> PostCommentPhoto { get; private set; }
        public IBaseRepository<Chat> Chat { get; private set; }
        public IBaseRepository<ChatPhoto> ChatPhoto { get; private set; }
        public IBaseRepository<ChatVedio> ChatVedio { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            UserAccounts = new BaseRepository<UserAccounts>(_context);
            ProfilePhoto = new BaseRepository<ProfilePhoto>(_context);
            CoverPhoto = new BaseRepository<CoverPhoto>(_context);

            QuestionPost= new BaseRepository<QuestionPost>(_context);
            QuestionPhoto= new BaseRepository<QuestionPhoto>(_context);
            QuestionVedio = new BaseRepository<QuestionVedio>(_context);
            QuestionComment = new BaseRepository<QuestionComment>(_context);
            QuestionReact = new BaseRepository<QuestionReact>(_context);
            QuestionCommentReact = new BaseRepository<QuestionCommentReact>(_context);
            QuestionCommentVedio = new BaseRepository<QuestionCommentVedio>(_context);
            QuestionCommentPhoto = new BaseRepository<QuestionCommentPhoto>(_context);
            
            Post = new BaseRepository<Post>(_context);
            PostPhoto = new BaseRepository<PostPhoto>(_context);
            PostVedio= new BaseRepository<PostVedio>(_context);
            PostComment = new BaseRepository<PostComment>(_context);
            PostReact = new BaseRepository<PostReact>(_context);
            PostCommentReact = new BaseRepository<PostCommentReact>(_context);
            PostCommentPhoto = new BaseRepository<PostCommentPhoto>(_context);
            PostCommentVedio = new BaseRepository<PostCommentVedio>(_context) ;

            Friends = new BaseRepository<Friend>(_context);
            FriendRequests = new BaseRepository<FriendRequest>(_context);
            Notification = new BaseRepository<Notifications>(_context);
            Chat = new BaseRepository<Chat>(_context);
            ChatPhoto = new BaseRepository<ChatPhoto>(_context);
            ChatVedio = new BaseRepository<ChatVedio>(_context);

        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
