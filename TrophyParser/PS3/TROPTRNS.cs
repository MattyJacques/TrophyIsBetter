using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static TrophyParser.PS3.Structs;

namespace TrophyParser.PS3
{
  public class TROPTRNS
  {
    #region Const Members

    private const string TROPTRNS_FILE_NAME = "TROPTRNS.DAT";

    #endregion Const Members
    #region Private Members

    private Header _header;
    private Dictionary<int, TypeRecord> _typeRecords;
    private string _accountID;
    private string _trophyID;
    private int _earnedCount;
    private int _syncedCount;
    private TrophyInitTime _trophyInitTime;
    private List<EarnedInfo> _timestamps = new List<EarnedInfo>();
    private int _u1;

    #endregion Private Members
    #region Constructors

    public TROPTRNS(string path)
    {
      string filePath = Utility.File.GetFullPath(path, TROPTRNS_FILE_NAME);

      using (var fileStream = new FileStream(filePath, FileMode.Open))
      using (var TROPTRNSReader = new BigEndianBinaryReader(fileStream))
      {
        _header = DataParsing.ParseHeader(filePath, TROPTRNSReader);
        _typeRecords = DataParsing.ParseTypeRecords(_header, TROPTRNSReader);

        // Type 2
        TypeRecord accountIDRecord = _typeRecords[2];
        TROPTRNSReader.BaseStream.Position = accountIDRecord.Offset + 32; // Skip blank lines
        _accountID = Encoding.UTF8.GetString(TROPTRNSReader.ReadBytes(16));

        // Type 3
        TypeRecord trophy_id_Record = _typeRecords[3];
        TROPTRNSReader.BaseStream.Position = trophy_id_Record.Offset + 16; // Skip blank lines
        _trophyID = Encoding.UTF8.GetString(TROPTRNSReader.ReadBytes(16)).Trim('\0');
        _u1 = TROPTRNSReader.ReadInt32(); // always 00000090
        _earnedCount = TROPTRNSReader.ReadInt32();
        _syncedCount = TROPTRNSReader.ReadInt32();

        // Type 4
        ParseTrophyInfo(TROPTRNSReader);
      }
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public bool IsSynced => _timestamps.Count == 0;

    #endregion Public Properties
    #region Public Methods

    public EarnedInfo? this[int TrophyID]
    {
      get
      {
        EarnedInfo? ret = null;
        for (int i = 0; i < _timestamps.Count; i++)
        {
          if (_timestamps[i].TrophyID == TrophyID)
          {
            ret = _timestamps[i];
            break;
          }
        }

        return ret;
      }
    } // []

    public void PrintState()
    {
      Console.WriteLine("\n----- TROPTRNS Data -----");

      Console.WriteLine("Account ID: {0}", _accountID);
      Console.WriteLine("Trophy ID: {0}", _trophyID);

      Console.WriteLine("Earned Trophys: {0} Synced Trophys: {1} ", _earnedCount, _syncedCount);

      _header.Output();

      Console.WriteLine("\nType Records");
      foreach (KeyValuePair<int, TypeRecord> record in _typeRecords)
      {
        Console.WriteLine(record.Value);
      }

      Console.WriteLine("\nTimestamps");
      for (int i = 0; i < _timestamps.Count; i++)
      {
        Console.WriteLine("ID: {0}, Trophy ID: {1}, Type: {2}, Exists: {3}, Timestamp: {4}, Synced: {5}",
            _timestamps[i].ID, _timestamps[i].TrophyID,
            _timestamps[i].TrophyType, _timestamps[i].DoesExist, _timestamps[i].Time,
            _timestamps[i].IsSynced
           );
      }
    } // PrintState

    #endregion Public Methods
    #region Private Methods

    private void ParseTrophyInfo(BigEndianBinaryReader reader)
    {
      TypeRecord TrophyInfoRecord = _typeRecords[4];
      reader.BaseStream.Position = TrophyInfoRecord.Offset;
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

    #endregion  Private Members
  } // TROPTRNS
} // TrophyParser
