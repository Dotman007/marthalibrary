using MarthaLibrary.Application.Interface;
using MarthaLibrary.Domain.Constant;
using MarthaLibrary.Domain.Dto;
using MarthaLibrary.Domain.Entities;
using MarthaLibrary.Infrastructure.DataAccessLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Application.Services
{
    public class UserService : IUserService
    {
        private MarthaLibraryDb _context;
        private IJwtUtils _jwtUtils;
        private readonly IConfiguration _config;

        public UserService(
            MarthaLibraryDb context,
            IJwtUtils jwtUtils,
            IConfiguration config)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _config = config;
        }


        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _context.Users.SingleOrDefault(x => x.UserName == model.UserName);

            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return new AuthenticateResponse(user,null)
                {
                    Response = new BookResponseDto
                    {
                        ResponseCode = ResponseMapping.InvalidCredentialsResponseCode,
                        ResponseMessage = ResponseMapping.InvalidCredentialsResponseMessage
                    }
                };

            // authentication successful so generate jwt token
            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken)
            {
                Response = new BookResponseDto
                {
                    ResponseCode = ResponseMapping.SuccessResponseCode,
                    ResponseMessage = ResponseMapping.SuccessResponseMessage
                }
            };
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(string id)
        {
            var user = _context.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
    }
}
