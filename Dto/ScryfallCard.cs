using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class ScryfallCard
    {
        [JsonProperty("object")]
        public string Object { get; set; } = string.Empty;

        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("oracle_id")]
        public Guid OracleId { get; set; } = Guid.Empty;

        [JsonProperty("multiverse_ids")]
        public List<int> MultiverseIds { get; set; } = new List<int>();

        [JsonProperty("mtgo_id")]
        public int? MtgoId { get; set; }

        [JsonProperty("arena_id")]
        public int? ArenaId { get; set; }

        [JsonProperty("tcgplayer_id")]
        public int? TcgplayerId { get; set; }

        [JsonProperty("cardmarket_id")]
        public int? CardmarketId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("lang")]
        public string Lang { get; set; } = "en";

        [JsonProperty("released_at")]
        public DateTime ReleasedAt { get; set; } = DateTime.MinValue;

        [JsonProperty("uri")]
        public string Uri { get; set; } = string.Empty;

        [JsonProperty("scryfall_uri")]
        public string ScryfallUri { get; set; } = string.Empty;

        [JsonProperty("layout")]
        public string Layout { get; set; } = string.Empty;

        [JsonProperty("highres_image")]
        public bool HighresImage { get; set; } = false;

        [JsonProperty("image_status")]
        public string ImageStatus { get; set; } = string.Empty;

        [JsonProperty("cmc")]
        public decimal Cmc { get; set; } = 0m;

        [JsonProperty("type_line")]
        public string TypeLine { get; set; } = string.Empty;

        [JsonProperty("color_identity")]
        public List<string> ColorIdentity { get; set; } = new List<string>();

        [JsonProperty("keywords")]
        public List<string> Keywords { get; set; } = new List<string>();

        [JsonProperty("produced_mana")]
        public List<string> ProducedMana { get; set; } = new List<string>();

        [JsonProperty("card_faces")]
        public List<CardFace> CardFaces { get; set; } = new List<CardFace>();

        [JsonProperty("legalities")]
        public Legalities Legalities { get; set; } = new Legalities();

        [JsonProperty("games")]
        public List<string> Games { get; set; } = new List<string>();

        [JsonProperty("reserved")]
        public bool Reserved { get; set; } = false;

        [JsonProperty("game_changer")]
        public bool GameChanger { get; set; } = false;

        [JsonProperty("foil")]
        public bool Foil { get; set; } = false;

        [JsonProperty("nonfoil")]
        public bool Nonfoil { get; set; } = false;

        [JsonProperty("finishes")]
        public List<string> Finishes { get; set; } = new List<string>();

        [JsonProperty("set_id")]
        public Guid SetId { get; set; } = Guid.Empty;

        [JsonProperty("set")]
        public string Set { get; set; } = string.Empty;

        [JsonProperty("set_name")]
        public string SetName { get; set; } = string.Empty;

        [JsonProperty("rarity")]
        public string Rarity { get; set; } = string.Empty;

        [JsonProperty("collector_number")]
        public string CollectorNumber { get; set; } = string.Empty;

        [JsonProperty("prices")]
        public Prices Prices { get; set; } = new Prices();

        [JsonProperty("preview")]
        public Preview Preview { get; set; } = new Preview();

        [JsonProperty("related_uris")]
        public RelatedUris RelatedUris { get; set; } = new RelatedUris();

        [JsonProperty("purchase_uris")]
        public PurchaseUris PurchaseUris { get; set; } = new PurchaseUris();

        [JsonProperty("image_uris")]
        public ImageUris ImageUris { get; set; } = new ImageUris();

        [JsonProperty("oracle_text")]
        public string OracleText { get; set; } = string.Empty;

        [JsonProperty("mana_cost")]
        public string ManaCost { get; set; } = string.Empty;

        [JsonProperty("artist")]
        public string Artist { get; set; } = string.Empty;

        [JsonProperty("flavor_text")]
        public string FlavorText { get; set; } = string.Empty;
    }

    public class CardFace
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mana_cost")]
        public string ManaCost { get; set; }

        [JsonProperty("type_line")]
        public string TypeLine { get; set; }

        [JsonProperty("oracle_text")]
        public string OracleText { get; set; }

        [JsonProperty("colors")]
        public List<string> Colors { get; set; }

        [JsonProperty("power")]
        public string Power { get; set; }

        [JsonProperty("toughness")]
        public string Toughness { get; set; }

        [JsonProperty("flavor_text")]
        public string FlavorText { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("artist_id")]
        public Guid ArtistId { get; set; }

        [JsonProperty("illustration_id")]
        public Guid IllustrationId { get; set; }

        [JsonProperty("image_uris")]
        public ImageUris ImageUris { get; set; }
    }

    public class ImageUris
    {
        [JsonProperty("small")]
        public string Small { get; set; }

        [JsonProperty("normal")]
        public string Normal { get; set; }

        [JsonProperty("large")]
        public string Large { get; set; }

        [JsonProperty("png")]
        public string Png { get; set; }

        [JsonProperty("art_crop")]
        public string ArtCrop { get; set; }

        [JsonProperty("border_crop")]
        public string BorderCrop { get; set; }
    }
    public class Legalities
    {
        [JsonProperty("standard")]
        public string Standard { get; set; } = "not_legal";

        [JsonProperty("future")]
        public string Future { get; set; } = "not_legal";

        [JsonProperty("historic")]
        public string Historic { get; set; } = "not_legal";

        [JsonProperty("timeless")]
        public string Timeless { get; set; } = "not_legal";

        [JsonProperty("gladiator")]
        public string Gladiator { get; set; } = "not_legal";

        [JsonProperty("pioneer")]
        public string Pioneer { get; set; } = "not_legal";

        [JsonProperty("modern")]
        public string Modern { get; set; } = "not_legal";

        [JsonProperty("legacy")]
        public string Legacy { get; set; } = "not_legal";

        [JsonProperty("pauper")]
        public string Pauper { get; set; } = "not_legal";

        [JsonProperty("vintage")]
        public string Vintage { get; set; } = "not_legal";

        [JsonProperty("penny")]
        public string Penny { get; set; } = "not_legal";

        [JsonProperty("commander")]
        public string Commander { get; set; } = "not_legal";

        [JsonProperty("oathbreaker")]
        public string Oathbreaker { get; set; } = "not_legal";

        [JsonProperty("standardbrawl")]
        public string StandardBrawl { get; set; } = "not_legal";

        [JsonProperty("brawl")]
        public string Brawl { get; set; } = "not_legal";

        [JsonProperty("alchemy")]
        public string Alchemy { get; set; } = "not_legal";

        [JsonProperty("paupercommander")]
        public string PauperCommander { get; set; } = "not_legal";

        [JsonProperty("duel")]
        public string Duel { get; set; } = "not_legal";

        [JsonProperty("oldschool")]
        public string Oldschool { get; set; } = "not_legal";

        [JsonProperty("premodern")]
        public string Premodern { get; set; } = "not_legal";

        [JsonProperty("predh")]
        public string Predh { get; set; } = "not_legal";
    }

    public class Prices
    {
        [JsonProperty("usd")]
        public string Usd { get; set; }

        [JsonProperty("usd_foil")]
        public string UsdFoil { get; set; }

        [JsonProperty("eur")]
        public string Eur { get; set; }

        [JsonProperty("eur_foil")]
        public string EurFoil { get; set; }

        [JsonProperty("tix")]
        public string Tix { get; set; }
    }
    public class Preview
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("source_uri")]
        public string SourceUri { get; set; }

        [JsonProperty("previewed_at")]
        public DateTime PreviewedAt { get; set; }
    }

    public class RelatedUris
    {
        [JsonProperty("gatherer")]
        public string Gatherer { get; set; }

        [JsonProperty("edhrec")]
        public string Edhrec { get; set; }
    }

    public class PurchaseUris
    {
        [JsonProperty("tcgplayer")]
        public string Tcgplayer { get; set; }

        [JsonProperty("cardmarket")]
        public string Cardmarket { get; set; }

        [JsonProperty("cardhoarder")]
        public string Cardhoarder { get; set; }
    }

}
