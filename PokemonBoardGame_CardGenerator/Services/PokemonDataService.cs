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
			CreateFolderWhenNotExist(imagePath);

			var pokemonFileName = pokeNo + imageHttpService.GetPokemonImageExtension();
			var pokePath = imagePath + pokemonFileName;

			if (!File.Exists(pokePath))
			{
				var bytes = await imageHttpService.GetPokemonImageAsync(pokeNo.ToString());
				await SavePokemonImagesAsync(pokePath,
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
			CreateFolderWhenNotExist(dirPath);

			var pokemonFileName = pokeNo.ToString() + ".json";
			var pokePath = dirPath + pokemonFileName;
			Pokemon pokemon; 

			if (!File.Exists(pokePath))
			{
				pokemon = await pokeApiHttpService.GetPokemonAsync(pokeNo);
				await SavePokemonDataJsonAsync(dirPath, pokeNo, pokemon);
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
			CreateFolderWhenNotExist(dirPath);

			var pokemonFileName = move + ".json";
			var pokePath = dirPath + pokemonFileName;
			PokemonMove data;

			if (!File.Exists(pokePath))
			{
				data = await pokeApiHttpService.GetPokemonMoveAsync(move);
				await SavePokemonDataJsonAsync(dirPath, move, data);
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
			CreateFolderWhenNotExist(dirPath);

			var pokemonFileName = pokeNo.ToString() + ".json";
			var pokePath = dirPath + pokemonFileName;
			PokemonSpecies pokemon;

			if (!File.Exists(pokePath))
			{
				pokemon = await pokeApiHttpService.GetPokemonSpeciesAsync(pokeNo);
				await SavePokemonDataJsonAsync(dirPath, pokeNo, pokemon);
			}
			else
			{
				var json = await File.ReadAllTextAsync(pokePath);
				pokemon = JsonConvert.DeserializeObject<PokemonSpecies>(json);
			}

			return pokemon;
		}

		private async Task SavePokemonImagesAsync(string path, PokemonImage pokemonImage)
		{
			Console.WriteLine($"Save image for Poke: {pokemonImage.FileName}");
			await File.WriteAllBytesAsync(path, pokemonImage.Image);
		}

		private async Task SavePokemonDataJsonAsync<T>(string path, int pokeNo, T model)
		{
			Console.WriteLine($"Save {typeof(T).Name} for Poke: {pokeNo}");
			var json = JsonConvert.SerializeObject(model, Formatting.Indented);
			await File.WriteAllTextAsync(path + pokeNo + ".json", json, System.Text.Encoding.UTF8);
		}

		private async Task SavePokemonDataJsonAsync<T>(string path, string name, T model)
		{
			Console.WriteLine($"Save {typeof(T).Name} for: {name}");
			var json = JsonConvert.SerializeObject(model, Formatting.Indented);
			await File.WriteAllTextAsync(path + name + ".json", json, System.Text.Encoding.UTF8);
		}

		private void CreateFolderWhenNotExist(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
	}
}
