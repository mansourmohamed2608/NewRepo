using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.Models
{
    public class CommentUpdateRequest
    {
        public Guid Id { get; set; }
        public string? comment { get; set; }
        public IFormFile? Photo { get; set; }
        public IFormFile? Vedio { get; set; }
        public Guid? DeletedPhotoId { get; set; }
        public Guid? DeletedVideoId { get; set; }

    }
    public class AddCommentRequest
    {
        public string? comment { get; set; }
        public Guid PostId { get; set; }
        public IFormFile? Photo { get; set; }
        public IFormFile? Vedio { get; set; }

    }
}
