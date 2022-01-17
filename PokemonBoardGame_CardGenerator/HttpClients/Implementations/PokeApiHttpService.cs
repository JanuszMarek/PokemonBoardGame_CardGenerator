using PokemonBoardGame_CardGenerator.Models.PokeApiModels;

namespace PokemonBoardGame_CardGenerator.HttpClients.Implementations
{
	public class PokeApiHttpService : HttpBaseClient
	{
		private readonly string ApiVersion;

		public PokeApiHttpService(HttpClient httpClient) : base(httpClient)
		{
			httpClient.BaseAddress = new Uri("https://pokeapi.co/");
			ApiVersion = "api/v2/";
		}

		public async Task<Pokemon> GetPokemonAsync(int pokeNo)
		{
			return await GetAsync<Pokemon>(ApiVersion + "pokemon/" + pokeNo.ToString());
		}

		public async Task<PokemonSpecies> GetPokemonSpeciesAsync(int pokeNo)
		{
			return await GetAsync<PokemonSpecies>(ApiVersion + "pokemon-species/" + pokeNo.ToString());
		}

		public async Task<PokemonMove> GetPokemonMoveAsync(string move)
		{
			return await GetAsync<PokemonMove>(ApiVersion + "move/" + move);
		}

		public async Task<PokemonEvolutionChain> GetPokemonEvolutionChainAsync(int chainNo)
		{
			return await GetAsync<PokemonEvolutionChain>(ApiVersion + "evolution-chain/" + chainNo);
		}
	}
}
