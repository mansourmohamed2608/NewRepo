using BDataBase.Core.Models.Accounts;
using DataBase.Core.Enums;
using DomainModels.Models;
using Microsoft.AspNetCore.Http;

namespace Business.Services
{
    public interface IAcountService
    {
        Task<UserAccounts> GetAccountProfileAsync(Guid userId);
        Task<bool> UpdateAccountProfileAsync(ProfileUpdateModel profileUpdateModel);
        Task<bool> UpdateProfilePhotoAsync(IFormFile formFile, string email);
        Task<bool> UpdateProfileCoverAsync(IFormFile formFile, string email);
        Task<List<SearchAccount>> SearchAccountsAsync(string searchValue);
        Task<bool> SendFriendRequest(Guid senderId, Guid recevierId);
        Task<(bool, Guid)> AproveFriendRequest(Guid friendRequestId);
        Task<bool> DeneyFriendRequest(Guid friendRequestId);
        Task<bool> DeleteFriend(Guid userId, Guid friendId);
        Task<List<DomainModels.DTO.FriendRequestUserAccount>> GetPendingFriendRequest(Guid userId);
        Task <List<DomainModels.DTO.UserAccount>>GetAllUserFreinds(Guid userId);
        Task<FriendRelation> GettwoUserFriendRelation(Guid FirstUserId , Guid secondUserId);
    }
}
