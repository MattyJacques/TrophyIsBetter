using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static TrophyParser.PS3.Structs;

namespace TrophyParser.PS3
{
  internal class TROPUSR
  {
    #region Const Members

    private const string TROPUSR_FILE_NAME = "TROPUSR.DAT";

    #endregion Const Members
    #region Private Members

    private string _filePath;
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

    internal TROPUSR(string path)
    {
      _filePath = Utility.File.GetFullPath(path, TROPUSR_FILE_NAME);

      BigEndianBinaryReader reader =
        new BigEndianBinaryReader(new FileStream(_filePath, FileMode.Open));

      _header = DataParsing.ParseHeader(_filePath, reader);
      _typeRecords = DataParsing.ParseTypeRecords(_header, reader);

      ParseBlocks(reader);

      // There was a bug that made the list info go out of sync. Call fix here
      // This can be removed in later versions
      reader.Close();
      reader =
        new BigEndianBinaryReader(new FileStream(_filePath, FileMode.Open));
      ResetListInfo();
      reader.Close();
      Save();
      reader =
        new BigEndianBinaryReader(new FileStream(_filePath, FileMode.Open));
      ParseBlocks(reader);
    } // Constructor

    #endregion Constructors
    #region Internal Properties

    internal List<TrophyType> TrophyTypes { get => _types; }
    internal int EarnedCount { get { return _listInfo.AchievedCount; } }
    internal DateTime? LastTimestamp
    {
      get
      {
        DateTime? timestamp = null;

        if (_listInfo.LastAchievedTrophyTime != DateTime.MinValue)
        {
          timestamp = _listInfo.LastAchievedTrophyTime;
        }

        return timestamp;
      }
    }

    internal DateTime LastSyncedTimestamp
    {
      get
      {
        DateTime result = new DateTime(2008, 1, 1);
        foreach (Timestamp timestamp in _timestamps)
        {
          if (timestamp.IsSynced && DateTime.Compare(timestamp.Time, result) > 0)
          {
            result = timestamp.Time;
          }
        }
        return result;
      }
    } // LastSyncedTimestamp

    #endregion Internal Properties
    #region Internal Methods

    internal Timestamp? this[int trophyID]
    {
      get
      {
        Timestamp? ret = null;
        foreach (Timestamp timestamp in _timestamps)
        {
          if (timestamp.ID == trophyID)
          {
            ret = timestamp;
            break;
          }
        }

        return ret;
      }
    } // []

    internal void UnlockTrophy(int id, DateTime unlockTime)
    {
      UpdateTimestamp(id, unlockTime);
      UpdateRates(id);
      UpdateListInfo(unlockTime);

      Debug.WriteLine($"Unlocked trophy {id} in TROPUSR");
    } // UnlockTrophy

    internal void ChangeTimestamp(int id, DateTime time)
    {
      CheckListInfo(_timestamps[id].Time);
      UpdateTimestamp(id, time);
      UpdateListInfo(time);
    } // ChangeTimestamp

    internal void LockTrophy(int id)
    {
      Timestamp timestamp = _timestamps[id];
      
      if (timestamp.SyncState == (int)TrophySyncState.Synced)
        throw new AlreadySyncedException(timestamp.ID);
      
      if (!timestamp.IsEarned)
        throw new AlreadyLockedException(timestamp.ID);

      DateTime removedTimestamp = timestamp.Time;

      ResetTimestamp(timestamp);
      RemoveFromRates(id);
      CheckListInfo(removedTimestamp);

      Debug.WriteLine($"Locked trophy {id} in TROPUSR");
    } // LockTrophy

    internal void Save()
    {
      BigEndianBinaryWriter writer =
        new BigEndianBinaryWriter(new FileStream(_filePath, FileMode.Open));

      writer.Write(_header.StructToBytes());
      SaveAccountInfo(writer);
      SaveListID(writer);
      SaveTrophyInfo(writer);
      SaveListInfo(writer);
      SaveTimestamps(writer);
      SaveAchievedInfo(writer);

      writer.Flush();
      writer.Close();

      Debug.WriteLine($"Saved {_listID} TROPUSR");
    } // Save()

    internal void PrintState()
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

    #endregion Internal Methods
    #region Private Methods
    #region File Parsing

    private void ParseBlocks(BigEndianBinaryReader reader)
    {
      do
      {
        int type = reader.ReadInt32();
        int blocksize = reader.ReadInt32();
        int id = reader.ReadInt32(); // if have more than same type block, it will be used
        int unknown = reader.ReadInt32();
        byte[] blockdata = reader.ReadBytes(blocksize);
        switch (type)
        {
          case 1: // Unknown
            break;
          case 2: // Account ID
            _accountID = Encoding.UTF8.GetString(blockdata, 16, 16);
            break;
          case 3: // List Definition
            ParseListDefinition(blockdata);
            break;
          case 4: // Trophy Info
            _types.Add(blockdata.ToStruct<TrophyType>());
            break;
          case 5: // List Info
            _listInfo = blockdata.ToStruct<ListInfo>();
            break;
          case 6: // Timestamps
            _timestamps.Add(blockdata.ToStruct<Timestamp>());
            break;
          case 7: // Achieved Stats
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

      } while (reader.BaseStream.Position < reader.BaseStream.Length);
    } // ParseBlocks

    private void ParseListDefinition(byte[] blockdata)
    {
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
    } // ParseListDefinition

    #endregion File Parsing
    #region Data Editing

    private void UpdateTimestamp(int id, DateTime unlockTime)
    {
      Timestamp timestamp = _timestamps[id];
      timestamp.Time = unlockTime;

      if (!timestamp.IsEarned)
      {
        _listInfo.AchievedCount++;
        _achievedStats.EarnedCount++;
        timestamp.IsEarned = true;
      }

      timestamp.SyncState = (int)TrophySyncState.NotSynced;
    } // UpdateTimestamp

    private void ResetTimestamp(Timestamp timestamp)
    {
      timestamp.Time = new DateTime(0);

      if (timestamp.IsEarned)
      {
        _listInfo.AchievedCount--;
        _achievedStats.EarnedCount--;
        timestamp.IsEarned = false;
      }
      
      timestamp.SyncState = 0;
    } // ResetTimestamp

    private void UpdateRates(int id)
    {
      _listInfo.AchievementRate[id / 32] |= (uint)(1 << id).ChangeEndian();
      _completionRate[id / 32] |= (uint)(1 << id);
    } // UpdateRates

    private void RemoveFromRates(int id)
    {
      _listInfo.AchievementRate[id / 32] &= 0xFFFFFFFF ^ (uint)(1 << id).ChangeEndian();
      _completionRate[id / 32] &= 0xFFFFFFFF ^ (uint)(1 << id);
    } // RemoveFromRates

    private void UpdateListInfo(DateTime timestamp)
    {
      if (timestamp > _listInfo.LastAchievedTrophyTime)
      {
        _listInfo.LastAchievedTrophyTime = timestamp;
        _listInfo.LastUpdated = timestamp;
      }

      if (_listInfo.DateAdded > timestamp)
      {
        _listInfo.DateAdded = timestamp.AddHours(-1);
      }
    } // UpdateListInfo

    private void CheckListInfo(DateTime removedStamp)
    {
      if (DateTime.Compare(CalcDateAdded(removedStamp), _listInfo.DateAdded) == 0
        || DateTime.Compare(removedStamp, _listInfo.LastAchievedTrophyTime) == 0)
      {
        var orderedTimestamps = _timestamps
          .Where(x => DateTime.Compare(x.Time, DateTime.MinValue) != 0)
          .OrderBy(p => p.Time);

        if (orderedTimestamps.Any())
        {
          _listInfo.LastAchievedTrophyTime = orderedTimestamps.Last().Time;
          _listInfo.DateAdded = CalcDateAdded(orderedTimestamps.First().Time);
          _listInfo.LastUpdated = orderedTimestamps.Last().Time;
        }
        else
        {
          _listInfo.LastAchievedTrophyTime = DateTime.MinValue;
          _listInfo.LastUpdated = DateTime.Now;
        }
      }
    } // CheckListInfo

    private void ResetListInfo()
    {
      int earnedCount = 0;
      _listInfo.LastAchievedTrophyTime = DateTime.MinValue;
      _listInfo.LastUpdated = DateTime.MinValue;
      _listInfo.DateAdded = DateTime.MaxValue;

      foreach (Timestamp timestamp in _timestamps)
      {
        if (timestamp.IsEarned)
        {
          DateTime earnedTime = timestamp.Time;
          if (earnedTime > _listInfo.LastAchievedTrophyTime)
          {
            _listInfo.LastAchievedTrophyTime = earnedTime;
            _listInfo.LastUpdated = earnedTime;
          }

          if (_listInfo.DateAdded > earnedTime)
          {
            _listInfo.DateAdded = CalcDateAdded(earnedTime);
          }

          earnedCount++;
        }
      } // foreach (Timestamp timestamp in _timestamps)

      _listInfo.AchievedCount = earnedCount;
    } // ResetListInfo

    private DateTime CalcDateAdded(DateTime timestamp) => timestamp.AddHours(-1);

    #endregion Data Ending
    #region File Saving

    private void SaveAccountInfo(BigEndianBinaryWriter writer)
    {
      TypeRecord accountIdRecord = _typeRecords[2];
      writer.BaseStream.Position = accountIdRecord.Offset + 32;
      writer.Write(_accountID.ToCharArray());
    } // SaveAccountInfo

    private void SaveListID(BigEndianBinaryWriter writer)
    {
      TypeRecord listIdRecord = _typeRecords[3];
      writer.BaseStream.Position = listIdRecord.Offset + 16;
      writer.Write(_listID.ToCharArray());
    } // SaveListID

    private void SaveTrophyInfo(BigEndianBinaryWriter writer)
    {
      TypeRecord trophyTypeRecord = _typeRecords[4];
      writer.BaseStream.Position = trophyTypeRecord.Offset;

      foreach (TrophyType type in _types)
      {
        writer.BaseStream.Position += 16;
        writer.Write(type.StructToBytes());
      }
    } // SaveTrophyInfo

    private void SaveListInfo(BigEndianBinaryWriter writer)
    {
      TypeRecord listInfoRecord = _typeRecords[5];
      writer.BaseStream.Position = listInfoRecord.Offset + 16;
      writer.Write(_listInfo.StructToBytes());
    } // SaveListInfo

    private void SaveTimestamps(BigEndianBinaryWriter writer)
    {
      TypeRecord timestampRecord = _typeRecords[6];
      writer.BaseStream.Position = timestampRecord.Offset;

      foreach (Timestamp time in _timestamps)
      {
        writer.BaseStream.Position += 16;
        writer.Write(time.StructToBytes());
      }
    } // SaveTimestamps

    private void SaveAchievedInfo(BigEndianBinaryWriter writer)
    {
      TypeRecord achievedRecord = _typeRecords[7];
      writer.BaseStream.Position = achievedRecord.Offset + 16;
      writer.Write(_achievedStats.StructToBytes());
    } // SaveAchievedInfo

    #endregion File Saving
    #endregion Private Methods
  } // TROPUSR
} // TrophyParser
