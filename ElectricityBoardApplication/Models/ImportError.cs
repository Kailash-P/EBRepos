using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectricityBoardApplication.Models
{
    public class ImportError
    {
        public int RowNumber { get; set; }

        public string ErrorLevel { get; set; }

        public string ColumnName { get; set; }

        public string ErrorMessage { get; set; }
    }
}