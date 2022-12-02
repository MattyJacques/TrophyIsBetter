using System;
using TrophyParser.Models;
using static TrophyParser.Enums;
using static TrophyParser.PS3.Structs;

namespace TrophyParser.PS4
{
  public class PS4TrophyList : TrophyList
  {
    #region Private Members

    private string _dbPath;
    private string _name;
    private string _npCommID;
    private bool _hasPlatinum;
    private bool _isSynced;
    private int _trophyCount;
    private int _earnedCount;

    #endregion Private Members
    #region Constructors

    public PS4TrophyList(string dbPath,
                         string npCommID,
                         string name,
                         bool hasPlatinum,
                         bool isSynced,
                         int trophyCount,
                         int earnedCount,
                         DateTime lastTimestamp)
    {
      _dbPath = dbPath;
      _npCommID = npCommID;
      _name = name;
      _hasPlatinum = hasPlatinum;
      _isSynced = isSynced;
      _trophyCount = trophyCount;
      _earnedCount = earnedCount;

      LoadData();
    } // Constructor

    #endregion Constructors
    #region Public Properties
    public override string Icon => throw new NotSupportedException();
    public override string Name => _name;
    public override string NpCommID => _npCommID;
    public override PlatformEnum Platform => PlatformEnum.PS4;
    public override bool HasPlatinum => _hasPlatinum;
    public override bool IsSynced => _isSynced;
    public override int TrophyCount => _trophyCount;
    public override int EarnedCount => _earnedCount;
    public override DateTime? LastTimestamp
    {
      get
      {
        DateTime result = DateTime.MinValue;
        foreach (Trophy trophy in _trophies)
        {
          if (trophy.Timestamp != null && trophy.Timestamp.Time.HasValue
           && DateTime.Compare(trophy.Timestamp.Time.Value, result) > 0)
          {
            result = trophy.Timestamp.Time.Value;
          }
        }
        return result;
      }
    }

    #endregion Public Properties
    #region Public Methods

    public override void ChangeTimestamp(int id, DateTime time)
    {
      Random rng = new Random();
      DateTime timeUnlockedUc = time.AddMilliseconds(rng.Next(1, 60));
      DateTime timeLastUnlocked = LastTimestamp.Value;
      DateTime timeLastUpdateUc = timeLastUnlocked.AddMilliseconds(rng.Next(1, 100));

      PS4TrophyDB db = new PS4TrophyDB(_dbPath);
      db.ChangeTimestamp(NpCommID, id, timeUnlockedUc, timeLastUnlocked, timeLastUpdateUc);
    } // ChangeTimestamp

    public override void LockTrophy(int id) => throw new NotSupportedException();

    public override void Save() => throw new NotSupportedException();

    public override void UnlockTrophy(int id, DateTime time) => throw new NotSupportedException();

    #endregion Public Methods
    #region Private Methods

    private void LoadData()
    {
      PS4TrophyDB db = new PS4TrophyDB(_dbPath);
      _trophies = db.GetTrophies(NpCommID);
    } // LoadData

    #endregion
  } // PS4TrophyList
} // TrophyParser.PS4
