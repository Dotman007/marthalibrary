using MarthaLibrary.Domain.Constant;
using MarthaLibrary.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Domain.Dto
{
    public class AuthenticateRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }


    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(User user, string token)
        {
            Id = user.Id;
            FullName = user.FullName;
            UserName = user.UserName;
            Role = user.Role;
            Token = token;
        }

        public BookResponseDto Response { get; set; }
    }
}
