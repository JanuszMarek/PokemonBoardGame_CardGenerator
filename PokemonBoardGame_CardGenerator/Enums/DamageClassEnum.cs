using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace PokemonBoardGame_CardGenerator.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum DamageClassEnum
	{
		[EnumMember(Value = "physical")]
		Physical,
		[EnumMember(Value = "special")]
		Special,
		[EnumMember(Value = "status")]
		Status
	}
}
