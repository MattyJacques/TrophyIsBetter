using System;
using TrophyIsBetter.Interfaces;

namespace TrophyIsBetter.Models
{
  public class Trophy : ITrophyModel
  {
    #region Public Properties

    public int ID { get; set; }
    public string Icon { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string Group { get; set; }
    public bool Hidden { get; set; }
    public bool Achieved { get; set; }
    public bool Synced { get; set; }
    public DateTime? Timestamp { get; set; }
    public DateTime? RemoteTimestamp { get; set; }
    public bool ShouldCopy { get; set; }

    #endregion Public Properties
  } // Trophy
} // TrophyIsBetter.Models
