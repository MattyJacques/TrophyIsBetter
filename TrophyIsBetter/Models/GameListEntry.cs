using System;
using System.IO;
using TrophyIsBetter.Interfaces;
using TrophyParser;
using TrophyParser.PS3;
using TrophyParser.Vita;

namespace TrophyIsBetter.Models
{
  public class GameListEntry : IGameListEntryModel
  {
    #region Private Members

    private string _path;
    private readonly TrophyList _trophyList;

    #endregion Private Members
    #region Constructors

    public GameListEntry(string path)
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
    } // GameListEntry

    #endregion Constructors
    #region Public Properties

    public string Icon => _trophyList.Icon;
    public string Name => _trophyList.Name;
    public string NpCommID => _trophyList.NpCommID;
    public string Platform => _trophyList.Platform;
    public bool HasPlatinum => _trophyList.HasPlatinum;
    public bool IsSynced => _trophyList.IsSynced;
    public string Progress => _trophyList.Progress;
    public DateTime? LastTimestamp => _trophyList.LastTimestamp;
    public DateTime? SyncTime => _trophyList.LastSyncedTimestamp;
    public string Path => _path;

    #endregion Public Properties
    #region Private Methods

    private string GetPlatform()
    {
      if (IsPS3())
      {
        return "PS3";
      }
      else
      {
        return "Vita";
      }
    } // GetPlatform

    private bool IsPS3()
    {
      return File.Exists(System.IO.Path.Combine(Path, "TROPCONF.SFM"));
    } // IsPS3

    #endregion
  } // GameListEntry
} // TrophyIsBetter.Models
