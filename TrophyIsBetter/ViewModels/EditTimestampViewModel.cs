using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace TrophyIsBetter.ViewModels
{
  public class EditTimestampViewModel : ObservableObject, IDataErrorInfo
  {
    #region Private Members

    private DateTime _timestamp;

    #endregion Private Members
    #region Constructors

    public EditTimestampViewModel(DateTime startTimestamp)
    {
      Timestamp = startTimestamp;
    } // Constructor

    #endregion Constructors
    #region Public Properties

    /// <summary>
    /// Confirm the edited trophy timestamp
    /// </summary>
    public ICommand OnConfirmCommand { get; set; }

    public DateTime Timestamp
    {
      get => _timestamp;
      set
      {
        SetProperty(ref _timestamp, value);
      }
    }

    public DateTime DateTimestamp
    {
      get => Timestamp;
      set
      {
        Timestamp = value.AddHours(Timestamp.Hour)
                         .AddMinutes(Timestamp.Minute)
                         .AddSeconds(Timestamp.Second);
      }
    }

    public string Error => throw new NotImplementedException();
    public string this[string propertyName] => IsValid(propertyName);

    #endregion Public Properties
    #region Private Methods

    private string IsValid(string propertyName)
    {
      string result = "";

      switch (propertyName)
      {
        case "Timestamp":
        case "DateTimestamp":
          result = ValidateTimestamp();
          break;
      }

      return result;
    } // IsValid

    private string ValidateTimestamp()
    {
      string result = "";

      if (DateTime.Compare(Timestamp, DateTime.UtcNow.AddHours(14)) > 0)
        result = "Too far in future, max 14 hours";

      return result;
    } // ValidateTimestamp

      #endregion Private Methods
    } // EditTimestampViewModel
} // TrophyIsBetter.ViewModels
