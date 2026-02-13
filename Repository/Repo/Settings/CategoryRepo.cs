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
    public class CategoryRepo
    {
        public List<NameDto> GetCategories()
        {
            using (var context = new IMSEntities())
            {
                return context.Categories.Select(x => new NameDto
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
            }
        }

        public NameDto Get(int id)
        {
            using (var context = new IMSEntities())
            {
                var record = context.Categories.Where(a => a.Id == id).Select(x => new NameDto
                {
                    Id = x.Id,
                    Name = x.Name
                }).FirstOrDefault();

                if (record == null)
                    return null;

                return record;
            }
        }

        public ReturnValue Add(string name)
        {
            var result = new ReturnValue();
            using (var context = new IMSEntities())
            {
                var existing = context.Categories.FirstOrDefault(x => x.Name == name);
                if(existing != null)
                {
                    result.Success = false;
                    result.Message = "Category already exists.";
                    return result;
                }
                else
                {
                    context.Categories.Add(new Category
                    {
                        Name = name
                    });

                    Db.SaveChanges(context, result, "Category added successfully.");
                }
            }
            return result;
        }

        public ReturnValue update(NameDto dto)
        {
            var result = new ReturnValue("Unable to access category details.");
            using (var context = new IMSEntities())
            {
                var existing = context.Categories.FirstOrDefault(x => x.Id == dto.Id);
                if (existing != null)
                {
                    existing.Name = dto.Name;
                    Db.SaveChanges(context, result, "Category updated successfully.");
                }
            }
            return result;
        }

        public ReturnValue Delete(int id)
        {
            var result = new ReturnValue("Unable to access category details.");
            using (var context = new IMSEntities())
            {
                var existing = context.Categories.FirstOrDefault(x => x.Id == id);
                if (existing != null)
                {
                    context.Categories.Remove(existing);
                    Db.SaveChanges(context, result, "Category updated successfully.");
                }
            }
            return result;
        }



    }
}
