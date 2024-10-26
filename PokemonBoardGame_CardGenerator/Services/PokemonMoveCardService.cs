using PokemonBoardGame_CardGenerator.Models;
using PokemonBoardGame_CardGenerator.Models.PokeApiModels;

namespace PokemonBoardGame_CardGenerator.Services
{
    public class PokemonMoveCardService(PokemonDataService pokemonDataService)
    {
        public async Task<List<PokemonCardMoveModel>> GetBestPokemonMoves(Pokemon pokemon, List<EvolutionChain> evolutionChain)
        {
            var allMoves = await GetAllPokemonMoves(pokemon);


            return allMoves.Select(x => MapApiMoveToLocalType(x, pokemon)).ToList();
        }

        private async Task<List<PokemonMove>> GetAllPokemonMoves(Pokemon pokemon)
        {
            var moveNames = pokemon.Moves.Select(x => x.Move2.Name).Distinct();
            var moves = new List<PokemonMove>();

            foreach (var moveName in moveNames)
            {
                var move = await pokemonDataService.GetPokemonMoveAsync(moveName);
                moves.Add(move);
            }

            return moves;
        }

        //private List<PokemonMove> GetMovesFromPrevEvolutions(Pokemon pokemon, List<EvolutionChain> evolutionChains, List<PokemonMove> moves = null)
        //{
        //    moves ??= new List<PokemonMove>();

        //    foreach (var evolution in evolutionChains)
        //    {
        //        if (evolution.Species == pokemon.Species) return moves;


        //    }


        //    static void GetPrevEvoMoves(PokemonCardModel pokemon, List<EvolutionChain> evolutionChains, List<PokemonCardModel> pokemonCards)
        //    {
        //        foreach (var evolutionChain in evolutionChains)
        //        {
        //            if (evolutionChain.Species.Name.ToLower() != pokemon.Name.ToLower())
        //            {
        //                var prevEvo = pokemonCards.FirstOrDefault(x => x.Name.ToLower() == evolutionChain.Species.Name.ToLower());
        //                if (prevEvo != null)
        //                {
        //                    foreach (var move in prevEvo?.Moves)
        //                    {
        //                        var newMove = move.Clone();
        //                        newMove.SetDamage(pokemon);

        //                        pokemon?.Moves?.Add(newMove);
        //                    }

        //                    GetPrevEvoMoves(pokemon, evolutionChain.EvolvesTo, pokemonCards);
        //                }

        //                pokemon.Moves = pokemon.Moves.Distinct().OrderBy(x => x.DamageClass).ThenByDescending(x => x.Damage).ToList();
        //            }
        //        }
        //    }
        //}

        private static PokemonCardMoveModel MapApiMoveToLocalType(PokemonMove x, Pokemon pokemon)
        {
            var moveFromPokemon = pokemon.Moves.FirstOrDefault(p => p.Move2.Name == x.Name);
            var version = moveFromPokemon?.VersionGroupDetails.LastOrDefault();

            var move = new PokemonCardMoveModel()
            {
                Name = x.Name,
                Accuracy = (int?)Math.Round(x.Accuracy.GetValueOrDefault() / 10.0),
                LearnMethod = version?.MoveLearnMethod.Name,
                LevelLearnedAt = version?.LevelLearnedAt,
                PowerUp = PowerTransformation(x.Power),
                MoveUsage = x.Pp / 5 == 0 ? 1 : x.Pp / 5,
                Type = x.Type.Name,
                DamageClass = x.DamageClass.Name,
                Description = x.EffectEntries.FirstOrDefault()?.ShortEffect
                    .Replace("$effect_chance", x.EffectChance)
                    //.Replace("Special", "")
                    .Replace("several", "2-5")
                    .Replace(" for 1-8 turns", "")
                    .Replace("  ", " ")
            };

            return move;
        }

        private static int? PowerTransformation(int? initPower)
        {
            if (!initPower.HasValue) return null;

            switch (initPower.Value)
            {
                case int n when n >= 250: { return 5; }
                case int n when n >= 180: { return 4; }
                case int n when n >= 140: { return 3; }
                case int n when n >= 100: { return 2; }
                case int n when n >= 60: { return 1; }
                default: { return 0; }
            }
        }

        //private void LimitMovesToEvoLvl(List<PokemonCardModel> pokemonCards)
        //{
        //    foreach (var pokemonCard in pokemonCards)
        //    {
        //        var lvl = GetEvolutionLevel(new List<EvolutionChain> { pokemonCard.EvolutionChain }, pokemonCard.Name);
        //        var limitedMoves = pokemonCard.Moves.Where(x => x.LevelLearnedAt == 0 || !lvl.HasValue || x.LevelLearnedAt <= lvl).ToList();
        //        if (limitedMoves.Count > 4)
        //        {
        //            pokemonCard.Moves = limitedMoves;
        //        }
        //    }
        //}

        private int? GetEvolutionLevel(List<EvolutionChain> evolutions, string pokeName)
        {
            foreach (var evolutionChain in evolutions)
            {
                if (evolutionChain.Species.Name.ToLower() == pokeName.ToLower())
                {
                    return evolutionChain.EvolvesTo.Where(x => x.EvolutionDetails.Any(x => x.Trigger.Name == "level-up")).FirstOrDefault()?
                        .EvolutionDetails.Where(x => x.Trigger.Name == "level-up").FirstOrDefault()?.MinLevel;
                }
                return GetEvolutionLevel(evolutionChain.EvolvesTo, pokeName);
            }

            return null;
        }

        //private PokemonCardMoveModel? SelectOptimumMoves(PokemonCardModel pokemonCard, List<PokemonCardMoveModel> moves)
        //{
        //    PokemonCardMoveModel toAdd = null;
        //    if (!moves.Any(x => x.DamageClass == DamageClassEnum.Status))
        //    {
        //        toAdd = pokemonCard.Moves.FirstOrDefault(x => x.DamageClass == DamageClassEnum.Status && !moves.Contains(x));
        //        if (toAdd != null)
        //            return toAdd;
        //    }

        //    if (!moves.Any(x => x.IsAttackingMove() && x.MoveUsage >= 2))
        //    {
        //        toAdd = pokemonCard.Moves.FirstOrDefault(x => x.IsAttackingMove() && x.Type == pokemonCard.Types[0] && x.MoveUsage >= 2 && x.Damage > 0 && !moves.Contains(x));
        //        if (toAdd != null)
        //            return toAdd;
        //    }

        //    if (!moves.Any(x => x.IsAttackingMove() && x.MoveUsage >= 2))
        //    {
        //        toAdd = pokemonCard.Moves.FirstOrDefault(x => x.IsAttackingMove() && x.MoveUsage >= 2 && x.Damage > 0 && !moves.Contains(x));
        //        if (toAdd != null)
        //            return toAdd;
        //    }

        //    toAdd = pokemonCard.Moves.FirstOrDefault(x => x.IsAttackingMove() && x.Type == pokemonCard.Types[0] && x.Damage > 0 && !moves.Contains(x));
        //    if (toAdd != null)
        //        return toAdd;

        //    toAdd = pokemonCard.Moves.FirstOrDefault(x => x.IsAttackingMove() && x.Type == PokemonTypeEnum.Normal && x.Damage > 0 && !moves.Contains(x));
        //    if (toAdd != null)
        //        return toAdd;

        //    toAdd = pokemonCard.Moves.FirstOrDefault(x => x.IsAttackingMove() && !moves.Contains(x));
        //    if (toAdd != null)
        //        return toAdd;

        //    toAdd = pokemonCard.Moves.FirstOrDefault(x => !moves.Contains(x));

        //    return toAdd;
        //}
    }
}
