using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MarthaLibrary.Application.Interface;
using MarthaLibrary.Domain.Constant;
using MarthaLibrary.Domain.Dto;
using MarthaLibrary.Domain.Entities;
using MarthaLibrary.Domain.Utility;
using MarthaLibrary.Infrastructure.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

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

        public async Task<BookResponseDto> ReserveBook(ReserveBookRequestDto reserverr)
        {
            try
            {
                var getBook = await _context.Books.Where(e => e.Id == reserverr.Id).FirstOrDefaultAsync();
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

                    await QueuePendingBooks(new BookingNotification
                    {
                        Book = getBook,
                        CustomerId = reserverr.BorrowerName
                    });
                    return new BookResponseDto
                    {
                        ResponseMessage = ResponseMapping.BookReservedResponseMessage,
                        ResponseCode = ResponseMapping.BookReservedResponseCode
                    };
                }


                if (getBook.BookStatus == Status.BORROWED  && getBook.NumberOfCopies < 1)
                {
                    await QueuePendingBooks(new BookingNotification
                    {
                        Book = getBook,
                        CustomerId = reserverr.BorrowerName
                    });
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
                    ReservationStatus =  ReservationStatus.RESERVED
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
                    await QueuePendingBooks(new BookingNotification
                    {
                        Book = getBook,
                        CustomerId = reserves.BorrowerName
                    });
                    return new BookResponseDto
                    {
                        ResponseMessage = ResponseMapping.BookReservedResponseMessage,
                        ResponseCode = ResponseMapping.BookReservedResponseCode
                    };
                }


                if (getBook.BookStatus == Status.BORROWED && getBook.NumberOfCopies < 1)
                {
                    await QueuePendingBooks(new BookingNotification
                    {
                        Book = getBook,
                        CustomerId = reserves.BorrowerName
                    });
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
                var getBooks = await _context.Books.Where(e => e.Id == getBook.Book.Id).FirstOrDefaultAsync();

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
                getBooks.NumberOfCopies += 1;
                getBooks.BookStatus =  Status.AVAILABLE;
                await _context.SaveChangesAsync();
                var book = await _context.PendingBookss.FirstOrDefaultAsync(e => e.Book.Id == getBook.Book.Id);
                await SendNotification(new BookingNotification { CustomerId = book.CustomerId, Book = getBooks });

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

        public void CheckReservedExpiry()
        {
            var allBooks = _context.Reservations.Include(x=>x.Book).Where(e => e.ReservationStatus == ReservationStatus.RESERVED && e.ExpiryDate.Value.Date <= DateTime.Now.Date).ToList();
            
            if (allBooks != null)
            {
                foreach (var item in allBooks)
                {
                    var getBook = _context.Books.Where(s => s.Id == item.Book.Id).FirstOrDefault();
                    getBook.NumberOfCopies += 1;
                    getBook.BookStatus = Status.AVAILABLE;
                    _context.SaveChanges();
                }
                _context.Reservations.RemoveRange(allBooks);
                _context.SaveChanges();

            }
        }

        public async Task<BookResponseDto> SendNotification(BookingNotification booking)
        {
            try
            {
                var notify = await _context.Notifications.AddAsync(new Notification
                {
                    Book = booking.Book,
                    CreatedBy = "",
                    CustomerId = booking.CustomerId,
                    Status = Status.SENT
                });
                //
                var getQueue = await _context.PendingBookss.Where(s => s.Book.Id == booking.Book.Id && s.CustomerId == booking.CustomerId).FirstOrDefaultAsync();
                _context.PendingBookss.Remove(getQueue);
                await _context.SaveChangesAsync();

                return new BookResponseDto
                {
                    ResponseCode = ResponseMapping.SuccessResponseCode,
                    ResponseMessage = ResponseMapping.SuccessResponseMessage
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<BookResponseDto> QueuePendingBooks(BookingNotification booking)
        {
            try
            {
                var notify = await _context.PendingBookss.AddAsync(new PendingBook
                {
                    Book = booking.Book,
                    CreatedBy = "",
                    CustomerId = booking.CustomerId,
                });
                await _context.SaveChangesAsync();
                return new BookResponseDto
                {
                    ResponseCode = ResponseMapping.SuccessResponseCode,
                    ResponseMessage = ResponseMapping.SuccessResponseMessage
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
