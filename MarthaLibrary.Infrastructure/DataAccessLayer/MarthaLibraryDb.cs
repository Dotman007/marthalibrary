using MarthaLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Infrastructure.DataAccessLayer
{
    public class MarthaLibraryDb : DbContext
    {
        public MarthaLibraryDb(DbContextOptions<MarthaLibraryDb> options) : base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PendingBook> PendingBookss { get; set; }
    }
}
