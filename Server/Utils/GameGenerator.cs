using System.Numerics;
using Domain.Models.Game;

namespace Server.Utils;

public static class GameGenerator
{
    public static Dictionary<Guid,PlayerModel> GeneratePlayers(int count, MapCell[,] map, int mapSize)
    {
        var players = new Dictionary<Guid, PlayerModel>();

        for (int i = 0; i < count; i++)
        {
            var id = Guid.NewGuid();
            Vector2 position;
            do
            {
                position = new Vector2(Random.Shared.Next(0, mapSize), Random.Shared.Next(0, mapSize));
            } while (map[(int)position.X, (int)position.Y].Containment == MapCellContainment.Wall);
            players[id] = new PlayerModel()
            {
                Id = id,
                Health = Random.Shared.Next(30,100),
                Position = position,
                UserId = Guid.NewGuid()
            };
        }

        return players;
    }

    public static MapCell[,] GenerateMap(int mapSize)
    {
        var map = new MapCell[mapSize, mapSize];
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                map[x, y] = new MapCell()
                {
                    Containment = Random.Shared.NextSingle() < 0.1f ? MapCellContainment.Wall : MapCellContainment.Empty,
                };
            }
        }

        return map;
    }
}