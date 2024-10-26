﻿using Newtonsoft.Json;
using PokemonBoardGame_CardGenerator.Enums;

namespace PokemonBoardGame_CardGenerator.Models.PokeApiModels
{

    public class Normal
	{
		[JsonProperty("use_after")]
		public object UseAfter { get; set; }

		[JsonProperty("use_before")]
		public List<global::PokemonLookup> UseBefore { get; set; }
	}

	public class Super
	{
		[JsonProperty("use_after")]
		public object UseAfter { get; set; }

		[JsonProperty("use_before")]
		public object UseBefore { get; set; }
	}

	public class ContestCombos
	{
		[JsonProperty("normal")]
		public Normal Normal { get; set; }

		[JsonProperty("super")]
		public Super Super { get; set; }
	}

	public class ContestEffect
	{
		[JsonProperty("url")]
		public string Url { get; set; }
	}

	public class ContestType
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}

	public class DamageClass
	{
		[JsonProperty("name")]
		public DamageClassEnum Name { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}

	public class EffectEntry
	{
		[JsonProperty("effect")]
		public string Effect { get; set; }

		[JsonProperty("language")]
		public global::PokemonLookup Language { get; set; }

		[JsonProperty("short_effect")]
		public string ShortEffect { get; set; }
	}

	public class FlavorTextEntry
	{
		[JsonProperty("flavor_text")]
		public string FlavorText { get; set; }

		[JsonProperty("language")]
		public global::PokemonLookup Language { get; set; }

		[JsonProperty("version_group")]
		public VersionGroup VersionGroup { get; set; }
	}

	public class Meta
	{
		[JsonProperty("ailment")]
		public global::PokemonLookup Ailment { get; set; }

		[JsonProperty("ailment_chance")]
		public int AilmentChance { get; set; }

		[JsonProperty("category")]
		public global::PokemonLookup Category { get; set; }

		[JsonProperty("crit_rate")]
		public int CritRate { get; set; }

		[JsonProperty("drain")]
		public int Drain { get; set; }

		[JsonProperty("flinch_chance")]
		public int FlinchChance { get; set; }

		[JsonProperty("healing")]
		public int Healing { get; set; }

		[JsonProperty("max_hits")]
		public object MaxHits { get; set; }

		[JsonProperty("max_turns")]
		public object MaxTurns { get; set; }

		[JsonProperty("min_hits")]
		public object MinHits { get; set; }

		[JsonProperty("min_turns")]
		public object MinTurns { get; set; }

		[JsonProperty("stat_chance")]
		public int StatChance { get; set; }
	}

	public class PokemonMoveName
	{
		[JsonProperty("language")]
		public global::PokemonLookup Language { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}

	public class SuperContestEffect
	{
		[JsonProperty("url")]
		public string Url { get; set; }
	}

	public class TypeUrl
	{
		[JsonProperty("name")]
		public PokemonTypeEnum Name { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}

	public class PokemonMove
	{
		[JsonProperty("accuracy")]
		public int? Accuracy { get; set; }

		[JsonProperty("contest_combos")]
		public ContestCombos ContestCombos { get; set; }

		[JsonProperty("contest_effect")]
		public ContestEffect ContestEffect { get; set; }

		[JsonProperty("contest_type")]
		public ContestType ContestType { get; set; }

		[JsonProperty("damage_class")]
		public DamageClass DamageClass { get; set; }

		[JsonProperty("effect_chance")]
		public string EffectChance { get; set; }

		[JsonProperty("effect_changes")]
		public List<object> EffectChanges { get; set; }

		[JsonProperty("effect_entries")]
		public List<EffectEntry> EffectEntries { get; set; }

		[JsonProperty("flavor_text_entries")]
		public List<FlavorTextEntry> FlavorTextEntries { get; set; }

		[JsonProperty("generation")]
		public PokemonLookup Generation { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("learned_by_pokemon")]
		public List<PokemonLookup> LearnedByPokemon { get; set; }

		[JsonProperty("machines")]
		public List<object> Machines { get; set; }

		[JsonProperty("meta")]
		public Meta Meta { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("names")]
		public List<PokemonMoveName> Names { get; set; }

		[JsonProperty("past_values")]
		public List<object> PastValues { get; set; }

		[JsonProperty("power")]
		public int? Power { get; set; }

		[JsonProperty("pp")]
		public int? Pp { get; set; }

		[JsonProperty("priority")]
		public int? Priority { get; set; }

		[JsonProperty("stat_changes")]
		public List<object> StatChanges { get; set; }

		[JsonProperty("super_contest_effect")]
		public SuperContestEffect SuperContestEffect { get; set; }

		[JsonProperty("target")]
		public PokemonLookup Target { get; set; }

		[JsonProperty("type")]
		public TypeUrl Type { get; set; }
	}


}
