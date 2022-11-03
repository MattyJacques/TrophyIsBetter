using System;
using System.Text;

namespace TrophyParser
{
  public class Structs
  {
    /// <summary>
    /// Holds the timestamp for a trophy
    /// </summary>
    public struct Timestamp
    {
      public bool IsEarned { get => Time.HasValue && Time.Value.CompareTo(DateTime.MinValue) != 0; }
      public DateTime? Time;
      public bool IsSynced;
      public byte Type;
      public byte Unknown;

      public override string ToString()
      {
        StringBuilder sb = new StringBuilder();
        sb.Append("Earned: ").AppendLine(IsEarned ? "YES" : "NO");
        sb.Append("Sync: ").AppendLine(IsSynced ? "YES" : "NO");
        sb.AppendLine(Time.HasValue ? Time.ToString() : "");
        return sb.ToString();
      } // ToString
    } // Timestamp
  } // Structs
} // TrophyParser
