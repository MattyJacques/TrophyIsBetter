using System;
using System.Collections.Generic;
using TrophyIsBetter.Interfaces;
using TrophyParser;
using TrophyParser.PS4;
using static TrophyParser.Enums;

namespace TrophyIsBetter.Models
{
  internal class PS4Game : IGameModel
  {
    #region Private Members

    private TrophyList _trophyList;
    private bool _hasChanges = false;
    private int _earnedExp = 0;
    private int _totalExp = 0;
    private List<ITrophyModel> _trophies = null;

    #endregion Private Members
    #region Constructors

    public PS4Game(PS4TrophyList trophyList)
    {
      _trophyList = trophyList;
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public string Icon => throw new NotSupportedException();
    public string Name { get => _trophyList.Name; set => _trophyList.Name = value; }
    public string NpCommID => _trophyList.NpCommID;
    public PlatformEnum Platform => PlatformEnum.PS4;
    public bool HasPlatinum => _trophyList.HasPlatinum;
    public bool IsSynced => _trophyList.IsSynced;
    public int EarnedCount => _trophyList.EarnedCount;
    public int TrophyCount => _trophyList.TrophyCount;
    public int EarnedExp { get => _earnedExp; set => _earnedExp = value; }
    public int TotalExp { get => _totalExp; set => _totalExp = value; }
    public DateTime? LastTimestamp => _trophyList.LastTimestamp;
    public List<ITrophyModel> Trophies { get => _trophies ?? ConvertTrophyData(_trophyList.Trophies); set => _trophies = value; } 
    public string Path => throw new NotSupportedException();
    public bool HasUnsavedChanges { get => _hasChanges; set => _hasChanges = value; }

    #endregion Public Properties
    #region Public Methods

    public void Export(string exportPath) => throw new NotSupportedException();

    public void LockTrophy(ITrophyModel trophy) => throw new NotSupportedException();

    public void Reload() => throw new NotSupportedException();

    public void Save() => throw new NotSupportedException();

    public void UnlockTrophy(ITrophyModel trophy, DateTime timestamp)
    {
      _trophyList.ChangeTimestamp(trophy.ID, timestamp);
    }

    #endregion Public Methods
    #region Private Methods

    private List<ITrophyModel> ConvertTrophyData(List<TrophyParser.Models.Trophy> trophies)
    {
      List<ITrophyModel> result = new List<ITrophyModel>();

      foreach (TrophyParser.Models.Trophy trophy in trophies)
      {
        Trophy convertedTrophy = new Trophy()
        {
          Game = Name,
          ID = trophy.ID,
          Icon = "",
          Name = trophy.Name,
          Description = trophy.Detail,
          Type = trophy.Rank,
          Hidden = trophy.Hidden == "yes",
          Group = trophy.Gid == 0 ? "Base Game" : $"DLC {trophy.Gid}",
          Achieved = trophy.Timestamp?.IsEarned == true,
          Synced = trophy.Timestamp?.IsSynced == true || trophy.Timestamp == null,
          Timestamp = trophy.Timestamp?.Time
        };

        result.Add(convertedTrophy);
      }

      return result;
    } // ConvertTrophyData

    #endregion Private Methods
  } // PS4Game
} // TrophyIsBetter.Models
