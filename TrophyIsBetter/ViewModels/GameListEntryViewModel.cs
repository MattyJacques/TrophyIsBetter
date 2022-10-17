using System;
using TrophyIsBetter.Models;

namespace TrophyIsBetter.ViewModels
{
  public class GameListEntryViewModel
  {
    #region Private Members

    private readonly GameListEntry _model;

    #endregion Private Members
    #region Constructors

    public GameListEntryViewModel(GameListEntry entry)
    {
      _model = entry;
    } // GameListEntryViewModel

    #endregion Constructors
    #region Public Properties

    public string Icon { get => _model.Icon; }
    public string Name { get => _model.Name; }
    public string NpCommunicationID { get => _model.NpCommID; }
    public string Platform { get => _model.Platform; }
    public bool HasPlatinum { get => _model.HasPlatinum; }
    public bool IsSynced { get => _model.IsSynced; }
    public string Progress { get => _model.Progress; }
    public DateTime? LastTimestamp { get => _model.LastTimestamp; }
    public DateTime? SyncTime { get => _model.SyncTime; }
    public string Path { get => _model.Path; }

    #endregion

  } // GameListEntryViewModel
} // TrophyIsBetter.ViewModels
