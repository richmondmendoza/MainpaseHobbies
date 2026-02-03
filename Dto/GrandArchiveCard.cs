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
        public List<string> classes { get; set; }

        [JsonPropertyName("cost_memory")]
        public int cost_memory { get; set; }

        [JsonPropertyName("cost_reserve")]
        public int? cost_reserve { get; set; }

        [JsonPropertyName("cost")]
        public Cost cost { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime created_at { get; set; }

        [JsonPropertyName("durability")]
        public int? durability { get; set; }

        [JsonPropertyName("editions")]
        public List<Edition> editions { get; set; }

        [JsonPropertyName("effect")]
        public string effect { get; set; }

        [JsonPropertyName("effect_raw")]
        public string effect_raw { get; set; }

        [JsonPropertyName("element")]
        public string element { get; set; }

        [JsonPropertyName("elements")]
        public List<string> elements { get; set; }

        [JsonPropertyName("flavor")]
        public string flavor { get; set; }

        [JsonPropertyName("last_update")]
        public DateTime last_update { get; set; }

        [JsonPropertyName("legality")]
        public object legality { get; set; }

        [JsonPropertyName("level")]
        public int level { get; set; }

        [JsonPropertyName("life")]
        public int life { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("power")]
        public int? power { get; set; }

        [JsonPropertyName("referenced_by")]
        public List<object> referenced_by { get; set; }

        [JsonPropertyName("references")]
        public List<object> references { get; set; }

        [JsonPropertyName("result_editions")]
        public List<Edition> result_editions { get; set; }

        [JsonPropertyName("rule")]
        public List<object> rule { get; set; }

        [JsonPropertyName("slug")]
        public string slug { get; set; }

        [JsonPropertyName("speed")]
        public object speed { get; set; }

        [JsonPropertyName("subtypes")]
        public List<string> subtypes { get; set; }

        [JsonPropertyName("types")]
        public List<string> types { get; set; }

        [JsonPropertyName("uuid")]
        public string uuid { get; set; }

        [JsonPropertyName("effect_html")]
        public string effect_html { get; set; }
    }

    public class Cost
    {
        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("value")]
        public string value { get; set; }
    }

    public class Edition
    {
        [JsonPropertyName("card_id")]
        public string card_id { get; set; }

        [JsonPropertyName("collector_number")]
        public string collector_number { get; set; }

        [JsonPropertyName("configuration")]
        public string configuration { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime created_at { get; set; }

        [JsonPropertyName("effect")]
        public string effect { get; set; }

        [JsonPropertyName("effect_raw")]
        public string effect_raw { get; set; }

        [JsonPropertyName("flavor")]
        public string flavor { get; set; }

        [JsonPropertyName("illustrator")]
        public string illustrator { get; set; }

        [JsonPropertyName("image")]
        public string image { get; set; }

        [JsonPropertyName("last_update")]
        public DateTime last_update { get; set; }

        [JsonPropertyName("orientation")]
        public object orientation { get; set; }

        [JsonPropertyName("rarity")]
        public int rarity { get; set; }

        [JsonPropertyName("slug")]
        public string slug { get; set; }

        [JsonPropertyName("thema_charm_foil")]
        public int thema_charm_foil { get; set; }

        [JsonPropertyName("thema_charm_nonfoil")]
        public int thema_charm_nonfoil { get; set; }

        [JsonPropertyName("thema_ferocity_foil")]
        public int thema_ferocity_foil { get; set; }

        [JsonPropertyName("thema_ferocity_nonfoil")]
        public int thema_ferocity_nonfoil { get; set; }

        [JsonPropertyName("thema_foil")]
        public int thema_foil { get; set; }

        [JsonPropertyName("thema_grace_foil")]
        public int thema_grace_foil { get; set; }

        [JsonPropertyName("thema_grace_nonfoil")]
        public int thema_grace_nonfoil { get; set; }

        [JsonPropertyName("thema_mystique_foil")]
        public int thema_mystique_foil { get; set; }

        [JsonPropertyName("thema_mystique_nonfoil")]
        public int thema_mystique_nonfoil { get; set; }

        [JsonPropertyName("thema_nonfoil")]
        public int thema_nonfoil { get; set; }

        [JsonPropertyName("thema_valor_foil")]
        public int thema_valor_foil { get; set; }

        [JsonPropertyName("thema_valor_nonfoil")]
        public int thema_valor_nonfoil { get; set; }

        [JsonPropertyName("uuid")]
        public string uuid { get; set; }

        [JsonPropertyName("collaborators")]
        public List<object> collaborators { get; set; }

        [JsonPropertyName("circulationTemplates")]
        public List<CirculationTemplate> circulationTemplates { get; set; }

        [JsonPropertyName("circulations")]
        public List<object> circulations { get; set; }

        [JsonPropertyName("other_orientations")]
        public List<object> other_orientations { get; set; }

        [JsonPropertyName("set")]
        public CardSet set { get; set; }

        [JsonPropertyName("effect_html")]
        public string effect_html { get; set; }
    }

    public class CirculationTemplate
    {
        [JsonPropertyName("created_at")]
        public DateTime created_at { get; set; }

        [JsonPropertyName("edition_id")]
        public string edition_id { get; set; }

        [JsonPropertyName("foil")]
        public bool foil { get; set; }

        [JsonPropertyName("kind")]
        public string kind { get; set; }

        [JsonPropertyName("last_update")]
        public DateTime last_update { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("population")]
        public int population { get; set; }

        [JsonPropertyName("population_operator")]
        public string population_operator { get; set; }

        [JsonPropertyName("printing")]
        public bool printing { get; set; }

        [JsonPropertyName("uuid")]
        public string uuid { get; set; }

        [JsonPropertyName("variants")]
        public List<object> variants { get; set; }
    }

    public class CardSet
    {
        [JsonPropertyName("created_at")]
        public DateTime created_at { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("language")]
        public string language { get; set; }

        [JsonPropertyName("last_update")]
        public DateTime last_update { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("prefix")]
        public string prefix { get; set; }

        [JsonPropertyName("release_date")]
        public DateTime release_date { get; set; }
    }

}
