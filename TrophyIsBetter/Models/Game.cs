using System;
using System.Collections.Generic;
using System.IO;
using TrophyIsBetter.Interfaces;
using TrophyParser;
using TrophyParser.PS3;
using TrophyParser.Vita;
using static TrophyParser.Enums;

namespace TrophyIsBetter.Models
{
  public class Game : IGameModel
  {
    #region Private Members

    private string _path;
    private TrophyList _trophyList;
    private bool _hasChanges = false;

    #endregion Private Members
    #region Constructors

    public Game(string path)
    {
      _path = path;

      if (IsPS3())
      {
        _trophyList = new PS3TrophyList(_path);
      }
      else
      {
        _trophyList = new VitaTrophyList(_path);
      }
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public string Icon => _trophyList.Icon;
    public string Name => _trophyList.Name;
    public string NpCommID => _trophyList.NpCommID;
    public PlatformEnum Platform => _trophyList.Platform;
    public bool HasPlatinum => _trophyList.HasPlatinum;
    public bool IsSynced => _trophyList.IsSynced;
    public string Progress => _trophyList.Progress;
    public DateTime? LastTimestamp => _trophyList.LastTimestamp;
    public DateTime? SyncTime => _trophyList.LastSyncedTimestamp;
    public List<Trophy> Trophies => ConvertTrophyData(_trophyList.Trophies);
    public string Path => _path;
    public bool HasUnsavedChanges { get => _hasChanges; set => _hasChanges = value; }

    #endregion Public Properties
    #region Public Methods

    public void UnlockTrophy(Trophy trophy, DateTime timestamp)
    {
      _trophyList.UnlockTrophy(trophy.ID, timestamp);
      HasUnsavedChanges = true;
    } // UnlockTrophy

    public void ChangeTimestamp(Trophy trophy, DateTime timestamp)
    {
      _trophyList.ChangeTimestamp(trophy.ID, timestamp);
      HasUnsavedChanges = true;
    } // ChangeTimestamp

    public void LockTrophy(Trophy trophy)
    {
      _trophyList.LockTrophy(trophy.ID);
      HasUnsavedChanges = true;
    } // LockTrophy

    public void Save()
    {
      _trophyList.Save();
      HasUnsavedChanges = false;
    } // Save

    public void Reload()
    {
      if (IsPS3())
      {
        _trophyList = new PS3TrophyList(_path);
      }
      else
      {
        _trophyList = new VitaTrophyList(_path);
      }

      HasUnsavedChanges = false;
    } // Reload

    #endregion Public Methods
    #region Private Methods

    private bool IsPS3()
    {
      return File.Exists(System.IO.Path.Combine(Path, "TROPCONF.SFM"));
    } // IsPS3

    private List<Trophy> ConvertTrophyData(List<TrophyParser.Models.Trophy> trophies)
    {
      List<Trophy> result = new List<Trophy>();

      foreach (TrophyParser.Models.Trophy trophy in trophies)
      {
        Trophy convertedTrophy = new Trophy()
        {
          ID = trophy.ID,
          Icon = GetIconPath(trophy.ID),
          Name = trophy.Name,
          Description = trophy.Detail,
          Type = trophy.Rank,
          Hidden = trophy.Hidden == "yes",
          Group = trophy.Gid == 0 ? "BaseGame" : $"DLC{trophy.Gid}",
          Achieved = trophy.Timestamp?.IsEarned != false,
          Synced = trophy.Timestamp?.IsSynced != false,
          Timestamp =
            trophy.Timestamp?.Time != null ? (DateTime)trophy.Timestamp?.Time : DateTime.MinValue
        };

        result.Add(convertedTrophy);
      }

      return result;
    } // ConvertTrophyData

    private string GetIconPath(int id)
    {
      string result = null;

      if (IsPS3())
      {
        result = Path + @"\TROP" + string.Format("{0:000}", id) + ".PNG";
      }
      else
      {
        result = $@"{Path}\ICON0.PNG";
      }

      return result;
    } // GetIconPath

    #endregion Private Methods
  } // GameListEntry
} // TrophyIsBetter.Models
