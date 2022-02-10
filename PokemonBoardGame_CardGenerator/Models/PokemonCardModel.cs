using Newtonsoft.Json;
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

		public List<PalParkEncounter>? PalParkEncounters { get; set; }
		//public List<>
	}

	public class PokemonCardMoveModel
	{
		public string? Name { get; set; }
		[JsonIgnore]
		public int? LevelLearnedAt { get; set; }
		[JsonIgnore]
		public LearnMethodEnum? LearnMethod { get; set; }
		[JsonIgnore]
		public int? Power { get; set; }
		public int? PP { get; set; }
		public int? Damage { get; set; }

		public int? Accuracy { get; set; }
		public PokemonTypeEnum Type { get; set; }
		[JsonIgnore]
		public DamageClassEnum DamageClass { get; set; }
		public string? Description { get; set; }

		public PokemonCardMoveModel Clone()
		{
			return new PokemonCardMoveModel()
			{
				Name = Name,
				Accuracy = Accuracy,
				Damage = Damage,
				DamageClass = DamageClass,
				Description = Description,
				LearnMethod = LearnMethod,
				LevelLearnedAt = LevelLearnedAt,
				Power = Power,
				PP = PP,
				Type = Type
			};
		}

		public override bool Equals(object? obj)
		{
			return obj is PokemonCardMoveModel model &&
				   Name == model.Name &&
				   Power == model.Power &&
				   Type == model.Type &&
				   DamageClass == model.DamageClass;
		}

		public override int GetHashCode()
		{
			HashCode hash = new HashCode();
			hash.Add(Name);
			hash.Add(Power);
			hash.Add(Type);
			hash.Add(DamageClass);
			return hash.ToHashCode();
		}

		public void SetDamage(PokemonCardModel pokemon)
		{
			var isTypeTheSame = pokemon.Types.Contains(Type);
			Damage = (int?)(Power * pokemon.Stats.FirstOrDefault(x => x.Name == "attack")?.Value * (isTypeTheSame ? 1.2 : 1));
		}
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
