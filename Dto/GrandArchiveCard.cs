using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dto
{
    public class GrandArchiveCard
    {
        [JsonPropertyName("classes")]
        public List<string> classes { get; set; } = new List<string>();

        [JsonPropertyName("cost_memory")]
        public int? cost_memory { get; set; } = 0;

        [JsonPropertyName("cost_reserve")]
        public int? cost_reserve { get; set; }

        [JsonPropertyName("cost")]
        public Cost cost { get; set; } = new Cost();

        [JsonPropertyName("created_at")]
        public DateTime? created_at { get; set; } = DateTime.Now;

        [JsonPropertyName("durability")]
        public int? durability { get; set; }

        [JsonPropertyName("editions")]
        public List<Edition> editions { get; set; } = new List<Edition>();

        [JsonPropertyName("effect")]
        public string effect { get; set; } = string.Empty;

        [JsonPropertyName("effect_raw")]
        public string effect_raw { get; set; } = string.Empty;

        [JsonPropertyName("element")]
        public string element { get; set; } = string.Empty;

        [JsonPropertyName("elements")]
        public List<string> elements { get; set; } = new List<string>();

        [JsonPropertyName("flavor")]
        public string flavor { get; set; } = string.Empty;

        [JsonPropertyName("last_update")]
        public DateTime? last_update { get; set; } = DateTime.Now;

        [JsonPropertyName("legality")]
        public object legality { get; set; } = new object();

        [JsonPropertyName("level")]
        public int? level { get; set; } = 0;

        [JsonPropertyName("life")]
        public int? life { get; set; } = 0;

        [JsonPropertyName("name")]
        public string name { get; set; } = string.Empty;

        [JsonPropertyName("power")]
        public int? power { get; set; }

        [JsonPropertyName("referenced_by")]
        public List<object> referenced_by { get; set; } = new List<object>();

        [JsonPropertyName("references")]
        public List<object> references { get; set; } = new List<object>();

        [JsonPropertyName("result_editions")]
        public List<Edition> result_editions { get; set; } = new List<Edition>();

        [JsonPropertyName("rule")]
        public List<object> rule { get; set; } = new List<object>();

        [JsonPropertyName("slug")]
        public string slug { get; set; } = string.Empty;

        [JsonPropertyName("speed")]
        public object speed { get; set; } = new object();

        [JsonPropertyName("subtypes")]
        public List<string> subtypes { get; set; } = new List<string>();

        [JsonPropertyName("types")]
        public List<string> types { get; set; } = new List<string>();

        [JsonPropertyName("uuid")]
        public string uuid { get; set; } = string.Empty;

        [JsonPropertyName("effect_html")]
        public string effect_html { get; set; } = string.Empty;
    }

    public class Cost
    {
        [JsonPropertyName("type")]
        public string type { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string value { get; set; } = string.Empty;
    }

    public class Edition
    {
        [JsonPropertyName("card_id")]
        public string card_id { get; set; } = string.Empty;

        [JsonPropertyName("collector_number")]
        public string collector_number { get; set; } = string.Empty;

        [JsonPropertyName("configuration")]
        public string configuration { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime? created_at { get; set; } = DateTime.Now;

        [JsonPropertyName("effect")]
        public string effect { get; set; } = string.Empty;

        [JsonPropertyName("effect_raw")]
        public string effect_raw { get; set; } = string.Empty;

        [JsonPropertyName("flavor")]
        public string flavor { get; set; } = string.Empty;

        [JsonPropertyName("illustrator")]
        public string illustrator { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public string image { get; set; } = string.Empty;

        [JsonPropertyName("last_update")]
        public DateTime? last_update { get; set; } = DateTime.Now;

        [JsonPropertyName("orientation")]
        public object orientation { get; set; } = new object();

        [JsonPropertyName("rarity")]
        public int rarity { get; set; } = 0;

        [JsonPropertyName("slug")]
        public string slug { get; set; } = string.Empty;

        [JsonPropertyName("thema_charm_foil")]
        public int thema_charm_foil { get; set; } = 0;

        [JsonPropertyName("thema_charm_nonfoil")]
        public int thema_charm_nonfoil { get; set; } = 0;

        [JsonPropertyName("thema_ferocity_foil")]
        public int thema_ferocity_foil { get; set; } = 0;

        [JsonPropertyName("thema_ferocity_nonfoil")]
        public int thema_ferocity_nonfoil { get; set; } = 0;

        [JsonPropertyName("thema_foil")]
        public int thema_foil { get; set; } = 0;

        [JsonPropertyName("thema_grace_foil")]
        public int thema_grace_foil { get; set; } = 0;

        [JsonPropertyName("thema_grace_nonfoil")]
        public int thema_grace_nonfoil { get; set; } = 0;

        [JsonPropertyName("thema_mystique_foil")]
        public int thema_mystique_foil { get; set; } = 0;

        [JsonPropertyName("thema_mystique_nonfoil")]
        public int thema_mystique_nonfoil { get; set; } = 0;

        [JsonPropertyName("thema_nonfoil")]
        public int thema_nonfoil { get; set; } = 0;

        [JsonPropertyName("thema_valor_foil")]
        public int thema_valor_foil { get; set; } = 0;

        [JsonPropertyName("thema_valor_nonfoil")]
        public int thema_valor_nonfoil { get; set; } = 0;

        [JsonPropertyName("uuid")]
        public string uuid { get; set; } = string.Empty;

        [JsonPropertyName("collaborators")]
        public List<object> collaborators { get; set; } = new List<object>();

        [JsonPropertyName("circulationTemplates")]
        public List<CirculationTemplate> circulationTemplates { get; set; } = new List<CirculationTemplate>();

        [JsonPropertyName("circulations")]
        public List<object> circulations { get; set; } = new List<object>();

        [JsonPropertyName("other_orientations")]
        public List<object> other_orientations { get; set; } = new List<object>();

        [JsonPropertyName("set")]
        public CardSet set { get; set; } = new CardSet();

        [JsonPropertyName("effect_html")]
        public string effect_html { get; set; } = string.Empty;
    }

    public class CirculationTemplate
    {
        [JsonPropertyName("created_at")]
        public DateTime? created_at { get; set; } = DateTime.Now;

        [JsonPropertyName("edition_id")]
        public string edition_id { get; set; } = string.Empty;

        [JsonPropertyName("foil")]
        public bool foil { get; set; } = false;

        [JsonPropertyName("kind")]
        public string kind { get; set; } = string.Empty;

        [JsonPropertyName("last_update")]
        public DateTime? last_update { get; set; } = DateTime.Now;

        [JsonPropertyName("name")]
        public string name { get; set; } = string.Empty;

        [JsonPropertyName("population")]
        public int population { get; set; } = 0;

        [JsonPropertyName("population_operator")]
        public string population_operator { get; set; } = string.Empty;

        [JsonPropertyName("printing")]
        public bool printing { get; set; } = false;

        [JsonPropertyName("uuid")]
        public string uuid { get; set; } = string.Empty;

        [JsonPropertyName("variants")]
        public List<object> variants { get; set; } = new List<object>();
    }

    public class CardSet
    {
        [JsonPropertyName("created_at")]
        public DateTime? created_at { get; set; } = DateTime.Now;

        [JsonPropertyName("id")]
        public string id { get; set; } = string.Empty;

        [JsonPropertyName("language")]
        public string language { get; set; } = string.Empty;

        [JsonPropertyName("last_update")]
        public DateTime? last_update { get; set; } = DateTime.Now;

        [JsonPropertyName("name")]
        public string name { get; set; } = string.Empty;

        [JsonPropertyName("prefix")]
        public string prefix { get; set; } = string.Empty;

        [JsonPropertyName("release_date")]
        public DateTime release_date { get; set; } = DateTime.Now;
    }

}
