using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using static TrophyParser.PS3.Structs;

namespace TrophyParser.PS3
{
  public class TROPUSR
  {
    #region Const Members

    private const string TROPUSR_FILE_NAME = "TROPUSR.DAT";

    #endregion Const Members
    #region Private Members

    private string _accountID;
    private string _listID;
    private int _trophyCount;
    private ListInfo _listInfo;
    private List<TrophyType> _types = new List<TrophyType>();
    private List<Timestamp> _timestamps = new List<Timestamp>();
    private Header _header;
    private Dictionary<int, TypeRecord> _typeRecords;
    private AchievedStats _achievedStats;
    private uint[] _completionRate = new uint[4];
    byte[] _unknownHash;

    #endregion Private Members
    #region Constructors

    public TROPUSR(string path)
    {
      string filePath = Utility.File.GetFullPath(path, TROPUSR_FILE_NAME);

      using (var fileStream = new FileStream(filePath, FileMode.Open))
      using (var TROPUSRReader = new BigEndianBinaryReader(fileStream))
      {
        _header = DataParsing.ParseHeader(filePath, TROPUSRReader);
        _typeRecords = DataParsing.ParseTypeRecords(_header, TROPUSRReader);

        do
        {
          // 1 unknown 2 account_id 3 trophy_id and hash(?) 4 trophy info
          // 
          int type = TROPUSRReader.ReadInt32();
          int blocksize = TROPUSRReader.ReadInt32();
          int id = TROPUSRReader.ReadInt32(); // if have more than same type block, it will be used
          int unknown = TROPUSRReader.ReadInt32();
          byte[] blockdata = TROPUSRReader.ReadBytes(blocksize);
          switch (type)
          {
            case 1: // Unknown
              break;
            case 2:
              _accountID = Encoding.UTF8.GetString(blockdata, 16, 16);
              break;
            case 3:
              _listID = Encoding.UTF8.GetString(blockdata, 0, 16).Trim('\0');
              short u1 = BitConverter.ToInt16(blockdata, 16).ChangeEndian();
              short u2 = BitConverter.ToInt16(blockdata, 18).ChangeEndian();
              short u3 = BitConverter.ToInt16(blockdata, 20).ChangeEndian();
              short u4 = BitConverter.ToInt16(blockdata, 22).ChangeEndian();
              _trophyCount = BitConverter.ToInt32(blockdata, 24).ChangeEndian();
              int u5 = BitConverter.ToInt32(blockdata, 28).ChangeEndian();
              _completionRate[0] = BitConverter.ToUInt32(blockdata, 64);
              _completionRate[1] = BitConverter.ToUInt32(blockdata, 68);
              _completionRate[2] = BitConverter.ToUInt32(blockdata, 72);
              _completionRate[3] = BitConverter.ToUInt32(blockdata, 76);
              break;
            case 4:
              _types.Add(blockdata.ToStruct<TrophyType>());
              break;
            case 5:
              _listInfo = blockdata.ToStruct<ListInfo>();
              break;
            case 6:
              _timestamps.Add(blockdata.ToStruct<Timestamp>());
              break;
            case 7:
              _achievedStats = blockdata.ToStruct<AchievedStats>();
              break;
            case 8: // Unknown Hash
              _unknownHash = blockdata.SubArray(0, 20);
              break;
            case 9: // Unknown, "some number of platinum trophy"
              break;
            case 10: // Padding
              break;
          }

        } while (TROPUSRReader.BaseStream.Position < TROPUSRReader.BaseStream.Length);

        _listInfo.LastUpdated = DateTime.Now;
      }
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public List<TrophyType> TrophyTypes { get => _types; }
    public int EarnedCount { get { return _listInfo.AchievedCount; } }
    public DateTime LastTimestamp { get { return _listInfo.LastAchievedTrophyTime; } }
    public DateTime LastSyncedTimestamp
    {
      get
      {
        DateTime result = new DateTime(2008, 1, 1);
        foreach (Timestamp timestamp in _timestamps)
        {
          if (timestamp.IsSync && DateTime.Compare(timestamp.Time, result) > 0)
          {
            result = timestamp.Time;
          }
        }
        return result;
      }
    } // LastSyncedTimestamp

    #endregion Public Properties
    #region Public Methods

    public void UnlockTrophy(int id, DateTime unlockTime)
    {
      Timestamp timestamp = _timestamps[id];
      timestamp.Time = unlockTime;
      if (!timestamp.IsEarned)
      {
        _listInfo.AchievedCount++;
        _achievedStats.EarnedCount++;
      }

      _listInfo.AchievementRate[id / 32] |= (uint)(1 << id).ChangeEndian();
      _completionRate[id / 32] |= (uint)(1 << id);
      timestamp.IsEarned = true;
      timestamp.SyncState = (int)TropSyncState.NotSync;
      _timestamps[id] = timestamp;
      if (unlockTime > _listInfo.LastAchievedTrophyTime)
      {
        _listInfo.LastAchievedTrophyTime = unlockTime;
        _listInfo.LastUpdated = unlockTime;
      }

      Debug.WriteLine($"Unlocked trophy {id} in TROPUSR");
    } // UnlockTrophy

    public void PrintState()
    { // Print all data within TROPUSR

      Console.WriteLine("\n----- TROPUSR Data -----");
      Console.WriteLine("Account ID: {0}", _accountID);
      _header.Output();

      Console.WriteLine("\nType Records");
      foreach (KeyValuePair<int, TypeRecord> record in _typeRecords)
      {
        Console.WriteLine(record.Value);
      }

      Console.WriteLine("\nList Info");
      Console.WriteLine("Game Added To Account On: {0}", _listInfo.DateAdded);
      Console.WriteLine("Last Trophy Earned On: {0}", _listInfo.LastAchievedTrophyTime);
      Console.WriteLine("List Last Updated: {0}", _listInfo.LastUpdated);
      Console.WriteLine("Trophies Earned: {0}", _listInfo.AchievedCount);
      Console.WriteLine("Completion Rate: {0}", _listInfo.AchievementRate[0]);

      Console.WriteLine("\nAchieved Data");
      for (int i = 0; i < _types.Count; i++)
      {
        Console.WriteLine("ID: {0}, Type: {1}, Earned: {2}, Timestamp: {3}", _types[i].ID,
            _types[i].Type, _timestamps[i].IsEarned, _timestamps[i].Time);
      }
    } // PrintState

    #endregion Public Methods
    #region Private Methods

    #endregion Private Methods
  } // TROPUSR
} // TrophyParser
