using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Domain.Entities
{
    public class PendingBook :AuditBase
    {
        public string CustomerId { get; set; }

        public Book Book { get; set; }
    }
}
