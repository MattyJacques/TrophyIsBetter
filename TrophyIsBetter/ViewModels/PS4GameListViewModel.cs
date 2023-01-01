using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using TrophyIsBetter.Interfaces;
using TrophyIsBetter.Models;
using TrophyIsBetter.Views;
using TrophyParser.PS4;

namespace TrophyIsBetter.ViewModels
{
  internal class PS4GameListViewModel : ObservableObject, IPageViewModel
  {
    #region Private Members

    private readonly PS4GameList _model;
    private string _name = "PS4 Games";

    private ObservableCollection<GameViewModel> _gameCollection = new ObservableCollection<GameViewModel>();
    private ListCollectionView _gameCollectionView = null;

    private bool _hasGames = false;

    #endregion Private Members
    #region Constructors

    internal PS4GameListViewModel(PS4GameList model)
    {
      _model = model;

      ImportCommand = new RelayCommand(Import);
      EditGameCommand = new RelayCommand(EditGame);
      MoveGameCommand = new RelayCommand(MoveGame);
      RemoveGameCommand = new RelayCommand(RemoveGame);
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
    /// Import a single trophy folder or a directory containing multiple
    /// </summary>
    public RelayCommand ImportCommand { get; set; }

    /// <summary>
    /// Edit a games trophies
    /// </summary>
    public RelayCommand EditGameCommand { get; set; }

    /// <summary>
    /// Edit a games trophies
    /// </summary>
    public RelayCommand MoveGameCommand { get; set; }

    /// <summary>
    /// Remove a game from the game list
    /// </summary>
    public RelayCommand RemoveGameCommand { get; set; }

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
    /// Import fake trophy list
    /// </summary>
    private void Import()
    {
      PS4TrophyList trophyList = new PS4TrophyList();
      PS4Game game = new PS4Game(trophyList);
      GameViewModel gameViewModel = new GameViewModel(game, this);

      ObservableCollection<TrophyViewModel> trophyCollection = new ObservableCollection<TrophyViewModel>();
      for (int i = 0; i < 200; i++)
      {
        trophyCollection.Add(new TrophyViewModel(new Trophy()));
      }

      CopyFromWindow window = new CopyFromWindow
      {
        DataContext = new CopyFromViewModel(trophyCollection)
      };

      bool? result = window.ShowDialog();

      if (result == true)
      {
        GameCollection.Add(gameViewModel);

        for (int i = 0; i < trophyCollection.Count; i++)
        {
          TrophyViewModel trophy = trophyCollection[i];

          if (trophy.ShouldCopy && trophy.RemoteTimestamp.HasValue)
          {
            trophy.Timestamp = trophy.RemoteTimestamp.Value;
            trophy.RemoteTimestamp = null;
            trophy.ShouldCopy = false;
            trophy.Synced = false;

            trophyList.Trophies.Add(new TrophyParser.Models.Trophy(i, "no", 'B', 1, trophy.Name, "", 0));
            trophyList.Trophies.Last().Timestamp = new TrophyParser.Models.Timestamp { Time = trophy.Timestamp, IsSynced = false };
          }
        }

        var emptyList = trophyCollection.Where(x => !x.Timestamp.HasValue).ToList();
        for (int i = 0; i < emptyList.Count(); i++)
        {
          trophyCollection.Remove(emptyList[i]);
        }

        try
        {
          game.Name = trophyCollection[0].Game;
        } catch { }

        gameViewModel.TrophyCollection = trophyCollection;

        if (_model.ImportGame(trophyList))
        {
          NotifyGameChanges();
        }
        else
        {
          GameCollection.Remove(gameViewModel);
        }
      }
    } // Import

    /// <summary>
    /// Switch page to the single game view
    /// </summary>
    private void EditGame()
    {
      ((ApplicationViewModel)Application.Current.MainWindow.DataContext)
        .ChangePageCommand.Execute(SelectedGame);
    } // EditGame

    private void MoveGame()
    {
      DateTime earliestFinish = new DateTime(2021, 04, 27);

      List<TrophyViewModel> gameTrophies = SelectedGame.TrophyCollection.OrderBy(x => x.Timestamp).ToList();
      List<TrophyViewModel> allTrophies = new List<TrophyViewModel>();

      foreach (GameViewModel game in GameCollection)
      {
        if (game.Name != SelectedGame.Name)// && game.TrophyCollection.OrderBy(x => x.Timestamp).Last().Timestamp.Value < earliestFinish)
          game.TrophyCollection.ToList().ForEach(allTrophies.Add);
      }

      TimeSpan dateDifference = gameTrophies.Last().Timestamp.Value - earliestFinish;
      List<string> possibleDates = new List<string>();

      for (int i = 0; i <= dateDifference.Days; i++)
      {
        bool hasClash = false;

        foreach (TrophyViewModel trophy in gameTrophies)
        {
          int index = allTrophies.FindIndex(f => f.Timestamp.Value.ToShortDateString() == trophy.Timestamp.Value.AddDays(i * -1).ToShortDateString());

          if (index >= 0)
          {
            hasClash = true;
            break;
          }
        }

        if (!hasClash)
        {
          possibleDates.Add($"{gameTrophies.First().Timestamp.Value.AddDays(i * -1).ToShortDateString()} - {gameTrophies.Last().Timestamp.Value.AddDays(i * -1).ToShortDateString()}");
        }
      }

      MessageBox.Show(string.Join("\n", possibleDates));

    } // MoveGame

    /// <summary>
    /// Remove a single game from the game list
    /// </summary>
    private void RemoveGame()
    {
      if (CheckShouldRemove(SelectedGame.Name))
      {        
        if (!_model.RemoveGame(SelectedGame.Model))
        {
          MessageBox.Show($"Failed to remove {SelectedGame.Name}");
        }
        else
        {
          GameCollection.Remove(SelectedGame);
        }

        NotifyGameChanges();
      }
    } // RemoveGame

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
