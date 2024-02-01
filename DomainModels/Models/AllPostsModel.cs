using DataBase.Core.Enums;
using DataBase.Core.Models.CommentModels;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.Reacts;
using DataBase.Core.Models.VedioModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.Models
{
    public class AllPostsModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TimeCreated { get; set; }
        public List<IFormFile> Photos { get; set; }
        public List<IFormFile> Vedios { get; set; }
        public List<BasePhoto> Photo { get; set; }
        public List<BaseVedio> Vedio { get; set; }
        public List<BaseComment> comments { get; set; }
        public List<BaseReact> reacts { get; set; }
        public PostsTypes Type { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public Guid UserAccountsId { get; set; }
        public string PostUserFirstName { get; set; }
        public string PostUserLastName { get; set; }


        [BindNever]
        public List<string> PhotosPath { get; set; }

        [BindNever]
        public List<string> VediosPath { get; set; }
    }
    public class UploadPost
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public List<IFormFile>? Vedios { get; set; }
        public PostsTypes Type { get; set; }
        public string Question { get; set; }
        public string? Answer { get; set; }

    }
    public class UpdataPost
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Guid> DeletedPhotoIds { get; set; }
        public List<Guid> DeletedVedioIds { get; set; }
        public List<IFormFile> NewPhotos { get; set; }
        public List<IFormFile> NewVedios { get; set; }
        public PostsTypes Type { get; set; }
        public string Question { get; set; } // Additional property for QuestionPost
        public string Answer { get; set; } // Additional property for QuestionPost

    }
}
