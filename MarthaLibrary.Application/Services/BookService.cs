using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MarthaLibrary.Application.Interface;
using MarthaLibrary.Domain.Constant;
using MarthaLibrary.Domain.Dto;
using MarthaLibrary.Domain.Entities;
using MarthaLibrary.Domain.Utility;
using MarthaLibrary.Infrastructure.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MarthaLibrary.Application.Services
{
    public class BookService : IBookService
    {
        private readonly MarthaLibraryDb _context;
        private readonly BlobServiceClient _blobServiceClient;
        public BookService(MarthaLibraryDb context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadImageUtility(ImageDto images)
        {
            if (images == null)
            {
                return null;
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient("thrybeimage");
            var url = new List<string>();
            
            var blobClient = containerClient.GetBlobClient(images.GetPathWithFileName(Guid.NewGuid().ToString()));
            await blobClient.UploadAsync(images.Content, new BlobHttpHeaders { ContentType = images.ContentType });
            url.Add(blobClient.Uri.ToString());
            return url[0].ToString();
        }
        public async Task<BookResponseDto> CreateBook(BookRequestDto bookRequestDto,ImageDto image)
        {
            try
            {
                var imageUrl = await UploadImageUtility(image);
                var book = new Book
                {
                    Title = bookRequestDto.Title,
                    Author = bookRequestDto.Author,
                    ImageUrl = imageUrl,
                    NumberOfCopies =  bookRequestDto.NumberOfCopies,
                    BookStatus = Domain.Constant.Status.AVAILABLE,
                    CreatedBy = "",
                };
               await _context.Books.AddAsync(book);
               await _context.SaveChangesAsync();
                return new BookResponseDto
                {
                    ResponseCode = ResponseMapping.SuccessResponseCode,
                    ResponseMessage = ResponseMapping.SuccessResponseMessage
                };
            }
            catch (Exception ex)
            {
                return new BookResponseDto
                {
                    ResponseCode = ResponseMapping.ErrorResponseCode,
                    ResponseMessage = ResponseMapping.ErrorResponseMessage
                };
            }
        }

        public async Task<BookResponseDto> ReserveBook(string id)
        {
            try
            {
                var getBook = await _context.Books.Where(e => e.Id == id).FirstOrDefaultAsync();
                if (getBook == null)
                {
                    return new BookResponseDto
                    {
                        ResponseMessage = ResponseMapping.NoRecordResponseMessage,
                        ResponseCode = ResponseMapping.NoRecordResponseCode
                    };
                }

                if (getBook.BookStatus == Status.RESERVED && getBook.NumberOfCopies < 1)
                {
                    return new BookResponseDto
                    {
                        ResponseMessage = ResponseMapping.BookReservedResponseMessage,
                        ResponseCode = ResponseMapping.BookReservedResponseCode
                    };
                }


                if (getBook.BookStatus == Status.BORROWED  && getBook.NumberOfCopies < 1)
                {
                    return new BookResponseDto
                    {
                        ResponseMessage = ResponseMapping.BookBorrowedResponseMessage,
                        ResponseCode = ResponseMapping.BookReservedResponseCode
                    };
                }

                var reserve = new Reservation
                {
                    Book = getBook,
                    CreatedBy = "",
                    DateCreated = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddHours(24),
                };

                await _context.Reservations.AddAsync(reserve);
                getBook.NumberOfCopies -= 1;
                getBook.BookStatus = Status.RESERVED;
                await _context.SaveChangesAsync();
                return new BookResponseDto
                {
                    ResponseCode = ResponseMapping.SuccessResponseCode,
                    ResponseMessage = ResponseMapping.SuccessResponseMessage
                };

            }
            catch (Exception ex)
            {
                return new BookResponseDto
                {
                    ResponseCode = ResponseMapping.ErrorResponseCode,
                    ResponseMessage = ResponseMapping.ErrorResponseMessage
                };
            }
        }

        public async Task<BookResponseDto> BorrowBook(ReserveBookRequestDto reserves)
        {
            try
            {
                var getBook = await _context.Books.Where(e => e.Id == reserves.Id).FirstOrDefaultAsync();
                if (getBook == null)
                {
                    return new BookResponseDto
                    {
                        ResponseMessage = ResponseMapping.NoRecordResponseMessage,
                        ResponseCode = ResponseMapping.NoRecordResponseCode
                    };
                }

                if (getBook.BookStatus == Status.RESERVED && getBook.NumberOfCopies < 1 )
                {
                    return new BookResponseDto
                    {
                        ResponseMessage = ResponseMapping.BookReservedResponseMessage,
                        ResponseCode = ResponseMapping.BookReservedResponseCode
                    };
                }


                if (getBook.BookStatus == Status.BORROWED && getBook.NumberOfCopies < 1)
                {
                    return new BookResponseDto
                    {
                        ResponseMessage = ResponseMapping.BookBorrowedResponseMessage,
                        ResponseCode = ResponseMapping.BookReservedResponseCode
                    };
                }

                var reserve = new Reservation
                {
                    Book = getBook,
                    CreatedBy = reserves.BorrowerName,
                    DateCreated = DateTime.Now,
                    ExpiryDate = reserves.ReturnDate,
                    ReturnDate = reserves.ReturnDate,
                    BookingDate = DateTime.Now,
                };

                await _context.Reservations.AddAsync(reserve);
                getBook.NumberOfCopies -= 1;
                getBook.BookStatus = Status.BORROWED;
                await _context.SaveChangesAsync();
                return new BookResponseDto
                {
                    ResponseCode = ResponseMapping.SuccessResponseCode,
                    ResponseMessage = ResponseMapping.SuccessResponseMessage
                };
            }
            catch (Exception ex)
            {
                return new BookResponseDto
                {
                    ResponseCode = ResponseMapping.ErrorResponseCode,
                    ResponseMessage = ResponseMapping.ErrorResponseMessage
                };
            }
        }

        public async Task<List<AllBookResponseDto>> GetAllBooks()
        {
            try
            {
                var getBooks = await _context.Books.Where(s => s.NumberOfCopies > 0).Select(d => new AllBookResponseDto
                {
                    Id = d.Id,
                    Author = d.Author,
                    ImageUrl = d.ImageUrl,
                    NumberOfCopies = d.NumberOfCopies,
                    Title = d.Title,
                }).ToListAsync();
                return getBooks;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<AllBookResponseDto>> GetBook(string search)
        {
            try
            {
                var getBooks = await _context.Books.Where(s => s.Title.Contains(search)
                || s.Author.Contains(search) || s.Id.Contains(search) && s.NumberOfCopies > 0)
                    .Select(d => new AllBookResponseDto
                    {
                        Id = d.Id,
                        Author = d.Author,
                        ImageUrl = d.ImageUrl,
                        NumberOfCopies = d.NumberOfCopies,
                        Title = d.Title,
                    }).ToListAsync();
                return getBooks;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<BookResponseDto> ReturnBook(string id)
        {
            try
            {
                var getBook = await _context.Reservations.Include(e=>e.Book).Where(e => e.Id ==id).FirstOrDefaultAsync();
                if (getBook == null)
                {
                    return new BookResponseDto
                    {
                        ResponseMessage = ResponseMapping.NoRecordResponseMessage,
                        ResponseCode = ResponseMapping.NoRecordResponseCode
                    };
                }
                if (getBook.ReservationStatus == ReservationStatus.RETURNED)
                {
                    return new BookResponseDto
                    {
                        ResponseMessage = ResponseMapping.AlreadyReturnedResponseMessage,
                        ResponseCode = ResponseMapping.AlreadyReturnedResponseCode
                    };
                }
                getBook.ReservationStatus = ReservationStatus.RETURNED;
                var getBooks = await _context.Books.Where(e => e.Id == getBook.Book.Id).FirstOrDefaultAsync();
                getBooks.NumberOfCopies += 1;
                getBooks.BookStatus =  Status.AVAILABLE;
                await _context.SaveChangesAsync();
                return new BookResponseDto
                {
                    ResponseCode = ResponseMapping.SuccessResponseCode,
                    ResponseMessage = ResponseMapping.SuccessResponseMessage
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
