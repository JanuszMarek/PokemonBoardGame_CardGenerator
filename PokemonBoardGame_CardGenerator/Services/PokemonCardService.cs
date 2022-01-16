using Microsoft.Extensions.Options;
using PokemonBoardGame_CardGenerator.Extensions;
using PokemonBoardGame_CardGenerator.Models;
using PokemonBoardGame_CardGenerator.Models.PokeApiModels;
using PokemonBoardGame_CardGenerator.Options;

namespace PokemonBoardGame_CardGenerator.Services
{
	public class PokemonCardService
	{
		private readonly PokemonDataService pokemonDataService;
		private readonly PokemonSettings pokemonSettings;


		public PokemonCardService(PokemonDataService pokemonDataService, IOptions<PokemonSettings> settings)
		{
			this.pokemonDataService = pokemonDataService;
			this.pokemonSettings = settings.Value;
		}

		public async Task GeneratePokemonCardsAsync()
		{
			var pokemonCards = new List<PokemonCardModel>();
			for (var i = pokemonSettings.FirstPokeNo.Value; i <= pokemonSettings.LastPokeNo.Value; i++)
			{
				Console.WriteLine($"{nameof(GeneratePokemonCardsAsync)} for pokeNo {i}");
				var pokemonCard = await GetPokemonCardModelAsync(i);
				pokemonCards.Add(pokemonCard);
			}
		}


		public async Task<PokemonCardModel> GetPokemonCardModelAsync(int pokeNo)
		{
			var pokemonImageUrl = await pokemonDataService.GetPokemonImageAsync(pokeNo);
			var pokemon = await pokemonDataService.GetPokemonDataAsync(pokeNo);
			var pokemonSpecies = await pokemonDataService.GetPokemonSpeciesDataAsync(pokeNo);

			List<PokemonMove> moves = await GetPokemonMoves(pokemon);

			var pokemonCardModel = new PokemonCardModel()
			{
				Id = pokeNo,
				Name = pokemon.Name.FirstCharToUpper(),
				ImageUrl = pokemonImageUrl,
				CaptureRate = pokemonSpecies.CaptureRate,
				Moves = moves.Select(x =>
				{
					var moveFromPokemon = pokemon.Moves.FirstOrDefault(p => p.Move2.Name == x.Name);
					var version = moveFromPokemon.VersionGroupDetails.FirstOrDefault(x => x.VersionGroup.Name == "firered-leafgreen");

					return new PokemonCardMoveModel()
					{
						Name = x.Name,
						Accuracy = x.Accuracy,
						LearnMethod = version?.MoveLearnMethod.Name,
						LevelLearnedAt = version?.LevelLearnedAt,
						Power = x.Power,
						PP = x.Pp,
						Type = x.Type.Name,
						DamageClass = x.DamageClass.Name
					};
				}).Where(x => (x.LearnMethod == "level-up" || x.LearnMethod == "egg") && (x.DamageClass == "physical" || x.DamageClass == "special" || x.DamageClass == "status"))
				.OrderBy(x => x.LevelLearnedAt).ToList(),
			};

			//Learn Method and DamageClass as enums to sort by level up and physical in first way and get 4

			return pokemonCardModel;

			async Task<List<PokemonMove>> GetPokemonMoves(Pokemon pokemon)
			{
				var movesToGet = pokemon.Moves.Where(x => x.VersionGroupDetails.Any(v => v.VersionGroup.Name == "firered-leafgreen" && (v.MoveLearnMethod.Name == "level-up" ||
					v.MoveLearnMethod.Name == "machine" || v.MoveLearnMethod.Name == "egg")));
				var moveNames = movesToGet.Select(x => x.Move2.Name).Distinct();
				var moves = new List<PokemonMove>();

				foreach (var moveName in moveNames)
				{
					var move = await pokemonDataService.GetPokemonMoveAsync(moveName);
					moves.Add(move);
				}

				return moves;
			}
		}
	}
}
