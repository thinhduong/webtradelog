using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using PagedList;

namespace WebApplication1.Models
{
    public class TradeLogViewModel
    {
        public string InsertedDate { get; set; }

        public Currency Currency { get; set; }

        public bool IsSell { get; set; }

        public string FromAmount { get; set; }

        public string ChangeRate { get; set; }

        public string ToAmount { get; set; }

        public string CustomerName { get; set; }

        public string Phone { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; }

    }

    public class TransactionPageViewModel
    {
        public TradeLogViewModel InputTrans { get; set; }
        public IList<TradeLogViewModel> NewestTrans { get; set; }
    }

    public class SearchPageViewModel
    {
        public SearchViewModel SearchCriteria { get; set; }

        public IPagedList<TradeLogViewModel> Trans { get; set; }

        public IList<OverviewViewModel> Overview { get; set; }
    }

    public class SearchViewModel
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public int? Page { get; set; }
    }

    public class OverviewModel
    {
        public Currency Currency { get; set; }

        public double Amount { get; set; }

        public DateTime InsertedDate { get; set; }

        public bool IsSell { get; set; }

        public double VndAmount { get; set; }
    }

    public class OverviewViewModel
    {
        public string Currency { get; set; }

        public string BuyAmount { get; set; }

        public string SellAmount { get; set; }

        public string InsertedDate { get; set; }

        public string VndSelldAmount { get; set; }

        public string VndBuyAmount { get; set; }
    }

    [Table("tradelog")]
    public class TradeLog
    {
        [Key]
        public int LogId { get; set; }

        public DateTime InsertedDate { get; set; }

        public Currency Currency { get; set; }

        public bool IsSell { get; set; }

        [Column(TypeName = "DOUBLE")]
        public double FromAmount { get; set; }

        [Column(TypeName = "DOUBLE")]
        public double ChangeRate { get; set; }

        [Column(TypeName = "DOUBLE")]
        public double ToAmount { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(50)]
        public string CustomerName { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(20)]
        public string Phone { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(20)]
        public string UserName { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(150)]
        public string Description { get; set; }

    }

    public enum Currency
    {
        USD,
        EUR,
        GBP,
        INR,
        AUD,
        CAD,
        SGD,
        CHF,
        MYR,
        JPY,
        CNY
    }

    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class TradeLogContext : DbContext
    {
        public TradeLogContext() : base("MyDB")
        {
            Database.SetInitializer(new TradeLogDbInitializer());
        }

        public DbSet<TradeLog> TradeLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }

    public class TradeLogDbInitializer : CreateDatabaseIfNotExists<TradeLogContext>
    {
        protected override void Seed(TradeLogContext context)
        {
            /*var rnd = new Random();

            var currencies = new List<Currency>() {Currency.USD, Currency.EUR, Currency.JPY};
            var rates = new List<double> {23000, 30100, 15000};

            var defaultTradeLogs = new List<TradeLog>();

            for(int i=0; i< currencies.Count; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    defaultTradeLogs.Add(new TradeLog()
                    {
                        Currency = currencies[i],
                        ChangeRate = rates[i],
                        Description = string.Empty,
                        FromAmount = rnd.Next(100, 300),
                        InsertedDate = DateTime.Now.Subtract(new TimeSpan(j, 0, 0, 0)),
                        IsSell = true,
                        CustomerName = string.Format("Customer {0} {1}", i, j),
                        UserName = string.Format("User {0} {1}", i, j)
                    });

                    defaultTradeLogs.Add(new TradeLog()
                    {
                        Currency = currencies[i],
                        ChangeRate = rates[i],
                        Description = string.Empty,
                        FromAmount = rnd.Next(100, 300),
                        InsertedDate = DateTime.Now.Subtract(new TimeSpan(j, 0, 0, 0)),
                        IsSell = false,
                        CustomerName = string.Format("Customer {0} {1}", i, j),
                        UserName = string.Format("User {0} {1}", i, j)
                    });
                }
            }

            defaultTradeLogs.ForEach(x =>
            {
                x.ToAmount = x.FromAmount*x.ChangeRate;
                context.TradeLogs.Add(x);
            });
            
            base.Seed(context);*/
        }
    }
}
