using Business;
using Business.Services;
using DataBase.Core.Enums;
using DomainModels.DTO;
using GAMAX.Services.Dto;
using GAMAX.Services.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GAMAX.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPostService _postService;
        private readonly INotificationServices _notificationServices;
        private readonly HubContextNotify _hubContextNotify;
        public PostsController(IHttpContextAccessor httpContextAccessor, IPostService postService, INotificationServices notificationServices, HubContextNotify hubContextNotify)
        {
            _httpContextAccessor = httpContextAccessor;
            _postService = postService;
            _notificationServices = notificationServices;
            _hubContextNotify = hubContextNotify;
        }
        [HttpPost("GetAllPosts")]
        public async Task<IActionResult> GetAllPosts(DateTime? Time)
        {
            var posts = await _postService.GetPostAsync(Time);
            return Ok(posts);
        }
        [HttpPost("GetAllQuestionPosts")]
        public async Task<IActionResult> GetAllQuestionPosts(DateTime? Time)
        {
            var posts = await _postService.GetQuestionPostAsync(Time);
            return Ok(posts);
        }
        [HttpPost("GetPostByID")]
        public async Task<IActionResult> GetPostByID(Guid id)
        {
            var post = await _postService.GetPostByIDAsync(id);
            return Ok(post);
        }
        [HttpPost("GetQuestionByID")]
        public async Task<IActionResult> GetQuestionByID(Guid id)
        {
            var question = await _postService.GetQuestionPostByIdAsync(id);
            return Ok(question);
        }
        [HttpPost("GetAllPostTypes")]
        public async Task<IActionResult> GetAllPostTypes(DateTime? PostTime, DateTime? QuestionTime)
        {
            var Posts = await _postService.GetPostTypesAsync(PostTime, QuestionTime);
            return Ok(Posts);
        }
        [HttpPost("GetAllPersonalPosts")]
        public async Task<IActionResult> GetAllPersonalPosts(DateTime? Time, Guid? userId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var posts = await _postService.GetPersonalPostAsync(Time, userId == null ? userInfo.Uid : (Guid)userId);
            return Ok(posts);
        }
        [HttpPost("GetAllPersonalQuestionPosts")]
        public async Task<IActionResult> GetAllPersonalQuestionPosts(DateTime? Time, Guid? userId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var posts = await _postService.GetPersonalQuestionPostAsync(Time, userId == null ? userInfo.Uid : (Guid)userId);
            return Ok(posts);
        }

        [HttpPost("GetAllPersonalPostTypes")]
        public async Task<IActionResult> GetAllPersonalPostTypes(DateTime? PostTime, DateTime? QuestionTime, Guid? userId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var posts = await _postService.GetPersonalPostTypesAsync(PostTime, QuestionTime, userId == null ? userInfo.Uid : (Guid)userId);
            return Ok(posts);
        }

        [HttpPost("DeletePost")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            return Ok(await _postService.DeletePostAsync(id, userInfo.Email));
        }
        [HttpPost("DeleteQuestionPost")]
        public async Task<IActionResult> DeleteQuestionPost(Guid id)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            return Ok(await _postService.DeleteQuestionPostAsync(id, userInfo.Email));
        }
        [HttpPost("AddPost")]
        public async Task<IActionResult> AddPost([FromForm] DomainModels.DTO.UploadPost postModel)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var uploadPost = new DomainModels.Models.UploadPost
            {
                Type = postModel.Type,
                Photos = postModel.Photos,
                Vedios = postModel.Vedios,
                Description = postModel.Description
            };
            (bool result, Guid id) = await _postService.AddPostAsync(uploadPost, userInfo.Email);
            if (result)
            {
                var post = await _postService.GetPostByIDAsync(id);
                _notificationServices.NotifyOnAddingPost(post);
                return Ok(post);
            }
            return BadRequest(result);
        }
        [HttpPost("AddQuestionPost")]
        public async Task<IActionResult> AddQuestionPost([FromForm] DomainModels.DTO.UploadQuestionPost questionPostModel)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var uploadPost = new DomainModels.Models.UploadPost
            {
                Answer = questionPostModel.Answer,
                Question = questionPostModel.Question,
                Photos = questionPostModel.Photos,
                Vedios = questionPostModel.Vedios,
                Type = questionPostModel.Type,

            };
            (bool result, Guid id) = await _postService.AddQuestionPostAsync(uploadPost, userInfo.Email);
            if (result)
            {
                var post = await _postService.GetQuestionPostByIdAsync(id);
                _notificationServices.NotifyOnAddingQuestion(post);
                return Ok(post);
            }
            return BadRequest(result);
        }

        [HttpPost("UpdateQuestion")]
        public async Task<IActionResult> UpdateQuestion([FromForm] DomainModels.DTO.UpdateQuestion questionPostModel)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var uploadPost = new DomainModels.Models.UpdataPost
            {
                Id = questionPostModel.Id,
                Question = questionPostModel.Question,
                Answer = questionPostModel.Answer,
                DeletedPhotoIds = questionPostModel.DeletedPhotoIds,
                DeletedVedioIds = questionPostModel.DeletedVedioIds,
                NewPhotos = questionPostModel.Photos,
                NewVedios = questionPostModel.Vedios,
                Type = questionPostModel.Type,

            };
            bool result = await _postService.UpdateQuestionPostAsync(uploadPost, userInfo.Email);
            if (result)
            {
                var post = await _postService.GetQuestionPostByIdAsync(questionPostModel.Id);
                return Ok(post);
            }
            return Ok(result);
        }
        [HttpPost("UpdatePost")]
        public async Task<IActionResult> UpdatePost([FromForm] DomainModels.DTO.UpdatePost postModel)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var uploadPost = new DomainModels.Models.UpdataPost
            {
                Id = postModel.Id,
                DeletedPhotoIds = postModel.DeletedPhotoIds,
                DeletedVedioIds = postModel.DeletedVedioIds,
                NewPhotos = postModel.Photos,
                NewVedios = postModel.Vedios,
                Type = postModel.Type,
                Description = postModel.Description,

            };
            bool susscuss = await _postService.UpdatePostAsync(uploadPost, userInfo.Email);
            if (susscuss)
            {
                var post = await _postService.GetPostByIDAsync(uploadPost.Id);
                return Ok(post);
            }
            return BadRequest(susscuss);
        }

    }
}
