using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TrophyIsBetter.Interfaces;

namespace TrophyIsBetter.ViewModels
{
  public class TrophyViewModel : ObservableObject
  {
    #region Private Members

    private readonly ITrophyModel _model;

    #endregion Private Members
    #region Constructors

    public TrophyViewModel(ITrophyModel model)
    {
      _model = model;
    } // Constructor

    #endregion Constructors
    #region Public Properties

    internal ITrophyModel Model { get => _model; }
    public string Icon { get => _model.Icon; }
    public string Name { get => _model.Name; }
    public string Description { get => _model.Description; }
    public string Type { get => _model.Type; }
    public string Group { get => _model.Group; }
    public bool Hidden { get => _model.Hidden; }
    public bool Achieved
    {
      get => _model.Achieved;
      set
      {
        _model.Achieved = value;
        OnPropertyChanged();
      }
    }
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

    #endregion Public Properties
  } // TrophyViewModel
} // TrophyIsBetter.ViewModels
