using System;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly TradeLogContext ctx = new TradeLogContext();

        public ActionResult Index()
        {
            var newestTransactions = ctx.TradeLogs.OrderByDescending(x => x.InsertedDate).Take(10).ToList().Select(x => new TradeLogViewModel()
            {
                ChangeRate = x.ChangeRate.ToString("N", new CultureInfo("en-US")),
                Currency = x.Currency,
                CustomerName = x.CustomerName,
                Description = x.Description,
                FromAmount = x.FromAmount.ToString("N", new CultureInfo("en-US")),
                IsSell = x.IsSell,
                Phone = x.Phone,
                ToAmount = x.ToAmount.ToString("N", new CultureInfo("en-US")),
                UserName = x.UserName
            }).ToList();

            var model = new TransactionPageViewModel()
            {
                NewestTrans = newestTransactions,
                InputTrans = new TradeLogViewModel()
            };

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
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
    }
}