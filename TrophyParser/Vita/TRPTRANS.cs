using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TrophyParser.Models;

namespace TrophyParser.Vita
{
  internal class TRPTRANS
  {
    #region Const Members

    private const string TROPTRANS_FILE_NAME = "TRPTRANS.DAT";

    #endregion Const Members
    #region Private Members

    private BinaryReader _reader;
    private const int _pointer = 0x377;
    private List<Timestamp> _timestamps = new List<Timestamp>();
    private string _path;
    private int _trophyCount = 0;
    private int _earnedCount = 0;

    #endregion Private Members
    #region Constructors

    internal TRPTRANS(string path, int count)
    {
      _path = Utility.File.GetFullPath(path, TROPTRANS_FILE_NAME);

      ReadFile(count);
    } // Constructor

    #endregion Constructors
    #region Private Properties

    private byte[] Block
    {
      get
      {
        var block =
          _reader.BaseStream.Position == _reader.BaseStream.Length ? null : _reader.ReadBytes(57);
        _reader.BaseStream.Position += 119;
        return block;
      }
    } // Block

    #endregion Private Properties
    #region Internal Methods

    internal void PrintState()
    {
      Console.WriteLine("\n----- TRPTITLE Data -----");

      Console.WriteLine("\nTimestamps");
      for (int i = 0; i < _timestamps.Count; i++)
      {
        Console.WriteLine(_timestamps[i]);
      }
    } // PrintState

    internal void UnlockTrophy(int id, char rank, DateTime time)
    {
      Timestamp timestamp = new Timestamp
      {
        TrophyID = id,
        Time = time,
        Unknown = 32,
        Type = GetTrophyType(rank),
      };

      _earnedCount++;

      InsertTimestamp(timestamp);

      Debug.WriteLine($"Unlocked trophy {id} in TRPTRANS");
    } // UnlockTrophy

    internal void ChangeTimestamp(int id, DateTime time)
    {
      int oldIndex = _timestamps.FindIndex(x => x.TrophyID == id);

      if (oldIndex == -1)
        throw new TrophyNotFound(id);

      Timestamp timestamp = _timestamps[oldIndex];

      if (timestamp.IsSynced)
        throw new AlreadySyncedException(timestamp.TrophyID);

      _timestamps.RemoveAt(oldIndex);
      timestamp.Time = time;

      InsertTimestamp(timestamp);

      Debug.WriteLine($"Changed trophy {id} timestamp in TRPTRNS");
    } // ChangeTimestamp

    internal void LockTrophy(int id)
    {
      Timestamp timestamp = _timestamps.Find(x => x.TrophyID == id);

      if (timestamp.IsSynced)
        throw new AlreadySyncedException(timestamp.TrophyID);

      _timestamps.Remove(timestamp);
      _earnedCount--;

      Debug.WriteLine($"Locked trophy {id} in TRPTRANS");
    } // LockTrophy

    internal void Save()
    {
      BinaryWriter writer = new BinaryWriter(new FileStream(_path, FileMode.Open));
      writer.BaseStream.Position = _pointer;

      foreach (Timestamp timestamp in _timestamps)
      {
        SaveTimestamp(writer, timestamp);
      }

      Timestamp emptyTimestamp = new Timestamp();
      for (int i = 0; i < _trophyCount - _earnedCount; i++)
      {
        SaveTimestamp(writer, emptyTimestamp);
      }
      
      writer.Flush();
      writer.Close();
    } // Save

    #endregion Internal Methods
    #region Private Methods

    private void ReadFile(int count)
    {
      try
      {
        _reader = new BinaryReader(new FileStream(_path, FileMode.Open));
        _reader.BaseStream.Position = _pointer;
        for (int i = 0; i < count; ++i)
        {
          var block = Block;

          if (block[0] != 0)
          {
            var time = block.Skip(41).Take(8).ToArray();
            Array.Reverse(time);
            ulong t = BitConverter.ToUInt64(time, 0);
            _timestamps.Add(new Timestamp
            {
              Time = new DateTime().AddMilliseconds(t / 1000),
              Unknown = block[35],
              Type = block[32],
              TrophyID = block[28]
            });

            _earnedCount++;
          }

          _trophyCount++;
        }
        _reader.Close();
      }
      catch (IOException)
      {
        throw new Exception("Fail in TRPTRANS.DAT");
      }
    } // ReadFile

    private void SaveTimestamp(BinaryWriter writer, Timestamp timestamp)
    {
      writer.Write((byte)(timestamp.IsEarned ? 0x02 : 0x00));

      writer.BaseStream.Position += 27;
      writer.Write(timestamp.TrophyID);

      writer.Write(timestamp.Type);

      writer.BaseStream.Position += 2;
      writer.Write(timestamp.Unknown);

      byte[] time = timestamp.Time.HasValue ?
        BitConverter.GetBytes(timestamp.Time.Value.Ticks / 10) : BitConverter.GetBytes((long)0);
      Array.Reverse(time);
      writer.BaseStream.Position += 5;
      writer.Write(time);
      writer.Write(time);
      writer.BaseStream.Position += 119;
    } // SaveTimestamp

    private void InsertTimestamp(Timestamp timestamp)
    {
      int insertPoint;

      for (insertPoint = 0; insertPoint < _timestamps.Count; insertPoint++)
      {
        if (DateTime.Compare(_timestamps[insertPoint].Time.Value, timestamp.Time.Value) > 0)
        {
          if (_timestamps[insertPoint].IsSynced)
          {
            throw new SyncTimeException(_timestamps[insertPoint].Time.Value);
          }

          break;
        }
      }

      _timestamps.Insert(insertPoint, timestamp);
    } // InsertTimestamp

    private byte GetTrophyType(char type)
    {
      byte result;

      switch (type)
      {
        case 'P':
          result = 0x01;
          break;
        case 'G':
          result = 0x02;
          break;
        case 'S':
          result = 0x03;
          break;
        case 'B':
          result = 0x04;
          break;
        default:
          result = 0x00;
          break;
      }

      return result;
    } // GetTrophyType

    #endregion Private Methods
  } // TRPTRANS
} // TrophyParser.Vita
