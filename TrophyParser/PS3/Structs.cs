using System;
using System.Text;

namespace TrophyParser.PS3
{
  enum TrophySyncState
  {
    Synced = 0x100,
    NotSynced = 0x100000
  }
  internal enum TropType
  {
    Platinum = 1,
    Gold = 2,
    Silver = 3,
    Bronze = 4
  }

  internal enum TropGrade
  {
    Platinum = 180,
    Gold = 90,
    Silver = 30,
    Bronze = 15
  }

  internal class Structs
  {
    #region TROPUSR

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct TypeRecord
    {

      /// int
      private int _id;
      internal int ID
      {
        get
        {
          return _id.ChangeEndian();
        }
      }

      /// int
      private int _size;
      internal int Size
      {
        get
        {
          return _size.ChangeEndian();
        }
      }

      /// int
      internal int _unknown3;
      internal int Unknown3
      {
        get
        {
          return _unknown3.ChangeEndian();
        }
      }

      /// int
      private int _timesUsed;
      internal int TimesUsed
      {
        get
        {
          return _timesUsed.ChangeEndian();
        }
      }

      /// int
      internal long _offset;
      internal long Offset
      {
        get
        {
          return _offset.ChangeEndian();
        }
      }

      /// int
      internal long _unknown6;
      internal long Unknown6
      {
        get
        {
          return _unknown6.ChangeEndian();
        }
      }

      public override string ToString()
      {
        StringBuilder sb = new StringBuilder();
        sb.Append("{ID:").Append(ID).Append(", ");
        sb.Append("Size:").Append(Size).Append(", ");
        sb.Append("u3:").Append(Unknown3).Append(", ");
        sb.Append("UsageCount:").Append(TimesUsed).Append(", ");
        sb.Append("Offset:").Append(Offset).Append(", ");
        sb.Append("u6:").Append(Unknown6).Append('}');
        return sb.ToString();
      } // ToString
    } // TypeRecord

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct TrophyType
    { // Type of trophy in TROPUSR

      /// int
      private int _id;
      internal int ID
      {
        get
        {
          return _id.ChangeEndian();
        }
      }

      /// int
      private int _type;
      internal int Type
      {
        get
        {
          return _type.ChangeEndian();
        }
        set
        {
          _type = value.ChangeEndian();
        }
      }

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] unknown;

      /// byte[56]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 56, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding;

      public override string ToString()
      {
        StringBuilder sb = new StringBuilder();
        sb.Append("[ID:").Append(ID).Append(", ");
        sb.Append("Type:").Append(Type).Append(']');
        return sb.ToString();
      } // ToString
    } // TrophyType

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct ListInfo
    { // General data on trophy list

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding;

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      private byte[] _dateAdded;
      internal DateTime DateAdded
      {
        get
        {
          DateTime realDateTime = new DateTime(BitConverter.ToInt64(_dateAdded, 0).ChangeEndian() * 10);
          if (realDateTime.Ticks == 0)
          {
            return realDateTime;
          }
          else
          {
            return realDateTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
          }
        }
        set
        {
          if (value.Ticks == 0)
          {
            Array.Clear(_dateAdded, 0, 16);
          }
          else
          {
            long temp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
            Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _dateAdded, 0, 8);
            Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _dateAdded, 8, 8);
          }
        }
      } // DateAdded

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      private byte[] _lastUpdated;
      internal DateTime LastUpdated
      {
        get
        {
          DateTime realDateTime = new DateTime(BitConverter.ToInt64(_lastUpdated, 0).ChangeEndian() * 10);
          if (realDateTime.Ticks == 0)
          {
            return realDateTime;
          }
          else
          {
            return realDateTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
          }
        }
        set
        {
          if (value.Ticks == 0)
          {
            Array.Clear(_lastUpdated, 0, 16);
          }
          else
          {
            long temp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
            Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _lastUpdated, 0, 8);
            Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _lastUpdated, 8, 8);
          }
        }
      } // LastUpdated

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding2;

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      private byte[] _lastAchievedTrophyTime;
      internal DateTime LastAchievedTrophyTime
      {
        get
        {
          DateTime realDateTime = new DateTime(BitConverter.ToInt64(_lastAchievedTrophyTime, 0).ChangeEndian() * 10);
          if (realDateTime.Ticks == 0)
          {
            return realDateTime;
          }
          else
          {
            return realDateTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
          }
        }
        set
        {
          if (value.Ticks == 0)
          {
            Array.Clear(_lastAchievedTrophyTime, 0, 16);
          }
          else
          {
            long temp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
            Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _lastAchievedTrophyTime, 0, 8);
            Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _lastAchievedTrophyTime, 8, 8);
          }
        }
      } // ListLastGetTrophyTime

      /// byte[32]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding3;

      private int _achievedCount;
      internal int AchievedCount
      {
        get
        {
          return _achievedCount.ChangeEndian();
        }
        set
        {
          _achievedCount = value.ChangeEndian();
        }
      } // AchievedCount

      /// byte[12]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding4;

      /// uint[4]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I4)]
      internal uint[] AchievementRate;

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding5;

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] hash;

      /// byte[32]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding6;
    } // TrophyListInfo

    /// <summary>
    /// Timestamp for a trophy
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct Timestamp
    {

      /// int
      private int _id;
      internal int ID
      {
        get
        {
          return _id.ChangeEndian();
        }
        set
        {
          _id = value.ChangeEndian();
        }
      }

      /// byte[4]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      private byte[] _achieved;
      internal bool IsEarned
      {
        get
        {
          return (_achieved[3] != 0) ? true : false;
        }
        set
        {
          _achieved[3] = (byte)((value) ? 1 : 0);
        }
      }

      /// int
      internal int SyncState;

      internal bool IsSynced => (SyncState & (int)TrophySyncState.Synced) == (int)TrophySyncState.Synced;

      /// int
      internal int Unknown2;

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      private byte[] _achievedTime;
      internal DateTime Time
      {
        get
        {
          DateTime realDateTime = new DateTime(BitConverter.ToInt64(_achievedTime, 0).ChangeEndian() * 10);
          if (realDateTime.Ticks == 0)
          {
            return realDateTime;
          }
          else
          {
            return realDateTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
          }
        }
        set
        {
          if (value.Ticks == 0)
          {
            Array.Clear(_achievedTime, 0, 16);
          }
          else
          {
            long temp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
            Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _achievedTime, 0, 8);
            Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _achievedTime, 8, 8);
          }
        }
      } // Time

      /// byte[64]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding;

      public override string ToString()
      {
        StringBuilder sb = new StringBuilder();
        sb.Append("[").Append("SequenceNumber:").Append(ID).Append(", ");
        sb.Append("Earned:").Append(IsEarned).Append(", ");
        sb.Append("Timestamp:").Append(Time).Append("]");

        return sb.ToString();
      } // ToString
    } // Timestamp

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct AchievedStats
    {

      /// int
      private int _earnedCount;
      internal int EarnedCount
      {
        get
        {
          return _earnedCount.ChangeEndian();
        }
        set
        {
          _earnedCount = value.ChangeEndian();
        }
      }

      /// int
      private int _syncedCount;

      /// int
      internal int u3;

      /// int
      internal int u4;

      /// byte[8]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      private byte[] _lastSyncTime;
      internal DateTime ListSyncTime
      {
        get
        {
          DateTime realDateTime = new DateTime(BitConverter.ToInt64(_lastSyncTime, 0).ChangeEndian() * 10);
          if (realDateTime.Ticks == 0)
          {
            return realDateTime;
          }
          else
          {
            return realDateTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
          }
        }
        set
        {
          if (value.Ticks == 0)
          {
            Array.Clear(_lastSyncTime, 0, 8);
          }
          else
          {
            long temp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
            Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _lastSyncTime, 0, 8);
          }
        }
      } // ListSyncTime


      /// byte[8]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding;

      /// byte[48]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding2;
    } // AchievedStats

    #endregion
    #region TROPTRNS

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct TrophyInitTime
    {
      internal int u1;
      internal int u2;
      internal int u3;
      internal int u4;

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding;

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] _initTime;

      /// byte[112]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 112, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding2;
    } // TrophyInitTime

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct EarnedInfo
    {

      /// int
      private int _id;
      internal int ID
      {
        get
        {
          return _id.ChangeEndian();
        }
        set
        {
          _id = value.ChangeEndian();
        }
      }

      /// byte[4]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      private byte[] _doesExist;
      internal bool DoesExist
      {
        set
        {
          _doesExist[3] = (byte)((value) ? 2 : 0);
        }
        get
        {
          return (_doesExist[3] == 2) ? true : false;
        }
      }


      /// byte[4]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      private byte[] _isSynced;
      internal bool IsSynced
      {
        set
        {
          _isSynced[3] = (byte)((value) ? 1 : 0);
        }
        get
        {
          return (_isSynced[3] == 0) ? false : true;
        }
      }

      /// int
      internal int _unknownInt1;

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding;


      /// int
      private int _trophyID;
      internal int TrophyID
      {
        get
        {
          return _trophyID.ChangeEndian();
        }
        set
        {
          _trophyID = value.ChangeEndian();
        }
      }

      /// int
      private int _trophyType;
      internal int TrophyType
      {
        get
        {
          return _trophyType.ChangeEndian();
        }
        set
        {
          _trophyType = value.ChangeEndian();
        }
      }


      /// int
      internal int _unknownInt2;

      /// int
      internal int _unknownInt3;

      /// byte[16]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      private byte[] _getTime;
      internal DateTime Time
      {
        get
        {
          DateTime dt = new DateTime(BitConverter.ToInt64(_getTime, 0).ChangeEndian() * 10);
          return dt.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
        }
        set
        {
          if (value.Ticks == 0)
          {
            Array.Clear(_getTime, 0, 16);
          }
          else
          {
            long tmp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
            Array.Copy(BitConverter.GetBytes((tmp / 10).ChangeEndian()), 0, _getTime, 0, 8);
            Array.Copy(BitConverter.GetBytes((tmp / 10).ChangeEndian()), 0, _getTime, 8, 8);
          }
        }
      }

      /// byte[96]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 96, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding2;

      public override string ToString()
      {
        StringBuilder sb = new StringBuilder();
        sb.Append("TrophyID: ").Append(TrophyID).Append(", ");
        sb.Append("Time: ").Append(Time).Append(", ");
        sb.Append("Earned: ").Append(DoesExist).Append(", ");
        sb.Append("Synced: ").Append(IsSynced);
        return sb.ToString();
      }

      internal EarnedInfo(int id, int TrophyType, DateTime dt)
      {
        _id = 0;
        _doesExist = new byte[4];
        _getTime = new byte[16];
        _isSynced = new byte[4];
        _trophyID = id.ChangeEndian();
        _trophyType = TrophyType.ChangeEndian();
        _unknownInt1 = 0;
        _unknownInt2 = 0x00100000;
        _unknownInt3 = 0;
        padding = new byte[16];
        padding2 = new byte[96];
        Time = dt;
        DoesExist = true;
      }
    } // EarnedInfo

    #endregion
    #region General

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct Header
    { // Header data for TROPUSR and TROPTRNS

      internal ulong Magic;

      internal int _unknownCount;
      internal int UnknownCount
      {
        get
        {
          return _unknownCount.ChangeEndian();
        }
      }

      /// byte[36]
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
      internal byte[] padding;

      internal void Output()
      {
        Console.WriteLine("\nHeader");
        Console.WriteLine("UnknownCounter: {0}", UnknownCount);
        Console.WriteLine("Padding:{0}", padding.ToHexString());
      } // Output
    } // Header

    #endregion
  }
}
