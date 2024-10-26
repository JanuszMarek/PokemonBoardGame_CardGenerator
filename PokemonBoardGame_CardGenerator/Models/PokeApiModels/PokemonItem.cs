using Newtonsoft.Json;

namespace PokemonBoardGame_CardGenerator.Models.PokeApiModels
{
    public class PokemonItem
    {
        [JsonProperty("sprites", NullValueHandling = NullValueHandling.Ignore)]
        public PokemonSprite Sprites { get; set; }
    }

    public class PokemonSprite
    {
        [JsonProperty("default", NullValueHandling = NullValueHandling.Ignore)]
        public string Default { get; set; }
    }
}
