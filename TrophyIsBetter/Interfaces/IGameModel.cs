using System;
using TrophyIsBetter.Models;

namespace TrophyIsBetter.Interfaces
{
  public interface IGameModel
  {
    /// <summary>
    /// Unlock a trophy in the game
    /// </summary>
    void UnlockTrophy(Trophy trophy, DateTime timestamp);

    /// <summary>
    /// Change the timestamp of a trophy in the game
    /// </summary>
    void ChangeTimestamp(Trophy trophy, DateTime timestamp);

    /// <summary>
    /// Lock a trophy in the game
    /// </summary>
    void LockTrophy(Trophy trophy);

    /// <summary>
    /// Save the game data to file
    /// </summary>
    void Save();

    /// <summary>
    /// Reload the data from file
    /// </summary>
    void Reload();

  } // IGameModel
} // TrophyIsBetter.Interfaces
