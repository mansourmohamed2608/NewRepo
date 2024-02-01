using BDataBase.Core.Models.Accounts;
using Business.Helper;
using Business.Services;
using DataBase.Core;
using DataBase.Core.Consts;
using DataBase.Core.Enums;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.Posts;
using DataBase.Core.Models.VedioModels;
using DomainModels;
using DomainModels.Models;
using System.Collections.ObjectModel;
using Utilites;

namespace Business.Implementation
{
    public class PostService : IPostService
    {
        private readonly int _pageSize = 5;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentServices _commentServices;
        public PostService(IUnitOfWork unitOfWork, ICommentServices commentServices)
        {
            _unitOfWork = unitOfWork;
            _commentServices = commentServices;
        }

        public async Task<(bool, Guid)> AddPostAsync(UploadPost postmodel, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            if (user == null) return (false, Guid.Empty);
            var newpostmodel = PostHelper.ConvertToPaths(postmodel, SharedFolderPaths.PostPhotos, SharedFolderPaths.PostVideos);
            var post = PreparePostModel(newpostmodel, user);
            await _unitOfWork.Post.AddAsync(post);
            var postID = post.Id;
            var saveresult = await _unitOfWork.Complete();
            return (saveresult > 0, postID);
        }
        public async Task<(bool, Guid)> AddQuestionPostAsync(UploadPost postmodel, string userEmail)
        {
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            if (user == null) return (false, Guid.Empty);
            var newpostmodel = PostHelper.ConvertToPaths(postmodel, SharedFolderPaths.QuestionPhotos, SharedFolderPaths.QuestionVideos);
            var post = PrepareQuestonPostModel(newpostmodel, user);
            await _unitOfWork.QuestionPost.AddAsync(post);
            var saveresult = await _unitOfWork.Complete();
            var PostID = post.Id;
            return (saveresult > 0, PostID);
        }
        public async Task<bool> UpdatePostAsync(UpdataPost postmodel, string userEmail)
        {
            string[] includes = { "Photos", "Vedios", "Reacts", "Comments", "UserAccounts" };
            var post = await _unitOfWork.Post.FindAsync(p => p.Id == postmodel.Id, includes);
            if (post == null)
                return false;
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            if (user == null)
                return false;
            if (post.UserAccountsId != user.Id)
                return false;
            post.Description = postmodel.Description;
            _unitOfWork.Post.Update(post);
            var update = await _unitOfWork.Complete();
            post = await AddNewPostPhotoAndVedioForPost(postmodel, post);
            _unitOfWork.Post.Update(post);
            var update2 = await _unitOfWork.Complete();

            await DeletePostPhotoAndVedio(postmodel);

            return true;
        }
        public async Task<bool> UpdateQuestionPostAsync(UpdataPost postmodel, string userEmail)
        {
            var questionpost = await _unitOfWork.QuestionPost.FindAsync(p => p.Id == postmodel.Id);

            if (questionpost == null)
                return false;

            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            if (user == null)
                return false;
            if (questionpost.UserAccountsId != user.Id)
                return false;
            questionpost.Question = postmodel.Question;
            questionpost.Answer = postmodel.Answer;
            _unitOfWork.QuestionPost.Update(questionpost);
            var update = await _unitOfWork.Complete();


            questionpost = await AddNewPostPhotoAndVedioForQuestion(postmodel, questionpost);
            _unitOfWork.QuestionPost.Update(questionpost);
            update = await _unitOfWork.Complete();

            await DeletePostPhotoAndVedio(postmodel);

            return update > 0;
        }
        public async Task<bool> DeletePostAsync(Guid id, string userEmail)
        {
            var post = await _unitOfWork.Post.FindAsync(p => p.Id == id);
            if (post == null)
                return false;
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            if (user == null)
                return false;
            if (post.UserAccountsId != user.Id)
                return false;
            _unitOfWork.Post.Delete(post);
            var result = _unitOfWork.Complete();
            if (await result > 0)
                return true;
            return false;

        }
        public async Task<bool> DeleteQuestionPostAsync(Guid id, string userEmail)
        {
            var questionpost = await _unitOfWork.QuestionPost.FindAsync(q => q.Id == id);
            if (questionpost == null)
                return false;
            var user = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == userEmail);
            if (user == null)
                return false;
            if (questionpost.UserAccountsId != user.Id)
                return false;
            _unitOfWork.QuestionPost.Delete(questionpost);
            var result = _unitOfWork.Complete();
            if (await result > 0)
                return true;
            return false;
        }
        private QuestionPost PrepareQuestonPostModel(AllPostsModel postmodel, UserAccounts user, QuestionPost post = null)
        {
            if (post == null)
            {
                post = new QuestionPost();
                post.Id = Guid.NewGuid();
                post.UserAccounts = user;
                post.UserAccountsId = user.Id;
            }

            if (post.Photos == null)
                post.Photos = new Collection<QuestionPhoto>();
            if (postmodel.PhotosPath != null)
                foreach (var photo in postmodel.PhotosPath)
                {
                    var photoModel = new QuestionPhoto()
                    {
                        Id = Guid.NewGuid(),
                        PhotoPath = photo,
                        QuestionPost = post,
                        QuestionId = post.Id
                    };
                    post.Photos.Add(photoModel);
                }
            if (post.Vedios == null)
                post.Vedios = new Collection<QuestionVedio>();
            if (postmodel.VediosPath != null)
                foreach (var vedio in postmodel.VediosPath)
                {
                    var vedioModel = new QuestionVedio()
                    {
                        Id = Guid.NewGuid(),
                        VedioPath = vedio,
                        QuestionPost = post,
                        QuestionPostId = post.Id
                    };
                    post.Vedios.Add(vedioModel);
                }
            //post.Title = postmodel.Title;
            //post.Description = postmodel.Description;
            post.Answer = postmodel.Answer;
            post.Question = postmodel.Question;
            return post;
        }
        private Post PreparePostModel(AllPostsModel postmodel, UserAccounts user, Post post = null)
        {

            if (post == null)
            {
                post = new Post();
                post.Id = Guid.NewGuid();
                post.UserAccounts = user;
                post.UserAccountsId = user.Id;
            }
            if (post.Photos == null)
                post.Photos = new Collection<PostPhoto>();
            if (postmodel.PhotosPath != null)
                foreach (var photo in postmodel.PhotosPath)
                {
                    var photoModel = new PostPhoto()
                    {
                        Id = Guid.NewGuid(),
                        PhotoPath = photo,
                        Post = post,
                        PostId = post.Id
                    };
                    post.Photos.Add(photoModel);
                }
            if (post.Vedios == null)
                post.Vedios = new Collection<PostVedio>();
            if (postmodel.VediosPath != null)
                foreach (var vedio in postmodel.VediosPath)
                {
                    var vedioModel = new PostVedio()
                    {
                        Id = Guid.NewGuid(),
                        VedioPath = vedio,
                        Post = post,
                        PostId = post.Id
                    };
                    post.Vedios.Add(vedioModel);
                }
            //post.Title = postmodel.Title;
            post.Description = postmodel.Description;
            return post;
        }
        private async Task<Post> AddNewPostPhotoAndVedioForPost(UpdataPost postmodel, Post post)
        {
            if (postmodel.NewPhotos != null)
            {
                if (post.Photos == null)
                    post.Photos = new List<PostPhoto>();
                foreach (var item in postmodel.NewPhotos)
                {
                    var path = MediaUtilites.ConverIformToPath(item, SharedFolderPaths.PostPhotos);
                    var photo = new PostPhoto
                    {
                        PhotoPath = path,
                        Post = post,
                        PostId = post.Id
                    };
                    post.Photos.Add(photo);

                }
            }
            if (postmodel.NewVedios != null)
            {
                if (post.Vedios == null)
                    post.Vedios = new List<PostVedio>();
                foreach (var item in postmodel.NewVedios)
                {
                    var path = MediaUtilites.ConverIformToPath(item, SharedFolderPaths.PostVideos);
                    var vedio = new PostVedio
                    {
                        VedioPath = path,
                        Post = post,
                        PostId = post.Id
                    };
                    post.Vedios.Add(vedio);
                }
            }
            return post;
        }
        private async Task<QuestionPost> AddNewPostPhotoAndVedioForQuestion(UpdataPost postmodel, QuestionPost questionPost)
        {
            if (postmodel.NewPhotos != null)
            {
                if (questionPost.Photos == null)
                    questionPost.Photos = new List<QuestionPhoto>();
                foreach (var item in postmodel.NewPhotos)
                {
                    var path = MediaUtilites.ConverIformToPath(item, SharedFolderPaths.QuestionPhotos);
                    var photo = new QuestionPhoto
                    {
                        PhotoPath = path,
                        QuestionId = questionPost.Id,
                        QuestionPost = questionPost
                    };
                    questionPost.Photos.Add(photo);

                }
            }
            if (postmodel.NewVedios != null)
            {
                if (questionPost.Vedios == null)
                    questionPost.Vedios = new List<QuestionVedio>();
                foreach (var item in postmodel.NewVedios)
                {
                    var path = MediaUtilites.ConverIformToPath(item, SharedFolderPaths.QuestionVideos);
                    var vedio = new QuestionVedio
                    {

                        VedioPath = path,
                        QuestionPostId = questionPost.Id,
                        QuestionPost = questionPost
                    };
                    questionPost.Vedios.Add(vedio);
                }
            }
            return questionPost;

        }
        private async Task DeletePostPhotoAndVedio(UpdataPost postmodel)
        {
            switch (postmodel.Type)
            {
                case PostsTypes.Post:
                    if (postmodel.DeletedPhotoIds != null)
                    {
                        foreach (var photoID in postmodel.DeletedPhotoIds)
                        {

                            var photo = await _unitOfWork.PostPhoto.FindAsync(p => p.Id == photoID);
                            if (photo != null)
                            {
                                MediaUtilites.DeleTeMediaPath(photo.PhotoPath);
                                _unitOfWork.PostPhoto.Delete(photo);

                            }
                        }
                    }
                    if (postmodel.DeletedVedioIds != null)
                    {
                        foreach (var delVedioID in postmodel.DeletedVedioIds)
                        {
                            var vedio = await _unitOfWork.PostVedio.FindAsync(p => p.Id == delVedioID);
                            if (vedio != null)
                            {
                                MediaUtilites.DeleTeMediaPath(vedio.VedioPath);
                                _unitOfWork.PostVedio.Delete(vedio);
                            }
                        }
                    }
                    break;
                case PostsTypes.Question:
                    if (postmodel.DeletedPhotoIds != null)
                    {
                        foreach (Guid photoID2 in postmodel.DeletedPhotoIds)
                        {
                            var photo = await _unitOfWork.QuestionPhoto.FindAsync(p => p.Id == photoID2);
                            if (photo != null)
                            {
                                MediaUtilites.DeleTeMediaPath(photo.PhotoPath);
                                _unitOfWork.QuestionPhoto.Delete(photo);
                            }
                        }
                    }
                    if (postmodel.DeletedVedioIds != null)
                    {
                        foreach (var VedioId in postmodel.DeletedVedioIds)
                        {
                            var vedio = await _unitOfWork.QuestionVedio.FindAsync(p => p.Id == VedioId);
                            if (vedio != null)
                            {
                                MediaUtilites.DeleTeMediaPath(vedio.VedioPath);
                                _unitOfWork.QuestionVedio.Delete(vedio);
                            }
                        }
                    }
                    break;
                default: break;


            }
            var result = await _unitOfWork.Complete();
        }
        //ALL NEW HERE
        public async Task<List<DomainModels.DTO.PostDTO>> GetPostAsync(DateTime? Time)
        {
            //var (take, skip) = BussnissHelper.GetTakeSkipValues(pageNumber, _pageSize);
            string[] includes = { "Photos", "Vedios", "Reacts", "UserAccounts" };
            var posts = await _unitOfWork.Post.FindAllAsync(p => Time == null || p.TimeCreated < Time, _pageSize, 0, includes, p => p.TimeCreated, OrderBy.Descending);
            var PostsDTO = OMapper.Mapper.Map<List<DomainModels.DTO.PostDTO>>(posts);
            foreach (var post in PostsDTO)
            {
                var commentCount = await _commentServices.GetPostCommentCount(post.Id);
                var CommentDtoList = await _commentServices.GetPostCommentsAsync(post.Id, null);
                post.commentsCount = commentCount;
                post.Comments = CommentDtoList;
            }
            return PostsDTO;
        }
        public async Task<List<DomainModels.DTO.QuestionPostDTO>> GetQuestionPostAsync(DateTime? Time)
        {
            //var (take, skip) = BussnissHelper.GetTakeSkipValues(pageNumber,_pageSize);
            string[] includes = { "Photos", "Vedios", "Reacts", "UserAccounts" };
            var posts = await _unitOfWork.QuestionPost.FindAllAsync(p => Time == null || p.TimeCreated < Time, _pageSize, 0, includes, p => p.TimeCreated, OrderBy.Descending);
            var QuestionPostsDTO = OMapper.Mapper.Map<List<DomainModels.DTO.QuestionPostDTO>>(posts);
            foreach (var questionPost in QuestionPostsDTO)
            {
                var commentCount = await _commentServices.GetQuestionPostCommentCount(questionPost.Id);
                var CommentDtoList = await _commentServices.GetQuestionCommentsAsync(questionPost.Id, null);
                questionPost.commentsCount = commentCount;
                questionPost.Comments = CommentDtoList;
            }
            return QuestionPostsDTO;
        }
        public async Task<List<DomainModels.DTO.AllPostDTO>> GetPostTypesAsync(DateTime? PostTime, DateTime? QuestionTime)
        {
            var posts = await GetPostAsync(PostTime);
            var QPosts = await GetQuestionPostAsync(QuestionTime);
            var AllPostDTO = OMapper.Mapper.Map<List<DomainModels.DTO.AllPostDTO>>(posts);
            var AllQuestionDTO = OMapper.Mapper.Map<List<DomainModels.DTO.AllPostDTO>>(QPosts);
            AllPostDTO.AddRange(AllQuestionDTO);
            return AllPostDTO.OrderByDescending(p => p.Time).ToList();
        }
        public async Task<DomainModels.DTO.PostDTO> GetPostByIDAsync(Guid id)
        {
            string[] includes = { "Photos", "Vedios", "Reacts", "UserAccounts" };
            var post = await _unitOfWork.Post.FindAsync(p => p.Id == id, includes);
            var PostDTO = OMapper.Mapper.Map<DomainModels.DTO.PostDTO>(post);
            var commentCount = await _commentServices.GetPostCommentCount(post.Id);
            var CommentDtoList = await _commentServices.GetPostCommentsAsync(post.Id, null);
            PostDTO.commentsCount = commentCount;
            PostDTO.Comments = CommentDtoList;
            return PostDTO;
        }
        public async Task<DomainModels.DTO.QuestionPostDTO> GetQuestionPostByIdAsync(Guid id)
        {
            string[] includes = { "Photos", "Vedios", "Reacts", "UserAccounts" };
            var post = await _unitOfWork.QuestionPost.FindAsync(p => p.Id == id, includes);
            var PostDTO = OMapper.Mapper.Map<DomainModels.DTO.QuestionPostDTO>(post);
            var commentCount = await _commentServices.GetQuestionPostCommentCount(post.Id);
            var CommentDtoList = await _commentServices.GetQuestionCommentsAsync(post.Id, null);
            PostDTO.commentsCount = commentCount;
            PostDTO.Comments = CommentDtoList;
            return PostDTO;
        }
        public async Task<List<DomainModels.DTO.PostDTO>> GetPersonalPostAsync(DateTime? Time, Guid userID)
        {
            //var (take, skip) = BussnissHelper.GetTakeSkipValues(pageNumber, _pageSize);
            string[] includes = { "Photos", "Vedios", "Reacts", "UserAccounts" };
            var posts = await _unitOfWork.Post.FindAllAsync(p =>p.UserAccountsId == userID &&(Time == null || p.TimeCreated < Time), _pageSize, 0, includes, p => p.TimeCreated, OrderBy.Descending);
            var PostsDTO = OMapper.Mapper.Map<List<DomainModels.DTO.PostDTO>>(posts);
            foreach (var post in PostsDTO)
            {
                var commentCount = await _commentServices.GetPostCommentCount(post.Id);
                var CommentDtoList = await _commentServices.GetPostCommentsAsync(post.Id, null);
                post.commentsCount = commentCount;
                post.Comments = CommentDtoList;
            }
            return PostsDTO;
        }
        public async Task<List<DomainModels.DTO.QuestionPostDTO>> GetPersonalQuestionPostAsync(DateTime? Time, Guid userID)
        {
            //var (take, skip) = BussnissHelper.GetTakeSkipValues(pageNumber, _pageSize);
            string[] includes = { "Photos", "Vedios", "Reacts", "UserAccounts" };
            var posts = await _unitOfWork.QuestionPost.FindAllAsync(p => p.UserAccountsId == userID && (Time == null || p.TimeCreated < Time), _pageSize, 0, includes, p => p.TimeCreated, OrderBy.Descending);
            var QuestionPostsDTO = OMapper.Mapper.Map<List<DomainModels.DTO.QuestionPostDTO>>(posts);
            foreach (var questionPost in QuestionPostsDTO)
            {
                var commentCount = await _commentServices.GetQuestionPostCommentCount(questionPost.Id);
                var CommentDtoList = await _commentServices.GetQuestionCommentsAsync(questionPost.Id, null);
                questionPost.commentsCount = commentCount;
                questionPost.Comments = CommentDtoList;
            }
            return QuestionPostsDTO;
        }
        public async Task<List<DomainModels.DTO.AllPostDTO>> GetPersonalPostTypesAsync(DateTime? PostTime, DateTime? QuestionTime, Guid userID)
        {

            var posts = await GetPersonalPostAsync(PostTime, userID);
            var questions = await GetPersonalQuestionPostAsync(QuestionTime, userID);
            var AllPostDTO = OMapper.Mapper.Map<List<DomainModels.DTO.AllPostDTO>>(posts);
            var AllQuestionDTO = OMapper.Mapper.Map<List<DomainModels.DTO.AllPostDTO>>(questions);
            AllPostDTO.AddRange(AllQuestionDTO);
            return AllPostDTO.OrderByDescending(p => p.Time).ToList();
        }

    }
}
