using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Domain.Entities
{
    public class AuditBase
    {
        public AuditBase()
        {
            DateCreated = DateTime.Now;
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? CreatedBy { get; set; }
    }
}
