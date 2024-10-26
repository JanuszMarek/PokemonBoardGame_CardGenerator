// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokemonBoardGame_CardGenerator.HttpClients;
using PokemonBoardGame_CardGenerator.HttpClients.Implementations;
using PokemonBoardGame_CardGenerator.Options;
using PokemonBoardGame_CardGenerator.Services;

Console.WriteLine("Hello, PBG Card Generator!");
var configuration = BuildConfig();
var serviceProvider = RegisterServices(configuration);
var cardService = serviceProvider.GetService<PokemonCardService>();

await cardService.GeneratePokemonCardsAsync();

ServiceProvider RegisterServices(IConfiguration configuration)
{
	var serviceCollection = new ServiceCollection();

	serviceCollection.AddOptions();
	
	serviceCollection.Configure<PokemonSettings>(x => configuration.GetSection("PokemonSettings").Bind(x));

	serviceCollection.AddHttpClient<IPokemonImageHttpService, PokemonAssetsService>();
	serviceCollection.AddHttpClient<PokeApiHttpService>();

	serviceCollection.AddTransient<PokemonCardService>();
	serviceCollection.AddTransient<PokemonEvolutionCardService>();
	serviceCollection.AddTransient<PokemonMoveCardService>();

	serviceCollection.AddSingleton<PokemonDataService>();

	return serviceCollection.BuildServiceProvider();

}

IConfiguration BuildConfig()
{
	return new ConfigurationBuilder()
		.SetBasePath(Directory.GetCurrentDirectory())
		.AddJsonFile("appsettings.json", false, true)
		.Build();
}