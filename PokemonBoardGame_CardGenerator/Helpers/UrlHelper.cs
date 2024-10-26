using PokemonBoardGame_CardGenerator.Extensions;

namespace PokemonBoardGame_CardGenerator.Helpers
{
    public static class UrlHelper
    {
        public static int GetIdFromUrl(string url, string substringUrl)
        {
            return int.Parse(url.GetSubstringAfter(substringUrl).Replace("/", ""));
        }
    }
}
