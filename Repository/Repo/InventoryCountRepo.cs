using Database.SQL;
using Dto;
using Dto.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class InventoryCountRepo
    {
        public InventoryCountDto ToDto(Inventory_Count inventoryCount)
        {
            if (inventoryCount == null) return null;
            return new InventoryCountDto
            {
                Id = inventoryCount.Id,
                InventoryId = inventoryCount.InventoryId,
                DateCreated = inventoryCount.DateCreated,
                CreatedBy = inventoryCount.CreatedBy,
                IsDeleted = inventoryCount.IsDeleted,
                Quantity = inventoryCount.Quantity,
                Remarks = inventoryCount.Remarks,
                Type = (Dto.Enums.InventoryCountTypeEnum)inventoryCount.Type,
                UOM = inventoryCount.UOM
            };
        }

        public InventoryCountDto GetById(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Inventory_Count.FirstOrDefault(a => a.InventoryId == id);

                return ToDto(record);
            }
        }

        public IEnumerable<InventoryCountDto> GetByInventoryId(int inventoryId)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Inventories.Where(a => a.Id == inventoryId && !a.IsDeleted)
                    .Include(a => a.Inventory_Count.Where(b => !b.IsDeleted)).FirstOrDefault();

                if(record == null || record.Inventory_Count == null)
                    return new List<InventoryCountDto>();

                return record.Inventory_Count.ToList().Select(a => ToDto(a));
            }
        }


        public void Create(IMSEntities context, InventoryCountDto dto)
        {
            var newRecord = new Inventory_Count
            {
                InventoryId = dto.InventoryId,
                DateCreated = dto.DateCreated,
                CreatedBy = dto.CreatedBy,
                IsDeleted = dto.IsDeleted,
                Quantity = dto.Quantity,
                Remarks = dto.Remarks,
                Type = (int)dto.Type,
                UOM = dto.UOM
            };

            context.Inventory_Count.Add(newRecord);
        }

        public void Create(Inventory inventory, InventoryCountDto dto)
        {
            var newRecord = new Inventory_Count
            {
                InventoryId = dto.InventoryId,
                DateCreated = dto.DateCreated,
                CreatedBy = dto.CreatedBy,
                IsDeleted = dto.IsDeleted,
                Quantity = dto.Quantity,
                Remarks = dto.Remarks,
                Type = (int)dto.Type,
                UOM = dto.UOM
            };

            inventory.Inventory_Count.Add(newRecord);
        }

        public ReturnValue Create(InventoryCountDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                Create(context, dto);
                Db.SaveChanges(context, result,
                        "Inventory count record created successfully.",
                        "The inventory count record you are trying to create has been modified or deleted by another user. Please reload and try again.",
                        "An error occurred while trying to create the inventory count record.");
            }

            return result;
        }

        public ReturnValue Update(InventoryCountDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Inventory_Count.FirstOrDefault(a => a.Id == dto.Id);

                if (record == null)
                    return new ReturnValue("The inventory count record you are trying to update does not exist. It may have been deleted by another user. Please reload and try again.");

                record.Quantity = dto.Quantity;
                record.UOM = dto.UOM;
                record.Remarks = dto.Remarks;
                record.Type = (int)dto.Type;

                Db.SaveChanges(context, result,
                        "Inventory count record updated successfully.",
                        "The inventory count record you are trying to update has been modified or deleted by another user. Please reload and try again.",
                        "An error occurred while trying to update the inventory count record.");
            }

            return result;
        }

        public ReturnValue Delete(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Inventory_Count.FirstOrDefault(a => a.Id == id);

                if (record == null)
                    return new ReturnValue("The inventory count record you are trying to update does not exist. It may have been deleted by another user. Please reload and try again.");

                record.IsDeleted = true;

                Db.SaveChanges(context, result,
                        "Inventory count record deleted successfully.",
                        "The inventory count record you are trying to delete has been modified or deleted by another user. Please reload and try again.",
                        "An error occurred while trying to delete the inventory count record.");
            }

            return result;
        }
    }
}
