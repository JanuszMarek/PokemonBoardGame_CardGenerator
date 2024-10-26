namespace PokemonBoardGame_CardGenerator.HttpClients.Implementations
{
    public class PokemonAssetsService : HttpBaseClient, IPokemonImageHttpService
	{
		private readonly string ImageExtension;

		public PokemonAssetsService(HttpClient httpClient) : base(httpClient)
		{
			httpClient.BaseAddress = new Uri("https://assets.pokemon.com");
			ImageExtension = ".png";
		}

        public string GetPokemonImagePath(int pokeNo, bool fullpath = false)
        {
            var threeDigitPokeNo = pokeNo.ToString("000");
            var baseUrl = fullpath ? GetBasePath()?.ToString() : string.Empty;
            return $"{baseUrl}/assets/cms2/img/pokedex/detail/{threeDigitPokeNo}{ImageExtension}";
        }


        public async Task<byte[]> GetPokemonImageAsync(int pokeNo)
		{
			var response = await GetAsync(GetPokemonImagePath(pokeNo));

			return await response.Content.ReadAsByteArrayAsync();
		}

        public string GetPokemonImageExtension() => ImageExtension;
    }
}
