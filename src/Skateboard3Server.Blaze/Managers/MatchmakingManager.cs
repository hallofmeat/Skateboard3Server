using Skateboard3Server.Blaze.Managers.Models;
using System.Linq;
using System.Threading;

namespace Skateboard3Server.Blaze.Managers;

public interface IMatchmakingManager
{
    uint CreateMatchmakingSession();
    void RemoveMatchmakingSession(uint matchmakingId);
    Game MatchGame();
}

/// <summary>
/// Manages matchmaking with people
/// </summary>
public class MatchmakingManager : IMatchmakingManager
{
    private readonly IGameManager _gameManager;

    private int _currentMatchmakingCount = 0;

    public MatchmakingManager(IGameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public uint CreateMatchmakingSession() //TODO: pass more stuff?
    {
        return (uint)Interlocked.Increment(ref _currentMatchmakingCount); //generate a matchmaking id
    }

    public void RemoveMatchmakingSession(uint matchmakingId)
    {
        //TODO do stuff
    }

    public Game MatchGame()
    {
        //TODO make smarter (like include player count to eliminate full games, game state etc)
        return _gameManager.FindGame(games => games.FirstOrDefault());
    }
}