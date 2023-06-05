using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Domain.Constant
{
    public class ResponseMapping
    {
        public static string SuccessResponseCode = "00";
        public static string SuccessResponseMessage = "Success";

        public static string NoRecordResponseCode = "89";
        public static string NoRecordResponseMessage = "No record found";


        public static string BookReservedResponseCode = "88";
        public static string BookReservedResponseMessage = "Book is already reserved";

        public static string BookBorrowedResponseCode = "80";
        public static string BookBorrowedResponseMessage = "Book is already borrowed";

        public static string AlreadyReturnedResponseCode = "23";
        public static string AlreadyReturnedResponseMessage = "Book is already returned";

        public static string ErrorResponseCode = "99";
        public static string ErrorResponseMessage = "an error occurred";

        public static string InvalidCredentialsResponseCode = "37";
        public static string InvalidCredentialsResponseMessage = "Invalid username or password";
    }
}
