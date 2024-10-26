using PokemonBoardGame_CardGenerator.Extensions;

namespace PokemonBoardGame_CardGenerator.Helpers
{
    public static class UrlHelper
    {
        public static int GetIdFromUrl(string url, string substringUrl) => int.Parse(url.GetSubstringAfter(substringUrl).Replace("/", ""));

        public static int? GetPokemonIdFromSpecies(PokemonLookup species) => GetIdFromUrl(species?.Url, "pokemon-species/");
    }
}
