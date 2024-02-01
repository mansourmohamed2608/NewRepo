using Business.Services;
using DataBase.Core;
using DataBase.Core.Consts;
using DataBase.Core.Models.CommentModels;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.VedioModels;
using DomainModels;
using DomainModels.Models;
using System.Drawing.Printing;
using System.Xml.Linq;
using Utilites;

namespace Business.Implementation
{
    public class CommentServices : ICommentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _pageSize = 5;

        public CommentServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> DeletePostCommentAsync(Guid commentId, string userEmail)
        {
            string[] includes = { "PostCommentPhoto", "PostCommentVedio", "PostCommentReacts" };
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var comment = await _unitOfWork.PostComment.FindAsync(p => p.Id == commentId, includes);
            if (comment == null || user == null)
                return false;
            if (comment.UserAccountsId != user.Id) return false;
            if (comment.PostCommentPhoto != null)
                _unitOfWork.PostCommentPhoto.Delete(comment.PostCommentPhoto);
            if (comment.PostCommentVedio != null)
                _unitOfWork.PostCommentVedio.Delete(comment.PostCommentVedio);
            if (comment.PostCommentReacts != null)
                _unitOfWork.PostCommentReact.DeleteRange(comment.PostCommentReacts);
            _unitOfWork.PostComment.Delete(comment);
            return await _unitOfWork.Complete() > 0;
        }

        public async Task<(bool, Guid)> AddPostCommentAsync(AddCommentRequest comment, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            if (user == null) return (false, Guid.Empty);
            var post = await _unitOfWork.Post.FindAsync(p => p.Id == comment.PostId);
            if (post == null) return (false, Guid.Empty);
            var Newcomment = new PostComment()
            {
                Id = Guid.NewGuid(),
                UserAccounts = user,
                UserAccountsId = user.Id,
                PostId = comment.PostId,
                Post = post,
                comment = comment.comment
            };
            await _unitOfWork.PostComment.AddAsync(Newcomment);
            if (comment.Photo != null)
            {
                var postComentPhoto = new PostCommentPhoto()
                {
                    Id = Guid.NewGuid(),
                    PhotoPath = MediaUtilites.ConverIformToPath(comment.Photo, SharedFolderPaths.CommentsPhotos),
                    PostCommentId = Newcomment.Id
                };
                await _unitOfWork.PostCommentPhoto.AddAsync(postComentPhoto);
            }
            if (comment.Vedio != null)
            {
                var postCommentVedio = new PostCommentVedio()
                {
                    Id = Guid.NewGuid(),
                    VedioPath = MediaUtilites.ConverIformToPath(comment.Vedio, SharedFolderPaths.CommentsVideos),
                    PostCommentId = Newcomment.Id
                };
                await _unitOfWork.PostCommentVedio.AddAsync(postCommentVedio);
            }
            var update = await _unitOfWork.Complete();
            return (update > 0, Newcomment.Id);
        }

        public async Task<bool> UpdatePostCommentAsync(CommentUpdateRequest comment, string userEmail)
        {
            string[] includes = { "PostCommentPhoto", "PostCommentVedio" };
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var cmnt = await _unitOfWork.PostComment.FindAsync(p => p.Id == comment.Id, includes);
            if (cmnt == null || user == null)
                return false;
            if (cmnt.UserAccountsId != user.Id) return false;
            cmnt.comment = comment.comment;
            _unitOfWork.PostComment.Update(cmnt);
            if (comment.DeletedVideoId != null)
            {
                _unitOfWork.PostCommentVedio.Delete(cmnt.PostCommentVedio);
                //await _unitOfWork.Complete();
            }
            if (comment.DeletedPhotoId != null)
            {
                _unitOfWork.PostCommentPhoto.Delete(cmnt.PostCommentPhoto);
                //await _unitOfWork.Complete();
            }
            if (comment.Photo != null)
            {

                var postCommentPhoto = new PostCommentPhoto()
                {
                    Id = Guid.NewGuid(),
                    PhotoPath = MediaUtilites.ConverIformToPath(comment.Photo, SharedFolderPaths.CommentsPhotos),
                    PostCommentId = cmnt.Id
                };
                await _unitOfWork.PostCommentPhoto.AddAsync(postCommentPhoto);

            }
            if (comment.Vedio != null)
            {

                var postCommentVedio = new PostCommentVedio()
                {
                    Id = Guid.NewGuid(),
                    VedioPath = MediaUtilites.ConverIformToPath(comment.Vedio, SharedFolderPaths.CommentsVideos),
                    PostCommentId = cmnt.Id
                };
                await _unitOfWork.PostCommentVedio.AddAsync(postCommentVedio);
            }

            return await _unitOfWork.Complete() > 0;
        }

        public async Task<bool> DeleteQuestionCommentAsync(Guid commentId, string userEmail)
        {
            string[] includes = { "QuestionCommentPhoto", "QuestionCommentVedio", "QuestionCommentReacts" };
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var comment = await _unitOfWork.QuestionComment.FindAsync(p => p.Id == commentId, includes);
            if (comment == null || user == null)
                return false;
            if (comment.UserAccountsId != user.Id) return false;
            if (comment.QuestionCommentPhoto != null)
                _unitOfWork.QuestionCommentPhoto.Delete(comment.QuestionCommentPhoto);
            if (comment.QuestionCommentVedio != null)
                _unitOfWork.QuestionCommentVedio.Delete(comment.QuestionCommentVedio);
            if (comment.QuestionCommentReacts != null)
                _unitOfWork.QuestionCommentReact.DeleteRange(comment.QuestionCommentReacts);
            _unitOfWork.QuestionComment.Delete(comment);
            return await _unitOfWork.Complete() > 0;
        }

        public async Task<(bool, Guid)> AddQuestionCommentAsync(AddCommentRequest comment, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            if (user == null) return (false, Guid.Empty);
            var post = await _unitOfWork.QuestionPost.FindAsync(p => p.Id == comment.PostId);
            if (post == null) return (false, Guid.Empty);
            var Newcomment = new QuestionComment()
            {
                Id = Guid.NewGuid(),
                UserAccounts = user,
                UserAccountsId = user.Id,
                QuestionPostId = comment.PostId,
                QuestionPost = post,
                comment = comment.comment
            };
            if (comment.Photo != null)
            {
                Newcomment.QuestionCommentPhoto = new QuestionCommentPhoto()
                {
                    Id = Guid.NewGuid(),
                    PhotoPath = MediaUtilites.ConverIformToPath(comment.Photo, SharedFolderPaths.CommentsPhotos),
                    QuestionCommentId = Newcomment.Id
                };
            }
            if (comment.Vedio != null)
            {
                Newcomment.QuestionCommentVedio = new QuestionCommentVedio()
                {
                    Id = Guid.NewGuid(),
                    VedioPath = MediaUtilites.ConverIformToPath(comment.Vedio, SharedFolderPaths.CommentsVideos),
                    QuestionCommentId = Newcomment.Id
                };
            }
            await _unitOfWork.QuestionComment.AddAsync(Newcomment);
            await _unitOfWork.Complete();
            return (true, Newcomment.Id);
        }

        public async Task<bool> UpdateQuestionCommentAsync(CommentUpdateRequest comment, string userEmail)
        {
            string[] includes = { "QuestionCommentPhoto", "QuestionCommentVedio" };
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var cmnt = await _unitOfWork.QuestionComment.FindAsync(p => p.Id == comment.Id, includes);
            if (cmnt == null || user == null)
                return false;
            if (cmnt.UserAccountsId != user.Id) return false;
            cmnt.comment = comment.comment;
            _unitOfWork.QuestionComment.Update(cmnt);
            if (comment.DeletedVideoId != null)
            {
                _unitOfWork.QuestionCommentVedio.Delete(cmnt.QuestionCommentVedio);
            }
            if (comment.DeletedPhotoId != null)
            {
                _unitOfWork.QuestionCommentPhoto.Delete(cmnt.QuestionCommentPhoto);
            }
            if (comment.Photo != null)
            {
                var questionCommentPhoto = new QuestionCommentPhoto()
                {
                    Id = Guid.NewGuid(),
                    PhotoPath = MediaUtilites.ConverIformToPath(comment.Photo, SharedFolderPaths.CommentsPhotos),
                    QuestionCommentId = cmnt.Id
                };
                await _unitOfWork.QuestionCommentPhoto.AddAsync(questionCommentPhoto);
            }
            if (comment.Vedio != null)
            {
                var questionCommentVedio = new QuestionCommentVedio()
                {
                    Id = Guid.NewGuid(),
                    VedioPath = MediaUtilites.ConverIformToPath(comment.Vedio, SharedFolderPaths.CommentsVideos),
                    QuestionCommentId = cmnt.Id
                };
                await _unitOfWork.QuestionCommentVedio.AddAsync(questionCommentVedio);
            }

            return await _unitOfWork.Complete() > 0;
        }
        // ALL NEW HERE
        public async Task<List<DomainModels.DTO.CommentDTO>> GetPostCommentsAsync(Guid postId, DateTime? Time)
        {

            string[] includes = { "PostCommentPhoto", "PostCommentVedio", "PostCommentReacts", "UserAccounts" };
            //var (take, skip) = BussnissHelper.GetTakeSkipValues(pageNumber, _pageSize);
            IEnumerable<PostComment> comments;
            if (Time == null)
                comments = await _unitOfWork.PostComment.FindAllAsync(p => p.PostId == postId, _pageSize, 0, includes, p => p.Date, OrderBy.Ascending);
            else
                comments = await _unitOfWork.PostComment.FindAllAsync(p => p.PostId == postId && p.Date > Time, 0, _pageSize, includes, p => p.Date, OrderBy.Ascending);

            var CommentsDTO = OMapper.Mapper.Map<IEnumerable<DomainModels.DTO.CommentDTO>>(comments);
            return CommentsDTO.ToList();
        }
        public async Task<List<DomainModels.DTO.CommentDTO>> GetQuestionCommentsAsync(Guid postId, DateTime? Time)
        {
            string[] includes = { "QuestionCommentPhoto", "QuestionCommentVedio", "QuestionCommentReacts", "UserAccounts" };
            //var (take, skip) = BussnissHelper.GetTakeSkipValues(pageNumber, _pageSize);
            IEnumerable<QuestionComment> comments;
            if (Time == null)
                comments = await _unitOfWork.QuestionComment.FindAllAsync(p => p.QuestionPostId == postId, _pageSize, 0, includes, p => p.Date, OrderBy.Ascending);
            else
                comments = await _unitOfWork.QuestionComment.FindAllAsync(p => p.QuestionPostId == postId && p.Date > Time, 0, _pageSize, includes, p => p.Date, OrderBy.Ascending);
            var CommentsDTO = OMapper.Mapper.Map<IEnumerable<DomainModels.DTO.CommentDTO>>(comments);
            return CommentsDTO.ToList();
        }
        public async Task<DomainModels.DTO.CommentDTO> GetPostCommentByIdAsync(Guid commentId)
        {
            string[] includes = { "PostCommentPhoto", "PostCommentVedio", "PostCommentReacts", "UserAccounts" };
            var comment = await _unitOfWork.PostComment.FindAsync(c => c.Id == commentId, includes);
            var CommentsDTO = OMapper.Mapper.Map<DomainModels.DTO.CommentDTO>(comment);
            return CommentsDTO;
        }
        public async Task<DomainModels.DTO.CommentDTO> GetQuestionCommentByIdAsync(Guid commentId)
        {
            string[] includes = { "QuestionCommentPhoto", "QuestionCommentVedio", "QuestionCommentReacts", "UserAccounts" };
            var comment = await _unitOfWork.QuestionComment.FindAsync(c => c.Id == commentId, includes);
            var CommentsDTO = OMapper.Mapper.Map<DomainModels.DTO.CommentDTO>(comment);
            return CommentsDTO;
        }
        public async Task<int> GetPostCommentCount(Guid postId)
        {
            var CommentCount = await _unitOfWork.PostComment.CountAsync(C => C.PostId == postId);
            return CommentCount;
        }
        public async Task<int> GetQuestionPostCommentCount(Guid postId)
        {
            var CommentCount = await _unitOfWork.QuestionComment.CountAsync(C => C.QuestionPostId == postId);
            return CommentCount;
        }
    }

}
