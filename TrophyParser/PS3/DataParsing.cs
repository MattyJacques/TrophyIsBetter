using System.Collections.Generic;
using System.Runtime.InteropServices;
using static TrophyParser.PS3.Structs;

namespace TrophyParser.PS3
{
  internal class DataParsing
  {
    internal static Header ParseHeader(string path, BigEndianBinaryReader reader)
    {
      Header header = reader.ReadBytes(Marshal.SizeOf(typeof(Header))).ToStruct<Header>();
      if (header.Magic != 0x0000000100ad548f81)
        throw new InvalidFileException(path);

      return header;
    } // Header

    internal static Dictionary<int, TypeRecord> ParseTypeRecords(Header header, BigEndianBinaryReader reader)
    {
      Dictionary<int, TypeRecord> typeRecords = new Dictionary<int, TypeRecord>();
      for (int i = 0; i < header.UnknownCount; i++)
      {
        TypeRecord TypeRecordTmp = reader.ReadBytes(Marshal.SizeOf(typeof(TypeRecord))).ToStruct<TypeRecord>();
        typeRecords.Add(TypeRecordTmp.ID, TypeRecordTmp);
      }

      return typeRecords;
    } // ParseTypeRecords
  }
}
