using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectricityBoardApplication.Models
{
    public class ImportProfile
    {
        public string UserName { get; set; } // UserName

        public string Password { get; set; } // Password

        public string ConsumerNo { get; set; } // ConsumerNo

        public string RegionCode { get; set; } // RegionCode        

        public string Address { get; set; } // Address

        public string Email { get; set; } // Email

        public string City { get; set; } // City

        public string State { get; set; } // State

        public string ZipCode { get; set; } // ZipCode

        public string IsAdmin { get; set; } // IsAdmin
    }
}