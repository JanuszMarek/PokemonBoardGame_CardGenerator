using Newtonsoft.Json;
using PokemonBoardGame_CardGenerator.Enums;

namespace PokemonBoardGame_CardGenerator.Models
{
    public class PokemonCardModel
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? ImageUrl { get; set; }
		public int? CaptureRate { get; set; }
        public string? Description { get; set; }
        public string? Generation { get; set; }
        public bool HasGenderDifferences { get; set; }
        public int GenderRate { get; set; }

        public bool? CanSwim { get; set; }
		public bool? CanRide { get; set; }
		public bool? CanFly { get; set; }

        public bool IsBaby { get; set; }
        public bool IsLegendary { get; set; }
        public bool IsMythical { get; set; }

        public List<PokemonTypeEnum>? Types { get; set; }
		public List<PokemonCardMoveModel>? Moves { get; set; }
		public List<PokemonCardEvolution>? Evolutions { get; set; }
		public List<PokemonCardStatModel>? Stats { get; set; }
		public List<AreaOccurrenceModel>? Areas { get; set; }
	}

    public class PokemonCardEvolution
    {
        public int PokemonId { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string Generation { get; set; }

        public List<PokemonCardEvolutionDetails> Details { get; set; }
    }

    public class PokemonCardEvolutionDetails
    {
        public string TriggerType { get; set; }
        public string Trigger { get; set; }
        public string TriggerImageUrl { get; set; }
        public string Location { get; set; }
    }


    public class PokemonCardMoveModel
	{
		public string? Name { get; set; }

        public int? PowerUp { get; set; }
		public int? MoveUsage { get; set; }

		public int? Accuracy { get; set; }
		public PokemonTypeEnum Type { get; set; }

		public string? Description { get; set; }
        public DamageClassEnum DamageClass { get; set; }

        public bool IsAttackingMove()
		{
			return DamageClass == DamageClassEnum.Special || DamageClass == DamageClassEnum.Physical;
		}

        [JsonIgnore]
        public int? LevelLearnedAt { get; set; }
        [JsonIgnore]
        public LearnMethodEnum? LearnMethod { get; set; }
        
        public PokemonCardMoveModel Clone()
		{
			return new PokemonCardMoveModel()
			{
				Name = Name,
				Accuracy = Accuracy,
				DamageClass = DamageClass,
				Description = Description,
				LearnMethod = LearnMethod,
				LevelLearnedAt = LevelLearnedAt,
				PowerUp = PowerUp,
				MoveUsage = MoveUsage,
				Type = Type
			};
		}

		public override bool Equals(object? obj)
		{
			return obj is PokemonCardMoveModel model &&
				   Name == model.Name &&
				   PowerUp == model.PowerUp &&
				   Type == model.Type &&
				   DamageClass == model.DamageClass;
		}

		public override int GetHashCode()
		{
			HashCode hash = new HashCode();
			hash.Add(Name);
			hash.Add(PowerUp);
			hash.Add(Type);
			hash.Add(DamageClass);
			return hash.ToHashCode();
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
		public int Rate { get; set; }
        public int Occurences { get; set; }
	}
}
