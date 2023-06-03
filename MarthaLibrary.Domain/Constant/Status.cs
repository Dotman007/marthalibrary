using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarthaLibrary.Domain.Constant
{
    public enum Status
    {
        RESERVED =1,
        BORROWED=2,
        AVAILABLE=3,
    }

    public enum ReservationStatus
    {
        BORROWED=1,
        RETURNED =2,
    }
}
