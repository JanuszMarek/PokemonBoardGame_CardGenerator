using Microsoft.Extensions.Options;
using PokemonBoardGame_CardGenerator.Enums;
using PokemonBoardGame_CardGenerator.Extensions;
using PokemonBoardGame_CardGenerator.Helpers;
using PokemonBoardGame_CardGenerator.Models;
using PokemonBoardGame_CardGenerator.Models.PokeApiModels;
using PokemonBoardGame_CardGenerator.Options;

namespace PokemonBoardGame_CardGenerator.Services
{
    public class PokemonCardService(
        PokemonDataService pokemonDataService, 
        PokemonEvolutionCardService pokemonEvolutionCardService,
        PokemonMoveCardService pokemonMoveCardService,
        IOptions<PokemonSettings> settings)
    {
        private readonly PokemonSettings pokemonSettings = settings.Value;

        private const int HpStatMultiplier = 5;
        private const double PowerMultiplier = 1.5;
        private const double StatsDivider = 10.0;

        private readonly string[] CanRidePokemons = [
               "Arcanine", "Charizard", "Dodrio", "Dragonite", "Gyarados", "Haunter", "Kangaskhan", "Lapras", "Machamp", "Onix", "Persian", "Rapidash", "Rhyhorn", "Rhydon", "Snorlax"
            ];

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

            SaveFileHelper.CreateFolderWhenNotExist(pokemonSettings.OutputPath + dir);
            await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "PokemonCardModels", pokemonCards);
            await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "Stats", pokemonCards.Select(x => (x.Name, x.Stats)));
            await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "Moves", pokemonCards.Select(x => (x.Name, x.Moves)));
            //await SaveFileHelper.SavePokemonDataJsonAsync(pokemonSettings.OutputPath + dir, "CatchRates", pokemonCards.Select(x => (x.Name, x.CaptureRate, (int)Math.Round((decimal)((x.CaptureRate * -0.0238) + 7.07)))));
        }

        #region Getters

        #endregion

        private async Task<PokemonCardModel> GetPokemonCardModelAsync(int pokeNo)
        {
            var pokemon = await pokemonDataService.GetPokemonAsync(pokeNo);
            var pokemonSpecies = await pokemonDataService.GetPokemonSpeciesAsync(pokeNo);
            var pokemonImageUrl = pokemonDataService.GetPokemonPokedexImageUrlAsync(pokeNo);
            var evolutionChain = await pokemonDataService.GetPokemonPokemonEvolutionChainAsync(pokemonSpecies.EvolutionChain.Url);

            var pokemonCardModel = new PokemonCardModel()
            {
                Id = pokeNo,
                Name = pokemon.Name.FirstCharToUpper(),
                ImageUrl = pokemonImageUrl,
                CaptureRate = (int)Math.Round((decimal)(pokemonSpecies.CaptureRate * -0.0238 + 7.07)),
                Stats = pokemon.Stats.Select(GetPokemonStat).ToList(),
                Types = pokemon.Types.OrderBy(x => x.Slot).Select(x => x.Type.Name).ToList(),
                Evolutions = await pokemonEvolutionCardService.GetPokemonCardEvolutions(pokemon, evolutionChain),
                Areas = MapAreas(pokemon.Name, pokemonSpecies.PalParkEncounters),
                CanFly = pokemon.Moves.Any(x => x.Move2.Name == "fly") && pokemon.Weight > 75,
                CanSwim = pokemon.Moves.Any(x => x.Move2.Name == "surf") && pokemon.Weight > 60,
                CanRide = CanRidePokemons.Contains(pokemon.Name.FirstCharToUpper()),
                Moves = (await pokemonMoveCardService.GetBestPokemonMoves(pokemon, pokemonSpecies))?.ToList(),
            };

            return pokemonCardModel;
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

        private PokemonCardStatModel GetPokemonStat(PokemonStat stat)
        {
            return new PokemonCardStatModel()
            {
                Name = stat.Stat.Name,
                Value = (int)((stat.Stat.Name == "hp" ? stat.BaseStat * HpStatMultiplier : stat.BaseStat) / StatsDivider),
            };
        }
    }
}
