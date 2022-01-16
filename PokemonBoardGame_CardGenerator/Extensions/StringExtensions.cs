namespace PokemonBoardGame_CardGenerator.Extensions
{
	public static class StringExtensions
	{
		public static string FirstCharToUpper(this string input) => input[0].ToString().ToUpper() + input.Substring(1);
	}
}
