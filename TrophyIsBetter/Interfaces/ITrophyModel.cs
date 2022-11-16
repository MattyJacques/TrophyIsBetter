using System;

namespace TrophyIsBetter.Interfaces
{
  internal interface ITrophyModel
  {
    int ID { get; }
    string Icon { get; }
    string Name { get; }
    string Description { get; }
    char Type { get; }
    string Group { get; }
    bool Hidden { get; }
    bool Achieved { get; set; }
    bool Synced { get; set; }
    DateTime? Timestamp { get; set; }
    DateTime? RemoteTimestamp { get; set; }
    bool ShouldCopy { get; set; }
  } // ITrophyModel
} // TrophyIsBetter.Interfaces
