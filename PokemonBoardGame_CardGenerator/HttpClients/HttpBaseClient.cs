using PokemonBoardGame_CardGenerator.Builders;

namespace PokemonBoardGame_CardGenerator.HttpClients
{
	public class HttpBaseClient
	{
		private readonly HttpClient httpClient;

		public HttpBaseClient(HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		protected async Task<HttpResponseMessage> GetAsync(string endpoint, Dictionary<string, string> parameters = null)
		{
			var url = new UrlBuilder()
				.AddRoute(endpoint)
				.AddQuery(parameters)
				.Build();

			var response = await httpClient.GetAsync(url);

			if (!response.IsSuccessStatusCode)
			{
				throw CreateException(nameof(GetAsync), response);
			}

			return response;
		}

		protected async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> parameters = null)
		{
			var response = await GetAsync(endpoint, parameters);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsAsync<T>();
			}

			throw CreateException(nameof(GetAsync), response);
		}

		private static Exception CreateException(string methodName, HttpResponseMessage response)
		{
			return new Exception($"{methodName}: {response.StatusCode}, {response.ReasonPhrase}");
		}
	}
}
