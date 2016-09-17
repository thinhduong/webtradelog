using System;
using System.Collections.Generic;
using PagedList;
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
        private readonly CultureInfo _usCultureInfo = new CultureInfo("en-US");
        private readonly CultureInfo _frCultureInfo = new CultureInfo("fr-FR");

        public HomeController()
        {
            Seed(ctx);
        }

        private TradeLogViewModel Convert(TradeLog log)
        {
            return new TradeLogViewModel()
            {
                ChangeRate = log.ChangeRate.ToString("N", _usCultureInfo),
                Currency = log.Currency,
                CustomerName = log.CustomerName,
                Description = log.Description,
                FromAmount = log.FromAmount.ToString("N", _usCultureInfo),
                IsSell = log.IsSell,
                Phone = log.Phone,
                ToAmount = log.ToAmount.ToString("N", _usCultureInfo),
                UserName = log.UserName,
                InsertedDate = log.InsertedDate.ToString(_frCultureInfo)
            };
        }

        protected void Seed(TradeLogContext context)
        {
          /*  var rnd = new Random();

            var currencies = new List<Currency>() { Currency.USD, Currency.EUR, Currency.JPY };
            var rates = new List<double> { 23000, 30100, 15000 };

            var defaultTradeLogs = new List<TradeLog>();

            for (int i = 0; i < currencies.Count; i++)
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
                x.ToAmount = x.FromAmount * x.ChangeRate;
                context.TradeLogs.Add(x);
            });

            context.SaveChanges(); */
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

        public ActionResult About(DateTime? from, DateTime? to, int? page)
        {
            var searchCriteria = new SearchViewModel()
            {
                From = from,
                To = to,
                Page = page
            };

            var model = new SearchPageViewModel()
            {
                SearchCriteria = searchCriteria,
                Overview = OnOverview(searchCriteria),
                Trans = OnSearch(searchCriteria)
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
                InsertedDate = DateTime.ParseExact(vmModel.InputTrans.InsertedDate, "dd/MM/yyyy HH:mm:ss", _frCultureInfo),
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

        private IPagedList<TradeLogViewModel> OnSearch(SearchViewModel search)
        {
            int pageSize = 50;
            int pageNumber = search.Page ?? 1;

            var query = ctx.TradeLogs.Select(x => x);

            if (search.From.HasValue)
            {
                query = query.Where(x => x.InsertedDate >= search.From.Value);
            }

            if (search.To.HasValue)
            {
                query = query.Where(x => x.InsertedDate <= search.To.Value);
            }

            query = query.OrderBy(x => x.InsertedDate);

            return query.Select(Convert).ToPagedList(pageNumber, pageSize);
        }

        private IList<OverviewViewModel> OnOverview(SearchViewModel search)
        {
            var query = new StringBuilder("select Date(InsertedDate) dat1, sum(FromAmount) Amount, Currency, IsSell, sum(ToAmount) VndAmount from tradelog where 1 = 1 ");

            if (search.From.HasValue)
            {
                query.AppendFormat("and InsertedDate >= '{0}' ", search.From.Value.ToString("yyyy-MM-dd"));
            }

            if (search.To.HasValue)
            {
                query.AppendFormat("and InsertedDate <= '{0}' ", search.To.Value.ToString("yyyy-MM-dd"));
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
                InsertedDate = x.Key.InsertedDate.ToString(_frCultureInfo),
                Currency = x.Key.Currency.ToString(),
                SellAmount = x.Where(y => y.IsSell).Sum(y => y.Amount).ToString("N", _usCultureInfo),
                BuyAmount = x.Where(y => !y.IsSell).Sum(y => y.Amount).ToString("N", _usCultureInfo),
                VndBuyAmount = x.Where(y => !y.IsSell).Sum(y => y.VndAmount).ToString("N", _usCultureInfo),
                VndSelldAmount = x.Where(y => y.IsSell).Sum(y => y.VndAmount).ToString("N", _usCultureInfo),
            }).OrderBy(x => x.Currency).ToList();
        }
    }
}