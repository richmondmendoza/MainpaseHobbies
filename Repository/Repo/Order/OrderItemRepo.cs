using Database.SQL;
using Dto;
using Dto.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo.Order
{
    public class OrderItemRepo
    {
        public static OrderItemDto ToDto(Order_Item item)
        {
            if (item == null) return null;

            return new OrderItemDto
            {
                Id = item.Id,
                OrderId = item.OrderId,
                ProductName = item.ProductName,
                SubName = item.SubName,
                SerialNumber = item.SerialNumber,
                Description = item.Description,
                Quantity = item.Quantity,
                Price = item.Price,
                Total = item.Total
            };
        }

        public static IEnumerable<OrderItemDto> GetList(IMSEntities context, int orderId)
        {
            var items = context.Order_Item.Where(a => a.OrderId == orderId).ToList();
            return items.Select(a => ToDto(a));
        }

        public static IEnumerable<OrderItemDto> GetList(int orderId)
        {
            using (IMSEntities context = new IMSEntities())
            {
                return GetList(context, orderId);
            }
        }

        public static OrderItemDto Get(IMSEntities context, int id)
        {
            var item = context.Order_Item.FirstOrDefault(a => a.Id == id);
            return ToDto(item);
        }

        public static OrderItemDto Get(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                return Get(context, id);
            }
        }

        public void AddToDB(Database.SQL.Order order, OrderItemDto dto)
        {
            var result = new ReturnValue();

            var item = new Order_Item
            {
                ProductName = dto.ProductName,
                SubName = dto.SubName,
                SerialNumber = dto.SerialNumber,
                Description = dto.Description,
                Quantity = dto.Quantity,
                Price = dto.Price,
                Total = dto.Total
            };

            order.Order_Item.Add(item);
        }

        public ReturnValue Add(OrderItemDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Orders.FirstOrDefault(a => a.Id == dto.OrderId);
                if (record != null)
                {
                    AddToDB(record, dto);
                    Db.SaveChanges(context, result, "Successfully added!");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Order details not found.";
                }

                return result;
            }

        }


        public ReturnValue Update(OrderItemDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Order_Item.FirstOrDefault(a => a.Id == dto.Id);
                if (record != null)
                {
                    record.ProductName = dto.ProductName;
                    record.SubName = dto.SubName;
                    record.SerialNumber = dto.SerialNumber;
                    record.Description = dto.Description;
                    record.Quantity = dto.Quantity;
                    record.Price = dto.Price;
                    record.Total = dto.Total;

                    Db.SaveChanges(context, result, "Successfully updated!");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Order item not found.";
                }

                return result;
            }

        }

        public ReturnValue Delete(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Order_Item.FirstOrDefault(a => a.Id == id);
                if (record != null)
                {
                    context.Order_Item.Remove(record);
                    Db.SaveChanges(context, result, "Order item deleted!");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Order item not found.";
                }

                return result;
            }

        }


    }
}
