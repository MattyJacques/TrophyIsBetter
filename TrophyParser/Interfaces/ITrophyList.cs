using static TrophyParser.Structs;
using System;
using System.Collections.Generic;
using TrophyParser.Models;
using static TrophyParser.Enums;

namespace TrophyParser.Interfaces
{
  public interface ITrophyList
  {
    string Icon { get; }
    string Name { get; }
    string NpCommID { get; }
    PlatformEnum Platform { get; }
    bool HasPlatinum { get; }
    bool IsSynced { get; }
    int TrophyCount { get; }
    int EarnedCount { get; }
    string Progress { get; }
    DateTime? LastTimestamp { get; }
    DateTime? LastSyncedTimestamp { get; }
    List<Trophy> Trophies { get; }

    void UnlockTrophy(int id, DateTime time);
    void UnlockTrophy(Trophy trophy, DateTime time);
    void LockTrophy(int id);
    void LockTrophy(Trophy trophy);
    void Save();
  } // ITrophyList
} // TrophyParser.Interfaces
