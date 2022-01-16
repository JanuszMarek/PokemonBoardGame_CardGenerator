namespace PokemonBoardGame_CardGenerator.HttpClients.Implementations
{
	public class CdnTractionHttpService : HttpBaseClient, IPokemonImageHttpService
	{
		private readonly string ImageExtension;

		public CdnTractionHttpService(HttpClient httpClient) : base(httpClient)
		{
			httpClient.BaseAddress = new Uri("https://cdn.traction.one");
			ImageExtension = ".png";
		}

		public async Task<byte[]> GetPokemonImageAsync(string pokeNo)
		{
			var response = await GetAsync("/pokedex/pokemon/" + pokeNo + ImageExtension);

			return await response.Content.ReadAsByteArrayAsync();
		}

		public string GetPokemonImageExtension() => ImageExtension;
	}
}
