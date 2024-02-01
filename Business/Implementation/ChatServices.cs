using Business.Services;
using DataBase.Core;
using DataBase.Core.Consts;
using DataBase.Core.Models;
using DataBase.Core.Models.Accounts;
using DataBase.Core.Models.Notifications;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.VedioModels;
//using DataBase.EF;
using DomainModels;
using DomainModels.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
//using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilites;

namespace Business.Implementation
{
    public class ChatServices : IChatServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAcountService _acountService;
        private readonly UserConnectionManager _userConnectionManager;
        //private readonly ApplicationDbContext _DbContext;
        public ChatServices(IUnitOfWork unitOfWork, IAcountService acountService ,UserConnectionManager userConnectionManager /*, ApplicationDbContext DbContext*/)
        {
            _unitOfWork = unitOfWork;
            _acountService = acountService;
            _userConnectionManager = userConnectionManager;
            //_DbContext = DbContext;

        }
        public async Task<List<ChatDTO>> GetUserChat(Guid firstUser, Guid secondUser)
        {
            string[] includes = { "Vedios", "Photos" };
            var chatForFirstUser =await  _unitOfWork.Chat.FindAllAsync(c=> c.SenderId == firstUser && c.ReciveId==secondUser ,includes);
            var chatForSecondUser =await _unitOfWork.Chat.FindAllAsync(c => c.SenderId == secondUser && c.ReciveId == firstUser,includes);
            var AllChats = chatForFirstUser.ToList();
            AllChats.AddRange(chatForSecondUser.ToList());
            AllChats =  AllChats.OrderByDescending(i=>i.TimeStamp).ToList();
            var ChatDto = OMapper.Mapper.Map<List<DomainModels.DTO.ChatDTO>>(AllChats);
            return ChatDto;
        }

        public async Task<bool> MarkUserChatAsRead(Guid firstUser, Guid secoundUserId)
        {
            var chatForFirstUser = await _unitOfWork.Chat.FindAllAsync(c => c.SenderId == firstUser && c.ReciveId == secoundUserId);
            var chatForSecondUser = await _unitOfWork.Chat.FindAllAsync(c => c.SenderId == secoundUserId && c.ReciveId == firstUser);
            var chatList = chatForFirstUser.ToList();
            for(int i =0;i< chatList.Count;i++)
                chatList[i].Read=true;
            _unitOfWork.Chat.UpdateRange(chatList);
            chatList= chatForSecondUser.ToList();
            for (int i = 0; i < chatList.Count; i++)
                chatList[i].Read = true;
            _unitOfWork.Chat.UpdateRange(chatList);
            return await _unitOfWork.Complete()>0;
        }

        public async Task<(bool, ChatDTO)> SendPrivateMessage(UploadChatDTO uploadChatDTO)
        {
            var listOfPhotos= IformListToPath(uploadChatDTO.Photos,SharedFolderPaths.ChatPhotos);
            var ListOfVideos= IformListToPath(uploadChatDTO.Vedios, SharedFolderPaths.ChatVideos);
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Message = uploadChatDTO.Message,
                ReciveId = uploadChatDTO.ReciveId,
                SenderId = uploadChatDTO.SenderId,
                TimeStamp = DateTime.UtcNow,
            };
            chat = PrepareChatForAdd(chat, listOfPhotos, ListOfVideos);
            await _unitOfWork.Chat.AddAsync(chat) ;
            var result = await _unitOfWork.Complete() > 0;
            if(result)
                return(true, OMapper.Mapper.Map<DomainModels.DTO.ChatDTO>(chat));
            else return(false, null);
        }
        public async Task<(bool, Chat)> DeleteChat(Guid userId, Guid chatId)
        {
            var chat = await _unitOfWork.Chat.FindAsync(c => c.Id == chatId);
            if(chat !=null && userId==chat.SenderId)
                  _unitOfWork.Chat.Delete(chat);
            var result =await _unitOfWork.Complete()>0;
            return (result, chat);
        }
        public async Task<(bool, ChatDTO)> UpdateChat(UpdateChatDTO updateChatDTO ,Guid userId)
        {
            string[] includes = { "Vedios", "Photos" };
            var chat = await _unitOfWork.Chat.FindAsync(c=>c.Id==updateChatDTO.Id, includes);
            if(chat == null || chat.SenderId!=userId)
                return(false, null);
            chat.Message= updateChatDTO.Message;
            if(updateChatDTO.DeletedPhotoIds!=null && updateChatDTO.DeletedPhotoIds.Count() > 0)
            {
                foreach (var id in updateChatDTO.DeletedPhotoIds)
                    chat.Photos.Remove(chat.Photos.Where(p => p.Id == id).First());
            }
            if (updateChatDTO.DeletedVedioIds != null && updateChatDTO.DeletedVedioIds.Count() > 0)
            {
                foreach (var id in updateChatDTO.DeletedVedioIds)
                    chat.Vedios.Remove(chat.Vedios.Where(p => p.Id == id).First());
            }
            _unitOfWork.Chat.Update(chat);
            var result = await _unitOfWork.Complete();
            if (updateChatDTO.Photos != null && updateChatDTO.Photos.Count > 0)
            {
                var photoList = IformListToPath(updateChatDTO.Photos, SharedFolderPaths.ChatPhotos);
                foreach (var photo in photoList)
                {
                    _unitOfWork.ChatPhoto.Add(new ChatPhoto
                    {
                        ChatId = chat.Id,
                        PhotoPath = photo,
                        Id = Guid.NewGuid(),
                    });
                }
            }
            if (updateChatDTO.Vedios != null && updateChatDTO.Vedios.Count > 0)
            {
                var vedioList = IformListToPath(updateChatDTO.Vedios, SharedFolderPaths.ChatVideos);
                foreach (var vedio in vedioList)
                {
                    _unitOfWork.ChatVedio.Add(new ChatVedio
                    {
                        ChatId = chat.Id,
                        VedioPath = vedio,
                        Id = Guid.NewGuid(),
                    });
                }
            }
            result = await _unitOfWork.Complete();
            var chatForDto = await _unitOfWork.Chat.FindAsync(c=>c.Id == chat.Id,includes);
            var chatDTO = OMapper.Mapper.Map<DomainModels.DTO.ChatDTO>(chatForDto);
            return (result>0, chatDTO);
        }
        public async Task<IEnumerable<friendChat>> GetFriendsWithLastMessage(Guid userId)
        {
            var userFriends = await _acountService.GetAllUserFreinds(userId);
            var userChat = await _unitOfWork.Chat.FindAllAsync(c=>c.SenderId == userId);
            var userRevicedChat = await _unitOfWork.Chat.FindAllAsync(c => c.ReciveId == userId);

            var join1 = from friend in userFriends
                        join chatR in userChat
                        on friend.Id equals chatR.ReciveId into chatGroup
                        from chatR in chatGroup.DefaultIfEmpty()  // Perform left join using DefaultIfEmpty
                        orderby chatR?.TimeStamp descending  // Use ?. operator to access properties of chatR safely
                        select new friendChat
                        {
                            UserId = friend.Id,
                            UserName = friend.UserName,
                            FirstName = friend.FirstName,
                            LastName = friend.LastName,
                            Email = friend.Email,
                            Message = chatR?.Message,
                            Read = chatR?.Read,
                            TimeStamp = chatR?.TimeStamp ?? DateTime.MinValue,  // Handle null TimeStamp
                            ReciveId = chatR?.ReciveId ?? Guid.Empty,  // Handle null ReciveId
                            SenderId = chatR?.SenderId ?? Guid.Empty,  // Handle null SenderId
                            LastMessageId=chatR?.Id??Guid.Empty,
                        };

            var join2 = from friend in userFriends
                        join chatS in userRevicedChat
                        on friend.Id equals chatS.SenderId into chatGroup
                        from chatS in chatGroup.DefaultIfEmpty()  // Perform left join using DefaultIfEmpty
                        orderby chatS?.TimeStamp descending  // Use ?. operator to access properties of chatS safely
                        select new friendChat
                        {
                            UserId = friend.Id,
                            UserName = friend.UserName,
                            FirstName = friend.FirstName,
                            LastName = friend.LastName,
                            Email = friend.Email,
                            Message = chatS?.Message,
                            Read = chatS?.Read,
                            TimeStamp = chatS?.TimeStamp ?? DateTime.MinValue,  // Handle null TimeStamp
                            ReciveId = chatS?.ReciveId ?? Guid.Empty,  // Handle null ReciveId
                            SenderId = chatS?.SenderId ?? Guid.Empty,  // Handle null SenderId
                            LastMessageId = chatS?.Id ?? Guid.Empty,
                        };

            var AllJoin = join2.ToList();
            AllJoin.AddRange(join1.ToList());
            var finalResult = AllJoin.OrderByDescending(x => x.TimeStamp).DistinctBy(x=>x.UserId).Select( f=> new friendChat
            {
                Online=IsOnline(f.UserId),
                UserId = f.UserId,
                UserName = f.UserName,
                FirstName = f.FirstName,
                LastName = f.LastName,
                Email = f.Email,
                Message = f.Message,
                Photos = GetChatPhotosByChatId(f.LastMessageId),
                Read = f.Read,
                TimeStamp = f.TimeStamp,
                TimeCreated=TimeHelper.ConvertTimeCreateToString(f?.TimeStamp?? DateTime.MinValue),
                Vedios = GetChatVediosByChatId(f.LastMessageId),
                ReciveId = f.ReciveId,
                SenderId = f.SenderId,
                LastMessageId=f.LastMessageId
            });
            return finalResult;
        }
        private bool IsOnline(Guid userId)
        {
            if (!string.IsNullOrEmpty(_userConnectionManager.GetUserConnection(userId)))
                return true;
            return false;
        }
        private List<string> IformListToPath(List<IFormFile>? formFiles , string folderPath)
        {
            List<string> Paths = new List<string>();
            if(formFiles != null)
            {
                foreach(var formFile in formFiles)
                {
                    var path = MediaUtilites.ConverIformToPath(formFile, folderPath);
                    Paths.Add(path);
                }
            }
            return Paths;
        }
        private Chat PrepareChatForAdd(Chat chat , List<string> photos , List<string> vedios)
        {
            chat.Photos = new Collection<ChatPhoto>();
            chat.Vedios = new Collection<ChatVedio>();
            foreach(var photo in photos)
            {
                if (!string.IsNullOrEmpty(photo))
                {
                    chat.Photos.Add(new ChatPhoto {
                        Id = Guid.NewGuid(),
                        PhotoPath = photo,
                        ChatId = chat.Id,
                    });
                }
            }
            foreach (var vedio in vedios)
            {
                if (!string.IsNullOrEmpty(vedio))
                {
                    chat.Vedios.Add(new ChatVedio
                    {
                        Id = Guid.NewGuid(),
                        VedioPath = vedio,
                        ChatId = chat.Id,
                    });
                }
            }
            return chat;
        }
        private List<BasePhoto> GetChatPhotosByChatId(Guid? chatId)
        {
            var chatPhotos = new List<BasePhoto>();
            if(chatId == Guid.Empty || chatId == null)
                return chatPhotos;
            var Photo = _unitOfWork.ChatPhoto.FindAll(c => c.ChatId == chatId);
            if(Photo != null)
                return Photo.Select(p=> new BasePhoto { Id = p.Id , PhotoPath =p.PhotoPath }).ToList();
            return chatPhotos;
        }
        private List<BaseVedio> GetChatVediosByChatId(Guid? chatId)
        {
            var chatvedio = new List<BaseVedio>();
            if (chatId == Guid.Empty || chatId==null)
                return chatvedio;
            var vedio = _unitOfWork.ChatVedio.FindAll(c => c.ChatId == chatId);
            if (vedio != null)
                return vedio.Select(p => new BaseVedio { Id = p.Id, VedioPath = p.VedioPath }).ToList();
            return chatvedio;
        }


        /// <summary>
        /// use this if meet performance Issue
        /// </summary>
        //public async Task<IEnumerable<friendChat>> GetFriendsWithLastMessageGPT(Guid userId)
        //{
        //    var userFriends = await _acountService.GetAllUserFreinds(userId);

        //    var userChatMessages = await _DbContext.Chat
        //        .Where(c => c.SenderId == userId || c.ReciveId == userId)
        //        .OrderByDescending(c => c.TimeStamp)
        //        .GroupBy(c => c.SenderId == userId ? c.ReciveId : c.SenderId)
        //        .Select(group => group.First()) // Select the latest message for each user
        //        .ToListAsync();

        //    var finalResult = userFriends
        //        .Join(userChatMessages,
        //            friend => friend.Id,
        //            chat => chat.SenderId == userId ? chat.ReciveId : chat.SenderId,
        //            (friend, chat) => new friendChat
        //            {
        //                UserId = friend.Id,
        //                UserName = friend.UserName,
        //                FirstName = friend.FirstName,
        //                LastName = friend.LastName,
        //                Email = friend.Email,
        //                Message = chat.Message,
        //                PhotoPath = chat.PhotoPath,
        //                Read = chat.Read,
        //                TimeStamp = chat.TimeStamp,
        //                VedioPath = chat.VedioPath,
        //                ReciveId = chat.ReciveId,
        //                SenderId = chat.SenderId,
        //            })
        //        .OrderByDescending(x => x.TimeStamp)
        //        .DistinctBy(x => x.UserId);

        //    return finalResult;
        //}

        public record friendChat
        {
            public Guid UserId { get; set; }
            public string Email { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Guid? SenderId { get; set; }
            public Guid? ReciveId { get; set; }
            public Guid? LastMessageId { get; set; }
            public string? Message { get; set; }
            public List<BasePhoto>? Photos { get; set; }
            public List<BaseVedio>? Vedios { get; set; }
            public bool? Read { get; set; }
            public bool? Online { get; set; }
            public DateTime? TimeStamp { get; set; }
            public string? TimeCreated { get; set; }
        }
    }
}
