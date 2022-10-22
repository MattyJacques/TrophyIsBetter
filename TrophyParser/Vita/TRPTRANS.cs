using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static TrophyParser.Structs;

namespace TrophyParser.Vita
{
  public class TRPTRANS
  {
    #region Private Members

    private BinaryReader _reader;
    private BinaryWriter _writer;
    private const int _pointer = 0x377;
    public List<Timestamp> _timestamps = new List<Timestamp>();
    private string _path;

    private byte[] Block
    {
      get
      {
        var block = _reader.BaseStream.Position == _reader.BaseStream.Length ? null : _reader.ReadBytes(57);
        _reader.BaseStream.Position += 119;
        return block;
      }
    } // Block

    #endregion Private Members
    #region Constructors

    public TRPTRANS(string path, int count)
    {
      if (!path.EndsWith(@"\"))
        path += @"\";
      if (!File.Exists(Path.Combine(path, "TRPTRANS.DAT")))
        throw new Exception($"Cannot find {path}/TRPTRANS.DAT");

      _path = path;

      try
      {
        _reader = new BinaryReader(new FileStream(path + "TRPTRANS.DAT", FileMode.Open));
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
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public bool IsSynced => _timestamps.Count == 0;

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

    #endregion Public Methods
  } // TRPTRANS
} // TrophyParser.Vita
