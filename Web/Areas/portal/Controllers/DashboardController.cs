using Dto.Dto;
using Dto.Enums;
using Microsoft.Ajax.Utilities;
using Repository.Repo.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters;

namespace Web.Areas.portal.Controllers
{
    [PortalAuthorize]
    public class DashboardController : BaseAdminController
    {
        OrderRepo _repo = new OrderRepo();
        IEnumerable<SalesDisplayDto> _sales;


        public ActionResult Index()
        {
            _sales = _repo.GetSalesDisplay(new DateTime(DateTime.Now.Year, 1, 1), DateTime.Now);
            ViewBag.Sales = _sales.ToList();
            var status = string.Join("|", new List<int>()
            {
                (int)OrderStatusEnum.Processing,
                (int)OrderStatusEnum.Pending,
                (int)OrderStatusEnum.Completed
            });

            ViewBag.Orders = _repo.GetListAllByFilter(status);
            return View();
        }

        public ActionResult LoadSales(DateTime dateFrom, DateTime dateTo)
        {
            var model = _repo.GetSalesDisplay(dateFrom, dateTo);
            return PartialView("_ListSales", model);
        }

        public ActionResult Details(int id)
        {
            var record = _repo.Get(id);
            ViewBag.RefundedAmount = _repo.GetRefundedAmount(id);
            return View(record);
        }

        [HttpPost]
        public ActionResult Refund(int id, decimal amount)
        {
            if (amount > 0)
            {
                var record = _repo.Get(id);
                if (record != null)
                {
                    var payment = new PaymentRepo().Add(new PaymentDto
                    {
                        Amount = amount,
                        Currency = "PHP",
                        OrderId = record.Id,
                        PaymentId = Guid.NewGuid().ToString(),
                        Status = PaymentStatus.Refunded,
                        CreatedAt = DateTime.Now,
                        PayoneerId = $"Refunded by {Identity.FirstName} {Identity.LastName}.",
                    });

                    ShowMessage(payment.Message, payment.Success);
                }
                else
                {
                    ShowErrorMessage("Record not found.");
                }
            }
            else
            {
                ShowErrorMessage("Invalid amount.");
            }


            return RedirectToAction("Index");
        }
    }
}