using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Input;

namespace TrophyIsBetter.ViewModels
{
  public class EditTimestampViewModel : ObservableObject
  {
    #region Private Members

    private DateTime _timestamp = DateTime.Now;

    #endregion Private Members
    #region Public Properties

    /// <summary>
    /// Confirm the edited trophy timestamp
    /// </summary>
    public ICommand OnConfirmCommand { get; set; }

    public DateTime Timestamp { get => _timestamp; set => SetProperty(ref _timestamp, value); }

    #endregion Public Properties
  } // EditTimestampViewModel
} // TrophyIsBetter.ViewModels
