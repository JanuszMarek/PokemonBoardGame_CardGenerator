using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace PokemonBoardGame_CardGenerator.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum PokemonTypeEnum
	{
		Normal,
		Fighting,
		Flying,
		Poison,
		Ground,
		Rock,
		Bug,
		Ghost,
		Steel,
		Fire,
		Water,
		Grass,
		Electric,
		Psychic,
		Ice,
		Dragon,
		Dark,
		Fairy,
		Unknown,
		Shadow
	}
}
