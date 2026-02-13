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
    }

    public class MtgCardDetailsViewModel : CardDetailsViewModel
    {
        public MtgCardDetailsViewModel() : base()
        {
        }

        public ScryfallCard ScryfallCard { get; set; } = new ScryfallCard();
    }

    public class GaCardDetailsViewModel : CardDetailsViewModel
    {
        public GaCardDetailsViewModel() : base()
        {
        }

        public GrandArchiveCard GrandArchiveCard { get; set; } = new GrandArchiveCard();
    }
}