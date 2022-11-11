using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.RegularExpressions;

namespace TrophyIsBetter.ViewModels
{
  internal class CopyFromViewModel : ObservableObject
  {
    #region Const Members

    private static readonly Regex DATE_EARNED_REGEX =
      new Regex("<td class=\"date_earned\">\\s+<span class=\"sort\">\\d+</span>");

    #endregion Const Members
    #region Private Members

    private string _copyUrl = "";
    private ObservableCollection<TrophyViewModel> _trophyCollection;

    #endregion Private Members
    #region Constructors

    public CopyFromViewModel(ObservableCollection<TrophyViewModel> trophies)
    {
      DownloadTimestampsCommand = new RelayCommand(DownloadTimestamps);

      TrophyCollection = trophies;
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public RelayCommand DownloadTimestampsCommand { get; set; }

    public string CopyUrl { get => _copyUrl; set => SetProperty(ref _copyUrl, value); }

    public ObservableCollection<TrophyViewModel> TrophyCollection
    {
      get => _trophyCollection;
      private set => SetProperty(ref _trophyCollection, value);
    }

    #endregion Public Properties
    #region Private Methods

    private void DownloadTimestamps()
    {
      WebClient client = new WebClient();
      client.Headers.Add("User-Agent: Other");
      MatchCollection timestamps = DATE_EARNED_REGEX.Matches(client.DownloadString(CopyUrl));

      for (int i = 0; i < timestamps.Count; i++)
      {
        long.TryParse(Regex.Match(timestamps[i].Value, "\\d+").ToString(),out var seconds);

        if (seconds > 0)
        {
          DateTime timestamp =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);

          TrophyCollection[i].RemoteTimestamp = timestamp;
          TrophyCollection[i].ShouldCopy = true;
        }
      }
    } // DownloadTimestamps

    #endregion Private Methods
  } // CopyFromViewModel
} // TrophyIsBetter.ViewModels
