using Database.SQL;
using Dto;
using Dto.BaseSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo.Settings
{
    public class ConversionRepo
    {
        public static ConversionDto ToDto(Conversion item)
        {
            if (item == null)
                return null;

            return new ConversionDto()
            {
                Id = item.Id,
                Date = item.Date,
                Amount = item.Amount,
                IsActive = item.IsActive
            };
        }

        public static ConversionDto Get()
        {
            using (var context = new IMSEntities())
            {
                var item = context.Conversions.FirstOrDefault(c => c.IsActive);

                if (item == null)
                {
                    item = new Conversion()
                    {
                        Amount = 60,
                        Date = DateTime.Now,
                        IsActive = true,
                    };
                    context.Conversions.Add(item);
                    Db.SaveChanges(context);
                }

                return ToDto(item);
            }
        }

        public static ReturnValue SaveConversion(ConversionDto dto)
        {
            var result = new ReturnValue();
            using (var context = new IMSEntities())
            {
                var existingItems = context.Conversions.Where(c => c.IsActive).ToList();
                foreach (var existingItem in existingItems)
                {
                    existingItem.IsActive = false;
                }
                var newItem = new Conversion()
                {
                    Date = DateTime.Now,
                    Amount = dto.Amount,
                    IsActive = true
                };
                context.Conversions.Add(newItem);
                Db.SaveChanges(context, result, "Changes Saved!");

                result.Data = ToDto(newItem);
            }

            return result;
        }

    }
}
