using System;

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
        trophy = _trop[i];
        trophy.Timestamp = _title[i];
        _trophies.Add(trophy);
      }
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public override string Icon => $"{_path}\\ICON0.PNG";
    public override string Name => _trop.TitleName;
    public override string NpCommID => _trop.NpCommID;
    public override string Platform => "PS Vita";
    public override bool HasPlatinum => _trop.HasPlatinum;
    public override bool IsSynced => _trans.IsSynced;
    public override int TrophyCount => _trop.TrophyCount;
    public override int EarnedCount => _title.EarnedCount;
    public override string Progress => $"{EarnedCount}/{TrophyCount}";
    public override DateTime? LastTimestamp => _title.LastTimestamp;
    public override DateTime? LastSyncedTimestamp => _title.LastSyncedTimestamp;

    #endregion Public Properties
    #region Public Methods

    public override void UnlockTrophy(int id, DateTime time)
    {
      throw new NotImplementedException();
    } // Unlock Trophy

    public override void LockTrophy(int id)
    {
      throw new NotImplementedException();
    } // LockTrophy

    public override void Save()
    {
      throw new NotImplementedException();
    } // Save

    #endregion Public Methods
  } // VitaTrophyList
} // TrophyParser.Vita
