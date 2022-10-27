using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TrophyIsBetter.Models;

namespace TrophyIsBetter.ViewModels
{
  public class TrophyViewModel : ObservableObject
  {
    #region Private Members

    private Trophy _model;

    #endregion Private Members
    #region Constructors

    public TrophyViewModel(Trophy model)
    {
      _model = model;
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public Trophy Model { get => _model; }
    public string Icon { get => _model.Icon; }
    public string Name { get => _model.Name; }
    public string Description { get => _model.Description; }
    public string Type { get => _model.Type; }
    public string Group { get => _model.Group; }
    public bool Hidden { get => _model.Hidden; }
    public bool Achieved { get => _model.Achieved; }
    public bool Synced { get => _model.Synced; }
    public DateTime Timestamp
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
