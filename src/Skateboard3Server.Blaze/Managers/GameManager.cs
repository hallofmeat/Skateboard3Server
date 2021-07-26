using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Skateboard3Server.Blaze.Managers
{
    public interface IGameManager
    {
        Game CreateGame(string gameName, uint gameSettings, Dictionary<string, string> gameAttributes);
        void UpdateAttributes(uint gameId, Dictionary<string, string> gameAttributes);
        void UpdateSettings(uint gameId, uint gameSettings);
        void UpdateState(uint gameId, GameState gameState);
    }

    /// <summary>
    /// Manages game sessions
    /// </summary>
    public class GameManager : IGameManager
    {
        private readonly ConcurrentDictionary<uint, Game> _games =
            new ConcurrentDictionary<uint, Game>();

        private int _currentGameCount = 1;

        public Game CreateGame(string gameName, uint gameSettings, Dictionary<string, string> gameAttributes)
        {
            var id = (uint)Interlocked.Increment(ref _currentGameCount); //generate a game id
            var game = new Game
            {
                GameId = id,
                Name = gameName,
                State = GameState.Init,
                Settings = gameSettings,
                Attributes = gameAttributes
            };
            _games.TryAdd(id, game);

            return game;
        }

        public void UpdateAttributes(uint gameId, Dictionary<string, string> gameAttributes)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                game.Attributes = gameAttributes;
            }

            throw new ArgumentException($"Cannot update attributes, GameId:{gameId} does not exist");
        }

        public void UpdateSettings(uint gameId, uint gameSettings)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                game.Settings = gameSettings;
            }

            throw new ArgumentException($"Cannot update settings, GameId:{gameId} does not exist");
        }

        public void UpdateState(uint gameId, GameState gameState)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                game.State = gameState;
            }

            throw new ArgumentException($"Cannot update state, GameId:{gameId} does not exist");
        }
    }

    public class Game
    {
        public uint GameId { get; set; }
        public string Name { get; set; }
        public GameState State { get; set; }
        public uint Settings { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }

    public class Player
    {

    }
}
