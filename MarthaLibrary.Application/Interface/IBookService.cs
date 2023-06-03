using MarthaLibrary.Domain.Dto;
using MarthaLibrary.Domain.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Application.Interface
{
    public interface IBookService
    {
        Task<BookResponseDto> CreateBook(BookRequestDto bookRequestDto, ImageDto image);
        Task<BookResponseDto> ReserveBook(string id);
        Task<BookResponseDto> ReturnBook(string id);
        Task<BookResponseDto> BorrowBook(ReserveBookRequestDto reserves);
        Task<List<AllBookResponseDto>> GetAllBooks();
        Task<List<AllBookResponseDto>> GetBook(string search);
    }
}
