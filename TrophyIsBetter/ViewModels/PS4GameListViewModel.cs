using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using TrophyIsBetter.Interfaces;
using TrophyIsBetter.Views;

namespace TrophyIsBetter.ViewModels
{
  internal class PS4GameListViewModel : ObservableObject, IPageViewModel
  {
    #region Private Members

    private readonly IGameListModel _model;
    private string _name = "PS4 Games";

    private ObservableCollection<GameViewModel> _gameCollection = new ObservableCollection<GameViewModel>();
    private ListCollectionView _gameCollectionView = null;
    private readonly SynchronizationContext _uiContext = SynchronizationContext.Current;

    private bool _hasGames = false;

    #endregion Private Members
    #region Constructors

    internal PS4GameListViewModel(IGameListModel model)
    {
      _model = model;

      EditGameCommand = new RelayCommand(EditGame);
      TrophyListCommand = new RelayCommand(OpenTrophyList);

      _gameCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(GameCollection);
      _gameCollectionView.SortDescriptions.Add(new SortDescription(nameof(GameViewModel.Name),
                                                                   ListSortDirection.Ascending));

      GameCollectionView.CurrentChanged += OnSelectedGameChanged;

      LoadGames();
    } // GameListViewModel

    #endregion Constructors
    #region Public Properties

    /// <summary>
    /// Edit a games trophies
    /// </summary>
    public RelayCommand EditGameCommand { get; set; }

    /// <summary>
    /// List all trophies chronologically
    /// </summary>
    public RelayCommand TrophyListCommand { get; set; }

    /// <summary>
    /// Get/Set the collection view, used for sorting
    /// </summary>
    public ListCollectionView GameCollectionView
    {
      get
      {
        // Horrific hack to fix sorting not being applied when view switches
        // It keeps losing the SortDescriptions
        _gameCollectionView.SortDescriptions.Add(new SortDescription(nameof(GameViewModel.Name),
                                                                     ListSortDirection.Ascending));

        return _gameCollectionView;
      }
    }

    /// <summary>
    /// Is a game selected
    /// </summary>
    public bool HasSelected => SelectedGame != null;

    /// <summary>
    /// Is a game selected
    /// </summary>
    public bool HasGames { get => _hasGames; set => SetProperty(ref _hasGames, value); }

    /// <summary>
    /// The name of the view model
    /// </summary>
    public string Name { get => _name; set => _name = value; }

    /// <summary>
    /// String showing current trophy progress
    /// </summary>
    public string ProgressString =>
      $"{TotalEarnedCount}/{TotalCount} ({TotalEarnedExp}/{TotalExp})";

    /// <summary>
    /// Total earned trophies
    /// </summary>
    public int TotalEarnedCount => GameCollection.Sum(game => game.EarnedCount);

    /// <summary>
    /// Total amount of trophies in lists
    /// </summary>
    public int TotalCount => GameCollection.Sum(game => game.TrophyCount);

    #endregion Public Properties
    #region Internal Properties

    /// <summary>
    /// Get/Set the list of games
    /// </summary>
    internal ObservableCollection<GameViewModel> GameCollection
    {
      get => _gameCollection;
      private set => SetProperty(ref _gameCollection, value);
    }

    /// <summary>
    /// Get the selected game from the list
    /// </summary>
    internal GameViewModel SelectedGame => (GameViewModel)_gameCollectionView.CurrentItem;

    /// <summary>
    /// Total amount of earned exp
    /// </summary>
    internal int TotalEarnedExp => GameCollection.Sum(game => game.EarnedExp);

    /// <summary>
    /// Total possible exp
    /// </summary>
    internal int TotalExp => GameCollection.Sum(game => game.TotalExp);

    #endregion Internal Properties
    #region Private Methods

    /// <summary>
    /// Notify that the selected game has changed
    /// </summary>
    private void OnSelectedGameChanged(object sender, EventArgs e)
    {
      OnPropertyChanged(nameof(HasSelected));
    } // OnSelectedGameChanged

    /// <summary>
    /// Load the folders contained in the game directory
    /// </summary>
    private void LoadGames()
    {
      List<IGameModel> games = _model.LoadGames();

      if (games != null)
      {
        GameCollection.Clear();

        foreach (IGameModel entry in games)
        {
          GameCollection.Add(new GameViewModel(entry, this));
        }

        NotifyGameChanges();
      }
    } // LoadGames

    /// <summary>
    /// Switch page to the single game view
    /// </summary>
    private void EditGame()
    {
      ((ApplicationViewModel)Application.Current.MainWindow.DataContext)
        .ChangePageCommand.Execute(SelectedGame);
    } // EditGame

    /// <summary>
    /// List all trophies chronologically
    /// </summary>
    private void OpenTrophyList()
    {
      ObservableCollection<TrophyViewModel> trophies =
        new ObservableCollection<TrophyViewModel>();

      foreach (GameViewModel game in GameCollection)
      {
        game.TrophyCollection.ToList().ForEach(trophies.Add);
      }

      TrophyListWindow window = new TrophyListWindow()
      {
        DataContext = new TrophyListViewModel(trophies)
      };

      window.Show();
    } // OpenTrophyList

    private bool CheckShouldRemove(string gameName)
      => MessageBox.Show($"Are you sure you want to delete {gameName}?", "Delete?",
                         MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;

    private void NotifyGameChanges()
    {
      HasGames = GameCollection.Count > 0;
      OnPropertyChanged(nameof(ProgressString));
    } // NotifyChanges

    #endregion Private Methods
  } // PS4GameListViewModel
} // TrophyIsBetter.ViewModels
