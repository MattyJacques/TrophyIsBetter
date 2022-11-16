using System.Collections.Generic;
using TrophyIsBetter.Models;

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
    List<Game> LoadGames();

    /// <summary>
    /// Remove a game from data
    /// </summary>
    void RemoveGame(IGameModel game);

  } // IGameListModel
} // TrophyIsBetter.Interfaces
