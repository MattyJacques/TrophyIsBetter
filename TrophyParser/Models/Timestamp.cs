using System.Text;
using System;

namespace TrophyParser.Models
{
  public class Timestamp
  {
    public int TrophyID = 0;
    public bool IsEarned { get => Time.HasValue && Time.Value.CompareTo(DateTime.MinValue) != 0; }
    public DateTime? Time;
    public bool IsSynced;
    internal byte Type;
    internal byte Unknown;

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine($"Trophy ID: {TrophyID} ");
      sb.AppendLine($"Earned: {IsEarned} ");
      sb.AppendLine($"Synced: {IsSynced} ");
      sb.AppendLine(Time.HasValue ? Time.ToString() : "");
      return sb.ToString();
    } // ToString
  } // Timestamp
} // TrophyParser.Models
