using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static TrophyParser.Structs;
using static TrophyParser.TROPUSR;

namespace TrophyParser
{
  public class TROPUSR
  {
    #region Const Members

    private const string TROPUSR_FILE_NAME = "TROPUSR.DAT";

    #endregion
    #region Public Members

    public string AccountID;
    public string ListID;
    public int TrophyCount;
    public ListInfo ListInfo;
    public List<TrophyType> Types = new();
    public List<Timestamp> Timestamps = new();

    #endregion
    #region Private Members

    private Header _header;
    private Dictionary<int, TypeRecord> _typeRecords;
    private AchievedStats _achievedStats;
    private uint[] _completionRate = new uint[4];
    byte[] _unknownHash;

    #endregion
    #region Public Constructors

    public TROPUSR(string path)
    {
      string filePath = GetFullPath(path);

      using (var fileStream = new FileStream(filePath, FileMode.Open))
      using (var TROPUSRReader = new BigEndianBinaryReader(fileStream))
      {
        _header = new(filePath, TROPUSRReader);

        _typeRecords = new Dictionary<int, TypeRecord>();
        for (int i = 0; i < _header.UnknownCount; i++)
        {
          TypeRecord TypeRecordTmp = TROPUSRReader.ReadBytes(Marshal.SizeOf(typeof(TypeRecord))).ToStruct<TypeRecord>();
          _typeRecords.Add(TypeRecordTmp.ID, TypeRecordTmp);
        }

        do
        {
          // 1 unknow 2 account_id 3 trophy_id and hash(?) 4 trophy info
          // 
          int type = TROPUSRReader.ReadInt32();
          int blocksize = TROPUSRReader.ReadInt32();
          int id = TROPUSRReader.ReadInt32(); // if have more than same type block, it will be used
          int unknown = TROPUSRReader.ReadInt32();
          byte[] blockdata = TROPUSRReader.ReadBytes(blocksize);
          switch (type)
          {
            case 1: // unknow
              break;
            case 2:
              AccountID = Encoding.UTF8.GetString(blockdata, 16, 16);
              break;
            case 3:
              ListID = Encoding.UTF8.GetString(blockdata, 0, 16).Trim('\0');
              short u1 = BitConverter.ToInt16(blockdata, 16).ChangeEndian();
              short u2 = BitConverter.ToInt16(blockdata, 18).ChangeEndian();
              short u3 = BitConverter.ToInt16(blockdata, 20).ChangeEndian();
              short u4 = BitConverter.ToInt16(blockdata, 22).ChangeEndian();
              TrophyCount = BitConverter.ToInt32(blockdata, 24).ChangeEndian();
              int u5 = BitConverter.ToInt32(blockdata, 28).ChangeEndian();
              _completionRate[0] = BitConverter.ToUInt32(blockdata, 64);
              _completionRate[1] = BitConverter.ToUInt32(blockdata, 68);
              _completionRate[2] = BitConverter.ToUInt32(blockdata, 72);
              _completionRate[3] = BitConverter.ToUInt32(blockdata, 76);
              break;
            case 4:
              Types.Add(blockdata.ToStruct<TrophyType>());
              break;
            case 5:
              ListInfo = blockdata.ToStruct<ListInfo>();
              break;
            case 6:
              Timestamps.Add(blockdata.ToStruct<Timestamp>());
              break;
            case 7:
              _achievedStats = blockdata.ToStruct<AchievedStats>();
              break;
            case 8: // hash
              _unknownHash = blockdata.SubArray(0, 20);
              break;
            case 9: // 通常寫著白金獎盃的一些數字，不明
                    // Console.WriteLine("Unsupported block type. (Type{0})", type);
              break;
            case 10: // i think it just a padding
              break;
          }

        } while (TROPUSRReader.BaseStream.Position < TROPUSRReader.BaseStream.Length);

        ListInfo.LastUpdated = DateTime.Now;
      }
    }

    #endregion
    #region Public Methods

    public void PrintState()
    { // Print all data within TROPUSR

      Console.WriteLine("\n----- TROPUSR Data -----");
      Console.WriteLine("Account ID: {0}", AccountID);
      _header.Output();

      Console.WriteLine("\nType Records");
      foreach (KeyValuePair<int, TypeRecord> record in _typeRecords)
      {
        Console.WriteLine(record.Value);
      }

      Console.WriteLine("\nList Info");
      Console.WriteLine("Game Added To Account On: {0}", ListInfo.DateAdded);
      Console.WriteLine("Last Trophy Earned On: {0}", ListInfo.LastAchievedTrophyTime);
      Console.WriteLine("List Last Updated: {0}", ListInfo.LastUpdated);
      Console.WriteLine("Trophies Earned: {0}", ListInfo.AchievedCount);
      Console.WriteLine("Completion Rate: {0}", ListInfo.AchievementRate[0]);

      Console.WriteLine("\nAchieved Data");
      for (int i = 0; i < Types.Count; i++)
      {
        Console.WriteLine("ID: {0}, Type: {1}, Earned: {2}, Timestamp: {3}", Types[i].ID,
            Types[i].Type, Timestamps[i].IsEarned, Timestamps[i].Time);
      }
    }

    #endregion
    #region Private Methods

    private static string GetFullPath(string directory)
    {
      if (directory == null || directory.Trim() == string.Empty)
        throw new Exception("Path cannot be null!");

      string filePath = Path.Combine(directory, TROPUSR_FILE_NAME);
      if (!File.Exists(filePath))
        throw new FileNotFoundException("File not found", TROPUSR_FILE_NAME);

      return filePath;
    } // GetFullPath

    #endregion
  } // TROPUSR
} // TrophyParser
