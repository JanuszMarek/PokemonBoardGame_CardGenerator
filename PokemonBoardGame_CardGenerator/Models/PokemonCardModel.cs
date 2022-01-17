using PokemonBoardGame_CardGenerator.Enums;
using PokemonBoardGame_CardGenerator.Models.PokeApiModels;

namespace PokemonBoardGame_CardGenerator.Models
{
	public class PokemonCardModel
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? ImageUrl { get; set; }
		public int? CaptureRate { get; set; }

		public List<PokemonTypeEnum>? Types { get; set; }
		public List<PokemonCardMoveModel>? Moves { get; set; }
		public EvolutionChain? EvolutionChain { get; set; }
		public List<PokemonCardStatModel>? Stats { get; set; }
		//public List<>
	}

	public class PokemonCardMoveModel
	{
		public string? Name { get; set; }
		public int? LevelLearnedAt { get; set; }
		public LearnMethodEnum? LearnMethod { get; set; }
		public int? Power { get; set; }
		public int? PP { get; set; }
		public int? Accuracy { get; set; }
		public PokemonTypeEnum Type { get; set; }
		public DamageClassEnum DamageClass { get; set; }
	}

	public class PokemonCardStatModel
	{
		public string? Name { get; set; }
		public int Value { get; set; }
	}

	public class AreaOccurrenceModel
	{
		public string? Name { get; set; }
		public int Score { get; set; }
		public int Rate { get; set; }
	}
}
