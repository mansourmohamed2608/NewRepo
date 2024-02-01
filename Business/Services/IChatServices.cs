using DataBase.Core.Models;
using DomainModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Business.Implementation.ChatServices;

namespace Business.Services
{
    public interface IChatServices
    {
        Task<(bool, ChatDTO)> SendPrivateMessage(UploadChatDTO uploadChatDTO);
        Task<(bool, ChatDTO)> UpdateChat(UpdateChatDTO updateChatDTO, Guid userId);
        Task<bool> MarkUserChatAsRead(Guid firstUser, Guid secoundUserId);
        Task<(bool, Chat)> DeleteChat(Guid userId, Guid chatId);
        Task<List<ChatDTO>> GetUserChat(Guid firstUser , Guid secondUser);
        Task<IEnumerable<friendChat>> GetFriendsWithLastMessage(Guid userId);
    }
}
