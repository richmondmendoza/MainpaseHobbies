using Database.SQL;
using Dto;
using Dto.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public static int GetCount(int userId, string userSessionId = "")
        {
            int count = 0;
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

                count = list.Count();
            }

            return count;
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
                var inventory = context.Inventories.Where(a => a.Id == dto.InventoryId).Include(a => a.Inventory_Count).FirstOrDefault();
                var exists = context.Carts.FirstOrDefault(a => a.InventoryId == dto.InventoryId);
                var limit = inventory?.Inventory_Count.Sum(a => a.Quantity) ?? 0;

                if (exists != null)
                {
                    var quantity = dto.Quantity + exists.Quantity;

                    if (quantity > limit)
                        return new ReturnValue("The selected quantity exceeds the current inventory stock. Please reload the page and try again.");

                    exists.Quantity = quantity;
                }
                else
                {
                    if (dto.Quantity > limit)
                        return new ReturnValue("The selected quantity exceeds the current inventory stock. Please reload the page and try again.");

                    var item = new Cart
                    {
                        UserId = dto.UserId,
                        InventoryId = dto.InventoryId,
                        DateCreated = dto.DateCreated,
                        Quantity = dto.Quantity,
                        UserSessionId = dto.UserSessionId,
                    };
                    context.Carts.Add(item);
                }

                Db.SaveChanges(context, result, "Item added to cart.");
            }

            return result;
        }

        public ReturnValue Update(int id, int quantity)
        {
            var result = new ReturnValue();
            using (IMSEntities context = new IMSEntities())
            {
                var item = context.Carts.FirstOrDefault(a => a.Id == id);
                if (item != null)
                {
                    var inventory = context.Inventories.Where(a => a.Id == item.InventoryId).Include(a => a.Inventory_Count).FirstOrDefault();
                    var limit = inventory?.Inventory_Count.Sum(a => a.Quantity) ?? 0;

                    if (quantity > limit)
                        return new ReturnValue("The selected quantity exceeds the current inventory stock. Please reload the page and try again.");

                    item.Quantity = quantity;
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

        public ReturnValue UpdateOnLogin(int userId, string userSessionKey)
        {
            var result = new ReturnValue();
            using (IMSEntities context = new IMSEntities())
            {
                var items = context.Carts.Where(a => !(a.UserId > 0) & a.UserSessionId == userSessionKey);
                foreach (var item in items)
                {
                    item.UserId = userId;
                }

                Db.SaveChanges(context, result, "Updated successfully.");
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