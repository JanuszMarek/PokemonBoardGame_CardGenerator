using Newtonsoft.Json;

public class EvolutionChainUrl
{
	[JsonProperty("url")]
	public string Url { get; set; }
}

public class FlavorTextEntry
{
    [JsonProperty("flavor_text")]
    public string FlavorText { get; set; }

    [JsonProperty("language")]
    public PokemonLookup Language { get; set; }

    [JsonProperty("version")]
    public PokemonLookup Version { get; set; }
}

public class Genera
{
    [JsonProperty("genus")]
    public string Genus { get; set; }

    [JsonProperty("language")]
    public PokemonLookup Language { get; set; }
}

public class PokemonName
{
    [JsonProperty("language")]
    public PokemonLookup Language { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}

public class PalParkEncounter
{
	[JsonProperty("area")]
	public PokemonLookup Area { get; set; }

	[JsonProperty("base_score")]
	public int BaseScore { get; set; }

	[JsonProperty("rate")]
	public int Rate { get; set; }
}

public class PokedexNumber
{
	[JsonProperty("entry_number")]
	public int EntryNumber { get; set; }

	[JsonProperty("pokedex")]
	public PokemonLookup Pokedex { get; set; }
}

public class PokemonLookup
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("url")]
	public string Url { get; set; }
}

public class Variety
{
	[JsonProperty("is_default")]
	public bool IsDefault { get; set; }

	[JsonProperty("pokemon")]
	public PokemonLookup Pokemon { get; set; }
}

public class PokemonSpecies
{
	[JsonProperty("base_happiness")]
	public int BaseHappiness { get; set; }

	[JsonProperty("capture_rate")]
	public int CaptureRate { get; set; }

	[JsonProperty("color")]
	public PokemonLookup Color { get; set; }

	[JsonProperty("egg_groups")]
	public List<PokemonLookup> EggGroups { get; set; }

	[JsonProperty("evolution_chain")]
	public EvolutionChainUrl EvolutionChain { get; set; }

    [JsonProperty("evolves_from_species")]
    public PokemonLookup EvolvesFromSpecies { get; set; }

    [JsonProperty("flavor_text_entries")]
    public List<FlavorTextEntry> FlavorTextEntries { get; set; }

    [JsonProperty("form_descriptions")]
	public List<object> FormDescriptions { get; set; }

	[JsonProperty("forms_switchable")]
	public bool FormsSwitchable { get; set; }

	[JsonProperty("gender_rate")]
	public int GenderRate { get; set; }

    [JsonProperty("genera")]
    public List<Genera> Genera { get; set; }

    [JsonProperty("generation")]
    public PokemonLookup? Generation { get; set; }

    [JsonProperty("growth_rate")]
	public PokemonLookup GrowthRate { get; set; }

	[JsonProperty("habitat")]
	public PokemonLookup Habitat { get; set; }

	[JsonProperty("has_gender_differences")]
	public bool HasGenderDifferences { get; set; }

	[JsonProperty("hatch_counter")]
	public int HatchCounter { get; set; }

	[JsonProperty("id")]
	public int Id { get; set; }

	[JsonProperty("is_baby")]
	public bool IsBaby { get; set; }

	[JsonProperty("is_legendary")]
	public bool IsLegendary { get; set; }

	[JsonProperty("is_mythical")]
	public bool IsMythical { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

    [JsonProperty("names")]
    public List<PokemonName> Names { get; set; }

    [JsonProperty("order")]
	public int Order { get; set; }

	[JsonProperty("pal_park_encounters")]
	public List<PalParkEncounter> PalParkEncounters { get; set; }

	[JsonProperty("pokedex_numbers")]
	public List<PokedexNumber> PokedexNumbers { get; set; }

	[JsonProperty("shape")]
	public PokemonLookup Shape { get; set; }

	[JsonProperty("varieties")]
	public List<Variety> Varieties { get; set; }
}

