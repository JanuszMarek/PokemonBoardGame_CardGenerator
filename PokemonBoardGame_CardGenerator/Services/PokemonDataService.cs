using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PokemonBoardGame_CardGenerator.HttpClients;
using PokemonBoardGame_CardGenerator.HttpClients.Implementations;
using PokemonBoardGame_CardGenerator.Models;
using PokemonBoardGame_CardGenerator.Models.PokeApiModels;
using PokemonBoardGame_CardGenerator.Options;

namespace PokemonBoardGame_CardGenerator.Services
{
	public class PokemonDataService
	{
		private readonly IPokemonImageHttpService imageHttpService;
		private readonly PokeApiHttpService pokeApiHttpService;
		private readonly PokemonSettings pokemonSettings;


		public PokemonDataService(IPokemonImageHttpService imageHttpService, PokeApiHttpService pokeApiHttpService, IOptions<PokemonSettings> settings)
		{
			this.imageHttpService = imageHttpService;
			this.pokeApiHttpService = pokeApiHttpService;
			this.pokemonSettings = settings.Value;
		}

		public async Task<string> GetPokemonImageAsync(int pokeNo)
		{
			var imagePath = pokemonSettings.OutputPath + "Images/";
			SaveFileHelper.CreateFolderWhenNotExist(imagePath);

			var pokemonFileName = pokeNo + imageHttpService.GetPokemonImageExtension();
			var pokePath = imagePath + pokemonFileName;

			if (!File.Exists(pokePath))
			{
				var bytes = await imageHttpService.GetPokemonImageAsync(pokeNo.ToString());
				await SaveFileHelper.SavePokemonImagesAsync(pokePath,
					new PokemonImage()
					{
						FileName = pokemonFileName,
						Image = bytes
					});
			}

			return pokePath;
		}

		public async Task<Pokemon> GetPokemonDataAsync(int pokeNo)
		{
			var dirPath = pokemonSettings.OutputPath + "Data/Pokemons/";
			SaveFileHelper.CreateFolderWhenNotExist(dirPath);

			var pokemonFileName = pokeNo.ToString() + ".json";
			var pokePath = dirPath + pokemonFileName;
			Pokemon pokemon; 

			if (!File.Exists(pokePath))
			{
				pokemon = await pokeApiHttpService.GetPokemonAsync(pokeNo);
				await SaveFileHelper.SavePokemonDataJsonAsync(dirPath, pokeNo, pokemon);
			}
			else
			{
				var json = await File.ReadAllTextAsync(pokePath);
				pokemon = JsonConvert.DeserializeObject<Pokemon>(json);
			}

			return pokemon;
		}

		public async Task<PokemonMove> GetPokemonMoveAsync(string move)
		{
			var dirPath = pokemonSettings.OutputPath + "Data/PokemonMoves/";
			SaveFileHelper.CreateFolderWhenNotExist(dirPath);

			var pokemonFileName = move + ".json";
			var pokePath = dirPath + pokemonFileName;
			PokemonMove data;

			if (!File.Exists(pokePath))
			{
				data = await pokeApiHttpService.GetPokemonMoveAsync(move);
				await SaveFileHelper.SavePokemonDataJsonAsync(dirPath, move, data);
			}
			else
			{
				var json = await File.ReadAllTextAsync(pokePath);
				data = JsonConvert.DeserializeObject<PokemonMove>(json);
			}

			return data;
		}

		public async Task<PokemonSpecies> GetPokemonSpeciesDataAsync(int pokeNo)
		{
			var dirPath = pokemonSettings.OutputPath + "Data/PokemonSpecies/";
			SaveFileHelper.CreateFolderWhenNotExist(dirPath);

			var pokemonFileName = pokeNo.ToString() + ".json";
			var pokePath = dirPath + pokemonFileName;
			PokemonSpecies pokemon;

			if (!File.Exists(pokePath))
			{
				pokemon = await pokeApiHttpService.GetPokemonSpeciesAsync(pokeNo);
				await SaveFileHelper.SavePokemonDataJsonAsync(dirPath, pokeNo, pokemon);
			}
			else
			{
				var json = await File.ReadAllTextAsync(pokePath);
				pokemon = JsonConvert.DeserializeObject<PokemonSpecies>(json);
			}

			return pokemon;
		}
	}
}
