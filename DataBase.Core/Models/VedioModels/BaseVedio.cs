using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.VedioModels
{
    public class BaseVedio
    {
        [Key]
        public Guid Id { get; set; }
        public string VedioPath { get; set; }
    }
}
