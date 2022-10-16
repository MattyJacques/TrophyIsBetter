using System;

namespace TrophyIsBetter.Models
{
  internal class Trophy
  {
    public string Icon { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string Group { get; set; }
    public bool Hidden { get; set; }
    public bool Achieved { get; set; }
    public bool Synced { get; set; }
    public DateTime Timestamp { get; set; }
  }
}
