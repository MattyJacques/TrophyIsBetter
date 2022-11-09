using System;

namespace TrophyIsBetter.Interfaces
{
  public interface ITrophyModel
  {
    int ID { get; }
    string Icon { get; }
    string Name { get; }
    string Description { get; }
    string Type { get; }
    string Group { get; }
    bool Hidden { get; }
    bool Achieved { get; set; }
    bool Synced { get; }
    DateTime? Timestamp { get; set; }
  } // ITrophyModel
} // TrophyIsBetter.Interfaces
