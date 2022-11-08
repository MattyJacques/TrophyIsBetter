using System.Text;
using System;

namespace TrophyParser.Models
{
  public class Timestamp
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
} // TrophyParser.Models
