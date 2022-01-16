using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace PokemonBoardGame_CardGenerator.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum LearnMethodEnum
	{
		[EnumMember(Value = "egg")]
		Egg,
		[EnumMember(Value = "level-up")]
		LevelUp,
		[EnumMember(Value = "machine")]
		Machine,
		[EnumMember(Value = "tutor")]
		Tutor,
		[EnumMember(Value = "stadium-surfing-pikachu")]
		SurfingPikachu
	}
}
