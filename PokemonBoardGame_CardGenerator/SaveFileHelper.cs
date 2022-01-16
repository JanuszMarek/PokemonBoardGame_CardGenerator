using Newtonsoft.Json;
using PokemonBoardGame_CardGenerator.Models;

namespace PokemonBoardGame_CardGenerator
{
	public static class SaveFileHelper
	{
		public static void CreateFolderWhenNotExist(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public static async Task SavePokemonImagesAsync(string path, PokemonImage pokemonImage)
		{
			Console.WriteLine($"Save image for Poke: {pokemonImage.FileName}");
			await File.WriteAllBytesAsync(path, pokemonImage.Image);
		}

		public static async Task SavePokemonDataJsonAsync<T>(string path, int pokeNo, T model)
		{
			Console.WriteLine($"Save {typeof(T).Name} for Poke: {pokeNo}");
			var json = JsonConvert.SerializeObject(model, Formatting.Indented);
			await File.WriteAllTextAsync(path + pokeNo + ".json", json, System.Text.Encoding.UTF8);
		}

		public static async Task SavePokemonDataJsonAsync<T>(string path, string name, T model)
		{
			Console.WriteLine($"Save {typeof(T).Name} for: {name}");
			var json = JsonConvert.SerializeObject(model, Formatting.Indented);
			await File.WriteAllTextAsync(path + name + ".json", json, System.Text.Encoding.UTF8);
		}
	}
}
