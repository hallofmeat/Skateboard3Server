using System.Collections.Concurrent;

namespace Skateboard3Server.Blaze.Managers
{
    public interface IGameManager
    {
    }

    /// <summary>
    /// Manages game sessions
    /// </summary>
    public class GameManager : IGameManager
    {
        private readonly ConcurrentDictionary<uint, GameData> _games =
            new ConcurrentDictionary<uint, GameData>();

        private int _currentGameCount = 1;

    }

    public class GameData
    {
    }
}
