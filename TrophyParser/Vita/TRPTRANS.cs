using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static TrophyParser.Structs;

namespace TrophyParser.Vita
{
  public class TRPTRANS
  {
    #region Const Members

    private const string TROPTRANS_FILE_NAME = "TRPTRANS.DAT";

    #endregion Const Members
    #region Private Members

    private BinaryReader _reader;
    private BinaryWriter _writer;
    private const int _pointer = 0x377;
    public List<Timestamp> _timestamps = new List<Timestamp>();
    private string _path;

    #endregion Private Members
    #region Constructors

    public TRPTRANS(string path, int count)
    {
      _path = Utility.File.GetFullPath(path, TROPTRANS_FILE_NAME);

      ReadFile(count);
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public bool IsSynced => _timestamps.Count == 0;

    #endregion Public Properties
    #region Private Properties

    private byte[] Block
    {
      get
      {
        var block = _reader.BaseStream.Position == _reader.BaseStream.Length ? null : _reader.ReadBytes(57);
        _reader.BaseStream.Position += 119;
        return block;
      }
    } // Block

    #endregion Private Properties
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

    public void UnlockTrophy(int id, string rank, DateTime time)
    {
      Timestamp timestamp = _timestamps[id];
      timestamp.Type = GetTrophyType(rank);
      timestamp.Time = time;
      timestamp.Unknown = 0x50;

      _timestamps[id] = timestamp;

      Debug.WriteLine($"Unlocked trophy {id} in TRPTRANS");
    } // UnlockTrophy

    public void Save()
    {
      _writer = new BinaryWriter(new FileStream(_path, FileMode.Open));
      _writer.BaseStream.Position = _pointer;

      foreach (Timestamp trophy in _timestamps)
      {

        _writer.Write((byte)(trophy.Earned ? 0x02 : 0x00));

        byte[] time = trophy.Time.HasValue ?
          BitConverter.GetBytes(trophy.Time.Value.Ticks / 10) : BitConverter.GetBytes((long)0);
        Array.Reverse(time);
        _writer.BaseStream.Position += 31;
        _writer.Write(trophy.Type);

        _writer.BaseStream.Position += 2;
        _writer.Write(trophy.Unknown);

        _writer.BaseStream.Position += 5;
        _writer.Write(time);

        _writer.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        _writer.BaseStream.Position += 119;
      }
      _writer.Flush();
      _writer.Close();
    } // Save

    #endregion Public Methods
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
          var time = block.Skip(41).Take(8).ToArray();
          Array.Reverse(time);
          ulong t = BitConverter.ToUInt64(time, 0);
          _timestamps.Add(new Timestamp
          {
            Time = new DateTime().AddMilliseconds(t / 1000),
            Unknown = block[35]
          });
        }
        _reader.Close();
      }
      catch (IOException)
      {
        throw new Exception("Fail in TRPTRANS.DAT");
      }
    } // ReadFile

    private byte GetTrophyType(string type)
    {
      byte result;

      switch (type[0])
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
