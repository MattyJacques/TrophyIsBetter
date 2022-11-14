using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TrophyIsBetter.Interfaces;

namespace TrophyIsBetter.ViewModels
{
  internal class TrophyViewModel : ObservableObject
  {
    #region Private Members

    private readonly ITrophyModel _model;

    #endregion Private Members
    #region Constructors

    internal TrophyViewModel(ITrophyModel model)
    {
      _model = model;
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public string Icon { get => _model.Icon; }
    public string Name { get => _model.Name; }
    public string Description { get => _model.Description; }
    public char Type { get => _model.Type; }
    public string Group { get => _model.Group; }
    public bool Hidden { get => _model.Hidden; }
    public bool Synced { get => _model.Synced; }
    public DateTime? Timestamp
    {
      get => _model.Timestamp;
      set
      {
        _model.Timestamp = value;
        OnPropertyChanged();
      }
    }
    public DateTime? RemoteTimestamp
    {
      get => _model.RemoteTimestamp;
      set
      {
        _model.RemoteTimestamp = value;
        OnPropertyChanged();
      }
    }
    public bool ShouldCopy
    {
      get => _model.ShouldCopy;
      set
      {
        _model.ShouldCopy = value;
        OnPropertyChanged();
      }
    }

    #endregion Public Properties
    #region Internal Properties

    internal ITrophyModel Model { get => _model; }
    internal bool Achieved { get => _model.Achieved; set => _model.Achieved = value; }

    #endregion Internal Properties
  } // TrophyViewModel
} // TrophyIsBetter.ViewModels
