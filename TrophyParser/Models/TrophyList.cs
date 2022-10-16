using System;
using System.Collections.Generic;
using TrophyParser.Interfaces;
using static TrophyParser.Structs;

namespace TrophyParser
{
  public abstract class TrophyList : ITrophyList
  {
    #region Protected Members

    protected List<Trophy> _trophies = new List<Trophy>();

    #endregion
    #region Public Properties

    public abstract string Icon { get; }
    public abstract string Name { get; }
    public abstract string NpCommID { get; }
    public abstract string Platform { get; }
    public abstract bool HasPlatinum { get; }
    public abstract bool IsSynced { get; }
    public abstract int TrophyCount { get; }
    public abstract int EarnedCount { get; }
    public abstract string Progress { get; }
    public abstract DateTime? LastTimestamp { get; }
    public abstract DateTime? LastSyncedTimestamp { get; }

    #endregion Public Properties
  } // TrophyList
} // TrophyParser
