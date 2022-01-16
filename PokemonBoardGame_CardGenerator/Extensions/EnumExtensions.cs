using System.Runtime.Serialization;

namespace PokemonBoardGame_CardGenerator.Extensions
{
	public static class EnumExtensions
	{

		public static string? ToSerializationName<T>(this T enumVal) where T : Enum
		{
			var enumType = typeof(T);
			var memInfo = enumType.GetMember(enumVal.ToString());
			var attr = memInfo[0].GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
			if (attr != null)
			{
				return attr.Value;
			}

			return null;
		}
	}
}
