using System;
using System.Collections.Generic;
using static TrophyParser.Enums;

namespace TrophyIsBetter.Interfaces
{
  internal interface IGameModel
  {
    string Icon { get; }
    string Name { get; }
    string NpCommID { get; }
    PlatformEnum Platform { get; }
    bool HasPlatinum { get; }
    bool IsSynced { get; }
    int EarnedCount { get; }
    int TrophyCount { get; }
    int EarnedExp { get; }
    int TotalExp { get; }
    DateTime? LastTimestamp { get; }
    List<ITrophyModel> Trophies { get; }
    string Path { get; }
    bool HasUnsavedChanges { get; }

    /// <summary>
    /// Unlock a trophy in the game
    /// </summary>
    void UnlockTrophy(ITrophyModel trophy, DateTime timestamp);

    /// <summary>
    /// Lock a trophy in the game
    /// </summary>
    void LockTrophy(ITrophyModel trophy);

    /// <summary>
    /// Save the game data to file
    /// </summary>
    void Save();

    /// <summary>
    /// Encrypt and export game data
    /// </summary>
    void Export(string exportPath);

    /// <summary>
    /// Reload the data from file
    /// </summary>
    void Reload();

  } // IGameModel
} // TrophyIsBetter.Interfaces
