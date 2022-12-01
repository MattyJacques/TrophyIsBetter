using System;
using System.Diagnostics;
using static TrophyParser.Enums;

namespace TrophyParser.Vita
{
  public class VitaTrophyList : TrophyList
  {
    #region Private Members

    private TROP _trop;
    private TRPTITLE _title;
    private TRPTRANS _trans;

    private string _path;

    #endregion Private Members
    #region Constructors

    public VitaTrophyList(string path)
    {
      _path = path;

      _trop = new TROP(path);
      _title = new TRPTITLE(path);
      _trans = new TRPTRANS(path, _trop.TrophyCount);

      for (int i = 0; i < _trop.TrophyCount; ++i)
      {
        var trophy = _trop[i];
        if (DateTime.Compare(_title[i].Time.Value, DateTime.MinValue) > 0)
        {
          trophy.Timestamp = _title[i];
        }
        _trophies.Add(trophy);
      }
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public override string Icon => $"{_path}\\ICON0.PNG";
    public override string Name => _trop.TitleName;
    public override string NpCommID => _trop.NpCommID;
    public override PlatformEnum Platform => PlatformEnum.Vita;
    public override bool HasPlatinum => _trop.HasPlatinum;
    public override bool IsSynced => _title.IsSynced;
    public override int TrophyCount => _trop.TrophyCount;
    public override int EarnedCount => _title.EarnedCount;
    public override DateTime? LastTimestamp => _title.LastTimestamp;

    #endregion Public Properties
    #region Public Methods

    public override void UnlockTrophy(int id, DateTime time)
    {
      Debug.WriteLine($"Unlocking {Name} (Vita) - {_trophies[id].Name} with timestamp: {time}");

      _trans.UnlockTrophy(id, _trophies[id].Rank, time);
      _title.UnlockTrophy(id, time);
    } // Unlock Trophy

    public override void ChangeTimestamp(int id, DateTime time)
    {
      _trans.ChangeTimestamp(id, time);
      _title.ChangeTimestamp(id, time);
    } // ChaneTimestamp

    public override void LockTrophy(int id)
    {
      Debug.WriteLine($"Locking {Name} (Vita) - {_trophies[id].Name}");

      _trans.LockTrophy(id);
      _title.LockTrophy(id);
    } // LockTrophy

    public override void Save()
    {
      Debug.WriteLine($"Saving {Name} (Vita)");

      _trans.Save();
      _title.Save();
    } // Save

    #endregion Public Methods
  } // VitaTrophyList
} // TrophyParser.Vita
