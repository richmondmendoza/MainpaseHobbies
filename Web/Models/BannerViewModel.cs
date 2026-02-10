using Dto.BaseSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class BannerViewModel
    {
        [Required(ErrorMessage = "Please select an image file.")]
        public HttpPostedFileBase ImageFile { get; set; }

        public BannerDto ToDto()
        {
            var dto = new BannerDto();

            if (ImageFile != null)
            {
                using (Stream inputStream = this.ImageFile.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    dto.Image = memoryStream.ToArray();
                }
            }

            return dto;
        }
    }
}