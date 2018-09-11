using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectricityBoardApi.Models
{
    public static class Session
    {
        public static int Login_ID { get; set; }

        public static string LoginEmail { get; set; }

        public static string LoginName { get; set; }
    }
}