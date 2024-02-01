using DataBase.Core.Models.Posts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase.Core.Models.CommentModels;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.VedioModels;
using DataBase.Core.Models.Reacts;

namespace DataBase.EF.DBConfiguration
{
    public class CommentPostConfiguration : IEntityTypeConfiguration<PostComment>
    {
        public void Configure(EntityTypeBuilder<PostComment> modelBuilder)
        {
        }

    }
}
