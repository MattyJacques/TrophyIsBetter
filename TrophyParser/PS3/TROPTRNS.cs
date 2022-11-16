using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static TrophyParser.PS3.Structs;

namespace TrophyParser.PS3
{
  internal class TROPTRNS
  {
    #region Const Members

    private const string TROPTRNS_FILE_NAME = "TROPTRNS.DAT";

    #endregion Const Members
    #region Private Members

    private string _filePath;
    private Header _header;
    private Dictionary<int, TypeRecord> _typeRecords;
    private string _accountID;
    private string _listID;
    private int _earnedCount;
    private int _syncedCount;
    private TrophyInitTime _trophyInitTime;
    private List<EarnedInfo> _timestamps = new List<EarnedInfo>();
    private int _u1;

    #endregion Private Members
    #region Constructors

    internal TROPTRNS(string path)
    {
      _filePath = Utility.File.GetFullPath(path, TROPTRNS_FILE_NAME);

      BigEndianBinaryReader reader =
        new BigEndianBinaryReader(new FileStream(_filePath, FileMode.Open));

      _header = DataParsing.ParseHeader(_filePath, reader);
      _typeRecords = DataParsing.ParseTypeRecords(_header, reader);

      ParseAccountInfo(reader);
      ParseListInfo(reader);
      ParseTrophyInfo(reader);
    } // Constructor

    #endregion Constructors
    #region Internal Properties

    internal bool IsSynced => _timestamps.Count == 0;

    #endregion Internal Properties
    #region Internal Methods

    internal void AddTrophy(int id, int trophyType, DateTime unlockTime)
    {
      EarnedInfo timestamp = new EarnedInfo(id, trophyType, unlockTime);

      InsertTimestamp(timestamp);
      _earnedCount++;

      Debug.WriteLine($"Added trophy {id} in TROPTRNS");
    } // AddTrophy

    internal void ChangeTimestamp(int id, DateTime time)
    {
      int oldIndex = _timestamps.FindIndex(x => x.TrophyID == id);

      if (oldIndex == -1)
        throw new TrophyNotFound(id);

      EarnedInfo timestamp = _timestamps[oldIndex];

      if (timestamp.IsSynced)
        throw new AlreadySyncedException(timestamp.TrophyID);

      _timestamps.RemoveAt(oldIndex);

      InsertTimestamp(timestamp);
    } // ChangeTimestamp

    internal void RemoveTrophy(int id)
    {
      EarnedInfo timestamp = _timestamps.Find(x => x.TrophyID == id);

      if (timestamp.IsSynced)
        throw new AlreadySyncedException(timestamp.TrophyID);

      _timestamps.Remove(timestamp);
      _earnedCount--;

      Debug.WriteLine($"Removed trophy {id} from TROPTRNS");
    } // RemoveTrophy

    internal void Save()
    {
      BigEndianBinaryWriter writer =
        new BigEndianBinaryWriter(new FileStream(_filePath, FileMode.Open));

      writer.Write(_header.StructToBytes());
      SaveAccountInfo(writer);
      SaveListInfo(writer);
      SaveTrophyInfo(writer);

      Debug.WriteLine($"Saved {_listID} TROPTRNS");
    } // Save

    internal void PrintState()
    {
      Console.WriteLine("\n----- TROPTRNS Data -----");

      Console.WriteLine("Account ID: {0}", _accountID);
      Console.WriteLine("Trophy ID: {0}", _listID);

      Console.WriteLine("Earned Trophys: {0} Synced Trophys: {1} ",
        _earnedCount, _syncedCount);

      _header.Output();

      Console.WriteLine("\nType Records");
      foreach (KeyValuePair<int, TypeRecord> record in _typeRecords)
      {
        Console.WriteLine(record.Value);
      }

      Console.WriteLine("\nTimestamps");
      for (int i = 0; i < _timestamps.Count; i++)
      {
        Console.WriteLine("ID: {0}, Trophy ID: {1}, Type: {2}, Exists: {3}, " +
          "Timestamp: {4}, Synced: {5}",
          _timestamps[i].ID, _timestamps[i].TrophyID,
          _timestamps[i].TrophyType, _timestamps[i].DoesExist, _timestamps[i].Time,
          _timestamps[i].IsSynced
        );
      }
    } // PrintState

    #endregion Internal Methods
    #region Private Methods
    #region File Parsing

    private void ParseAccountInfo(BigEndianBinaryReader reader)
    {
      TypeRecord accountIDRecord = _typeRecords[2];
      reader.BaseStream.Position = accountIDRecord.Offset + 32; // Skip blank lines
      _accountID = Encoding.UTF8.GetString(reader.ReadBytes(16));
    } // ParseAccountInfo

    private void ParseListInfo(BigEndianBinaryReader reader)
    {
      TypeRecord listInfoRecord = _typeRecords[3];
      reader.BaseStream.Position = listInfoRecord.Offset + 16; // Skip blank lines
      _listID = Encoding.UTF8.GetString(reader.ReadBytes(16)).Trim('\0');
      _u1 = reader.ReadInt32(); // always 00000090
      _earnedCount = reader.ReadInt32();
      _syncedCount = reader.ReadInt32();
    } // ParseListInfo

    private void ParseTrophyInfo(BigEndianBinaryReader reader)
    {
      TypeRecord trophyInfoRecord = _typeRecords[4];
      reader.BaseStream.Position = trophyInfoRecord.Offset;
      int type = reader.ReadInt32();
      int blocksize = reader.ReadInt32();
      int sequenceNumber = reader.ReadInt32(); // if have more than same type block, it will be used
      int unknown = reader.ReadInt32();
      byte[] blockdata = reader.ReadBytes(blocksize);
      _trophyInitTime = blockdata.ToStruct<TrophyInitTime>();


      for (int i = 0; i < _earnedCount - 1; i++)
      {
        reader.BaseStream.Position += 16;
        EarnedInfo ti = reader.ReadBytes(blocksize).ToStruct<EarnedInfo>();
        _timestamps.Add(ti);
      }
    } // ParseTrophyInfo

    #endregion File Parsing
    #region File Saving

    private void SaveAccountInfo(BigEndianBinaryWriter writer)
    {
      TypeRecord accountInfoRecord = _typeRecords[2];
      writer.BaseStream.Position = accountInfoRecord.Offset + 32;
      writer.Write(_accountID.ToCharArray());
    } // SaveAccountInfo

    private void SaveListInfo(BigEndianBinaryWriter writer)
    {
      TypeRecord listInfoRecord = _typeRecords[3];
      writer.BaseStream.Position = listInfoRecord.Offset + 16;
      writer.Write(_listID.ToCharArray());
      writer.BaseStream.Position = listInfoRecord.Offset + 32;
      writer.Write(_u1);
      writer.Write(_timestamps.Count + 1);
      _syncedCount = 0;
      for (int i = 0; i < _timestamps.Count; i++)
      {
        if (_timestamps[i].IsSynced)
        {
          _syncedCount++;
        }
      }

      writer.Write(_syncedCount + 1);
    } // SaveListInfo

    private void SaveTrophyInfo(BigEndianBinaryWriter writer)
    {
      TypeRecord trophyTypeRecord = _typeRecords[4];
      writer.BaseStream.Position = trophyTypeRecord.Offset;
      writer.BaseStream.Position += 16;
      writer.Write(_trophyInitTime.StructToBytes());

      for (int i = 0; i < _timestamps.Count; i++)
      {
        writer.BaseStream.Position += 16;
        EarnedInfo info = _timestamps[i];
        info.ID = i + 1;
        info._unknownInt3 = 0;

        //_timestamps[i] = info;
        writer.Write(_timestamps[i].StructToBytes());
      }

      byte[] emptyStruct = new byte[Marshal.SizeOf(typeof(EarnedInfo))];
      Array.Clear(emptyStruct, 0, emptyStruct.Length);
      EarnedInfo emptyTrophyInfo = emptyStruct.ToStruct<EarnedInfo>();
      for (int i = _timestamps.Count; i < trophyTypeRecord.Size; i++)
      {
        writer.BaseStream.Position += 16;
        emptyTrophyInfo.ID = i + 1;
        writer.Write(emptyTrophyInfo.StructToBytes());
      }
    } // SaveTrophyInfo

    #endregion File Saving

    private void InsertTimestamp(EarnedInfo timestamp)
    {
      int insertPoint;

      for (insertPoint = 0; insertPoint < _timestamps.Count; insertPoint++)
      {
        if (DateTime.Compare(_timestamps[insertPoint].Time, timestamp.Time) > 0)
        {
          if (_timestamps[insertPoint].IsSynced)
          {
            throw new SyncTimeException(_timestamps[insertPoint].Time);
          }

          break;
        }
      }

      _timestamps.Insert(insertPoint, timestamp);
    } // InsertTimestamp()

    #endregion  Private Members
  } // TROPTRNS
} // TrophyParser
