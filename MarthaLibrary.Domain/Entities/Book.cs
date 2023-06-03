using MarthaLibrary.Domain.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Domain.Entities
{
    public class Book: AuditBase
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ImageUrl { get; set; }
        public Status BookStatus  { get; set; }
        public int NumberOfCopies { get; set; }


    }
}
