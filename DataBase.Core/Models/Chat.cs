using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.VedioModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models
{
    public class Chat
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReciveId { get; set; }
        public string? Message { get; set; }
        public bool Read { get; set; } = false;
        public ICollection<ChatPhoto> Photos { get; set; }
        public ICollection<ChatVedio> Vedios { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
