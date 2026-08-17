using Database.SQL;
using Dto;
using Dto.BaseSettings;
using Dto.Dto;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Repository.Repo
{
    public class EventRepo
    {
        public static EventDto ToDto(Event item)
        {
            if (item == null) return null;

            return new EventDto()
            {
                Id = item.Id,
                DateCreated = item.DateCreated,
                DateOfEvent = item.DateOfEvent.Value,
                Body = item.Body,
                FeaturedImage = item.FeaturedImage,
                IsActive = item.IsActive,
                SubTitle = item.SubTitle,
                Title = item.Title,
                MimeType = item.FeaturedImage.GetImageExtension().Replace(".", ""),
                IsFeatured = item.IsFeatured,
            };
        }

        public static EventDetailsDto ToEventDetailsDto(Event item)
        {
            if (item == null) return null;

            var model = new EventDetailsDto(ToDto(item));
            model.EventImages = item.Event_Image.ToList().Select(a => ToEventImageDto(a)).ToList();

            return model;
        }

        public static EventImagesDto ToEventImageDto(Event_Image item)
        {
            if (item == null) return null;

            return new EventImagesDto()
            {
                Id = item.Id,
                EventId = item.Id,
                Image = item.Image,
                MimeType = item.Image.GetImageExtension(),
            };
        }

        public static IEnumerable<EventDto> GetListFeatured()
        {
            using (IMSEntities context = new IMSEntities())
            {
                var items = context.Events.Where(a => a.IsActive & a.IsFeatured).ToList().Select(a => ToDto(a));
                return items.ToList();
            }
        }

        public static IEnumerable<EventDto> GetList()
        {
            using (IMSEntities context = new IMSEntities())
            {
                var items = context.Events.Where(a => a.IsActive).ToList().Select(a => ToDto(a));
                return items.ToList();
            }
        }

        public static EventDetailsDto GetDetails(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Events.Where(a => a.Id == id).Include(a => a.Event_Image).FirstOrDefault();
                return ToEventDetailsDto(record);
            }
        }

        public ReturnValue Add(EventDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var newItem = new Event()
                {
                    DateCreated = DateTime.Now,
                    DateOfEvent = dto.DateOfEvent,
                    Body = dto.Body,
                    FeaturedImage = dto.FeaturedImage,
                    IsActive = dto.IsActive,
                    SubTitle = dto.SubTitle,
                    Title = dto.Title,
                    IsFeatured = dto.IsFeatured,
                };

                context.Events.Add(newItem);
                Db.SaveChanges(context, result, "Event successfully created!");
                result.Data = ToDto(newItem);

                if (result.Success)
                {
                    if (!Directory.Exists(StoragePath.EventImageStoragePath))
                    {
                        Directory.CreateDirectory(StoragePath.EventImageStoragePath);
                    }

                    if (dto.FeaturedImage != null && dto.FeaturedImage.Length > 0)
                    {
                        var path = Path.Combine(StoragePath.EventImageStoragePath, $"{newItem.Id.ToString()}.png");
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        File.WriteAllBytes(path, dto.FeaturedImage);
                    }
                }
            }

            return result;
        }

        public ReturnValue Update(EventDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Events.FirstOrDefault(a => a.Id == dto.Id);

                if (record == null)
                    return new ReturnValue("Event details not found!");

                if (dto.FeaturedImage.Length > 0)
                    record.FeaturedImage = dto.FeaturedImage;

                record.DateOfEvent = dto.DateOfEvent;
                record.Body = dto.Body;
                record.SubTitle = dto.SubTitle;
                record.Title = dto.Title;
                record.IsFeatured = dto.IsFeatured;

                Db.SaveChanges(context, result, "Event successfully updated!");
                result.Data = ToDto(record);

                if (result.Success)
                {
                    if (!Directory.Exists(StoragePath.EventImageStoragePath))
                    {
                        Directory.CreateDirectory(StoragePath.EventImageStoragePath);
                    }


                    if (record.FeaturedImage != null && record.FeaturedImage.Length > 0)
                    {
                        var path = Path.Combine(StoragePath.EventImageStoragePath, $"{record.Id.ToString()}.png");
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        File.WriteAllBytes(path, record.FeaturedImage);
                    }
                }
            }

            return result;
        }

        public ReturnValue Delete(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Events.FirstOrDefault(a => a.Id == id);

                if (record == null)
                    return new ReturnValue("Event details not found!");

                record.IsActive = false;
                Db.SaveChanges(context, result, "Event successfully deleted!");
                result.Data = ToDto(record);
            }

            return result;
        }

    }
}
