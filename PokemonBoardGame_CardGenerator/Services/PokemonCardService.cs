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
			var dir = "Output/";

			var pokemonCards = new List<PokemonCardModel>();
			for (var i = pokemonSettings.FirstPokeNo.Value; i <= pokemonSettings.LastPokeNo.Value; i++)
			{
				Console.WriteLine($"{nameof(GeneratePokemonCardsAsync)} for pokeNo {i}");
				var pokemonCard = await GetPokemonCardModelAsync(i);
				pokemonCards.Add(pokemonCard);
			}

			GetMovesFromPrevEvolutions(pokemonCards);
			await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "MovesBeforeLimit", pokemonCards.Select(x => (x.Name, x.Moves)));

			LimitMovesToEvoLvl(pokemonCards);
			Select4Moves(pokemonCards);

			SaveFileHelper.CreateFolderWhenNotExist(pokemonSettings.OutputPath + dir);
			await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "PokemonCardModels", pokemonCards);
			await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "Stats", pokemonCards.Select(x => (x.Name, x.Stats)));
			await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "Moves", pokemonCards.Select(x => (x.Name, x.Moves)));
			//await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "CatchRates", pokemonCards.Select(x => (x.Name, x.CaptureRate, (int)Math.Round((decimal)((x.CaptureRate * -0.0238) + 7.07)))));

			// x -> y
			// A 255 -> 1
			// B 45 -> 6

			// a = 5/-210 = -0.238
			// y = -0.238 x +  60,69 +1
			// y = -0.238x + 10,71+6
		}

		private void Select4Moves(List<PokemonCardModel> pokemonCards)
		{
			foreach (var pokemonCard in pokemonCards)
			{
				if (pokemonCard.Moves.Count > 4)
				{
					var moves = new List<PokemonCardMoveModel>();
					foreach (var type in pokemonCard.Types)
					{
						var typeAttack = pokemonCard.Moves.FirstOrDefault(x => x.Type == type);
						if (typeAttack != null)
						{
							moves.Add(typeAttack);
						}
						else
						{

						}
					}

					while (moves.Count < 4)
					{
						moves.Add(SelectOptimumMoves(pokemonCard, moves));

						if (moves.Any(x => x == null))
						{
							throw new Exception();
						}
					}

					pokemonCard.Moves = moves;
				}
			}
		}

		private PokemonCardMoveModel? SelectOptimumMoves(PokemonCardModel pokemonCard, List<PokemonCardMoveModel> moves)
		{
			PokemonCardMoveModel toAdd = null;
			if (!moves.Any(x => x.DamageClass == DamageClassEnum.Status))
			{
				toAdd = pokemonCard.Moves.FirstOrDefault(x => x.DamageClass == DamageClassEnum.Status && !moves.Contains(x));
				if (toAdd != null)
					return toAdd;
			}

			if (!moves.Any(x => x.IsAttackingMove() && x.PP >= 2))
			{
				toAdd = pokemonCard.Moves.FirstOrDefault(x => x.IsAttackingMove() && x.Type == pokemonCard.Types[0] && x.PP >= 2 && x.Damage > 0 && !moves.Contains(x));
				if (toAdd != null)
					return toAdd;
			}

			if (!moves.Any(x => x.IsAttackingMove() && x.PP >= 2))
			{
				toAdd = pokemonCard.Moves.FirstOrDefault(x => x.IsAttackingMove() && x.PP >= 2 && x.Damage > 0 && !moves.Contains(x));
				if (toAdd != null)
					return toAdd;
			}

			toAdd = pokemonCard.Moves.FirstOrDefault(x => x.IsAttackingMove() && x.Type == pokemonCard.Types[0] && x.Damage > 0 && !moves.Contains(x)); 
			if (toAdd != null)
				return toAdd;

			toAdd = pokemonCard.Moves.FirstOrDefault(x => x.IsAttackingMove() && x.Type == PokemonTypeEnum.Normal && x.Damage > 0 && !moves.Contains(x));
			if (toAdd != null)
				return toAdd;

			toAdd = pokemonCard.Moves.FirstOrDefault(x => x.IsAttackingMove() && !moves.Contains(x));
			if (toAdd != null)
				return toAdd;

			toAdd = pokemonCard.Moves.FirstOrDefault(x => !moves.Contains(x));

			return toAdd;
		}

		private List<AreaOccurrenceModel> MapAreas(string pokemonName, List<PalParkEncounter> encounters)
		{
			var pokemonsApperInSafari = new List<string>()
			{
				"poliwag", "magicarp", "goldeen", "paras", "venonat", "psyduck", "slowpoke", "seaking"
			};

			var pokemonsApperOnlyInSafari = new List<string>()
			{
				"nidoran-f", "nidoran-m", "doduo", "nidorino", "nidorina", "exeggcute", "parasect", "venomoth", "kangaskhan", "scyther", "pinsir", "tauros", "dragonair", "dratini", "chansey"
			};

			var starters = new List<string>()
			{
				"bulbasaur", "ivysaur", "venusaur", "charmander", "charmeleon", "charizard", "squirtle", "wartortle", "blastoise", "pikachu", "raichu"
			};

			var appearInSafari = pokemonsApperInSafari.Contains(pokemonName.ToLower());
			var appearOnlyInSafari = pokemonsApperOnlyInSafari.Contains(pokemonName.ToLower());
			var isStarter = starters.Contains(pokemonName.ToLower());
			var areas = encounters.Select(x => new AreaOccurrenceModel()
			{
				Name = x.Area.Name,
				Rate = x.Rate
			}).ToList();

			if (appearInSafari || appearOnlyInSafari)
			{
				//Safari - https://bulbapedia.bulbagarden.net/wiki/Kanto_Safari_Zone#Generation_I_2
				var defaultArea = areas.FirstOrDefault();
				var safariArea = new AreaOccurrenceModel()
				{
					Name = "safari",
					Rate = defaultArea.Rate
				};

				if (appearOnlyInSafari)
				{
					return new List<AreaOccurrenceModel>() { safariArea };
				}

				areas.Add(safariArea);
			}
			else if (isStarter)
			{
				var starter = new AreaOccurrenceModel()
				{
					Name = "starter",
					Rate = 0,
				};

				return new List<AreaOccurrenceModel>() { starter };
			}

			return areas;
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

						pokemon.Moves = pokemon.Moves.Distinct().OrderBy(x => x.DamageClass).ThenByDescending(x => x.Damage).ToList();
					}
				}

			}
		}

		private void LimitMovesToEvoLvl(List<PokemonCardModel> pokemonCards)
		{
			foreach (var pokemonCard in pokemonCards)
			{
				var lvl = GetEvolutionLevel(new List<EvolutionChain> { pokemonCard.EvolutionChain }, pokemonCard.Name);
				var limitedMoves = pokemonCard.Moves.Where(x => x.LevelLearnedAt == 0 || !lvl.HasValue || x.LevelLearnedAt <= lvl).ToList();
				if (limitedMoves.Count > 4)
				{
					pokemonCard.Moves = limitedMoves;
				}
			}
		}

		private int? GetEvolutionLevel(List<EvolutionChain> evolutions, string pokeName)
		{
			foreach (var evolutionChain in evolutions)
			{
				if (evolutionChain.Species.Name.ToLower() == pokeName.ToLower())
				{
					return evolutionChain.EvolvesTo.Where(x => x.EvolutionDetails.Any(x => x.Trigger.Name == "level-up")).FirstOrDefault()?
						.EvolutionDetails.Where(x => x.Trigger.Name == "level-up").FirstOrDefault()?.MinLevel;
				}
				return GetEvolutionLevel(evolutionChain.EvolvesTo, pokeName);
			}

			return null;
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

			List<string> ridePokemons = new List<string>()
			{
				"Aerodactly", "Arcanine", "Charizard", "Dodrio", "Dragonite", "Gyarados", "Haunter", "Kangaskhan", "Lapras", "Machamp", "Onix", "Persian", "Rapidash", "Rhyhorn", "Rhydon", "Snorlax"
			};

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
				Areas = MapAreas(pokemon.Name, pokemonSpecies.PalParkEncounters),
				CanFly = pokemon.Moves.Any(x => x.Move2.Name == "fly") && pokemon.Weight > 75,
				CanSwim = pokemon.Moves.Any(x => x.Move2.Name == "surf") && pokemon.Weight > 60,
				CanRide = ridePokemons.Contains(pokemon.Name.FirstCharToUpper()),
			};

			MergeSpecialStatsWithNormal(pokemonCardModel);
			DivideStatsToBetterExperience(pokemonCardModel);

			var mappedMoves = moves.Select(x => MapApiMoveToLocalType(x, pokemon, pokemonCardModel));
			var limitedMoves = mappedMoves.Where(moveLimitation);

			if (limitedMoves.Count() < 4)
			{
				limitedMoves = mappedMoves;
			}

			pokemonCardModel.Moves = limitedMoves.OrderBy(x => x.DamageClass).ThenByDescending(x => x.Damage).ToList();

			return pokemonCardModel;

			async Task<List<PokemonMove>> GetPokemonMoves(Pokemon pokemon)
			{
				var movesToGet = pokemon.Moves.Where(x => x.VersionGroupDetails.Any(v => 
					v.VersionGroup.Name == VersionGroupEnum.FireRed_LeafGreen.ToSerializationName() ||
					v.VersionGroup.Name == VersionGroupEnum.HeartGold_SoulSilver.ToSerializationName() ||
					v.VersionGroup.Name == VersionGroupEnum.OmegaRuby_AlphaSapphire.ToSerializationName()
				));

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
				var version = moveFromPokemon.VersionGroupDetails.FirstOrDefault(x => x.VersionGroup.Name == VersionGroupEnum.FireRed_LeafGreen.ToSerializationName());
				if (version == null)
				{
					version = moveFromPokemon.VersionGroupDetails.FirstOrDefault(x => x.VersionGroup.Name == VersionGroupEnum.HeartGold_SoulSilver.ToSerializationName());
				}
				if (version == null)
				{
					version = moveFromPokemon.VersionGroupDetails.FirstOrDefault(x => x.VersionGroup.Name == VersionGroupEnum.OmegaRuby_AlphaSapphire.ToSerializationName());
				}

				var move = new PokemonCardMoveModel()
				{
					Name = x.Name,
					Accuracy = (int?)Math.Round(x.Accuracy.GetValueOrDefault() / 10.0),
					LearnMethod = version?.MoveLearnMethod.Name,
					LevelLearnedAt = version?.LevelLearnedAt,
					Power = (int?)Math.Round((x.Power.GetValueOrDefault() / StatsDivider) * PowerMultiplier),
					PP = x.Pp / 10 == 0 ? 1 : x.Pp / 10,
					Type = x.Type.Name,
					DamageClass = x.DamageClass.Name,
					Description = x.EffectEntries.FirstOrDefault()?.ShortEffect
						.Replace("$effect_chance", x.EffectChance)
						.Replace("Special", "")
						.Replace("several", "2-5")
						.Replace(" for 1-8 turns", "")
						.Replace("  ", " ")
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
