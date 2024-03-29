﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TrophyParser.Models;

namespace TrophyParser.Vita
{
  internal class TRPTITLE
  {
    #region Const Members

    private const string TRPTITLE_FILE_NAME = "TRPTITLE.DAT";

    #endregion Const Members
    #region Private Members

    private BinaryReader _reader;
    private BinaryWriter _writer;
    private long pointer = -1;
    internal List<Timestamp> _timestamps = new List<Timestamp>();
    private string _path;

    #endregion Private Members
    #region Constructors

    internal TRPTITLE(string path)
    {
      _path = Utility.File.GetFullPath(path, TRPTITLE_FILE_NAME);

      ReadFile();
    } // Constructor

    #endregion Constructors
    #region Internal Properties

    internal Timestamp this[int index] => _timestamps[index];

    internal bool IsSynced => _timestamps.Where(x => !x.IsSynced).Count() == 0;

    internal int EarnedCount
    {
      get
      {
        int count = 0;

        foreach (Timestamp timestamp in _timestamps)
        {
          if (timestamp.IsEarned)
          {
            count++;
          }
        }

        return count;
      }
    } // EarnedCount

    internal DateTime? LastTimestamp
    {
      get
      {
        DateTime? result = DateTime.MinValue;
        foreach (Timestamp timestamp in _timestamps)
        {
          if (timestamp.IsEarned && timestamp.Time > result)
          {
            result = timestamp.Time;
          }
        }

        return result == DateTime.MinValue ? null : result;
      }
    } // LastTimestamp

    internal DateTime? LastSyncedTimestamp
    {
      get
      {
        DateTime? result = new DateTime(2008, 1, 1);
        foreach (Timestamp timestamp in _timestamps)
        {
          if (timestamp.IsSynced && timestamp.Time > result)
          {
            result = timestamp.Time;
          }
        }

        return result;
      }
    } // LastSyncedTimestamp

    #endregion Internal Properties
    #region Private Properties

    private byte[] Block
    {
      get
      {
        if (pointer == -1)
        {
          _reader.BaseStream.Position = 0;
          var pos = _reader.ReadBytes((int)_reader.BaseStream.Length).Search(
              "50000000000000000000000000000000".ToBytes(), 2
          );
          if (pos == -1) throw new Exception("Can't find TrophyBloack");
          pointer = pos + 16;
          _reader.BaseStream.Position = pointer;
          return _reader.ReadBytes(25);

        }
        _reader.BaseStream.Position += 71;
        return _reader.BaseStream.Position == _reader.BaseStream.Length ? null : _reader.ReadBytes(25);
      }
      set
      {
        _writer.Write(value);
        _writer.BaseStream.Position += 71;
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

    internal void UnlockTrophy(int id, DateTime time)
    {
      _timestamps[id].Time = time;
      _timestamps[id].Unknown = 32;
      _timestamps[id].IsSynced = false;

      Debug.WriteLine($"Unlocked trophy {id} in TRPTITLE");
    } // UnlockTrophy

    internal void ChangeTimestamp(int id, DateTime time) => _timestamps[id].Time = time;

    internal void LockTrophy(int id)
    {
      if (_timestamps[id].IsSynced)
        throw new Exception("Can't delete sync trophies");

      _timestamps[id].Time = null;
      _timestamps[id].Type = 0;

      Debug.WriteLine($"Locked trophy {id} in TRPTITLE");
    } // LockTrophy

    internal void Save()
    {
      _writer = new BinaryWriter(new FileStream(_path, FileMode.Open));
      _writer.BaseStream.Position = pointer;

      foreach (var trophy in _timestamps)
      {
        var data = new List<byte>
        {
          (byte)(trophy.IsEarned ? 0x01 : 0x00)
        };

        data.AddRange(new byte[] { 0, 0, trophy.Unknown, 0, 0, 0, 0, 0 });

        var time = trophy.Time.HasValue ?
          BitConverter.GetBytes(trophy.Time.Value.Ticks / 10) : BitConverter.GetBytes((long)0);
        Array.Reverse(time);
        data.AddRange(time);
        data.AddRange(time);

        Block = data.ToArray();
      }

      _writer.Flush();
      _writer.Close();
    } // Save

    #endregion Internal Methods
    #region Private Methods

    private void ReadFile()
    {
      try
      {
        _reader = new BinaryReader(new FileStream(_path, FileMode.Open));
        var block = Block;
        do
        {
          _timestamps.Add(GetTimestamp(block));
          block = Block;
        } while (block.Any());
        _reader.Close();
      }
      catch (IOException)
      {
        throw new InvalidFileException("Fail in TRPTITLE.DAT");
      }
    } // ReadFile

    private Timestamp GetTimestamp(byte[] block)
    {
      Timestamp timestamp = new Timestamp();

      byte[] time = block.Skip(9).Take(8).ToArray();
      Array.Reverse(time);
      ulong t = BitConverter.ToUInt64(time, 0);
      timestamp.Time = new DateTime().AddMilliseconds(t / 1000);
      timestamp.Unknown = block[3];

      return timestamp;
    } // GetTimestamp

    #endregion Private Methods

  } // TRPTITLE
} // TrophyParser.Vita
