using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly TradeLogContext ctx = new TradeLogContext();

        private TradeLogViewModel Convert(TradeLog log)
        {
            return new TradeLogViewModel()
            {
                ChangeRate = log.ChangeRate.ToString("N", new CultureInfo("en-US")),
                Currency = log.Currency,
                CustomerName = log.CustomerName,
                Description = log.Description,
                FromAmount = log.FromAmount.ToString("N", new CultureInfo("en-US")),
                IsSell = log.IsSell,
                Phone = log.Phone,
                ToAmount = log.ToAmount.ToString("N", new CultureInfo("en-US")),
                UserName = log.UserName,
                InsertedDate = log.InsertedDate.ToString()
            };
        }

        public ActionResult Index()
        {
            var newestTransactions = ctx.TradeLogs.OrderByDescending(x => x.InsertedDate).Take(10).ToList().Select(Convert).ToList();

            var model = new TransactionPageViewModel()
            {
                NewestTrans = newestTransactions,
                InputTrans = new TradeLogViewModel()
            };

            return View(model);
        }

        public ActionResult About()
        {
            var model = new SearchPageViewModel()
            {
                Overview = new List<OverviewViewModel>(),
                Trans = new List<TradeLogViewModel>()
            };

            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Create(TransactionPageViewModel vmModel)
        {
            var model = new TradeLog
            {
                ChangeRate = double.Parse(vmModel.InputTrans.ChangeRate),
                Currency = vmModel.InputTrans.Currency,
                CustomerName = vmModel.InputTrans.CustomerName,
                Description = vmModel.InputTrans.Description,
                FromAmount = double.Parse(vmModel.InputTrans.FromAmount),
                InsertedDate = DateTime.Parse(vmModel.InputTrans.InsertedDate),
                IsSell = vmModel.InputTrans.IsSell,
                Phone = vmModel.InputTrans.Phone,
                ToAmount = double.Parse(vmModel.InputTrans.ToAmount),
                UserName = vmModel.InputTrans.UserName
            };

            ctx.TradeLogs.Add(model);
            ctx.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Search(SearchPageViewModel vmModel)
        {
            var model = new SearchPageViewModel()
            {
                SearchCriteria = vmModel.SearchCriteria,
                Trans = OnSearch(vmModel.SearchCriteria),
                Overview = OnOverview(vmModel.SearchCriteria)
            };

            return View("About", model);
        }

        private IList<TradeLogViewModel> OnSearch(SearchViewModel search)
        {
            var query = new StringBuilder("select * from tradelog where 1 = 1 ");

            if (search.From.HasValue)
            {
                query.AppendFormat("and InsertedDate >= '{0}' ", search.From.Value.ToString("yyyy-dd-M"));
            }

            if (search.To.HasValue)
            {
                query.AppendFormat("and InsertedDate <= '{0}' ", search.To.Value.ToString("yyyy-dd-M"));
            }

            query.Append("order by InsertedDate desc");

            IList<TradeLog> rets = new List<TradeLog>();

            using (var cn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
            {
                cn.Open();

                var cmd = new MySqlCommand(query.ToString(), cn);

                var rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    rets.Add(new TradeLog()
                    {
                        InsertedDate = (DateTime)rd["InsertedDate"],
                        Currency = (Currency)rd["Currency"],
                        IsSell = (bool)rd["IsSell"],
                        FromAmount = (double)rd["FromAmount"],
                        ToAmount = (double)rd["ToAmount"],
                        ChangeRate = (double)rd["ChangeRate"],
                        CustomerName = (string)rd["CustomerName"],
                        Phone = (string)rd["Phone"],
                        UserName = (string)rd["UserName"],
                        Description = (string)rd["Description"]
                    });
                }
            }

            return rets.Select(Convert).ToList();
        }

        private IList<OverviewViewModel> OnOverview(SearchViewModel search)
        {
            var query = new StringBuilder("select Date(InsertedDate) dat1, sum(FromAmount) Amount, Currency, IsSell, sum(ToAmount) VndAmount from tradelog where 1 = 1 ");

            if (search.From.HasValue)
            {
                query.AppendFormat("and InsertedDate >= '{0}' ", search.From.Value.ToString("yyyy-dd-M"));
            }

            if (search.To.HasValue)
            {
                query.AppendFormat("and InsertedDate <= '{0}' ", search.To.Value.ToString("yyyy-dd-M"));
            }

            query.Append("group by dat1, Currency, IsSell order by dat1 desc");

            IList<OverviewModel> rets = new List<OverviewModel>();

            using (var cn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString))
            {
                cn.Open();

                var cmd = new MySqlCommand(query.ToString(), cn);

                var rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    rets.Add(new OverviewModel()
                    {
                        InsertedDate = (DateTime)rd["dat1"],
                        Currency = (Currency)rd["Currency"],
                        IsSell = (bool)rd["IsSell"],
                        Amount = (double)rd["Amount"],
                        VndAmount = (double)rd["VndAmount"]
                    });
                }
            }

            return rets.GroupBy(x => new {x.InsertedDate, x.Currency}).Select(x => new OverviewViewModel()
            {
                InsertedDate = x.Key.InsertedDate.ToString("d"),
                Currency = x.Key.Currency.ToString(),
                SellAmount = x.Where(y => y.IsSell).Sum(y => y.Amount).ToString("N", new CultureInfo("en-US")),
                BuyAmount = x.Where(y => !y.IsSell).Sum(y => y.Amount).ToString("N", new CultureInfo("en-US")),
                VndBuyAmount = x.Where(y => !y.IsSell).Sum(y => y.VndAmount).ToString("N", new CultureInfo("en-US")),
                VndSelldAmount = x.Where(y => y.IsSell).Sum(y => y.VndAmount).ToString("N", new CultureInfo("en-US")),
            }).OrderBy(x => x.Currency).ToList();
        }
    }
}