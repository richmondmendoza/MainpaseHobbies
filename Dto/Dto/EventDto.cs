using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Dto
{
    public class EventDto
    {
        public int Id { get; set; } = 0;
        public byte[] FeaturedImage { get; set; } = new byte[0];
        public DateTime DateOfEvent { get; set; } = DateTime.Now;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string Body { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        public string FeaturedImageBase64 { get { return Convert.ToBase64String(FeaturedImage); } }
        public string MimeType { get; set; } = string.Empty;
    }

    public class EventDetailsDto : EventDto
    {
        public EventDetailsDto() { }

        public EventDetailsDto(EventDto item)
        {
            if (item != null)
            {
                Id = item.Id;
                DateCreated = item.DateCreated;
                DateOfEvent = item.DateOfEvent;
                Body = item.Body;
                FeaturedImage = item.FeaturedImage;
                IsActive = item.IsActive;
                SubTitle = item.SubTitle;
                Title = item.Title;
                MimeType = item.MimeType;
                IsFeatured = item.IsFeatured;
            }
        }

        public IEnumerable<EventImagesDto> EventImages { get; set; } = new List<EventImagesDto>();
    }

    public class EventImagesDto
    {
        public int Id { get; set; } = 0;
        public int EventId { get; set; } = 0;
        public byte[] Image { get; set; } = new byte[0];

        public string ImageBase64 { get { return Convert.ToBase64String(Image); } }
        public string MimeType { get; set; } = string.Empty;
    }

}
