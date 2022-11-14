using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.RegularExpressions;
using TrophyIsBetter.Views;
using TrophyParser.Models;

namespace TrophyIsBetter.ViewModels
{
  internal class CopyFromViewModel : ObservableObject
  {
    #region Const Members

    private static readonly DateTime MIN_UNIX_TIMESTAMP =
      new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly Regex DATE_EARNED_REGEX =
      new Regex("<td class=\"date_earned\">\\s+<span class=\"sort\">\\d+</span>");

    #endregion Const Members
    #region Private Members

    private string _copyUrl = "";
    private ObservableCollection<TrophyViewModel> _trophyCollection;

    #endregion Private Members
    #region Constructors

    internal CopyFromViewModel(ObservableCollection<TrophyViewModel> trophies)
    {
      GetTimestampsCommand = new RelayCommand(GetTimestamps);

      TrophyCollection = trophies;
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public RelayCommand GetTimestampsCommand { get; set; }

    public string CopyUrl { get => _copyUrl; set => SetProperty(ref _copyUrl, value); }

    public ObservableCollection<TrophyViewModel> TrophyCollection
    {
      get => _trophyCollection;
      private set => SetProperty(ref _trophyCollection, value);
    }

    #endregion Public Properties
    #region Private Methods

    private void GetTimestamps()
    {
      MatchCollection timestamps = DownloadTimestamps();
      double offset = GetOffset(GetFirstTimestamp(timestamps));
      
      CalcTimestamps(offset, timestamps);
    } // GetTimestamps

    private MatchCollection DownloadTimestamps()
    {
      WebClient client = new WebClient();
      client.Headers.Add("User-Agent: Other");
      return DATE_EARNED_REGEX.Matches(client.DownloadString(CopyUrl));
    } // DownloadTimestamps

    private DateTime GetFirstTimestamp(MatchCollection matches)
    {
      long lowestSeconds = long.MaxValue;

      foreach (Match match in matches)
      {
        long seconds = ParseSecondsFromMatch(match);

        if (seconds > 0 && seconds < lowestSeconds)
          lowestSeconds = seconds;
      }

      return MIN_UNIX_TIMESTAMP.AddSeconds(lowestSeconds);
    } // GetFirstTimestamp

    private double GetOffset(DateTime firstRemote)
    {
      EditTimestampWindow window = new EditTimestampWindow("Starting Timestamp")
      {
        DataContext = new EditTimestampViewModel(firstRemote)
      };
      window.ShowDialog();

      return (((EditTimestampViewModel)window.DataContext).Timestamp - firstRemote).TotalSeconds;
    } // GetStartFromTimestamp

    private void CalcTimestamps(double offset, MatchCollection timestamps)
    {
      for (int i = 0; i < timestamps.Count; i++)
      {
        long seconds = ParseSecondsFromMatch(timestamps[i]);

        if (seconds > 0)
        {
          DateTime timestamp = MIN_UNIX_TIMESTAMP.AddSeconds(seconds + offset);

          TrophyCollection[i].RemoteTimestamp = timestamp;
          TrophyCollection[i].ShouldCopy = true;
        }
      }
    } // CalcTimestamps

    private long ParseSecondsFromMatch(Match match)
    {
      long.TryParse(Regex.Match(match.Value, "\\d+").ToString(), out var seconds);
      return seconds;
    } // ParseMatch

    #endregion Private Methods
  } // CopyFromViewModel
} // TrophyIsBetter.ViewModels
