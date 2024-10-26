using PokemonBoardGame_CardGenerator.Helpers;
using PokemonBoardGame_CardGenerator.Models;
using PokemonBoardGame_CardGenerator.Models.PokeApiModels;

namespace PokemonBoardGame_CardGenerator.Services
{
    public class PokemonEvolutionCardService(PokemonDataService pokemonDataService)
    {
        public async Task<List<PokemonCardEvolution>?> GetPokemonCardEvolutions(Pokemon pokemon, PokemonEvolutionChain pokemonEvolutionChain)
        {
            var nextEvolutionChain = GetNextEvolutionChain(pokemon, pokemonEvolutionChain.Chain);
            if (nextEvolutionChain == null || nextEvolutionChain.EvolvesTo == null) return null;

            List<PokemonCardEvolution> pokemonCardEvolutions = [];
            foreach (var nextEvolution in nextEvolutionChain.EvolvesTo)
            {
                var evolutionCardModel = new PokemonCardEvolution();
                evolutionCardModel.PokemonId = UrlHelper.GetPokemonIdFromSpecies(nextEvolution.Species).GetValueOrDefault();

                evolutionCardModel.Name = nextEvolution.Species?.Name;
                var nextEvolutionSpecies = await pokemonDataService.GetPokemonSpeciesAsync(evolutionCardModel.PokemonId);
                evolutionCardModel.Generation = nextEvolutionSpecies?.Generation?.Name;
                evolutionCardModel.ImageUrl = pokemonDataService.GetPokemonImageUrl(evolutionCardModel.PokemonId);

                await SetEvolutionDetails(evolutionCardModel, nextEvolution.EvolutionDetails);


                pokemonCardEvolutions.Add(evolutionCardModel);
            }

            return pokemonCardEvolutions;

            async Task SetEvolutionDetails(PokemonCardEvolution cardEvolution, List<EvolutionDetail> evolutionDetails)
            {
                //https://pokeapi.co/api/v2/evolution-trigger

                cardEvolution.Details ??= [];

                foreach (var evolutionDetail in evolutionDetails)
                {
                    var cardEvolutionDetail = new PokemonCardEvolutionDetails
                    {
                        TriggerType = evolutionDetail.Trigger?.Name
                    };

                    switch (cardEvolutionDetail.TriggerType)
                    {
                        case "trade":
                            {
                                if (evolutionDetail.HeldItem != null)
                                {
                                    evolutionDetail.Item = evolutionDetail.HeldItem;
                                    cardEvolutionDetail.TriggerType = "use-item";

                                    goto case "use-item";
                                }
                                else
                                {
                                    cardEvolutionDetail.TriggerType = "level-up";
                                    goto case "level-up";
                                }
                            }
                        case "use-item":
                            {
                                cardEvolutionDetail.Trigger = evolutionDetail.Item.Name;
                                cardEvolutionDetail.TriggerImageUrl = await pokemonDataService.GetPokemonItemAsync(evolutionDetail.Item.Url);
                            }
                            break;
                        case "level-up":
                            {
                                if (!string.IsNullOrEmpty(evolutionDetail.TimeOfDay))
                                {
                                    cardEvolutionDetail.TriggerType = "use-item";
                                    switch (evolutionDetail.TimeOfDay)
                                    {
                                        case "day":
                                            {
                                                cardEvolutionDetail.Trigger = "sun-stone";
                                                cardEvolutionDetail.TriggerImageUrl = await pokemonDataService.GetPokemonItemAsync("https://pokeapi.co/api/v2/item/80");
                                            }
                                            break;
                                        case "night":
                                            {
                                                cardEvolutionDetail.Trigger = "dusk-stone";
                                                cardEvolutionDetail.TriggerImageUrl = await pokemonDataService.GetPokemonItemAsync("https://pokeapi.co/api/v2/item/108");
                                            }
                                            break;
                                    }
                                    break;
                                }
                                
                                evolutionDetail.MinLevel ??= evolutionDetail.MinHappiness ?? evolutionDetail.MinBeauty ?? evolutionDetail.MinAffection ?? null;
                                cardEvolutionDetail.Trigger = Math.Ceiling((evolutionDetail.MinLevel.GetValueOrDefault() / 10.0)).ToString() ?? "3";
                                if (evolutionDetail.Location != null)
                                {
                                    cardEvolutionDetail.Location = evolutionDetail.Location.Name;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    cardEvolution.Details.Add(cardEvolutionDetail);
                }
            }
        }

        private static EvolutionChain? GetNextEvolutionChain(Pokemon pokemon, EvolutionChain evolutionChain)
        {
            if (evolutionChain == null) return null;

            if (evolutionChain.Species.Name == pokemon.Species.Name)
                return evolutionChain;

            if (evolutionChain.EvolvesTo == null) return null;

            foreach (var nextEvolution in evolutionChain.EvolvesTo)
            {
                var result = GetNextEvolutionChain(pokemon, nextEvolution);
                if (result != null) return result;
            }

            return null;
        }
    }
}
