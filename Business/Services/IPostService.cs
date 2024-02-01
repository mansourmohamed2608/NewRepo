using DomainModels.Models;

namespace Business.Services
{
    public interface IPostService
    {
        Task<List<DomainModels.DTO.PostDTO>> GetPostAsync(DateTime? Time);
        Task<List<DomainModels.DTO.QuestionPostDTO>> GetQuestionPostAsync(DateTime? Time);
        Task<List<DomainModels.DTO.AllPostDTO>> GetPostTypesAsync(DateTime? PostTime, DateTime? QuestionTime);
        Task<DomainModels.DTO.PostDTO> GetPostByIDAsync(Guid id);
        Task<DomainModels.DTO.QuestionPostDTO> GetQuestionPostByIdAsync(Guid id);
        Task<List<DomainModels.DTO.PostDTO>> GetPersonalPostAsync(DateTime? Time, Guid userID);
        Task<List<DomainModels.DTO.QuestionPostDTO>> GetPersonalQuestionPostAsync(DateTime? Time, Guid userID);
        Task<List<DomainModels.DTO.AllPostDTO>> GetPersonalPostTypesAsync(DateTime? PostTime, DateTime? QuestionTime, Guid userID);

        Task<(bool, Guid)> AddQuestionPostAsync(UploadPost postmodel, string userEmail);
        Task<(bool, Guid)> AddPostAsync(UploadPost postmodel, string userEmail);
        Task<bool> UpdatePostAsync(UpdataPost postmodel, string userEmail);
        Task<bool> UpdateQuestionPostAsync(UpdataPost postmodel, string userEmail);
        Task<bool> DeletePostAsync(Guid id, string userEmail);
        Task<bool> DeleteQuestionPostAsync(Guid id, string userEmail);

    }
}
