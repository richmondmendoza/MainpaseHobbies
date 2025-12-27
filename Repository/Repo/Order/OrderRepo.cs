using Database.SQL;
using Dto;
using Dto.Dto;
using Dto.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo.Order
{
    public class OrderRepo
    {
        public static OrderDto ToDto(Database.SQL.Order item)
        {
            if (item == null) return null;

            return new OrderDto
            {
                Id = item.Id,
                UserId = item.UserId,
                DateCreated = item.DateCreated,
                DueDate = item.DueDate,
                OrderNumber = item.OrderNumber,
                InvoiceNumber = item.InvoiceNumber,
                CustomerName = item.CustomerName,
                Address1 = item.Address1,
                Address2 = item.Address2,
                ContactNumber = item.ContactNumber,
                ContactEmail = item.ContactEmail,
                PaymentMethod = (PaymentMethodEnum)item.PaymentMethod,
                SubTotal = item.SubTotal,
                Tax = item.Tax ?? 0,
                Shipping = item.Shipping ?? 0,
                Total = item.Total ?? 0,
                BookAccountNumber = item.BookAccountNumber,
                Status = (OrderStatusEnum)item.Status,
                DeliveryStatus = (DeliveryStatusEnum)item.DeliveryStatus,
            };
        }

        public static OrderDetailsDto ToDetailsDto(Database.SQL.Order item)
        {
            if (item == null) return null;
            var dto = new OrderDetailsDto(ToDto(item));
            dto.Items = item.Order_Item.ToList().Select(x => OrderItemRepo.ToDto(x)).ToList();

            return dto;
        }

        public static IEnumerable<OrderDetailsDto> GetList()
        {
            using (IMSEntities context = new IMSEntities())
            {
                var list = context.Orders.ToList();
                return list.Select(x => ToDetailsDto(x)).ToList();
            }
        }

        public static OrderDetailsDto Get(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var item = context.Orders.FirstOrDefault(a => a.Id == id);
                return ToDetailsDto(item);
            }
        }

        public ReturnValue Add(OrderDetailsDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var item = new Database.SQL.Order
                {
                    UserId = dto.UserId,
                    DateCreated = dto.DateCreated,
                    DueDate = dto.DueDate,
                    OrderNumber = dto.OrderNumber,
                    InvoiceNumber = dto.InvoiceNumber,
                    CustomerName = dto.CustomerName,
                    Address1 = dto.Address1,
                    Address2 = dto.Address2,
                    ContactNumber = dto.ContactNumber,
                    ContactEmail = dto.ContactEmail,
                    PaymentMethod = (int)dto.PaymentMethod,
                    SubTotal = dto.SubTotal,
                    Tax = dto.Tax,
                    Shipping = dto.Shipping,
                    Total = dto.Total,
                    BookAccountNumber = dto.BookAccountNumber,
                    Status = (int)dto.Status,
                    DeliveryStatus = (int)dto.DeliveryStatus,
                };

                context.Orders.Add(item);

                if (dto.Items != null && dto.Items.Any())
                {
                    foreach (var itemDto in dto.Items)
                    {
                        new OrderItemRepo().AddToDB(item, itemDto);
                    }
                }

                Db.SaveChanges(context, result, "Your order has been placed.");

            }

            return result;
        }

        public ReturnValue Cancel(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Orders.FirstOrDefault(a => a.Id == id);
                if (record != null)
                {
                    record.Status = (int)OrderStatusEnum.Cancelled;
                    Db.SaveChanges(context, result, "Order successfully cancelled!");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Order item not found.";
                }
            }

            return result;
        }

        public ReturnValue Delete(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Orders.FirstOrDefault(a => a.Id == id);
                if (record != null)
                {
                    context.Orders.Remove(record);
                    Db.SaveChanges(context, result, "Order deleted!");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Order item not found.";
                }
            }

            return result;
        }








    }
}
