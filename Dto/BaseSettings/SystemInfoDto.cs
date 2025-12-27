using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.BaseSettings
{
    public class SystemInfoDto
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } =string.Empty;
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
