using Database.SQL;
using Dto;
using Dto.BaseSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo.Settings
{
    public class BannerRepo
    {
        public BannerDto ToDto(System_Banner item)
        {
            if (item == null) return null;

            return new BannerDto()
            {
                Id = item.Id,
                Image = item.Image,
            };
        }

        public IEnumerable<BannerDto> GetList()
        {
            using (var context = new IMSEntities())
            {
                var items = context.System_Banner.ToList();
                return items.Select(x => ToDto(x)).ToList();
            }
        }

        public ReturnValue Save(BannerDto dto)
        {
            var result = new ReturnValue();
            using (var context = new IMSEntities())
            {
                var item = new System_Banner()
                {
                    Image = dto.Image,
                };
                context.System_Banner.Add(item);

                Db.SaveChanges(context, result, "Banner successfully added");

                if (result.Success)
                {
                    if (!Directory.Exists(StoragePath.BannerImageStoragePath))
                    {
                        Directory.CreateDirectory(StoragePath.BannerImageStoragePath);
                    }

                    if (dto.Image != null && dto.Image.Length > 0)
                    {
                        var path = Path.Combine(StoragePath.BannerImageStoragePath, $"{item.Id.ToString()}.png");
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        File.WriteAllBytes(path, dto.Image);
                    }
                }
            }
            return result;

        }

        public ReturnValue Delete(int id)
        {
            var result = new ReturnValue();
            using (var context = new IMSEntities())
            {
                var item = context.System_Banner.FirstOrDefault(x => x.Id == id);
                if (item == null)
                {
                    result.Success = false;
                    result.Message = "Banner not found.";
                    return result;
                }
                else
                {
                    context.System_Banner.Remove(item);
                    Db.SaveChanges(context, result, "Banner successfully removed");

                    if (result.Success)
                    {
                        var path = Path.Combine(StoragePath.BannerImageStoragePath, $"{item.Id.ToString()}.png");
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                }
            }
            return result;

        }

    }
}
