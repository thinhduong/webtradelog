using System;

namespace WebApplication1.Models
{
    public class TradeLogViewModel
    {
        public string InsertedDate { get; set; }

        public Currency FromCurrency { get; set; }

        public Currency ToCurrency { get; set; }

        public string FromAmount { get; set; }

        public string ChangeRate { get; set; }

        public string ToAmount { get; set; }

        public string CustomerName { get; set; }

        public string Phone { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; }

    }


    public enum Currency
    {
        VND,
        USD,
        HK
    }
}
