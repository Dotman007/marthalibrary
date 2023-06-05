using MarthaLibrary.Domain.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Domain.Entities
{
    public class User :AuditBase
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public Role Role { get; set; }
        public string PasswordHash { get; set; }
    }
}
