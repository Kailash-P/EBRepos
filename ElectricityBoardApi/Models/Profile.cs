using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectricityBoardApi.Models
{
    public class Profile
    {
        public string ProfilePicture { get; set; }
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }
        public bool IsAdmin { get; set; }
        public int ConsumerNo { get; set; }
        public string RegionCode { get; set; }
        public int ID { get; set; }
    }

    public class Bill
    {
        public int ConsumerID { get; set; } // Consumer No

        public string BillDate { get; set; } // Bill Date

        public int Units { get; set; } // Units

        public int Amount { get; set; } // Amount

        public int ID { get; set; } // ID

    }
}