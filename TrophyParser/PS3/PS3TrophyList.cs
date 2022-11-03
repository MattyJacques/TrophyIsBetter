using System;
using System.Diagnostics;
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
        var trophyInfo = _usr[i];
        if (trophyInfo.HasValue && trophyInfo.Value.IsEarned)
        {
          trophy.Timestamp = new Timestamp { Time = trophyInfo.Value.Time, IsSynced = trophyInfo.Value.IsSynced };
        }
        else
        {
          trophy.Timestamp = new Timestamp { Time = null, IsSynced = false };
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
    #region Public Methods

    public override void UnlockTrophy(int id, DateTime time)
    {
      Debug.WriteLine($"Unlocking {Name} (PS3) - {_trophies[id].Name} with timestamp: {time}");

      _usr.UnlockTrophy(id, time);
      _trns.AddTrophy(id, _usr.TrophyTypes[id].Type, time);
      _trophies[id].Timestamp = new Timestamp { Time = time, IsSynced = false };
    } // UnlockTrophy

    public override void LockTrophy(int id)
    {
      Debug.WriteLine($"Locking {Name} (PS3) - {_trophies[id].Name}");

      _usr.LockTrophy(id);
      _trns.RemoveTrophy(id);
      _trophies[id].Timestamp = new Timestamp { Time = null, IsSynced = false }; ;
    } // LockTrophy

    public override void Save()
    {
      Debug.WriteLine($"Saving {Name} (PS3)");

      _usr.Save();
      _trns.Save();
    } // Save

    #endregion Public Methods
  } // PS3TrophyList
} // TrophyParser.PS3
