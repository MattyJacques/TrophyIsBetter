using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace TrophyIsBetter.ViewModels
{
  internal class TrophyListViewModel : ObservableObject
  {
    #region Private Members

    private ObservableCollection<TrophyViewModel> _trophyCollection;
    private ListCollectionView _trophyCollectionView;

    #endregion Private Members
    #region Constructors

    internal TrophyListViewModel(ObservableCollection<TrophyViewModel> trophies)
    {
      TrophyCollection = trophies;

      _trophyCollectionView = new ListCollectionView(TrophyCollection)
      {
        Filter = t => ((TrophyViewModel)t).Timestamp != null,
        IsLiveFiltering = true,
        LiveFilteringProperties = { nameof(TrophyViewModel.Timestamp) },
        SortDescriptions = { new SortDescription("Timestamp", ListSortDirection.Descending) },
        IsLiveSorting = true,
        LiveSortingProperties = { nameof(TrophyViewModel.Timestamp) }
      };
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

    #endregion Internal Properties
  } // TrophyListViewModel
} // TrophyIsBetter.ViewModels
