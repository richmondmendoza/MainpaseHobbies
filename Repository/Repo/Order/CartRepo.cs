using Database.SQL;
using Dto;
using Dto.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Repository.Repo.Order
{
    public class CartRepo
    {
        public static CartDto ToDto(Cart item)
        {
            if (item == null) return null;

            return new CartDto
            {
                Id = item.Id,
                UserId = item.UserId,
                InventoryId = item.InventoryId,
                DateCreated = item.DateCreated,
                Quantity = item.Quantity,
                UserSessionId = item.UserSessionId,
            };
        }

        public static CartDetailsDto ToDetailsDto(Cart item)
        {
            if (item == null) return null;

            var dto = new CartDetailsDto(ToDto(item));
            dto.ProductName = item.Inventory.Name;
            dto.Price = item.Inventory.Price;
            dto.Stock = (int)item.Inventory.Inventory_Count.Sum(a => a.Quantity);
            dto.FoilType = item.Inventory.FoilType;
            dto.Condition = item.Inventory.Condition;
            dto.ImageData = item.Inventory.Image;
            dto.Currency = item.Inventory.PurchaseCurrency;

            return dto;
        }

        public static IEnumerable<CartDetailsDto> GetList(int userId, string userSessionId = "")
        {
            using (IMSEntities context = new IMSEntities())
            {
                var list = context.Carts.Where(a => a != null);

                if (userId > 0)
                {
                    list = list.Where(a => a.UserId == userId);
                }
                else if (!string.IsNullOrEmpty(userSessionId))
                {
                    list = list.Where(a => !(a.UserId > 0) & a.UserSessionId == userSessionId);
                }
                else
                {
                    return new List<CartDetailsDto>();
                }

                return list.ToList().Select(x => ToDetailsDto(x)).ToList();
            }
        }

        public static CartDetailsDto Get(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var item = context.Carts.FirstOrDefault(a => a.Id == id);
                return ToDetailsDto(item);
            }
        }

        public ReturnValue Add(CartDto dto)
        {
            var result = new ReturnValue();
            using (IMSEntities context = new IMSEntities())
            {
                var item = new Cart
                {
                    UserId = dto.UserId,
                    InventoryId = dto.InventoryId,
                    DateCreated = dto.DateCreated,
                    Quantity = dto.Quantity,
                    UserSessionId = dto.UserSessionId,
                };
                context.Carts.Add(item);

                Db.SaveChanges(context, result, "Cart item added successfully.");
            }

            return result;
        }

        public ReturnValue Update(CartDto dto)
        {
            var result = new ReturnValue();
            using (IMSEntities context = new IMSEntities())
            {
                var item = context.Carts.FirstOrDefault(a => a.Id == dto.Id);
                if (item != null)
                {
                    item.Quantity = dto.Quantity;
                    Db.SaveChanges(context, result, "Cart item updated successfully.");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Cart item not found.";
                }
            }
            return result;
        }

        public ReturnValue Delete(int id)
        {
            var result = new ReturnValue();
            using (IMSEntities context = new IMSEntities())
            {
                var item = context.Carts.FirstOrDefault(a => a.Id == id);
                if (item != null)
                {
                    context.Carts.Remove(item);
                    Db.SaveChanges(context, result, "Cart item deleted successfully.");
                }
                else
                {
                    result.Success = false;
                    result.Message = "Cart item not found.";
                }
            }
            return result;

        }

        public void DeleteTemporaryCarts()
        {
            using (IMSEntities context = new IMSEntities())
            {
                var date = DateTime.Now.AddDays(-7);
                var items = context.Carts.Where(a => !(a.UserId == 0) & a.DateCreated <= date).ToList();
                if (items.Any())
                {
                    context.Carts.RemoveRange(items);
                    Db.SaveChanges(context);
                }
            }
        }

    }
}