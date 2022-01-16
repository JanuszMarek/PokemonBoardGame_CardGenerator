
namespace PokemonBoardGame_CardGenerator.HttpClients
{
	public interface IPokemonImageHttpService
	{
		Task<byte[]> GetPokemonImageAsync(string pokeNo);
		string GetPokemonImageExtension();
	}
}