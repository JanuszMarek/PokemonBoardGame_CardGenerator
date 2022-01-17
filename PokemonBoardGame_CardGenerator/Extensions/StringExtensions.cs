namespace PokemonBoardGame_CardGenerator.Extensions
{
	public static class StringExtensions
	{
		public static string FirstCharToUpper(this string input) => input[0].ToString().ToUpper() + input.Substring(1);

		public static string GetSubstringAfter(this string input, string lookingText)
		{
			var index = input.IndexOf(lookingText);
			if (index != -1)
			{
				return input.Substring(index + lookingText.Length, input.Length - lookingText.Length - index);
			}

			return string.Empty;
		}
	}
}
