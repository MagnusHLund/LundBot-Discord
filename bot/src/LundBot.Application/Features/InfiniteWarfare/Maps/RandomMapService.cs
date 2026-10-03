using LundBot.Domain.Common.Enums;

namespace LundBot.Application.Features.InfiniteWarfare.Maps
{
    public sealed class RandomMapService : IRandomMapService
    {
        public string GetRandomMap()
        {
            var maps = Enum.GetValues<InfiniteWarfareZombiesMapsEnum>();
            var randomMap = maps[Random.Shared.Next(maps.Length)];

            return randomMap switch
            {
                InfiniteWarfareZombiesMapsEnum.ZombiesInSpaceland => "Zombies In Spaceland",
                InfiniteWarfareZombiesMapsEnum.RaveInTheRedwoods => "Rave In The Redwoods",
                InfiniteWarfareZombiesMapsEnum.ShaolinShuffle => "Shaolin Shuffle",
                InfiniteWarfareZombiesMapsEnum.AttackOfTheRadioactiveThing => "Attack Of The Radioactive Thing",
                InfiniteWarfareZombiesMapsEnum.TheBeastFromBeyond => "The Beast From Beyond",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
