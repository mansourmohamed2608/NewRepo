using DomainModels.Models;

namespace Business.Services
{
    public interface ICommentServices
    {
        Task<bool> DeletePostCommentAsync(Guid commentId, string userEmail);
        Task<(bool, Guid)> AddPostCommentAsync(AddCommentRequest comment, string userEmail);
        Task<bool> UpdatePostCommentAsync(CommentUpdateRequest comment, string userEmail);
        Task<bool> DeleteQuestionCommentAsync(Guid commentId, string userEmail);
        Task<(bool, Guid)> AddQuestionCommentAsync(AddCommentRequest comment, string userEmail);
        Task<bool> UpdateQuestionCommentAsync(CommentUpdateRequest comment, string userEmail);
        Task<List<DomainModels.DTO.CommentDTO>> GetPostCommentsAsync(Guid postId, DateTime? Time);
        Task<List<DomainModels.DTO.CommentDTO>> GetQuestionCommentsAsync(Guid postId, DateTime? Time);
        Task<DomainModels.DTO.CommentDTO> GetPostCommentByIdAsync(Guid commentId);
        Task<DomainModels.DTO.CommentDTO> GetQuestionCommentByIdAsync(Guid commentId);
        Task<int> GetPostCommentCount(Guid postId);
        Task<int> GetQuestionPostCommentCount(Guid postId);
    }
}
