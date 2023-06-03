using MarthaLibrary.Domain.Constant;
using MarthaLibrary.Domain.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Domain.Dto
{

    public class ReserveBookRequest
    {
        public string BookId { get; set; }
    }
    public class BookRequestDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int NumberOfCopies { get; set; }
        public IFormFile formFile { get; set; }
    }

    public class ReserveBookRequestDto
    {
        public string Id { get; set; }
        public DateTime ReturnDate { get; set; }
        public string BorrowerName { get; set; }
    }
    public class BookResponseDto
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }

    public class BookResponse
    {
        public BookResponseDto Response { get; set; }
        public List<AllBookResponseDto> Books { get; set; }
    }

    public class AllBookResponseDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int NumberOfCopies { get; set; }

        public string ImageUrl { get; set; }

    }
}
