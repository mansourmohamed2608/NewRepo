using BDataBase.Core.Models.Accounts;
using Business.Services;
using DataBase.Core;
using DataBase.Core.Models.CommentModels;
using DataBase.Core.Models.Reacts;
using DomainModels;
using DomainModels.DTO;
using DomainModels.Models;

namespace Business.Implementation
{
    public class ReactsServices : IReactServices
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReactsServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool, Guid)> AddReactOnPostAsync(AddReactRequest reactRequest, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var post = await _unitOfWork.Post.FindAsync(p => p.Id == reactRequest.ObjectId);
            if (user == null || post == null) return (false, Guid.Empty);
            var react = new PostReact
            {
                Id = Guid.NewGuid(),
                PostId = post.Id,
                react = reactRequest.ReactType,
                UserAccountsId = user.Id
            };
            await _unitOfWork.PostReact.AddAsync(react);
            return (await _unitOfWork.Complete() > 0, react.Id);
        }

        public async Task<(bool, Guid)> AddReactOnPostCommentAsync(AddReactRequest reactRequest, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var Comment = await _unitOfWork.PostComment.FindAsync(p => p.Id == reactRequest.ObjectId);
            if (user == null || Comment == null) return (false, Guid.Empty);
            var react = new PostCommentReact
            {
                Id = Guid.NewGuid(),
                PostCommentId = Comment.Id,
                react = reactRequest.ReactType,
                UserAccountsId = user.Id
            };
            await _unitOfWork.PostCommentReact.AddAsync(react);
            return (await _unitOfWork.Complete() > 0, react.Id);
        }

        public async Task<(bool, Guid)> AddReactOnQuestionPostAsync(AddReactRequest reactRequest, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var post = await _unitOfWork.QuestionPost.FindAsync(p => p.Id == reactRequest.ObjectId);
            if (user == null || post == null) return (false, Guid.Empty);
            var react = new QuestionReact
            {
                Id = Guid.NewGuid(),
                QuestionPostId = post.Id,
                react = reactRequest.ReactType,
                UserAccountsId = user.Id
            };
            await _unitOfWork.QuestionReact.AddAsync(react);
            return (await _unitOfWork.Complete() > 0, react.Id);
        }

        public async Task<(bool, Guid)> AddReactOnQuestionPostCommentAsync(AddReactRequest reactRequest, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var Comment = await _unitOfWork.QuestionComment.FindAsync(p => p.Id == reactRequest.ObjectId);
            if (user == null || Comment == null) return (false, Guid.Empty);
            var react = new QuestionCommentReact
            {
                Id = Guid.NewGuid(),
                QuestionCommentId = Comment.Id,
                react = reactRequest.ReactType,
                UserAccountsId = user.Id
            };
            await _unitOfWork.QuestionCommentReact.AddAsync(react);
            return (await _unitOfWork.Complete() > 0, react.Id);
        }

        public async Task<bool> DeleteCommentPostReactAsync(Guid reactId, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var react = await _unitOfWork.PostCommentReact.FindAsync(r => r.Id == reactId);
            if (react == null || user == null || react.UserAccountsId != user.Id)
                return false;
            _unitOfWork.PostCommentReact.Delete(react);
            return await _unitOfWork.Complete() > 0;
        }

        public async Task<bool> DeleteCommentQuestionReactAsync(Guid reactId, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var react = await _unitOfWork.QuestionCommentReact.FindAsync(r => r.Id == reactId);
            if (react == null || user == null || react.UserAccountsId != user.Id)
                return false;
            _unitOfWork.QuestionCommentReact.Delete(react);
            return await _unitOfWork.Complete() > 0;
        }

        public async Task<bool> DeletePostReactAsync(Guid reactId, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var react = await _unitOfWork.PostReact.FindAsync(r => r.Id == reactId);
            if (react == null || user == null || react.UserAccountsId != user.Id)
                return false;
            _unitOfWork.PostReact.Delete(react);
            return await _unitOfWork.Complete() > 0;
        }

        public async Task<bool> DeleteQuestionPostReactAsync(Guid reactId, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var react = await _unitOfWork.QuestionReact.FindAsync(r => r.Id == reactId);
            if (react == null || user == null || react.UserAccountsId != user.Id)
                return false;
            _unitOfWork.QuestionReact.Delete(react);
            return await _unitOfWork.Complete() > 0;
        }

        public async Task<List<ReactsDTO>> GetPostCommentReacts(Guid postCommentId)
        {
            string[] includes = { "UserAccounts" };
            var reacts = await _unitOfWork.PostCommentReact.FindAllAsync(r => r.PostCommentId == postCommentId, includes);
            var reactDTO = OMapper.Mapper.Map<List<ReactsDTO>>(reacts);
            return reactDTO;
        }

        public async Task<List<ReactsDTO>> GetPostReacts(Guid postId)
        {
            string[] includes = { "UserAccounts" };
            var reacts = await _unitOfWork.PostReact.FindAllAsync(r => r.PostId == postId, includes);
            var reactDTO = OMapper.Mapper.Map<List<ReactsDTO>>(reacts);
            return reactDTO;
        }

        public async Task<List<ReactsDTO>> GetQuestionCommentReacts(Guid questionPostCommentId)
        {
            string[] includes = { "UserAccounts" };
            var reacts = await _unitOfWork.QuestionCommentReact.FindAllAsync(r => r.QuestionCommentId == questionPostCommentId, includes);
            var reactDTO = OMapper.Mapper.Map<List<ReactsDTO>>(reacts);
            return reactDTO;
        }

        public async Task<List<ReactsDTO>> GetQuestionReacts(Guid questionId)
        {
            string[] includes = { "UserAccounts" };
            var reacts = await _unitOfWork.QuestionReact.FindAllAsync(r => r.QuestionPostId == questionId, includes);
            var reactDTO = OMapper.Mapper.Map<List<ReactsDTO>>(reacts);
            return reactDTO;
        }

        public async Task<ReactsDTO> GetReactByIdOnQuestionPost(Guid reactId)
        {
            string[] includes = { "UserAccounts" };
            var react = await _unitOfWork.QuestionReact.FindAsync(r => r.Id == reactId, includes);
            var reactDTO = OMapper.Mapper.Map<ReactsDTO>(react);
            return reactDTO;
        }
        public async Task<ReactsDTO> GetReactByIdOnPost(Guid reactId)
        {
            string[] includes = { "UserAccounts" };
            var react = await _unitOfWork.PostReact.FindAsync(r => r.Id == reactId, includes);
            var reactDTO = OMapper.Mapper.Map<ReactsDTO>(react);
            return reactDTO;
        }
        public async Task<ReactsDTO> GetReactByIdOnPostComment(Guid reactId)
        {
            string[] includes = { "UserAccounts" };
            var react = await _unitOfWork.PostCommentReact.FindAsync(r => r.Id == reactId, includes);
            var reactDTO = OMapper.Mapper.Map<ReactsDTO>(react);
            return reactDTO;
        }
        public async Task<ReactsDTO> GetReactByIdOnQuestionComment(Guid reactId)
        {
            string[] includes = { "UserAccounts" };
            var react = await _unitOfWork.QuestionCommentReact.FindAsync(r => r.Id == reactId, includes);
            var reactDTO = OMapper.Mapper.Map<ReactsDTO>(react);
            return reactDTO;
        }

        public async Task<bool> UpdatePostCommentReact(ReactUpdateRequest reactRequest, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var react = await _unitOfWork.PostCommentReact.FindAsync(r => r.Id == reactRequest.ReactId);
            if (react == null || user == null || react.UserAccountsId != user.Id)
                return false;
            react.react = reactRequest.ReactType;
            _unitOfWork.PostCommentReact.Update(react);
            return await _unitOfWork.Complete() > 0;
        }

        public async Task<bool> UpdatePostReact(ReactUpdateRequest reactRequest, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var react = await _unitOfWork.PostReact.FindAsync(r => r.Id == reactRequest.ReactId);
            if (react == null || user == null || react.UserAccountsId != user.Id)
                return false;
            react.react = reactRequest.ReactType;
            _unitOfWork.PostReact.Update(react);
            return await _unitOfWork.Complete() > 0;
        }

        public async Task<bool> UpdateQuestionCommentReact(ReactUpdateRequest reactRequest, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var react = await _unitOfWork.QuestionCommentReact.FindAsync(r => r.Id == reactRequest.ReactId);
            if (react == null || user == null || react.UserAccountsId != user.Id)
                return false;
            react.react = reactRequest.ReactType;
            _unitOfWork.QuestionCommentReact.Update(react);
            return await _unitOfWork.Complete() > 0;
        }

        public async Task<bool> UpdateQuestionReact(ReactUpdateRequest reactRequest, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            var react = await _unitOfWork.QuestionReact.FindAsync(r => r.Id == reactRequest.ReactId);
            if (react == null || user == null || react.UserAccountsId != user.Id)
                return false;
            react.react = reactRequest.ReactType;
            _unitOfWork.QuestionReact.Update(react);
            return await _unitOfWork.Complete() > 0;
        }
    }
}
