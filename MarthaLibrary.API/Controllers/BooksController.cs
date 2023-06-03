using MarthaLibrary.Application.Interface;
using MarthaLibrary.Domain.Dto;
using MarthaLibrary.Domain.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MarthaLibrary.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _service;
        public BooksController(IBookService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<IActionResult> CreateBook([FromForm]BookRequestDto book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var filer = new ImageDto
            {
                Content = book.formFile.OpenReadStream(),
                Name = book.formFile.FileName,
                ContentType = book.formFile.ContentType,
            };
            var response = await _service.CreateBook(book,filer);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook(ReserveBookRequestDto reserves)
        {
            var response = await _service.BorrowBook(reserves);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> ReserveBook(ReserveBookRequestDto book)
        {
            var response = await _service.ReserveBook(book);
            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> AllBooks()
        {
            var response = await _service.GetAllBooks();
            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> GetBook(string search)
        {
            var response = await _service.GetBook(search);
            return Ok(response);
        }


        [HttpPost]
        public async Task<IActionResult> ReturnBook(string id)
        {
            var response = await _service.ReturnBook(id);
            return Ok(response);
        }
    }
}
