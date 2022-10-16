using System.Collections.Generic;
using TrophyIsBetter.Models;

namespace TrophyIsBetter.Interfaces
{
  public interface IGameListModel
  {
    /// <summary>
    /// Import trophy folders from the given directory
    /// </summary>
    void ImportGames(string pathToGames);

    /// <summary>
    /// Load all of the games in the application data directory
    /// </summary>
    List<GameListEntry> LoadGames();

    /// <summary>
    /// Close the files that have been opened
    /// </summary>
    void CloseFiles();

  } // IGameListModel
} // TrophyIsBetter.Interfaces
