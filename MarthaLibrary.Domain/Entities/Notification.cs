using MarthaLibrary.Domain.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Domain.Entities
{
    public class Notification: AuditBase
    {
        public string CustomerId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }
        public Status Status { get; set; }
    }
}
