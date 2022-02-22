using System.Runtime.Serialization;

namespace PokemonBoardGame_CardGenerator.Enums
{
	public enum VersionGroupEnum
	{
		[EnumMember(Value = "firered-leafgreen")]
		FireRed_LeafGreen,
		[EnumMember(Value = "heartgold-soulsilver")]
		HeartGold_SoulSilver,
		[EnumMember(Value = "omega-ruby-alpha-sapphire")]
		OmegaRuby_AlphaSapphire
	}
}
