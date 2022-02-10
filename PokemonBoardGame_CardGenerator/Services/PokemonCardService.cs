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

		private const int HpStatMultiplier = 5;
		private const double PowerMultiplier = 1.5;
		private const double StatsDivider = 10.0;

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

			GetMovesFromPrevEvolutions(pokemonCards);
			//Safari - https://bulbapedia.bulbagarden.net/wiki/Kanto_Safari_Zone#Generation_I_2

			var dir = "Output/";
			SaveFileHelper.CreateFolderWhenNotExist(pokemonSettings.OutputPath + dir);
			await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "PokemonCardModels", pokemonCards);
			//await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "CatchRates", pokemonCards.Select(x => (x.Name, x.CaptureRate, (int)Math.Round((decimal)((x.CaptureRate * -0.0238) + 7.07)))));

			// x -> y
			// A 255 -> 1
			// B 45 -> 6

			// a = 5/-210 = -0.238
			// y = -0.238 x +  60,69 +1
			// y = -0.238x + 10,71+6
		}

		private void GetMovesFromPrevEvolutions(List<PokemonCardModel> pokemonCards)
		{
			foreach (var pokemonCard in pokemonCards)
			{
				GetPrevEvoMoves(pokemonCard, new List<EvolutionChain> { pokemonCard.EvolutionChain }, pokemonCards);
			}

			static void GetPrevEvoMoves(PokemonCardModel pokemon, List<EvolutionChain> evolutionChains, List<PokemonCardModel> pokemonCards)
			{
				foreach (var evolutionChain in evolutionChains)
				{
					if (evolutionChain.Species.Name.ToLower() != pokemon.Name.ToLower())
					{
						var prevEvo = pokemonCards.FirstOrDefault(x => x.Name.ToLower() == evolutionChain.Species.Name.ToLower());
						if (prevEvo != null)
						{
							foreach (var move in prevEvo?.Moves)
							{
								var newMove = move.Clone();
								newMove.SetDamage(pokemon);

								pokemon?.Moves?.Add(newMove);
							}

							GetPrevEvoMoves(pokemon, evolutionChain.EvolvesTo, pokemonCards);
						}

						pokemon.Moves = pokemon.Moves.Distinct().ToList();
					}
				}

			}
		}

		public async Task<PokemonCardModel> GetPokemonCardModelAsync(int pokeNo)
		{
			var pokemonImageUrl = await pokemonDataService.GetPokemonImageAsync(pokeNo);
			var pokemon = await pokemonDataService.GetPokemonDataAsync(pokeNo);
			var pokemonSpecies = await pokemonDataService.GetPokemonSpeciesDataAsync(pokeNo);
			var evolutionChain = await pokemonDataService.GetPokemonEvolutionChainDataAsync(pokemonSpecies.EvolutionChain.Url);

			List<PokemonMove> moves = await GetPokemonMoves(pokemon);
			Func<PokemonCardMoveModel, bool> moveLimitation = x => (x.LearnMethod == LearnMethodEnum.LevelUp || x.LearnMethod == LearnMethodEnum.Egg) &&
					(x.DamageClass == DamageClassEnum.Physical || x.DamageClass == DamageClassEnum.Special || x.DamageClass == DamageClassEnum.Status);

			var pokemonCardModel = new PokemonCardModel()
			{
				Id = pokeNo,
				Name = pokemon.Name.FirstCharToUpper(),
				ImageUrl = pokemonImageUrl,
				CaptureRate = (int)Math.Round((decimal)((pokemonSpecies.CaptureRate * -0.0238) + 7.07)),
				Stats = pokemon.Stats.Select(x => new PokemonCardStatModel()
				{
					Name = x.Stat2.Name,
					Value = x.Stat2.Name == "hp" ? x.BaseStat * HpStatMultiplier : x.BaseStat,
				}).ToList(),
				Types = pokemon.Types.OrderBy(x => x.Slot).Select(x => x.Type2.Name).ToList(),
				EvolutionChain = evolutionChain.Chain,
				PalParkEncounters = pokemonSpecies.PalParkEncounters
			};

			MergeSpecialStatsWithNormal(pokemonCardModel);
			DivideStatsToBetterExperience(pokemonCardModel);

			pokemonCardModel.Moves = moves.Select(x => MapApiMoveToLocalType(x, pokemon, pokemonCardModel)).Where(moveLimitation)
				.OrderBy(x => x.DamageClass).ThenByDescending(x => x.Power).ToList();

			return pokemonCardModel;

			async Task<List<PokemonMove>> GetPokemonMoves(Pokemon pokemon)
			{
				var movesToGet = pokemon.Moves.Where(x => x.VersionGroupDetails.Any(v => v.VersionGroup.Name == VersionGroupEnum.FireRedLeafGreen.ToSerializationName() &&
					(v.MoveLearnMethod.Name == LearnMethodEnum.LevelUp || v.MoveLearnMethod.Name == LearnMethodEnum.Machine || v.MoveLearnMethod.Name == LearnMethodEnum.Egg)));

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
					var normalStatName = stat.Name.GetSubstringAfter("special-");
					if (!string.IsNullOrEmpty(normalStatName))
					{
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

			static PokemonCardMoveModel MapApiMoveToLocalType(PokemonMove x, Pokemon pokemon, PokemonCardModel cardModel)
			{
				var moveFromPokemon = pokemon.Moves.FirstOrDefault(p => p.Move2.Name == x.Name);
				var version = moveFromPokemon.VersionGroupDetails.FirstOrDefault(x => x.VersionGroup.Name == VersionGroupEnum.FireRedLeafGreen.ToSerializationName());

				var move =  new PokemonCardMoveModel()
				{
					Name = x.Name,
					Accuracy = (int?)Math.Round(x.Accuracy.GetValueOrDefault() / 10.0),
					LearnMethod = version?.MoveLearnMethod.Name,
					LevelLearnedAt = version?.LevelLearnedAt,
					Power = (int?)Math.Round((x.Power.GetValueOrDefault() / StatsDivider) * PowerMultiplier),
					PP = x.Pp / 10 == 0 ? 1 : x.Pp / 10,
					Type = x.Type.Name,
					DamageClass = x.DamageClass.Name,
					Description = x.EffectEntries.FirstOrDefault()?.ShortEffect.Replace("$effect_chance", x.EffectChance)
				};

				move.SetDamage(cardModel);
				return move;
			}
		}

		private void DivideStatsToBetterExperience(PokemonCardModel pokemonCardModel)
		{
			foreach (var stat in pokemonCardModel.Stats)
			{
				stat.Value = (int)Math.Round(stat.Value / StatsDivider);
			}
		}
	}
}
