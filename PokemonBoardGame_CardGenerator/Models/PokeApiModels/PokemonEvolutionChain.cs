using Newtonsoft.Json;

namespace PokemonBoardGame_CardGenerator.Models.PokeApiModels
{
	public class Trigger
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}

    public class Location
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class EvolutionDetail
	{
		[JsonProperty("gender", NullValueHandling = NullValueHandling.Ignore)]
		public object Gender { get; set; }

		[JsonProperty("held_item", NullValueHandling = NullValueHandling.Ignore)]
		public EvolutionDetailsItem? HeldItem { get; set; }

		[JsonProperty("item", NullValueHandling = NullValueHandling.Ignore)]
		public EvolutionDetailsItem? Item { get; set; }

		[JsonProperty("known_move", NullValueHandling = NullValueHandling.Ignore)]
		public object KnownMove { get; set; }

		[JsonProperty("known_move_type", NullValueHandling = NullValueHandling.Ignore)]
		public object KnownMoveType { get; set; }

		[JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
		public Location Location { get; set; }

		[JsonProperty("min_affection", NullValueHandling = NullValueHandling.Ignore)]
		public int? MinAffection { get; set; }

		[JsonProperty("min_beauty", NullValueHandling = NullValueHandling.Ignore)]
		public int? MinBeauty { get; set; }

		[JsonProperty("min_happiness", NullValueHandling = NullValueHandling.Ignore)]
		public int? MinHappiness { get; set; }

		[JsonProperty("min_level", NullValueHandling = NullValueHandling.Ignore)]
		public int? MinLevel { get; set; }

		[JsonProperty("needs_overworld_rain", NullValueHandling = NullValueHandling.Ignore)]
		public bool NeedsOverworldRain { get; set; }

		[JsonProperty("party_species", NullValueHandling = NullValueHandling.Ignore)]
		public object PartySpecies { get; set; }

		[JsonProperty("party_type", NullValueHandling = NullValueHandling.Ignore)]
		public object PartyType { get; set; }

		[JsonProperty("relative_physical_stats", NullValueHandling = NullValueHandling.Ignore)]
		public object RelativePhysicalStats { get; set; }

		[JsonProperty("time_of_day", NullValueHandling = NullValueHandling.Ignore)]
		public string TimeOfDay { get; set; }

		[JsonProperty("trade_species", NullValueHandling = NullValueHandling.Ignore)]
		public object TradeSpecies { get; set; }

		[JsonProperty("trigger", NullValueHandling = NullValueHandling.Ignore)]
		public Trigger Trigger { get; set; }

		[JsonProperty("turn_upside_down", NullValueHandling = NullValueHandling.Ignore)]
		public bool TurnUpsideDown { get; set; }
	}

    public class EvolutionDetailsItem
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
    }

	//public class EvolvesTo
	//{
	//	[JsonProperty("evolution_details")]
	//	public List<EvolutionDetail> EvolutionDetails { get; set; }

	//	[JsonProperty("evolves_to")]
	//	public List<EvolvesTo> NextEvolvesTo { get; set; }

	//	[JsonProperty("is_baby")]
	//	public bool IsBaby { get; set; }

	//	[JsonProperty("species")]
	//	public Species Species { get; set; }
	//}

	public class EvolutionChain
	{
		[JsonProperty("evolution_details")]
		public List<EvolutionDetail> EvolutionDetails { get; set; }

		[JsonProperty("evolves_to")]
		public List<EvolutionChain> EvolvesTo { get; set; }

		[JsonProperty("is_baby")]
		public bool IsBaby { get; set; }

		[JsonProperty("species")]
		public global::PokemonLookup Species { get; set; }

		public string ImageUrl { get; set; }
	}

	public class PokemonEvolutionChain
	{
		[JsonProperty("baby_trigger_item")]
		public object BabyTriggerItem { get; set; }

		[JsonProperty("chain")]
		public EvolutionChain Chain { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }
	}


}
