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
        Task<BookResponseDto> ReserveBook(ReserveBookRequestDto reserves);
        Task<BookResponseDto> ReturnBook(string id);
        Task<BookResponseDto> BorrowBook(ReserveBookRequestDto reserves);
        Task<List<AllBookResponseDto>> GetAllBooks();
        Task<List<AllBookResponseDto>> GetBook(string search);
        void CheckReservedExpiry();
        Task<BookResponseDto> SendNotification(BookingNotification booking);
        Task<BookResponseDto> QueuePendingBooks(BookingNotification booking);
    }
}
