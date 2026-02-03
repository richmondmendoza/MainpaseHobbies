using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.BaseSettings
{
    public class ConversionDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Amount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
