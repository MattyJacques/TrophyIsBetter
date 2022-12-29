using System.Collections.Generic;

namespace TrophyIsBetter.Interfaces
{
  internal interface IGameListModel
  {
    /// <summary>
    /// Import trophy folders from the given directory
    /// </summary>
    void ImportGames(string pathToGames);

    /// <summary>
    /// Load all of the games in the application data directory
    /// </summary>
    List<IGameModel> LoadGames();

    /// <summary>
    /// Remove a game from data
    /// </summary>
    bool RemoveGame(IGameModel game);

  } // IGameListModel
} // TrophyIsBetter.Interfaces
