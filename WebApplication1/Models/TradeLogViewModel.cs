using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

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

        public IList<TradeLogViewModel> Trans { get; set; }

        public IList<OverviewViewModel> Overview { get; set; }
    }

    public class SearchViewModel
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }

    public class OverviewViewModel
    {
        public Currency Currency { get; set; }

        public double Amount { get; set; }

        public DateTime InsertedDate { get; set; }

        public bool IsSell { get; set; }
    }

    [Table("tradelog")]
    public class TradeLog
    {
        [Key]
        public int LogId { get; set; }

        public DateTime InsertedDate { get; set; }

        public Currency Currency { get; set; }

        public bool IsSell { get; set; }

        public double FromAmount { get; set; }

        public double ChangeRate { get; set; }

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
        HK
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
    }
}
