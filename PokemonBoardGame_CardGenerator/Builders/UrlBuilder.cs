using System.Text;

namespace PokemonBoardGame_CardGenerator.Builders
{
	public class UrlBuilder
	{
		private readonly StringBuilder Url = new();

		public string Build()
		{
			return Url.ToString();
		}

		public UrlBuilder AddRoute(string route)
		{
			if (string.IsNullOrEmpty(route))
			{
				return this;
			}
			else
			if (route[0] != '/' && (Url.Length == 0 || Url[^1] != '/'))
			{
				Url.Append('/');
			}

			Url.Append(route);
			return this;
		}

		public UrlBuilder AddQuery(IDictionary<string, string> parametersDict)
		{
			if (parametersDict == null)
			{
				return this;
			}

			var queryBuilder = new StringBuilder("?");
			var keysArray = parametersDict.Keys.ToArray();

			for (int i = 0; i < keysArray.Length; i++)
			{
				var key = keysArray[i];
				queryBuilder.Append(key + "=" + parametersDict[key]);
				if (i < (keysArray.Length - 1))
				{
					queryBuilder.Append('&');
				}
			}

			Url.Append(queryBuilder);
			return this;
		}
	}
}
