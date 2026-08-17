using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Dto.BaseSettings
{
    public class StoragePath
    {
        public static string CardImageStoragePath = ConfigurationSettings.AppSettings["CardImageStorage"] ?? "C:\\CardImages";

        public static string EventImageStoragePath = HttpContext.Current?.Server?.MapPath("~/Content/website_img/event_img") ?? "";

        public static string BannerImageStoragePath = HttpContext.Current?.Server?.MapPath("~/Content/website_img/banner_img") ?? "";
    }

    public class SystemInfoDto
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string LongName { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber1 { get; set; } = string.Empty;
        public string ContactNumber2 { get; set; } = string.Empty;
        public string ContactEmail1 { get; set; } = string.Empty;
        public string ContactEmail2 { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string WorkingHour { get; set; } = string.Empty;
        public string FB { get; set; } = string.Empty;
        public string Insta { get; set; } = string.Empty;
        public string Twitter { get; set; } = string.Empty;
        public string LinkeIn { get; set; } = string.Empty;
    }
}
