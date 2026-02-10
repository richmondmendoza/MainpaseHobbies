using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.BaseSettings
{
    public class BannerDto
    {
        public int Id { get; set; }
        public byte[] Image { get; set; } = new byte[0];

        public string ImageBase64
        {
            get
            {
                return Convert.ToBase64String(Image);
            }
        }
    }
}
