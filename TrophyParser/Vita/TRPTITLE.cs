﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static TrophyParser.Structs;

namespace TrophyParser.Vita
{
  internal class TRPTITLE
  {
    #region Private Members

    private BinaryReader _reader;
    private BinaryWriter _writer;
    private long pointer = -1;
    public List<Timestamp> _timestamps = new List<Timestamp>();
    private string _path;

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

    #endregion Private Members
    #region Constructors

    public TRPTITLE(string path)
    {
      if (!path.EndsWith(@"\"))
        path += @"\";
      if (!File.Exists(Path.Combine(path, "TRPTITLE.DAT")))
        throw new Exception($"Cannot find {path}/TRPTITLE.DAT");

      _path = path;

      try
      {
        _reader = new BinaryReader(new FileStream(path + "TRPTITLE.DAT", FileMode.Open));
        var block = Block;
        do
        {
          var time = block.Skip(9).Take(8).ToArray();
          Array.Reverse(time);
          ulong t = BitConverter.ToUInt64(time, 0);
          _timestamps.Add(new Timestamp
          {
            Time = new DateTime().AddMilliseconds(t / 1000),
            Unknown = block[3]

          });
          block = Block;
        } while (block.Any());
        _reader.Close();
      }
      catch (IOException)
      {
        throw new InvalidFileException("Fail in TRPTITLE.DAT");
      }
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public Timestamp this[int index] => _timestamps[index];

    public int EarnedCount
    {
      get
      {
        int count = 0;

        foreach (Timestamp timestamp in _timestamps)
        {
          if (timestamp.Earned)
          {
            count++;
          }
        }

        return count;
      }
    } // EarnedCount

    public DateTime? LastTimestamp
    {
      get
      {
        DateTime? result = new DateTime(2008, 1, 1);
        foreach (Timestamp timestamp in _timestamps)
        {
          if (timestamp.Earned && timestamp.Time > result)
          {
            result = timestamp.Time;
          }
        }

        return result;
      }
    } // LastTimestamp

    public DateTime? LastSyncedTimestamp
    {
      get
      {
        DateTime? result = new DateTime(2008, 1, 1);
        foreach (Timestamp timestamp in _timestamps)
        {
          if (timestamp.Synced && timestamp.Time > result)
          {
            result = timestamp.Time;
          }
        }

        return result;
      }
    } // LastSyncedTimestamp

    #endregion Public Properties
    #region Public Methods

    public void PrintState()
    {
      Console.WriteLine("\n----- TRPTITLE Data -----");

      Console.WriteLine("\nTimestamps");
      for (int i = 0; i < _timestamps.Count; i++)
      {
        Console.WriteLine(_timestamps[i]);
      }
    } // PrintState

    #endregion
  } // TRPTITLE
} // TrophyParser.Vita
