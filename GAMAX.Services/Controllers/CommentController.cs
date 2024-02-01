using Business;
using Business.Services;
using DataBase.Core.Enums;
using GAMAX.Services.Dto;
using GAMAX.Services.Hubs;
using Microsoft.AspNetCore.Mvc;

namespace GAMAX.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommentServices _commentServices;
        private readonly INotificationServices _notificationServices;
        private readonly HubContextNotify _hubContextNotify;
        public CommentController(IHttpContextAccessor httpContextAccessor, ICommentServices commentServices, INotificationServices notificationServices, HubContextNotify hubContextNotify)
        {
            _httpContextAccessor = httpContextAccessor;
            _commentServices = commentServices;
            _notificationServices = notificationServices;
            _hubContextNotify = hubContextNotify;
        }
        [HttpPost("GetPostComments")]
        public async Task<IActionResult> GetPostComments(Guid postId, DateTime? Time)
        {
            var comments = await _commentServices.GetPostCommentsAsync(postId, Time);
            return Ok(comments);
        }
        [HttpPost("GetQuestionComments")]
        public async Task<IActionResult> GetQuestionComments(Guid postId, DateTime? Time)
        {
            var comments = await _commentServices.GetQuestionCommentsAsync(postId, Time);
            return Ok(comments);
        }
        [HttpPost("AddPostComment")]
        public async Task<IActionResult> AddPostComment([FromForm] DomainModels.DTO.AddCommentRequest requestModel)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var cmmnt = new DomainModels.Models.AddCommentRequest
            {
                comment = requestModel.comment,
                Photo = requestModel.Photo,
                Vedio = requestModel.Vedio,
                PostId = requestModel.PostId,
            };
            var (result, id) = await _commentServices.AddPostCommentAsync(cmmnt, userInfo.Email);
            if (result)
            {
                var commentDto = await _commentServices.GetPostCommentByIdAsync(id);
                _notificationServices.NotifyOnAddingComment(commentDto, requestModel.PostId, PostsTypes.Post);
                return Ok(commentDto);
            }

            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost("AddQuestionComment")]
        public async Task<IActionResult> AddQuestionComment([FromForm] DomainModels.DTO.AddCommentRequest requestModel)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var cmmnt = new DomainModels.Models.AddCommentRequest
            {
                comment = requestModel.comment,
                Photo = requestModel.Photo,
                Vedio = requestModel.Vedio,
                PostId = requestModel.PostId,
            };
            var (result, id) = await _commentServices.AddQuestionCommentAsync(cmmnt, userInfo.Email);
            if (result)
            {
                var commentDto = await _commentServices.GetQuestionCommentByIdAsync(id);
                _notificationServices.NotifyOnAddingComment(commentDto, requestModel.PostId, PostsTypes.Question);
                return Ok(commentDto);
            }

            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost("DeletePostComment")]
        public async Task<IActionResult> DeletePostComment(Guid commentId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _commentServices.DeletePostCommentAsync(commentId, userInfo.Email);
            if (result)
                return Ok();

            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost("DeleteQuestionComment")]
        public async Task<IActionResult> DeleteQuestionComment(Guid commentId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _commentServices.DeleteQuestionCommentAsync(commentId, userInfo.Email);
            if (result)
                return Ok();

            return BadRequest(new
            {
                Message = "Fail"
            });
        }
        [HttpPost("UpdatePostComment")]
        public async Task<IActionResult> UpdatePostComment([FromForm] DomainModels.DTO.CommentUpdateRequest requestModel)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var cmmnt = new DomainModels.Models.CommentUpdateRequest
            {
                comment = requestModel.comment,
                Photo = requestModel.Photo,
                Vedio = requestModel.Vedio,
                Id = requestModel.Id,
                DeletedPhotoId = requestModel.DeletedPhotoId,
                DeletedVideoId = requestModel.DeletedVedioId

            };
            var result = await _commentServices.UpdatePostCommentAsync(cmmnt, userInfo.Email);
            if (result)
            {
                var commentDto = await _commentServices.GetPostCommentByIdAsync(cmmnt.Id);
                return Ok(commentDto);
            }
            return BadRequest(new
            {
                Message = "Fail"
            });

        }
        [HttpPost("UpdateQuestionComment")]
        public async Task<IActionResult> UpdateQuestionComment([FromForm] DomainModels.DTO.CommentUpdateRequest requestModel)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var cmmnt = new DomainModels.Models.CommentUpdateRequest
            {
                comment = requestModel.comment,
                Photo = requestModel.Photo,
                Vedio = requestModel.Vedio,
                Id = requestModel.Id,
                DeletedPhotoId = requestModel.DeletedPhotoId,
                DeletedVideoId = requestModel.DeletedVedioId

            };
            var result = await _commentServices.UpdateQuestionCommentAsync(cmmnt, userInfo.Email);
            if (result)
            {
                var commentDto = await _commentServices.GetQuestionCommentByIdAsync(cmmnt.Id);
                return Ok(commentDto);
            }
            return BadRequest(new
            {
                Message = "Fail"
            });
        }

    }
}
