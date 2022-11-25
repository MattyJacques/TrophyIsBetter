using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Data;
using TrophyIsBetter.Views;

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
    private ListCollectionView _trophyCollectionView;

    #endregion Private Members
    #region Constructors

    internal CopyFromViewModel(ObservableCollection<TrophyViewModel> trophies)
    {
      GetTimestampsCommand = new RelayCommand(GetTimestamps);
      UpdateTimestampsCommand = new RelayCommand(UpdateTimestamps);

      TrophyCollection = trophies;
      _trophyCollectionView =
        (ListCollectionView)CollectionViewSource.GetDefaultView(TrophyCollection);

      TrophyCollectionView.SortDescriptions.Add(
        new SortDescription(nameof(TrophyViewModel.RemoteTimestamp), ListSortDirection.Ascending));
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public RelayCommand GetTimestampsCommand { get; set; }

    public RelayCommand UpdateTimestampsCommand { get; set; }

    public string CopyUrl { get => _copyUrl; set => SetProperty(ref _copyUrl, value); }

    public ListCollectionView TrophyCollectionView { get => _trophyCollectionView; }

    #endregion Public Properties
    #region Internal Properties

    internal ObservableCollection<TrophyViewModel> TrophyCollection
    {
      get => _trophyCollection;
      private set => SetProperty(ref _trophyCollection, value);
    }

    internal TrophyViewModel SelectedTrophy
    {
      get => (TrophyViewModel)TrophyCollectionView.CurrentItem;
    }

    #endregion Internal Properties
    #region Private Methods
    #region Initial Timestamp Downloads

    private void GetTimestamps()
    {
      MatchCollection timestamps = DownloadTimestamps();
      double offset = GetOffset(GetFirstTimestamp(timestamps));
      
      CalcTimestamps(offset, timestamps);

      TrophyCollectionView.Refresh();
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

      return CalcOffset(((EditTimestampViewModel)window.DataContext).Timestamp, firstRemote);
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

    #endregion Initial Timestamp Downloads
    #region Edit Timestamps

    private void UpdateTimestamps()
    {
      if (SelectedTrophy.RemoteTimestamp.HasValue)
      {
        EditTimestampWindow window = new EditTimestampWindow("Edit Timestamp")
        {
          DataContext = new EditTimestampViewModel(SelectedTrophy.RemoteTimestamp.Value)
        };

        bool? result = window.ShowDialog();

        if (result == true)
        {
          DateTime timestamp = ((EditTimestampViewModel)window.DataContext).Timestamp;
          DateTime changedTimestamp = SelectedTrophy.RemoteTimestamp.Value;

          var futureTrophies =
            TrophyCollection.Where(x => x.RemoteTimestamp != null
                                     && x.RemoteTimestamp >= changedTimestamp);
          double offset = CalcOffset(((EditTimestampViewModel)window.DataContext).Timestamp,
                                     SelectedTrophy.RemoteTimestamp.Value);

          CalcNewTimestamps(futureTrophies, offset);
        }
      }
    } // UpdateTimestamps

    private void CalcNewTimestamps(IEnumerable<TrophyViewModel> futureTrophies, double offset)
    {
      foreach (TrophyViewModel trophy in futureTrophies)
      {
        trophy.RemoteTimestamp = trophy.RemoteTimestamp.Value.AddSeconds(offset);
      }
    } // CalcNewTimestamps

    #endregion Edit Timestamps

    private double CalcOffset(DateTime originalTimestamp, DateTime newTimestamp)
      => (originalTimestamp - newTimestamp).TotalSeconds;

    #endregion Private Methods
  } // CopyFromViewModel
} // TrophyIsBetter.ViewModels
