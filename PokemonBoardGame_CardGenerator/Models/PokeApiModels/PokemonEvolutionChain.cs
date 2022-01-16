using Newtonsoft.Json;

namespace PokemonBoardGame_CardGenerator.Models.PokeApiModels
{
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class Trigger
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}

	public class EvolutionDetail
	{
		[JsonProperty("gender")]
		public object Gender { get; set; }

		[JsonProperty("held_item")]
		public object HeldItem { get; set; }

		[JsonProperty("item")]
		public object Item { get; set; }

		[JsonProperty("known_move")]
		public object KnownMove { get; set; }

		[JsonProperty("known_move_type")]
		public object KnownMoveType { get; set; }

		[JsonProperty("location")]
		public object Location { get; set; }

		[JsonProperty("min_affection")]
		public object MinAffection { get; set; }

		[JsonProperty("min_beauty")]
		public object MinBeauty { get; set; }

		[JsonProperty("min_happiness")]
		public object MinHappiness { get; set; }

		[JsonProperty("min_level")]
		public int MinLevel { get; set; }

		[JsonProperty("needs_overworld_rain")]
		public bool NeedsOverworldRain { get; set; }

		[JsonProperty("party_species")]
		public object PartySpecies { get; set; }

		[JsonProperty("party_type")]
		public object PartyType { get; set; }

		[JsonProperty("relative_physical_stats")]
		public object RelativePhysicalStats { get; set; }

		[JsonProperty("time_of_day")]
		public string TimeOfDay { get; set; }

		[JsonProperty("trade_species")]
		public object TradeSpecies { get; set; }

		[JsonProperty("trigger")]
		public Trigger Trigger { get; set; }

		[JsonProperty("turn_upside_down")]
		public bool TurnUpsideDown { get; set; }
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
		public Species Species { get; set; }
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
