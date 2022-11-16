using System;
using System.Collections.Generic;
using TrophyParser.Models;
using static TrophyParser.Enums;

namespace TrophyParser.Interfaces
{
  internal interface ITrophyList
  {
    string Icon { get; }
    string Name { get; }
    string NpCommID { get; }
    PlatformEnum Platform { get; }
    bool HasPlatinum { get; }
    bool IsSynced { get; }
    int TrophyCount { get; }
    int EarnedCount { get; }
    DateTime? LastTimestamp { get; }
    DateTime? LastSyncedTimestamp { get; }
    List<Trophy> Trophies { get; }

    void UnlockTrophy(int id, DateTime time);
    void ChangeTimestamp(int id, DateTime time);
    void LockTrophy(int id);
    void Save();
  } // ITrophyList
} // TrophyParser.Interfaces
