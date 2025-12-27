using Dto.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Dto
{
    public class InventoryCountDto
    {
        public int Id { get; set; } = 0;
        public int InventoryId { get; set; } = 0;
        public decimal Quantity { get; set; } = 0;
        public string UOM { get; set; } = "PC(S)";
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public InventoryCountTypeEnum Type { get; set; } = InventoryCountTypeEnum.Sell;
        public string CreatedBy { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }
}
