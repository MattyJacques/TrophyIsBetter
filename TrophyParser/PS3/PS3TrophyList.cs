using System;
using static TrophyParser.Structs;

namespace TrophyParser.PS3
{
  public class PS3TrophyList : TrophyList
  {
    #region Private Members

    private SFM _conf;
    private TROPTRNS _trns;
    private TROPUSR _usr;

    private string _path;

    #endregion Private Members
    #region Constructors

    public PS3TrophyList(string path)
    {
      _path = path;

      _conf = new TROPCONF(path);
      _trns = new TROPTRNS(path);
      _usr = new TROPUSR(path);

      for (int i = 0; i < TrophyCount; ++i)
      {
        var trophy = _conf[i];
        var trophyInfo = _trns[i];
        if (trophyInfo.HasValue)
        {
          trophy.Timestamp = new Timestamp { Time = trophyInfo.Value.Time, Synced = trophyInfo.Value.IsSynced };
        }
        else
        {
          trophy.Timestamp = new Timestamp { Time = null, Synced = false };
        }
        _trophies.Add(trophy);
      }
    } // PS3TrophyList

    #endregion Constructors
    #region Public Properties

    public override string Icon => $"{_path}\\ICON0.PNG";
    public override string Name => _conf.TitleName;
    public override string NpCommID => _conf.NpCommID;
    public override string Platform => "PS3";
    public override bool HasPlatinum => _conf.HasPlatinum;
    public override bool IsSynced => _trns.IsSynced;
    public override int TrophyCount => _conf.TrophyCount;
    public override int EarnedCount => _usr.EarnedCount;
    public override string Progress => $"{EarnedCount}/{TrophyCount}";
    public override DateTime? LastTimestamp => _usr.LastTimestamp;
    public override DateTime? LastSyncedTimestamp => _usr.LastSyncedTimestamp;

    #endregion Public Properties
  } // PS3TrophyList
} // TrophyParser.PS3
