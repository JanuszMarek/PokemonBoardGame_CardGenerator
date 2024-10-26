
namespace PokemonBoardGame_CardGenerator.HttpClients
{
	public interface IPokemonImageHttpService
	{
		Task<byte[]> GetPokemonImageAsync(int pokeNo);
        string GetPokemonImagePath(int pokeNo, bool fullpath = false);

        string GetPokemonImageExtension();

    }
}