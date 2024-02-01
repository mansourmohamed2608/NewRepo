using AutoMapper;
using BDataBase.Core.Models.Accounts;
using DataBase.Core.Enums;
using DataBase.Core.Models;
using DataBase.Core.Models.Accounts;
using DataBase.Core.Models.CommentModels;
using DataBase.Core.Models.Notifications;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.Posts;
using DataBase.Core.Models.Reacts;
using DataBase.Core.Models.VedioModels;
using DomainModels.DTO;
using Utilites;

namespace DomainModels
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PostComment, DTO.CommentDTO>()
                .ForMember(dest => dest.CommentPhoto, opt => opt.MapFrom<PostCommentPhotoResolver>())
                .ForMember(dest => dest.CommentVedio, opt => opt.MapFrom<PostCommentVedioResolver>())
                .ForMember(dest => dest.Date, src => src.MapFrom(src => TimeHelper.ConvertTimeCreateToString(src.Date)))
                .ForMember(dest => dest.Time, src => src.MapFrom(src => src.Date))
                .ForMember(dest => dest.UserFirstName, src => src.MapFrom(src => src.UserAccounts.FirstName))
                .ForMember(dest => dest.UserLastName, src => src.MapFrom(src => src.UserAccounts.LastName))
                .ForMember(dest => dest.UserId, src => src.MapFrom(src => src.UserAccounts.Id))
                .ForMember(dest => dest.CommentReacts, src => src.MapFrom(src => src.PostCommentReacts.Select(pp => new ReactsDTO { Id = pp.Id, react = pp.react , UserId = pp.UserAccountsId }).ToList()));


            CreateMap<QuestionComment, DTO.CommentDTO>()
                .ForMember(dest => dest.CommentPhoto, src => src.MapFrom<QuestionCommentPhotoResolver>())
                .ForMember(dest => dest.CommentVedio, src => src.MapFrom<QuestionCommentVedioResolver>())
                .ForMember(dest => dest.Date, src => src.MapFrom(src => TimeHelper.ConvertTimeCreateToString(src.Date)))
                .ForMember(dest => dest.Time, src => src.MapFrom(src => src.Date))
                .ForMember(dest => dest.UserFirstName, src => src.MapFrom(src => src.UserAccounts.FirstName))
                .ForMember(dest => dest.UserLastName, src => src.MapFrom(src => src.UserAccounts.LastName))
                .ForMember(dest => dest.UserId, src => src.MapFrom(src => src.UserAccounts.Id))
                .ForMember(dest => dest.CommentReacts, src => src.MapFrom(src => src.QuestionCommentReacts.Select(pp => new ReactsDTO { Id = pp.Id, react = pp.react , UserId =pp.UserAccountsId}).ToList()));

            CreateMap<Post, DTO.PostDTO>()
                .ForMember(dest => dest.Time, src => src.MapFrom(src => src.TimeCreated))
                .ForMember(dest => dest.Photos, src => src.MapFrom(src => src.Photos.Select(pp => new BasePhoto { Id = pp.Id, PhotoPath = pp.PhotoPath }).ToList()))
                .ForMember(dest => dest.Vedios, src => src.MapFrom(src => src.Vedios.Select(pp => new BaseVedio { Id = pp.Id, VedioPath = pp.VedioPath }).ToList()))
                .ForMember(dest => dest.TimeCreated, src => src.MapFrom(src => TimeHelper.ConvertTimeCreateToString(src.TimeCreated)))
                .ForMember(dest => dest.UserFirstName, src => src.MapFrom(src => src.UserAccounts.FirstName))
                .ForMember(dest => dest.UserLastName, src => src.MapFrom(src => src.UserAccounts.LastName))
                .ForMember(dest => dest.UserId, src => src.MapFrom(src => src.UserAccounts.Id))
                .ForMember(dest => dest.Reacts, src => src.MapFrom(src => src.Reacts.Select(pp => new ReactsDTO { Id = pp.Id, react = pp.react, UserId = pp.UserAccountsId }).ToList()));


            CreateMap<QuestionPost, DTO.QuestionPostDTO>()
                .ForMember(dest => dest.Time, src => src.MapFrom(src => src.TimeCreated))
                .ForMember(dest => dest.Photos, src => src.MapFrom(src => src.Photos.Select(pp => new BasePhoto { Id = pp.Id, PhotoPath = pp.PhotoPath }).ToList()))
                .ForMember(dest => dest.Vedios, src => src.MapFrom(src => src.Vedios.Select(pp => new BaseVedio { Id = pp.Id, VedioPath = pp.VedioPath }).ToList()))
                .ForMember(dest => dest.TimeCreated, src => src.MapFrom(src => TimeHelper.ConvertTimeCreateToString(src.TimeCreated)))
                .ForMember(dest => dest.UserFirstName, src => src.MapFrom(src => src.UserAccounts.FirstName))
                .ForMember(dest => dest.UserLastName, src => src.MapFrom(src => src.UserAccounts.LastName))
                .ForMember(dest => dest.UserId, src => src.MapFrom(src => src.UserAccounts.Id))
                .ForMember(dest => dest.Reacts, src => src.MapFrom(src => src.Reacts.Select(pp => new ReactsDTO { Id = pp.Id, react = pp.react , UserId = pp.UserAccountsId }).ToList()));


            CreateMap<DTO.PostDTO, DTO.AllPostDTO>()
                .ForMember(dest => dest.Description, src => src.MapFrom(src => src.Description))
                .ForMember(dest => dest.Type, src => src.MapFrom(src => DataBase.Core.Enums.PostsTypes.Post));
            CreateMap<DTO.QuestionPostDTO, DTO.AllPostDTO>()
                .ForMember(dest => dest.Type, src => src.MapFrom(src => DataBase.Core.Enums.PostsTypes.Question))
                .ForMember(dest => dest.Question, src => src.MapFrom(src => src.Question))
                .ForMember(dest => dest.Answer, src => src.MapFrom(src => src.Answer));

            CreateMap<PostCommentReact, DTO.ReactsDTO>()
                .ForMember(dest => dest.UserId, src => src.MapFrom(src => src.UserAccountsId))
                .ForMember(dest => dest.UserFirstName, src => src.MapFrom(src => src.UserAccounts.FirstName))
                .ForMember(dest => dest.UserLastName, src => src.MapFrom(src => src.UserAccounts.LastName));

            CreateMap<QuestionCommentReact, DTO.ReactsDTO>()
                .ForMember(dest => dest.UserId, src => src.MapFrom(src => src.UserAccountsId))
                .ForMember(dest => dest.UserFirstName, src => src.MapFrom(src => src.UserAccounts.FirstName))
                .ForMember(dest => dest.UserLastName, src => src.MapFrom(src => src.UserAccounts.LastName));

            CreateMap<PostReact, DTO.ReactsDTO>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.Id))
                .ForMember(dest => dest.react, src => src.MapFrom(src => src.react))
                .ForMember(dest => dest.UserId, src => src.MapFrom(src => src.UserAccountsId))
                .ForMember(dest => dest.UserFirstName, src => src.MapFrom(src => src.UserAccounts.FirstName))
                .ForMember(dest => dest.UserLastName, src => src.MapFrom(src => src.UserAccounts.LastName));

            CreateMap<QuestionReact, DTO.ReactsDTO>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.Id))
                .ForMember(dest => dest.react, src => src.MapFrom(src => src.react))
                .ForMember(dest => dest.UserId, src => src.MapFrom(src => src.UserAccountsId))
                .ForMember(dest => dest.UserFirstName, src => src.MapFrom(src => src.UserAccounts.FirstName))
                .ForMember(dest => dest.UserLastName, src => src.MapFrom(src => src.UserAccounts.LastName));

            CreateMap<Notifications, DTO.NotificationDTO>()
                .ForMember(dest => dest.PostsType, src => src.MapFrom<PostTypeResolver>())
                .ForMember(dest => dest.PostId, src => src.MapFrom(src => src.ItemId))
                .ForMember(dest => dest.ActionUserFirstName, src => src.MapFrom(src => src.ActionedUser.FirstName))
                .ForMember(dest => dest.ActionUserLastName, src => src.MapFrom(src => src.ActionedUser.LastName));
            
            CreateMap<FriendRequest, DTO.UserAccount>()
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.RequestorId))
                .ForMember(dest => dest.FirstName, src => src.MapFrom(src => src.Requestor.FirstName))
                .ForMember(dest => dest.LastName, src => src.MapFrom(src => src.Requestor.LastName))
                .ForMember(dest => dest.UserName, src => src.MapFrom(src => src.Requestor.UserName))
                .ForMember(dest => dest.City, src => src.MapFrom(src => src.Requestor.City))
                .ForMember(dest => dest.gender, src => src.MapFrom(src => src.Requestor.gender))
                .ForMember(dest => dest.Bio, src => src.MapFrom(src => src.Requestor.Bio))
                .ForMember(dest => dest.Country, src => src.MapFrom(src => src.Requestor.Country))
                .ForMember(dest => dest.Birthdate, src => src.MapFrom(src => src.Requestor.Birthdate))
                .ForMember(dest => dest.Type, src => src.MapFrom(src => src.Requestor.Type))
                .ForMember(dest => dest.Email, src => src.MapFrom(src => src.Requestor.Email));

            CreateMap<FriendRequest, DTO.FriendRequestUserAccount>()
                .ForMember(dest => dest.RequestId, src => src.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, src => src.MapFrom(src => src.RequestorId))
                .ForMember(dest => dest.FirstName, src => src.MapFrom(src => src.Requestor.FirstName))
                .ForMember(dest => dest.LastName, src => src.MapFrom(src => src.Requestor.LastName))
                .ForMember(dest => dest.UserName, src => src.MapFrom(src => src.Requestor.UserName))
                .ForMember(dest => dest.City, src => src.MapFrom(src => src.Requestor.City))
                .ForMember(dest => dest.gender, src => src.MapFrom(src => src.Requestor.gender))
                .ForMember(dest => dest.Bio, src => src.MapFrom(src => src.Requestor.Bio))
                .ForMember(dest => dest.Country, src => src.MapFrom(src => src.Requestor.Country))
                .ForMember(dest => dest.Birthdate, src => src.MapFrom(src => src.Requestor.Birthdate))
                .ForMember(dest => dest.Type, src => src.MapFrom(src => src.Requestor.Type))
                .ForMember(dest => dest.Email, src => src.MapFrom(src => src.Requestor.Email));

            CreateMap<Chat, DTO.ChatDTO>()
                .ForMember(dest => dest.Photos, src => src.MapFrom(src => src.Photos.Select(pp => new BasePhoto { Id = pp.Id, PhotoPath = pp.PhotoPath }).ToList()))
                .ForMember(dest => dest.Vedios, src => src.MapFrom(src => src.Vedios.Select(pp => new BaseVedio { Id = pp.Id, VedioPath = pp.VedioPath }).ToList()))
                .ForMember(dest => dest.TimeStamp, src => src.MapFrom(src => src.TimeStamp))
                .ForMember(dest => dest.TimeCreated, src => src.MapFrom(src => TimeHelper.ConvertTimeCreateToString(src.TimeStamp)));
        }
        public class PostCommentPhotoResolver : IValueResolver<PostComment, CommentDTO, BasePhoto?>
        {
            public BasePhoto? Resolve(PostComment source, CommentDTO destination, BasePhoto? destMember, ResolutionContext context)
            {
                if (source.PostCommentPhoto != null)
                {
                    return new BasePhoto
                    {
                        Id = source.PostCommentPhoto.Id,
                        PhotoPath = source.PostCommentPhoto.PhotoPath
                    };
                }
                return null;
            }
        }
        public class QuestionCommentPhotoResolver : IValueResolver<QuestionComment, CommentDTO, BasePhoto?>
        {
            public BasePhoto? Resolve(QuestionComment source, CommentDTO destination, BasePhoto? destMember, ResolutionContext context)
            {
                if (source.QuestionCommentPhoto != null)
                {
                    return new BasePhoto
                    {
                        Id = source.QuestionCommentPhoto.Id,
                        PhotoPath = source.QuestionCommentPhoto.PhotoPath
                    };
                }
                return null;
            }
        }
        public class PostCommentVedioResolver : IValueResolver<PostComment, CommentDTO, BaseVedio?>
        {
            public BaseVedio? Resolve(PostComment source, CommentDTO destination, BaseVedio? destMember, ResolutionContext context)
            {
                if (source.PostCommentVedio != null)
                {
                    return new BaseVedio
                    {
                        Id = source.PostCommentVedio.Id,
                        VedioPath = source.PostCommentVedio.VedioPath
                    };
                }
                return null;
            }
        }
        public class QuestionCommentVedioResolver : IValueResolver<QuestionComment, CommentDTO, BaseVedio?>
        {
            public BaseVedio? Resolve(QuestionComment source, CommentDTO destination, BaseVedio? destMember, ResolutionContext context)
            {
                if (source.QuestionCommentVedio != null)
                {
                    return new BaseVedio
                    {
                        Id = source.QuestionCommentVedio.Id,
                        VedioPath = source.QuestionCommentVedio.VedioPath
                    };
                }
                return null;
            }
        }

        public class PostTypeResolver : IValueResolver<Notifications, DTO.NotificationDTO, PostsTypes>
        {
            public PostsTypes Resolve(Notifications source, DTO.NotificationDTO destination, PostsTypes destMember, ResolutionContext context)
            {
                if (source.NotificatinType==NotificatinTypes.AddPost || source.NotificatinType == NotificatinTypes.AddReactOnPost || source.NotificatinType == NotificatinTypes.AddComment || source.NotificatinType == NotificatinTypes.AddReactOnComment)
                {
                    return PostsTypes.Post;
                }
                return PostsTypes.Question;
            }
        }
    }
}
