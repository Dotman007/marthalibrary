using MarthaLibrary.Application.Interface;
using MarthaLibrary.Domain.Constant;
using MarthaLibrary.Domain.Dto;
using MarthaLibrary.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarthaLibrary.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);
            return Ok(response);
        }


        
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(string id)
        {
            // only admins can access other user records
            var currentUser = (User)HttpContext.Items["User"];
            if (id != currentUser.Id && currentUser.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            var user = _userService.GetById(id);
            return Ok(user);
        }
    }
}
