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
  internal class Game : IGameModel
  {
    #region Private Members

    private string _path;
    private TrophyList _trophyList;
    private bool _hasChanges = false;
    private int _earnedExp = 0;
    private int _totalExp = 0;

    #endregion Private Members
    #region Constructors

    internal Game(string path)
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

      UpdatePSNExp();
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public string Icon => _trophyList.Icon;
    public string Name => _trophyList.Name;
    public string NpCommID => _trophyList.NpCommID;
    public PlatformEnum Platform => _trophyList.Platform;
    public bool HasPlatinum => _trophyList.HasPlatinum;
    public bool IsSynced => _trophyList.IsSynced;
    public int EarnedCount => _trophyList.EarnedCount;
    public int TrophyCount => _trophyList.TrophyCount;
    public int EarnedExp { get => _earnedExp; set => _earnedExp = value; }
    public int TotalExp { get => _totalExp; set => _totalExp = value; }
    public DateTime? LastTimestamp => _trophyList.LastTimestamp;
    public DateTime? SyncTime => _trophyList.LastSyncedTimestamp;
    public List<ITrophyModel> Trophies => ConvertTrophyData(_trophyList.Trophies);
    public string Path => _path;
    public bool HasUnsavedChanges { get => _hasChanges; set => _hasChanges = value; }

    #endregion Public Properties
    #region Public Methods

    /// <summary>
    /// Unlock trophy
    /// </summary>
    public void UnlockTrophy(ITrophyModel trophy, DateTime timestamp)
    {
      if (trophy.Achieved)
      {
        _trophyList.ChangeTimestamp(trophy.ID, timestamp);
      }
      else
      {
        _trophyList.UnlockTrophy(trophy.ID, timestamp);
        trophy.Achieved = true;
      }

      trophy.Timestamp = timestamp;
      HasUnsavedChanges = true;
      UpdatePSNExp();
    } // UnlockTrophy

    public void LockTrophy(ITrophyModel trophy)
    {
      _trophyList.LockTrophy(trophy.ID);

      HasUnsavedChanges = true;
      UpdatePSNExp();
    } // LockTrophy

    public void Save()
    {
      _trophyList.Save();
      HasUnsavedChanges = false;
    } // Save

    public void Export(string exportPath)
    {
      string fullExportPath = System.IO.Path.Combine(exportPath, System.IO.Path.GetFileName(Path));

      Utility.File.CopyDirectory(Path, fullExportPath, false);
      Utility.PfdTool.EncryptTrophyData(fullExportPath);
    } // Export

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

    private List<ITrophyModel> ConvertTrophyData(List<TrophyParser.Models.Trophy> trophies)
    {
      List<ITrophyModel> result = new List<ITrophyModel>();

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
          Group = trophy.Gid == 0 ? "Base Game" : $"DLC {trophy.Gid}",
          Achieved = trophy.Timestamp?.IsEarned == true,
          Synced = trophy.Timestamp?.IsSynced == true || trophy.Timestamp == null,
          Timestamp = trophy.Timestamp?.Time
        };

        result.Add(convertedTrophy);
      }

      return result;
    } // ConvertTrophyData

    private string GetIconPath(int id)
    {
      string result;
      
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

    private void UpdatePSNExp()
    {
      EarnedExp = 0;
      TotalExp = 0;

      foreach (ITrophyModel trophy in Trophies)
      {
        switch (trophy.Type)
        {
          case 'P':
            TotalExp += 180;
            EarnedExp += trophy.Achieved ? 180 : 0;
            break;
          case 'G':
            TotalExp += 90;
            EarnedExp += trophy.Achieved ? 90 : 0;
            break;
          case 'S':
            TotalExp += 30;
            EarnedExp += trophy.Achieved ? 30 : 0;
            break;
          case 'B':
            TotalExp += 15;
            EarnedExp += trophy.Achieved ? 15 : 0;
            break;
          default:
            throw new NotSupportedException($"Unknown trophy grade: {trophy.Type}");
        }
      }
    } // UpdatePSNExp

    #endregion Private Methods
  } // GameListEntry
} // TrophyIsBetter.Models
