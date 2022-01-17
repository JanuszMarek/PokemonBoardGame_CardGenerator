using Microsoft.Extensions.Options;
using PokemonBoardGame_CardGenerator.Enums;
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

			var dir = "Output/";
			SaveFileHelper.CreateFolderWhenNotExist(pokemonSettings.OutputPath + dir);
			await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "PokemonCardModels", pokemonCards);
		}


		public async Task<PokemonCardModel> GetPokemonCardModelAsync(int pokeNo)
		{
			var pokemonImageUrl = await pokemonDataService.GetPokemonImageAsync(pokeNo);
			var pokemon = await pokemonDataService.GetPokemonDataAsync(pokeNo);
			var pokemonSpecies = await pokemonDataService.GetPokemonSpeciesDataAsync(pokeNo);

			List<PokemonMove> moves = await GetPokemonMoves(pokemon);
			Func<PokemonCardMoveModel, bool> moveLimitation = x => (x.LearnMethod == LearnMethodEnum.LevelUp || x.LearnMethod == LearnMethodEnum.Egg) &&
					(x.DamageClass == DamageClassEnum.Physical || x.DamageClass == DamageClassEnum.Special || x.DamageClass == DamageClassEnum.Status);

			var pokemonCardModel = new PokemonCardModel()
			{
				Id = pokeNo,
				Name = pokemon.Name.FirstCharToUpper(),
				ImageUrl = pokemonImageUrl,
				CaptureRate = pokemonSpecies.CaptureRate,
				Moves = moves.Select(x => MapApiMoveToLocalType(x, pokemon)).Where(moveLimitation)
					.OrderBy(x => x.DamageClass).ThenByDescending(x => x.Power).ToList(),
				Stats = pokemon.Stats.Select(x => new PokemonCardStatModel()
				{
					Name = x.Stat2.Name,
					Value = x.BaseStat
				}).ToList(),
				Types = pokemon.Types.OrderBy(x => x.Slot).Select(x => x.Type2.Name).ToList(),
			};

			MergeSpecialStatsWithNormal(pokemonCardModel);

			return pokemonCardModel;

			async Task<List<PokemonMove>> GetPokemonMoves(Pokemon pokemon)
			{
				var movesToGet = pokemon.Moves.Where(x => x.VersionGroupDetails.Any(v => v.VersionGroup.Name == VersionGroupEnum.FireRedLeafGreen.ToSerializationName() && (v.MoveLearnMethod.Name == LearnMethodEnum.LevelUp ||
					v.MoveLearnMethod.Name == LearnMethodEnum.Machine || v.MoveLearnMethod.Name == LearnMethodEnum.Egg)));
				var moveNames = movesToGet.Select(x => x.Move2.Name).Distinct();
				var moves = new List<PokemonMove>();

				foreach (var moveName in moveNames)
				{
					var move = await pokemonDataService.GetPokemonMoveAsync(moveName);
					moves.Add(move);
				}

				return moves;
			}

			static void MergeSpecialStatsWithNormal(PokemonCardModel pokemonCardModel)
			{
				var statsToRemove = new List<PokemonCardStatModel>();
				foreach (var stat in pokemonCardModel.Stats)
				{
					var specialString = "special-";
					var statIsSpecial = stat.Name.IndexOf(specialString);
					if (statIsSpecial != -1)
					{
						var normalStatName = stat.Name.Substring(specialString.Length, stat.Name.Length - specialString.Length);
						var normalStat = pokemonCardModel.Stats.FirstOrDefault(x => x.Name == normalStatName);

						normalStat.Value = (normalStat.Value + stat.Value) / 2;
						statsToRemove.Add(stat);
					}
				}
				foreach (var stat in statsToRemove)
				{
					pokemonCardModel.Stats.Remove(stat);
				}
			}

			static PokemonCardMoveModel MapApiMoveToLocalType(PokemonMove x, Pokemon pokemon)
			{
				var moveFromPokemon = pokemon.Moves.FirstOrDefault(p => p.Move2.Name == x.Name);
				var version = moveFromPokemon.VersionGroupDetails.FirstOrDefault(x => x.VersionGroup.Name == VersionGroupEnum.FireRedLeafGreen.ToSerializationName());

				return new PokemonCardMoveModel()
				{
					Name = x.Name,
					Accuracy = x.Accuracy,
					LearnMethod = version?.MoveLearnMethod.Name,
					LevelLearnedAt = version?.LevelLearnedAt,
					Power = x.Power,
					PP = x.Pp / 10 == 0 ? 1 : x.Pp / 10,
					Type = x.Type.Name,
					DamageClass = x.DamageClass.Name,
					Description = x.EffectEntries.FirstOrDefault()?.ShortEffect.Replace("$effect_chance", x.EffectChance)
				};
			}
		}
	}
}
