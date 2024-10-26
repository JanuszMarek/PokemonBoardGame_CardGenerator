using PokemonBoardGame_CardGenerator.Enums;
using PokemonBoardGame_CardGenerator.Extensions;
using PokemonBoardGame_CardGenerator.Helpers;
using PokemonBoardGame_CardGenerator.Models;
using PokemonBoardGame_CardGenerator.Models.PokeApiModels;

namespace PokemonBoardGame_CardGenerator.Services
{
    public class PokemonMoveCardService(PokemonDataService pokemonDataService)
    {
        public async Task<IEnumerable<PokemonCardMoveModel>> GetBestPokemonMoves(Pokemon pokemon, PokemonSpecies pokemonSpecies)
        {
            var allMoves = await GetAllPokemonMoves(pokemon);
            allMoves.AddRange(await GetMovesFromPrevEvolutions(pokemonSpecies) ?? []);

            var bestmoves = Select4BestMovesV2(pokemon, OrderMoves(allMoves));

            return bestmoves.Select(MapApiMoveToLocalType);
        }

        private static List<PokemonMove> OrderMoves(List<PokemonMove> pokemonMoves)
        {
            return [.. pokemonMoves.OrderByDescending(x => x.Type.Name).ThenByDescending(x => x.Power).ThenByDescending(x => x.Pp).ThenByDescending(x => x.Accuracy)];
        }

        private async Task<List<PokemonMove>> GetAllPokemonMoves(Pokemon pokemon)
        {
            string[] moveMethodsToFilterOut = ["machine", "tutor"];
            var moveNames = pokemon.Moves.Where(x => !x.VersionGroupDetails.Any(y => moveMethodsToFilterOut.Contains(y.MoveLearnMethod.Name))).Select(x => x.Move2.Name).Distinct();
            var moves = new List<PokemonMove>();

            foreach (var moveName in moveNames)
            {
                var move = await pokemonDataService.GetPokemonMoveAsync(moveName);
                moves.Add(move);
            }

            return moves;
        }

        private async Task<List<PokemonMove>?> GetMovesFromPrevEvolutions(PokemonSpecies pokemonSpecies)
        {
            if (pokemonSpecies?.EvolvesFromSpecies == null) return null;

            var prevEvoPokemonId = UrlHelper.GetPokemonIdFromSpecies(pokemonSpecies?.EvolvesFromSpecies).GetValueOrDefault();
            var prevEvoPokemonSpecies = await pokemonDataService.GetPokemonSpeciesAsync(prevEvoPokemonId);
            var prevEvoPokemon = await pokemonDataService.GetPokemonAsync(prevEvoPokemonId);

            var moves = await GetAllPokemonMoves(prevEvoPokemon);
            moves.AddRange(await GetMovesFromPrevEvolutions(prevEvoPokemonSpecies) ?? []);

            return moves;
        }

        private List<PokemonMove?> Select4BestMovesV2(Pokemon pokemon, List<PokemonMove> allMoves)
        {
            PokemonMove? move1 = null, move2 = null, move3 = null, move4 = null;

            var pokemonTypes = pokemon.Types.OrderBy(x => x.Slot).Select(x => (x.Type?.Name?.ToEnum<PokemonTypeEnum>()).Value);

            move1 = allMoves.FirstOrDefault(x => x.Type.Name == pokemonTypes.First() && x.DamageClass.Name != DamageClassEnum.Status);

            if (pokemonTypes.Count() > 1)
            {
                move2 = allMoves.FirstOrDefault(x => x.Type.Name == pokemonTypes.Last() && x.DamageClass.Name != DamageClassEnum.Status);
            }
            else
            {
                move2 = allMoves.FirstOrDefault(x => x.Type.Name == pokemonTypes.First() && x.DamageClass.Name != DamageClassEnum.Status && x.Id != move1?.Id);
            }

            move3 = allMoves.FirstOrDefault(x => (pokemonTypes.Contains(x.Type.Name) || x.Type.Name == PokemonTypeEnum.Normal) && x.DamageClass.Name == DamageClassEnum.Status);

            move4 = allMoves.FirstOrDefault(x => (pokemonTypes.Contains(x.Type.Name) || x.Type.Name == PokemonTypeEnum.Normal) && x.DamageClass.Name == DamageClassEnum.Physical && x.Pp > 30);

            return new List<PokemonMove?> { move1, move2, move3, move4 };
        }

        private List<PokemonMove> Select4BestMoves(Pokemon pokemon, List<PokemonMove> allMoves)
        {
            List<PokemonMove> bestMoves = [];
            bool hasType1Move = false,
                hasType2Move = false,
                hasStrongMove = false,
                hasManyPPMove = false,
                hasStatusMove = false,
                hasPhysicalAttack = false,
                hasSpecialAttack = false;

            var pokemonTypes = pokemon.Types.ToDictionary(x => x.Slot, x => x.Type?.Name?.ToEnum<PokemonTypeEnum>());

            foreach (var move in allMoves)
            {
                if (bestMoves.Count == 4) break;

                var canBeAdded = false;

                if (move.DamageClass.Name == DamageClassEnum.Status)
                {
                    if (!hasStatusMove)
                    {
                        hasStatusMove = true;
                        canBeAdded = true;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (pokemonTypes.TryGetValue(1, out PokemonTypeEnum? value) && move.Type.Name == value.Value && !hasType1Move && move.DamageClass.Name != DamageClassEnum.Status)
                {
                    hasType1Move = true;
                    canBeAdded = true;
                }

                if (pokemonTypes.TryGetValue(2, out PokemonTypeEnum? value2) && move.Type.Name == value2.Value && !hasType2Move && move.DamageClass.Name != DamageClassEnum.Status)
                {
                    hasType2Move = true;
                    canBeAdded = true;
                }

                if (move.DamageClass.Name == DamageClassEnum.Special && !hasSpecialAttack)
                {
                    hasSpecialAttack = true;
                    canBeAdded = true;
                }

                if (move.DamageClass.Name == DamageClassEnum.Physical && !hasPhysicalAttack)
                {
                    hasPhysicalAttack = true;
                    canBeAdded = true;
                }

                if (move.Power > 140 && !hasStrongMove)
                {
                    hasStrongMove = true;
                    canBeAdded = true;
                }

                if (move.Pp > 30 && !hasManyPPMove)
                {
                    hasManyPPMove = true;
                    canBeAdded = true;
                }

                if (canBeAdded) bestMoves.Add(move);
            }

            return bestMoves;
        }

        private static PokemonCardMoveModel? MapApiMoveToLocalType(PokemonMove x)
        {
            return x != null ? new PokemonCardMoveModel()
            {
                Name = x.Name,
                Accuracy = (int?)Math.Round(x.Accuracy.GetValueOrDefault() / 10.0),
                PowerUp = PowerTransformation(x.Power),
                MoveUsage = x.Pp / 5 == 0 ? 1 : x.Pp / 5,
                Type = x.Type.Name,
                EffectChance = x.EffectChance,
                DamageClass = x.DamageClass.Name,
                Description = x.EffectEntries.FirstOrDefault()?.ShortEffect
                    .Replace("$effect_chance", x.EffectChance)
                    //.Replace("Special", "")
                    .Replace("several", "2-5")
                    .Replace(" for 1-8 turns", "")
                    .Replace("  ", " ")
            } : null;
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
