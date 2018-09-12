using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectricityBoardApi.Models
{
    public class SentimentAnalysis
    {
        public int ID { get; set; }

        public string ConsumerName { get; set; }

        public decimal Sentiment { get; set; }

        public string FeedBack { get; set; }
    }

    public class FeedBack
    {
        public string languate { get; set; }

        public int id { get; set; } //

        public string text { get; set; } // Content


    }

    public class Result
    {
        public decimal score { get; set; }

        public int id { get; set; }
    }

    public class SentimentResponse
    {
        public IEnumerable<Result> documents { get; set; }
    }
}