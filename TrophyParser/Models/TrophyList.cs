﻿using System;
using System.Collections.Generic;
using TrophyParser.Interfaces;
using TrophyParser.Models;

namespace TrophyParser
{
  public abstract class TrophyList : ITrophyList
  {
    #region Protected Members

    protected List<Trophy> _trophies = new List<Trophy>();

    #endregion
    #region Public Abstract Properties

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

    #endregion Public Abstract Properties
    #region Public Properties

    public List<Trophy> Trophies { get => _trophies; }

    #endregion Public Properties
    #region Public Abstract Methods

    public abstract void UnlockTrophy(int id, DateTime time);
    public abstract void LockTrophy(int id);
    public abstract void Save();

    #endregion Public Abstract Methods
    #region Public Methods

    public void UnlockTrophy(Trophy trophy, DateTime time) => UnlockTrophy(trophy.ID, time);
    public void LockTrophy(Trophy trophy) => LockTrophy(trophy.ID);

    #endregion Public Methods
  } // TrophyList
} // TrophyParser