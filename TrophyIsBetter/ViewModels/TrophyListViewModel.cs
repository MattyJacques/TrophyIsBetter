using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace TrophyIsBetter.ViewModels
{
  internal class TrophyListViewModel : ObservableObject
  {
    #region Private Members

    private ObservableCollection<TrophyViewModel> _trophyCollection =
      new ObservableCollection<TrophyViewModel>();
    private ObservableCollection<GameViewModel> _gameCollection;
    private ListCollectionView _trophyCollectionView;

    #endregion Private Members
    #region Constructors

    internal TrophyListViewModel(ObservableCollection<GameViewModel> games)
    {
      GameCollection = games;

      _trophyCollectionView = new ListCollectionView(TrophyCollection)
      {
        Filter = t => ((TrophyViewModel)t).Timestamp != null,
        IsLiveFiltering = true,
        LiveFilteringProperties = { nameof(TrophyViewModel.Timestamp) },
        SortDescriptions = { new SortDescription("Timestamp", ListSortDirection.Descending) },
        IsLiveSorting = true,
        LiveSortingProperties = { nameof(TrophyViewModel.Timestamp) }
      };

      UpdateTrophies();
    } // Constructor

    #endregion Constructors
    #region Public Properties

    public ListCollectionView TrophyCollectionView { get => _trophyCollectionView; }

    #endregion Public Properties
    #region Internal Properties

    internal ObservableCollection<TrophyViewModel> TrophyCollection
    {
      get => _trophyCollection;
      set => SetProperty(ref _trophyCollection, value);
    }

    internal ObservableCollection<GameViewModel> GameCollection
    {
      get => _gameCollection;
      set => SetProperty(ref _gameCollection, value);
    }

    #endregion Internal Properties
    #region Private Methods

    private void UpdateTrophies()
    {
      TrophyCollection.Clear();
      foreach (GameViewModel game in GameCollection)
      {
        game.TrophyCollection.ToList().ForEach(TrophyCollection.Add);
      }
    } // UpdateTrophies

    private void TrophyChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      TrophyCollectionView.Refresh();
      OnPropertyChanged(nameof(TrophyCollectionView));
    }

    #endregion
  } // TrophyListViewModel
} // TrophyIsBetter.ViewModels
