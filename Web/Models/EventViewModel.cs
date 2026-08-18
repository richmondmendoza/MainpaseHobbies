using Dto.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Web.Utils;

namespace Web.Models
{
    public class EventViewModel
    {
        public EventViewModel() { }
        public EventViewModel(EventDetailsDto record = null)
        {
            if (record != null)
            {
                Id = record.Id;
                DateCreated = record.DateCreated;
                DateOfEvent = record.DateOfEvent;
                Body = record.Body;
                FeaturedImage = record.FeaturedImage;
                IsActive = record.IsActive;
                SubTitle = record.SubTitle;
                Title = record.Title;
                MimeType = record.MimeType;
                IsFeatured = record.IsFeatured;

                FeaturedImageFile = (HttpPostedFileBase)new MemoryPostedFile(this.FeaturedImage);
            }
        }

        public int Id { get; set; } = 0;
        public byte[] FeaturedImage { get; set; } = new byte[0];
        public HttpPostedFileBase FeaturedImageFile { get; set; }

        [Required(ErrorMessage = "Date of event is required.")]
        public DateTime DateOfEvent { get; set; } = DateTime.Now;
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [AllowHtml]
        public string Body { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event title is required.")]
        public string Title { get; set; } = string.Empty;

        [AllowHtml]
        public string SubTitle { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;

        public string FeaturedImageBase64 { get { return Convert.ToBase64String(FeaturedImage); } }
        public string MimeType { get; set; } = string.Empty;

        public IEnumerable<EventImagesDto> EventImages { get; set; } = new List<EventImagesDto>();

        public EventDto ToDto()
        {
            if (this.FeaturedImageFile != null)
            {
                using (Stream inputStream = this.FeaturedImageFile.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    this.FeaturedImage = memoryStream.ToArray();
                }
            }

            return new EventDto()
            {
                Id = this.Id,
                FeaturedImage = this.FeaturedImage,
                MimeType = this.MimeType,
                Title = this.Title,
                SubTitle = this.SubTitle,
                Body = this.Body,
                DateCreated = this.DateCreated,
                DateOfEvent = this.DateOfEvent,
                IsActive = this.IsActive,
                IsFeatured = this.IsFeatured,
            };
        }
    }

    public class EventImageViewModel
    {
        public EventImageViewModel() { }

        public EventImageViewModel(EventImagesDto dto)
        {
            if (dto != null)
            {
                this.Id = dto.Id;
                this.EventId = dto.EventId;
                this.Image = dto.Image;
                this.MimeType = dto.MimeType;

                ImageFile = (HttpPostedFileBase)new MemoryPostedFile(this.Image);
            }
        }

        public int Id { get; set; } = 0;
        public int EventId { get; set; } = 0;
        public byte[] Image { get; set; } = new byte[0];
        public HttpPostedFileBase ImageFile { get; set; }

        public string ImageBase64 { get { return Convert.ToBase64String(Image); } }
        public string MimeType { get; set; } = string.Empty;

        public EventImagesDto ToDto()
        {
            if (this.ImageFile != null)
            {
                using (Stream inputStream = this.ImageFile.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    this.Image = memoryStream.ToArray();
                }
            }

            return new EventImagesDto()
            {
                Id = Id,
                EventId = EventId,
                Image = Image,
                MimeType = MimeType,
            };
        }

    }
}