using Database.SQL;
using Dto;
using Dto.Dto;
using Dto.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo.Order
{
    public class PaymentRepo
    {
        public PaymentDto ToDto(Payment item)
        {
            if (item == null) return null;

            return new PaymentDto()
            {
                Id = item.Id,
                OrderId = item.OrderId,
                PayoneerId = item.PayoneerId,
                PaymentId = item.PaymentId,
                CreatedAt = item.CreatedAt,
                Currency = item.Currency,
                Amount = item.Amount,
                ErrorCode = item.ErrorCode,
                ErrorMessage = item.ErrorMessage,
                Status = (PaymentStatus)item.Status,
                UpdatedAt = item.UpdatedAt.Value,
            };
        }

        public PaymentDto GetPayment(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var item = context.Payments.FirstOrDefault(x => x.Id == id);
                return ToDto(item);
            }
        }

        public IEnumerable<PaymentDto> GetPayments(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var items = context.Payments;
                return items.ToList().Select(a => ToDto(a));
            }
        }

        public ReturnValue Create(string paymentId, int orderId, string currency, decimal amount)
        {
            return Add(new PaymentDto()
            {
                OrderId = orderId,
                PaymentId = paymentId,
                PayoneerId = "",
                CreatedAt = DateTime.Now,
                Currency = currency,
                Amount = amount,
                Status = PaymentStatus.Created,
                ErrorCode = "",
                ErrorMessage = "",
                UpdatedAt = DateTime.MinValue,
            });
        }

        public ReturnValue Add(PaymentDto item)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var newPayment = new Payment()
                {
                    OrderId = item.OrderId,
                    PayoneerId = item.PayoneerId ?? "",
                    PaymentId = item.PaymentId ?? "",
                    CreatedAt = DateTime.Now,
                    Currency = item.Currency ?? "",
                    Amount = item.Amount,
                    Status = (int)item.Status,
                    ErrorCode = item.ErrorCode ?? "",
                    ErrorMessage = item.ErrorMessage ?? "",
                    Id = item.Id,
                    UpdatedAt = null,
                };

                context.Payments.Add(newPayment);
                Db.SaveChanges(context, result, "Payment saved!");
            }

            return result;
        }

        public ReturnValue Update(PaymentDto item)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Payments.FirstOrDefault(a => a.PayoneerId == item.PayoneerId);
                if (record != null)
                {
                    return new ReturnValue("Payment not found!");
                }

                record.CreatedAt = item.CreatedAt;
                record.Currency = item.Currency;
                record.Amount = item.Amount;
                record.ErrorCode = item.ErrorCode;
                record.ErrorMessage = item.ErrorMessage;
                record.Status = (int)item.Status;
                record.UpdatedAt = DateTime.Now;
            }

            return result;
        }

        public ReturnValue UpdateStatus(string paymentId, PaymentStatus status)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Payments.FirstOrDefault(a => a.PaymentId == paymentId);
                if (record != null)
                {
                    return new ReturnValue("Payment not found!");
                }

                record.Status = (int)status;
                record.UpdatedAt = DateTime.Now;
            }

            return result;
        }

    }
}
