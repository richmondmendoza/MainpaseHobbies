using Dto;
using Dto.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class CardDetailsViewModel
    {
        public CardDetailsViewModel()
        {
        }

        public InventoryDetailsDto Details { get; set; } = new InventoryDetailsDto();

        public ScryfallCard ScryfallCard { get; set; } = new ScryfallCard();
    }
}