using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PokemonBoardGame_CardGenerator.Helpers;
using PokemonBoardGame_CardGenerator.HttpClients;
using PokemonBoardGame_CardGenerator.HttpClients.Implementations;
using PokemonBoardGame_CardGenerator.Models;
using PokemonBoardGame_CardGenerator.Models.PokeApiModels;
using PokemonBoardGame_CardGenerator.Options;
using System;

namespace PokemonBoardGame_CardGenerator.Services
{
    public class PokemonDataService(IPokemonImageHttpService imageHttpService, PokeApiHttpService pokeApiHttpService, IOptions<PokemonSettings> settings)
    {
        private readonly PokemonSettings pokemonSettings = settings.Value;

        private Dictionary<int, Pokemon> Pokemons = [];
        private Dictionary<string, PokemonMove> PokemonMoves = [];
        private Dictionary<int, PokemonSpecies> PokemonSpecies = [];
        private Dictionary<string, PokemonEvolutionChain> PokemonEvolutionChains = [];
        private Dictionary<int, string> PokemonImages = [];
        private Dictionary<int, string> PokemonItems = [];

        public async Task<Pokemon> GetPokemonAsync(int id)
        {
            if (!Pokemons.ContainsKey(id))
                Pokemons.TryAdd(id, await GetPokemonDataAsync(id));

            return Pokemons[id];
        }

        public async Task<PokemonSpecies> GetPokemonSpeciesAsync(int id)
        {
            if (!PokemonSpecies.ContainsKey(id))
                PokemonSpecies.TryAdd(id, await GetPokemonSpeciesDataAsync(id));

            return PokemonSpecies[id];
        }

        public string GetPokemonPokedexImageUrlAsync(int id)
        {
            if (!PokemonImages.ContainsKey(id))
                PokemonImages.TryAdd(id, GetPokemonImageUrl(id));

            return PokemonImages[id];
        }

        public async Task<PokemonEvolutionChain> GetPokemonPokemonEvolutionChainAsync(string url)
        {
            if (!PokemonEvolutionChains.ContainsKey(url))
                PokemonEvolutionChains.TryAdd(url, await GetPokemonEvolutionChainDataAsync(url));

            return PokemonEvolutionChains[url];
        }

        public async Task<PokemonMove> GetPokemonMoveAsync(string name)
        {
            if (!PokemonMoves.ContainsKey(name))
                PokemonMoves.TryAdd(name, await GetPokemonMoveDataAsync(name));

            return PokemonMoves[name];
        }

        public async Task<string> GetPokemonItemAsync(string itemUrl)
        {
            var no = UrlHelper.GetIdFromUrl(itemUrl, "item/");

            if (!PokemonItems.ContainsKey(no))
                PokemonItems.TryAdd(no, (await GetPokemonItemDataAsync(itemUrl))?.Sprites?.Default);

            return PokemonItems[no];
        }

        public string GetPokemonImageUrl(int pokeNo)
        {
            return imageHttpService.GetPokemonImagePath(pokeNo, true);
        }

        private async Task<string> GetPokemonImageAsync(int pokeNo)
        {
            var imagePath = pokemonSettings.OutputPath + "Images/";
            SaveFileHelper.CreateFolderWhenNotExist(imagePath);

            var pokemonFileName = pokeNo + imageHttpService.GetPokemonImageExtension();
            var pokePath = imagePath + pokemonFileName;

            if (!File.Exists(pokePath))
            {
                var bytes = await imageHttpService.GetPokemonImageAsync(pokeNo);
                await SaveFileHelper.SavePokemonImagesAsync(pokePath,
                    new PokemonImage()
                    {
                        FileName = pokemonFileName,
                        Image = bytes
                    });
            }

            return pokePath;
        }

        private async Task<Pokemon> GetPokemonDataAsync(int pokeNo)
        {
            var dirPath = pokemonSettings.OutputPath + "Data/Pokemons/";
            SaveFileHelper.CreateFolderWhenNotExist(dirPath);

            var pokemonFileName = pokeNo.ToString() + ".json";
            var pokePath = dirPath + pokemonFileName;
            Pokemon result;

            if (!File.Exists(pokePath))
            {
                result = await pokeApiHttpService.GetPokemonAsync(pokeNo);
                await SaveFileHelper.SavePokemonDataJsonAsync(dirPath, pokeNo, result);
            }
            else
            {
                var json = await File.ReadAllTextAsync(pokePath);
                result = JsonConvert.DeserializeObject<Pokemon>(json);
            }

            return result;
        }

        private async Task<PokemonMove> GetPokemonMoveDataAsync(string move)
        {
            var dirPath = pokemonSettings.OutputPath + "Data/PokemonMoves/";
            SaveFileHelper.CreateFolderWhenNotExist(dirPath);

            var pokemonFileName = move + ".json";
            var pokePath = dirPath + pokemonFileName;
            PokemonMove result;

            if (!File.Exists(pokePath))
            {
                result = await pokeApiHttpService.GetPokemonMoveAsync(move);
                await SaveFileHelper.SavePokemonDataJsonAsync(dirPath, move, result);
            }
            else
            {
                var json = await File.ReadAllTextAsync(pokePath);
                result = JsonConvert.DeserializeObject<PokemonMove>(json);
            }

            return result;
        }

        private async Task<PokemonSpecies> GetPokemonSpeciesDataAsync(int pokeNo)
        {
            var dirPath = pokemonSettings.OutputPath + "Data/PokemonSpecies/";
            SaveFileHelper.CreateFolderWhenNotExist(dirPath);

            var pokemonFileName = pokeNo.ToString() + ".json";
            var pokePath = dirPath + pokemonFileName;
            PokemonSpecies result;

            if (!File.Exists(pokePath))
            {
                result = await pokeApiHttpService.GetPokemonSpeciesAsync(pokeNo);
                await SaveFileHelper.SavePokemonDataJsonAsync(dirPath, pokeNo, result);
            }
            else
            {
                var json = await File.ReadAllTextAsync(pokePath);
                result = JsonConvert.DeserializeObject<PokemonSpecies>(json);
            }

            return result;
        }

        private async Task<PokemonEvolutionChain> GetPokemonEvolutionChainDataAsync(string evolutionUrl)
        {
            var dirPath = pokemonSettings.OutputPath + "Data/PokemonEvolution/";
            SaveFileHelper.CreateFolderWhenNotExist(dirPath);

            var id = UrlHelper.GetIdFromUrl(evolutionUrl, "evolution-chain/");

            var pokemonFileName = id.ToString() + ".json";
            var pokePath = dirPath + pokemonFileName;
            PokemonEvolutionChain result;

            if (!File.Exists(pokePath))
            {
                result = await pokeApiHttpService.GetPokemonEvolutionChainAsync(id);
                await SaveFileHelper.SavePokemonDataJsonAsync(dirPath, id, result);
            }
            else
            {
                var json = await File.ReadAllTextAsync(pokePath);
                result = JsonConvert.DeserializeObject<PokemonEvolutionChain>(json);
            }

            return result;
        }


        private async Task<PokemonItem> GetPokemonItemDataAsync(string url)
        {
            var no = UrlHelper.GetIdFromUrl(url, "item/");
            var result = await pokeApiHttpService.GetPokemonItemAsync(no);

            return result;
        }
    }
}
