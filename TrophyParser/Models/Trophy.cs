using System.Text;
using static TrophyParser.Structs;

namespace TrophyParser.Models
{
  public class Trophy
  {
    public int ID;
    public string Hidden;
    public string Rank;
    public int Pid;
    public string Name;
    public string Detail;
    public int Gid;
    public Timestamp? Timestamp;

    public Trophy(int id, string hidden, string rank, int pid, string name, string detail, int gid)
    {
      ID = id;
      Hidden = hidden;
      Rank = rank;
      Pid = pid;
      Name = name;
      Detail = detail;
      Gid = gid;
      Timestamp = null;

    } // Constructor

    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();
      builder.Append('[').Append(ID).Append(',');
      builder.Append(Hidden).Append(',');
      builder.Append(Rank).Append(',');
      builder.Append(Pid).Append(',');
      builder.Append(Name).Append(',');
      builder.Append(Detail).Append(']');
      return builder.ToString();
    } // ToString
  } // Trophy
} // TrophyParser.Models
