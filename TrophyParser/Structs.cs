using System;
using System.Text;

namespace TrophyParser
{
    public class Structs
    {
        /// <summary>
        /// Holds data on a individual trophy from a trophy set
        /// </summary>
        public struct Trophy
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

        /// <summary>
        /// Holds the timestamp for a trophy
        /// </summary>
        public struct Timestamp
        {
            public bool Earned { get => Time.HasValue && Time.Value.CompareTo(DateTime.MinValue) != 0; }
            public DateTime? Time;
            public bool Synced;
            public byte Unknown;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Earned: ").AppendLine(Earned ? "YES" : "NO");
                sb.Append("Sync: ").AppendLine(Synced ? "YES" : "NO");
                sb.AppendLine(Time.HasValue ? Time.ToString() : "");
                return sb.ToString();
            } // ToString
        } // Timestamp
    } // Structs
} // TrophyParser
