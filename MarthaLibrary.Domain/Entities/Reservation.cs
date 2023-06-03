using MarthaLibrary.Domain.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Domain.Entities
{
    public class Reservation: AuditBase
    {
        [ForeignKey("BookId")]
        public Book Book { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime BookingDate { get; set; }
        public ReservationStatus ReservationStatus { get; set; }
    }
}
