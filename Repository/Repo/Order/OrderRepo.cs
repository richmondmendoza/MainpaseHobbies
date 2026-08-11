using Database.SQL;
using Dto;
using Dto.Dto;
using Dto.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo.Order
{
    public class OrderRepo
    {
        public OrderDto ToDto(Database.SQL.Order item)
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
                DeliveryMethod = (DeliveryMethodEnum)item.DeliveryMethod,
                DeliveryNote = item.DeliveryNote,
                Currency = item.Currency,
                IsPaid = item.IsPaid,
            };
        }

        public OrderDetailsDto ToDetailsDto(Database.SQL.Order item)
        {
            if (item == null) return null;
            var dto = new OrderDetailsDto(ToDto(item));
            dto.Items = item.Order_Item.ToList().Select(x => OrderItemRepo.ToDto(x)).ToList();

            return dto;
        }

        public IEnumerable<OrderDto> GetList(int status = (int)OrderStatusEnum.Pending)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var list = context.Orders.ToList();
                return list.Select(x => ToDto(x)).ToList();
            }
        }

        public IEnumerable<OrderDto> GetListAllByFilter(string status)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var statuses = status.Split('|').Where(a => !string.IsNullOrEmpty(a)).Select(a => Convert.ToInt32(a));
                var list = context.Orders.Where(a => statuses.Any(b => b == a.Status));


                return list.ToList().Select(x => ToDto(x)).ToList();
            }
        }

        public IEnumerable<OrderDto> GetListByUser(int userid, string status)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var statuses = status.Split('|').Where(a => !string.IsNullOrEmpty(a)).Select(a => Convert.ToInt32(a));
                var list = context.Orders.Where(a => a.UserId == userid & statuses.Any(b => b == a.Status));

                return list.ToList().Select(x => ToDto(x)).ToList();
            }
        }

        public IEnumerable<OrderDetailsDto> GetListDetails()
        {
            using (IMSEntities context = new IMSEntities())
            {
                var list = context.Orders.ToList();
                return list.Select(x => ToDetailsDto(x)).ToList();
            }
        }

        public OrderDetailsDto Get(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var item = context.Orders.FirstOrDefault(a => a.Id == id);
                return ToDetailsDto(item);
            }
        }

        public decimal GetRefundedAmount(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var items = context.Payments.Where(a => a.OrderId == id & a.Status == (int)PaymentStatus.Refunded).ToList();
                return items.ToList().Sum(a => a.Amount);
            }
        }

        public OrderDetailsDto GetByOrderNumber(string orderNumber)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var item = context.Orders.FirstOrDefault(a => a.OrderNumber == orderNumber);
                return ToDetailsDto(item);
            }
        }

        public ReturnValue Add(OrderDetailsDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var date = dto.DateCreated.Date;
                var count = context.Orders.Where(a => a.DateCreated >= date).Count() + 1;

                if (string.IsNullOrEmpty(dto.OrderNumber))
                    dto.OrderNumber = $"ORD{DateTime.UtcNow:yyyyMMddHHmmssfff}".Substring(0, 19);

                var item = new Database.SQL.Order
                {
                    UserId = dto.UserId,
                    DateCreated = dto.DateCreated,
                    DueDate = dto.DueDate,
                    OrderNumber = dto.OrderNumber,
                    InvoiceNumber = dto.InvoiceNumber ?? "",
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
                    BookAccountNumber = dto.BookAccountNumber ?? "",
                    Status = (int)dto.Status,
                    DeliveryStatus = (int)dto.DeliveryStatus,
                    DeliveryMethod = (int)dto.DeliveryMethod,
                    DeliveryNote = dto.DeliveryNote,
                    Currency = dto.Currency,
                    IsPaid = dto.IsPaid
                };

                context.Orders.Add(item);

                if (dto.Items != null && dto.Items.Any())
                {
                    foreach (var itemDto in dto.Items)
                    {
                        new OrderItemRepo().AddToDB(item, itemDto);

                        var cartItem = context.Carts.FirstOrDefault(a => a.Id == itemDto.Id);
                        context.Carts.Remove(cartItem);
                    }
                }

                Db.SaveChanges(context, result, "Your order has been placed.");
                result.Data = item.Id;
            }

            return result;
        }

        public ReturnValue Cancel(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Orders.Where(a => a.Id == id).Include(a => a.Order_Item).FirstOrDefault();
                if (record != null)
                {
                    if (record.IsPaid)
                    {
                        foreach (var item in record.Order_Item)
                        {
                            var remarks = $"Order #{record.OrderNumber}".ToString();
                            var inventoryCount = context.Inventory_Count.Where(a => a.Remarks != null).AsEnumerable().Where(a => a.Remarks.Equals(remarks)).ToList();
                            foreach (var inv in inventoryCount)
                            {
                                context.Inventory_Count.Remove(inv);
                            }
                        }
                    }

                    record.Status = (int)OrderStatusEnum.Cancelled;
                    Db.SaveChanges(context, result, "Order successfully cancelled!");
                    result.Data = ToDto(record);
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

        public ReturnValue Pay(int id, string orderId)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Orders.Where(a => a.Id == id).Include(a => a.Order_Item).FirstOrDefault();
                if (record != null)
                {
                    record.IsPaid = true;
                    record.Status = (int)OrderStatusEnum.Processing;

                    var payment = context.Payments.FirstOrDefault(a => a.OrderId == record.Id & a.PayoneerId == orderId);
                    if (payment != null)
                        payment.Status = (int)PaymentStatus.Paid;

                    foreach (var item in record.Order_Item)
                    {
                        var inventory = context.Inventories.FirstOrDefault(a => a.Name == item.ProductName);

                        if (inventory != null)
                        {
                            inventory.Inventory_Count.Add(new Inventory_Count()
                            {
                                CreatedBy = "system",
                                DateCreated = DateTime.Now,
                                Quantity = item.Quantity,
                                Type = (int)InventoryCountTypeEnum.Sell,
                                Remarks = $"Order #{record.OrderNumber}",
                                UOM = "PC",
                                IsDeleted = false,
                            });
                        }
                    }

                    Db.SaveChanges(context, result, "Order paid!");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Order item not found.";
                }
            }

            return result;
        }

        public ReturnValue Completed(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Orders.FirstOrDefault(a => a.Id == id);
                if (record != null)
                {
                    record.Status = (int)OrderStatusEnum.Completed;
                    Db.SaveChanges(context, result, "Order complete!");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Order item not found.";
                }
            }

            return result;
        }

        public ReturnValue Refunded(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Orders.FirstOrDefault(a => a.Id == id);
                if (record != null)
                {
                    if (record.IsPaid)
                    {
                        foreach (var item in record.Order_Item)
                        {
                            var remarks = $"Order #{record.OrderNumber}";
                            var inventoryCount = context.Inventory_Count.Where(a => a.Remarks != null).AsEnumerable().Where(a => a.Remarks.Equals(remarks)).ToList();
                            foreach (var inv in inventoryCount)
                            {
                                context.Inventory_Count.Remove(inv);
                            }
                        }
                    }

                    record.Status = (int)OrderStatusEnum.Refunded;
                    Db.SaveChanges(context, result, "Payment refunded!");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Order item not found.";
                }
            }

            return result;
        }

        public ReturnValue ForDelivery(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Orders.FirstOrDefault(a => a.Id == id);
                if (record != null)
                {
                    record.DeliveryStatus = (int)DeliveryStatusEnum.Processing;
                    Db.SaveChanges(context, result, "Pending for delivery!");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Order item not found.";
                }
            }

            return result;
        }

        public ReturnValue DeliveryCompleted(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Orders.FirstOrDefault(a => a.Id == id);
                if (record != null)
                {
                    record.DeliveryStatus = (int)DeliveryStatusEnum.Completed;
                    Db.SaveChanges(context, result, "Order delivered!");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Order item not found.";
                }
            }

            return result;
        }

        public IEnumerable<SalesDisplayDto> GetSalesDisplay(DateTime dateFrom, DateTime dateTo)
        {
            using (IMSEntities context = new IMSEntities())
            {
                dateFrom = dateFrom.Date;
                dateTo = dateTo.Date.AddDays(1).AddSeconds(-1);

                var records = context.Payments.Where(a => a.Status == (int)PaymentStatus.Paid | a.Status == (int)PaymentStatus.Refunded);
                records = records.Where(a => a.CreatedAt >= dateFrom && a.CreatedAt <= dateTo);

                var list = from p in records
                           join o in context.Orders on p.OrderId equals o.Id
                           select new SalesDisplayDto
                           {
                               OrderId = o.Id,
                               OrderNumber = o.OrderNumber,
                               PaymentDate = p.CreatedAt,
                               Amount = p.Amount,
                               Currency = o.Currency,
                               Remarks = p.PayoneerId,
                               IsRefund = p.Status == (int)PaymentStatus.Refunded
                           };

                return list.ToList();
            }

        }


    }
}
