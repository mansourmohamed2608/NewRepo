using BDataBase.Core.Models.Accounts;
using DataBase.Core.Models.Accounts;
using DataBase.Core;
using Microsoft.AspNetCore.Http;
using DomainModels.Models;
using Business.Helper;
using Business.Services;
using DomainModels;
using System.Xml.Linq;
using DomainModels.DTO;
using DataBase.Core.Enums;

namespace Business.Implementation
{
    public class AcountService : IAcountService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AcountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(bool, Guid)> AproveFriendRequest(Guid friendRequestId)
        {
            var friendRequest = await _unitOfWork.FriendRequests.FindAsync(f => f.Id == friendRequestId);
            if (friendRequest == null)
                return (false, default);
            await _unitOfWork.Friends.AddAsync(new Friend
            {
                Id = Guid.NewGuid(),
                ApprovedTime = DateTime.UtcNow,
                FirstUser = friendRequest.Requestor,
                FirstUserId = friendRequest.RequestorId,
                SecondUser = friendRequest.Receiver,
                SecondUserId = friendRequest.ReceiverId
            });
            _unitOfWork.FriendRequests.Delete(friendRequest);
            await _unitOfWork.Complete();
            return (true, friendRequest.RequestorId);
        }
        public async Task<bool> SendFriendRequest(Guid senderId, Guid recevierId)
        {
            var sender = await _unitOfWork.UserAccounts.FindAsync(u => u.Id == senderId);
            var recevier = await _unitOfWork.UserAccounts.FindAsync(u => u.Id == recevierId);
            if (sender == null || recevier == null)
                return false;
            //case already a friend 
            var isSendReqBefor = _unitOfWork.FriendRequests.Count(f => f.ReceiverId == recevierId && f.RequestorId == senderId);
            if (isSendReqBefor > 0)
                return true;
            await _unitOfWork.FriendRequests.AddAsync(new FriendRequest
            {
                Id = Guid.NewGuid(),
                ReceiverId = recevier.Id,
                Receiver = recevier,
                RequestorId = sender.Id,
                Requestor = sender
            });
            await _unitOfWork.Complete();

            return true;
        }
        public async Task<bool> DeneyFriendRequest(Guid friendRequestId)
        {
            var friendRequest = await _unitOfWork.FriendRequests.FindAsync(f => f.Id == friendRequestId);
            if (friendRequest == null)
                return false;
            _unitOfWork.FriendRequests.Delete(friendRequest);
            await _unitOfWork.Complete();
            return true;
        }
        public async Task<bool> DeleteFriend(Guid userId, Guid friendId)
        {
            var friendEntity = await _unitOfWork.Friends.FindAsync(f =>
                                f.FirstUserId == userId && f.SecondUserId == friendId ||
                                f.FirstUserId == friendId && f.SecondUserId == userId);
            if (friendEntity == null) return false;
            _unitOfWork.Friends.Delete(friendEntity);
            await _unitOfWork.Complete();
            return true;

        }
        public async Task<UserAccounts> GetAccountProfileAsync(Guid userId)
        {
            //string[] includes = { "Posts", "QuestionPosts" , "CoverPhoto", "ProfilePhoto" };
            var accountProfile = await _unitOfWork.UserAccounts.FindAllAsync(p => p.Id == userId /*,includes*/);
            return accountProfile.FirstOrDefault();
        }
        public async Task<List<SearchAccount>> SearchAccountsAsync(string searchValue)
        {
            string[] includes = { "ProfilePhoto" };
            var accountProfile = await _unitOfWork.UserAccounts.FindAllAsync(p => p.Email.ToLower() == searchValue || p.UserName.ToLower() == searchValue || p.FirstName.Contains(searchValue) || (p.FirstName + p.LastName).ToLower().Contains(searchValue), includes);
            var searchResult = new List<SearchAccount>();
            foreach (var account in accountProfile)
            {
                searchResult.Add(new SearchAccount
                {
                    Id = account.Id,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    ProfilePhoto = account.ProfilePhoto.PhotoPath
                });
            }
            return searchResult;
        }
        public async Task<bool> UpdateAccountProfileAsync(DomainModels.Models.ProfileUpdateModel profileUpdateModel)
        {
            var profileAccount = await _unitOfWork.UserAccounts.FindAsync(p => p.Id == profileUpdateModel.Id);
            profileAccount.FirstName = profileUpdateModel.FirstName;
            profileAccount.LastName = profileUpdateModel.LastName;
            profileAccount.City = profileUpdateModel.City;
            profileAccount.Country = profileUpdateModel.Country;
            profileAccount.Bio = profileUpdateModel.Bio;
            profileAccount.Birthdate = profileUpdateModel.Birthdate;
            profileAccount.gender = profileUpdateModel.gender;
            _unitOfWork.UserAccounts.Update(profileAccount);
            var UpdateResult = _unitOfWork.Complete();
            return await UpdateResult > 0;
        }
        public async Task<bool> UpdateProfilePhotoAsync(IFormFile formFile, string email)
        {
            string[] includes = { "ProfilePhoto" };
            var profileAccount = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == email, includes);
            if (profileAccount == null) return false;
            var photoPath = AccountHelpers.IformToProfilePath(formFile, profileAccount.Id);
            if (profileAccount.ProfilePhoto != null)
                _unitOfWork.ProfilePhoto.Delete(profileAccount.ProfilePhoto);
            var ProfilePhoto = new DataBase.Core.Models.PhotoModels.ProfilePhoto
            {
                Id = Guid.NewGuid(),
                PhotoPath = photoPath,
                UserAccountsId = profileAccount.Id
            };
            _unitOfWork.ProfilePhoto.Add(ProfilePhoto);
            var result = await _unitOfWork.Complete();
            return result > 0;

        }
        public async Task<bool> UpdateProfileCoverAsync(IFormFile formFile, string email)
        {
            string[] includes = { "CoverPhoto" };
            var profileAccount = await _unitOfWork.UserAccounts.FindAsync(p => p.Email == email, includes);
            if (profileAccount == null) return false;
            var photoPath = AccountHelpers.IformToCoverPath(formFile, profileAccount.Id);
            if (profileAccount.CoverPhoto != null)
                _unitOfWork.CoverPhoto.Delete(profileAccount.CoverPhoto);
            var CoverPhoto = new DataBase.Core.Models.PhotoModels.CoverPhoto
            {
                Id = Guid.NewGuid(),
                PhotoPath = photoPath,
                UserAccountsId = profileAccount.Id
            };
            _unitOfWork.CoverPhoto.Add(CoverPhoto);
            var result = await _unitOfWork.Complete();
            return result > 0;
        }
        public async Task<List<DomainModels.DTO.FriendRequestUserAccount>> GetPendingFriendRequest(Guid userId)
        {
            string[] includes = { "Requestor" };
            var pendingList = await _unitOfWork.FriendRequests.FindAllAsync(f => f.ReceiverId == userId, includes);
            var userAccounts = OMapper.Mapper.Map<List<DomainModels.DTO.FriendRequestUserAccount>>(pendingList);
            return userAccounts;
        }
        public async Task<List<DomainModels.DTO.UserAccount>> GetAllUserFreinds(Guid userId)
        {
            string[] includes = { "FirstUser", "SecondUser" };
            var frindsList = await _unitOfWork.Friends.FindAllAsync(f => f.FirstUserId == userId || f.SecondUserId == userId, includes);
            List<UserAccount> users = new List<UserAccount>();
            foreach (var frind in frindsList)
            {
                var userDto = new UserAccount();
                if (userId == frind.FirstUserId)
                {
                    userDto.Id = frind.SecondUserId;
                    userDto.UserName = frind.SecondUser.UserName;
                    userDto.FirstName = frind.SecondUser.FirstName;
                    userDto.LastName = frind.SecondUser.LastName;
                    userDto.Birthdate = frind.SecondUser.Birthdate;
                    userDto.Bio = frind.SecondUser.Bio;
                    userDto.City = frind.SecondUser.City;
                    userDto.Country = frind.SecondUser.Country;
                }
                else
                {
                    userDto.Id = frind.FirstUserId;
                    userDto.UserName = frind.FirstUser.UserName;
                    userDto.FirstName = frind.FirstUser.FirstName;
                    userDto.LastName = frind.FirstUser.LastName;
                    userDto.Birthdate = frind.FirstUser.Birthdate;
                    userDto.Bio = frind.FirstUser.Bio;
                    userDto.City = frind.FirstUser.City;
                    userDto.Country = frind.FirstUser.Country;
                }
                users.Add(userDto);
            }
            return users;
        }

        public async Task<FriendRelation> GettwoUserFriendRelation(Guid FirstUserId, Guid secondUserId)
        {
            var isFriends =  _unitOfWork.Friends.FindAll(f => (f.FirstUserId == FirstUserId && f.SecondUserId == secondUserId) || (f.FirstUserId == secondUserId && f.SecondUserId == FirstUserId)).FirstOrDefault();
            if (isFriends != null)
                return FriendRelation.Friends;
            var pendingFriends = _unitOfWork.FriendRequests.FindAll(f => (f.RequestorId == FirstUserId && f.ReceiverId == secondUserId) || (f.ReceiverId == secondUserId && f.RequestorId == FirstUserId)).FirstOrDefault();
            if(pendingFriends != null)
                return FriendRelation.PendingFriends;
            return FriendRelation.NotFriends;
        }
    }
}
