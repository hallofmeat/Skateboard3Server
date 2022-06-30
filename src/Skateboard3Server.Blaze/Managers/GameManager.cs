using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NLog;
using Skateboard3Server.Blaze.Managers.Models;

namespace Skateboard3Server.Blaze.Managers;

public interface IGameManager
{
    Game CreateGame(ushort capacity);
    bool RemoveGame(uint gameId);
    void UpdateAttributes(uint gameId, Dictionary<string, string> gameAttributes);
    void UpdateSettings(uint gameId, uint gameSettings);
    void UpdateState(uint gameId, GameState gameState);
    Game? FindGame(Func<ICollection<Game>, Game?> func);
    void AddPlayer(uint gameId, Player player);
}

/// <summary>
/// Manages game sessions
/// </summary>
public class GameManager : IGameManager
{

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly ConcurrentDictionary<uint, Game> _games = new();

    private int _currentGameCount;

    public Game CreateGame(ushort capacity)
    {
        var id = (uint)Interlocked.Increment(ref _currentGameCount); //generate a game id
        var game = new Game
        {
            GameId = id,
            Uuid = Guid.NewGuid().ToString(),
            State = GameState.Init,
            Capacity = capacity,
            Players = new List<Player?>(new Player[capacity]) //create slots
        };

        if (!_games.TryAdd(id, game))
        {
            throw new Exception($"Could not add game GameId:{id}");
        }

        Logger.Info($"Created game: {id}");
        return game;
    }

    public bool RemoveGame(uint gameId)
    {
        return _games.TryRemove(gameId, out _);
    }

    public void UpdateAttributes(uint gameId, Dictionary<string, string> gameAttributes)
    {
        if (!_games.TryGetValue(gameId, out var game))
        {
            throw new ArgumentException($"Could not update attributes, GameId:{gameId} does not exist");
        }

        game.Attributes = gameAttributes;
    }

    public void UpdateSettings(uint gameId, uint gameSettings)
    {
        if (!_games.TryGetValue(gameId, out var game))
        {
            throw new ArgumentException($"Could not update settings, GameId:{gameId} does not exist");
        }

        game.Settings = gameSettings;

    }

    public void UpdateState(uint gameId, GameState gameState)
    {
        if (!_games.TryGetValue(gameId, out var game))
        {
            throw new ArgumentException($"Could not update state, GameId:{gameId} does not exist");
        }

        game.State = gameState;
    }

    public Game? FindGame(Func<ICollection<Game>, Game?> func)
    {
        //I think this is thread safe?
        return func.Invoke(_games.Values);
    }

    public void AddPlayer(uint gameId, Player newPlayer)
    {
        if (!_games.TryGetValue(gameId, out var game))
        {
            throw new ArgumentException($"Could not get players, GameId:{gameId} does not exist");
        }

        var foundSlot = false;
        for (byte i = 0; i < game.Players.Count; i++)
        {
            if (game.Players[i] == null)
            {
                newPlayer.SlotId = i;
                game.Players[i] = newPlayer;
                foundSlot = true;
                break;
            }
        }

        if (!foundSlot)
        {
            throw new Exception($"Could not add player:{newPlayer.UserId} to game:{gameId}");
        }
    }
}